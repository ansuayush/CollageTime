using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExecViewHrk.WebUI.Models;
using System.Data.Entity.Validation;
using ExecViewHrk.Models;
using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Helpers;
using ExecViewHrk.Domain.Interface;
using ExecViewHrk.EfAdmin;
namespace ExecViewHrk.WebUI.Controllers
{
    public class ClientConfigurationController : Controller
    {
        private IClientConfigurationRepository _ClientConfigRepository;

        public ClientConfigurationController(IClientConfigurationRepository ClientConfigRepository)
        {
            _ClientConfigRepository = ClientConfigRepository;
        }

        public ActionResult ClientConfigurationList() {
            return View();
        }
        public ActionResult ClientConfigurationList_Read([DataSourceRequest]DataSourceRequest request)
        {
            int getSelectedClientID = Convert.ToInt32(User.Identity.GetSelectedClientID());
            int getClientAdminID = Convert.ToInt32(User.Identity.GetClientAdminEmployerID());
            var clientConfigurationList = _ClientConfigRepository.getClientConfigurationList(getSelectedClientID);
            return Json(clientConfigurationList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ClientConfigurationAddEdit(int ClientConfigId)
        {
            ClientConfigurationVM clientConfigurationVM = new ClientConfigurationVM();

            if (ClientConfigId == 0)
            {
                clientConfigurationVM.ClientConfigId = 0;
                clientConfigurationVM.ConfigurationValue = 0;
            }
            else
            {
                clientConfigurationVM = _ClientConfigRepository.getClientConfigurationDetails(ClientConfigId);
              //  clientConfigurationVM.FiscalYearList = getYearList();
                clientConfigurationVM.FiscalMonthList = getMonthList();
            }
            return View(clientConfigurationVM);
        }

        public ActionResult ClientConfigurationSaveAjax(ClientConfigurationVM clientConfigurationVM)
        {
            string _message = "";
            AdminDbContext adminDbContext = new AdminDbContext();
            if (ModelState.IsValid)
            {
                try
                {
                    clientConfigurationVM = _ClientConfigRepository.clientConfigurationSave(clientConfigurationVM);
                    return Json(new { clientConfigurationVM, succeed = true}, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    IEnumerable<DbEntityValidationResult> errors = adminDbContext.GetValidationErrors();
                    if (errors.Count() == 0)
                    {
                        _message += err.InnerException.Message;
                        if (_message.Contains("Cannot insert duplicate key"))
                        {
                            return Json(new { Message = CustomErrorMessages.ERROR_DUPLICATE_RECORD, success = false }, JsonRequestBehavior.AllowGet);
                        }
                    }

                }
            }
            else
            {
                var modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);
                foreach (var item in modelStateErrors)
                {
                    if (_message != "") _message += "<br />";
                    _message += item.ErrorMessage;
                }
                return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { clientConfigurationVM, succeed = true }, JsonRequestBehavior.AllowGet);
        }

        public List<DropDownModel> getYearList()
        {

            return Enumerable.Range(2011, 9).Select(x => new DropDownModel
            {
                keyvalue = x.ToString(),
                keydescription = x.ToString(),
            }).ToList();
        }
        public List<DropDownModel> getMonthList()
        {

            return Enumerable.Range(1, 12).Select(x => new DropDownModel
            {
                keyvalue = x.ToString(),
                keydescription = System.Globalization.DateTimeFormatInfo.CurrentInfo.GetMonthName(x)
            }).ToList();
        }
    }
}