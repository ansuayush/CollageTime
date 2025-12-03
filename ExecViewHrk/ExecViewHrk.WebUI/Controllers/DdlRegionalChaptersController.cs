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
    public class DdlRegionalChaptersController : Controller
    {
        // GET: DdlRegionalChapters
        public ActionResult DdlRegionalChaptersListMaintenance()
        {
            return View();
        }

        public ActionResult DdlRegionalChaptersList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var regionalChapterList = clientDbContext.DdlRegionalChapters.OrderBy(e => e.Description).ToList();
                return Json(regionalChapterList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlRegionalChaptersList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlRegionalChapter regionalChapter)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (regionalChapter != null && ModelState.IsValid)
                {
                    var regionalChapterInDb = clientDbContext.DdlRegionalChapters
                        .Where(x => x.Code == regionalChapter.Code)
                        .SingleOrDefault();
                    var regionalChapterDescInDb = clientDbContext.DdlRegionalChapters.ToList();

                    var regionalbodylist = clientDbContext.DdlRegionalChapters.Where(n => n.RegionalChapterId != regionalChapter.RegionalChapterId).ToList();
                    if (regionalChapterInDb != null || regionalChapterDescInDb.Select(m => m.Description == regionalChapter.Description) != null)
                    {
                        if (regionalbodylist.Select(m => m.Code).Contains(regionalChapter.Code.Trim()) || regionalbodylist.Select(m => m.Description).Contains(regionalChapter.Description.Trim()))
                        {
                            ModelState.AddModelError("", "The Regional Chapter is already exists!");
                        }
                        else
                        {
                            var newregionalChapter = new DdlRegionalChapter
                            {
                                Description = regionalChapter.Description,
                                Code = regionalChapter.Code,
                                Active = regionalChapter.Active
                            };

                            clientDbContext.DdlRegionalChapters.Add(newregionalChapter);
                            clientDbContext.SaveChanges();
                            regionalChapter.RegionalChapterId = newregionalChapter.RegionalChapterId;
                        }
                    }
                }
                return Json(new[] { regionalChapter }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlRegionalChaptersList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlRegionalChapter regionalChapter)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (regionalChapter != null && ModelState.IsValid)
                {
                    var regionalChapterInDb = clientDbContext.DdlRegionalChapters
                        .Where(x => x.RegionalChapterId == regionalChapter.RegionalChapterId)
                        .SingleOrDefault();
                    var regionalbodylist = clientDbContext.DdlRegionalChapters.Where(n => n.RegionalChapterId != regionalChapter.RegionalChapterId).ToList();
                    var usedmemberlist = clientDbContext.PersonMemberships.ToList();
                    var active = regionalChapterInDb.Active;

                    if (regionalChapterInDb != null)
                    {
                        if (regionalbodylist.Select(m => m.Code).Contains(regionalChapter.Code.Trim()) || regionalbodylist.Select(m => m.Description).Contains(regionalChapter.Description.Trim()))
                        {
                            ModelState.AddModelError("", "The Regional Chapter is already exists!");
                        }
                        else if (usedmemberlist.Select(s => s.DdlRegionalChapter.Description).Contains(regionalChapter.Description) && active != regionalChapter.Active)
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                        }
                        else
                        {
                            regionalChapterInDb.Code = regionalChapter.Code;
                            regionalChapterInDb.Description = regionalChapter.Description;
                            regionalChapterInDb.Active = regionalChapter.Active;
                            clientDbContext.SaveChanges();
                        }
                    }
                }

                return Json(new[] { regionalChapter }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlRegionalChaptersList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlRegionalChapter regionalChapter)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (regionalChapter != null)
                {
                    DdlRegionalChapter regionalChapterInDb = clientDbContext.DdlRegionalChapters
                        .Where(x => x.RegionalChapterId == regionalChapter.RegionalChapterId).SingleOrDefault();

                    if (regionalChapterInDb != null)
                    {
                        clientDbContext.DdlRegionalChapters.Remove(regionalChapterInDb);

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

                return Json(new[] { regionalChapter }.ToDataSourceResult(request, ModelState));
            }
        }

        //[AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GetRegionalChapters()
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var regionalChapters = clientDbContext.DdlRegionalChapters.Where(x => x.Active == true).OrderBy(x => x.Description).ToList();

                return Json(regionalChapters, JsonRequestBehavior.AllowGet);
            }

        }
    }
}