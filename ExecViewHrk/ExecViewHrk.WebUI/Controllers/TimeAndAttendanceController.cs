using ExecViewHrk.WebUI.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    public class TimeAndAttendanceController : Controller
    {
        // GET: TimeAndAttendance
        public ActionResult TimeCardMatrix(string requestType = "NSS")
        {
            if (requestType == "NSS" && User.IsInRole("ClientEmployees"))
                throw new Exception("Client Employee trying to access NSS.");

            User.Identity.AddUpdateClaim(SessionStateKeys.REQUEST_TYPE.ToString(), requestType);
            return View();
        }
    }
}