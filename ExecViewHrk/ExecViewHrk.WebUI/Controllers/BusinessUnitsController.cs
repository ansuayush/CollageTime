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
using ExecViewHrk.WebUI.Models;
using ExecViewHrk.WebUI.Helpers;
namespace ExecViewHrk.WebUI.Controllers
{
    public class BusinessUnitsController : Controller
    {
        // GET: BusinessUnits
        public ActionResult BusinessUnitsMatrixPartial()
        {
            return View();
        }

        public ActionResult BusinessUnitsList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var businessUnitsList = clientDbContext.BusinessUnits.OrderBy(e => e.BusinessUnitDescription).ToList();
                return Json(businessUnitsList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BusinessUnitsList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.BusinessUnit businessUnit)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (businessUnit != null && ModelState.IsValid)
                {
                    var businessUnitInDb = clientDbContext.BusinessUnits
                        .Where(x => x.BusinessUnitCode == businessUnit.BusinessUnitCode)
                        .SingleOrDefault();

                    if (businessUnitInDb != null)
                    {
                        ModelState.AddModelError("", "The BusinessUnit" + CustomErrorMessages.ERROR_ALREADY_DEFINED);
                    }
                    else
                    {
                        var newBusinessUnit = new BusinessUnit
                        {
                            BusinessUnitDescription = businessUnit.BusinessUnitDescription,
                            BusinessUnitCode = businessUnit.BusinessUnitCode,
                        };

                        clientDbContext.BusinessUnits.Add(newBusinessUnit);
                        clientDbContext.SaveChanges();
                        businessUnit.BusinessUnitId = newBusinessUnit.BusinessUnitId;
                    }
                }

                return Json(new[] { businessUnit }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BusinessUnitsList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.BusinessUnit businessUnit)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (businessUnit != null && ModelState.IsValid)
                {
                    var businessUnitInDb = clientDbContext.BusinessUnits
                        .Where(x => x.BusinessUnitId == businessUnit.BusinessUnitId)
                        .SingleOrDefault();

                    if (businessUnitInDb != null)
                    {
                        businessUnitInDb.BusinessUnitCode = businessUnit.BusinessUnitCode;
                        businessUnitInDb.BusinessUnitDescription = businessUnit.BusinessUnitDescription;
                        clientDbContext.SaveChanges();
                    }
                }

                return Json(new[] { businessUnit }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BusinessUnitsList_Destroy([DataSourceRequest] DataSourceRequest request
            , BusinessUnit businessUnit)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (businessUnit != null)
                {
                    BusinessUnit businessUnitInDb = clientDbContext.BusinessUnits
                        .Where(x => x.BusinessUnitId == businessUnit.BusinessUnitId).SingleOrDefault();

                    if (businessUnitInDb != null)
                    {
                        clientDbContext.BusinessUnits.Remove(businessUnitInDb);

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

                return Json(new[] { businessUnit }.ToDataSourceResult(request, ModelState));
            }
        }

        public JsonResult GetBusinessUnits(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var businessUnits = clientDbContext.BusinessUnits
                    //.Where(x => x.IsBusinessUnitActive == true)
                    .Select(m => new
                    {
                        BusinessUnitId = m.BusinessUnitId,
                        BusinessUnitDescription = m.BusinessUnitDescription
                    }).OrderBy(x => x.BusinessUnitDescription).ToList();

                return Json(businessUnits, JsonRequestBehavior.AllowGet);
            }

        }


    }
}