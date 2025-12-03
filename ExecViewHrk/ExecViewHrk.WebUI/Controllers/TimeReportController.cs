using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    [Authorize]

    public class TimeReportController : BaseController
    {
        ITimeReportRepository _timeReportRepository;

        public TimeReportController(ITimeReportRepository timeReportRepository)
        {
            _timeReportRepository = timeReportRepository;
        }

        // GET: TimeReport
        public ActionResult TimeReport()
        {
            var model = new PayPeriodVM();
            return View();
        }


        public ActionResult _TimeReportList()
        {
            var lst = new List<TimeReportVm>();
            return PartialView(lst);
        }

        public ActionResult _TimeReportList_Read([DataSourceRequest]DataSourceRequest request, DateTime startDate, DateTime endDate)
        {
            //var list = _timeReportRepository.GetTimeReportList(startDate, endDate);
            var lstEmpTimeReports = _timeReportRepository.GetEmpTimeReportList(startDate, endDate);
            foreach (var item in lstEmpTimeReports)
            {
                item.ActualDate = TimeZoneInfo.ConvertTimeToUtc(item.ActualDate, TimeZoneInfo.Utc);
                if (item.DailyHours != null)
                {
                    item.DailyHours = Math.Round(item.DailyHours.Value, 2);
                }
                if (item.TimeIn != null)
                {
                    item.TimeIn = TimeZoneInfo.ConvertTimeToUtc(item.TimeIn.Value, TimeZoneInfo.Utc);
                }
                if (item.LunchOut != null)
                {
                    item.LunchOut = TimeZoneInfo.ConvertTimeToUtc(item.LunchOut.Value, TimeZoneInfo.Utc);
                }
                if (item.LunchBack != null)
                {
                    item.LunchBack = TimeZoneInfo.ConvertTimeToUtc(item.LunchBack.Value, TimeZoneInfo.Utc);
                }
                if (item.TimeOut != null)
                {
                    item.TimeOut = TimeZoneInfo.ConvertTimeToUtc(item.TimeOut.Value, TimeZoneInfo.Utc);
                }
                if (item.LastModifiedDate != null)
                {
                    item.LastModifiedDate = TimeZoneInfo.ConvertTimeToUtc(item.LastModifiedDate.Value, TimeZoneInfo.Utc);
                }
            }
            return KendoCustomResult(lstEmpTimeReports.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }
}