using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    public class NoRolesController : Controller
    {
        // GET: NoRoles
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
    }
}