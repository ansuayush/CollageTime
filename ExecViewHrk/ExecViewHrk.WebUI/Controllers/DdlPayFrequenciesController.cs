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
    public class DdlPayFrequenciesController : Controller
    {
        // GET: DdlPayFrequencies
        public ActionResult DdlPayFrequenciesListMaintenance()
        {
            return View();
        }

        public ActionResult DdlPayFrequenciesList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var payFrequencyList = clientDbContext.DdlPayFrequencies.OrderBy(e => e.Description).ToList();
                return Json(payFrequencyList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlPayFrequenciesList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlPayFrequency payFrequency)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (payFrequency != null && ModelState.IsValid)
                {
                    var payFrequencyInDb = clientDbContext.DdlPayFrequencies.Where(x => x.Code == payFrequency.Code || x.Description == payFrequency.Description).SingleOrDefault();

                    if (payFrequencyInDb != null)
                    {
                        ModelState.AddModelError("", "The Pay Frequency" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                    }
                    else
                    {
                        var newPayFrequency = new DdlPayFrequency
                        {
                            Description = payFrequency.Description,
                            Code = payFrequency.Code,
                            Active = payFrequency.Active
                        };

                        clientDbContext.DdlPayFrequencies.Add(newPayFrequency);
                        clientDbContext.SaveChanges();
                        payFrequency.PayFrequencyId = newPayFrequency.PayFrequencyId;
                    }
                }

                return Json(new[] { payFrequency }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlPayFrequenciesList_Update([DataSourceRequest] DataSourceRequest request, DdlPayFrequency payFrequency)
        {
            if (payFrequency != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var payFrequencyInDb = clientDbContext.DdlPayFrequencies.Where(x => x.PayFrequencyId != payFrequency.PayFrequencyId && (x.Code == payFrequency.Code || x.Description == payFrequency.Description)).SingleOrDefault();
                if (payFrequencyInDb == null)
                {
                    payFrequencyInDb = clientDbContext.DdlPayFrequencies.Where(x => x.PayFrequencyId == payFrequency.PayFrequencyId).SingleOrDefault();

                    if (payFrequencyInDb.Active != payFrequency.Active && payFrequency.Active == false)
                    {
                        if (clientDbContext.E_Positions.Where(x => x.PayFrequencyId == payFrequency.PayFrequencyId).Count() == 0)
                        {
                            payFrequencyInDb.Code = payFrequency.Code;
                            payFrequencyInDb.Description = payFrequency.Description;
                            payFrequencyInDb.Active = payFrequency.Active;
                            clientDbContext.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                        }
                    }
                    else
                    {
                        payFrequencyInDb.Code = payFrequency.Code;
                        payFrequencyInDb.Description = payFrequency.Description;
                        payFrequencyInDb.Active = payFrequency.Active;
                        clientDbContext.SaveChanges();
                    }
                }
                else
                {
                    ModelState.AddModelError("", string.Format(CustomErrorMessages.ERROR_LOOKUP_DUPLICATE_RECORD, "Suffix"));
                }
            }


            return Json(new[] { payFrequency }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlPayFrequenciesList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlPayFrequency payFrequency)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (payFrequency != null)
                {
                    DdlPayFrequency payFrequencyInDb = clientDbContext.DdlPayFrequencies
                        .Where(x => x.PayFrequencyId == payFrequency.PayFrequencyId).SingleOrDefault();

                    if (payFrequencyInDb != null)
                    {
                        clientDbContext.DdlPayFrequencies.Remove(payFrequencyInDb);

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

                return Json(new[] { payFrequency }.ToDataSourceResult(request, ModelState));
            }
        }


        public JsonResult GetPayFrequencies()
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var payFrequencyTypes = clientDbContext.DdlPayFrequencies.Where(x => x.Active == true).ToList();

                return Json(payFrequencyTypes, JsonRequestBehavior.AllowGet);
            }

        }

    }
}