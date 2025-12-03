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
    public class DdlHospitalsController : Controller
    {
        // GET api/<controller>
        public ActionResult DdlApplicantListMaintenance()
        {
            return View();
        }

        public ActionResult DdlHospitalList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var hospitalList = clientDbContext.DdlHospitals.OrderBy(e => e.Description).ToList();
                return Json(hospitalList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlHospitalList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlHospital hospital)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (hospital != null && ModelState.IsValid)
                {
                    var hospitalInDb = clientDbContext.DdlHospitals
                        .Where(x => x.Code == hospital.Code)
                        .SingleOrDefault();

                    if (hospitalInDb != null)
                    {
                        ModelState.AddModelError("", "This Hospital is already exists.");
                    }
                    else
                    {
                        var newHospital = new DdlHospital
                        {
                            Code = hospital.Code,
                            Description = hospital.Description,
                            AddressLineOne = hospital.AddressLineOne,
                            AddressLineTwo = hospital.AddressLineTwo,
                            City = hospital.City,
                            PhoneNumber = hospital.PhoneNumber,
                            FaxNumber = hospital.FaxNumber,
                            Contact = hospital.Contact,
                            WebAddress = hospital.WebAddress,
                            ZipCode = hospital.ZipCode,
                        };

                        clientDbContext.DdlHospitals.Add(newHospital);
                        clientDbContext.SaveChanges();
                        hospital.HospitalId = newHospital.HospitalId;
                    }
                }

                return Json(new[] { hospital }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlHospitalList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlHospital hospital)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (hospital != null && ModelState.IsValid)
                {
                    var hospitalInDb = clientDbContext.DdlHospitals
                        .Where(x => x.HospitalId == hospital.HospitalId)
                        .SingleOrDefault();

                    var hospitalIsDefined = clientDbContext.DdlHospitals
                       .Where(x => x.Code == hospital.Code && x.HospitalId != hospital.HospitalId)
                       .SingleOrDefault();

                    if (hospitalIsDefined != null)
                    {
                        ModelState.AddModelError("", "This Hospital is already exists.");
                    }else
                    {
                        if (hospitalInDb != null)
                        {
                            hospitalInDb.Code = hospital.Code;
                            hospitalInDb.Description = hospital.Description;
                            hospitalInDb.AddressLineOne = hospital.AddressLineOne;
                            hospitalInDb.AddressLineTwo = hospital.AddressLineTwo;
                            hospitalInDb.City = hospital.City;
                            hospitalInDb.PhoneNumber = hospital.PhoneNumber;
                            hospitalInDb.FaxNumber = hospital.FaxNumber;
                            hospitalInDb.Contact = hospital.Contact;
                            hospitalInDb.WebAddress = hospital.WebAddress;
                            hospitalInDb.ZipCode = hospital.ZipCode;
                            clientDbContext.SaveChanges();
                        }

                    }

                }

                return Json(new[] { hospital }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlHospitalList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlHospital hospital)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (hospital != null)
                {
                    DdlHospital hospitalInDb = clientDbContext.DdlHospitals
                        .Where(x => x.HospitalId == hospital.HospitalId).SingleOrDefault();

                    if (hospitalInDb != null)
                    {
                        clientDbContext.DdlHospitals.Remove(hospitalInDb);

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

                return Json(new[] { hospital }.ToDataSourceResult(request, ModelState));
            }
        }


        public JsonResult GetHospitals(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var hospitals = clientDbContext.DdlHospitals
                    //.Where(x => x.Active == true)
                    .Select(m => new
                    {
                        HospitalId = m.HospitalId,
                        HospitalDescription = m.Description,

                    }).OrderBy(x => x.HospitalDescription).ToList();

                return Json(hospitals, JsonRequestBehavior.AllowGet);
            }

        }
    }
}