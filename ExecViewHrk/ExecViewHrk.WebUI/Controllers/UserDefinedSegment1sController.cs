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
    public class UserDefinedSegment1sController : Controller
    {
        // GET: Segment1s
        public ActionResult UserDefinedSegment1sMatrixPartial()
        {
            return View();
        }

        public ActionResult UserDefinedSegment1sList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var userDefinedSegment1sList = clientDbContext.UserDefinedSegment1s.OrderBy(e => e.UserDefinedSegment1Description).ToList();
                return Json(userDefinedSegment1sList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UserDefinedSegment1sList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.UserDefinedSegment1s userDefinedSegment1)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (userDefinedSegment1 != null && ModelState.IsValid)
                {
                    var userDefinedSegment1InDb = clientDbContext.UserDefinedSegment1s
                        .Where(x => x.UserDefinedSegment1Code == userDefinedSegment1.UserDefinedSegment1Code)
                        .SingleOrDefault();

                    if (userDefinedSegment1InDb != null)
                    {
                        ModelState.AddModelError("", "The Segment 1" + CustomErrorMessages.ERROR_ALREADY_DEFINED);
                    }
                    else
                    {
                        var newUserDefinedSegment1 = new UserDefinedSegment1s
                        {
                            UserDefinedSegment1Description = userDefinedSegment1.UserDefinedSegment1Description,
                            UserDefinedSegment1Code = userDefinedSegment1.UserDefinedSegment1Code,
                        };

                        clientDbContext.UserDefinedSegment1s.Add(newUserDefinedSegment1);
                        clientDbContext.SaveChanges();
                        userDefinedSegment1.UserDefinedSegment1Id = newUserDefinedSegment1.UserDefinedSegment1Id;
                    }
                }

                return Json(new[] { userDefinedSegment1 }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UserDefinedSegment1sList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.UserDefinedSegment1s userDefinedSegment1)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (userDefinedSegment1 != null && ModelState.IsValid)
                {
                    var userDefinedSegment1InDb = clientDbContext.UserDefinedSegment1s
                        .Where(x => x.UserDefinedSegment1Id == userDefinedSegment1.UserDefinedSegment1Id)
                        .SingleOrDefault();

                    if (userDefinedSegment1InDb != null)
                    {
                        userDefinedSegment1InDb.UserDefinedSegment1Code = userDefinedSegment1.UserDefinedSegment1Code;
                        userDefinedSegment1InDb.UserDefinedSegment1Description = userDefinedSegment1.UserDefinedSegment1Description;
                        clientDbContext.SaveChanges();
                    }
                }

                return Json(new[] { userDefinedSegment1 }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UserDefinedSegment1sList_Destroy([DataSourceRequest] DataSourceRequest request
            , UserDefinedSegment1s userDefinedSegment1)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (userDefinedSegment1 != null)
                {
                    UserDefinedSegment1s userDefinedSegment1InDb = clientDbContext.UserDefinedSegment1s
                        .Where(x => x.UserDefinedSegment1Id == userDefinedSegment1.UserDefinedSegment1Id).SingleOrDefault();

                    if (userDefinedSegment1InDb != null)
                    {
                        clientDbContext.UserDefinedSegment1s.Remove(userDefinedSegment1InDb);

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

                return Json(new[] { userDefinedSegment1 }.ToDataSourceResult(request, ModelState));
            }
        }

        public JsonResult GetUserDefinedSegment1s(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var userDefinedSegment1s = clientDbContext.UserDefinedSegment1s
                    //.Where(x => x.IsUserDefinedSegment1Active == true)
                    .Select(m => new
                    {
                        UserDefinedSegment1Id = m.UserDefinedSegment1Id,
                        UserDefinedSegment1Description = m.UserDefinedSegment1Description
                    }).OrderBy(x => x.UserDefinedSegment1Description).ToList();

                return Json(userDefinedSegment1s, JsonRequestBehavior.AllowGet);
            }

        }
    }
}