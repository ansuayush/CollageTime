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
using System.Data.Entity;
using ExecViewHrk.WebUI.Helpers;
namespace ExecViewHrk.WebUI.Controllers
{
    public class BenefitOESchedulesController : Controller
    {
        // GET: BenefitGroup
        public ActionResult BenefitOESchedulesMatrixPartial()
        {
            PopulateCompanyCodes();
            return View();
        }

        public ActionResult BenefitOESchedulesList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var benefitOESchedulesList = clientDbContext.BenefitOESchedules.OrderBy(e => e.CompanyCodeId).ThenByDescending(e => e.BenefitOEScheduleId).ToList();
                return Json(benefitOESchedulesList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BenefitOESchedulesList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.BenefitOESchedule benefitOESchedule)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (benefitOESchedule != null && ModelState.IsValid)
                {
                    var benefitOEScheduleInDb = clientDbContext.BenefitOESchedules
                        .Where(x => x.CompanyCodeId == benefitOESchedule.CompanyCodeId && DbFunctions.TruncateTime(x.StartDate) == DbFunctions.TruncateTime(benefitOESchedule.StartDate)
                         && x.ScheduleGroup == benefitOESchedule.ScheduleGroup).SingleOrDefault();


                    if (benefitOEScheduleInDb != null)
                    {
                        ModelState.AddModelError("", "The benefit OESchedule is already defined.");
                    }
                    else
                    {
                        //check StartDate > PreviousEnddate in a group and EffectiveDate > EndDate
                        if (clientDbContext.BenefitOESchedules.All(x => DbFunctions.TruncateTime(x.EndDate) < DbFunctions.TruncateTime(benefitOESchedule.StartDate) 
                            && DbFunctions.TruncateTime(benefitOESchedule.EndDate) > DbFunctions.TruncateTime(benefitOESchedule.StartDate)
                            && DbFunctions.TruncateTime(benefitOESchedule.EffectiveDate) > DbFunctions.TruncateTime(benefitOESchedule.EndDate)
                            && x.ScheduleGroup == benefitOESchedule.ScheduleGroup ))
                        {
                            var newBenefitOESchedule = new BenefitOESchedule
                            {
                                CompanyCodeId = benefitOESchedule.CompanyCodeId,
                                StartDate = benefitOESchedule.StartDate,
                                EndDate = benefitOESchedule.EndDate,
                                EffectiveDate = benefitOESchedule.EffectiveDate,
                                ScheduleGroup = benefitOESchedule.ScheduleGroup
                            };

                            clientDbContext.BenefitOESchedules.Add(newBenefitOESchedule);
                            clientDbContext.SaveChanges();
                            benefitOESchedule.BenefitOEScheduleId = newBenefitOESchedule.BenefitOEScheduleId;
                        }
                        else
                        {
                            ModelState.AddModelError("", "Please verify the dates.");
                        }
                        
                    }
                }

                return Json(new[] { benefitOESchedule }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BenefitOESchedulesList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.BenefitOESchedule benefitOESchedule)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (benefitOESchedule != null && ModelState.IsValid)
                {
                    //check group with the same schedule
                    var chkAlreadyExistInDb = clientDbContext.BenefitOESchedules
                       .Where(x => x.CompanyCodeId == benefitOESchedule.CompanyCodeId && DbFunctions.TruncateTime(x.StartDate) == DbFunctions.TruncateTime(benefitOESchedule.StartDate)
                        && x.ScheduleGroup == benefitOESchedule.ScheduleGroup).SingleOrDefault();

                    if (chkAlreadyExistInDb != null)
                    {
                        ModelState.AddModelError("", "The benefit OESchedule is already defined.");
                    }
                    else
                    {
                        if (clientDbContext.BenefitOESchedules.All(x => DbFunctions.TruncateTime(x.EndDate) < DbFunctions.TruncateTime(benefitOESchedule.StartDate) 
                            && DbFunctions.TruncateTime(benefitOESchedule.EndDate) > DbFunctions.TruncateTime(benefitOESchedule.StartDate)
                            && DbFunctions.TruncateTime(benefitOESchedule.EffectiveDate) > DbFunctions.TruncateTime(benefitOESchedule.EndDate)
                            && x.ScheduleGroup == benefitOESchedule.ScheduleGroup ))
                        {
                            var benefitOEScheduleInDb = clientDbContext.BenefitOESchedules
                            .Where(x => x.BenefitOEScheduleId == benefitOESchedule.BenefitOEScheduleId)
                            .SingleOrDefault();

                            if (benefitOEScheduleInDb != null)
                            {
                                benefitOEScheduleInDb.CompanyCodeId = benefitOESchedule.CompanyCodeId;
                                benefitOEScheduleInDb.StartDate = benefitOESchedule.StartDate;
                                benefitOEScheduleInDb.EndDate = benefitOESchedule.EndDate;
                                benefitOEScheduleInDb.EffectiveDate = benefitOESchedule.EffectiveDate;
                                benefitOEScheduleInDb.ScheduleGroup = benefitOESchedule.ScheduleGroup;
                                clientDbContext.SaveChanges();
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Please verify the dates.");
                        }
                        
                    }
                }

                return Json(new[] { benefitOESchedule }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BenefitOESchedulesList_Destroy([DataSourceRequest] DataSourceRequest request
            , BenefitOESchedule benefitOESchedule)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (benefitOESchedule != null)
                {
                    BenefitOESchedule benefitOEScheduleInDb = clientDbContext.BenefitOESchedules
                        .Where(x => x.BenefitOEScheduleId == benefitOESchedule.BenefitOEScheduleId).SingleOrDefault();

                    if (benefitOEScheduleInDb != null)
                    {
                        clientDbContext.BenefitOESchedules.Remove(benefitOEScheduleInDb);

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

                return Json(new[] { benefitOESchedule }.ToDataSourceResult(request, ModelState));
            }
        }

        //Repeated
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

        public JsonResult GetBenefitOESchedules(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var benefitOESchedules = clientDbContext.BenefitOESchedules
                    .Select(m => new
                    {
                        BenefitOEScheduleId = m.BenefitOEScheduleId,
                        BenefitOEScheduleDescription = m.CompanyCodeId + " " + m.StartDate
                    }).OrderBy(x => x.BenefitOEScheduleDescription).ToList();

                return Json(benefitOESchedules, JsonRequestBehavior.AllowGet);
            }

        }
    }
}