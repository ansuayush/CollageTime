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
    public class DdlEducationTypesController : Controller
    {
        // GET: DdlEducationTypes
        public ActionResult DdlEducationTypesListMaintenance()
        {
            return View();
        }

        public ActionResult DdlEducationTypesList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var educationTypesList = clientDbContext.DdlEducationTypes.OrderBy(e => e.Description).ToList();
                return Json(educationTypesList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlEducationTypesList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlEducationType educationType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (educationType != null && ModelState.IsValid)
                {
                    var EducationTypeInDb = clientDbContext.DdlEducationTypes
                      .Where(x => x.Code == educationType.Code || x.Description == educationType.Description)
                      .SingleOrDefault();
                 
                    if (EducationTypeInDb != null )
                    {
                        ModelState.AddModelError("", "The Education Type with same code or description already defined.");
                    }
                    else
                    {
                        var newEducationType = new DdlEducationType
                        {
                            Description = educationType.Description,
                            Code = educationType.Code,
                            Active = educationType.Active
                        };

                        clientDbContext.DdlEducationTypes.Add(newEducationType);
                        clientDbContext.SaveChanges();
                        educationType.EducationTypeId = newEducationType.EducationTypeId;
                    }
                }

                return Json(new[] { educationType }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlEducationTypesList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlEducationType educationType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (educationType != null && ModelState.IsValid)
                {
                    var EducationTypeInDb = clientDbContext.DdlEducationTypes.Where(x => x.EducationTypeId != educationType.EducationTypeId && (x.Code == educationType.Code || x.Description == educationType.Description)).SingleOrDefault();
                    if (EducationTypeInDb == null)
                    {
                        EducationTypeInDb = clientDbContext.DdlEducationTypes.Where(x => x.EducationTypeId == educationType.EducationTypeId).SingleOrDefault();

                        if (EducationTypeInDb.Active != educationType.Active && educationType.Active == false)
                        {
                            if (clientDbContext.DdlEducationEstablishments.Where(x => x.EducationTypeId == educationType.EducationTypeId).Count() == 0)
                            {
                                EducationTypeInDb.Code = educationType.Code;
                                EducationTypeInDb.Description = educationType.Description;
                                EducationTypeInDb.Active = educationType.Active;
                                clientDbContext.SaveChanges();
                            }
                            else
                            {
                                ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                            }
                        }
                        else
                        {
                            EducationTypeInDb.Code = educationType.Code;
                            EducationTypeInDb.Description = educationType.Description;
                            EducationTypeInDb.Active = educationType.Active;
                            clientDbContext.SaveChanges();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", string.Format(CustomErrorMessages.ERROR_LOOKUP_DUPLICATE_RECORD, "Eduaction Type"));
                    }
                }

                return Json(new[] { educationType }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlEducationTypesList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlEducationType educationType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (educationType != null)
                {
                    DdlEducationType educationTypeInDb = clientDbContext.DdlEducationTypes
                        .Where(x => x.EducationTypeId == educationType.EducationTypeId).SingleOrDefault();

                    if (educationTypeInDb != null)
                    {
                        clientDbContext.DdlEducationTypes.Remove(educationTypeInDb);

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

                return Json(new[] { educationType }.ToDataSourceResult(request, ModelState));
            }
        }

        public JsonResult GetEducationTypes(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var educationTypes = clientDbContext.DdlEducationTypes
                    .Where(x => x.Active == true)
                    .Select(m => new
                    {
                        EducationTypeId = m.EducationTypeId,
                        EducationDescription = m.Description
                    }).OrderBy(x => x.EducationDescription).ToList();

                return Json(educationTypes, JsonRequestBehavior.AllowGet);
            }

        }
    }
}