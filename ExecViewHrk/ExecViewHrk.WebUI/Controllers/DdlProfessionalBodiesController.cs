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
    public class DdlProfessionalBodiesController : Controller
    {
        // GET: DdlProfessionalBodies
        public ActionResult DdlProfessionalBodiesListMaintenance()
        {
            return View();
        }

        public ActionResult DdlProfessionalBodiesList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var professionalBodyList = clientDbContext.DdlProfessionalBodies.OrderBy(e => e.Description).ToList();
                return Json(professionalBodyList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlProfessionalBodiesList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlProfessionalBody professionalBody)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (professionalBody != null && ModelState.IsValid)
                {
                    var professionalBodyInDb = clientDbContext.DdlProfessionalBodies
                        .Where(x => x.Code == professionalBody.Code)
                        .SingleOrDefault();
                    var professionalBodyDescInDb = clientDbContext.DdlProfessionalBodies.ToList();

                    var professionalbodylist = clientDbContext.DdlProfessionalBodies.Where(n => n.ProfessionalBodyId != professionalBody.ProfessionalBodyId).ToList();
                    if (professionalBodyInDb != null || professionalBodyDescInDb.Select(x => x.Description == professionalBody.Description) != null)
                    {
                        if (professionalbodylist.Select(m => m.Code).Contains(professionalBody.Code.Trim()) || professionalbodylist.Select(s => s.Description).Contains(professionalBody.Description.Trim()))
                        {
                            ModelState.AddModelError("", "The Professional Body is already exists!");
                        }
                        else
                        {
                            var newProfessionalBody = new DdlProfessionalBody
                            {
                                Description = professionalBody.Description,
                                Code = professionalBody.Code,
                                Active = professionalBody.Active,
                                WebAddress = professionalBody.WebAddress
                            };

                            clientDbContext.DdlProfessionalBodies.Add(newProfessionalBody);
                            clientDbContext.SaveChanges();
                            professionalBody.ProfessionalBodyId = newProfessionalBody.ProfessionalBodyId;
                        }
                    }
                }

                return Json(new[] { professionalBody }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlProfessionalBodiesList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlProfessionalBody professionalBody)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (professionalBody != null && ModelState.IsValid)
                {
                    var professionalBodyInDb = clientDbContext.DdlProfessionalBodies
                        .Where(x => x.ProfessionalBodyId == professionalBody.ProfessionalBodyId)
                        .SingleOrDefault();
                    var professionalbodylist = clientDbContext.DdlProfessionalBodies.Where(n => n.ProfessionalBodyId != professionalBody.ProfessionalBodyId).ToList();
                    var usedmemberlist = clientDbContext.PersonMemberships.ToList();
                    var active = usedmemberlist.Where(c => c.DdlProfessionalBody.ProfessionalBodyId == professionalBody.ProfessionalBodyId).Select(m => m.DdlProfessionalBody.Active).SingleOrDefault();

                    if (professionalBodyInDb != null)
                    {
                        if (professionalbodylist.Select(m => m.Code).Contains(professionalBody.Code.Trim()) || professionalbodylist.Select(s => s.Description).Contains(professionalBody.Description.Trim()))
                        {
                            ModelState.AddModelError("", "The Professional Body is already exists!");
                        }
                        else if (usedmemberlist.Select(s => s.DdlProfessionalBody.Description).Contains(professionalBody.Description) && active != professionalBody.Active)
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                        }
                        else
                        {
                            professionalBodyInDb.Code = professionalBody.Code;
                            professionalBodyInDb.Description = professionalBody.Description;
                            professionalBodyInDb.Active = professionalBody.Active;
                            professionalBodyInDb.WebAddress = professionalBody.WebAddress;
                            clientDbContext.SaveChanges();
                        }
                    }
                }

                return Json(new[] { professionalBody }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlProfessionalBodiesList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlProfessionalBody professionalBody)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (professionalBody != null)
                {
                    DdlProfessionalBody professionalBodyInDb = clientDbContext.DdlProfessionalBodies
                        .Where(x => x.ProfessionalBodyId == professionalBody.ProfessionalBodyId).SingleOrDefault();

                    if (professionalBodyInDb != null)
                    {
                        clientDbContext.DdlProfessionalBodies.Remove(professionalBodyInDb);

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

                return Json(new[] { professionalBody }.ToDataSourceResult(request, ModelState));
            }
        }

        public JsonResult GetProfessionalBodies()
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var professionalBodies = clientDbContext.DdlProfessionalBodies.Where(x => x.Active == true).OrderBy(x => x.Description).ToList();

                return Json(professionalBodies, JsonRequestBehavior.AllowGet);
            }

        }
    }
}