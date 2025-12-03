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
    public class DdlEeoTypesController : Controller
    {
        // GET: DdlEeoTypes
        public ActionResult DdlEeoTypesListMaintenance()
        {
            return View();
        }

        public ActionResult DdlEeoTypesList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var eeoTypeList = clientDbContext.DdlEeoTypes.OrderBy(e => e.Description).ToList();
                return Json(eeoTypeList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlEeoTypesList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlEeoType eeoType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (eeoType != null && ModelState.IsValid)
                {
                    var EEOTypeInDb = clientDbContext.DdlEeoTypes
                       .Where(x => x.Code == eeoType.Code || x.Description == eeoType.Description)
                       .SingleOrDefault();
                  
                    if (EEOTypeInDb != null )
                    {
                        ModelState.AddModelError("", "The EEO Type with same code or description already defined.");
                    }
                    else
                    {
                        var newEEOType = new DdlEeoType
                        {
                            Description = eeoType.Description,
                            Code = eeoType.Code,
                            Active = eeoType.Active
                        };

                        clientDbContext.DdlEeoTypes.Add(newEEOType);
                        clientDbContext.SaveChanges();
                        eeoType.EeoTypeId = newEEOType.EeoTypeId;
                    }
                }

                return Json(new[] { eeoType }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlEeoTypesList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlEeoType eeoType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (eeoType != null && ModelState.IsValid)
                {
                    var EEOTypeInDb = clientDbContext.DdlEeoTypes.Where(x => x.EeoTypeId != eeoType.EeoTypeId && (x.Code == eeoType.Code || x.Description == eeoType.Description)).SingleOrDefault();
                    if (EEOTypeInDb == null)
                    {
                        EEOTypeInDb = clientDbContext.DdlEeoTypes.Where(x => x.EeoTypeId == eeoType.EeoTypeId).SingleOrDefault();

                        if (EEOTypeInDb.Active != eeoType.Active && eeoType.Active == false)
                        {
                            if (clientDbContext.PersonAdditionals.Where(x => x.EeoTypeId == eeoType.EeoTypeId).Count() == 0)
                            {
                                EEOTypeInDb.Code = eeoType.Code;
                                EEOTypeInDb.Description = eeoType.Description;
                                EEOTypeInDb.Active = eeoType.Active;
                                clientDbContext.SaveChanges();
                            }
                            else
                            {
                                ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                            }
                        }
                        else
                        {
                            EEOTypeInDb.Code = eeoType.Code;
                            EEOTypeInDb.Description = eeoType.Description;
                            EEOTypeInDb.Active = eeoType.Active;
                            clientDbContext.SaveChanges();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", string.Format(CustomErrorMessages.ERROR_LOOKUP_DUPLICATE_RECORD, "EEO Type"));
                    }
                }

                return Json(new[] { eeoType }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlEeoTypesList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlEeoType eeoType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (eeoType != null)
                {
                    DdlEeoType eeoTypeInDb = clientDbContext.DdlEeoTypes
                        .Where(x => x.EeoTypeId == eeoType.EeoTypeId).SingleOrDefault();

                    if (eeoTypeInDb != null)
                    {
                        clientDbContext.DdlEeoTypes.Remove(eeoTypeInDb);

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

                return Json(new[] { eeoType }.ToDataSourceResult(request, ModelState));
            }
        }


        public JsonResult GetEeoTypes(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var eeoTypes = clientDbContext.DdlEeoTypes
                    //.Where(x => x.Active == true)
                    .Select(m => new
                    {
                        EeoTypeId = m.EeoTypeId,
                        EeoDescription = m.Description
                    }).OrderBy(x => x.EeoDescription).ToList();

                return Json(eeoTypes, JsonRequestBehavior.AllowGet);
            }

        }
    }
}