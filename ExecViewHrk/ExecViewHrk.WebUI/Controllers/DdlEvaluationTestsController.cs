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
using ExecViewHrk.WebUI.Models;

namespace ExecViewHrk.WebUI.Controllers
{
    public class DdlEvaluationTestsController : Controller
    {
        // GET: DdlEvaluationTests
        public ActionResult DdlEvaluationTestsListMaintenance()
        {
              return View();
        }

        public ActionResult DdlEvaluationTestsList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var evaluationTestList = clientDbContext.DdlEvaluationTests.OrderBy(e => e.Description).ToList();
                return Json(evaluationTestList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlEvaluationTestsList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlEvaluationTest evaluationTest)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (evaluationTest != null && ModelState.IsValid)
                {
                    var evaluationTestInDb = clientDbContext.DdlEvaluationTests
                        .Where(x => x.Code == evaluationTest.Code)
                        .SingleOrDefault();

                    if (evaluationTestInDb != null)
                    {
                        ModelState.AddModelError("", "The Evaluation Test" + CustomErrorMessages.ERROR_ALREADY_DEFINED);
                    }
                    else
                    {
                        var newEvaluationTest = new DdlEvaluationTest
                        {
                            Description = evaluationTest.Description ,
                            Code = evaluationTest.Code,
                            Active = true
                        };

                        clientDbContext.DdlEvaluationTests.Add(newEvaluationTest);
                        clientDbContext.SaveChanges();
                        evaluationTest.EvaluationTestId = newEvaluationTest.EvaluationTestId;
                    }
                }

                return Json(new[] { evaluationTest }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlEvaluationTestsList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlEvaluationTest evaluationTest)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (evaluationTest != null && ModelState.IsValid)
                {
                    var evaluationTestInDb = clientDbContext.DdlEvaluationTests
                        .Where(x => x.EvaluationTestId == evaluationTest.EvaluationTestId)
                        .SingleOrDefault();
                    var evaluationIsDefined = clientDbContext.DdlEvaluationTests
                       .Where(x => x.Code == evaluationTest.Code && x.EvaluationTestId != evaluationTest.EvaluationTestId)
                       .SingleOrDefault();

                    if (evaluationIsDefined != null)
                    {
                        ModelState.AddModelError(string.Empty, "The evolution test already exists.");
                        return Json(new[] { evaluationTest }.ToDataSourceResult(request, ModelState));
                    }
                    List<PersonTest> PersonTests = clientDbContext.PersonTests.Where(x => x.EvaluationTestId == evaluationTest.EvaluationTestId).ToList();
                    if (PersonTests.Count > 0 && !evaluationTest.Active)
                    {
                        ModelState.AddModelError(string.Empty, "Can not be Inactive due to record is in Use.");
                        return Json(new[] { evaluationTest }.ToDataSourceResult(request, ModelState));

                    }

                    if (evaluationTestInDb != null)
                    {
                        evaluationTestInDb.Code = evaluationTest.Code;
                        evaluationTestInDb.Description = evaluationTest.Description;
                        evaluationTestInDb.Active = evaluationTest.Active;
                        clientDbContext.SaveChanges();
                    }
                }

                return Json(new[] { evaluationTest }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlEvaluationTestsList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlEvaluationTest evaluationTest)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (evaluationTest != null)
                {
                    DdlEvaluationTest evaluationTestInDb = clientDbContext.DdlEvaluationTests
                        .Where(x => x.EvaluationTestId == evaluationTest.EvaluationTestId).SingleOrDefault();
                    if (evaluationTestInDb != null)
                    {
                        clientDbContext.DdlEvaluationTests.Remove(evaluationTestInDb);

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

                return Json(new[] { evaluationTest }.ToDataSourceResult(request, ModelState));
            }
        }


        public JsonResult GetEvaluationTests()
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var evaluationTests = clientDbContext.DdlEvaluationTests.Where(x => x.Active == true).OrderBy(x => x.Description).ToList();

                return Json(evaluationTests, JsonRequestBehavior.AllowGet);
            }

        }

    }
}