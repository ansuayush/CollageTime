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
    public class TimeCardConfigurationsController : Controller
    {
        readonly ITimeCardConfigurationsRepository _timeCardConfigRepository;

        public TimeCardConfigurationsController(ITimeCardConfigurationsRepository timeCardConfigRepository)
        {
            _timeCardConfigRepository = timeCardConfigRepository;
        }

        // GET: TimeCardConfigurations
        public PartialViewResult TimeCardConfigurationsMatrixPartial()
        {
            PopulateTimeCardTypes();
            return PartialView();
        }

        #region List, Details and Update
        public ActionResult TimeCardColumnsList_Read([DataSourceRequest]DataSourceRequest request)
        {
            var timeCardColumnList = _timeCardConfigRepository.GetTimeCardColumnsList();
            return Json(timeCardColumnList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTimeCardColumnsDetails(int timeCardTypeId)
        {
            PopulateTimeCardTypes();
            var timeCardColumnsDetails = _timeCardConfigRepository.GetTimeCardColumnsById(timeCardTypeId);
            return View("TimeCardConfigurationsDetails", timeCardColumnsDetails);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TimeCardColumnsList_Update([DataSourceRequest] DataSourceRequest request
            , TimeCardDisplayColumnVM timecardColumn)
        {
            if (timecardColumn != null && ModelState.IsValid)
            {
                var status = _timeCardConfigRepository.TimeCardColumnsList_Update(timecardColumn);
            }

            return Json(new[] { timecardColumn }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Dropdown Lists

        /// <summary>
        /// Populates the Time Card Type List for Dropdown.
        /// </summary>
        private void PopulateTimeCardTypes()
        {
            var timecardTypesList = _timeCardConfigRepository.PopulateTimeCardTypes();
            ViewData["timecardTypesList"] = timecardTypesList;
        }

        #endregion
    }

}