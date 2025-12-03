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
    public class BenefitGroupsController : Controller
    {
        // GET: BenefitGroup
        public ActionResult BenefitGroupsMatrixPartial()
        {
            PopulateCompanyCodes();
            return View();
        }

        public ActionResult BenefitGroupsList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var benefitGroupsList = clientDbContext.BenefitGroups.OrderBy(e => e.BenefitGroupDescription).ToList();
                return Json(benefitGroupsList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BenefitGroupsList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.BenefitGroup benefitGroup)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (benefitGroup != null && ModelState.IsValid)
                {
                    var benefitGroupInDb = clientDbContext.BenefitGroups
                        .Where(x => x.BenefitGroupCode == benefitGroup.BenefitGroupCode)
                        .SingleOrDefault();

                    if (benefitGroupInDb != null)
                    {
                        ModelState.AddModelError("", "The benefit group" + CustomErrorMessages.ERROR_ALREADY_DEFINED);
                    }
                    else
                    {
                        var newBenefitGroup = new BenefitGroup
                        {
                            CompanyCodeId = benefitGroup.CompanyCodeId,
                            BenefitGroupCode = benefitGroup.BenefitGroupCode,
                            BenefitGroupDescription = benefitGroup.BenefitGroupDescription,
                            DeductionCode = benefitGroup.DeductionCode,
                            DisplayPosition =benefitGroup.DisplayPosition,
                            CanBeDeclined = benefitGroup.CanBeDeclined,
                            ScheduleGroup = benefitGroup.ScheduleGroup
                        };

                        clientDbContext.BenefitGroups.Add(newBenefitGroup);
                        clientDbContext.SaveChanges();
                        benefitGroup.BenefitGroupId = newBenefitGroup.BenefitGroupId;
                    }
                }

                return Json(new[] { benefitGroup }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BenefitGroupsList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.BenefitGroup benefitGroup)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (benefitGroup != null && ModelState.IsValid)
                {
                    var benefitGroupInDb = clientDbContext.BenefitGroups
                        .Where(x => x.BenefitGroupId == benefitGroup.BenefitGroupId)
                        .SingleOrDefault();

                    if (benefitGroupInDb != null)
                    {
                        benefitGroupInDb.CompanyCodeId = benefitGroup.CompanyCodeId;
                        benefitGroupInDb.BenefitGroupCode = benefitGroup.BenefitGroupCode;
                        benefitGroupInDb.BenefitGroupDescription = benefitGroup.BenefitGroupDescription;
                        benefitGroupInDb.DeductionCode = benefitGroup.DeductionCode;
                        benefitGroupInDb.DisplayPosition = benefitGroup.DisplayPosition;
                        benefitGroupInDb.CanBeDeclined = benefitGroup.CanBeDeclined;
                        benefitGroupInDb.ScheduleGroup = benefitGroup.ScheduleGroup;
                        clientDbContext.SaveChanges();
                    }
                }

                return Json(new[] { benefitGroup }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BenefitGroupsList_Destroy([DataSourceRequest] DataSourceRequest request
            , BenefitGroup benefitGroup)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (benefitGroup != null)
                {
                    BenefitGroup benefitGroupInDb = clientDbContext.BenefitGroups
                        .Where(x => x.BenefitGroupId == benefitGroup.BenefitGroupId).SingleOrDefault();

                    if (benefitGroupInDb != null)
                    {
                        clientDbContext.BenefitGroups.Remove(benefitGroupInDb);

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

                return Json(new[] { benefitGroup }.ToDataSourceResult(request, ModelState));
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
               // ViewData["defaultCompanyCode"] = companyCodesList.First();
            }
        }

        public JsonResult GetBenefitGroups(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var benefitGroups = clientDbContext.BenefitGroups                    
                    .Select(m => new
                    {
                        BenefitGroupId = m.BenefitGroupId,
                        BenefitGroupDescription = m.BenefitGroupDescription
                    }).OrderBy(x => x.BenefitGroupDescription).ToList();

                return Json(benefitGroups, JsonRequestBehavior.AllowGet);
            }

        }
    }
}