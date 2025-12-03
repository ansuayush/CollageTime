using ExecViewHrk.WebUI.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    public class ListController : Controller
    {
        // GET: List
        public ActionResult ListMatrix(string requestType = "NSS")
        {
            if (requestType == "NSS" && User.IsInRole("ClientEmployees"))
                throw new Exception("Client Employee trying to access NSS.");

            User.Identity.AddUpdateClaim(SessionStateKeys.REQUEST_TYPE.ToString(), requestType);
            return View();
        }
    }
}