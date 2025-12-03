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
    public class DdlInnoculationTypesController : Controller
    {
        // GET: DdlInnoculationType
        public ActionResult DdlInnoculationTypesListMaintenance()
        {
            return View();
        }

        public ActionResult DdlInnoculationTypesList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var innoculationTypeList = clientDbContext.DdlInnoculationTypes.OrderBy(e => e.Description).ToList();
                return Json(innoculationTypeList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlInnoculationTypesList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlInnoculationType innoculationType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (innoculationType != null && ModelState.IsValid)
                {
                    var innoculationTypeInDb = clientDbContext.DdlInnoculationTypes
                        .Where(x => x.Code.Trim() == innoculationType.Code.Trim() || x.Description.Trim() == innoculationType.Description.Trim())
                        .SingleOrDefault();

                    if (innoculationTypeInDb != null)
                    {
                        ModelState.AddModelError("", "The Innoculation Type"+ CustomErrorMessages.ERROR_ALREADY_DEFINED);
                    }
                    else
                    {
                        var newInnoculationType = new DdlInnoculationType
                        {
                            Description = innoculationType.Description,
                            Code = innoculationType.Code,
                            Active = innoculationType.Active
                        };

                        clientDbContext.DdlInnoculationTypes.Add(newInnoculationType);
                        clientDbContext.SaveChanges();
                        innoculationType.InnoculationTypeId = newInnoculationType.InnoculationTypeId;
                    }
                }

                return Json(new[] { innoculationType }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlInnoculationTypesList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlInnoculationType innoculationType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (innoculationType != null && ModelState.IsValid)
                {
                    var duplicateInnoculationTypeInDb = clientDbContext.DdlInnoculationTypes
                    .Where(x => x.InnoculationTypeId != innoculationType.InnoculationTypeId && 
                            (x.Code.Trim() == innoculationType.Code.Trim() || x.Description.Trim() == innoculationType.Description.Trim()))
                    .SingleOrDefault();
                    if (duplicateInnoculationTypeInDb == null)
                    {
                        var innoculationTypeInDb = clientDbContext.DdlInnoculationTypes.Where(x => x.InnoculationTypeId == innoculationType.InnoculationTypeId).SingleOrDefault();

                        if (innoculationTypeInDb != null)
                        {
                            innoculationTypeInDb.Code = innoculationType.Code;
                            innoculationTypeInDb.Description = innoculationType.Description;
                            if (innoculationTypeInDb.Active != innoculationType.Active && innoculationType.Active == false)
                            {
                                if (clientDbContext.PersonInnoculations.Where(x => x.InnoculationTypeId == innoculationType.InnoculationTypeId).Count() == 0)
                                {
                                    innoculationTypeInDb.Active = innoculationType.Active;
                                    clientDbContext.SaveChanges();
                                }
                                else
                                {
                                    ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                                }
                            }
                            else
                            {

                                innoculationTypeInDb.Active = innoculationType.Active;
                                clientDbContext.SaveChanges();
                            }

                        }
                    }
                    else {
                        ModelState.AddModelError("", "The Innoculation Type" + CustomErrorMessages.ERROR_ALREADY_DEFINED);
                    }
                }

                return Json(new[] { innoculationType }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlInnoculationTypesList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlInnoculationType innoculationType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (innoculationType != null)
                {
                    DdlInnoculationType innoculationTypeInDb = clientDbContext.DdlInnoculationTypes
                        .Where(x => x.InnoculationTypeId == innoculationType.InnoculationTypeId).SingleOrDefault();

                    if (innoculationTypeInDb != null)
                    {
                        clientDbContext.DdlInnoculationTypes.Remove(innoculationTypeInDb);

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

                return Json(new[] { innoculationType }.ToDataSourceResult(request, ModelState));
            }
        }


        public JsonResult GetInnoculationTypes(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var innoculationTypes = clientDbContext.DdlInnoculationTypes
                    .Where(x => x.Active == true)
                    .Select(m => new
                    {
                        InnoculationTypeId = m.InnoculationTypeId,
                        InnoculationDescription = m.Description
                    }).OrderBy(x => x.InnoculationDescription).ToList();

                return Json(innoculationTypes, JsonRequestBehavior.AllowGet);
            }

        }
    }
}