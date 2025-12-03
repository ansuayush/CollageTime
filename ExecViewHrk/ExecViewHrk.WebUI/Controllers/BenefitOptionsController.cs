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
    public class BenefitOptionsController : Controller
    {
        // GET: BenefitGroup
        public ActionResult BenefitOptionsMatrixPartial()
        {
            PopulateCompanyCodes();
            return View();
        }

        public ActionResult BenefitOptionsList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var benefitOptionsList = clientDbContext.BenefitOptions.OrderBy(e => e.BenefitOptionDescription).ToList();
                return Json(benefitOptionsList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BenefitOptionsList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.BenefitOption benefitOption)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (benefitOption != null && ModelState.IsValid)
                {
                    var benefitOptionInDb = clientDbContext.BenefitOptions
                        .Where(x => x.BenefitOptionCode == benefitOption.BenefitOptionCode)
                        .SingleOrDefault();

                    if (benefitOptionInDb != null)
                    {
                        ModelState.AddModelError("", "The benefit Option" + CustomErrorMessages.ERROR_ALREADY_DEFINED);
                    }
                    else
                    {
                        var newBenefitOption = new BenefitOption
                        {
                            CompanyCodeId = benefitOption.CompanyCodeId,
                            BenefitOptionCode = benefitOption.BenefitOptionCode,
                            BenefitOptionDescription = benefitOption.BenefitOptionDescription
                        };

                        clientDbContext.BenefitOptions.Add(newBenefitOption);
                        clientDbContext.SaveChanges();
                        benefitOption.BenefitOptionId = newBenefitOption.BenefitOptionId;
                    }
                }

                return Json(new[] { benefitOption }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BenefitOptionsList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.BenefitOption benefitOption)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (benefitOption != null && ModelState.IsValid)
                {
                    var benefitOptionInDb = clientDbContext.BenefitOptions
                        .Where(x => x.BenefitOptionId == benefitOption.BenefitOptionId)
                        .SingleOrDefault();

                    if (benefitOptionInDb != null)
                    {
                        benefitOptionInDb.CompanyCodeId = benefitOption.CompanyCodeId;
                        benefitOptionInDb.BenefitOptionCode = benefitOption.BenefitOptionCode;
                        benefitOptionInDb.BenefitOptionDescription = benefitOption.BenefitOptionDescription;
                        clientDbContext.SaveChanges();
                    }
                }

                return Json(new[] { benefitOption }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BenefitOptionsList_Destroy([DataSourceRequest] DataSourceRequest request
            , BenefitOption benefitOption)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (benefitOption != null)
                {
                    BenefitOption benefitOptionInDb = clientDbContext.BenefitOptions
                        .Where(x => x.BenefitOptionId == benefitOption.BenefitOptionId).SingleOrDefault();

                    if (benefitOptionInDb != null)
                    {
                        clientDbContext.BenefitOptions.Remove(benefitOptionInDb);

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

                return Json(new[] { benefitOption }.ToDataSourceResult(request, ModelState));
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

        public JsonResult GetBenefitOptions(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var benefitOptions = clientDbContext.BenefitOptions
                    .Select(m => new
                    {
                        BenefitOptionId = m.BenefitOptionId,
                        BenefitOptionDescription = m.BenefitOptionDescription
                    }).OrderBy(x => x.BenefitOptionDescription).ToList();

                return Json(benefitOptions, JsonRequestBehavior.AllowGet);
            }

        }
    }
}