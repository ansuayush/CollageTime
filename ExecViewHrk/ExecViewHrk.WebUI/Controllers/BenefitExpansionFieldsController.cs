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
    public class BenefitExpansionFieldsController : Controller
    {
        // GET: BenefitExpansionFields
        public ActionResult BenefitExpansionFieldsMatrixPartial()
        {
            PopulateCompanyCodes();
            return View();
        }

        public ActionResult BenefitExpansionFieldsList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var benefitExpansionFieldsList = clientDbContext.BenefitExpansionFields.OrderBy(e => e.BenefitExpansionFieldDescription).ToList();
                return Json(benefitExpansionFieldsList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BenefitExpansionFieldsList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.BenefitExpansionField benefitExpansionField)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (benefitExpansionField != null && ModelState.IsValid)
                {
                    var benefitExpansionFieldInDb = clientDbContext.BenefitExpansionFields
                        .Where(x => x.BenefitExpansionFieldCode == benefitExpansionField.BenefitExpansionFieldCode)
                        .SingleOrDefault();

                    if (benefitExpansionFieldInDb != null)
                    {
                        ModelState.AddModelError("", "The Benefit Expansion" + CustomErrorMessages.ERROR_ALREADY_DEFINED);
                    }
                    else
                    {
                        var newBenefitExpansionField = new BenefitExpansionField
                        {
                            CompanyCodeId = benefitExpansionField.CompanyCodeId,
                            BenefitExpansionFieldCode = benefitExpansionField.BenefitExpansionFieldCode,
                            BenefitExpansionFieldDescription = benefitExpansionField.BenefitExpansionFieldDescription
                        };

                        clientDbContext.BenefitExpansionFields.Add(newBenefitExpansionField);
                        clientDbContext.SaveChanges();
                        benefitExpansionField.BenefitExpansionFieldId = newBenefitExpansionField.BenefitExpansionFieldId;
                    }
                }

                return Json(new[] { benefitExpansionField }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BenefitExpansionFieldsList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.BenefitExpansionField benefitExpansionField)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (benefitExpansionField != null && ModelState.IsValid)
                {
                    var benefitExpansionFieldInDb = clientDbContext.BenefitExpansionFields
                        .Where(x => x.BenefitExpansionFieldId == benefitExpansionField.BenefitExpansionFieldId)
                        .SingleOrDefault();

                    if (benefitExpansionFieldInDb != null)
                    {
                        benefitExpansionFieldInDb.CompanyCodeId = benefitExpansionField.CompanyCodeId;
                        benefitExpansionFieldInDb.BenefitExpansionFieldCode = benefitExpansionField.BenefitExpansionFieldCode;
                        benefitExpansionFieldInDb.BenefitExpansionFieldDescription = benefitExpansionField.BenefitExpansionFieldDescription;
                        clientDbContext.SaveChanges();
                    }
                }

                return Json(new[] { benefitExpansionField }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BenefitExpansionFieldsList_Destroy([DataSourceRequest] DataSourceRequest request
            , BenefitExpansionField benefitExpansionField)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (benefitExpansionField != null)
                {
                    BenefitExpansionField benefitExpansionFieldInDb = clientDbContext.BenefitExpansionFields
                        .Where(x => x.BenefitExpansionFieldId == benefitExpansionField.BenefitExpansionFieldId).SingleOrDefault();

                    if (benefitExpansionFieldInDb != null)
                    {
                        clientDbContext.BenefitExpansionFields.Remove(benefitExpansionFieldInDb);

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

                return Json(new[] { benefitExpansionField }.ToDataSourceResult(request, ModelState));
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

        public JsonResult GetBenefitExpansionFields(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var benefitExpansionFields = clientDbContext.BenefitExpansionFields
                    .Select(m => new
                    {
                        BenefitExpansionFieldId = m.BenefitExpansionFieldId,
                        BenefitExpansionFieldDescription = m.BenefitExpansionFieldDescription
                    }).OrderBy(x => x.BenefitExpansionFieldDescription).ToList();

                return Json(benefitExpansionFields, JsonRequestBehavior.AllowGet);
            }

        }
    }
}