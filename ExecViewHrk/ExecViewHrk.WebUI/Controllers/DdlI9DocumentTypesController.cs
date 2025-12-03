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
    public class DdlI9DocumentTypesController : Controller
    {
        // GET: DdlI9DocumentTypes
        public ActionResult DdlI9DocumentTypesListMaintenance()
        {
            return View();
        }

        public ActionResult DdlI9DocumentTypesList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var i9DocumentTypesList = clientDbContext.DdlI9DocumentTypes.OrderBy(e => e.Description).ToList();
                return Json(i9DocumentTypesList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlI9DocumentTypesList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlI9DocumentTypes i9DocumentType)  //DdlI9DocumentTypes property is auto geneterated instead of DdlI9DocumentType
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (i9DocumentType != null && ModelState.IsValid)
                {
                    var i9DocumentTypeInDb = clientDbContext.DdlI9DocumentTypes
                        .Where(x => x.Code == i9DocumentType.Code)
                        .SingleOrDefault();

                    if (i9DocumentTypeInDb != null)
                    {
                        ModelState.AddModelError("", "The I9 Document type" + CustomErrorMessages.ERROR_ALREADY_DEFINED);
                    }
                    else
                    {
                        var newI9DocumentType = new DdlI9DocumentTypes
                        {
                            Description = i9DocumentType.Description,
                            Code = i9DocumentType.Code,
                            Active = true
                        };

                        clientDbContext.DdlI9DocumentTypes.Add(newI9DocumentType);
                        clientDbContext.SaveChanges();
                        i9DocumentType.I9DocumentTypeId = newI9DocumentType.I9DocumentTypeId;
                    }
                }

                return Json(new[] { i9DocumentType }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlI9DocumentTypesList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlI9DocumentTypes i9DocumentType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (i9DocumentType != null && ModelState.IsValid)
                {
                    var i9DocumentTypeInDb = clientDbContext.DdlI9DocumentTypes
                        .Where(x => x.I9DocumentTypeId == i9DocumentType.I9DocumentTypeId)
                        .SingleOrDefault();

                    var evaluationIsDefined = clientDbContext.DdlI9DocumentTypes
                     .Where(x => x.Code == i9DocumentType.Code  && x.I9DocumentTypeId != i9DocumentType.I9DocumentTypeId)
                     .SingleOrDefault();

                    if (evaluationIsDefined != null)
                    {
                        ModelState.AddModelError(string.Empty, "The I9 document already exists.");
                        return Json(new[] { i9DocumentType }.ToDataSourceResult(request, ModelState));
                    }
                    List<EmployeeI9Documents> i9DocumentTypes = clientDbContext.EmployeeI9Documents.Where(x => x.I9DocumentTypeId == i9DocumentType.I9DocumentTypeId).ToList();
                    if (i9DocumentTypes.Count > 0 && !i9DocumentType.Active)
                    {
                        ModelState.AddModelError(string.Empty, "Can not be Inactive due to record is in Use.");
                        return Json(new[] { i9DocumentType }.ToDataSourceResult(request, ModelState));

                    }

                    if (i9DocumentTypeInDb != null)
                    {
                        i9DocumentTypeInDb.Code = i9DocumentType.Code;
                        i9DocumentTypeInDb.Description = i9DocumentType.Description;
                        i9DocumentTypeInDb.Active = i9DocumentType.Active;
                        clientDbContext.SaveChanges();
                    }
                }

                return Json(new[] { i9DocumentType }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlI9DocumentTypesList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlI9DocumentTypes i9DocumentType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (i9DocumentType != null)
                {
                    DdlI9DocumentTypes i9DocumentTypeInDb = clientDbContext.DdlI9DocumentTypes
                        .Where(x => x.I9DocumentTypeId == i9DocumentType.I9DocumentTypeId).SingleOrDefault();

                    if (i9DocumentTypeInDb != null)
                    {
                        clientDbContext.DdlI9DocumentTypes.Remove(i9DocumentTypeInDb);

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

                return Json(new[] { i9DocumentType }.ToDataSourceResult(request, ModelState));
            }
        }

        public JsonResult GetI9DocumentTypes(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var i9DocumentTypes = clientDbContext.DdlI9DocumentTypes
                    .Where(x => x.Active == true)
                    .Select(m => new
                    {
                        I9DocumentTypeId = m.I9DocumentTypeId,
                        I9DocumentDescription = m.Description
                    }).OrderBy(x => x.I9DocumentDescription).ToList();

                return Json(i9DocumentTypes, JsonRequestBehavior.AllowGet);
            }

        }
    }
}