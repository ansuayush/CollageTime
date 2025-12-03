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
    public class DdlEducationLevelsController : Controller
    {
        // GET: DdlEducationLevels
        public ActionResult DdlEducationLevelsListMaintenance()
        {
            return View();
        }

        public ActionResult DdlEducationLevelsList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var educationLevelsList = clientDbContext.DdlEducationLevels.OrderBy(e => e.Description).ToList();
                return Json(educationLevelsList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlEducationLevelsList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlEducationLevel educationLevel)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (educationLevel != null && ModelState.IsValid)
                {
                    var educationLevelInDb = clientDbContext.DdlEducationLevels
                        .Where(x => x.Code == educationLevel.Code || x.Description == educationLevel.Description)
                        .SingleOrDefault();

                    if (educationLevelInDb != null)
                    {
                        ModelState.AddModelError("", "The Education level" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                    }
                    else
                    {
                        var newEducationLevel = new DdlEducationLevel
                        {
                            Description = educationLevel.Description,
                            Code = educationLevel.Code,
                            Active = educationLevel.Active
                        };

                        clientDbContext.DdlEducationLevels.Add(newEducationLevel);
                        clientDbContext.SaveChanges();
                        educationLevel.EducationLevelId = newEducationLevel.EducationLevelId;
                    }
                }

                return Json(new[] { educationLevel }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlEducationLevelsList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlEducationLevel educationLevel)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var EducationLevelInDb = clientDbContext.DdlEducationLevels
                        .Where(x => x.EducationLevelId != educationLevel.EducationLevelId && (x.Code == educationLevel.Code || x.Description == educationLevel.Description))
                        .SingleOrDefault();
                if (EducationLevelInDb == null)
                {
                    if (educationLevel != null && ModelState.IsValid)
                    {
                        var educationLevelInDb = clientDbContext.DdlEducationLevels
                          .Where(x => x.EducationLevelId == educationLevel.EducationLevelId)
                          .SingleOrDefault();
                        if (educationLevelInDb != null)
                        {
                            educationLevelInDb.Code = educationLevel.Code;
                            educationLevelInDb.Description = educationLevel.Description;
                            educationLevelInDb.Active = educationLevel.Active;
                            clientDbContext.SaveChanges();
                        }                       

                    }
                }
                else
                {
                    ModelState.AddModelError("", "The Education level" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                }

                return Json(new[] { educationLevel }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlEducationLevelsList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlEducationLevel educationLevel)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (educationLevel != null)
                {
                    DdlEducationLevel educationLevelInDb = clientDbContext.DdlEducationLevels
                        .Where(x => x.EducationLevelId == educationLevel.EducationLevelId).SingleOrDefault();

                    if (educationLevelInDb != null)
                    {
                        clientDbContext.DdlEducationLevels.Remove(educationLevelInDb);

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

                return Json(new[] { educationLevel }.ToDataSourceResult(request, ModelState));
            }
        }

        public JsonResult GetEducationLevels(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var educationLevels = clientDbContext.DdlEducationLevels
                    .Where(x => x.Active == true)
                    .Select(m => new
                    {
                        EducationLevelId = m.EducationLevelId,
                        EducationLevelDescription = m.Description
                    }).OrderBy(x => x.EducationLevelDescription).ToList();

                return Json(educationLevels, JsonRequestBehavior.AllowGet);
            }

        }

    }
}