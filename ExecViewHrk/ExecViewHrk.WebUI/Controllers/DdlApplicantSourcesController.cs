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
    public class DdlApplicantSourcesController : Controller
    {
        // GET: api/DdlApplicantSources
        public ActionResult DdlApplicantListMaintenance()
        {
            return View();
        }

        public ActionResult DdlApplicantSourceList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var applicantSourceList = clientDbContext.DdlApplicantSources.OrderBy(e => e.Description).ToList();
                return Json(applicantSourceList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlApplicantSourceList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlApplicantSource applicantSource)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (applicantSource != null && ModelState.IsValid)
                {
                    var applicantSourceInDb = clientDbContext.DdlApplicantSources
                        .Where(x => x.Code == applicantSource.Code)
                        .SingleOrDefault();

                    if (applicantSourceInDb != null)
                    {
                        ModelState.AddModelError("", "The Applicant Source is already exists.");
                    }
                    else
                    {
                        var newApplicantSource = new DdlApplicantSource
                        {
                            Code = applicantSource.Code,
                            Description = applicantSource.Description,
                            AddressLineOne = applicantSource.AddressLineOne,
                            AddressLineTwo = applicantSource.AddressLineTwo,
                            City = applicantSource.City,
                            PhoneNumber = applicantSource.PhoneNumber,
                            FaxNumber = applicantSource.FaxNumber,
                            Contact = applicantSource.Contact,
                            WebAddress = applicantSource.WebAddress,
                            ZipCode = applicantSource.ZipCode,
                            AccountNumber = applicantSource.AccountNumber,
                        };

                        clientDbContext.DdlApplicantSources.Add(newApplicantSource);
                        clientDbContext.SaveChanges();
                        applicantSource.ApplicantSourceId = newApplicantSource.ApplicantSourceId;
                    }
                }

                return Json(new[] { applicantSource }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlApplicantSourceList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlApplicantSource applicantSource)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (applicantSource != null && ModelState.IsValid)
                {
                    var applicantSourceInDb = clientDbContext.DdlApplicantSources
                        .Where(x => x.ApplicantSourceId == applicantSource.ApplicantSourceId)
                        .SingleOrDefault();
                    var applicantSourceIsDefined = clientDbContext.DdlApplicantSources
                       .Where(x => x.Code == applicantSource.Code && x.ApplicantSourceId != applicantSource.ApplicantSourceId)
                       .SingleOrDefault();

                    if (applicantSourceIsDefined != null )
                    {
                        ModelState.AddModelError("", "The Applicant Source is already exists");
                    }
                    else
                    {

                        if (applicantSourceInDb != null)
                        {
                            applicantSourceInDb.Code = applicantSource.Code;
                            applicantSourceInDb.Description = applicantSource.Description;
                            applicantSourceInDb.AddressLineOne = applicantSource.AddressLineOne;
                            applicantSourceInDb.AddressLineTwo = applicantSource.AddressLineTwo;
                            applicantSourceInDb.City = applicantSource.City;
                            applicantSourceInDb.PhoneNumber = applicantSource.PhoneNumber;
                            applicantSourceInDb.FaxNumber = applicantSource.FaxNumber;
                            applicantSourceInDb.Contact = applicantSource.Contact;
                            applicantSourceInDb.WebAddress = applicantSource.WebAddress;
                            applicantSourceInDb.ZipCode = applicantSource.ZipCode;
                            applicantSourceInDb.AccountNumber = applicantSource.AccountNumber;
                            clientDbContext.SaveChanges();
                        }
                    }
                }

                return Json(new[] { applicantSource }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlApplicantSourceList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlApplicantSource applicantSource)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (applicantSource != null)
                {
                    DdlApplicantSource applicantSourceInDb = clientDbContext.DdlApplicantSources
                        .Where(x => x.ApplicantSourceId == applicantSource.ApplicantSourceId).SingleOrDefault();

                    if (applicantSourceInDb != null)
                    {
                        clientDbContext.DdlApplicantSources.Remove(applicantSourceInDb);

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

                return Json(new[] { applicantSource }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GetDdlApplicantSource(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var applicantSources = clientDbContext.DdlApplicantSources
                    //.Where(x => x.Active == true)
                    .Select(m => new
                    {
                        ApplicantSourceId = m.ApplicantSourceId,
                        ApplicantDescription = m.Description,

                    }).OrderBy(x => x.ApplicantDescription).ToList();

                return Json(applicantSources, JsonRequestBehavior.AllowGet);
            }

        }
    }
}
