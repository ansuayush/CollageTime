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
    public class DdlDegreeTypesController : Controller
    {
        // GET: DdlDegreeTypes
        public ActionResult DdlDegreeTypesListMaintenance()
        {
            return View();
        }

        public ActionResult DdlDegreeTypesList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var degreeTypeList = clientDbContext.DdlDegreeTypes.OrderBy(e => e.Description).ToList();
                return Json(degreeTypeList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlDegreeTypesList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlDegreeType degreeType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (degreeType != null && ModelState.IsValid)
                {
                    var degreeTypeInDb = clientDbContext.DdlDegreeTypes
                        .Where(x => x.Code == degreeType.Code)
                        .SingleOrDefault();

                    if (degreeTypeInDb != null)
                    {
                        ModelState.AddModelError("", "The Degree Type" + CustomErrorMessages.ERROR_ALREADY_DEFINED);
                    }
                    else
                    {
                        var newDegreeType = new DdlDegreeType
                        {
                            Description = degreeType.Description,
                            Code = degreeType.Code,
                            Active = true
                        };

                        clientDbContext.DdlDegreeTypes.Add(newDegreeType);
                        clientDbContext.SaveChanges();
                        degreeType.DegreeTypeId = newDegreeType.DegreeTypeId;
                    }
                }

                return Json(new[] { degreeType }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlDegreeTypesList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlDegreeType degreeType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (degreeType != null && ModelState.IsValid)
                {
                    var degreeTypeInDb = clientDbContext.DdlDegreeTypes
                        .Where(x => x.DegreeTypeId == degreeType.DegreeTypeId)
                        .SingleOrDefault();

                    if (degreeTypeInDb != null)
                    {
                        degreeTypeInDb.Code = degreeType.Code;
                        degreeTypeInDb.Description = degreeType.Description;
                        degreeTypeInDb.Active = degreeType.Active;
                        clientDbContext.SaveChanges();
                    }
                }

                return Json(new[] { degreeType }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlDegreeTypesList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlDegreeType degreeType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (degreeType != null)
                {
                    DdlDegreeType degreeTypeInDb = clientDbContext.DdlDegreeTypes
                        .Where(x => x.DegreeTypeId == degreeType.DegreeTypeId).SingleOrDefault();

                    if (degreeTypeInDb != null)
                    {
                        clientDbContext.DdlDegreeTypes.Remove(degreeTypeInDb);

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

                return Json(new[] { degreeType }.ToDataSourceResult(request, ModelState));
            }
        }

        public JsonResult GetDegreeTypes(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var degreeTypes = clientDbContext.DdlDegreeTypes
                    .Where(x => x.Active == true)
                    .Select(m => new
                    {
                        DegreeTypeId = m.DegreeTypeId,
                        DegreeDescription = m.Description
                    }).OrderBy(x => x.DegreeDescription).ToList();

                return Json(degreeTypes, JsonRequestBehavior.AllowGet);
            }

        }
    }
}