using ExecViewHrk.Domain.Interface;
using ExecViewHrk.EfClient;
using ExecViewHrk.Models;
using ExecViewHrk.WebUI.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    public class ReportsController : Controller
    {
        readonly IReportRepository _reportRepository;

        public ReportsController(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public ActionResult PositionReportList()
        {
            return View();
        }


        public ActionResult PositionReportList_Read([DataSourceRequest]DataSourceRequest request)
        {
            var positionReports = _reportRepository.GetPositionReportList();
            return Json(positionReports.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult EmployeePositionReport()
        {
            return View();
        }
        public ActionResult EmployeePositionReport_Read([DataSourceRequest]DataSourceRequest request)
        {

            var _positionReport = _reportRepository.GetEmployeePositionReportList();
            return Json(_positionReport.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult OpenPositionReport()
        {
            return View();
        }
        public ActionResult OpenPositionReport_Read([DataSourceRequest]DataSourceRequest request)
        {

            var openpositionReport = _reportRepository.GetOpenPositionReportList();
            return Json(openpositionReport.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult PositionsInUseReport()
        {
            var positionInUseVm = new PositionsInUseReportVm
            {
                lstBudgetYears = GetYears(),
                budgetYear = System.DateTime.Now.Year
            };

            return View(positionInUseVm);

        }
        public ActionResult PositionsInUseReportList_Read([DataSourceRequest]DataSourceRequest request)
        {
            var positionInUseReports = _reportRepository.GetPositionsInUseReportList();
            return Json(positionInUseReports.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult TurnoverRpt()
        {
            return View();
        }

        public ActionResult PositionClosedReport()
        {
            var positionClosedVm = new PositionClosedReportVm
            {
                lstBudgetYears = GetYears()
            };
            var currentYear = System.DateTime.Now.Year;
            positionClosedVm.budgetYear = currentYear;
            return View(positionClosedVm);

        }
        public ActionResult PositionClosedReportList_Read([DataSourceRequest]DataSourceRequest request)
        {
            var positionClosedReports = _reportRepository.GetPositionClosedReportListList();
            return Json(positionClosedReports.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult BudgetToActualsReport()
        {
            var budgetToActualsReportVm = new BudgetToActualsReportVm
            {
                lstBudgetYears = GetYears(),
                lstBudgetMonths = GetMonths()
            };
            var currentYear = System.DateTime.Now.Year;
            budgetToActualsReportVm.budgetMonth = 1;
            budgetToActualsReportVm.budgetYear = currentYear;

            return View(budgetToActualsReportVm);

        }
        public ActionResult BudgetToActualsReportList_Read([DataSourceRequest]DataSourceRequest request, int budgetYear, int month)
        {
            var budgetToActualsReport = _reportRepository.GetBudgetToActualsReportList(budgetYear, month).ToList();
            return Json(budgetToActualsReport.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public List<DropDownModel> GetYears()
        {
            var lstDropDownYears = new List<DropDownModel>
            {
                new DropDownModel { keyvalue = "2011", keydescription = "2011" },
                new DropDownModel { keyvalue = "2012", keydescription = "2012" },
                new DropDownModel { keyvalue = "2013", keydescription = "2013" },
                new DropDownModel { keyvalue = "2014", keydescription = "2014" },
                new DropDownModel { keyvalue = "2015", keydescription = "2015" },
                new DropDownModel { keyvalue = "2016", keydescription = "2016" },
                new DropDownModel { keyvalue = "2017", keydescription = "2017" },
                new DropDownModel { keyvalue = "2018", keydescription = "2018" },
                new DropDownModel { keyvalue = "2019", keydescription = "2019" },
                new DropDownModel { keyvalue = "2020", keydescription = "2020" }
            };
            return lstDropDownYears;
        }

        public List<DropDownModel> GetMonths()
        {
            var lstDropDownMonths = new List<DropDownModel>
            {
                new DropDownModel {keyvalue = "1", keydescription = "January"},
                new DropDownModel {keyvalue = "2", keydescription = "February"},
                new DropDownModel {keyvalue = "3", keydescription = "March"},
                new DropDownModel {keyvalue = "4", keydescription = "April"},
                new DropDownModel {keyvalue = "5", keydescription = "May"},
                new DropDownModel {keyvalue = "6", keydescription = "June"},
                new DropDownModel {keyvalue = "7", keydescription = "July"},
                new DropDownModel {keyvalue = "8", keydescription = "August"},
                new DropDownModel {keyvalue = "9", keydescription = "September"},
                new DropDownModel {keyvalue = "10", keydescription = "October"},
                new DropDownModel {keyvalue = "11", keydescription = "November"},
                new DropDownModel {keyvalue = "12", keydescription = "December"}
            };
            return lstDropDownMonths;
        }
        public ActionResult ContractReport()
        {
            return View();
        }
        public ActionResult ContractReport_Read([DataSourceRequest]DataSourceRequest request)
        {

            var contractReport = _reportRepository.GetContractReportList();
            return Json(contractReport.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        #region ContractsReport       
        public ActionResult ContractsReport()
        {
            return View();
        }

        public ActionResult ContractsReportList_Read([DataSourceRequest]DataSourceRequest request)
        {
            var contractsReport = _reportRepository.GetContractsReportList();
            return Json(contractsReport.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Contract Earn Burn Report
        public ActionResult ContractEarnBurnReport()
        {
            return View();
        }


        public ActionResult ContractEarnBurnReport_Read([DataSourceRequest]DataSourceRequest request)
        {
            var positionReports = _reportRepository.GetContractEarnBurnReportList();
            return Json(positionReports.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Salary Fringe Report
        public ActionResult SalaryFringeReport()
        {
            return View();
        }


        public ActionResult SalaryFringeReport_Read([DataSourceRequest]DataSourceRequest request)
        {
            var positionReports = _reportRepository.GetSalaryFringeReportList();
            return Json(positionReports.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }



        public ActionResult StudentPositionReport()
        {
            return View();
        }
        public ActionResult StudentPositionReport_Read([DataSourceRequest]DataSourceRequest request)
        {

            var _positionReport = _reportRepository.GetStudentPositionReportList();
            return Json(_positionReport.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        #endregion
        #region Treaty Report
        public ActionResult TreatyReport()
        {
            return View();
        }
        public ActionResult TreatyReport_Read([DataSourceRequest]DataSourceRequest request)
        {

            var treatyreport = _reportRepository.GetTreatyReportList();
            return Json(treatyreport.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region  Work Study limits
        public ActionResult WorkStudylimit()
        {
            return View();
        }
        public ActionResult WorkStudylimitReport_Read([DataSourceRequest]DataSourceRequest request)
        {

            var workstudyreport = _reportRepository.GetWorkStudylimitReportList();
            return Json(workstudyreport.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        #endregion


    }
}