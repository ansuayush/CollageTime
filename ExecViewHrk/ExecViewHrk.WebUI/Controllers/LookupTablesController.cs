using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using ExecViewHrk.WebUI.Models;
using System.Data.Entity.Validation;
using ExecViewHrk.Models;
using ExecViewHrk.Domain.Interface;
using Newtonsoft.Json;

using Microsoft.AspNet.Identity;
using ExecViewHrk.WebUI.Infrastructure;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System.Data.SqlClient;

namespace ExecViewHrk.WebUI.Controllers
{
    [Authorize]
    public class LookupTablesController : Controller
    {
        private ILookupTablesRepository _lookupTablesRepository;
        readonly ITimeCardMatrixReposotory _timeCardsMatrixRepo;
        public LookupTablesController(ILookupTablesRepository lookupTablesRepository,
                                      ITimeCardMatrixReposotory timeCardsMatrixRepo)
        {
            _lookupTablesRepository = lookupTablesRepository;
            _timeCardsMatrixRepo = timeCardsMatrixRepo;
        }
        // GET: LookupTables
        public ActionResult Index()
        {
            return View();
        }

        #region "Gender"

        public ActionResult AddGender()
        {
            var model = new DdlGender() { Active = true };
            return View(model);
        }

        public ActionResult DdlGenderListMaintenance()
        {
            return View();
        }

        public JsonResult GetDdlGenderList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var maritalStatusList = clientDbContext.DdlGenders.Where(x => x.Active == true).OrderBy(e => e.Description).ToList();

