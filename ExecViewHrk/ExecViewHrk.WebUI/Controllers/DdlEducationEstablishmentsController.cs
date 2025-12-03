using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExecViewHrk.WebUI.Infrastructure;
using ExecViewHrk.EfClient;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Net;
using ExecViewHrk.WebUI.Models;
using ExecViewHrk.WebUI.Helpers;
namespace ExecViewHrk.WebUI.Controllers
{
    public class DdlEducationEstablishmentsController : Controller
    {
        // GET: DdlEducationEstablishments
        public ActionResult DdlEducationEstablishmentsListMaintenance()
        {
            //PopulateStates();
            //PopulateContries();
            //PopulateEducationTypes();
            return View();
        }

        //public ActionResult DdlEducationEstablishmentsList_Read([DataSourceRequest]DataSourceRequest request)
        //{
        //    string connString = User.Identity.GetClientConnectionString();

        //    using (ClientDbContext clientDbContext = new ClientDbContext(connString))
        //    {
        //        var educationEstablishmentList = clientDbContext.DdlEducationEstablishments.OrderBy(e => e.Description).ToList();
        //        return Json(educationEstablishmentList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        //    }
        //}

        public ActionResult DdlEducationEstablishmentsList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var educationEstablishmentList = clientDbContext.DdlEducationEstablishments;
                //.OrderBy(e => e.Description).ToList();
                // DdlEducationEstablishmentViewModel vm = new DdlEducationEstablishmentViewModel();
                var vmEducationEstablishment = educationEstablishmentList.Select(x => new DdlEducationEstablishmentViewModel()
                {
                    EducationEstablishmentId = x.EducationEstablishmentId,
                    Description = x.Description,
                    Code = x.Code,
                    AddressLineOne = x.AddressLineOne,
                    AddressLineTwo = x.AddressLineTwo,
                    City = x.City,
                    StateId = x.StateId,
                    ZipCode = x.ZipCode,
                    CountryId = x.CountryId,
                    PhoneNumber = x.PhoneNumber,
                    FaxNumber = x.FaxNumber,
                    EducationTypeId = x.EducationTypeId,
                    AccountNumber = x.AccountNumber,
                    Contact = x.Contact,
                    WebAddress = x.WebAddress,
                    Notes = x.Notes,
                    Active = x.Active,
                    DdlCountry = x.DdlCountry,
                    DdlEducationType = x.DdlEducationType,
                    DdlState = x.DdlState,
                    PersonEducations = x.PersonEducations,
                }).OrderBy(x => x.Description).ToList();
                return Json(vmEducationEstablishment.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlEducationEstablishmentsList_Create([DataSourceRequest] DataSourceRequest request
            , DdlEducationEstablishmentViewModel vmEducationEstablishment)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (vmEducationEstablishment != null && ModelState.IsValid)
                {
                    var educationEstablishmentInDb = clientDbContext.DdlEducationEstablishments
                        .Where(x => x.Code == vmEducationEstablishment.Code)
                        .SingleOrDefault();

                    if (educationEstablishmentInDb != null)
                    {
                        ModelState.AddModelError("", "The Education Establishment" + CustomErrorMessages.ERROR_ALREADY_DEFINED);
                    }
                    else
                    {
                        if (vmEducationEstablishment.StateId == 0) { vmEducationEstablishment.StateId = null; }
                        if (vmEducationEstablishment.CountryId == 0) { vmEducationEstablishment.CountryId = null; }
                        if (vmEducationEstablishment.EducationTypeId == 0) { vmEducationEstablishment.EducationTypeId = null; }

                        var newEducationEstablishment = new DdlEducationEstablishment
                        {
                            Description = vmEducationEstablishment.Description,
                            Code = vmEducationEstablishment.Code,
                            PhoneNumber = vmEducationEstablishment.PhoneNumber,
                            ZipCode = vmEducationEstablishment.ZipCode,
                            StateId = vmEducationEstablishment.StateId,
                            CountryId = vmEducationEstablishment.CountryId,
                            EducationTypeId = vmEducationEstablishment.EducationTypeId,
                            Active = true,
                            AddressLineTwo = vmEducationEstablishment.AddressLineOne,
                            City = vmEducationEstablishment.City,
                            FaxNumber = vmEducationEstablishment.FaxNumber,
                            AccountNumber = vmEducationEstablishment.AccountNumber,
                            Contact = vmEducationEstablishment.Contact,
                            WebAddress = vmEducationEstablishment.WebAddress,
                            Notes = vmEducationEstablishment.Notes,
                        };

                        clientDbContext.DdlEducationEstablishments.Add(newEducationEstablishment);
                        clientDbContext.SaveChanges();
                        vmEducationEstablishment.EducationEstablishmentId = newEducationEstablishment.EducationEstablishmentId;
                    }
                }

                return Json(new[] { vmEducationEstablishment }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlEducationEstablishmentsList_Update([DataSourceRequest] DataSourceRequest request
            , DdlEducationEstablishmentViewModel vmEducationEstablishment)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                try
                {
                    if (vmEducationEstablishment != null && ModelState.IsValid)
                    {
                        var educationEstablishmentInDb = clientDbContext.DdlEducationEstablishments
                            .Where(x => x.EducationEstablishmentId == vmEducationEstablishment.EducationEstablishmentId)
                            .SingleOrDefault();

                        if (educationEstablishmentInDb != null)
                        {
                            educationEstablishmentInDb.Code = vmEducationEstablishment.Code;
                            educationEstablishmentInDb.Description = vmEducationEstablishment.Description;
                            educationEstablishmentInDb.StateId = vmEducationEstablishment.StateId;
                            educationEstablishmentInDb.EducationTypeId = vmEducationEstablishment.EducationTypeId;
                            educationEstablishmentInDb.CountryId = vmEducationEstablishment.CountryId;
                            educationEstablishmentInDb.PhoneNumber = vmEducationEstablishment.PhoneNumber;
                            educationEstablishmentInDb.ZipCode = vmEducationEstablishment.ZipCode;
                            educationEstablishmentInDb.Active = vmEducationEstablishment.Active;
                            educationEstablishmentInDb.AddressLineOne = vmEducationEstablishment.AddressLineOne;
                            educationEstablishmentInDb.AddressLineTwo = vmEducationEstablishment.AddressLineTwo;
                            educationEstablishmentInDb.City = vmEducationEstablishment.City;
                            educationEstablishmentInDb.FaxNumber = vmEducationEstablishment.FaxNumber;
                            educationEstablishmentInDb.AccountNumber = vmEducationEstablishment.AccountNumber;
                            educationEstablishmentInDb.Contact = vmEducationEstablishment.Contact;
                            educationEstablishmentInDb.WebAddress = vmEducationEstablishment.WebAddress;
                            educationEstablishmentInDb.Notes = vmEducationEstablishment.Notes;
                            clientDbContext.SaveChanges();
                        }
                    }
                }
                catch
                {
                    ModelState.AddModelError("Add", "Unable to update");
                }
                return Json(new[] { vmEducationEstablishment }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlEducationEstablishmentsList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlEducationEstablishmentViewModel vmEducationEstablishment)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (vmEducationEstablishment != null)
                {
                    DdlEducationEstablishment educationEstablishmentInDb = clientDbContext.DdlEducationEstablishments
                        .Where(x => x.EducationEstablishmentId == vmEducationEstablishment.EducationEstablishmentId).SingleOrDefault();

                    if (educationEstablishmentInDb != null)
                    {
                        clientDbContext.DdlEducationEstablishments.Remove(educationEstablishmentInDb);

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

                return Json(new[] { vmEducationEstablishment }.ToDataSourceResult(request, ModelState));
            }
        }

        public JsonResult GetEducationEstablishments(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var educationEstablishments = clientDbContext.DdlEducationEstablishments
                    .Where(x => x.Active == true)
                    .Select(m => new
                    {
                        EducationEstablishmentId = m.EducationEstablishmentId,
                        EducationDescription = m.Description,
                        EducationCode = m.Code,
                        EducationAddressLineOne = m.AddressLineOne,
                        EducationAddressLineTwo = m.AddressLineTwo,
                        EducationCity = m.City,
                        EducationStateId = m.StateId,
                        EducationZipCode = m.ZipCode,
                        EducationCountryId = m.CountryId,
                        EducationPhoneNumber = m.PhoneNumber,
                        EducationFaxNumber = m.FaxNumber,
                        EducationTypeId = m.EducationTypeId,
                        EducationAccountNumber = m.AccountNumber,
                        EducationContact = m.Contact,
                        EducationWebAddress = m.WebAddress,
                        EducationNotes = m.Notes,
                        EducationActive = m.Active,
                        EducationDdlCountry = m.DdlCountry,
                        EducationDdlEducationType = m.DdlEducationType,
                        EducationDdlState = m.DdlState,
                        EducationPersonEducations = m.PersonEducations,
                    }).OrderBy(x => x.EducationDescription).ToList();

                var vmEducationEstablishment = educationEstablishments.Select(x => new DdlEducationEstablishmentViewModel()
                {
                    EducationEstablishmentId = x.EducationEstablishmentId,
                    Description = x.EducationDescription,
                    Code = x.EducationCode,
                    AddressLineOne = x.EducationAddressLineOne,
                    AddressLineTwo = x.EducationAddressLineTwo,
                    City = x.EducationCity,
                    StateId = x.EducationStateId,
                    ZipCode = x.EducationZipCode,
                    CountryId = x.EducationCountryId,
                    PhoneNumber = x.EducationPhoneNumber,
                    FaxNumber = x.EducationFaxNumber,
                    EducationTypeId = x.EducationTypeId,
                    AccountNumber = x.EducationAccountNumber,
                    Contact = x.EducationContact,
                    WebAddress = x.EducationWebAddress,
                    Notes = x.EducationNotes,
                    Active = x.EducationActive,
                    DdlCountry = x.EducationDdlCountry,
                    DdlEducationType = x.EducationDdlEducationType,
                    DdlState = x.EducationDdlState,
                    PersonEducations = x.EducationPersonEducations,
                }).OrderBy(x => x.Description).ToList();

                return Json(vmEducationEstablishment, JsonRequestBehavior.AllowGet);
            }

        }

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult DdlEducationEstablishmentsList_Create([DataSourceRequest] DataSourceRequest request
        //    , ExecViewHrk.EfClient.DdlEducationEstablishment educationEstablishment)
        //{
        //    string connString = User.Identity.GetClientConnectionString();
        //    using (ClientDbContext clientDbContext = new ClientDbContext(connString))
        //    {
        //        if (educationEstablishment != null && ModelState.IsValid)
        //        {
        //            var educationEstablishmentInDb = clientDbContext.DdlEducationEstablishments
        //                .Where(x => x.Code == educationEstablishment.Code)
        //                .SingleOrDefault();

        //            if (educationEstablishmentInDb != null)
        //            {
        //                ModelState.AddModelError("", "The Address Type" + CustomErrorMessages.ERROR_ALREADY_DEFINED);
        //            }
        //            else
        //            {
        //                if (educationEstablishment.StateId == 0)
        //                {
        //                    educationEstablishment.StateId = null;
        //                }
        //                if (educationEstablishment.CountryId == 0)
        //                {
        //                    educationEstablishment.CountryId = null;
        //                }
        //                if (educationEstablishment.EducationTypeId == 0)
        //                {
        //                    educationEstablishment.EducationTypeId = null;
        //                }
        //                var newEducationEstablishment = new DdlEducationEstablishment
        //                {
        //                    Description = educationEstablishment.Description,
        //                    Code = educationEstablishment.Code,
        //                    PhoneNumber = educationEstablishment.PhoneNumber,
        //                    ZipCode = educationEstablishment.ZipCode,
        //                    StateId = educationEstablishment.StateId,
        //                    CountryId = educationEstablishment.CountryId,
        //                    EducationTypeId = educationEstablishment.EducationTypeId,
        //                    Active = true,
        //                    AddressLineTwo = educationEstablishment.AddressLineOne,
        //                    City = educationEstablishment.City,
        //                    FaxNumber = educationEstablishment.FaxNumber,
        //                    AccountNumber = educationEstablishment.AccountNumber,
        //                    Contact = educationEstablishment.Contact,
        //                    WebAddress = educationEstablishment.WebAddress,
        //                    Notes = educationEstablishment.Notes,
        //                };

        //                clientDbContext.DdlEducationEstablishments.Add(newEducationEstablishment);
        //                clientDbContext.SaveChanges();
        //                educationEstablishment.EducationEstablishmentId = newEducationEstablishment.EducationEstablishmentId;
        //            }
        //        }

        //        return Json(new[] { educationEstablishment }.ToDataSourceResult(request, ModelState));
        //    }
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult DdlEducationEstablishmentsList_Update([DataSourceRequest] DataSourceRequest request
        //    , ExecViewHrk.EfClient.DdlEducationEstablishment educationEstablishment)//, byte StateId)
        //{
        //    string connString = User.Identity.GetClientConnectionString();
        //    using (ClientDbContext clientDbContext = new ClientDbContext(connString))
        //    {
        //        if (educationEstablishment != null && ModelState.IsValid)
        //        {
        //            var educationEstablishmentInDb = clientDbContext.DdlEducationEstablishments
        //                .Where(x => x.EducationEstablishmentId == educationEstablishment.EducationEstablishmentId)
        //                .SingleOrDefault();

        //            if (educationEstablishmentInDb != null)
        //            {
        //                educationEstablishmentInDb.Code = educationEstablishment.Code;
        //                educationEstablishmentInDb.Description = educationEstablishment.Description;
        //                //var stateId = ValueProvider.GetValue("StateId");
        //                //if (stateId == null)

        //                    educationEstablishmentInDb.StateId = educationEstablishment.StateId;



        //                    educationEstablishmentInDb.CountryId = educationEstablishment.CountryId;



        //                    educationEstablishmentInDb.EducationTypeId = educationEstablishment.EducationTypeId;


        //                educationEstablishmentInDb.CountryId = educationEstablishment.CountryId; 
        //                educationEstablishmentInDb.PhoneNumber = educationEstablishment.PhoneNumber;
        //                educationEstablishmentInDb.ZipCode = educationEstablishment.ZipCode;
        //                educationEstablishmentInDb.Active = educationEstablishment.Active;
        //                educationEstablishmentInDb.AddressLineOne = educationEstablishment.AddressLineOne;
        //                educationEstablishmentInDb.AddressLineTwo = educationEstablishment.AddressLineTwo;
        //                educationEstablishmentInDb.City = educationEstablishment.City;
        //                educationEstablishmentInDb.FaxNumber = educationEstablishment.FaxNumber;
        //                educationEstablishmentInDb.AccountNumber = educationEstablishment.AccountNumber;
        //                educationEstablishmentInDb.Contact = educationEstablishment.Contact;
        //                educationEstablishmentInDb.WebAddress = educationEstablishment.WebAddress;
        //                educationEstablishmentInDb.Notes = educationEstablishment.Notes;
        //                clientDbContext.SaveChanges();
        //            }
        //        }

        //        return Json(new[] { educationEstablishment }.ToDataSourceResult(request, ModelState));
        //    }
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult DdlEducationEstablishmentsList_Destroy([DataSourceRequest] DataSourceRequest request
        //    , DdlEducationEstablishment educationEstablishment)
        //{
        //    string connString = User.Identity.GetClientConnectionString();
        //    using (ClientDbContext clientDbContext = new ClientDbContext(connString))
        //    {
        //        if (educationEstablishment != null)
        //        {
        //            DdlEducationEstablishment educationEstablishmentInDb = clientDbContext.DdlEducationEstablishments
        //                .Where(x => x.EducationEstablishmentId == educationEstablishment.EducationEstablishmentId).SingleOrDefault();

        //            if (educationEstablishmentInDb != null)
        //            {
        //                clientDbContext.DdlEducationEstablishments.Remove(educationEstablishmentInDb);

        //                try
        //                {
        //                    clientDbContext.SaveChanges();
        //                }
        //                catch (Exception err)
        //                {
        //                    ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
        //                }

        //            }
        //        }

        //        return Json(new[] { educationEstablishment }.ToDataSourceResult(request, ModelState));
        //    }
        //}

        //public JsonResult GetEducationEstablishments(string text)
        //{
        //    string connString = User.Identity.GetClientConnectionString();

        //    using (ClientDbContext clientDbContext = new ClientDbContext(connString))
        //    {

        //        var educationEstablishments = clientDbContext.DdlEducationEstablishments
        //            .Where(x => x.Active == true)
        //            .Select(m => new
        //            {
        //                EducationEstablishmentId = m.EducationEstablishmentId,
        //                EducationEstablishmentDescription = m.Description
        //            }).OrderBy(x => x.EducationEstablishmentDescription).ToList();

        //        return Json(educationEstablishments, JsonRequestBehavior.AllowGet);
        //    }

        //}

        private void PopulateStates()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var statesList = new ClientDbContext(connString).DdlStates
                        .Select(s => new StateDdlEntryViewModel
                        {
                            StateId = s.StateId,
                            Title = s.Title //+ "-" + e.SkillId.ToString()
                        })
                        .OrderBy(s => s.Title).ToList();

                statesList.Insert(0, new StateDdlEntryViewModel { StateId = 0, Title = "--select one--" });


                ViewData["statesList"] = statesList;
               // ViewData["defaultState"] = statesList.First();
            }
        }


        private void PopulateContries()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var countriesList = new ClientDbContext(connString).DdlCountries
                            .Select(c => new CountryDdlEntryViewModel
                            {
                                CountryId = c.CountryId,
                                Description = c.Description //+ "-" + e.SkillId.ToString()
                            })
                            .OrderBy(c => c.Description).ToList();

                countriesList.Insert(0, new CountryDdlEntryViewModel { CountryId = 0, Description = "--select one--" });

                ViewData["countriesList"] = countriesList;
                //ViewData["defaultCountry"] = countriesList.First();
            }
        }

        private void PopulateEducationTypes()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var educationTypesList = new ClientDbContext(connString).DdlEducationTypes
                            .Select(e => new EducationTypeDdlEntryViewModel
                            {
                                EducationTypeId = e.EducationTypeId,
                                Description = e.Description //+ "-" + e.SkillId.ToString()
                            })
                            .OrderBy(e => e.Description).ToList();

                educationTypesList.Insert(0, new EducationTypeDdlEntryViewModel { EducationTypeId = 0, Description = "--select one--" });

                ViewData["educationTypesList"] = educationTypesList;
                //ViewData["defaultCountry"] = educationTypesList.First();
            }
        }
    }
}