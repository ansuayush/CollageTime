using ExecViewHrk.Domain.Interface;
using ExecViewHrk.EfClient;
using ExecViewHrk.Models;
using ExecViewHrk.WebUI.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    public class HoursCodesController : BaseController
    {
        readonly IHoursCodesRepository _hoursCodesRepository;
        readonly ILookupTablesRepository _ilookuprepo;
        public HoursCodesController(IHoursCodesRepository hoursCodesRepository, ILookupTablesRepository ilookuprepo)
        {
            _hoursCodesRepository = hoursCodesRepository;
            _ilookuprepo = ilookuprepo;
        }

        public ActionResult HoursCodesMatrixPartial()
        {
            ViewData["companyCodesList"] = _ilookuprepo.GetCompanyCodes();
            ViewData["adpFieldMappingsList"] = _ilookuprepo.GetADPFieldMappings();
            ViewData["adpAccNumbersList"] = _ilookuprepo.GetADPAccNumbers();
            return View();
        }

        //#region Dropdown Lists

        ///// <summary>
        ///// Populates Company Codes for Dropdown
        ///// </summary>

        //private void PopulateCompanyCodes()
        //{
        //    var companyCodesList = _hoursCodesRepository.PopulateCompanyCodes();
        //    if (companyCodesList != null)
        //        ViewData["companyCodesList"] = companyCodesList;
        //    else
        //        ViewData["companyCodesList"] = null;
        //}

        ///// <summary>
        ///// Populates ADP Fields Mappings for Dropdown
        ///// </summary>

        //private void PopulateADPFieldMappings()
        //{
        //    var adpFieldMappingsList = _hoursCodesRepository.PopulateADPFieldMappings();
        //    if (adpFieldMappingsList != null)
        //        ViewData["adpFieldMappingsList"] = adpFieldMappingsList;
        //    else
        //        ViewData["adpFieldMappingsList"] = null;
        //}

        ///// <summary>
        ///// Populates ADP Account Numbes for Dropdown
        ///// </summary>
        //private void PopulateADPAccNumbers()
        //{
        //    var adpAccNumbersList = _hoursCodesRepository.PopulateADPAccNumbers();
        //    if (adpAccNumbersList != null)
        //        ViewData["adpAccNumbersList"] = adpAccNumbersList;
        //    else
        //        ViewData["adpAccNumbersList"] = null;
        //}

        //#endregion

        #region List and Details. Insert, Update and Delete

        /// <summary>
        /// Returns the HoursCodes List Partial View
        /// </summary>
        /// <returns></returns>

        /// <summary>
        /// Returns the List of Records to the Partial View-HoursCodesMatrixPartial
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        public ActionResult HoursCodesList_Read([DataSourceRequest]DataSourceRequest request)
        {
            var hoursCodesList = _hoursCodesRepository.HoursCodesList_Read();
            return Json(hoursCodesList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Returns the Hours Codes Details Partial View By HoursCodeId
        /// </summary>
        /// <param name="HoursCodeId"></param>
        /// <returns></returns>

        public ActionResult HoursEditMatrix(int HoursCodeId)
        {
            ViewData["companyCodesList"] = _ilookuprepo.GetCompanyCodes();
            ViewData["adpFieldMappingsList"] = _ilookuprepo.GetADPFieldMappings();
            ViewData["adpAccNumbersList"] = _ilookuprepo.GetADPAccNumbers();
            var hoursCodeVm = _hoursCodesRepository.HoursEditMatrix(HoursCodeId);
            return View(hoursCodeVm);
        }

        /// <summary>
        /// Inserts and Updates the Hours Codes record. And throws the Model error for Duplication.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="hoursCodeVM"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult HoursCodeSaveAjax([DataSourceRequest] DataSourceRequest request, HoursCodeVm hoursCodeVM)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var errors = ModelState.Values.SelectMany(m => m.Errors);
            bool succeed = false;
            try
            {
                if (hoursCodeVM != null && ModelState.IsValid)
                {
                    var hourscode = _hoursCodesRepository.HoursCodesList_Read().Where(x => x.HoursCodeId == hoursCodeVM.HoursCodeId).FirstOrDefault();
                    //var isHoursCodeExists1 = _hoursCodesRepository.HoursCodesList_Read().Where(x => x.HoursCodeCode == hoursCodeVM.HoursCodeCode && x.CompanyCodeId == hoursCodeVM.CompanyCodeId).Count();
                    //if (isHoursCodeExists1 > 0)
                    //{
                    //    return Json(new { succeed = false, Message = string.Format(CustomErrorMessages.ERROR_DUPLICATE_RECORD, "Hours Code") }, JsonRequestBehavior.AllowGet);
                    //}
                    var hourscodelist = clientDbContext.HoursCodes.ToList();
                    if (hourscodelist != null)
                    {
                        foreach (var item in hourscodelist)
                        {
                            if ((item.CompanyCodeId == hoursCodeVM.CompanyCodeId) && (hoursCodeVM.StartDate >= item.StartDate && hoursCodeVM.StartDate <= item.EndDate) && (item.HoursCodeId != hoursCodeVM.HoursCodeId))
                            {
                                return Json(new { succeed = false, Message = string.Format(CustomErrorMessages.ERROR_DUPLICATE_RECORD, "Start Date is Overlap with existing dates") }, JsonRequestBehavior.AllowGet);

                            }
                            if ((item.CompanyCodeId == hoursCodeVM.CompanyCodeId) && (hoursCodeVM.EndDate >= item.StartDate && hoursCodeVM.EndDate <= item.EndDate) && (item.HoursCodeId != hoursCodeVM.HoursCodeId))
                            {
                                return Json(new { succeed = false, Message = string.Format(CustomErrorMessages.ERROR_DUPLICATE_RECORD, "End Date is Overlap with existing dates") }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        succeed = _hoursCodesRepository.HoursCodeSaveAjax(hoursCodeVM);
                    }
                    else
                    {
                        //var isHoursCodeExists = _hoursCodesRepository.HoursCodesList_Read().Where(x => x.HoursCodeCode == hoursCodeVM.HoursCodeCode && x.CompanyCodeId == hoursCodeVM.CompanyCodeId).Count();
                        //if (isHoursCodeExists > 0)
                        //{
                        //    return Json(new { succeed = false, Message = string.Format(CustomErrorMessages.ERROR_DUPLICATE_RECORD, "Hours Code") }, JsonRequestBehavior.AllowGet);
                        //}

                        if (hourscodelist != null)
                        {
                            foreach (var item in hourscodelist)
                            {
                                if ((item.CompanyCodeId == hoursCodeVM.CompanyCodeId) && (item.HoursCodeId != hoursCodeVM.HoursCodeId) && (hoursCodeVM.StartDate >= item.StartDate && hoursCodeVM.StartDate <= item.EndDate))
                                {
                                    return Json(new { succeed = false, Message = string.Format(CustomErrorMessages.ERROR_DUPLICATE_RECORD, "Start Date is Overlap with existing dates") }, JsonRequestBehavior.AllowGet);

                                }
                                if ((item.CompanyCodeId == hoursCodeVM.CompanyCodeId) && (item.HoursCodeId != hoursCodeVM.HoursCodeId) && (hoursCodeVM.EndDate >= item.StartDate && hoursCodeVM.EndDate <= item.EndDate))
                                {
                                    return Json(new { succeed = false, Message = string.Format(CustomErrorMessages.ERROR_DUPLICATE_RECORD, "End Date is Overlap with existing dates") }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            succeed = _hoursCodesRepository.HoursCodeSaveAjax(hoursCodeVM);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                string error = ModelState.Values.SelectMany(m => m.Errors).Select(m => m.ErrorMessage).LastOrDefault();
                return Json(new { succeed = false, Message = error }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { hoursCodeVM, succeed }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Deletes the record
        /// </summary>
        /// <param name="request"></param>
        /// <param name="HoursCodeId"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult HoursCodesDeleteAjax([DataSourceRequest] DataSourceRequest request, int HoursCodeId)
        {
            if (HoursCodeId != 0)
            {
                try
                {
                    //Check Existing Records with PayPeriodId
                    string connString = User.Identity.GetClientConnectionString();
                    ClientDbContext clientDbContext = new ClientDbContext(connString);
                    if (clientDbContext.TimeCards.Any(x => x.HoursCodeId == HoursCodeId && x.IsDeleted ==false))
                    {
                        return Json(new { Message = CustomErrorMessages.ERROR_DELETING_RECORD_ALREADY_IN_USE, succeed = false }, JsonRequestBehavior.AllowGet);
                    }
                    if (clientDbContext.EmployeeRetroHours.Any(x => x.HoursCodeId == HoursCodeId))
                    {
                        return Json(new { Message = CustomErrorMessages.ERROR_DELETING_RECORD_ALREADY_IN_USE, succeed = false }, JsonRequestBehavior.AllowGet);
                    }

                    _hoursCodesRepository.HourCodeDestroy(HoursCodeId);
                }
                catch (Exception ex)
                {
                    //ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                    string error = ModelState.Values.SelectMany(m => m.Errors).Select(m => m.ErrorMessage).LastOrDefault();
                    return Json(new { succeed = false, Message = error }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new[] { HoursCodeId }.ToDataSourceResult(request, ModelState));
        }

        #endregion
    }
}