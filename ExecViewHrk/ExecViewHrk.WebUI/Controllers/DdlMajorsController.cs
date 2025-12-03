using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExecViewHrk.WebUI.Infrastructure;
using ExecViewHrk.EfAdmin;
using ExecViewHrk.EfClient;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Net;
using ExecViewHrk.WebUI.Helpers;
namespace ExecViewHrk.WebUI.Controllers
{
    public class DdlMajorsController : Controller
    {
        // GET: DdlMajors
        public ActionResult DdlMajorsListMaintenance()
        {
            return View();
        }

        public ActionResult DdlMajorsList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var majorsList = clientDbContext.DdlMajors.OrderBy(e => e.Description).ToList();
                return Json(majorsList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlMajorsList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlMajor major)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (major != null && ModelState.IsValid)
                {
                    var majorInDb = clientDbContext.DdlMajors
                        .Where(x => x.Code == major.Code)
                        .SingleOrDefault();

                    if (majorInDb != null)
                    {
                        ModelState.AddModelError("", "The Major" + CustomErrorMessages.ERROR_ALREADY_DEFINED);
                    }
                    else
                    {
                        var newMajor = new DdlMajor
                        {
                            Description = major.Description,
                            Code = major.Code,
                            Active = true
                        };

                        clientDbContext.DdlMajors.Add(newMajor);
                        clientDbContext.SaveChanges();
                        major.MajorId = newMajor.MajorId;
                    }
                }

                return Json(new[] { major }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlMajorsList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlMajor major)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var MajorsInDb = clientDbContext.DdlMajors.Where(x => x.MajorId != major.MajorId && (x.Code == major.Code || x.Description == major.Description)).SingleOrDefault();
                if (MajorsInDb == null)
                {
                    //MajorsInDb = clientDbContext.DdlMajors.Where(x => x.MajorId == major.MajorId).SingleOrDefault();
                    if (major != null && ModelState.IsValid)
                    {
                        var majorInDb = clientDbContext.DdlMajors
                            .Where(x => x.MajorId == major.MajorId)
                            .SingleOrDefault();

                        if (majorInDb != null)
                        {
                            majorInDb.Code = major.Code;
                            majorInDb.Description = major.Description;
                            majorInDb.Active = major.Active;
                            clientDbContext.SaveChanges();
                        }
                    }
                }
                else {
                    ModelState.AddModelError("", "Major Code" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                }
                return Json(new[] { major }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlMajorsList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlMajor major)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (major != null)
                {
                    DdlMajor majorInDb = clientDbContext.DdlMajors
                        .Where(x => x.MajorId == major.MajorId).SingleOrDefault();

                    if (majorInDb != null)
                    {
                        clientDbContext.DdlMajors.Remove(majorInDb);

                        try
                        {
                            clientDbContext.SaveChanges();
                        }
                        catch// (Exception err)
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                        }

                    }
                }

                return Json(new[] { major }.ToDataSourceResult(request, ModelState));
            }
        }

        public JsonResult GetMajors(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var majors = clientDbContext.DdlMajors
                    .Where(x => x.Active == true)
                    .Select(m => new
                    {
                        MajorId = m.MajorId,
                        MajorDescription = m.Description
                    }).OrderBy(x => x.MajorDescription).ToList();

                return Json(majors, JsonRequestBehavior.AllowGet);
            }

        }
    }
}