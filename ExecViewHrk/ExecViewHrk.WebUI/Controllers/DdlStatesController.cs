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
    public class DdlStatesController : Controller
    {
        // GET: DdlStates
        public ActionResult DdlStatesListMaintenance()
        {
            return View();
        }

        public ActionResult DdlStatesList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var stateList = clientDbContext.DdlStates.OrderBy(e => e.Title).ToList();
                return Json(stateList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlStatesList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlState state)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (state != null && ModelState.IsValid)
                {
                    var stateInDb = clientDbContext.DdlStates
                        .Where(x => x.Code == state.Code)
                        .SingleOrDefault();

                    if (stateInDb != null)
                    {
                        ModelState.AddModelError("", "The State" + CustomErrorMessages.ERROR_ALREADY_DEFINED);
                    }
                    else
                    {
                        var newState = new DdlState
                        {
                            Title = state.Title,
                            Code = state.Code,
                        };

                        clientDbContext.DdlStates.Add(newState);
                        clientDbContext.SaveChanges();
                        state.StateId = newState.StateId;
                    }
                }

                return Json(new[] { state }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlStatesList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlState state)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (state != null && ModelState.IsValid)
                {
                    var stateInDb = clientDbContext.DdlStates
                        .Where(x => x.StateId == state.StateId)
                        .SingleOrDefault();

                    if (stateInDb != null)
                    {
                        stateInDb.Code = state.Code;
                        stateInDb.Title = state.Title;
                        clientDbContext.SaveChanges();
                    }
                }

                return Json(new[] { state }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlStatesList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlState state)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (state != null)
                {
                    DdlState stateInDb = clientDbContext.DdlStates
                        .Where(x => x.StateId == state.StateId).SingleOrDefault();

                    if (stateInDb != null)
                    {
                        clientDbContext.DdlStates.Remove(stateInDb);

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

                return Json(new[] { state }.ToDataSourceResult(request, ModelState));
            }
        }


        public JsonResult GetStates(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var addressTypes = clientDbContext.DdlStates
                    .Select(m => new
                    {
                        StateId = m.StateId,
                        StateTitle = m.Title,
                    }).OrderBy(x => x.StateTitle).ToList();

                return Json(addressTypes, JsonRequestBehavior.AllowGet);
            }

        }
    }
}