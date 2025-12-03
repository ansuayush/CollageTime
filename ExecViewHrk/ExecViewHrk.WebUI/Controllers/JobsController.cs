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
    public class JobsController : Controller
    {
        // GET: Jobs
        public ActionResult JobsMatrixPartial()
        {
            PopulateCompanyCodes();
            return View();
        }

        public ActionResult JobsList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var jobsList = clientDbContext.Jobs.OrderBy(e => e.JobDescription).ToList();
                return Json(jobsList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobsList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.Job job)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (job != null && ModelState.IsValid)
                {
                    var jobInDb = clientDbContext.Jobs
                        .Where(x => x.JobCode == job.JobCode)
                        .SingleOrDefault();

                    if (jobInDb != null)
                    {
                        ModelState.AddModelError("", "The Job" + CustomErrorMessages.ERROR_ALREADY_DEFINED);
                    }
                    else
                    {
                        var newJob = new Job
                        {
                            CompanyCodeId = job.CompanyCodeId,
                            JobDescription = job.JobDescription,
                            JobCode = job.JobCode,
                            IsJobActive = true
                        };

                        clientDbContext.Jobs.Add(newJob);
                        clientDbContext.SaveChanges();
                        job.JobId = newJob.JobId;
                    }
                }

                return Json(new[] { job }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobsList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.Job job)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (job != null && ModelState.IsValid)
                {
                    var jobInDb = clientDbContext.Jobs
                        .Where(x => x.JobId == job.JobId)
                        .SingleOrDefault();

                    if (jobInDb != null)
                    {
                        jobInDb.CompanyCodeId = job.CompanyCodeId;
                        jobInDb.JobCode = job.JobCode;
                        jobInDb.JobDescription = job.JobDescription;
                        jobInDb.IsJobActive = job.IsJobActive;
                        clientDbContext.SaveChanges();
                    }
                }

                return Json(new[] { job }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobsList_Destroy([DataSourceRequest] DataSourceRequest request
            , Job job)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (job != null)
                {
                    Job jobInDb = clientDbContext.Jobs
                        .Where(x => x.JobId == job.JobId).SingleOrDefault();

                    if (jobInDb != null)
                    {
                        clientDbContext.Jobs.Remove(jobInDb);

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

                return Json(new[] { job }.ToDataSourceResult(request, ModelState));
            }
        }

        public JsonResult GetJobs(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var jobs = clientDbContext.Jobs
                    .Where(x => x.IsJobActive == true)
                    .Select(m => new
                    {
                        JobId = m.JobId,
                        JobDescription = m.JobDescription
                    }).OrderBy(x => x.JobDescription).ToList();

                return Json(jobs, JsonRequestBehavior.AllowGet);
            }

        }

        private void PopulateCompanyCodes()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var companyCodesList = new ClientDbContext(connString).CompanyCodes
                        .Select(c => new CompanyCodeVm
                        {
                            CompanyCodeId = c.CompanyCodeId,
                            CompanyCodeDescription = c.CompanyCodeDescription //+ "-" + e.SkillId.ToString()
                        })
                        .OrderBy(s => s.CompanyCodeDescription).ToList();

                //companyCodesList.Insert(0, new CompanyCodeVm { CompanyCodeId = 0, CompanyCodeDescription = "--select one--" });

                ViewData["companyCodesList"] = companyCodesList;
                //ViewData["defaultCompanyCode"] = companyCodesList.First();
            }
        }
    }
}