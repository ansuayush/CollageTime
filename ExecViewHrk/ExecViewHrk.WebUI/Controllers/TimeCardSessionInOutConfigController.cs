using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Infrastructure;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Domain.Models;
using ExecViewHrk.Models;

namespace ExecViewHrk.WebUI.Controllers
{
    public class TimeCardSessionInOutConfigController : Controller
    {
        readonly ITimeCardSessionInOutRepository _itimeseiionRepo;
        public TimeCardSessionInOutConfigController(ITimeCardSessionInOutRepository itimeseiionRepo)
        {
            _itimeseiionRepo = itimeseiionRepo;
        }

        public PartialViewResult TimeCardSessionInOutMatrixPartial()
        {
            var timeCardColumnList = _itimeseiionRepo.GetTimeCardSessionList();
            return PartialView(timeCardColumnList);
        }
       
        public ActionResult TimeCardsessionList_Read([DataSourceRequest]DataSourceRequest request)
        {
            var timeCardColumnList = _itimeseiionRepo.GetTimeCardSessionList();
            return Json(timeCardColumnList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTimeCardsessionInoutDetails(int timecardssessionId)
        { 
            var timeCardColumnsDetails = _itimeseiionRepo.GetTimecradssessiondeatils(timecardssessionId);
            return View("TimeCardSessionInOuDetails", timeCardColumnsDetails);
        }

        [HttpPost]
        public ActionResult TimeCardSessionInOut_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<TimeCardSessionInOutConfigsVm> tcsvm)
        {
            if (tcsvm != null && ModelState.IsValid)
            {
                foreach (var item in tcsvm)
                {
                    var status = _itimeseiionRepo.updatetimecardsseiion(item);
                }
            }

            return Json(new[] { tcsvm }.ToDataSourceResult(request, ModelState));
        }
    
    }
}