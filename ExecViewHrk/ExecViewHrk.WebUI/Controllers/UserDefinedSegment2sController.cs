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
    public class UserDefinedSegment2sController : Controller
    {
        // GET: UserDefinedSegment2
        public ActionResult UserDefinedSegment2sMatrixPartial()
        {
            return View();
        }

        public ActionResult UserDefinedSegment2sList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var userDefinedSegment2sList = clientDbContext.UserDefinedSegment2s.OrderBy(e => e.UserDefinedSegment2Description).ToList();
                return Json(userDefinedSegment2sList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UserDefinedSegment2sList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.UserDefinedSegment2s userDefinedSegment2)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (userDefinedSegment2 != null && ModelState.IsValid)
                {
                    var userDefinedSegment2InDb = clientDbContext.UserDefinedSegment2s
                        .Where(x => x.UserDefinedSegment2Code == userDefinedSegment2.UserDefinedSegment2Code)
                        .SingleOrDefault();

                    if (userDefinedSegment2InDb != null)
                    {
                        ModelState.AddModelError("", "The Segment 2" + CustomErrorMessages.ERROR_ALREADY_DEFINED);
                    }
                    else
                    {
                        var newUserDefinedSegment2 = new UserDefinedSegment2s
                        {
                            UserDefinedSegment2Description = userDefinedSegment2.UserDefinedSegment2Description,
                            UserDefinedSegment2Code = userDefinedSegment2.UserDefinedSegment2Code,
                        };

                        clientDbContext.UserDefinedSegment2s.Add(newUserDefinedSegment2);
                        clientDbContext.SaveChanges();
                        userDefinedSegment2.UserDefinedSegment2Id = newUserDefinedSegment2.UserDefinedSegment2Id;
                    }
                }

                return Json(new[] { userDefinedSegment2 }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UserDefinedSegment2sList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.UserDefinedSegment2s userDefinedSegment2)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (userDefinedSegment2 != null && ModelState.IsValid)
                {
                    var userDefinedSegment2InDb = clientDbContext.UserDefinedSegment2s
                        .Where(x => x.UserDefinedSegment2Id == userDefinedSegment2.UserDefinedSegment2Id)
                        .SingleOrDefault();

                    if (userDefinedSegment2InDb != null)
                    {
                        userDefinedSegment2InDb.UserDefinedSegment2Code = userDefinedSegment2.UserDefinedSegment2Code;
                        userDefinedSegment2InDb.UserDefinedSegment2Description = userDefinedSegment2.UserDefinedSegment2Description;
                        clientDbContext.SaveChanges();
                    }
                }

                return Json(new[] { userDefinedSegment2 }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UserDefinedSegment2sList_Destroy([DataSourceRequest] DataSourceRequest request
            , UserDefinedSegment2s userDefinedSegment2)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (userDefinedSegment2 != null)
                {
                    UserDefinedSegment2s userDefinedSegment2InDb = clientDbContext.UserDefinedSegment2s
                        .Where(x => x.UserDefinedSegment2Id == userDefinedSegment2.UserDefinedSegment2Id).SingleOrDefault();

                    if (userDefinedSegment2InDb != null)
                    {
                        clientDbContext.UserDefinedSegment2s.Remove(userDefinedSegment2InDb);

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

                return Json(new[] { userDefinedSegment2 }.ToDataSourceResult(request, ModelState));
            }
        }

        public JsonResult GetUserDefinedSegment2s(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var userDefinedSegment2s = clientDbContext.UserDefinedSegment2s
                    //.Where(x => x.IsUserDefinedSegment2Active == true)
                    .Select(m => new
                    {
                        UserDefinedSegment2Id = m.UserDefinedSegment2Id,
                        UserDefinedSegment2Description = m.UserDefinedSegment2Description
                    }).OrderBy(x => x.UserDefinedSegment2Description).ToList();

                return Json(userDefinedSegment2s, JsonRequestBehavior.AllowGet);
            }
        }

    }
}