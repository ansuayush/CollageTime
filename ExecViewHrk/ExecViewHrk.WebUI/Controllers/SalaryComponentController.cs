using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Infrastructure;
using ExecViewHrk.WebUI.Models;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using ExecViewHrk.WebUI.Helpers;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Domain.Repositories;
using ExecViewHrk.Models;

namespace ExecViewHrk.WebUI.Controllers

{
    public class SalaryComponentController : Controller
    {
        ISalaryComponent _salaryComponent;
        public SalaryComponentController(SalaryComponenRepository salaryComponent)
        {
            _salaryComponent = salaryComponent;
        }
        public ActionResult SalaryComponentList(int? EmpId)
        {
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            string connString = User.Identity.GetClientConnectionString();
            SalaryComponentViewModel salaryComponentViewModel = new SalaryComponentViewModel();
            var EmpList = (from p in clientDbContext.Persons
                           join E in clientDbContext.Employees on p.PersonId equals E.PersonId
                           where E.TerminationDate == null
                           select new DropDownModel
                           {
                               keyvalue = E.EmployeeId.ToString(),
                               keydescription = p.Firstname + " " + p.Lastname
                           }).OrderBy(m => m.keydescription).ToList();
            salaryComponentViewModel.EmployeeList = EmpList.ToList();
            salaryComponentViewModel.SelectedEmployeeID = EmpId == null ? Convert.ToInt32(salaryComponentViewModel.EmployeeList.ElementAtOrDefault(0).keyvalue) : EmpId;
            return View("SalaryComponentList", salaryComponentViewModel);
        }
        public ActionResult SalaryComponentList_Read([DataSourceRequest]DataSourceRequest request, int EmpId)
        {
            //int EmpId = GetEmpId();
      
            var salaryComponentList = _salaryComponent.getSalaryComponentList(EmpId);

            return Json(salaryComponentList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult SalaryComponentDetail(int id, int EmpId)  
        {
            SalaryComponentViewModel salaryComponentViewModel = new SalaryComponentViewModel();
            salaryComponentViewModel = _salaryComponent.GetSalaryComponentDetail(id);
            if (salaryComponentViewModel.id == 0) { salaryComponentViewModel.employeeID = EmpId; }
            return PartialView("SalaryComponentAddEdit", salaryComponentViewModel);
        }

        public ActionResult SaveSalaryComponent(SalaryComponentViewModel salaryComponentViewModel)
        {
            bool recordIsNew = false;
            string _message = "";
            salaryComponentViewModel.enteredBy = User.Identity.Name;
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            if (ModelState.IsValid)
            {
                try
                {
                    if (salaryComponentViewModel.id == 0) { recordIsNew = true; }
                    salaryComponentViewModel = _salaryComponent.SaveSalaryComponent(salaryComponentViewModel);
                    return Json(new { salaryComponentViewModel, succeed = true, recordIsNew }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    IEnumerable<DbEntityValidationResult> errors = clientDbContext.GetValidationErrors();
                    if (errors.Any())
                    {
                        _message += err.InnerException.InnerException.Message;
                    }
                    else
                    {
                        foreach (DbEntityValidationResult error in errors)
                        {
                            foreach (var valError in error.ValidationErrors)
                            {
                                if (_message != "") _message += "<br />";
                                _message += valError.ErrorMessage;
                            }
                        }
                    }
                    return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
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
        }      
        public ActionResult AdaList_Destroy([DataSourceRequest] DataSourceRequest request, SalaryComponentViewModel salaryComponentViewModel)
        {
            try
            {
                _salaryComponent.SalaryComponentsDelete(salaryComponentViewModel.id);
            }
            catch
            {
                ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
            }
            return Json(new[] { salaryComponentViewModel }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult GetValues(int id)
        {
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            string connString = User.Identity.GetClientConnectionString();
            var data = clientDbContext.DdlSalaryComponents.Where(m => m.salaryComponentID == id).FirstOrDefault();
             return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}