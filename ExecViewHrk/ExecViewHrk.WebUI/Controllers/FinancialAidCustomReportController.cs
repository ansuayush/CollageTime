using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Domain.Models;
using ExecViewHrk.EfClient;
using ExecViewHrk.Models;
using ExecViewHrk.WebUI.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    public class FinancialAidCustomReportController : BaseController
    {
        IFinancialRepository _FinancialRepository;

        public FinancialAidCustomReportController(IFinancialRepository FinancialRepository)
        {
            _FinancialRepository = FinancialRepository;
        }
        // GET: FinancialAidCustomReport
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FinancialAidCustomReport()
        {
            var id = _FinancialRepository.GetPayperiodidlist();
            ViewBag.PayPeriodId = id;
            return View();
        }
        public ActionResult FinancialAidList_Read([DataSourceRequest]DataSourceRequest request)
        {
            var lstEmpFinancialReports = _FinancialRepository.GetEmpFinancialReportList();
            foreach (var item in lstEmpFinancialReports)
            {
                if (item.DailyHours != null)
                {
                    item.DailyHours = Math.Round(item.DailyHours.Value, 2);
                }
            }
            return Json(lstEmpFinancialReports.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
       
        public ActionResult FinancialAidList1_Read([DataSourceRequest]DataSourceRequest request,int payperiodid)
        {
            if(payperiodid ==null)
            {
                payperiodid = _FinancialRepository.GetPayperiodidlist();
            }
             
            var lstEmpFinancialReports1 = _FinancialRepository.GetEmpFinancialReportList1(payperiodid);
            
            foreach (var item in lstEmpFinancialReports1)
            {
                if (item.DailyHours != null)
                {
                    item.DailyHours = Math.Round(item.DailyHours.Value, 2);
                }
            }
            return Json(lstEmpFinancialReports1.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPayPeriodsList()
        {
            var payPeriodsList = _FinancialRepository.GetPayPeriodsList();
            return Json(payPeriodsList, JsonRequestBehavior.AllowGet);
        }
    }
}