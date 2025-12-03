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
    public class CompanyCodesController : Controller
    {
        // GET: ComapnyCode
        //public ActionResult CompanyCodesMatrixPartial()
        //{
        //    return View();
        //}

        //public ActionResult CompanyCodesList_Read([DataSourceRequest]DataSourceRequest request)
        //{
        //    string connString = User.Identity.GetClientConnectionString();

        //    using (ClientDbContext clientDbContext = new ClientDbContext(connString))
        //    {
        //        var companyCodesList = clientDbContext.CompanyCodes.OrderBy(e => e.CompanyCodeDescription).ToList();
        //        return Json(companyCodesList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        //    }
        //}


        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult CompanyCodesList_Create([DataSourceRequest] DataSourceRequest request, CompanyCode companyCode)
        //{
        //    string connString = User.Identity.GetClientConnectionString();
        //    using (ClientDbContext clientDbContext = new ClientDbContext(connString))
        //    {
        //        if (companyCode != null && ModelState.IsValid)
        //        {
        //            var companyCodeInDb = clientDbContext.CompanyCodes.Where(x => x.CompanyCodeCode == companyCode.CompanyCodeCode || x.CompanyCodeDescription == companyCode.CompanyCodeDescription).FirstOrDefault();

        //            if (companyCodeInDb != null)
        //            {
        //                ModelState.AddModelError("", "Company Code" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
        //            }
        //            else
        //            {
        //                var newCompanyCode = new CompanyCode
        //                {
        //                    CompanyCodeDescription = companyCode.CompanyCodeDescription,
        //                    CompanyCodeCode = companyCode.CompanyCodeCode,
        //                    IsCompanyCodeActive = companyCode.IsCompanyCodeActive,
        //                };

        //                clientDbContext.CompanyCodes.Add(newCompanyCode);
        //                clientDbContext.SaveChanges();
        //                companyCode.CompanyCodeId = newCompanyCode.CompanyCodeId;
        //            }
        //        }

        //        return Json(new[] { companyCode }.ToDataSourceResult(request, ModelState));
        //    }
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult CompanyCodesList_Update([DataSourceRequest] DataSourceRequest request, CompanyCode companyCode)
        //{
        //    if (companyCode != null && ModelState.IsValid)
        //    {
        //        string connString = User.Identity.GetClientConnectionString();
        //        ClientDbContext clientDbContext = new ClientDbContext(connString);
        //        var companyCodeInDb = clientDbContext.CompanyCodes.Where(x => x.CompanyCodeId != companyCode.CompanyCodeId && (x.CompanyCodeCode == companyCode.CompanyCodeCode || x.CompanyCodeDescription == companyCode.CompanyCodeDescription)).FirstOrDefault();
        //        if (companyCodeInDb == null)
        //        {
        //            companyCodeInDb = clientDbContext.CompanyCodes.Where(x => x.CompanyCodeId == companyCode.CompanyCodeId).SingleOrDefault();

        //            if (companyCodeInDb.IsCompanyCodeActive != companyCode.IsCompanyCodeActive && companyCode.IsCompanyCodeActive == false)
        //            {
        //                if (clientDbContext.Jobs.Where(x => x.CompanyCodeId == companyCode.CompanyCodeId).Count() == 0 &&
        //                    clientDbContext.Employees.Where(x => x.CompanyCodeId == companyCode.CompanyCodeId).Count() == 0)
        //                {
        //                    companyCodeInDb.CompanyCodeCode = companyCode.CompanyCodeCode;
        //                    companyCodeInDb.CompanyCodeDescription = companyCode.CompanyCodeDescription;
        //                    companyCodeInDb.IsCompanyCodeActive = companyCode.IsCompanyCodeActive;
        //                    clientDbContext.SaveChanges();
        //                }
        //                else
        //                {
        //                    ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
        //                }
        //            }
        //            else
        //            {
        //                companyCodeInDb.CompanyCodeCode = companyCode.CompanyCodeCode;
        //                companyCodeInDb.CompanyCodeDescription = companyCode.CompanyCodeDescription;
        //                companyCodeInDb.IsCompanyCodeActive = companyCode.IsCompanyCodeActive;
        //                clientDbContext.SaveChanges();
        //            }
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", string.Format(CustomErrorMessages.ERROR_LOOKUP_DUPLICATE_RECORD, "Company Code"));
        //        }


        //    }
        //    return Json(new[] { companyCode }.ToDataSourceResult(request, ModelState));

        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult CompanyCodesList_Destroy([DataSourceRequest] DataSourceRequest request
        //    , CompanyCode companyCode)
        //{
        //    string connString = User.Identity.GetClientConnectionString();
        //    using (ClientDbContext clientDbContext = new ClientDbContext(connString))
        //    {
        //        if (companyCode != null)
        //        {
        //            if (clientDbContext.Jobs.Where(x => x.CompanyCodeId == companyCode.CompanyCodeId).Count() > 0 ||
        //                    clientDbContext.Employees.Where(x => x.CompanyCodeId == companyCode.CompanyCodeId).Count() > 0)
        //            {
        //                ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
        //            }
        //            else
        //            {
        //                CompanyCode companyCodeInDb = clientDbContext.CompanyCodes.Where(x => x.CompanyCodeId == companyCode.CompanyCodeId).SingleOrDefault();

        //                if (companyCodeInDb != null)
        //                {
        //                    clientDbContext.CompanyCodes.Remove(companyCodeInDb);

        //                    try
        //                    {
        //                        clientDbContext.SaveChanges();
        //                    }
        //                    catch// (Exception err)
        //                    {
        //                        ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
        //                    }

        //                }
        //            }
        //        }

        //        return Json(new[] { companyCode }.ToDataSourceResult(request, ModelState));
        //    }
        //}

        public JsonResult GetCompanyCodes()
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var companyCodes = clientDbContext.CompanyCodes
                    .Where(x => x.IsCompanyCodeActive == true)
                    .Select(m => new
                    {
                        CompanyCodeId = m.CompanyCodeId,
                        CompanyCodeDescription = m.CompanyCodeDescription
                    }).OrderBy(x => x.CompanyCodeDescription).ToList();

                return Json(companyCodes, JsonRequestBehavior.AllowGet);
            }

        }
    }
}