            return Json(maritalStatusList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DdlGenderList_Read([DataSourceRequest]DataSourceRequest request)
        {
            try
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);

                var genderList = clientDbContext.DdlGenders.OrderBy(e => e.Description).ToList();

                return Json(genderList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                //Response.StatusCode = (int)HttpStatusCode.BadRequest;

                //return Json(new DataSourceResult
                //{
                //    Errors = new Dictionary<string, string> {{"xxx","my custom error"}}
                //});

                return Json(new DataSourceResult
                {
                    Errors = err.Message
                });
                //return Json("error", JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlGenderList_Create([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfClient.DdlGender gender)
        {
            if (gender != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var genderInDb = clientDbContext.DdlGenders.Where(x => x.Code == gender.Code || x.Description == gender.Description).SingleOrDefault();

                if (genderInDb != null)
                {
                    ModelState.AddModelError("", "Gender" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                }
                else
                {
                    var newGender = new DdlGender { Description = gender.Description, Code = gender.Code, Active = gender.Active };
                    clientDbContext.DdlGenders.Add(newGender);
                    clientDbContext.SaveChanges();
                    gender.GenderId = newGender.GenderId;
                }
            }

            return Json(new[] { gender }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlGenderList_Update([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfClient.DdlGender gender)
        {
            if (gender != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var genderInDb = clientDbContext.DdlGenders.Where(x => x.GenderId != gender.GenderId && (x.Code == gender.Code || x.Description == gender.Description)).SingleOrDefault();
                if (genderInDb == null)
                {
                    genderInDb = clientDbContext.DdlGenders.Where(x => x.GenderId == gender.GenderId).SingleOrDefault();

                    if (genderInDb.Active != gender.Active && gender.Active == false)
                    {
                        if (clientDbContext.Persons.Where(x => x.GenderId == gender.GenderId).Count() == 0)
                        {
                            genderInDb.Code = gender.Code;
                            genderInDb.Description = gender.Description;
                            genderInDb.Active = gender.Active;
                            clientDbContext.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                        }
                    }
                    else
                    {
                        genderInDb.Code = gender.Code;
                        genderInDb.Description = gender.Description;
                        genderInDb.Active = gender.Active;
                        clientDbContext.SaveChanges();
                    }
                }
                else
                {
                    ModelState.AddModelError("", string.Format(CustomErrorMessages.ERROR_LOOKUP_DUPLICATE_RECORD, "Gender"));
                }
            }

            return Json(new[] { gender }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlGenderList_Destroy([DataSourceRequest] DataSourceRequest request, DdlGender gender)
        {
            if (gender != null)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                if (clientDbContext.Persons.Where(x => x.GenderId == gender.GenderId).Count() > 0)
                {
                    ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                }
                else
                {
                    DdlGender GenderInDb = clientDbContext.DdlGenders.Where(x => x.GenderId == gender.GenderId).SingleOrDefault();
                    clientDbContext.DdlGenders.Remove(GenderInDb);
                    try
                    {
                        clientDbContext.SaveChanges();
                    }
                    catch { ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE); }
                }
            }

            return Json(new[] { gender }.ToDataSourceResult(request, ModelState));
        }

        #endregion "Gender"

        #region "Marital Status"

        public ActionResult AddMaritalStatus()
        {
            var model = new DdlMaritalStatus() { Active = true };
            return View(model);
        }

        public ActionResult DdlMaritalStatusListMaintenance()
        {
            return View();
        }

        public ActionResult DdlMaritalStatusList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var maritalStatusList = clientDbContext.DdlMaritalStatuses.OrderBy(e => e.Description).ToList();

            return Json(maritalStatusList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDdlMaritalStatusList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var maritalStatusList = clientDbContext.DdlMaritalStatuses.Where(x => x.Active == true).OrderBy(e => e.Description).ToList();

            return Json(maritalStatusList, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlMaritalStatusList_Create([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfClient.DdlMaritalStatus maritalStatus)
        {
            if (maritalStatus != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var StatusInDb = clientDbContext.DdlMaritalStatuses.Where(x => x.Code == maritalStatus.Code || x.Description == maritalStatus.Description).SingleOrDefault();

                if (StatusInDb != null)
                {
                    ModelState.AddModelError("", "Marital Status" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                }
                else
                {
                    var newMaritalStatus = new DdlMaritalStatus { Description = maritalStatus.Description, Code = maritalStatus.Code, Active = true };
                    clientDbContext.DdlMaritalStatuses.Add(newMaritalStatus);
                    clientDbContext.SaveChanges();
                    maritalStatus.MaritalStatusId = newMaritalStatus.MaritalStatusId;
                }
            }

            return Json(new[] { maritalStatus }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlMaritalStatusList_Update([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfClient.DdlMaritalStatus maritalStatus)
        {
            if (maritalStatus != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);

                var maritalStatusInDb = clientDbContext.DdlMaritalStatuses.Where(x => x.MaritalStatusId != maritalStatus.MaritalStatusId && (x.Code == maritalStatus.Code || x.Description == maritalStatus.Description)).SingleOrDefault();
                if (maritalStatusInDb == null)
                {
                    maritalStatusInDb = clientDbContext.DdlMaritalStatuses.Where(x => x.MaritalStatusId == maritalStatus.MaritalStatusId).SingleOrDefault();

                    if (maritalStatusInDb.Active != maritalStatus.Active && maritalStatus.Active == false)
                    {
                        if (clientDbContext.Persons.Where(x => x.ActualMaritalStatusId == maritalStatus.MaritalStatusId).Count() == 0
                            && clientDbContext.Employees.Where(x => x.MaritalStatusID == maritalStatus.MaritalStatusId).Count() == 0)
                        {
                            maritalStatusInDb.Code = maritalStatus.Code;
                            maritalStatusInDb.Description = maritalStatus.Description;
                            maritalStatusInDb.Active = maritalStatus.Active;
                            clientDbContext.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                        }
                    }
                    else
                    {
                        maritalStatusInDb.Code = maritalStatus.Code;
                        maritalStatusInDb.Description = maritalStatus.Description;
                        maritalStatusInDb.Active = maritalStatus.Active;
                        clientDbContext.SaveChanges();
                    }
                }
                else
                {
                    ModelState.AddModelError("", string.Format(CustomErrorMessages.ERROR_LOOKUP_DUPLICATE_RECORD, "Marital Status"));
                }
            }

            return Json(new[] { maritalStatus }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlMaritalStatusList_Destroy([DataSourceRequest] DataSourceRequest request, DdlMaritalStatus maritalStatus)
        {
            if (maritalStatus != null)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                DdlMaritalStatus maritalStatusInDb = clientDbContext.DdlMaritalStatuses.Where(x => x.MaritalStatusId == maritalStatus.MaritalStatusId).SingleOrDefault();

                if (maritalStatusInDb != null)
                {
                    clientDbContext.DdlMaritalStatuses.Remove(maritalStatusInDb);
                    try
                    {
                        clientDbContext.SaveChanges();
                    }
                    catch
                    {
                        ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                    }
                }
            }

            return Json(new[] { maritalStatus }.ToDataSourceResult(request, ModelState));
        }

        #endregion "Marital Status"

        #region "Prefix"

        public ActionResult AddPrefix()
        {
            var model = new DdlPrefix() { Active = true };
            return View(model);
        }

        public ActionResult DdlPrefixListMaintenance()
        {
            return View();
        }

        public ActionResult DdlPrefixList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var prefixList = clientDbContext.DdlPrefixes.OrderBy(e => e.Description).ToList();

            return Json(prefixList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDdlPrefixList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var prefixList = clientDbContext.DdlPrefixes.Where(x => x.Active == true).OrderBy(e => e.Description).ToList();

            return Json(prefixList, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlPrefixList_Create([DataSourceRequest] DataSourceRequest request, DdlPrefix prefix)
        {
            if (prefix != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var prefixInDb = clientDbContext.DdlPrefixes
                    .Where(x => x.Code == prefix.Code || x.Description == prefix.Description)
                    .SingleOrDefault();

                if (prefixInDb != null)
                {
                    ModelState.AddModelError("", "Prefix" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                }
                else
                {
                    var newPrefix = new DdlPrefix { Description = prefix.Description, Code = prefix.Code, Active = prefix.Active };
                    clientDbContext.DdlPrefixes.Add(newPrefix);
                    clientDbContext.SaveChanges();
                    prefix.PrefixId = newPrefix.PrefixId;
                }
            }

            return Json(new[] { prefix }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlPrefixList_Update([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfClient.DdlPrefix prefix)
        {
            if (prefix != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);

                var PrefixInDb = clientDbContext.DdlPrefixes.Where(x => x.PrefixId != prefix.PrefixId && (x.Code == prefix.Code || x.Description == prefix.Description)).SingleOrDefault();
                if (PrefixInDb == null)
                {
                    PrefixInDb = clientDbContext.DdlPrefixes.Where(x => x.PrefixId == prefix.PrefixId).SingleOrDefault();

                    if (PrefixInDb.Active != prefix.Active && prefix.Active == false)
                    {
                        if (clientDbContext.Persons.Where(x => x.PrefixId == prefix.PrefixId).Count() == 0)
                        {
                            PrefixInDb.Code = prefix.Code;
                            PrefixInDb.Description = prefix.Description;
                            PrefixInDb.Active = prefix.Active;
                            clientDbContext.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                        }
                    }
                    else
                    {
                        PrefixInDb.Code = prefix.Code;
                        PrefixInDb.Description = prefix.Description;
                        PrefixInDb.Active = prefix.Active;
                        clientDbContext.SaveChanges();
                    }
                }
                else
                {
                    ModelState.AddModelError("", string.Format(CustomErrorMessages.ERROR_LOOKUP_DUPLICATE_RECORD, "Prefix"));
                }
            }

            return Json(new[] { prefix }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlPrefixList_Destroy([DataSourceRequest] DataSourceRequest request, DdlPrefix prefix)
        {
            if (prefix != null)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                if (clientDbContext.Persons.Where(x => x.PrefixId == prefix.PrefixId).Count() > 0)
                {
                    ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                }
                else
                {
                    DdlPrefix PrefixInDb = clientDbContext.DdlPrefixes.Where(x => x.PrefixId == prefix.PrefixId).SingleOrDefault();
                    clientDbContext.DdlPrefixes.Remove(PrefixInDb);
                    try
                    {
                        clientDbContext.SaveChanges();
                    }
                    catch { ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE); }
                }
            }

            return Json(new[] { prefix }.ToDataSourceResult(request, ModelState));
        }

        #endregion "Prefix"

        #region "Suffix"

        public ActionResult AddSuffix()
        {
            var model = new DdlSuffix() { Active = true };
            return View(model);
        }

        public ActionResult DdlSuffixListMaintenance()
        {
            return View();
        }

        public ActionResult DdlSuffixList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var suffixList = clientDbContext.DdlSuffixes.OrderBy(e => e.Description).ToList();

            return Json(suffixList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDdlSuffixList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var suffixList = clientDbContext.DdlSuffixes.Where(x => x.Active == true).OrderBy(e => e.Description).ToList();

            return Json(suffixList, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlSuffixList_Create([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfClient.DdlSuffix suffix)
        {
            if (suffix != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var suffixInDb = clientDbContext.DdlSuffixes.Where(x => x.Code == suffix.Code || x.Description == suffix.Description).SingleOrDefault();

                if (suffixInDb != null)
                {
                    ModelState.AddModelError("", "Suffix" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                }
                else
                {
                    var newSuffix = new DdlSuffix
                    {
                        Description = suffix.Description,
                        Code = suffix.Code,
                        Active = true
                    };
                    clientDbContext.DdlSuffixes.Add(newSuffix);
                    clientDbContext.SaveChanges();
                    suffix.SuffixId = newSuffix.SuffixId;
                }
            }

            return Json(new[] { suffix }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlSuffixList_Update([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfClient.DdlSuffix suffix)
        {
            if (suffix != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);

                var suffixInDb = clientDbContext.DdlSuffixes.Where(x => x.SuffixId != suffix.SuffixId && (x.Code == suffix.Code || x.Description == suffix.Description)).SingleOrDefault();
                if (suffixInDb == null)
                {
                    suffixInDb = clientDbContext.DdlSuffixes.Where(x => x.SuffixId == suffix.SuffixId).SingleOrDefault();

                    if (suffixInDb.Active != suffix.Active && suffix.Active == false)
                    {
                        if (clientDbContext.Persons.Where(x => x.SuffixId == suffix.SuffixId).Count() == 0)
                        {
                            suffixInDb.Code = suffix.Code;
                            suffixInDb.Description = suffix.Description;
                            suffixInDb.Active = suffix.Active;
                            clientDbContext.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                        }
                    }
                    else
                    {
                        suffixInDb.Code = suffix.Code;
                        suffixInDb.Description = suffix.Description;
                        suffixInDb.Active = suffix.Active;
                        clientDbContext.SaveChanges();
                    }
                }
                else
                {
                    ModelState.AddModelError("", string.Format(CustomErrorMessages.ERROR_LOOKUP_DUPLICATE_RECORD, "Suffix"));
                }
            }

            return Json(new[] { suffix }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlSuffixList_Destroy([DataSourceRequest] DataSourceRequest request, DdlSuffix suffix)
        {
            if (suffix != null)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);

                if (clientDbContext.Persons.Where(x => x.SuffixId == suffix.SuffixId).Count() > 0)
                {
                    ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                }
                else
                {
                    DdlSuffix suffixInDb = clientDbContext.DdlSuffixes.Where(x => x.SuffixId == suffix.SuffixId).SingleOrDefault();
                    clientDbContext.DdlSuffixes.Remove(suffixInDb);
                    try
                    {
                        clientDbContext.SaveChanges();
                    }
                    catch { ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE); }
                }
            }

            return Json(new[] { suffix }.ToDataSourceResult(request, ModelState));
        }

        #endregion "Suffix"

        #region "AccommodationTypes"
        public ActionResult DdlAccommodationTypesListMaintenance()
        {
            return View();
        }

        public JsonResult DdlAccommodationTypesList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var accommodationTypeList = clientDbContext.DdlAccommodationTypes.OrderBy(e => e.Description).ToList();
                return Json(accommodationTypeList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlAccommodationTypesList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlAccommodationType accommodationType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (accommodationType != null && ModelState.IsValid)
                {
                    var accommodationTypeInDb = clientDbContext.DdlAccommodationTypes
                        .Where(x => x.Code == accommodationType.Code || x.Description == accommodationType.Description)
                        .SingleOrDefault();

                    if (accommodationTypeInDb != null)
                    {
                        ModelState.AddModelError("", "Accommodation Type" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                    }
                    else
                    {
                        var newAccommodationType = new DdlAccommodationType
                        {
                            Description = accommodationType.Description,
                            Code = accommodationType.Code,
                            Active = accommodationType.Active
                        };

                        clientDbContext.DdlAccommodationTypes.Add(newAccommodationType);
                        clientDbContext.SaveChanges();
                        accommodationType.AccommodationTypeId = newAccommodationType.AccommodationTypeId;
                    }
                }

                return Json(new[] { accommodationType }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlAccommodationTypesList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlAccommodationType accommodationType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var duplicateAccommodationInDb = clientDbContext.DdlAccommodationTypes
                            .Where(x => x.AccommodationTypeId != accommodationType.AccommodationTypeId
                            && (x.Code.Trim() == accommodationType.Code.Trim() || x.Description.Trim() == accommodationType.Description.Trim()))
                            .SingleOrDefault();
                if (duplicateAccommodationInDb == null)
                {
                    if (accommodationType != null && ModelState.IsValid)
                    {
                        var accommodationTypeInDb = clientDbContext.DdlAccommodationTypes.Where(x => x.AccommodationTypeId == accommodationType.AccommodationTypeId).SingleOrDefault();

                        if (accommodationTypeInDb != null)
                        {
                            accommodationTypeInDb.Code = accommodationType.Code;
                            accommodationTypeInDb.Description = accommodationType.Description;
                            //accommodationTypeInDb.Active = accommodationType.Active;

                            if (accommodationTypeInDb.Active != accommodationType.Active && accommodationType.Active == false)
                            {
                                if (clientDbContext.PersonADAs.Where(x => x.AccommodationTypeId == accommodationType.AccommodationTypeId).Count() == 0)
                                {
                                    accommodationTypeInDb.Active = accommodationType.Active;
                                    clientDbContext.SaveChanges();
                                }
                                else
                                {
                                    ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                                }
                            }
                            else
                            {
                                accommodationTypeInDb.Active = accommodationType.Active;
                                clientDbContext.SaveChanges();
                            }
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Accommodation Type" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                }

                return Json(new[] { accommodationType }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlAccommodationTypesList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlAccommodationType accommodationType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (accommodationType != null)
                {
                    DdlAccommodationType accommodationTypeInDb = clientDbContext.DdlAccommodationTypes
                        .Where(x => x.AccommodationTypeId == accommodationType.AccommodationTypeId).SingleOrDefault();

                    if (accommodationTypeInDb != null)
                    {
                        clientDbContext.DdlAccommodationTypes.Remove(accommodationTypeInDb);

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

                return Json(new[] { accommodationType }.ToDataSourceResult(request, ModelState));
            }
        }

        public JsonResult GetAccommodationTypes()
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var accommodationTypes = clientDbContext.DdlAccommodationTypes
                    .Where(x => x.Active == true)
                    .Select(m => new
                    {
                        AccommodationTypeId = m.AccommodationTypeId,
                        AccommodationDescription = m.Description
                    }).OrderBy(x => x.AccommodationDescription).ToList();

                return Json(accommodationTypes, JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult DdlAccommodationTypesPartial()
        {
            return PartialView();
        }

        #endregion

        #region "Disability Types"

        public ActionResult DdlDisabilityTypesListMaintenance()
        {
            return View();
        }

        public ActionResult DdlDisabilityTypesList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var disabilityTypeList = clientDbContext.DdlDisabilityTypes.OrderBy(e => e.Description).ToList();
                return Json(disabilityTypeList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlDisabilityTypesList_Create([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfClient.DdlDisabilityType disabilityType)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var DisableInDb = clientDbContext.DdlDisabilityTypes.Where(x => x.Code == disabilityType.Code || x.Description == disabilityType.Description).ToList();
            if (DisableInDb.Count == 0)
            {

                if (disabilityType != null && ModelState.IsValid)
                {
                    var disabilityTypeInDb = clientDbContext.DdlDisabilityTypes
                        .Where(x => x.Code == disabilityType.Code)
                        .SingleOrDefault();

                    if (disabilityTypeInDb != null)
                    {
                        ModelState.AddModelError("", "Disability Type" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                    }
                    else
                    {
                        var newDisabilityType = new DdlDisabilityType
                        {
                            Description = disabilityType.Description
                            ,
                            Code = disabilityType.Code,
                            Active = true
                        };

                        clientDbContext.DdlDisabilityTypes.Add(newDisabilityType);
                        clientDbContext.SaveChanges();
                        disabilityType.DisabilityTypeId = newDisabilityType.DisabilityTypeId;
                    }
                }

                return Json(new[] { disabilityType }.ToDataSourceResult(request, ModelState));
            }
            else
            {
                ModelState.AddModelError("", string.Format(CustomErrorMessages.ERROR_LOOKUP_DUPLICATE_RECORD, "Associated Disability"));
            }
            return Json(new[] { disabilityType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlDisabilityTypesList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlDisabilityType disabilityType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (disabilityType != null && ModelState.IsValid)
                {
                    var disabilityTypeInDb = clientDbContext.DdlDisabilityTypes
                        .Where(x => x.DisabilityTypeId == disabilityType.DisabilityTypeId)
                        .SingleOrDefault();

                    if (disabilityTypeInDb != null)
                    {
                        disabilityTypeInDb.Code = disabilityType.Code;
                        disabilityTypeInDb.Description = disabilityType.Description;
                        disabilityTypeInDb.Active = disabilityType.Active;
                        clientDbContext.SaveChanges();
                    }
                }

                return Json(new[] { disabilityType }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlDisabilityTypesList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlDisabilityType disabilityType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (disabilityType != null)
                {
                    DdlDisabilityType disabilityTypeInDb = clientDbContext.DdlDisabilityTypes
                        .Where(x => x.DisabilityTypeId == disabilityType.DisabilityTypeId).SingleOrDefault();

                    if (disabilityTypeInDb != null)
                    {
                        clientDbContext.DdlDisabilityTypes.Remove(disabilityTypeInDb);
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

                return Json(new[] { disabilityType }.ToDataSourceResult(request, ModelState));
            }
        }

        public JsonResult GetDisabilityTypes()
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var disabilityTypes = clientDbContext.DdlDisabilityTypes
                    .Where(x => x.Active == true)
                    .Select(m => new
                    {
                        AssociatedDisabilityTypeId = m.DisabilityTypeId,
                        AssociatedDisabilityDescription = m.Description
                    }).OrderBy(x => x.AssociatedDisabilityDescription).ToList();

                return Json(disabilityTypes, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult AddLicenseType()
        {
            var model = new DdlLicenseType() { Active = true };
            return View(model);
        }

        #endregion

        public ActionResult AddEvaluationTest()
        {
            var model = new DdlEvaluationTest() { Active = true };
            return View(model);
        }

        public ActionResult AddMedicalExaminationType()
        {
            var model = new DdlMedicalExaminationType() { Active = true };
            return View(model);
        }
        public ActionResult AddInnoculationTypes()
        {
            var model = new DdlInnoculationType() { Active = true };

            return View(model);
        }
        public ActionResult AddPropertiesTypes()
        {
            var model = new DdlPropertyType() { Active = true };

            return View(model);
        }
        public ActionResult AddProfessionalBody()
        {
            var model = new DdlProfessionalBody() { Active = true };
            return View(model);
        }

        public ActionResult AddRegionalChapter()
        {
            var model = new DdlRegionalChapter() { Active = true };
            return View(model);
        }

        public ActionResult AddAddressType()
        {
            var model = new DdlAddressType() { Active = true };
            return View(model);
        }

        public ActionResult AddState()
        {
            var model = new DdlState();
            return View(model);
        }

        public ActionResult AddCountry()
        {
            var model = new DdlCountry() { Active = true };
            return View(model);
        }

        #region "Address Types"
        public JsonResult GetAddressTypes()
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var addressTypes = clientDbContext.DdlAddressTypes
                    .Where(x => x.Active == true)
                    .Select(m => new
                    {
                        AddressTypeId = m.AddressTypeId,
                        AddressDescription = m.Description
                    }).OrderBy(x => x.AddressDescription).ToList();

                return Json(addressTypes, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlAddressType_Create([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfClient.DdlAddressType ddlAddressType)
        {
            if (ddlAddressType != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var StatusInDb = clientDbContext.DdlAddressTypes
                    .Where(x => x.Code == ddlAddressType.Code)
                    .SingleOrDefault();

                if (StatusInDb != null)
                {
                    ModelState.AddModelError("", "Address Type" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                }
                else
                {
                    var newAddressType = new DdlAddressType { Description = ddlAddressType.Description, Code = ddlAddressType.Code, Active = true };
                    clientDbContext.DdlAddressTypes.Add(newAddressType);
                    clientDbContext.SaveChanges();
                    ddlAddressType.AddressTypeId = newAddressType.AddressTypeId;
                }
            }

            return Json(new[] { ddlAddressType }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult GetDdlAddressTypeList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var addressTypeList = clientDbContext.DdlAddressTypes.Where(x => x.Active == true).OrderBy(e => e.Description).ToList();

            return Json(addressTypeList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region "State"

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlAddState_Create([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfClient.DdlState ddlState)
        {
            if (ddlState != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var StatusInDb = clientDbContext.DdlStates
                    .Where(x => x.Code == ddlState.Code)
                    .SingleOrDefault();

                if (StatusInDb != null)
                {
                    ModelState.AddModelError("", "State" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                }
                else
                {
                    var newState = new DdlState { Title = ddlState.Title, Code = ddlState.Code };
                    clientDbContext.DdlStates.Add(newState);
                    clientDbContext.SaveChanges();
                    ddlState.StateId = newState.StateId;
                }
            }

            return Json(new[] { ddlState }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult GetDdlStateList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var addressTypeList = clientDbContext.DdlStates.OrderBy(e => e.Title).ToList();

            return Json(addressTypeList, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlCountry_Create([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfClient.DdlCountry ddlCountry)
        {
            if (ddlCountry != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var StatusInDb = clientDbContext.DdlCountries
                    .Where(x => x.Code == ddlCountry.Code)
                    .SingleOrDefault();

                if (StatusInDb != null)
                {
                    ModelState.AddModelError("", "Address Type" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                }
                else
                {
                    var newCountryType = new DdlCountry { Description = ddlCountry.Description, Code = ddlCountry.Code, Active = true };
                    clientDbContext.DdlCountries.Add(newCountryType);
                    clientDbContext.SaveChanges();
                    ddlCountry.CountryId = newCountryType.CountryId;
                }
            }

            return Json(new[] { ddlCountry }.ToDataSourceResult(request, ModelState));
        }

        #endregion

        public JsonResult GetDdlCountryList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var addressTypeList = clientDbContext.DdlCountries.Where(x => x.Active == true).OrderBy(e => e.Description).ToList();

            return Json(addressTypeList, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlLicenseType_Create([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfClient.DdlLicenseType ddlLicenseType)
        {

            if (ddlLicenseType != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var StatusInDb = clientDbContext.DdlLicenseTypes
                    .Where(x => x.Code == ddlLicenseType.Code)
                    .SingleOrDefault();

                if (StatusInDb != null)
                {
                    ModelState.AddModelError("", "The Address Type" + CustomErrorMessages.ERROR_ALREADY_DEFINED);
                }
                else
                {
                    var newLicenseType = new DdlLicenseType { Description = ddlLicenseType.Description, Code = ddlLicenseType.Code, Active = true };
                    clientDbContext.DdlLicenseTypes.Add(newLicenseType);
                    clientDbContext.SaveChanges();
                    ddlLicenseType.LicenseTypeId = newLicenseType.LicenseTypeId;
                }

            }

            return Json(new[] { ddlLicenseType }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult GetDdlLicenseList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var addressTypeList = clientDbContext.DdlLicenseTypes.Where(x => x.Active == true).OrderBy(e => e.Description).ToList();

            return Json(addressTypeList, JsonRequestBehavior.AllowGet);

        }



        #region Business Level

        public ActionResult DdlBusinessLevelTypesListMaintenance()
        {
            return View();
        }

        public PartialViewResult DdlBusinessLevelPartial()
        {
            return PartialView();
        }

        public JsonResult DdlbusinessLevelList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var businessLevelTypeList = clientDbContext.DdlBusinessLevelTypes.OrderBy(e => e.Description).ToList();
                return Json(businessLevelTypeList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlBusinessLevelList_Update([DataSourceRequest] DataSourceRequest request
           , ExecViewHrk.EfClient.DdlBusinessLevelTypes businessType)
        {
            if (businessType != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var businessLevelInDb = clientDbContext.DdlBusinessLevelTypes
                    .Where(x => x.BusinessLevelTypeNbr == businessType.BusinessLevelTypeNbr)
                    .SingleOrDefault();

                if (businessLevelInDb != null)
                {
                    businessLevelInDb.Code = businessType.Code;
                    businessLevelInDb.Description = businessType.Description;
                    businessLevelInDb.Active = businessType.Active;
                    clientDbContext.SaveChanges();
                }
            }

            return Json(new[] { businessType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlBusinessLevel_Destroy([DataSourceRequest] DataSourceRequest request, DdlBusinessLevelTypes businessLevel)
        {
            if (businessLevel != null)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                DdlBusinessLevelTypes businessInDb = clientDbContext.DdlBusinessLevelTypes.Where(x => x.BusinessLevelTypeNbr == businessLevel.BusinessLevelTypeNbr).SingleOrDefault();

                if (businessInDb != null)
                {
                    clientDbContext.DdlBusinessLevelTypes.Remove(businessInDb);
                    try
                    {
                        clientDbContext.SaveChanges();
                    }
                    catch
                    {
                        ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                    }
                }
            }

            return Json(new[] { businessLevel }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult DdlBusinessLevelList_Create([DataSourceRequest] DataSourceRequest request
          , ExecViewHrk.EfClient.DdlBusinessLevelTypes businessLevelType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (businessLevelType != null && ModelState.IsValid)
                {
                    var businessLevelInDb = clientDbContext.DdlBusinessLevelTypes
                        .Where(x => x.Code == businessLevelType.Code || x.Description == businessLevelType.Description)
                        .SingleOrDefault();

                    if (businessLevelInDb != null)
                    {
                        ModelState.AddModelError("", "Business Level Type" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                    }
                    else
                    {
                        var newbusinessType = new DdlBusinessLevelTypes
                        {
                            Description = businessLevelType.Description,
                            Code = businessLevelType.Code,
                            Active = businessLevelType.Active
                        };
                        try
                        {
                            clientDbContext.DdlBusinessLevelTypes.Add(newbusinessType);
                            clientDbContext.SaveChanges();
                            businessLevelType.BusinessLevelTypeNbr = newbusinessType.BusinessLevelTypeNbr;
                        }
                        catch { }
                    }
                }
            }

            return Json(new[] { businessLevelType }.ToDataSourceResult(request, ModelState));
        }

        #endregion Business Level

        #region EEO JobTraining

        public ActionResult ddlEEOJobTrainingStatusesListMaintenance()
        {
            return View();
        }

        public ActionResult ddlEEOJobTrainingList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var eeoJobTrainingStatusList = clientDbContext.DdlEEOJobTrainingStatuses.OrderBy(e => e.Description).ToList();

            return Json(eeoJobTrainingStatusList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ddlEEOJobTrainingList_Create([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfClient.ddlEEOJobTrainingStatuses eeoJobTraining)
        {
            if (eeoJobTraining != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var StatusInDb = clientDbContext.DdlEEOJobTrainingStatuses.Where(x => x.Code == eeoJobTraining.Code || x.Description == eeoJobTraining.Description).SingleOrDefault();

                if (StatusInDb != null)
                {
                    ModelState.AddModelError("", "EEO Job Training" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                }
                else
                {
                    var newEEOJobTraining = new ddlEEOJobTrainingStatuses { Description = eeoJobTraining.Description, Code = eeoJobTraining.Code, Active = eeoJobTraining.Active };
                    clientDbContext.DdlEEOJobTrainingStatuses.Add(newEEOJobTraining);
                    clientDbContext.SaveChanges();
                    eeoJobTraining.eeoJobTrainingStatusID = newEEOJobTraining.eeoJobTrainingStatusID;
                }
            }

            return Json(new[] { eeoJobTraining }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ddlEEOJobTrainingList_Update([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfClient.ddlEEOJobTrainingStatuses eeoJobTraining)
        {
            if (eeoJobTraining != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var eeoJobTrainingInDb = clientDbContext.DdlEEOJobTrainingStatuses.Where(x => x.eeoJobTrainingStatusID != eeoJobTraining.eeoJobTrainingStatusID && (x.Code == eeoJobTraining.Code || x.Description == eeoJobTraining.Description)).SingleOrDefault();
                if (eeoJobTrainingInDb == null)
                {
                    eeoJobTrainingInDb = clientDbContext.DdlEEOJobTrainingStatuses.Where(x => x.eeoJobTrainingStatusID == eeoJobTraining.eeoJobTrainingStatusID).SingleOrDefault();

                    if (eeoJobTrainingInDb.Active != eeoJobTraining.Active && eeoJobTraining.Active == false)
                    {
                        if (clientDbContext.Jobs.Where(x => x.eeoJobTrainingStatusID == eeoJobTraining.eeoJobTrainingStatusID).Count() == 0)
                        {
                            eeoJobTrainingInDb.Code = eeoJobTraining.Code;
                            eeoJobTrainingInDb.Description = eeoJobTraining.Description;
                            eeoJobTrainingInDb.Active = eeoJobTraining.Active;
                            clientDbContext.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                        }
                    }
                    else
                    {
                        eeoJobTrainingInDb.Code = eeoJobTraining.Code;
                        eeoJobTrainingInDb.Description = eeoJobTraining.Description;
                        eeoJobTrainingInDb.Active = eeoJobTraining.Active;
                        clientDbContext.SaveChanges();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "EEO Job Training" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                }

            }

            return Json(new[] { eeoJobTraining }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ddlEEOJobTrainingList_Destroy([DataSourceRequest] DataSourceRequest request, ddlEEOJobTrainingStatuses eeoJobTraining)
        {
            if (eeoJobTraining != null)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                ddlEEOJobTrainingStatuses eeoJobTrainingInDb = clientDbContext.DdlEEOJobTrainingStatuses.Where(x => x.eeoJobTrainingStatusID == eeoJobTraining.eeoJobTrainingStatusID).SingleOrDefault();

                if (eeoJobTrainingInDb != null)
                {
                    if (clientDbContext.Jobs.Where(x => x.eeoJobTrainingStatusID == eeoJobTraining.eeoJobTrainingStatusID).Count() > 0)
                    {
                        ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                    }
                    else
                    {
                        clientDbContext.DdlEEOJobTrainingStatuses.Remove(eeoJobTrainingInDb);
                        try
                        {
                            clientDbContext.SaveChanges();
                        }
                        catch
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                        }
                    }

                }
            }

            return Json(new[] { eeoJobTraining }.ToDataSourceResult(request, ModelState));
        }

        #endregion EEO JobTraining

        #region EEO File Status

        public ActionResult DdlEEOFileStatusListMaintenance()
        {
            return View();
        }

        public PartialViewResult DdlEEOFileStatusPartial()
        {
            return PartialView();
        }

        public JsonResult DdlEEOFileStatusList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var eEOFileStatusesList = clientDbContext.DdlEEOFileStatuses.OrderBy(e => e.Description).ToList();
                return Json(eEOFileStatusesList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlEEOFileStatusList_Update([DataSourceRequest] DataSourceRequest request
           , ExecViewHrk.EfClient.DdlEEOFileStatuses eEoFilestatus)
        {
            if (eEoFilestatus != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var eEOFileStatusInDb = clientDbContext.DdlEEOFileStatuses
                    .Where(x => x.EEoFileStatusNbr != eEoFilestatus.EEoFileStatusNbr && (x.Code == eEoFilestatus.Code || x.Description == eEoFilestatus.Description))
                    .SingleOrDefault();

                if (eEOFileStatusInDb == null)
                {
                    eEOFileStatusInDb = clientDbContext.DdlEEOFileStatuses.Where(x => x.EEoFileStatusNbr == eEoFilestatus.EEoFileStatusNbr).SingleOrDefault();
                    if (eEOFileStatusInDb.Active != eEoFilestatus.Active && eEoFilestatus.Active == false)
                    {
                        if (clientDbContext.PositionBusinessLevels.Where(x => x.BusinessLevelNbr == eEoFilestatus.EEoFileStatusNbr).Count() == 0)
                        {
                            eEOFileStatusInDb.Code = eEoFilestatus.Code;
                            eEOFileStatusInDb.Description = eEoFilestatus.Description;
                            eEOFileStatusInDb.Active = eEoFilestatus.Active;
                            clientDbContext.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                        }
                    }
                    else
                    {
                        eEOFileStatusInDb.Code = eEoFilestatus.Code;
                        eEOFileStatusInDb.Description = eEoFilestatus.Description;
                        eEOFileStatusInDb.Active = eEoFilestatus.Active;
                        clientDbContext.SaveChanges();
                    }
                }
                else
                {
                    ModelState.AddModelError("", string.Format(CustomErrorMessages.ERROR_LOOKUP_DUPLICATE_RECORD, "EEO file Status"));
                }
            }

            return Json(new[] { eEoFilestatus }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlEEOFileStatus_Destroy([DataSourceRequest] DataSourceRequest request, DdlEEOFileStatuses eEoFilestatus)
        {
            if (eEoFilestatus != null)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                DdlEEOFileStatuses eEoFileStatusInDb = clientDbContext.DdlEEOFileStatuses.Where(x => x.EEoFileStatusNbr == eEoFilestatus.EEoFileStatusNbr).SingleOrDefault();

                if (eEoFileStatusInDb != null)
                {
                    clientDbContext.DdlEEOFileStatuses.Remove(eEoFileStatusInDb);
                    try
                    {
                        clientDbContext.SaveChanges();
                    }
                    catch
                    {
                        ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                    }
                }
            }

            return Json(new[] { eEoFilestatus }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult DdlEEOFileStatus_Create([DataSourceRequest] DataSourceRequest request
          , ExecViewHrk.EfClient.DdlEEOFileStatuses EEOFileStatuse)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (EEOFileStatuse != null && ModelState.IsValid)
                {
                    var eEOFileStatusesInDb = clientDbContext.DdlEEOFileStatuses
                        .Where(x => x.Code == EEOFileStatuse.Code || x.Description == EEOFileStatuse.Description)
                        .SingleOrDefault();

                    if (eEOFileStatusesInDb != null)
                    {
                        ModelState.AddModelError("", "EEO file Status" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                    }
                    else
                    {
                        var neweEcoFileStatus = new DdlEEOFileStatuses
                        {
                            Description = EEOFileStatuse.Description,
                            Code = EEOFileStatuse.Code,
                            Active = EEOFileStatuse.Active
                        };
                        try
                        {
                            clientDbContext.DdlEEOFileStatuses.Add(neweEcoFileStatus);
                            clientDbContext.SaveChanges();
                            EEOFileStatuse.EEoFileStatusNbr = neweEcoFileStatus.EEoFileStatusNbr;
                        }
                        catch (Exception ex) { }
                    }
                }
            }

            return Json(new[] { EEOFileStatuse }.ToDataSourceResult(request, ModelState));
        }

        #endregion EEO File Status

        #region Fund Code

        public ActionResult DdlFundCodeListMaintenance()
        {
            return View();
        }

        public PartialViewResult DdlFundCodePartial()
        {
            return PartialView();
        }

        public JsonResult DdlFundCodeList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var fundList = clientDbContext.Funds.OrderBy(e => e.Description).ToList();
                return Json(fundList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlFundCodeList_Update([DataSourceRequest] DataSourceRequest request
           , ExecViewHrk.EfClient.Funds funds)
        {
            if (funds != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var fundCodeInDbDupli = clientDbContext.Funds.Where(x => x.ID != funds.ID && (x.Code == funds.Code || x.Description == funds.Description)).SingleOrDefault();

                if (fundCodeInDbDupli != null)
                {
                    ModelState.AddModelError("", "Fund Code " + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                    return Json(new[] { funds }.ToDataSourceResult(request, ModelState));
                }

                var fundCodeInDb = clientDbContext.Funds
                .Where(x => x.ID == funds.ID)
                .SingleOrDefault();

                try
                {
                    if (fundCodeInDb.Active == true && funds.Active == false)
                    {
                        var UserInRole = from pos in clientDbContext.PositionsFundHistory
                                         join fh in clientDbContext.FundHistory on pos.FundHistoryID equals fh.ID
                                         join f in clientDbContext.Funds on fh.FundID equals f.ID
                                         where f.ID == fundCodeInDb.ID
                                         select pos.ID;
                        if (UserInRole.Count() > 0)
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE_Active);
                            return Json(new[] { funds }.ToDataSourceResult(request, ModelState));
                        }
                    }
                }
                catch (Exception ex) { }

                if (fundCodeInDb != null)
                {
                    fundCodeInDb.Code = funds.Code;
                    fundCodeInDb.Description = funds.Description;
                    fundCodeInDb.Active = funds.Active;
                    clientDbContext.SaveChanges();
                }
            }

            return Json(new[] { funds }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlFundCode_Destroy([DataSourceRequest] DataSourceRequest request, Funds fundCode)
        {
            if (fundCode != null)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                Funds fundCodeInDb = clientDbContext.Funds.Where(x => x.ID == fundCode.ID).SingleOrDefault();

                if (fundCodeInDb != null)
                {
                    clientDbContext.Funds.Remove(fundCodeInDb);
                    try
                    {
                        clientDbContext.SaveChanges();
                    }
                    catch
                    {
                        ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                    }
                }
            }

            return Json(new[] { fundCode }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult DdlFundCode_Create([DataSourceRequest] DataSourceRequest request
          , ExecViewHrk.EfClient.Funds FundCode)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (FundCode != null && ModelState.IsValid)
                {
                    var fundCodeInDb = clientDbContext.Funds.Where(x => x.Code == FundCode.Code || x.Description == FundCode.Description).SingleOrDefault();

                    if (fundCodeInDb != null)
                    {
                        ModelState.AddModelError("", "Fund Code " + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                    }
                    else
                    {
                        var neweFundcode = new Funds
                        {
                            Description = FundCode.Description,
                            Code = FundCode.Code,
                            Active = FundCode.Active
                        };
                        try
                        {
                            clientDbContext.Funds.Add(neweFundcode);
                            clientDbContext.SaveChanges();
                            FundCode.ID = neweFundcode.ID;
                        }
                        catch { }
                    }
                }
            }

            return Json(new[] { FundCode }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult AddEditFundCode(int fundCodeId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            Funds _fund = new Funds();
            if (fundCodeId == 0)
            {
                _fund.ID = 0;

            }
            else
            {
                _fund = clientDbContext.Funds.Where(x => x.ID == fundCodeId).FirstOrDefault();
            }
            return View(_fund);
        }
        [HttpPost]
        public ActionResult UpdateFundCode(Funds _fund)
        {
            string _message = "";
            int FedralEINNbr = _fund.ID;
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            if (ModelState.IsValid)
            {
                if (_fund.ID == 0)
                {
                    var fundCodeInDb = clientDbContext.Funds.Where(x => x.Code == _fund.Code || x.Description == _fund.Description).FirstOrDefault();

                    if (fundCodeInDb != null)
                    {
                        _message = "Fund Code " + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED;
                        return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var neweFundcode = new Funds
                        {
                            Description = _fund.Description,
                            Code = _fund.Code,
                            Active = _fund.Active,
                            Amount = _fund.Amount,
                            FTE = _fund.FTE,
                            EffectiveStartDate = _fund.EffectiveStartDate,
                            EffectiveEndDate = _fund.EffectiveEndDate
                        };
                        try
                        {
                            clientDbContext.Funds.Add(neweFundcode);
                            clientDbContext.SaveChanges();
                            _fund.ID = neweFundcode.ID;
                        }
                        catch { }
                    }
                }
                else
                {
                    var fundCodeInDbDupli = clientDbContext.Funds.Where(x => x.ID != _fund.ID && (x.Code == _fund.Code || x.Description == _fund.Description)).SingleOrDefault();

                    if (fundCodeInDbDupli != null)
                    {
                        _message = "Fund Code " + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED;
                        return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
                    }

                    var fundCodeInDb = clientDbContext.Funds.Where(x => x.ID == _fund.ID).FirstOrDefault();

                    try
                    {
                        if (fundCodeInDb.Active == true && _fund.Active == false)
                        {
                            var UserInRole = from pos in clientDbContext.PositionsFundHistory
                                             join fh in clientDbContext.FundHistory on pos.FundHistoryID equals fh.ID
                                             join f in clientDbContext.Funds on fh.FundID equals f.ID
                                             where f.ID == fundCodeInDb.ID
                                             select pos.ID;
                            if (UserInRole.Count() > 0)
                            {
                                return Json(new { Message = CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE_Active, succeed = false }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    catch { }

                    if (fundCodeInDb != null)
                    {
                        fundCodeInDb.Code = _fund.Code;
                        fundCodeInDb.Description = _fund.Description;
                        fundCodeInDb.Active = _fund.Active;
                        fundCodeInDb.Amount = _fund.Amount;
                        fundCodeInDb.FTE = _fund.FTE;
                        fundCodeInDb.EffectiveStartDate = _fund.EffectiveStartDate;
                        fundCodeInDb.EffectiveEndDate = _fund.EffectiveEndDate;
                        clientDbContext.SaveChanges();
                    }
                }
            }
            //if (_fund.FedralEINNbr > 0)
            //{
            //    fedralEIN = clientDbContext.DdlEINs.Where(x => x.FedralEINNbr == _fund.FedralEINNbr).SingleOrDefault();
            //}
            //else
            //{
            //    fedralEIN = clientDbContext.DdlEINs.Where(x => x.EIN == _fund.EIN || x.description == _fund.Description).SingleOrDefault();
            //}

            //if (ModelState.IsValid)
            //{
            //    if (fedralEINVM.FedralEINNbr == 0)
            //    {
            //        if (fedralEIN != null)
            //        {
            //            ModelState.AddModelError("", "EIN with same EIN or description already exists.");
            //            string _message = Utils.GetErrorString(null, null, this.ModelState);
            //            return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
            //        }
            //        else
            //        {
            //            recordIsNew = true;
            //            fedralEIN = new DdlEINs();
            //            fedralEIN.EIN = String.Concat(fedralEINVM.EIN.Where(Char.IsDigit));
            //            fedralEIN.description = fedralEINVM.Description;
            //            fedralEIN.addressLineOne = fedralEINVM.addressLineOne;
            //            fedralEIN.addressLineTwo = fedralEINVM.addressLineTwo;
            //            fedralEIN.city = fedralEINVM.city;
            //            fedralEIN.stateID = fedralEINVM.stateID.HasValue ? fedralEINVM.stateID.Value : (byte?)null;
            //            fedralEIN.zipCode = fedralEINVM.zipCode == null ? null : String.Concat(fedralEINVM.zipCode.Where(Char.IsDigit));
            //            fedralEIN.countryID = fedralEINVM.countryID.HasValue ? fedralEINVM.countryID.Value : (short?)null;
            //            fedralEIN.phoneNumber = fedralEINVM.phoneNumber == null ? null : String.Concat(fedralEINVM.phoneNumber.Where(Char.IsDigit));
            //            fedralEIN.faxNumber = fedralEINVM.faxNumber == null ? null : String.Concat(fedralEINVM.faxNumber.Where(Char.IsDigit));
            //            fedralEIN.EEOFileStatusID = fedralEINVM.EEOFileStatusID.HasValue ? fedralEINVM.EEOFileStatusID.Value : (byte?)null;
            //            fedralEIN.active = fedralEINVM.Active;
            //            fedralEIN.notes = fedralEINVM.notes;
            //            clientDbContext.DdlEINs.Add(fedralEIN);

            //            try
            //            {
            //                clientDbContext.SaveChanges();
            //                ViewBag.AlertMessage = recordIsNew == true ? "New Fedral EIN Added" : "Fedral EIN Saved";
            //            }
            //            catch (Exception err)
            //            {
            //                string _message = Utils.GetErrorString(err, clientDbContext, null);
            //                return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        var dddlEINsInDb = clientDbContext.DdlEINs.Where(x => x.FedralEINNbr != fedralEINVM.FedralEINNbr && (x.EIN == fedralEINVM.EIN || x.description == fedralEINVM.Description)).SingleOrDefault();
            //        if (dddlEINsInDb == null)
            //        {
            //            dddlEINsInDb = clientDbContext.DdlEINs.Where(x => x.FedralEINNbr == fedralEINVM.FedralEINNbr).SingleOrDefault();

            //            if (dddlEINsInDb.active != fedralEINVM.Active && fedralEINVM.Active == false)
            //            {
            //                if (clientDbContext.PositionBusinessLevels.Where(x => x.FedralEINNbr == fedralEINVM.FedralEINNbr).Count() == 0)
            //                {
            //                    fedralEIN.EIN = String.Concat(fedralEINVM.EIN.Where(Char.IsDigit));
            //                    fedralEIN.description = fedralEINVM.Description;
            //                    fedralEIN.addressLineOne = fedralEINVM.addressLineOne;
            //                    fedralEIN.addressLineTwo = fedralEINVM.addressLineTwo;
            //                    fedralEIN.city = fedralEINVM.city;
            //                    fedralEIN.stateID = fedralEINVM.stateID.HasValue ? fedralEINVM.stateID.Value : (byte?)null;
            //                    fedralEIN.zipCode = fedralEINVM.zipCode == null ? null : String.Concat(fedralEINVM.zipCode.Where(Char.IsDigit));
            //                    fedralEIN.countryID = fedralEINVM.countryID.HasValue ? fedralEINVM.countryID.Value : (short?)null;
            //                    fedralEIN.phoneNumber = fedralEINVM.phoneNumber == null ? null : String.Concat(fedralEINVM.phoneNumber.Where(Char.IsDigit));
            //                    fedralEIN.faxNumber = fedralEINVM.faxNumber == null ? null : String.Concat(fedralEINVM.faxNumber.Where(Char.IsDigit));
            //                    fedralEIN.EEOFileStatusID = fedralEINVM.EEOFileStatusID.HasValue ? fedralEINVM.EEOFileStatusID.Value : (byte?)null;
            //                    fedralEIN.active = fedralEINVM.Active;
            //                    fedralEIN.notes = fedralEINVM.notes;

            //                }
            //                else
            //                {
            //                    ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
            //                    string _message = Utils.GetErrorString(null, null, this.ModelState);
            //                    return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
            //                }
            //            }
            //            else
            //            {
            //                fedralEIN.EIN = String.Concat(fedralEINVM.EIN.Where(Char.IsDigit));
            //                fedralEIN.description = fedralEINVM.Description;
            //                fedralEIN.addressLineOne = fedralEINVM.addressLineOne;
            //                fedralEIN.addressLineTwo = fedralEINVM.addressLineTwo;
            //                fedralEIN.city = fedralEINVM.city;
            //                fedralEIN.stateID = fedralEINVM.stateID.HasValue ? fedralEINVM.stateID.Value : (byte?)null;
            //                fedralEIN.zipCode = fedralEINVM.zipCode == null ? null : String.Concat(fedralEINVM.zipCode.Where(Char.IsDigit));
            //                fedralEIN.countryID = fedralEINVM.countryID.HasValue ? fedralEINVM.countryID.Value : (short?)null;
            //                fedralEIN.phoneNumber = fedralEINVM.phoneNumber == null ? null : String.Concat(fedralEINVM.phoneNumber.Where(Char.IsDigit));
            //                fedralEIN.faxNumber = fedralEINVM.faxNumber == null ? null : String.Concat(fedralEINVM.faxNumber.Where(Char.IsDigit));
            //                fedralEIN.EEOFileStatusID = fedralEINVM.EEOFileStatusID.HasValue ? fedralEINVM.EEOFileStatusID.Value : (byte?)null;
            //                fedralEIN.active = fedralEINVM.Active;
            //                fedralEIN.notes = fedralEINVM.notes;

            //            }
            //        }
            //        else
            //        {
            //            ModelState.AddModelError("", "EIN with same EIN or description already exists.");
            //            string _message = Utils.GetErrorString(null, null, this.ModelState);
            //            return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
            //        }

            //        try
            //        {
            //            clientDbContext.SaveChanges();
            //            ViewBag.AlertMessage = recordIsNew == true ? "New Fedral EIN Added" : "Fedral EIN Saved";
            //        }
            //        catch (Exception err)
            //        {
            //            string _message = Utils.GetErrorString(err, clientDbContext, null);
            //            return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
            //        }
            //    }

            //}
            //else
            //{
            //    string _message = Utils.GetErrorString(null, null, this.ModelState);
            //    return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
            //}


            return Json(new { Message = "", succeed = true }, JsonRequestBehavior.AllowGet);
        }
        #endregion Fund Code

        #region Unions

        public ActionResult DdlUnionsListMaintenance()
        {
            return View();
        }

        public PartialViewResult DdlUnionsPartial()
        {
            return PartialView();
        }

        public JsonResult DdlUnionsList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var unionsList = clientDbContext.DdlUnions.OrderBy(e => e.Description).ToList();
                return Json(unionsList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlUnionsList_Update([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfClient.ddlUnions unions)
        {
            if (unions != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);

                var unionsInDb = clientDbContext.DdlUnions.Where(x => x.unionID != unions.unionID && (x.Code == unions.Code || x.Description == unions.Description)).SingleOrDefault();
                if (unionsInDb == null)
                {
                    unionsInDb = clientDbContext.DdlUnions.Where(x => x.unionID == unions.unionID).SingleOrDefault();

                    if (unionsInDb.Active != unions.Active && unions.Active == false)
                    {
                        if (clientDbContext.Jobs.Where(x => x.unionID == unions.unionID).Count() == 0)
                        {
                            unionsInDb.Code = unions.Code;
                            unionsInDb.Description = unions.Description;
                            unionsInDb.Active = unions.Active;
                            clientDbContext.SaveChanges();
                            unions.unionID = unionsInDb.unionID;
                        }
                        else
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                        }
                    }
                    else
                    {
                        unionsInDb.Code = unions.Code;
                        unionsInDb.Description = unions.Description;
                        unionsInDb.Active = unions.Active;
                        clientDbContext.SaveChanges();
                        unions.unionID = unionsInDb.unionID;
                    }
                }
                else
                {
                    ModelState.AddModelError("", string.Format(CustomErrorMessages.ERROR_LOOKUP_DUPLICATE_RECORD, "Union"));
                }

            }

            return Json(new[] { unions }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlUnions_Destroy([DataSourceRequest] DataSourceRequest request, ddlUnions unions)
        {
            if (unions != null)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                ddlUnions unionsInDb = clientDbContext.DdlUnions.Where(x => x.unionID == unions.unionID).SingleOrDefault();

                if (unionsInDb != null)
                {
                    if (clientDbContext.Jobs.Where(x => x.unionID == unions.unionID).Count() > 0)
                    {
                        ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                    }
                    else
                    {
                        clientDbContext.DdlUnions.Remove(unionsInDb);
                        try
                        {
                            clientDbContext.SaveChanges();
                        }
                        catch
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                        }
                    }
                }
            }

            return Json(new[] { unions }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult DdlUnions_Create([DataSourceRequest] DataSourceRequest request, ddlUnions unions)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (unions != null && ModelState.IsValid)
                {
                    var unionsInDb = clientDbContext.DdlUnions.Where(x => x.Code == unions.Code || x.Description == unions.Description).SingleOrDefault();

                    if (unionsInDb != null)
                    {
                        ModelState.AddModelError("", "Union " + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                    }
                    else
                    {
                        var neweunion = new ddlUnions
                        {
                            Description = unions.Description,
                            Code = unions.Code,
                            Active = unions.Active
                        };
                        try
                        {
                            clientDbContext.DdlUnions.Add(neweunion);
                            clientDbContext.SaveChanges();
                            unions.unionID = unionsInDb.unionID;
                        }
                        catch { }
                    }
                }
            }

            return Json(new[] { unions }.ToDataSourceResult(request, ModelState));
        }

        #endregion Unions

        #region EEO JobCodes

        public ActionResult ddlEEOJobCodesListMaintenance()
        {
            return View();
        }

        public ActionResult ddlEEOJobCodesList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var eeoJobCodesList = clientDbContext.DdlEEOJobCodes.OrderBy(e => e.Description).ToList();

            return Json(eeoJobCodesList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ddlEEOJobCodesList_Create([DataSourceRequest] DataSourceRequest request, ddlEEOJobCodes eeoJobCodes)
        {
            if (eeoJobCodes != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var StatusInDb = clientDbContext.DdlEEOJobCodes.Where(x => x.Code == eeoJobCodes.Code || x.Description == eeoJobCodes.Description).SingleOrDefault();

                if (StatusInDb != null)
                {
                    ModelState.AddModelError("", "EEO Job Code" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                }
                else
                {
                    var newEEOJobCodes = new ddlEEOJobCodes { Description = eeoJobCodes.Description, Code = eeoJobCodes.Code, Active = eeoJobCodes.Active };
                    clientDbContext.DdlEEOJobCodes.Add(newEEOJobCodes);
                    clientDbContext.SaveChanges();
                    eeoJobCodes.eeoJobCodeID = newEEOJobCodes.eeoJobCodeID;
                }
            }

            return Json(new[] { eeoJobCodes }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ddlEEOJobCodesList_Update([DataSourceRequest] DataSourceRequest request, ddlEEOJobCodes eeoJobCodes)
        {
            if (eeoJobCodes != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);

                var eeoJobCodesInDb = clientDbContext.DdlEEOJobCodes.Where(x => x.eeoJobCodeID != eeoJobCodes.eeoJobCodeID && (x.Code == eeoJobCodes.Code || x.Description == eeoJobCodes.Description)).SingleOrDefault();
                if (eeoJobCodesInDb == null)
                {
                    eeoJobCodesInDb = clientDbContext.DdlEEOJobCodes.Where(x => x.eeoJobCodeID == eeoJobCodes.eeoJobCodeID).SingleOrDefault();

                    if (eeoJobCodesInDb.Active != eeoJobCodes.Active && eeoJobCodes.Active == false)
                    {
                        if (clientDbContext.Jobs.Where(x => x.eeoJobCodeID == eeoJobCodes.eeoJobCodeID).Count() == 0)
                        {
                            eeoJobCodesInDb.Code = eeoJobCodes.Code;
                            eeoJobCodesInDb.Description = eeoJobCodes.Description;
                            eeoJobCodesInDb.Active = eeoJobCodes.Active;
                            clientDbContext.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                        }
                    }
                    else
                    {
                        eeoJobCodesInDb.Code = eeoJobCodes.Code;
                        eeoJobCodesInDb.Description = eeoJobCodes.Description;
                        eeoJobCodesInDb.Active = eeoJobCodes.Active;
                        clientDbContext.SaveChanges();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "EEO Job Code" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                }

            }

            return Json(new[] { eeoJobCodes }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ddlEEOJobCodesList_Destroy([DataSourceRequest] DataSourceRequest request, ddlEEOJobCodes eeoJobCodes)
        {
            if (eeoJobCodes != null)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                ddlEEOJobCodes eeoJobCodesInDb = clientDbContext.DdlEEOJobCodes.Where(x => x.eeoJobCodeID == eeoJobCodes.eeoJobCodeID).SingleOrDefault();

                if (eeoJobCodesInDb != null)
                {
                    if (clientDbContext.Jobs.Where(x => x.eeoJobCodeID == eeoJobCodes.eeoJobCodeID).Count() > 0)
                    {
                        ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                    }
                    else
                    {
                        clientDbContext.DdlEEOJobCodes.Remove(eeoJobCodesInDb);
                        try
                        {
                            clientDbContext.SaveChanges();
                        }
                        catch
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                        }
                    }


                }
            }

            return Json(new[] { eeoJobCodes }.ToDataSourceResult(request, ModelState));
        }

        #endregion EEO JobCodes

        #region FLSA

        public ActionResult DdlFLSAListMaintenance()
        {
            return View();
        }

        public PartialViewResult DdlFLSAPartial()
        {
            return PartialView();
        }

        public JsonResult DdlFLSAList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var flsaList = clientDbContext.DddlFLSAs.OrderBy(e => e.Description).ToList();
                return Json(flsaList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlFLSAList_Update([DataSourceRequest] DataSourceRequest request, ddlFLSAs FLSAs)
        {
            if (FLSAs != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);

                var flsaInDb = clientDbContext.DddlFLSAs.Where(x => x.FLSAID != FLSAs.FLSAID && (x.Code == FLSAs.Code || x.Description == FLSAs.Description)).SingleOrDefault();
                if (flsaInDb == null)
                {
                    flsaInDb = clientDbContext.DddlFLSAs.Where(x => x.FLSAID == FLSAs.FLSAID).SingleOrDefault();

                    if (flsaInDb.Active != FLSAs.Active && FLSAs.Active == false)
                    {
                        if (clientDbContext.Jobs.Where(x => x.FLSAID == FLSAs.FLSAID).Count() == 0)
                        {
                            flsaInDb.Code = FLSAs.Code;
                            flsaInDb.Description = FLSAs.Description;
                            flsaInDb.Active = FLSAs.Active;
                            clientDbContext.SaveChanges();
                            FLSAs.FLSAID = flsaInDb.FLSAID;
                        }
                        else
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                        }
                    }
                    else
                    {
                        flsaInDb.Code = FLSAs.Code;
                        flsaInDb.Description = FLSAs.Description;
                        flsaInDb.Active = FLSAs.Active;
                        clientDbContext.SaveChanges();
                        FLSAs.FLSAID = flsaInDb.FLSAID;
                    }
                }
                else
                {
                    ModelState.AddModelError("", string.Format(CustomErrorMessages.ERROR_LOOKUP_DUPLICATE_RECORD, "FLSA"));
                }

            }

            return Json(new[] { FLSAs }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlFLSA_Destroy([DataSourceRequest] DataSourceRequest request, ddlFLSAs FLSAs)
        {
            if (FLSAs != null)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                ddlFLSAs fLSAInDb = clientDbContext.DddlFLSAs.Where(x => x.FLSAID == FLSAs.FLSAID).SingleOrDefault();

                if (fLSAInDb != null)
                {
                    if (clientDbContext.Jobs.Where(x => x.FLSAID == FLSAs.FLSAID).Count() > 0)
                    {
                        ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                    }
                    else
                    {
                        clientDbContext.DddlFLSAs.Remove(fLSAInDb);
                        try
                        {
                            clientDbContext.SaveChanges();
                        }
                        catch
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                        }
                    }
                }

            }

            return Json(new[] { FLSAs }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult DdlFLSA_Create([DataSourceRequest] DataSourceRequest request, ddlFLSAs FLSAs)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (FLSAs != null && ModelState.IsValid)
                {
                    var unionsInDb = clientDbContext.DddlFLSAs.Where(x => x.Code == FLSAs.Code || x.Description == FLSAs.Description).SingleOrDefault();

                    if (unionsInDb != null)
                    {
                        ModelState.AddModelError("", "FLSA " + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                    }
                    else
                    {
                        var neweFLSA = new ddlFLSAs
                        {
                            Description = FLSAs.Description,
                            Code = FLSAs.Code,
                            Active = FLSAs.Active
                        };
                        try
                        {
                            clientDbContext.DddlFLSAs.Add(neweFLSA);
                            clientDbContext.SaveChanges();
                            FLSAs.FLSAID = neweFLSA.FLSAID;
                        }
                        catch { }
                    }
                }
            }

            return Json(new[] { FLSAs }.ToDataSourceResult(request, ModelState));
        }

        #endregion FLSA

        #region Job Family

        public ActionResult DdlJobFamilysListMaintenance()
        {
            return View();
        }

        public PartialViewResult DdlJobFamilysPartial()
        {
            return PartialView();
        }

        public JsonResult DdlJobFamilysList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var jobFamilysList = clientDbContext.DdlJobFamilys.OrderBy(e => e.Description).ToList();
                return Json(jobFamilysList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlJobFamilysList_Update([DataSourceRequest] DataSourceRequest request, ddlJobFamilys JobFamily)
        {
            if (JobFamily != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);

                var JobFamilyInDb = clientDbContext.DdlJobFamilys.Where(x => x.JobFamilyId != JobFamily.JobFamilyId && (x.Code == JobFamily.Code || x.Description == JobFamily.Description)).SingleOrDefault();
                if (JobFamilyInDb == null)
                {
                    JobFamilyInDb = clientDbContext.DdlJobFamilys.Where(x => x.JobFamilyId == JobFamily.JobFamilyId).SingleOrDefault();

                    if (JobFamilyInDb.Active != JobFamily.Active && JobFamily.Active == false)
                    {
                        if (clientDbContext.Jobs.Where(x => x.JobFamilyId == JobFamily.JobFamilyId).Count() == 0)
                        {
                            JobFamilyInDb.Code = JobFamily.Code;
                            JobFamilyInDb.Description = JobFamily.Description;
                            JobFamilyInDb.Active = JobFamily.Active;
                            clientDbContext.SaveChanges();
                            JobFamily.JobFamilyId = JobFamilyInDb.JobFamilyId;
                        }
                        else
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                        }
                    }
                    else
                    {
                        JobFamilyInDb.Code = JobFamily.Code;
                        JobFamilyInDb.Description = JobFamily.Description;
                        JobFamilyInDb.Active = JobFamily.Active;
                        clientDbContext.SaveChanges();
                        JobFamily.JobFamilyId = JobFamilyInDb.JobFamilyId;
                    }
                }
                else
                {
                    ModelState.AddModelError("", string.Format(CustomErrorMessages.ERROR_LOOKUP_DUPLICATE_RECORD, "Union"));
                }
            }

            return Json(new[] { JobFamily }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlJobFamilys_Destroy([DataSourceRequest] DataSourceRequest request, ddlJobFamilys JobFamily)
        {
            if (JobFamily != null)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                ddlJobFamilys JobFamilyInDb = clientDbContext.DdlJobFamilys.Where(x => x.JobFamilyId == JobFamily.JobFamilyId).SingleOrDefault();
                if (JobFamilyInDb != null)
                {
                    if (clientDbContext.Jobs.Where(x => x.JobFamilyId == JobFamily.JobFamilyId).Count() > 0)
                    {
                        ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                    }
                    else
                    {
                        clientDbContext.DdlJobFamilys.Remove(JobFamilyInDb);
                        try
                        {
                            clientDbContext.SaveChanges();
                        }
                        catch
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                        }
                    }
                }
            }

            return Json(new[] { JobFamily }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult DdlJobFamilys_Create([DataSourceRequest] DataSourceRequest request
          , ExecViewHrk.EfClient.ddlJobFamilys JobFamily)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (JobFamily != null && ModelState.IsValid)
                {
                    var jobFamilyInDb = clientDbContext.DdlJobFamilys.Where(x => x.Code == JobFamily.Code || x.Description == JobFamily.Description)
                        .SingleOrDefault();

                    if (jobFamilyInDb != null)
                    {
                        ModelState.AddModelError("", "Job Family " + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                    }
                    else
                    {
                        var neweJobFamily = new ddlJobFamilys
                        {
                            Description = JobFamily.Description,
                            Code = JobFamily.Code,
                            Active = JobFamily.Active
                        };
                        try
                        {
                            clientDbContext.DdlJobFamilys.Add(neweJobFamily);
                            clientDbContext.SaveChanges();
                            JobFamily.JobFamilyId = neweJobFamily.JobFamilyId;
                        }
                        catch (Exception ex) { }
                    }
                }
            }

            return Json(new[] { JobFamily }.ToDataSourceResult(request, ModelState));
        }

        #endregion Job Family

        #region Workers Compensation

        public ActionResult ddlWorkersCompensationsListMaintenance()
        {
            return View("");
        }
        public ActionResult ddlSemester()
        {
            return View("");
        }

        public ActionResult WorkersCompensationList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var workersCompensationList = clientDbContext.DdlWorkersCompensations.OrderBy(e => e.Description).ToList();

            return Json(workersCompensationList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkersCompensationList_Create([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfClient.ddlWorkersCompensations workersCompensation)
        {
            if (workersCompensation != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var StatusInDb = clientDbContext.DdlWorkersCompensations
                    .Where(x => x.Code == workersCompensation.Code)
                    .SingleOrDefault();

                if (StatusInDb != null)
                {
                    ModelState.AddModelError("", "Worker's Compensation" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                }
                else
                {
                    var newWorkersCompensation = new ddlWorkersCompensations { Description = workersCompensation.Description, Code = workersCompensation.Code, Active = workersCompensation.Active };
                    clientDbContext.DdlWorkersCompensations.Add(newWorkersCompensation);
                    clientDbContext.SaveChanges();
                    workersCompensation.workersCompensationID = newWorkersCompensation.workersCompensationID;
                }
            }

            return Json(new[] { workersCompensation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkersCompensationList_Update([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfClient.ddlWorkersCompensations workersCompensation)
        {
            if (workersCompensation != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);

                var workersCompensationInDb = clientDbContext.DdlWorkersCompensations.Where(x => x.workersCompensationID != workersCompensation.workersCompensationID && (x.Code == workersCompensation.Code || x.Description == workersCompensation.Description)).SingleOrDefault();
                if (workersCompensationInDb == null)
                {
                    workersCompensationInDb = clientDbContext.DdlWorkersCompensations.Where(x => x.workersCompensationID == workersCompensation.workersCompensationID).SingleOrDefault();

                    if (workersCompensationInDb.Active != workersCompensation.Active && workersCompensation.Active == false)
                    {
                        if (clientDbContext.Jobs.Where(x => x.workersCompensationID == workersCompensation.workersCompensationID).Count() == 0)
                        {
                            workersCompensationInDb.Code = workersCompensation.Code;
                            workersCompensationInDb.Description = workersCompensation.Description;
                            workersCompensationInDb.Active = workersCompensation.Active;
                            clientDbContext.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                        }
                    }
                    else
                    {
                        workersCompensationInDb.Code = workersCompensation.Code;
                        workersCompensationInDb.Description = workersCompensation.Description;
                        workersCompensationInDb.Active = workersCompensation.Active;
                        clientDbContext.SaveChanges();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Worker's Compensation" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                }

            }

            return Json(new[] { workersCompensation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkersCompensationList_Destroy([DataSourceRequest] DataSourceRequest request, ddlWorkersCompensations workersCompensation)
        {
            if (workersCompensation != null)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                ddlWorkersCompensations workersCompensationInDb = clientDbContext.DdlWorkersCompensations.Where(x => x.workersCompensationID == workersCompensation.workersCompensationID).SingleOrDefault();

                if (workersCompensationInDb != null)
                {
                    if (clientDbContext.Jobs.Where(x => x.workersCompensationID == workersCompensation.workersCompensationID).Count() > 0)
                    {
                        ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                    }
                    else
                    {
                        clientDbContext.DdlWorkersCompensations.Remove(workersCompensationInDb);
                        try
                        {
                            clientDbContext.SaveChanges();
                        }
                        catch
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                        }
                    }

                }
            }

            return Json(new[] { workersCompensation }.ToDataSourceResult(request, ModelState));
        }

        #endregion Workers Compensation

        #region Job Class

        public ActionResult ddlJobClassListMaintenance()
        {
            return View("");
        }

        public ActionResult JobClassList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var jobClassList = clientDbContext.DdlJobClasses.OrderBy(e => e.Description).ToList();

            return Json(jobClassList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobClassList_Create([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfClient.ddlJobClasses JobClass)
        {
            if (JobClass != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var StatusInDb = clientDbContext.DdlJobClasses.Where(x => x.jobClassID != JobClass.jobClassID
                    && (x.Code == JobClass.Code || x.Description.Trim() == JobClass.Description.Trim())).SingleOrDefault();
                if (StatusInDb != null)
                {
                    ModelState.AddModelError("", "Job Class" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                }
                else
                {
                    var newJobClass = new ddlJobClasses { Description = JobClass.Description, Code = JobClass.Code, Active = JobClass.Active };
                    clientDbContext.DdlJobClasses.Add(newJobClass);
                    clientDbContext.SaveChanges();
                    JobClass.jobClassID = newJobClass.jobClassID;
                }
            }

            return Json(new[] { JobClass }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobClassList_Update([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfClient.ddlJobClasses jobclass)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            if (jobclass != null && ModelState.IsValid)
            {
                var jobclassInDb = clientDbContext.DdlJobClasses.Where(x => x.jobClassID != jobclass.jobClassID && (x.Code == jobclass.Code || x.Description == jobclass.Description)).SingleOrDefault();
                if (jobclassInDb == null)
                {
                    jobclassInDb = clientDbContext.DdlJobClasses.Where(x => x.jobClassID == jobclass.jobClassID).SingleOrDefault();

                    if (jobclassInDb.Active != jobclass.Active && jobclass.Active == false)
                    {
                        if (clientDbContext.Jobs.Where(x => x.jobClassID == jobclass.jobClassID).Count() == 0)
                        {
                            jobclassInDb.Code = jobclass.Code;
                            jobclassInDb.Description = jobclass.Description;
                            jobclassInDb.Active = jobclass.Active;
                            clientDbContext.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                        }
                    }
                    else
                    {
                        jobclassInDb.Code = jobclass.Code;
                        jobclassInDb.Description = jobclass.Description;
                        jobclassInDb.Active = jobclass.Active;
                        clientDbContext.SaveChanges();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Job Class" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                }
            }

            return Json(new[] { jobclass }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobClassList_Destroy([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfClient.ddlJobClasses jobClass)
        {
            if (jobClass != null)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                ddlJobClasses jobClassInDb = clientDbContext.DdlJobClasses.Where(x => x.jobClassID == jobClass.jobClassID).SingleOrDefault();

                if (jobClassInDb != null)
                {
                    if (clientDbContext.Jobs.Where(x => x.jobClassID == jobClass.jobClassID).Count() > 0)
                    {
                        ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                    }
                    else
                    {
                        clientDbContext.DdlJobClasses.Remove(jobClassInDb);
                        try
                        {
                            clientDbContext.SaveChanges();
                        }
                        catch
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                        }
                    }


                }
            }

            return Json(new[] { jobClass }.ToDataSourceResult(request, ModelState));
        }

        #endregion Job Class

        #region Locations

        public ActionResult LocationsListMaintenance()
        {

            return View();
        }

        public ActionResult LocationsList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var lstLocations = clientDbContext.Locations.OrderBy(e => e.LocationDescription).ToList();

            List<LocationsVM> lstlocationsVM = new List<LocationsVM>();
            foreach (var item in lstLocations)
            {
                lstlocationsVM.Add(new LocationsVM
                {
                    Code = item.LocationCode,
                    Description = item.LocationDescription,
                    Active = item.Active,
                    locationID = item.LocationId
                });
            }

            return Json(lstlocationsVM.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LocationsList_Create([DataSourceRequest] DataSourceRequest request, LocationsVM locationVM)
        {

            if (locationVM != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var StatusInDb = clientDbContext.Locations.Where(x => x.LocationCode == locationVM.Code || x.LocationDescription == locationVM.Description).SingleOrDefault();

                if (StatusInDb != null)
                {
                    ModelState.AddModelError("", "location " + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                }
                else
                {
                    var newLocation = new Location { LocationCode = locationVM.Code, LocationDescription = locationVM.Description, Active = locationVM.Active };
                    clientDbContext.Locations.Add(newLocation);
                    clientDbContext.SaveChanges();
                    locationVM.locationID = locationVM.locationID;
                }

            }

            return Json(new[] { locationVM }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LocationsList_Update([DataSourceRequest] DataSourceRequest request, LocationsVM locationsVM)
        {
            if (locationsVM != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);

                var locationsVMInDb = clientDbContext.Locations.Where(x => x.LocationId != locationsVM.locationID && (x.LocationCode == locationsVM.Code || x.LocationDescription == locationsVM.Description)).FirstOrDefault();
                if (locationsVMInDb == null)
                {
                    locationsVMInDb = clientDbContext.Locations.Where(x => x.LocationId == locationsVM.locationID).SingleOrDefault();

                    if (locationsVMInDb.Active != locationsVM.Active && locationsVM.Active == false)
                    {
                        if (clientDbContext.PositionBusinessLevels.Where(x => x.LocationId == locationsVM.locationID).Count() == 0)
                        {
                            locationsVMInDb.LocationCode = locationsVM.Code;
                            locationsVMInDb.LocationDescription = locationsVM.Description;
                            locationsVMInDb.Active = locationsVM.Active;
                            clientDbContext.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                        }
                    }
                    else
                    {
                        locationsVMInDb.LocationCode = locationsVM.Code;
                        locationsVMInDb.LocationDescription = locationsVM.Description;
                        locationsVMInDb.Active = locationsVM.Active;
                        clientDbContext.SaveChanges();
                    }
                }
                else
                {
                    ModelState.AddModelError("", string.Format(CustomErrorMessages.ERROR_LOOKUP_DUPLICATE_RECORD, "Locations"));
                }

            }

            return Json(new[] { locationsVM }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LocationsList_Destroy([DataSourceRequest] DataSourceRequest request, LocationsVM locationsVM)
        {
            if (locationsVM != null)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                Location locationInDb = clientDbContext.Locations.Where(x => x.LocationId == locationsVM.locationID).SingleOrDefault();

                if (locationInDb != null)
                {
                    clientDbContext.Locations.Remove(locationInDb);
                    try
                    {
                        clientDbContext.SaveChanges();
                    }
                    catch { ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE); }
                }
            }

            return Json(new[] { locationsVM }.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region Fedral EIN

        public ActionResult FedralEINListMaintenance()
        {
            FedralEinVM lstfedralEIN = new FedralEinVM();
            return View(lstfedralEIN);
        }

        public ActionResult FedralEINList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var lstFedralEIN = clientDbContext.DdlEINs.OrderBy(e => e.description).ToList();

            List<FedralEinVM> lstFedralEinVM = new List<FedralEinVM>();
            foreach (var item in lstFedralEIN)
            {
                lstFedralEinVM.Add(new FedralEinVM
                {
                    EIN = item.EIN,
                    Description = item.description,
                    Active = item.active,
                    FedralEINNbr = item.FedralEINNbr
                });
            }

            return Json(lstFedralEinVM.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FedralEINList_Create([DataSourceRequest] DataSourceRequest request, FedralEinVM fedralEinVM)
        {

            //if (locationVM != null && ModelState.IsValid)
            //{
            //    string connString = User.Identity.GetClientConnectionString();
            //    ClientDbContext clientDbContext = new ClientDbContext(connString);
            //    var StatusInDb = clientDbContext.Locations.Where(x => x.LocationCode == locationVM.Code || x.LocationDescription == locationVM.Description).SingleOrDefault();

            //    if (StatusInDb != null)
            //    {
            //        ModelState.AddModelError("", "The location " + CustomErrorMessages.ERROR_ALREADY_DEFINED);
            //    }
            //    else
            //    {
            //        var newLocation = new Location { LocationCode = locationVM.Code, LocationDescription = locationVM.Description, Active = locationVM.Active };
            //        clientDbContext.Locations.Add(newLocation);
            //        clientDbContext.SaveChanges();
            //        locationVM.locationID = locationVM.locationID;
            //    }

            //}

            return Json(new[] { fedralEinVM }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult AddEditPopupFedralEIN(int FedralEINNbr)
        {
            bool isNewRecord = false;
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            FedralEinVM fedralEinVM = new FedralEinVM();
            if (FedralEINNbr == 0)
            {
                isNewRecord = true;
                fedralEinVM.FedralEINNbr = 0;
                fedralEinVM.isNewRecord = isNewRecord;
            }
            else
            {
                fedralEinVM = clientDbContext.DdlEINs
                         .Where(x => x.FedralEINNbr == FedralEINNbr)
                         .Select(e => new FedralEinVM
                         {
                             EIN = e.EIN,
                             Description = e.description,
                             addressLineOne = e.addressLineOne,
                             addressLineTwo = e.addressLineTwo,
                             city = e.city,
                             stateID = e.stateID,
                             zipCode = e.zipCode,
                             countryID = e.countryID,
                             phoneNumber = e.phoneNumber,
                             faxNumber = e.faxNumber,
                             EEOFileStatusID = e.EEOFileStatusID,
                             Active = e.active,
                             notes = e.notes
                         }).FirstOrDefault();
            }
            fedralEinVM.lstCountries = getAllCountries().CleanUp();
            fedralEinVM.lstStates = getAllStates().CleanUp();
            fedralEinVM.lstEEOFileStatuses = getAllEEOFileStatues().CleanUp();
            return View("FedralEINAddEditPartial", fedralEinVM);
        }

        public ActionResult AddPopupFedralEINforDDL(int FedralEINNbr)
        {
            bool isNewRecord = false;
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            FedralEinVM fedralEinVM = new FedralEinVM();
            if (FedralEINNbr == 0)
            {
                isNewRecord = true;
                fedralEinVM.FedralEINNbr = 0;
                fedralEinVM.isNewRecord = isNewRecord;
            }
            else
            {
                fedralEinVM = clientDbContext.DdlEINs
                         .Where(x => x.FedralEINNbr == FedralEINNbr)
                         .Select(e => new FedralEinVM
                         {
                             EIN = e.EIN,
                             Description = e.description,
                             addressLineOne = e.addressLineOne,
                             addressLineTwo = e.addressLineTwo,
                             city = e.city,
                             stateID = e.stateID,
                             zipCode = e.zipCode,
                             countryID = e.countryID,
                             phoneNumber = e.phoneNumber,
                             faxNumber = e.faxNumber,
                             EEOFileStatusID = e.EEOFileStatusID,
                             Active = e.active,
                             notes = e.notes
                         }).FirstOrDefault();
            }
            fedralEinVM.lstCountries = getAllCountries().CleanUp();
            fedralEinVM.lstStates = getAllStates().CleanUp();
            fedralEinVM.lstEEOFileStatuses = getAllEEOFileStatues().CleanUp();
            return View("AddDDLEIN", fedralEinVM);
        }
        [HttpPost]
        public ActionResult FedralEINSaveAjax(FedralEinVM fedralEINVM)
        {
            bool recordIsNew = false;
            int FedralEINNbr = fedralEINVM.FedralEINNbr;
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            DdlEINs fedralEIN;

            if (fedralEINVM.FedralEINNbr > 0)
            {
                fedralEIN = clientDbContext.DdlEINs.Where(x => x.FedralEINNbr == fedralEINVM.FedralEINNbr).SingleOrDefault();
            }
            else
            {
                fedralEIN = clientDbContext.DdlEINs.Where(x => x.EIN == fedralEINVM.EIN || x.description == fedralEINVM.Description).SingleOrDefault();
            }

            if (ModelState.IsValid)
            {
                if (fedralEINVM.FedralEINNbr == 0)
                {
                    if (fedralEIN != null)
                    {
                        ModelState.AddModelError("", "EIN with same EIN or description already exists.");
                        string _message = Utils.GetErrorString(null, null, this.ModelState);
                        return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        recordIsNew = true;
                        fedralEIN = new DdlEINs();
                        fedralEIN.EIN = String.Concat(fedralEINVM.EIN.Where(Char.IsDigit));
                        fedralEIN.description = fedralEINVM.Description;
                        fedralEIN.addressLineOne = fedralEINVM.addressLineOne;
                        fedralEIN.addressLineTwo = fedralEINVM.addressLineTwo;
                        fedralEIN.city = fedralEINVM.city;
                        fedralEIN.stateID = fedralEINVM.stateID.HasValue ? fedralEINVM.stateID.Value : (byte?)null;
                        fedralEIN.zipCode = fedralEINVM.zipCode == null ? null : String.Concat(fedralEINVM.zipCode.Where(Char.IsDigit));
                        fedralEIN.countryID = fedralEINVM.countryID.HasValue ? fedralEINVM.countryID.Value : (int?)null;
                        fedralEIN.phoneNumber = fedralEINVM.phoneNumber == null ? null : String.Concat(fedralEINVM.phoneNumber.Where(Char.IsDigit));
                        fedralEIN.faxNumber = fedralEINVM.faxNumber == null ? null : String.Concat(fedralEINVM.faxNumber.Where(Char.IsDigit));
                        fedralEIN.EEOFileStatusID = fedralEINVM.EEOFileStatusID.HasValue ? fedralEINVM.EEOFileStatusID.Value : (byte?)null;
                        fedralEIN.active = fedralEINVM.Active;
                        fedralEIN.notes = fedralEINVM.notes;
                        clientDbContext.DdlEINs.Add(fedralEIN);

                        try
                        {
                            clientDbContext.SaveChanges();
                            ViewBag.AlertMessage = recordIsNew == true ? "New Fedral EIN Added" : "Fedral EIN Saved";
                        }
                        catch (Exception err)
                        {
                            string _message = Utils.GetErrorString(err, clientDbContext, null);
                            return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    var dddlEINsInDb = clientDbContext.DdlEINs.Where(x => x.FedralEINNbr != fedralEINVM.FedralEINNbr && (x.EIN == fedralEINVM.EIN || x.description == fedralEINVM.Description)).SingleOrDefault();
                    if (dddlEINsInDb == null)
                    {
                        dddlEINsInDb = clientDbContext.DdlEINs.Where(x => x.FedralEINNbr == fedralEINVM.FedralEINNbr).SingleOrDefault();

                        if (dddlEINsInDb.active != fedralEINVM.Active && fedralEINVM.Active == false)
                        {
                            if (clientDbContext.PositionBusinessLevels.Where(x => x.FedralEINNbr == fedralEINVM.FedralEINNbr).Count() == 0)
                            {
                                fedralEIN.EIN = String.Concat(fedralEINVM.EIN.Where(Char.IsDigit));
                                fedralEIN.description = fedralEINVM.Description;
                                fedralEIN.addressLineOne = fedralEINVM.addressLineOne;
                                fedralEIN.addressLineTwo = fedralEINVM.addressLineTwo;
                                fedralEIN.city = fedralEINVM.city;
                                fedralEIN.stateID = fedralEINVM.stateID.HasValue ? fedralEINVM.stateID.Value : (byte?)null;
                                fedralEIN.zipCode = fedralEINVM.zipCode == null ? null : String.Concat(fedralEINVM.zipCode.Where(Char.IsDigit));
                                fedralEIN.countryID = fedralEINVM.countryID.HasValue ? fedralEINVM.countryID.Value : (int?)null;
                                fedralEIN.phoneNumber = fedralEINVM.phoneNumber == null ? null : String.Concat(fedralEINVM.phoneNumber.Where(Char.IsDigit));
                                fedralEIN.faxNumber = fedralEINVM.faxNumber == null ? null : String.Concat(fedralEINVM.faxNumber.Where(Char.IsDigit));
                                fedralEIN.EEOFileStatusID = fedralEINVM.EEOFileStatusID.HasValue ? fedralEINVM.EEOFileStatusID.Value : (byte?)null;
                                fedralEIN.active = fedralEINVM.Active;
                                fedralEIN.notes = fedralEINVM.notes;

                            }
                            else
                            {
                                ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                                string _message = Utils.GetErrorString(null, null, this.ModelState);
                                return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            fedralEIN.EIN = String.Concat(fedralEINVM.EIN.Where(Char.IsDigit));
                            fedralEIN.description = fedralEINVM.Description;
                            fedralEIN.addressLineOne = fedralEINVM.addressLineOne;
                            fedralEIN.addressLineTwo = fedralEINVM.addressLineTwo;
                            fedralEIN.city = fedralEINVM.city;
                            fedralEIN.stateID = fedralEINVM.stateID.HasValue ? fedralEINVM.stateID.Value : (byte?)null;
                            fedralEIN.zipCode = fedralEINVM.zipCode == null ? null : String.Concat(fedralEINVM.zipCode.Where(Char.IsDigit));
                            fedralEIN.countryID = fedralEINVM.countryID.HasValue ? fedralEINVM.countryID.Value : (int?)null;
                            fedralEIN.phoneNumber = fedralEINVM.phoneNumber == null ? null : String.Concat(fedralEINVM.phoneNumber.Where(Char.IsDigit));
                            fedralEIN.faxNumber = fedralEINVM.faxNumber == null ? null : String.Concat(fedralEINVM.faxNumber.Where(Char.IsDigit));
                            fedralEIN.EEOFileStatusID = fedralEINVM.EEOFileStatusID.HasValue ? fedralEINVM.EEOFileStatusID.Value : (byte?)null;
                            fedralEIN.active = fedralEINVM.Active;
                            fedralEIN.notes = fedralEINVM.notes;

                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "EIN with same EIN or description already exists.");
                        string _message = Utils.GetErrorString(null, null, this.ModelState);
                        return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
                    }

                    try
                    {
                        clientDbContext.SaveChanges();
                        ViewBag.AlertMessage = recordIsNew == true ? "New Fedral EIN Added" : "Fedral EIN Saved";
                    }
                    catch (Exception err)
                    {
                        string _message = Utils.GetErrorString(err, clientDbContext, null);
                        return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
                    }
                }

            }
            else
            {
                string _message = Utils.GetErrorString(null, null, this.ModelState);
                return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
            }


            return Json(new { fedralEINVM, succeed = true, recordIsNew }, JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FedralEINList_Update([DataSourceRequest] DataSourceRequest request, FedralEinVM fedralEinVM)
        {
            return Json(new[] { fedralEinVM }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FedralEINList_Destroy([DataSourceRequest] DataSourceRequest request, FedralEinVM fedralEinVM)
        {

            return Json(new[] { fedralEinVM }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FedralEINDelete(int FedralEINNbr)
        {
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            var dbRecord = clientDbContext.DdlEINs.Where(x => x.FedralEINNbr == FedralEINNbr).SingleOrDefault();
            if (dbRecord != null)
            {
                clientDbContext.DdlEINs.Remove(dbRecord);
                try
                {
                    clientDbContext.SaveChanges();
                }
                catch// (Exception ex)
                {
                    return Json(new { Message = CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE, succeed = false }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Message = "Record does not exist.", succeed = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Message = "", succeed = true }, JsonRequestBehavior.AllowGet);

        }

        public List<DropDownModel> getAllCountries()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var lstCountries = clientDbContext.DdlCountries
                    .Select(s => new DropDownModel
                    {
                        keyvalue = s.CountryId.ToString(),
                        keydescription = s.Description
                    }).OrderBy(s => s.keydescription).ToList();

            return lstCountries;
        }


        public List<DropDownModel> getAllStates()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var lstStates = clientDbContext.DdlStates
                    .Select(s => new DropDownModel
                    {
                        keyvalue = s.StateId.ToString(),
                        keydescription = s.Title
                    }).OrderBy(s => s.keydescription).ToList();

            return lstStates;
        }

        public List<DropDownModel> getAllEEOFileStatues()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var lstEEOFIleStatuses = clientDbContext.DdlEEOFileStatuses
                    .Select(s => new DropDownModel
                    {
                        keyvalue = s.EEoFileStatusNbr.ToString(),
                        keydescription = s.Description
                    }).OrderBy(s => s.keydescription).ToList();

            return lstEEOFIleStatuses;
        }
        #endregion
        #region Company

        public ActionResult DdlCompanyCodeListMaintenance()
        {

            return View();
        }

        public ActionResult CompanyCodeList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var lstCompany = clientDbContext.CompanyCodes.OrderBy(e => e.CompanyCodeDescription).ToList();

            List<CompanyCodeVm> lstCompanyVM = new List<CompanyCodeVm>();
            foreach (var item in lstCompany)
            {
                lstCompanyVM.Add(new CompanyCodeVm
                {
                    Code = item.CompanyCodeCode,
                    Description = item.CompanyCodeDescription,
                    Active = item.IsCompanyCodeActive,
                    CompanyCodeId = item.CompanyCodeId
                });
            }

            return Json(lstCompanyVM.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CompanyCodeList_Create([DataSourceRequest] DataSourceRequest request, CompanyCodeVm companyvm)
        {

            if (companyvm != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var StatusInDb = clientDbContext.CompanyCodes.Where(x => x.CompanyCodeCode == companyvm.Code || x.CompanyCodeDescription == companyvm.Description).SingleOrDefault();

                if (StatusInDb != null)
                {
                    ModelState.AddModelError("", "Company " + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                }
                else
                {
                    var newCompany = new CompanyCode { CompanyCodeCode = companyvm.Code, CompanyCodeDescription = companyvm.Description, IsCompanyCodeActive = companyvm.Active };
                    clientDbContext.CompanyCodes.Add(newCompany);
                    clientDbContext.SaveChanges();
                    companyvm.CompanyCodeId = companyvm.CompanyCodeId;
                }

            }

            return Json(new[] { companyvm }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CompanyCodeList_Update([DataSourceRequest] DataSourceRequest request, CompanyCodeVm companyVM)
        {
            if (companyVM != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);

                var companyVMInDb = clientDbContext.CompanyCodes.Where(x => x.CompanyCodeId != companyVM.CompanyCodeId && (x.CompanyCodeCode == companyVM.Code || x.CompanyCodeDescription == companyVM.Description)).SingleOrDefault();
                if (companyVMInDb == null)
                {
                    companyVMInDb = clientDbContext.CompanyCodes.Where(x => x.CompanyCodeId == companyVM.CompanyCodeId).SingleOrDefault();

                    if (companyVMInDb.IsCompanyCodeActive != companyVM.Active && companyVM.Active == false)
                    {
                        if (clientDbContext.Jobs.Where(x => x.CompanyCodeId == companyVM.CompanyCodeId).Count() == 0 &&
                            clientDbContext.Employees.Where(x => x.CompanyCodeId == companyVM.CompanyCodeId).Count() == 0)
                        {
                            companyVMInDb.CompanyCodeCode = companyVM.Code;
                            companyVMInDb.CompanyCodeDescription = companyVM.Description;
                            companyVMInDb.IsCompanyCodeActive = companyVM.Active;
                            clientDbContext.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                        }
                    }
                    else
                    {
                        companyVMInDb.CompanyCodeCode = companyVM.Code;
                        companyVMInDb.CompanyCodeDescription = companyVM.Description;
                        companyVMInDb.IsCompanyCodeActive = companyVM.Active;
                        clientDbContext.SaveChanges();
                    }
                }
                else
                {
                    ModelState.AddModelError("", string.Format(CustomErrorMessages.ERROR_LOOKUP_DUPLICATE_RECORD, "Company"));
                }

            }

            return Json(new[] { companyVM }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CompanyCodeList_Destroy([DataSourceRequest] DataSourceRequest request, CompanyCodeVm companyCode)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (companyCode != null)
                {
                    if (clientDbContext.Jobs.Where(x => x.CompanyCodeId == companyCode.CompanyCodeId).Count() > 0 ||
                            clientDbContext.Employees.Where(x => x.CompanyCodeId == companyCode.CompanyCodeId).Count() > 0)
                    {
                        ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                    }
                    else
                    {
                        CompanyCode companyCodeInDb = clientDbContext.CompanyCodes.Where(x => x.CompanyCodeId == companyCode.CompanyCodeId).SingleOrDefault();

                        if (companyCodeInDb != null)
                        {
                            clientDbContext.CompanyCodes.Remove(companyCodeInDb);

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
                }

                return Json(new[] { companyCode }.ToDataSourceResult(request, ModelState));
            }


        }

        #endregion





        public ActionResult AddPayFrequency()
        {
            var model = new DdlPayFrequency() { Active = true };
            return View(model);
        }
        public ActionResult AddPositionGrade(int salaryGradeID)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var ddlSalaryGradeHistoryList = (from psgsh in clientDbContext.PositionSalaryGradeSourceHistories
                                             join sg in clientDbContext.DdlSalaryGrades on
                                                 psgsh.SalaryGradeID equals sg.SalaryGradeID
                                             where sg.active == true && sg.SalaryGradeID == salaryGradeID
                                             select new SalaryGradeHistoryListVm
                                             {
                                                 description = sg.description,
                                                 salaryMaximum = psgsh.salaryMaximum.ToString(),
                                                 salaryMidpoint = psgsh.salaryMidpoint.ToString(),
                                                 salaryMinimum = psgsh.salaryMinimum.ToString(),
                                                 validFrom = psgsh.ValidDate,
                                                 ChangeDate = psgsh.ChangeEffectiveDate
                                             }).FirstOrDefault();

            //var model = clientDbContext.PositionSalaryGradeSourceHistories.OrderBy(e => e.PositionSalaryGradeHistoriesID).FirstOrDefault();
            return View(ddlSalaryGradeHistoryList);
        }
        public ActionResult AddPositionCategory()
        {
            var model = new DdlPositionCategory() { active = true };
            return View(model);
        }
        public ActionResult AddPositionType()
        {
            var model = new DdlPositionTypes() { active = true };
            return View(model);
        }
        public PartialViewResult GetDdlPositionGradeList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var salarygradeList = clientDbContext.PositionSalaryGradeSourceHistories.OrderBy(e => e.PositionSalaryGradeHistoriesID).FirstOrDefault();

            return PartialView("AddPositionGrade", salarygradeList);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlPositionGradeList_Create([DataSourceRequest] DataSourceRequest request, DdlPositionGrade positionGrade)
        {
            if (positionGrade != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var positionGradeInDb = clientDbContext.DdlPositionGrade.Where(x => x.Code == positionGrade.Code || x.Description == positionGrade.Description).SingleOrDefault();

                if (positionGradeInDb != null)
                {
                    ModelState.AddModelError("", "Position Grade" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                }
                else
                {
                    var newpositionGrade = new DdlPositionGrade { Description = positionGrade.Description, Code = positionGrade.Code, Active = positionGrade.Active };
                    clientDbContext.DdlPositionGrade.Add(newpositionGrade);
                    clientDbContext.SaveChanges();
                    positionGrade.PositionGradeID = newpositionGrade.PositionGradeID;
                }
            }

            return Json(new[] { positionGrade }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult GetDdlPositionCategoryList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var maritalStatusList = clientDbContext.DdlPositionCategory.Where(x => x.active == true).OrderBy(e => e.description).ToList();

            return Json(maritalStatusList, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlPositionCategoryList_Create([DataSourceRequest] DataSourceRequest request, DdlPositionCategory positionCategory)
        {
            if (positionCategory != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var positionGradeInDb = clientDbContext.DdlPositionCategory.Where(x => x.code == positionCategory.code || x.description == positionCategory.description).SingleOrDefault();

                if (positionGradeInDb != null)
                {
                    ModelState.AddModelError("", "Position Category" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                }
                else
                {
                    var newpositionGrade = new DdlPositionCategory { description = positionCategory.description, code = positionCategory.code, active = positionCategory.active };
                    clientDbContext.DdlPositionCategory.Add(newpositionGrade);
                    clientDbContext.SaveChanges();
                    positionCategory.PositionCategoryID = newpositionGrade.PositionCategoryID;
                }
            }

            return Json(new[] { positionCategory }.ToDataSourceResult(request, ModelState));
        }


        public JsonResult GetDdlPositionTypeList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var positionTypeStatusList = clientDbContext.DdlPositionTypes.Where(x => x.active == true).OrderBy(e => e.description).ToList();

            return Json(positionTypeStatusList, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlPositionTypeList_Create([DataSourceRequest] DataSourceRequest request, DdlPositionTypes positionType)
        {
            if (positionType != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var positionTypeInDb = clientDbContext.DdlPositionTypes.Where(x => x.code == positionType.code || x.description == positionType.description).SingleOrDefault();

                if (positionTypeInDb != null)
                {
                    ModelState.AddModelError("", "Position Type" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                }
                else
                {
                    var newpositionType = new DdlPositionTypes { description = positionType.description, code = positionType.code, active = positionType.active };
                    clientDbContext.DdlPositionTypes.Add(newpositionType);
                    clientDbContext.SaveChanges();
                    positionType.PositionTypeId = positionType.PositionTypeId;
                }
            }

            return Json(new[] { positionType }.ToDataSourceResult(request, ModelState));
        }


        public JsonResult GetPositionTypesList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);


            var maritalStatusList = clientDbContext.DdlPositionTypes.Where(x => x.active == true).OrderBy(e => e.description).ToList();

            return Json(maritalStatusList, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlPositionTypesList_Create([DataSourceRequest] DataSourceRequest request, DdlPositionTypes positionType)
        {
            if (positionType != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var positionGradeInDb = clientDbContext.DdlPositionTypes.Where(x => x.code == positionType.code || x.description == positionType.description).SingleOrDefault();

                if (positionGradeInDb != null)
                {
                    ModelState.AddModelError("", "Position Types" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                }
                else
                {
                    var newPositionType = new DdlPositionTypes { description = positionType.description, code = positionType.code, active = positionType.active };
                    clientDbContext.DdlPositionTypes.Add(newPositionType);
                    clientDbContext.SaveChanges();
                    positionType.PositionTypeId = newPositionType.PositionTypeId;
                }
            }

            return Json(new[] { positionType }.ToDataSourceResult(request, ModelState));
        }




        #region Position Grades

        public ActionResult DdlPositionGradesListMaintenance()
        {

            return View();
        }

        public ActionResult DdlPositionGradeList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var lstPositionGrades = clientDbContext.DdlPositionGrade.OrderBy(e => e.Description).ToList();

            return Json(lstPositionGrades.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlPositionGradeList_Update([DataSourceRequest] DataSourceRequest request, DdlPositionGrade positionGrade)
        {
            if (positionGrade != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);

                var positionGradeInDb = clientDbContext.DdlPositionGrade.Where(x => x.PositionGradeID != positionGrade.PositionGradeID && (x.Code == positionGrade.Code || x.Description == positionGrade.Description)).FirstOrDefault();
                if (positionGradeInDb == null)
                {
                    positionGradeInDb = clientDbContext.DdlPositionGrade.Where(x => x.PositionGradeID == positionGrade.PositionGradeID).SingleOrDefault();

                    if (positionGradeInDb.Active != positionGrade.Active && positionGrade.Active == false)
                    {
                        if (clientDbContext.Positions.Where(x => x.PositionGradeID == positionGrade.PositionGradeID).Count() == 0)
                        {
                            positionGradeInDb.Code = positionGrade.Code;
                            positionGradeInDb.Description = positionGrade.Description;
                            positionGradeInDb.Active = positionGrade.Active;
                            clientDbContext.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                        }
                    }
                    else
                    {
                        positionGradeInDb.Code = positionGrade.Code;
                        positionGradeInDb.Description = positionGrade.Description;
                        positionGradeInDb.Active = positionGrade.Active;
                        clientDbContext.SaveChanges();
                    }
                }
                else
                {
                    ModelState.AddModelError("", string.Format(CustomErrorMessages.ERROR_LOOKUP_DUPLICATE_RECORD, "Position Grade"));
                }

            }

            return Json(new[] { positionGrade }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlPositionGradeList_Destroy([DataSourceRequest] DataSourceRequest request, DdlPositionGrade positionGrade)
        {
            if (positionGrade != null)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                DdlPositionGrade positionGradeInDb = clientDbContext.DdlPositionGrade.Where(x => x.PositionGradeID == positionGrade.PositionGradeID).SingleOrDefault();

                if (positionGradeInDb != null)
                {
                    if (clientDbContext.Positions.Where(x => x.PositionGradeID == positionGrade.PositionGradeID).Count() > 0)
                    {
                        ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                    }
                    else
                    {
                        clientDbContext.DdlPositionGrade.Remove(positionGradeInDb);
                        try
                        {
                            clientDbContext.SaveChanges();
                        }
                        catch { ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE); }
                    }
                }
            }

            return Json(new[] { positionGrade }.ToDataSourceResult(request, ModelState));
        }

        #endregion


        #region Position Categories

        public ActionResult DdlPositionCategoriesListMaintenance()
        {

            return View();
        }

        public ActionResult DdlPositionCategoryList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var lstPositionCategories = clientDbContext.DdlPositionCategory.OrderBy(e => e.description).ToList();

            List<PositionCategoriesVM> lstpositionCategoryVM = new List<PositionCategoriesVM>();

            foreach (var item in lstPositionCategories)
            {
                lstpositionCategoryVM.Add(new PositionCategoriesVM { PositionCategoryID = item.PositionCategoryID, Code = item.code, Description = item.description, Active = item.active });
            }

            return Json(lstpositionCategoryVM.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }

        public ActionResult DdlPositionCategorysList_Create([DataSourceRequest] DataSourceRequest request, PositionCategoriesVM positionCategory)
        {
            if (positionCategory != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var positionGradeInDb = clientDbContext.DdlPositionCategory.Where(x => x.code == positionCategory.Code || x.description == positionCategory.Description).SingleOrDefault();

                if (positionGradeInDb != null)
                {
                    ModelState.AddModelError("", "Position Category" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                }
                else
                {
                    var newpositionCategory = new DdlPositionCategory { description = positionCategory.Description, code = positionCategory.Code, active = positionCategory.Active };
                    clientDbContext.DdlPositionCategory.Add(newpositionCategory);
                    clientDbContext.SaveChanges();
                    positionCategory.PositionCategoryID = newpositionCategory.PositionCategoryID;
                    positionCategory.Code = newpositionCategory.code;
                    positionCategory.Description = newpositionCategory.description;
                    positionCategory.Active = newpositionCategory.active;
                }
            }

            return Json(new[] { positionCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlPositionCategoryList_Update([DataSourceRequest] DataSourceRequest request, PositionCategoriesVM positionCategoryVM)
        {
            if (positionCategoryVM != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);

                var positionCategoryInDb = clientDbContext.DdlPositionCategory.Where(x => x.PositionCategoryID != positionCategoryVM.PositionCategoryID && (x.code == positionCategoryVM.Code || x.description == positionCategoryVM.Description)).FirstOrDefault();
                if (positionCategoryInDb == null)
                {
                    positionCategoryInDb = clientDbContext.DdlPositionCategory.Where(x => x.PositionCategoryID == positionCategoryVM.PositionCategoryID).SingleOrDefault();

                    if (positionCategoryInDb.active != positionCategoryVM.Active && positionCategoryVM.Active == false)
                    {
                        if (clientDbContext.Positions.Where(x => x.PositionCategoryID == positionCategoryVM.PositionCategoryID).Count() == 0)
                        {
                            positionCategoryInDb.code = positionCategoryVM.Code;
                            positionCategoryInDb.description = positionCategoryVM.Description;
                            positionCategoryInDb.active = positionCategoryVM.Active;
                            clientDbContext.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                        }
                    }
                    else
                    {
                        positionCategoryInDb.code = positionCategoryVM.Code;
                        positionCategoryInDb.description = positionCategoryVM.Description;
                        positionCategoryInDb.active = positionCategoryVM.Active;
                        clientDbContext.SaveChanges();
                    }
                }
                else
                {
                    ModelState.AddModelError("", string.Format(CustomErrorMessages.ERROR_LOOKUP_DUPLICATE_RECORD, "Position Category"));
                }

            }

            return Json(new[] { positionCategoryVM }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlPositionCategoryList_Destroy([DataSourceRequest] DataSourceRequest request, PositionCategoriesVM positionCategoryVM)
        {
            if (positionCategoryVM != null)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                DdlPositionCategory positionCategoryInDb = clientDbContext.DdlPositionCategory.Where(x => x.PositionCategoryID == positionCategoryVM.PositionCategoryID).SingleOrDefault();

                if (positionCategoryInDb != null)
                {
                    if (clientDbContext.Positions.Where(x => x.PositionCategoryID == positionCategoryVM.PositionCategoryID).Count() > 0)
                    {
                        ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                    }
                    else
                    {
                        clientDbContext.DdlPositionCategory.Remove(positionCategoryInDb);
                        try
                        {
                            clientDbContext.SaveChanges();
                        }
                        catch { ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE); }
                    }
                }
            }

            return Json(new[] { positionCategoryVM }.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region States

        public ActionResult DdlStatesListMaintenance()
        {

            return View();
        }

        public ActionResult DdlStatesList_Read([DataSourceRequest]DataSourceRequest request)
        {
            var connString = User.Identity.GetClientConnectionString();
            var clientDbContext = new ClientDbContext(connString);

            var lstDdlStates = clientDbContext.DdlStates.OrderBy(e => e.Title).ToList();

            var llstDdlStatesVM = new List<StateListVM>();

            foreach (var item in lstDdlStates)
            {
                llstDdlStatesVM.Add(new StateListVM { StateID = item.StateId, Code = item.Code, Description = item.Title });
            }

            return Json(llstDdlStatesVM.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }

        public ActionResult DdlStatesList_Create([DataSourceRequest] DataSourceRequest request, StateListVM statesVm)
        {
            if (statesVm != null && ModelState.IsValid)
            {
                var connString = User.Identity.GetClientConnectionString();
                var clientDbContext = new ClientDbContext(connString);
                var positionGradeInDb = clientDbContext.DdlStates.Where(x => x.Code == statesVm.Code || x.Title == statesVm.Description).SingleOrDefault();

                if (positionGradeInDb != null)
                {
                    ModelState.AddModelError("", "States" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                }
                else
                {
                    var newStatesVm = new DdlState { Title = statesVm.Description, Code = statesVm.Code };
                    clientDbContext.DdlStates.Add(newStatesVm);
                    clientDbContext.SaveChanges();
                    statesVm.StateID = newStatesVm.StateId;
                    statesVm.Code = newStatesVm.Code;
                    statesVm.Description = newStatesVm.Title;
                }
            }

            return Json(new[] { statesVm }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlStatesList_Update([DataSourceRequest] DataSourceRequest request, StateListVM statesLVM)
        {
            if (statesLVM != null && ModelState.IsValid)
            {
                var connString = User.Identity.GetClientConnectionString();
                var clientDbContext = new ClientDbContext(connString);

                var statesInDb = clientDbContext.DdlStates.Where(x => x.StateId != statesLVM.StateID && (x.Code == statesLVM.Code || x.Title == statesLVM.Description)).FirstOrDefault();
                if (statesInDb == null)
                {
                    statesInDb = clientDbContext.DdlStates.Where(x => x.StateId == statesLVM.StateID).SingleOrDefault();

                    if (statesLVM.Active && statesLVM.Active == false)
                    {
                        if (clientDbContext.DdlStates.Where(x => x.StateId == statesLVM.StateID).Count() == 0)
                        {
                            statesInDb.Code = statesLVM.Code;
                            statesInDb.Title = statesLVM.Description;
                            clientDbContext.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                        }
                    }
                    else
                    {
                        statesInDb.Code = statesLVM.Code;
                        statesInDb.Title = statesLVM.Description;
                        clientDbContext.SaveChanges();
                    }
                }
                else
                {
                    ModelState.AddModelError("", string.Format(CustomErrorMessages.ERROR_LOOKUP_DUPLICATE_RECORD, "State"));
                }

            }

            return Json(new[] { statesLVM }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlStates_Destroy([DataSourceRequest] DataSourceRequest request, StateListVM stateListVM)
        {
            if (stateListVM != null)
            {
                var connString = User.Identity.GetClientConnectionString();
                var clientDbContext = new ClientDbContext(connString);
                var statesInDb = clientDbContext.DdlStates.Where(x => x.StateId == stateListVM.StateID).SingleOrDefault();

                if (statesInDb != null)
                {
                    if (clientDbContext.DdlStates.Where(x => x.StateId == stateListVM.StateID).Count() > 0)
                    {
                        ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                    }
                    else
                    {
                        clientDbContext.DdlStates.Remove(statesInDb);
                        try
                        {
                            clientDbContext.SaveChanges();
                        }
                        catch { ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE); }
                    }
                }
            }

            return Json(new[] { stateListVM }.ToDataSourceResult(request, ModelState));
        }

        #endregion
        public ActionResult AddDisability()
        {
            var model = new DdlDisabilityType() { Active = true };
            return View(model);
        }
        public ActionResult AddAccomodation()
        {
            var model = new DdlAccommodationType() { Active = true };
            return View(model);
        }

        #region Relationship Types
        public ActionResult DdlRelationshipTypesList()
        {
            DdlRelationshipTypeVm ddlRelationshipTypeVm = new DdlRelationshipTypeVm();
            return View(ddlRelationshipTypeVm);
        }

        public ActionResult DdlRelationshipTypesList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var relationshipTypesList = _lookupTablesRepository.getDdlRelationshipTypesList();
            return Json(relationshipTypesList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult DdlRelationshipTypesDetails(int relationshipTypeId)
        {
            DdlRelationshipTypeVm ddlRelationshipTypeVm = new DdlRelationshipTypeVm();
            ddlRelationshipTypeVm = _lookupTablesRepository.getDdlRelationshipTypesDetails(relationshipTypeId);
            return View(ddlRelationshipTypeVm);
        }

        [HttpPost]
        public ActionResult DdlRelationshipTypesSaveAjax(DdlRelationshipTypeVm ddlRelationshipTypeVm)
        {
            bool recordIsNew = false;
            string _message = "";
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            DdlRelationshipType ddlRelationshipTypedb = clientDbContext.DdlRelationshipTypes.Where(x => x.RelationshipTypeId == ddlRelationshipTypeVm.RelationshipTypeId).SingleOrDefault();
            if (ddlRelationshipTypedb != null && ModelState.IsValid)
            {
                try
                {
                    var relationshipTypeInDb = clientDbContext.DdlRelationshipTypes
                        .Where(x => x.RelationshipTypeId == ddlRelationshipTypeVm.RelationshipTypeId)
                        .SingleOrDefault();
                    var relationtypelist = clientDbContext.DdlRelationshipTypes.Where(n => n.RelationshipTypeId != ddlRelationshipTypeVm.RelationshipTypeId).ToList();
                    var usedrelationlist = clientDbContext.PersonRelationships.ToList();
                    var active = relationshipTypeInDb.Active;
                    if (relationshipTypeInDb != null)
                    {
                        if (relationtypelist.Select(m => m.Code).Contains(ddlRelationshipTypeVm.Code) || relationtypelist.Select(m => m.Description).Contains(ddlRelationshipTypeVm.Description))
                        {
                            ModelState.AddModelError("", "The Relationship Type Code or Description already exists!");
                            var modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);
                            foreach (var item in modelStateErrors)
                            {
                                if (_message != "") _message += "<br />";
                                _message += item.ErrorMessage;
                            }
                            return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
                        }
                        else if (usedrelationlist.Select(s => s.DdlRelationshipType.Description).Contains(ddlRelationshipTypeVm.Description) && active != ddlRelationshipTypeVm.Active)
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                            var modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);
                            foreach (var item in modelStateErrors)
                            {
                                if (_message != "") _message += "<br />";
                                _message += item.ErrorMessage;
                            }
                            return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            if (ddlRelationshipTypeVm.RelationshipTypeId == 0) { recordIsNew = true; }
                            ddlRelationshipTypeVm = _lookupTablesRepository.updateDdlRelationshipTypes(ddlRelationshipTypeVm);
                        }
                    }
                    return Json(new { ddlRelationshipTypeVm, succeed = true, recordIsNew }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    IEnumerable<DbEntityValidationResult> errors = clientDbContext.GetValidationErrors();
                    if (errors.Any())
                    {
                        _message += err.InnerException.InnerException.Message;
                    }
                    else
                    {
                        foreach (DbEntityValidationResult error in errors)
                        {
                            foreach (var valError in error.ValidationErrors)
                            {
                                if (_message != "") _message += "<br />";
                                _message += valError.ErrorMessage;
                            }
                        }
                    }
                    return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var relationshipTypeInDb = clientDbContext.DdlRelationshipTypes
                     .Where(x => x.Code == ddlRelationshipTypeVm.Code)
                        .SingleOrDefault();
                var relationshipTypeDescInDb = clientDbContext.DdlRelationshipTypes.ToList();

                var relationtypelist = clientDbContext.DdlRelationshipTypes.Where(n => n.RelationshipTypeId != ddlRelationshipTypeVm.RelationshipTypeId).ToList();
                var relationDesc = relationshipTypeDescInDb.Select(x => x.Description == ddlRelationshipTypeVm.Description);

                if (relationshipTypeInDb != null || relationDesc != null)
                {
                    if (relationtypelist.Select(m => m.Code).Contains(ddlRelationshipTypeVm.Code) || relationtypelist.Select(m => m.Description).Contains(ddlRelationshipTypeVm.Description))
                    {
                        ModelState.AddModelError("", "The Relationship Type Code or Description already exists!");
                        var modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);
                        foreach (var item in modelStateErrors)
                        {
                            if (_message != "") _message += "<br />";
                            _message += item.ErrorMessage;
                        }
                        return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ddlRelationshipTypeVm = _lookupTablesRepository.updateDdlRelationshipTypes(ddlRelationshipTypeVm);
                    }
                }
                return Json(new { ddlRelationshipTypeVm, succeed = true, recordIsNew }, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlRelationshipTypesDelete(int relationshipTypeId)
        {
            string _message = "";
            try
            {
                _lookupTablesRepository.deleteDdlRelationshipTypes(relationshipTypeId);
            }
            catch
            {
                ModelState.AddModelError("", "Can not be deleted due to record is already in use or not exists.");
                var modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);
                foreach (var item in modelStateErrors)
                {
                    if (_message != "") _message += "<br />";
                    _message += item.ErrorMessage;
                }
                return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Message = "", succeed = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddSalaryComponentType()
        {
            var model = new ddlSalaryComponents() { active = true };
            return View(model);
        }
        public ActionResult AddPaytype()
        {
            var model = new ddlPayTypes() { active = true };
            return View(model);
        }
        #endregion

        #region EducationEstablishments
        public ActionResult DdlEducationEstablishmentsList()
        {
            ExecViewHrk.Models.DdlEducationEstablishmentViewModel ddlEducationEstablishmentViewModel = new ExecViewHrk.Models.DdlEducationEstablishmentViewModel();
            return View(ddlEducationEstablishmentViewModel);
        }
        public ActionResult DdlEducationEstablishmentsList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var educationEstablishmentsList = _lookupTablesRepository.getDdlEducationEstablishmentList();
            return Json(educationEstablishmentsList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult EducationEstablishmentsListMaintenance(int educationestablishmentId)
        {

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            ExecViewHrk.Models.DdlEducationEstablishmentViewModel ddlEducationEstablishmentViewModel = new ExecViewHrk.Models.DdlEducationEstablishmentViewModel();
            var objEduEsta = clientDbContext.DdlEducationEstablishments.Where(x => x.EducationEstablishmentId == educationestablishmentId).FirstOrDefault();
            ddlEducationEstablishmentViewModel.Code = objEduEsta.Code;
            ddlEducationEstablishmentViewModel.Description = objEduEsta.Description;
            ddlEducationEstablishmentViewModel.StateId = objEduEsta.StateId;
            ddlEducationEstablishmentViewModel.ZipCode = objEduEsta.ZipCode;
            ddlEducationEstablishmentViewModel.PhoneNumber = objEduEsta.PhoneNumber;
            ddlEducationEstablishmentViewModel.AddressLineOne = objEduEsta.AddressLineOne;
            ddlEducationEstablishmentViewModel.AddressLineTwo = objEduEsta.AddressLineTwo;
            ddlEducationEstablishmentViewModel.City = objEduEsta.City;
            ddlEducationEstablishmentViewModel.FaxNumber = objEduEsta.FaxNumber;
            ddlEducationEstablishmentViewModel.AccountNumber = objEduEsta.AccountNumber;
            ddlEducationEstablishmentViewModel.Contact = objEduEsta.Contact;
            ddlEducationEstablishmentViewModel.WebAddress = objEduEsta.WebAddress;
            ddlEducationEstablishmentViewModel.Active = objEduEsta.Active;
            ddlEducationEstablishmentViewModel.StateDropDownList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetStateList());
            ddlEducationEstablishmentViewModel.CountryDropDownList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetCountryList());
            return View(ddlEducationEstablishmentViewModel);
        }

        public string GetStateList()
        {
            var lstState = _lookupTablesRepository.GetStateList().CleanUp();
            ViewData["stateList"] = lstState;
            return JsonConvert.SerializeObject(lstState);
        }

        public string GetCountryList()
        {

            var lstCountry = _lookupTablesRepository.GetCountryList().CleanUp();
            ViewData["countryList"] = lstCountry;
            return JsonConvert.SerializeObject(lstCountry);
        }

        public ActionResult EducationEstablishmentsList()
        {
            GetStateList();
            GetCountryList();
            return View();
        }

        [HttpPost]
        public ActionResult DdlEducationEstablishmentsList_Create([DataSourceRequest] DataSourceRequest request
           , ExecViewHrk.Models.DdlEducationEstablishmentViewModel vmEducationEstablishment)
        {
            if (ModelState.IsValid)
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
            else
            {
                var modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);
                string _message = "";
                foreach (var item in modelStateErrors)
                {
                    if (_message != "") _message += "\n";
                    _message += item.ErrorMessage;
                }
                _message += "\n\nRecord could not be updated at this time.";
                return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult DdlEducationEstablishmentsList_Destroy([DataSourceRequest] DataSourceRequest request
          , ExecViewHrk.Models.DdlEducationEstablishmentViewModel vmEducationEstablishment)
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
                        catch (Exception err)
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                        }

                    }
                }

                return Json(new[] { vmEducationEstablishment }.ToDataSourceResult(request, ModelState));
            }
        }
        #endregion

        #region TimeCards Bindings
        [HttpPost]
        public JsonResult GetCompanyCodes()
        {
            var companyCodes = Enumerable.Empty<CompanyCodeVM>();
            if (User.IsInRole("ClientEmployees"))
            {
                return GetStudentCompanyCodes();
            }
            else
                companyCodes = _lookupTablesRepository.GetCompanyCodes();
            return Json(companyCodes, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Student Login: #2915:Returns multi companies which student belongs to...
        /// </summary>
        /// <returns></returns>
        public JsonResult GetStudentCompanyCodes()
        {
            string connString = User.Identity.GetClientConnectionString();
            string username = "";
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                username = User.Identity.Name;
            }
            var companyCodes = _lookupTablesRepository.GetStudentCompanyCodes(username);
            return Json(companyCodes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDepartmentsList(string filterInput, short? CompanyCodeIdDdl)
        {
            var departmentList = Enumerable.Empty<DepartmentVm>();
            if (User.IsInRole("HrkAdministrators") || User.IsInRole("ClientAdministrators"))
            {
                departmentList = _lookupTablesRepository.GetAdminDepartmentsList(CompanyCodeIdDdl);
            }
            else if (User.IsInRole("ClientManagers"))
            {
                string useridentityname = User.Identity.Name;
                departmentList = _lookupTablesRepository.GetManagersDepartmentsList(CompanyCodeIdDdl, useridentityname);
            }
            else
            {
                return GetStudentDepartmentsList(filterInput, CompanyCodeIdDdl);
            }
            if (!string.IsNullOrEmpty(filterInput))
                departmentList = (departmentList.Where(c => c.CompCode_DeptCode_DeptDescription.ToLower().Contains(filterInput.ToLower()))).ToList();
            return Json(departmentList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        //Student Login: #2915: Returns Departments of Students based on assigned Positions
        /// </summary>
        /// <param name="filterInput"></param>
        /// <param name="CompanyCodeIdDdl"></param>
        /// <returns></returns>
        public JsonResult GetStudentDepartmentsList(string filterInput, short? CompanyCodeIdDdl)
        {
            string useridentityname = User.Identity.Name;
            var departmentList = Enumerable.Empty<DepartmentVm>();
                departmentList = _lookupTablesRepository.GetStudentDepartmentsList(CompanyCodeIdDdl, useridentityname);
            if (!string.IsNullOrEmpty(filterInput))
                departmentList = (departmentList.Where(c => c.CompCode_DeptCode_DeptDescription.ToLower().Contains(filterInput.ToLower()))).ToList();
            return Json(departmentList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployeesList(string filterInput, short? DepartmentIdDdl, bool isActive)
        {
            /*public JsonResult GetEmployeesList()
                {
               string connString = User.Identity.GetClientConnectionString();
               ClientDbContext clientDbContext = new ClientDbContext(connString);
               string requestType = User.Identity.GetRequestType();
               var employeeList = _lookupTablesRepository.GetEmployeesList(DepartmentIdDdl);
               //var employeeList = _lookupTablesRepository.GetEmployeesList();
               return Json(employeeList, JsonRequestBehavior.AllowGet);
              } */

            var employeeList = Enumerable.Empty<EmployeesVM>();
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            string requestType = User.Identity.GetRequestType();
            if (User.IsInRole("HrkAdministrators") || User.IsInRole("ClientAdministrators"))
            {
                employeeList = _lookupTablesRepository.GetEmployeesList(DepartmentIdDdl, isActive);
            }
            if (User.IsInRole("ClientManagers"))
            {
                string useridentityname = User.Identity.Name;
                employeeList = _lookupTablesRepository.GetManagerEmployeesList(DepartmentIdDdl, useridentityname, isActive);
            }
            if (User.IsInRole("ClientEmployees"))
            {
                string useridentityname = User.Identity.Name;
                employeeList = _lookupTablesRepository.GetStudent(DepartmentIdDdl, useridentityname);
            }
            if (!string.IsNullOrEmpty(filterInput))
                employeeList = (employeeList.Where(e => e.EmployeeFullName.ToLower().Contains(filterInput.ToLower()))).ToList();
            return Json(employeeList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployees(string filterInput, int CompanyCodeIdDdl)
        {

            var employeeList = Enumerable.Empty<EmployeesVM>();
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            string requestType = User.Identity.GetRequestType();
            if (User.IsInRole("HrkAdministrators") || User.IsInRole("ClientAdministrators"))
            {
                employeeList = _lookupTablesRepository.GetEmployees(CompanyCodeIdDdl);
            }
            if (User.IsInRole("ClientManagers"))
            {
                string useridentityname = User.Identity.Name;
                employeeList = _lookupTablesRepository.GetManagerEmployeesListByCompanyId(CompanyCodeIdDdl, useridentityname);
               
            }            
            if (!string.IsNullOrEmpty(filterInput))
                employeeList = (employeeList.Where(e => e.EmployeeFullName.ToLower().Contains(filterInput.ToLower()))).ToList();
            return Json(employeeList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHoursCode(string filterInput, int CompanyCodeIdDdl)
        {
            var HoursCodeLst = Enumerable.Empty<HoursCodeVm>();
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            string requestType = User.Identity.GetRequestType();
            //if (User.IsInRole("HrkAdministrators") || User.IsInRole("ClientAdministrators"))
            //{
                HoursCodeLst = _lookupTablesRepository.GetHoursList(CompanyCodeIdDdl);
           // }
            if (!string.IsNullOrEmpty(filterInput))
                HoursCodeLst = (HoursCodeLst.Where(e => e.HoursCodeCode.ToLower().Contains(filterInput.ToLower()))).ToList();
            return Json(HoursCodeLst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHourPayPeriodsList(int? EmployeeIdDdl)
        {

            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                EmployeeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).EmployeeId : EmployeeIdDdl;
            }
            var payPeriodsList = _lookupTablesRepository.GetHourPayPeriodsList(EmployeeIdDdl);
            return Json(payPeriodsList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPositionList(string filterInput, int Employeeid)
        {

            var positionList = Enumerable.Empty<Positions>();
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            string requestType = User.Identity.GetRequestType();
            if (User.IsInRole("HrkAdministrators") || User.IsInRole("ClientAdministrators"))
            {
                positionList = _lookupTablesRepository.GetPositionList(Employeeid);
            }
            if (User.IsInRole("ClientManagers"))
            {
                string useridentityname = User.Identity.Name;
                positionList = _lookupTablesRepository.GetManagerPositionByEmployeeId(Employeeid, useridentityname);
            }

            return Json(positionList, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetPayPeriodsList(int? CompanyCodeIdDdl, bool IsArchived)
        public JsonResult GetPayPeriodsList(int? EmployeeIdDdl)
        {
            bool IsArchived = false;
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                //CompanyCodeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).CompanyCodeId : CompanyCodeIdDdl;
                EmployeeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).EmployeeId : EmployeeIdDdl;
            }
            var payPeriodsList = _lookupTablesRepository.GetPayPeriodsList(EmployeeIdDdl, IsArchived);
            return Json(payPeriodsList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidHourCodes(int? CompanyCodeIdDdl)
        {
            IEnumerable<HoursCodeVm> hourCodesList = new List<HoursCodeVm>();

            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                CompanyCodeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).CompanyCodeId : CompanyCodeIdDdl;

                hourCodesList = _lookupTablesRepository.ValidateHoursCodes(CompanyCodeIdDdl);
            }
            return Json(hourCodesList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidEarningCodes(int? CompanyCodeIdDdl)
        {
            IEnumerable<EarningCodeVm> earningCodesList = new List<EarningCodeVm>();

            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                CompanyCodeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).CompanyCodeId : CompanyCodeIdDdl;
                earningCodesList = _lookupTablesRepository.ValidEarningCodes(CompanyCodeIdDdl);
            }
            return Json(earningCodesList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ValidTempDeptCodes(int? CompanyCodeIdDdl)
        {
            IEnumerable<DepartmentVm> tempDepartmentCodesList = new List<DepartmentVm>();

            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                CompanyCodeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).CompanyCodeId : CompanyCodeIdDdl;
                tempDepartmentCodesList = _lookupTablesRepository.ValidTempDeptCodes(CompanyCodeIdDdl);
            }
            return Json(tempDepartmentCodesList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidTempJobCodes(int? CompanyCodeIdDdl)
        {
            IEnumerable<JobsVM> tempJobCodesList = new List<JobsVM>();

            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                CompanyCodeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).CompanyCodeId : CompanyCodeIdDdl;

                tempJobCodesList = _lookupTablesRepository.ValidateTempJobCodes(CompanyCodeIdDdl);
            }

            return Json(tempJobCodesList, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult ValidateEmployeePositions(int? EmployeeIdDdl)
        //{
        //    List<PositionsVM> employeePositions = new List<PositionsVM>();
        //    string connString = User.Identity.GetClientConnectionString();
        //    string loggedinuserid = User.Identity.Name;
        //    using (ClientDbContext clientDbContext = new ClientDbContext(connString))
        //    {
        //        if (User.IsInRole("ClientEmployees") || User.IsInRole("HrkAdministrators") || User.IsInRole("ClientAdministrators"))
        //        {
        //            EmployeeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).EmployeeId : EmployeeIdDdl;
        //            employeePositions = _lookupTablesRepository.ValidateEmployeePositions(EmployeeIdDdl);
        //        }
        //        else
        //        {
        //            EmployeeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).EmployeeId : EmployeeIdDdl;
        //            employeePositions = _lookupTablesRepository.ValidateEmployeePositionManager(EmployeeIdDdl, loggedinuserid);
        //        }
        //    }
        //    return Json(employeePositions, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult ValidateEmployeePositionsByPayPeriod(int? EmployeeIdDdl, int payPeriodId)
        {
            List<PositionsVM> employeePositions = new List<PositionsVM>();
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (User.IsInRole("ClientEmployees") || User.IsInRole("HrkAdministrators") || User.IsInRole("ClientAdministrators"))
                {
                    EmployeeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).EmployeeId : EmployeeIdDdl;

                    employeePositions = _lookupTablesRepository.ValidateEmployeePositionsByPayPeriod(EmployeeIdDdl, payPeriodId);
                }
                else
                {
                    string loggedInUserId = User.Identity.Name;
                    EmployeeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).EmployeeId : EmployeeIdDdl;
                    employeePositions = _lookupTablesRepository.ValidateEmployeePositionManager(EmployeeIdDdl, payPeriodId, loggedInUserId);
                }
            }

            return Json(employeePositions, JsonRequestBehavior.AllowGet);
        }

        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }

        //Checks whether the Administrator has approved the TimeCard.
        //private bool IsAdministratorApprovedTimeCard(int employeeId, int payPeriodId)
        //{
        //    bool isAdminApproved = false;
        //    string connString = User.Identity.GetClientConnectionString();
        //    using (ClientDbContext clientDbContext = new ClientDbContext(connString))
        //    {
        //        //Finds whether Administrator Approved
        //        var approvalList = clientDbContext.TimeCardApprovals.Where(x => x.EmployeeId == employeeId && x.PayPeriodId == payPeriodId).ToList();
        //        foreach (var approvedUser in approvalList)
        //        {
        //            string approvedUserId;
        //            approvedUserId = approvedUser.ManagerId.ToString();
        //            AppUser ApprovedUser = UserManager.FindByName(approvedUserId);
        //            IList<string> userRoles = UserManager.GetRoles(ApprovedUser.Id);
        //            string ApprovedUserRole = userRoles.FirstOrDefault();
        //            if (ApprovedUserRole == "HrkAdministrators" || ApprovedUserRole == "ClientAdministrators")
        //            {
        //                isAdminApproved = approvedUser.Approved;
        //                break;
        //            }
        //        }
        //    }
        //    return isAdminApproved;
        //}

        [HttpGet]
        public JsonResult getTimeCardApprovalStatus_Ajax(int employeeId, int payPeriodId)//TimeCardVm timeCardVm,string employeeId)
        {
            string connString = User.Identity.GetClientConnectionString();
            bool timeCardStatus = false;
            //bool isAdminApproved = false;
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                //Verifies whether Administrator has Approved the TimeCard.
                //isAdminApproved = IsAdministratorApprovedTimeCard(employeeId, payPeriodId);
                
                //Verifies the Manger Approvals...
                var empId = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).EmployeeId : Convert.ToInt32(employeeId);
                if (payPeriodId != 0 && empId != 0)
                {
                    timeCardStatus = _lookupTablesRepository.getTimeCardApproval(employeeId, payPeriodId, User.Identity.GetLoggedInUserRoleName(), User.Identity.Name);//, isAdminApproved);
                }
            }
            return Json(timeCardStatus, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult getTimeCardApprovalStatus_Ajax(TimeCardVm timeCardVm)
        //{
        //    string connString = User.Identity.GetClientConnectionString();
        //    bool timeCardStatus = false;
        //    using (ClientDbContext clientDbContext = new ClientDbContext(connString))
        //    {
        //        var approvedBy = clientDbContext.TimeCards.Where(r => r.UserId == User.Identity.Name).Select(r => r.ApprovedBy).FirstOrDefault();
        //        var empId = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).EmployeeId : timeCardVm.EmployeeId;

        //        if (timeCardVm.PayPeriodId.HasValue && empId != 0)
        //        {
        //            timeCardStatus = _lookupTablesRepository.getTimeCardApproval(timeCardVm, empId, User.Identity.GetLoggedInUserRoleName(), User.Identity.Name);
        //        }
        //    }
                
        [HttpPost]
        //Approves / Disapproves punches for the Employee and Pay Period
        public ActionResult TimeCard_Approved_Ajax(TimeCardVm timeCardVm)
        {
            string connString = User.Identity.GetClientConnectionString();
            bool result = false;
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (timeCardVm.PayPeriodId.HasValue && timeCardVm.EmployeeId != 0)
                {
                    result = _lookupTablesRepository.GetTimeCard_Approved(timeCardVm, User.Identity.Name, User.Identity.GetLoggedInUserRoleName()); ;
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Approves All Time Card based on Weekly Session Time
        [HttpPost]
        public ActionResult TimeCardApproveAll(TimeCardVm timeCardVm)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                bool result = false;
                Session["Isvalid"] = false;
                DateTime? effdate = null;
                TimeCardApproval timeCardApprovalRecordInDb = new TimeCardApproval();
               // bool result1 = false;
                bool IsArchived = false;
                List<TimeCard> empTimeCardList = new List<TimeCard>();
                var timeCardApproval = clientDbContext.TimeCardApprovals
                                  .Where(x => x.EmployeeId == timeCardVm.EmployeeId && x.PayPeriodId == timeCardVm.PayPeriodId).ToList();
                var payPeriod = clientDbContext.PayPeriods.Where(x => x.PayPeriodId == timeCardVm.PayPeriodId).FirstOrDefault();
                if (User.IsInRole("ClientManagers"))
                {
                    var reportstoid = clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name).Select(x => x.PersonId).FirstOrDefault();
                    timeCardApprovalRecordInDb = timeCardApproval.Where(x => x.ManagerId == User.Identity.Name).SingleOrDefault();

                    empTimeCardList = (from t in clientDbContext.TimeCards
                                       join ep in clientDbContext.E_Positions on t.PositionId equals ep.PositionId
                                       where t.EmployeeId == timeCardVm.EmployeeId &&
                                       (t.ActualDate >= payPeriod.StartDate && t.ActualDate <= payPeriod.EndDate) &&
                                       ep.ReportsToID == reportstoid && ep.EmployeeId == timeCardVm.EmployeeId && t.IsDeleted == false && ep.IsDeleted == false
                                       select t
                                     ).Distinct().ToList();
                    foreach (var item in empTimeCardList)
                    {
                        var effdatecount = clientDbContext.E_PositionSalaryHistories.Where(x => x.E_PositionId == item.E_PositionId).Select(x => x.EffectiveDate).Count();
                        if (effdatecount > 0)
                        {
                            var effdate1 = clientDbContext.E_PositionSalaryHistories.Where(x => x.E_PositionId == item.E_PositionId).ToList();
                            if (effdate1.Count > 0)
                            {
                                var effdate2 = effdate1.Where(x => x.EndDate == null).ToList();
                                if (effdate2.Count > 0)
                                {
                                    effdate = effdate2.Select(x => x.EffectiveDate).FirstOrDefault();
                                }
                                else
                                {
                                    effdate = effdate1.Where(x => x.EndDate > DateTime.Now).Select(x => x.EffectiveDate).FirstOrDefault();
                                }
                            }
                        }
                        else
                        {
                            effdate = clientDbContext.E_PositionSalaryHistories.Where(x => x.E_PositionId == item.E_PositionId).Select(x => x.EffectiveDate).FirstOrDefault();
                        }
                        if (effdate != null)
                        {
                            if (effdate > DateTime.Now)
                            {
                                // Isvalid = true;
                                Session["Isvalid"] = true;
                                ModelState.AddModelError("", "Pay Rate Effective Date is not active for an approved day. An administrative review of the student’s position record is needed.");
                            }
                        }

                    }
                    bool isvalid = Convert.ToBoolean(Session["Isvalid"]);
                    if (!isvalid)
                    {
                        if (timeCardVm.PayPeriodId.HasValue && timeCardVm.EmployeeId != 0)
                        {
                            result = _lookupTablesRepository.TimeCardApproveAll(timeCardVm, User.Identity.Name, User.Identity.GetLoggedInUserRoleName()); ;
                        }
                    }
                }
                else
                {
                    var TimeCardListByPayPeriod = _lookupTablesRepository.TimeCardApproveAllList(timeCardVm, User.Identity.Name, User.Identity.GetLoggedInUserRoleName()).ToList();
                    // List<TimeCardCollectionVm> TimeCardListByPayPeriod = new List<TimeCardCollectionVm>();
                    // TimeCardListByPayPeriod = Query<TimeCardCollectionVm>("sp_GetTimeCardsList", new { @empId = timeCardVm.EmployeeId, @isArchived = IsArchived, @PayPeriodId = timeCardVm.PayPeriodId }).ToList();
                    if (TimeCardListByPayPeriod.Count != 0)
                    {
                        foreach (var item in TimeCardListByPayPeriod)
                        {
                            if (item.TimeOut == null && item.Hours == null)
                            {
                                ModelState.AddModelError("", "Please enter the Time Out before approving.");
                                return Json(result, JsonRequestBehavior.AllowGet);
                            }
                            var effdatecount = clientDbContext.E_PositionSalaryHistories.Where(x => x.E_PositionId == item.E_PositionId).Select(x => x.EffectiveDate).Count();
                            if (effdatecount > 0)
                            {
                                var effdate1 = clientDbContext.E_PositionSalaryHistories.Where(x => x.E_PositionId == item.E_PositionId).ToList();
                                if (effdate1.Count > 0)
                                {
                                    var effdate2 = effdate1.Where(x => x.EndDate == null).ToList();
                                    if (effdate2.Count > 0)
                                    {
                                        effdate = effdate2.Select(x => x.EffectiveDate).FirstOrDefault();
                                    }
                                    else
                                    {
                                        effdate = effdate1.Where(x => x.EndDate > DateTime.Now).Select(x => x.EffectiveDate).FirstOrDefault();
                                    }
                                }
                            }
                            else
                            {
                                effdate = clientDbContext.E_PositionSalaryHistories.Where(x => x.E_PositionId == item.E_PositionId).Select(x => x.EffectiveDate).FirstOrDefault();
                            }
                            if (effdate != null)
                            {
                                if (effdate > DateTime.Now)
                                {
                                    // Isvalid = true;
                                    Session["Isvalid"] = true;
                                    ModelState.AddModelError("", "Pay Rate Effective Date is not active for an approved day. An administrative review of the student’s position record is needed.");
                                }
                            }
                        }
                        bool isvalid = Convert.ToBoolean(Session["Isvalid"]);
                        if (!isvalid)
                        {
                            if (timeCardVm.PayPeriodId.HasValue && timeCardVm.EmployeeId != 0)
                            {
                                result = _lookupTablesRepository.TimeCardApproveAll(timeCardVm, User.Identity.Name, User.Identity.GetLoggedInUserRoleName()); ;
                            }
                        }
                    }
                    else
                    {
                        if (timeCardVm.PayPeriodId.HasValue && timeCardVm.EmployeeId != 0)
                        {
                            result = _lookupTablesRepository.TimeCardApproveAll(timeCardVm, User.Identity.Name, User.Identity.GetLoggedInUserRoleName()); ;
                        }
                    }
                }
            return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion


        public ActionResult SemisterList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var DdlSemisterList = clientDbContext.DdlSemisters.OrderBy(e => e.Description).ToList();

            return Json(DdlSemisterList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SemisterList_Create([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfClient.ddlSemisters semister)
        {
            if (semister != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var StatusInDb = clientDbContext.DdlSemisters
                    .Where(x => x.Code == semister.Code)
                    .SingleOrDefault();

                if (StatusInDb != null)
                {
                    ModelState.AddModelError("", "Semister" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                }
                else
                {
                    var newSemister = new ddlSemisters { Description = semister.Description, Code = semister.Code, Active = semister.Active };
                    clientDbContext.DdlSemisters.Add(newSemister);
                    clientDbContext.SaveChanges();
                    semister.SemisterID = newSemister.SemisterID;
                }
            }

            return Json(new[] { semister }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SemisterList_Update([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfClient.ddlSemisters semister)
        {
            if (semister != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);

                var semisterInDb = clientDbContext.DdlSemisters.Where(x => x.SemisterID != semister.SemisterID && (x.Code == semister.Code || x.Description == semister.Description)).SingleOrDefault();
                if (semisterInDb == null)
                {
                    semisterInDb = clientDbContext.DdlSemisters.Where(x => x.SemisterID == semister.SemisterID).SingleOrDefault();

                    if (semisterInDb.Active != semister.Active && semister.Active == false)
                    {
                        var semisterId = semister.SemisterID;
                        if (clientDbContext.Contracts.Where(x => x.SemisterId == semisterId).Count() == 0)
                        {
                            semisterInDb.Code = semister.Code;
                            semisterInDb.Description = semister.Description;
                            semisterInDb.Active = semister.Active;
                            clientDbContext.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                        }
                    }
                    else
                    {
                        semisterInDb.Code = semister.Code;
                        semisterInDb.Description = semister.Description;
                        semisterInDb.Active = semister.Active;
                        clientDbContext.SaveChanges();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Semister" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                }

            }

            return Json(new[] { semister }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SemisterList_Destroy([DataSourceRequest] DataSourceRequest request, ddlSemisters semister)
        {
            if (semister != null)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                ddlSemisters semisterInDb = clientDbContext.DdlSemisters.Where(x => x.SemisterID == semister.SemisterID).SingleOrDefault();

                if (semisterInDb != null)
                {
                    if (clientDbContext.Jobs.Where(x => x.workersCompensationID == semister.SemisterID).Count() > 0)
                    {
                        ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                    }
                    else
                    {
                        clientDbContext.DdlSemisters.Remove(semisterInDb);
                        try
                        {
                            clientDbContext.SaveChanges();
                        }
                        catch
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                        }
                    }

                }
            }

            return Json(new[] { semister }.ToDataSourceResult(request, ModelState));
        }

    }
}