using AutoMapper;
using ExecViewHrk.Domain.Interface;
using ExecViewHrk.EfClient;
using ExecViewHrk.Models;
using ExecViewHrk.WebUI.Helpers;
using ExecViewHrk.WebUI.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    public class TimeCardUnApprovedReportController : BaseController
    {
        readonly ILookupTablesRepository _ilookuprepo;
        readonly ITimeCardUnApprovedReportRepository _iTimeCardUnApprovedReportRepository;

        public TimeCardUnApprovedReportController(ILookupTablesRepository ilookuprepo, ITimeCardUnApprovedReportRepository iTimeCardUnApprovedReportRepository)
        {
            _ilookuprepo = ilookuprepo;
            _iTimeCardUnApprovedReportRepository = iTimeCardUnApprovedReportRepository;
        }

        // GET: TimeCardSummaryReport
        public PartialViewResult TimeCardUnapprovedReportPartial()
        {
            //ViewData["employeesList"] = _ilookuprepo.GetEmployeesList();
            TimeCardUnApprovedReportVM timeCardSummaryReport = new TimeCardUnApprovedReportVM();
            return PartialView(timeCardSummaryReport);
        }

        public PartialViewResult TimeCardUnapprovedReportPartialNew(int companyCodeId, int payPeriodId)
        {
            //ViewData["employeesList"] = _ilookuprepo.GetEmployeesList();
            TimeCardUnApprovedReportVM timeCardSummaryReport = new TimeCardUnApprovedReportVM();
            return PartialView("TimeCardUnapprovedReportPartial", timeCardSummaryReport);
        }

        public JsonResult GetPayPeriodsList(int? CompanyCodeIdDdl)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                CompanyCodeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).CompanyCodeId : CompanyCodeIdDdl;
                var payPeriodsList = _ilookuprepo.GetPayPeriodsList(CompanyCodeIdDdl);
                return Json(payPeriodsList, JsonRequestBehavior.AllowGet);
            }
        }

        //#2959: Loads payperiods regardless of company       
        public JsonResult GetGlobalPayPeriodsList()
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var payPeriodsList = _ilookuprepo.GetGlobalPayPeriodsList();
                return Json(payPeriodsList, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult TimeCardSummaryList_ReadUnApproved([DataSourceRequest]DataSourceRequest request, int? companyCodeId, int? payPeriodId)
        {
            var employeeTimeCardSummaryListVM = new List<TimeCardUnApprovedReportVM>();
            try
            {
                if (companyCodeId.HasValue && payPeriodId.HasValue)
                {
                    List<TimeCardUnApprovedReportDM> summaryDM = _iTimeCardUnApprovedReportRepository.GetTimeCardUnApprovedReportList(companyCodeId.Value, payPeriodId.Value);
                    employeeTimeCardSummaryListVM = Mapper.Map<List<TimeCardUnApprovedReportDM>, List<TimeCardUnApprovedReportVM>>(summaryDM);
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.InnerException.Message);
            }
            return Json(employeeTimeCardSummaryListVM.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }
}