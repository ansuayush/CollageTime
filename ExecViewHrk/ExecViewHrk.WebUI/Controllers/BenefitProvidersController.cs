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
    public class BenefitProvidersController : Controller
    {
        // GET: BenefitGroup
        public ActionResult BenefitProvidersMatrixPartial()
        {
            PopulateCompanyCodes();
            return View();
        }

        public ActionResult BenefitProvidersList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var benefitProvidersList = clientDbContext.BenefitProviders.OrderBy(e => e.BenefitProviderDescription).ToList();
                return Json(benefitProvidersList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BenefitProvidersList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.BenefitProvider benefitProvider)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (benefitProvider != null && ModelState.IsValid)
                {
                    var benefitProviderInDb = clientDbContext.BenefitProviders
                        .Where(x => x.BenefitProviderCode == benefitProvider.BenefitProviderCode)
                        .SingleOrDefault();

                    if (benefitProviderInDb != null)
                    {
                        ModelState.AddModelError("", "The benefit Provider" + CustomErrorMessages.ERROR_ALREADY_DEFINED);
                    }
                    else
                    {
                        var newBenefitProvider = new BenefitProvider
                        {
                            CompanyCodeId = benefitProvider.CompanyCodeId,
                            BenefitProviderCode = benefitProvider.BenefitProviderCode,
                            BenefitProviderDescription = benefitProvider.BenefitProviderDescription                         
                        };

                        clientDbContext.BenefitProviders.Add(newBenefitProvider);
                        clientDbContext.SaveChanges();
                        benefitProvider.BenefitProviderId = newBenefitProvider.BenefitProviderId;
                    }
                }

                return Json(new[] { benefitProvider }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BenefitProvidersList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.BenefitProvider benefitProvider)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (benefitProvider != null && ModelState.IsValid)
                {
                    var benefitProviderInDb = clientDbContext.BenefitProviders
                        .Where(x => x.BenefitProviderId == benefitProvider.BenefitProviderId)
                        .SingleOrDefault();

                    if (benefitProviderInDb != null)
                    {
                        benefitProviderInDb.CompanyCodeId = benefitProvider.CompanyCodeId;
                        benefitProviderInDb.BenefitProviderCode = benefitProvider.BenefitProviderCode;
                        benefitProviderInDb.BenefitProviderDescription = benefitProvider.BenefitProviderDescription;                      
                        clientDbContext.SaveChanges();
                    }
                }

                return Json(new[] { benefitProvider }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BenefitProvidersList_Destroy([DataSourceRequest] DataSourceRequest request
            , BenefitProvider benefitProvider)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (benefitProvider != null)
                {
                    BenefitProvider benefitProviderInDb = clientDbContext.BenefitProviders
                        .Where(x => x.BenefitProviderId == benefitProvider.BenefitProviderId).SingleOrDefault();

                    if (benefitProviderInDb != null)
                    {
                        clientDbContext.BenefitProviders.Remove(benefitProviderInDb);

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

                return Json(new[] { benefitProvider }.ToDataSourceResult(request, ModelState));
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

        public JsonResult GetBenefitProviders(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var benefitProviders = clientDbContext.BenefitProviders
                    .Select(m => new
                    {
                        BenefitProviderId = m.BenefitProviderId,
                        BenefitProviderDescription = m.BenefitProviderDescription
                    }).OrderBy(x => x.BenefitProviderDescription).ToList();

                return Json(benefitProviders, JsonRequestBehavior.AllowGet);
            }

        }
    }
}