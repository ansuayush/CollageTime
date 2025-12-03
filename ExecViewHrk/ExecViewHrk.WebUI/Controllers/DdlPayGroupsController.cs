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
    public class DdlPayGroupsController : Controller
    {
        
        public ActionResult DdlPayGroupsMain()
        {
            return View();
        }

        public ActionResult DdlPayGroupsList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var paygroupsList = clientDbContext.DdlPayGroups.OrderBy(e => e.Description).ToList();
                return Json(paygroupsList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlPayGroupsList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlPayGroup paygroup)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (paygroup != null && ModelState.IsValid)
                {
                    var paygroupInDb = clientDbContext.DdlPayGroups
                        .Where(x => x.Code == paygroup.Code)
                        .SingleOrDefault();

                    if (paygroupInDb != null)
                    {
                        ModelState.AddModelError("", "The Pay Group is already defined.");
                    }
                    else
                    {
                        var newpaygroup = new DdlPayGroup
                        {
                            Description = paygroup.Description,
                            Code = paygroup.Code,
                            Active = paygroup.Active
                        };

                        clientDbContext.DdlPayGroups.Add(newpaygroup);
                        clientDbContext.SaveChanges();
                        paygroup.PayGroupId = newpaygroup.PayGroupId;
                        paygroup.Code = newpaygroup.Code;
                        paygroup.Active = newpaygroup.Active;
                        paygroup.Description = newpaygroup.Description;
                    }
                }

                return Json(new[] { paygroup }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlPayGroupsList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlPayGroup paygroup)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (paygroup != null && ModelState.IsValid)
                {
                    var paygroupInDb = clientDbContext.DdlPayGroups
                        .Where(x => x.PayGroupId == paygroup.PayGroupId)
                        .SingleOrDefault();

                    if (paygroupInDb != null)
                    {
                        paygroupInDb.Code = paygroup.Code;
                        paygroupInDb.Description = paygroup.Description;
                        paygroupInDb.Active = paygroup.Active;
                        clientDbContext.SaveChanges();
                    }
                }

                return Json(new[] { paygroup }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlPayGroupsList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlPayGroup paygroup)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (paygroup != null)
                {
                    DdlPayGroup paygroupInDb = clientDbContext.DdlPayGroups
                        .Where(x => x.PayGroupId == paygroup.PayGroupId).SingleOrDefault();

                    if (paygroupInDb != null)
                    {
                        clientDbContext.DdlPayGroups.Remove(paygroupInDb);

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

                return Json(new[] { paygroup }.ToDataSourceResult(request, ModelState));
            }
        }
       
    }
}