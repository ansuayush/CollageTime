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
    public class DdlQualificationTypesController : Controller
    {
        // GET: DdlQualificationTypes
        public ActionResult DdlQualificationTypesListMaintenance()
        {
            return View();
        }

        public ActionResult DdlQualificationTypesList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var qualificationTypeList = clientDbContext.DdlQualificationTypes.OrderBy(e => e.Description).ToList();
                return Json(qualificationTypeList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlQualificationTypesList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlQualificationType qualificationType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (qualificationType != null && ModelState.IsValid)
                {
                    var qualificationTypeInDb = clientDbContext.DdlQualificationTypes
                        .Where(x => x.Code == qualificationType.Code)
                        .SingleOrDefault();
                    var qualificationtypelist = clientDbContext.DdlQualificationTypes.Where(n => n.QualificationTypeId != qualificationType.QualificationTypeId).ToList();
                    var qualificationTypeDescInDb = clientDbContext.DdlQualificationTypes.ToList();
                    var qualificationDesc = qualificationTypeDescInDb.Select(x => x.Description == qualificationType.Description);
                    if (qualificationTypeInDb != null || qualificationDesc != null)
                    {
                        if (qualificationtypelist.Select(m => m.Code).Contains(qualificationType.Code) || qualificationtypelist.Select(m => m.Description).Contains(qualificationType.Description))
                        {
                            ModelState.AddModelError("", "The Qualification Type Code or Description already exists!");
                        }
                        else
                        {
                            var newQualificationType = new DdlQualificationType
                            {
                                Description = qualificationType.Description,
                                Code = qualificationType.Code,
                                Active = qualificationType.Active,
                            };

                            clientDbContext.DdlQualificationTypes.Add(newQualificationType);
                            clientDbContext.SaveChanges();
                            qualificationType.QualificationTypeId = newQualificationType.QualificationTypeId;
                        }
                    }
                }

                return Json(new[] { qualificationType }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlQualificationTypesList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlQualificationType qualificationType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (qualificationType != null && ModelState.IsValid)
                {
                    var qualificationTypeInDb = clientDbContext.DdlQualificationTypes
                        .Where(x => x.QualificationTypeId == qualificationType.QualificationTypeId)
                        .SingleOrDefault();
                    var qualificationTypeDescInDb = clientDbContext.DdlQualificationTypes.ToList();

                    var qualificationTypelist = clientDbContext.DdlQualificationTypes.Where(n => n.QualificationTypeId != qualificationType.QualificationTypeId).ToList();
                    var rqualificationTypeDesc = qualificationTypeDescInDb.Select(x => x.Description == qualificationType.Description);

                    if (qualificationTypeInDb != null)
                    {
                        if (qualificationTypelist.Select(m => m.Code).Contains(qualificationType.Code) || qualificationTypelist.Select(m => m.Description).Contains(qualificationType.Description))
                        {
                            ModelState.AddModelError("", "The Qualification Type Code or Description already exists!");
                        }
                        else
                        {
                            qualificationTypeInDb.Code = qualificationType.Code;
                            qualificationTypeInDb.Description = qualificationType.Description;
                            qualificationTypeInDb.Active = qualificationType.Active;
                            clientDbContext.SaveChanges();
                        }
                    }
                }

                return Json(new[] { qualificationType }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlQualificationTypesList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlQualificationType qualificationType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (qualificationType != null)
                {
                    DdlQualificationType qualificationTypeInDb = clientDbContext.DdlQualificationTypes
                        .Where(x => x.QualificationTypeId == qualificationType.QualificationTypeId).SingleOrDefault();

                    if (qualificationTypeInDb != null)
                    {
                        clientDbContext.DdlQualificationTypes.Remove(qualificationTypeInDb);

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

                return Json(new[] { qualificationType }.ToDataSourceResult(request, ModelState));
            }
        }


        public JsonResult GetQualificationTypes(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var qualificationTypes = clientDbContext.DdlQualificationTypes
                    .Where(x => x.Active == true)
                    .Select(m => new
                    {
                        QualificationTypeId = m.QualificationTypeId,
                        QualificationDescription = m.Description
                    }).OrderBy(x => x.QualificationDescription).ToList();

                return Json(qualificationTypes, JsonRequestBehavior.AllowGet);
            }

        }
    }
}