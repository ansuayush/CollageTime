using ExecViewHrk.WebUI.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExecViewHrk.EfAdmin;
using ExecViewHrk.EfClient;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Net;
using ExecViewHrk.WebUI.Helpers;
namespace ExecViewHrk.WebUI.Controllers
{
    public class DdlCountriesController : Controller
    {
        // GET: DdlCountries
        public ActionResult DdlCountriesListMaintenance()
        {
            return View();
        }

        public ActionResult DdlCountriesList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ExecViewHrk.EfClient.ClientDbContext clientDbContext = new ExecViewHrk.EfClient.ClientDbContext(connString))
            {
                var countriesTypeList = clientDbContext.DdlCountries.OrderBy(e => e.Description).ToList();
                return Json(countriesTypeList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlCountriesList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlCountry country)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (country != null && ModelState.IsValid)
                {
                    var CountriesInDb = clientDbContext.DdlCountries
                        .Where(x => x.Code == country.Code || x.Description.Trim() == country.Description.Trim())
                        .SingleOrDefault();

                    if (CountriesInDb != null)
                    {
                        ModelState.AddModelError("", "The country" + CustomErrorMessages.ERROR_ALREADY_DEFINED);
                    }
                    else
                    {
                        var newCountry = new DdlCountry
                        {
                            Description = country.Description,
                            Code = country.Code,
                            Active = country.Active
                        };

                        clientDbContext.DdlCountries.Add(newCountry);
                        clientDbContext.SaveChanges();
                        country.CountryId = newCountry.CountryId;
                    }
                }

                return Json(new[] { country }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlCountriesList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlCountry country)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (country != null && ModelState.IsValid)
                {
                    var duplicateCountyInDb = clientDbContext.DdlCountries.Where(x => x.CountryId != country.CountryId 
                    && (x.Code == country.Code || x.Description.Trim() == country.Description.Trim())).SingleOrDefault();

                    if (duplicateCountyInDb == null)
                    {
                        var countryInDb = clientDbContext.DdlCountries.Where(x => x.CountryId == country.CountryId).SingleOrDefault();
                        var usedcountrieslistPassport = clientDbContext.PersonPassports.ToList();
                        var usedcountrieslistAddress = clientDbContext.PersonAddresses.ToList();
                        var usedcountrieslistLicnces = clientDbContext.PersonLicenses.ToList();

                        if (countryInDb != null)
                        {
                            if (usedcountrieslistPassport.Select(s => s.CountryId).Contains(country.CountryId) && !country.Active)
                            {
                                ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);

                            }
                            else if (usedcountrieslistAddress.Select(s => s.CountryId).Contains(country.CountryId) && !country.Active)
                            {
                                ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);

                            }
                            else if (usedcountrieslistLicnces.Select(s => s.CountryId).Contains(country.CountryId) && !country.Active)
                            {
                                ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                            }
                            else
                            {
                                countryInDb.Code = country.Code;
                                countryInDb.Description = country.Description;
                                countryInDb.Active = country.Active;
                                clientDbContext.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "The country" + CustomErrorMessages.ERROR_ALREADY_DEFINED);
                    }
                }

                return Json(new[] { country }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlCountriesList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlCountry country)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (country != null)
                {
                    DdlCountry countryInDb = clientDbContext.DdlCountries
                        .Where(x => x.CountryId == country.CountryId).SingleOrDefault();

                    if (countryInDb != null)
                    {
                        clientDbContext.DdlCountries.Remove(countryInDb);

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

                return Json(new[] { country }.ToDataSourceResult(request, ModelState));
            }
        }

        public JsonResult GetCountries(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var country = clientDbContext.DdlCountries
                    .Where(x => x.Active == true)
                    .Select(m => new
                    {
                        CountryId = m.CountryId,
                        CountryDescription = m.Description
                    }).OrderBy(x => x.CountryDescription).ToList();

                return Json(country, JsonRequestBehavior.AllowGet);
            }

        }
    }
}