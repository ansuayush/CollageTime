using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    public class DdlGLCodesController : Controller
    {
        // GET: DdlGLCodes
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DdlGLCodesListMaintenance()
        {
            return View();
        }
        public ActionResult DdlGLCodesList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var GlcodeList = clientDbContext.DdlGLCodes.OrderBy(e => e.Description).ToList();
                return Json(GlcodeList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlGLCodesList_Create([DataSourceRequest] DataSourceRequest request
            , DdlGLCodes ddlgltype)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (ddlgltype != null && ModelState.IsValid)
                {
                    var ddlgltypeInDb = clientDbContext.DdlGLCodes
                        .Where(x => x.Code == ddlgltype.Code)
                        .SingleOrDefault();

                    if (ddlgltypeInDb != null)
                    {
                        ModelState.AddModelError("", "The GL TYPE is already defined.");
                    }
                    else
                    {
                        var newddlGLcode = new DdlGLCodes
                        {
                            Description = ddlgltype.Description,
                            Code = ddlgltype.Code,
                            Active = ddlgltype.Active
                        };

                        clientDbContext.DdlGLCodes.Add(newddlGLcode);
                        clientDbContext.SaveChanges();
                        ddlgltype.GLCodeId = newddlGLcode.GLCodeId;
                        ddlgltype.Code = newddlGLcode.Code;
                        ddlgltype.Active = newddlGLcode.Active;
                        ddlgltype.Description = newddlGLcode.Description;
                    }
                }

                return Json(new[] { ddlgltype }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlGLCodesList_Update([DataSourceRequest] DataSourceRequest request
            , DdlGLCodes ddlgltype)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (ddlgltype != null && ModelState.IsValid)
                {
                    var ddlgltypeInDb = clientDbContext.DdlGLCodes
                        .Where(x => x.GLCodeId != ddlgltype.GLCodeId && (x.Code == ddlgltype.Code || x.Description == ddlgltype.Description))
                        .SingleOrDefault();
                    if (ddlgltypeInDb == null)
                    {
                        ddlgltypeInDb = clientDbContext.DdlGLCodes.Where(x => x.GLCodeId == ddlgltype.GLCodeId).SingleOrDefault();

                        if (ddlgltypeInDb.Active != ddlgltype.Active && ddlgltype.Active == false)
                        {
                            if (clientDbContext.Contracts.Where(x => x.GLCodeId == ddlgltype.GLCodeId).Count() == 0)
                            {
                                ddlgltypeInDb.Code = ddlgltype.Code;
                                ddlgltypeInDb.Description = ddlgltype.Description;
                                ddlgltypeInDb.Active = ddlgltype.Active;
                                clientDbContext.SaveChanges();
                            }
                            else
                            {
                                ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                            }
                        }
                        else
                        {
                            ddlgltypeInDb.Code = ddlgltype.Code;
                            ddlgltypeInDb.Description = ddlgltype.Description;
                            ddlgltypeInDb.Active = ddlgltype.Active;
                            clientDbContext.SaveChanges();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", string.Format(CustomErrorMessages.ERROR_LOOKUP_DUPLICATE_RECORD, "GLCodes"));
                    }
                }
                    return Json(new[] { ddlgltype }.ToDataSourceResult(request, ModelState));
                }
            }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlGLCodesList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlGLCodes ddlgltype)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (ddlgltype != null)
                {
                    DdlGLCodes ddlgltypeInDb = clientDbContext.DdlGLCodes
                        .Where(x => x.GLCodeId == ddlgltype.GLCodeId).SingleOrDefault();
                    if (clientDbContext.Contracts.Where(x => x.GLCodeId == ddlgltype.GLCodeId).Count() > 0)
                    {
                        ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                    }
                    else
                    {
                        if (ddlgltypeInDb != null)
                        {
                            clientDbContext.DdlGLCodes.Remove(ddlgltypeInDb);

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
                }
            }
            return Json(new[] { ddlgltype }.ToDataSourceResult(request, ModelState));
        }
    }
}