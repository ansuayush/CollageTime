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
using System.Data.Entity;
using ExecViewHrk.WebUI.Helpers;

namespace ExecViewHrk.WebUI.Controllers
{
    public class DdlLicenseTypesController : Controller
    {
        // GET: DdlLicenseTypes
        public ActionResult DdlLicenseTypesListMaintenance()
        {
            return View();
        }

        public ActionResult DdlLicenseTypesList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var licenseTypeList = clientDbContext.DdlLicenseTypes.OrderBy(e => e.Description).ToList();
                return Json(licenseTypeList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlLicenseTypesList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlLicenseType licenseType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (licenseType != null && ModelState.IsValid)
                {
                    var licenseTypeInDb = clientDbContext.DdlLicenseTypes
                        .Where(x => x.Code == licenseType.Code)
                        .SingleOrDefault();
                    var licenseTypeDescInDb = clientDbContext.DdlLicenseTypes.ToList();

                    var licensetypelist = clientDbContext.DdlLicenseTypes.Where(n => n.LicenseTypeId != licenseType.LicenseTypeId).ToList();
                    var licenseDesc = licenseTypeDescInDb.Select(x => x.Description == licenseType.Description);
                    if (licenseTypeInDb != null || licenseDesc != null)
                    {
                        if (licensetypelist.Select(m => m.Code).Contains(licenseType.Code) || licensetypelist.Select(m => m.Description).Contains(licenseType.Description))
                        {
                            ModelState.AddModelError("", "The License Type Code or Description already exists!");
                        }
                        else
                        {
                            var newLicenseType = new DdlLicenseType
                            {
                                Description = licenseType.Description,
                                Code = licenseType.Code,
                                Active = licenseType.Active
                            };

                            clientDbContext.DdlLicenseTypes.Add(newLicenseType);
                            clientDbContext.SaveChanges();
                            licenseType.LicenseTypeId = newLicenseType.LicenseTypeId;
                        }
                    }
                }

                return Json(new[] { licenseType }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlLicenseTypesList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlLicenseType licenseType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (licenseType != null && ModelState.IsValid)
                {
                    var licenseTypeInDb = clientDbContext.DdlLicenseTypes
                        .Where(x => x.LicenseTypeId == licenseType.LicenseTypeId)
                        .SingleOrDefault();
                    var licensetypelist = clientDbContext.DdlLicenseTypes.Where(n => n.LicenseTypeId != licenseType.LicenseTypeId).ToList();
                    var usedlicenselist = clientDbContext.PersonLicenses.ToList();
                    var active = usedlicenselist.Where(c => c.DdlLicenseType.LicenseTypeId == licenseType.LicenseTypeId).Select(m => m.DdlLicenseType.Active).SingleOrDefault();

                    if (licenseTypeInDb != null)
                    {
                        if (licensetypelist.Select(m => m.Code).Contains(licenseType.Code) || licensetypelist.Select(m => m.Description).Contains(licenseType.Description))
                        {
                            ModelState.AddModelError("", "The License Type Code or Description already exists!");
                        }
                        else if (usedlicenselist.Select(s => s.DdlLicenseType.Description).Contains(licenseType.Description) && active != licenseType.Active)
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                        }
                        else
                        {
                            licenseTypeInDb.Code = licenseType.Code;
                            licenseTypeInDb.Description = licenseType.Description;
                            licenseTypeInDb.Active = licenseType.Active;
                            clientDbContext.SaveChanges();
                        }
                    }
                }

                return Json(new[] { licenseType }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlLicenseTypesList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlLicenseType licenseType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (licenseType != null)
                {
                    DdlLicenseType licenseTypeInDb = clientDbContext.DdlLicenseTypes.Include(n => n.PersonLicenses)
                       .Where(x => x.LicenseTypeId == licenseType.LicenseTypeId).SingleOrDefault();

                    if (licenseTypeInDb != null)
                    {
                        try
                        {
                            clientDbContext.DdlLicenseTypes.Remove(licenseTypeInDb);
                            clientDbContext.SaveChanges();
                        }
                        catch// (Exception err)
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                        }

                    }
                }

                return Json(new[] { licenseType }.ToDataSourceResult(request, ModelState));
            }
        }


        public JsonResult GetLicenseTypes(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var licenseTypes = clientDbContext.DdlLicenseTypes
                    .Where(x => x.Active == true)
                    .Select(m => new
                    {
                        LicenseTypeId = m.LicenseTypeId,
                        LicenseDescription = m.Description
                    }).OrderBy(x => x.LicenseDescription).ToList();

                return Json(licenseTypes, JsonRequestBehavior.AllowGet);
            }

        }
    }
}