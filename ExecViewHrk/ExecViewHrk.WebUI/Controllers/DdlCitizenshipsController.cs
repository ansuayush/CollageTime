using ExecViewHrk.WebUI.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExecViewHrk.EfAdmin;
using ExecViewHrk.EfClient;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Net;
using ExecViewHrk.WebUI.Helpers;

namespace ExecViewHrk.WebUI.Controllers
{
    public class DdlCitizenshipsController : Controller
    {
        // GET: DdlCitizenships
        public ActionResult DdlCitizenshipsListMaintenance()
        {
            return View();
        }

        public ActionResult DdlCitizenshipsList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ExecViewHrk.EfClient.ClientDbContext clientDbContext = new ExecViewHrk.EfClient.ClientDbContext(connString))
            {
                var citizenshipsList = clientDbContext.DdlCitizenships.OrderBy(e => e.Description).ToList();
                return Json(citizenshipsList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlCitizenshipsList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlCitizenship citizenship)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (citizenship != null && ModelState.IsValid)
                {
                    var CitizenshipInDb = clientDbContext.DdlCitizenships
                        .Where(x => x.Code == citizenship.Code)
                        .SingleOrDefault();

                    if (CitizenshipInDb != null)
                    {
                        ModelState.AddModelError("", "The citizenship is already exists.");
                    }
                    else
                    {
                        var newCitizenship = new DdlCitizenship
                        {
                            Description = citizenship.Description,
                            Code = citizenship.Code,
                            Active = citizenship.Active
                        };

                        clientDbContext.DdlCitizenships.Add(newCitizenship);
                        clientDbContext.SaveChanges();
                        citizenship.CitizenshipId = newCitizenship.CitizenshipId;
                    }
                }

                return Json(new[] { citizenship }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlCitizenshipsList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlCitizenship citizenship)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (citizenship != null && ModelState.IsValid)
                {
                    var citizenshipInDb = clientDbContext.DdlCitizenships
                        .Where(x => x.CitizenshipId == citizenship.CitizenshipId)
                        .SingleOrDefault();
                    var CitizenshipIsDefined = clientDbContext.DdlCitizenships
                       .Where(x => x.Code == citizenship.Code &&  x.CitizenshipId != citizenship.CitizenshipId)
                       .SingleOrDefault();

                    if (CitizenshipIsDefined != null)
                    {
                        ModelState.AddModelError(string.Empty, "The citizenship is already exists.");
                        return Json(new[] { citizenship }.ToDataSourceResult(request, ModelState));
                    }
                    List<PersonAdditional> personAdditionalVm = personAdditionalVm = clientDbContext.PersonAdditionals.Where(x => x.CitizenshipId == citizenship.CitizenshipId ).ToList();
                    if(personAdditionalVm .Count > 0  && !citizenship.Active)
                    {
                        ModelState.AddModelError(string.Empty, "Can not be Inactive due to record is in Use.");
                        return Json(new[] { citizenship }.ToDataSourceResult(request, ModelState));

                    }
                    else
                    {
                        if (citizenshipInDb != null)
                        {
                            citizenshipInDb.Code = citizenship.Code;
                            citizenshipInDb.Description = citizenship.Description;
                            citizenshipInDb.Active = citizenship.Active;
                            clientDbContext.SaveChanges();
                        }
                    }
                   
                }

                return Json(new[] { citizenship }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlCitizenshipsList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlCitizenship citizenship)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (citizenship != null)
                {
                    DdlCitizenship citizenshipInDb = clientDbContext.DdlCitizenships
                        .Where(x => x.CitizenshipId == citizenship.CitizenshipId).SingleOrDefault();

                    if (citizenshipInDb != null)
                    {
                        clientDbContext.DdlCitizenships.Remove(citizenshipInDb);

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

                return Json(new[] { citizenship }.ToDataSourceResult(request, ModelState));
            }
        }

        public JsonResult GetCitizenships(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var citizenship = clientDbContext.DdlCitizenships
                    .Where(x => x.Active == true)
                    .Select(m => new
                    {
                        CitizenshipId = m.CitizenshipId,
                        CitizenshipDescription = m.Description
                    }).OrderBy(x => x.CitizenshipDescription).ToList();

                return Json(citizenship, JsonRequestBehavior.AllowGet);
            }

        }
    }
}