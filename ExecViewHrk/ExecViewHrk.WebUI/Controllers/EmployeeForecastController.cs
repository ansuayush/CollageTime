using ExecViewHrk.Domain.Interface;
using ExecViewHrk.EfClient;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    public class EmployeeForecastController : Controller
    {
        // GET: EmployeeForecast
        IEmployeeForecast _employeeForecast;
        public EmployeeForecastController(IEmployeeForecast empforecast)
        {
            _employeeForecast = empforecast;
        }

        public ActionResult EmployeeForeCastMain()
        {
            return View();
        }


        public ActionResult EmployeeForecasMatrixPartial()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EmployeeForeCastMain(FormCollection obj, string submit)
        {
            switch (submit)
            {
                case "updateall":
                    _employeeForecast.UpdateAllEmpForecast(Convert.ToDecimal(obj["allempforecastpercentval"]));
                    break;
                case "clearall":
                    _employeeForecast.ClearAllEmpForecast();
                    break;
            }
            return RedirectToAction("EmployeeForeCastMain");
        }


        // GET: EmployeeForecast
        public ActionResult EmployeeForecastList([DataSourceRequest]DataSourceRequest request)
        {
            List<EmployeeForecastVm> employeeForecastVm = new List<EmployeeForecastVm>();
            employeeForecastVm = _employeeForecast.GetEmployeeForecast();
            return Json(employeeForecastVm.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }


        public ActionResult EditEmployeeForecast(string fileNumber)
        {
            EmployeeForecastVm obj = new EmployeeForecastVm();
            obj = _employeeForecast.GetEmployeeByFileNumber(Convert.ToInt32(fileNumber));
            return View(obj);
        }

        public ActionResult UpdateEmpForecast([DataSourceRequest] DataSourceRequest request, EmployeeForecastVm objempforecast)
        {
            //EmployeeForecastVm obj = new EmployeeForecastVm();
            _employeeForecast.UpdateIncreasePercent(objempforecast.ePosSalHistoryId, objempforecast.IncreasePercent);
            return Json(new[] { objempforecast }.ToDataSourceResult(request, ModelState));
        }
    }
}