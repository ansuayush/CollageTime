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
    public class BenefitDeductionCodesController : Controller
    {
        // GET: BenefitDeductionCodes
        public ActionResult BenefitDeductionCodesMatrixPartial()
        {
            PopulateCompanyCodes();
            return View();
        }

        public ActionResult BenefitDeductionCodesList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var benefitDeductionCodesList = clientDbContext.BenefitDeductionCodes.OrderBy(e => e.BenefitDeductionCodeDescription).ToList();
                return Json(benefitDeductionCodesList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BenefitDeductionCodesList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.BenefitDeductionCode benefitDeductionCode)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (benefitDeductionCode != null && ModelState.IsValid)
                {
                    var benefitDeductionCodeInDb = clientDbContext.BenefitDeductionCodes
                        .Where(x => x.BenefitDeductionCodeCode == benefitDeductionCode.BenefitDeductionCodeCode)
                        .SingleOrDefault();

                    if (benefitDeductionCodeInDb != null)
                    {
                        ModelState.AddModelError("", "The benefit Option" + CustomErrorMessages.ERROR_ALREADY_DEFINED);
                    }
                    else
                    {
                        var newBenefitDeductionCode = new BenefitDeductionCode
                        {
                            CompanyCodeId = benefitDeductionCode.CompanyCodeId,
                            BenefitDeductionCodeCode = benefitDeductionCode.BenefitDeductionCodeCode,
                            BenefitDeductionCodeDescription = benefitDeductionCode.BenefitDeductionCodeDescription
                        };

                        clientDbContext.BenefitDeductionCodes.Add(newBenefitDeductionCode);
                        clientDbContext.SaveChanges();
                        benefitDeductionCode.BenefitDeductionCodeId = newBenefitDeductionCode.BenefitDeductionCodeId;
                    }
                }

                return Json(new[] { benefitDeductionCode }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BenefitDeductionCodesList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.BenefitDeductionCode benefitDeductionCode)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (benefitDeductionCode != null && ModelState.IsValid)
                {
                    var benefitDeductionCodeInDb = clientDbContext.BenefitDeductionCodes
                        .Where(x => x.BenefitDeductionCodeId == benefitDeductionCode.BenefitDeductionCodeId)
                        .SingleOrDefault();

                    if (benefitDeductionCodeInDb != null)
                    {
                        benefitDeductionCodeInDb.CompanyCodeId = benefitDeductionCode.CompanyCodeId;
                        benefitDeductionCodeInDb.BenefitDeductionCodeCode = benefitDeductionCode.BenefitDeductionCodeCode;
                        benefitDeductionCodeInDb.BenefitDeductionCodeDescription = benefitDeductionCode.BenefitDeductionCodeDescription;
                        clientDbContext.SaveChanges();
                    }
                }

                return Json(new[] { benefitDeductionCode }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BenefitDeductionCodesList_Destroy([DataSourceRequest] DataSourceRequest request
            , BenefitDeductionCode benefitDeductionCode)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (benefitDeductionCode != null)
                {
                    BenefitDeductionCode benefitDeductionCodeInDb = clientDbContext.BenefitDeductionCodes
                        .Where(x => x.BenefitDeductionCodeId == benefitDeductionCode.BenefitDeductionCodeId).SingleOrDefault();

                    if (benefitDeductionCodeInDb != null)
                    {
                        clientDbContext.BenefitDeductionCodes.Remove(benefitDeductionCodeInDb);

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

                return Json(new[] { benefitDeductionCode }.ToDataSourceResult(request, ModelState));
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

        public JsonResult GetBenefitDeductionCodes(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var benefitDeductionCodes = clientDbContext.BenefitDeductionCodes
                    .Select(m => new
                    {
                        BenefitDeductionCodeId = m.BenefitDeductionCodeId,
                        BenefitDeductionCodeDescription = m.BenefitDeductionCodeDescription
                    }).OrderBy(x => x.BenefitDeductionCodeDescription).ToList();

                return Json(benefitDeductionCodes, JsonRequestBehavior.AllowGet);
            }

        }
    }
}