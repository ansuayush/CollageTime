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
    public class DdlTrainingCoursesController : Controller
    {
        // GET: DdlTrainingCourses
        public ActionResult DdlTrainingCoursesListMaintenance()
        {
            return View();
        }

        public ActionResult DdlTrainingCoursesList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var trainingCourseList = clientDbContext.DdlTrainingCourses.OrderBy(e => e.Description).ToList();
                return Json(trainingCourseList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlTrainingCoursesList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlTrainingCours trainingCourse)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (trainingCourse != null && ModelState.IsValid)
                {
                    var trainingCourseInDb = clientDbContext.DdlTrainingCourses
                        .Where(x => x.Code == trainingCourse.Code)
                        .SingleOrDefault();

                    if (trainingCourseInDb != null)
                    {
                        ModelState.AddModelError("", "The training course is already defined.");
                    }
                    else
                    {
                        var newTrainingCourse = new DdlTrainingCours
                        {
                            Description = trainingCourse.Description,
                            Code = trainingCourse.Code,
                            Active = true
                        };

                        clientDbContext.DdlTrainingCourses.Add(newTrainingCourse);
                        clientDbContext.SaveChanges();
                        trainingCourse.TrainingCourseId = newTrainingCourse.TrainingCourseId;
                    }
                }

                return Json(new[] { trainingCourse }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlTrainingCoursesList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlTrainingCours trainingCourse)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (trainingCourse != null && ModelState.IsValid)
                {
                    var trainingCourseInDb = clientDbContext.DdlTrainingCourses
                        .Where(x => x.TrainingCourseId == trainingCourse.TrainingCourseId)
                        .SingleOrDefault();

                    if (trainingCourseInDb != null)
                    {
                        trainingCourseInDb.Code = trainingCourse.Code;
                        trainingCourseInDb.Description = trainingCourse.Description;
                        trainingCourseInDb.Active = trainingCourseInDb.Active;
                        clientDbContext.SaveChanges();
                    }
                }

                return Json(new[] { trainingCourse }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlTrainingCoursesList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlTrainingCours trainingCourse)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (trainingCourse != null)
                {
                    DdlTrainingCours trainingCourseInDb = clientDbContext.DdlTrainingCourses
                        .Where(x => x.TrainingCourseId == trainingCourse.TrainingCourseId).SingleOrDefault();

                    if (trainingCourseInDb != null)
                    {
                        clientDbContext.DdlTrainingCourses.Remove(trainingCourseInDb);

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

                return Json(new[] { trainingCourse }.ToDataSourceResult(request, ModelState));
            }
        }


        public JsonResult GetTrainingCourses(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var trainingCourses = clientDbContext.DdlTrainingCourses
                    .Where(x => x.Active == true)
                    .Select(m => new
                    {
                        TrainingCourseId = m.TrainingCourseId,
                        TrainingCourseDescription = m.Description,
                    }).OrderBy(x => x.TrainingCourseDescription).ToList();

                return Json(trainingCourses, JsonRequestBehavior.AllowGet);
            }

        }
    }
}