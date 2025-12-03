using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using ExecViewHrk.WebUI.Helpers;
using Newtonsoft.Json;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using ExecViewHrk.Models;
using ExecViewHrk.Domain.Interface;

namespace ExecViewHrk.WebUI.Controllers
{
    public class PersonLicensesController : Controller
    {
        IPersonLicenseRepository _personlicenseRepository;
        public PersonLicensesController(IPersonLicenseRepository personlicenseRepository)
        {
            _personlicenseRepository = personlicenseRepository;
        }
        // GET: PersonLicenses
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PersonLicenseList_Read([DataSourceRequest]DataSourceRequest request)
        {
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            string connString = User.Identity.GetClientConnectionString();
            string requestType = User.Identity.GetRequestType();

            if (requestType == "NSS" && User.IsInRole("ClientEmployees")) throw new Exception("Client Employee trying to access NSS.");

            if (requestType != "IsSelfService") SessionStateHelper.CheckForPersonSelectedValue();

            int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name)
                .Select(x => x.PersonId).SingleOrDefault() : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));
            var personLicenseList = _personlicenseRepository.GetPersonLicenseList(personId);
            return Json(personLicenseList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }
        public ActionResult PersonLicensesDelete(int personLicenseId)
        {
            try
            {
                _personlicenseRepository.DeletePersonLicense(personLicenseId);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, succeed = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Message = "", succeed = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PersonLicenseDetail(int personLicenseId, int personId)
        {
            return View(GetPersonLicenseRecord(personLicenseId, personId));
        }
        public PersonLicensVm GetPersonLicenseRecord(int personLicenseId, int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            PersonLicensVm personLicenseVm = new PersonLicensVm();

            if (personLicenseId != 0)
            {
                personLicenseVm = clientDbContext.PersonLicenses
               .Include(x => x.DdlLicenseType.Description)
               .Where(x => x.PersonLicenseId == personLicenseId)
               .Select(x => new PersonLicensVm
               {
                   PersonLicenseId = x.PersonLicenseId,
                   PersonId = x.PersonId,
                   PersonName = x.Person.Lastname + ", " + x.Person.Firstname,
                   LicenseTypeId = x.LicenseTypeId,
                   LicenseDescription = x.DdlLicenseType.Description,
                   StateId = x.StateId,
                   CountryId = x.CountryId,
                   ExpirationDate = x.ExpirationDate,
                   RevokedDate = x.RevokedDate,
                   ReinstatedDate = x.ReinstatedDate,
                   LicenseNumber = x.LicenseNumber,
                   Notes = x.Notes,
               })
               .FirstOrDefault();
            }

            personLicenseVm.PersonId = personId;
            personLicenseVm.PersonLicenseId = 0;
            personLicenseVm.PersonName = clientDbContext.Persons.Where(x => x.PersonId == personId).Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();
            personLicenseVm.LicenseDropDownList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetPersonLicenseList());
            personLicenseVm.StateDropDownList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetStateList());
            personLicenseVm.CountryDropDownList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetCountryList());
            return personLicenseVm;
        }
        public string GetPersonLicenseList()
        {
            var _list = _personlicenseRepository.GetPersonLicenseList().CleanUp();

            return JsonConvert.SerializeObject(_list);
        }
        public string GetStateList()
        {
            var _list = _personlicenseRepository.GetStateList().CleanUp();
            return JsonConvert.SerializeObject(_list);
        }
        public string GetCountryList()
        {

            var _list = _personlicenseRepository.GetCountryList().CleanUp();
            return JsonConvert.SerializeObject(_list);
        }
        public JsonResult GetStatesByCountry(string id)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);


            var StateList = clientDbContext.DdlStates
                .Select(m => new DropDownModel { keyvalue = m.StateId.ToString(), keydescription = m.Title })
                .Where(m => m.keyvalue == id)
                .OrderBy(x => x.keydescription)
                .ToList()
                .CleanUp();

            return Json(new SelectList(StateList, "keyvalue", "keydescription"));
        }


        public PartialViewResult PersonLicensesMatrixPartial(bool isSelectedIndex = false, int personLicenseIdParam = 0)
        {
            string requestType = User.Identity.GetRequestType();

            if (requestType == "NSS" && User.IsInRole("ClientEmployees"))
                throw new Exception("Client Employee trying to access NSS.");


            if (requestType != "IsSelfService")
                SessionStateHelper.CheckForPersonSelectedValue();

            string connString = User.Identity.GetClientConnectionString();

            ClientDbContext clientDbContext = new ClientDbContext(connString);

            int personId = requestType == "IsSelfService" ? clientDbContext.Persons
                    .Where(x => x.eMail == User.Identity.Name)
                    .Select(x => x.PersonId).SingleOrDefault()
                : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));

            if ((personId == 0) && (requestType == "IsSelfService"))
                throw new Exception("Self service person id is 0 - Personal record cannot be displayed. It doesn't exist.");

            PersonLicensVm personLicenseVm;

            if (isSelectedIndex && personLicenseIdParam > 0)
            {
                personLicenseVm = GetPersonLicensesRecord(personLicenseIdParam);
            }
            else
            {
                int personLicenseId = clientDbContext.PersonLicenses.Where(x => x.PersonId == personId).Select(x => x.PersonLicenseId).FirstOrDefault();
                if (personLicenseId == 0)
                {
                    personLicenseVm = new PersonLicensVm();
                    personLicenseVm.PersonName = clientDbContext.Persons
                        .Where(x => x.PersonId == personId)
                        .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                    personLicenseVm.PersonId = personId;
                }
                else
                    personLicenseVm = GetPersonLicensesRecord(personLicenseId);
            }


            return PartialView(personLicenseVm);
        }

        // the selected person has changed to a different person
        public ActionResult PersonIndexChangedLicensesAjax(int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            // this code is duplicated from get PersonAda() action; drop down on top left of employment screen
            PersonLicensVm personLicenseVm;
            int personLicenseId = clientDbContext.PersonLicenses.Where(x => x.PersonId == personId).Select(x => x.PersonLicenseId).FirstOrDefault();
            if (personLicenseId == 0)
            {
                personLicenseVm = new PersonLicensVm();
                personLicenseVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personLicenseVm.PersonId = personId;
            }
            else
                personLicenseVm = GetPersonLicensesRecord(personLicenseId);

            SessionStateHelper.Set(SessionStateKeys.PERSON_SELECTED_ID, personId);

            if (ControllerContext.ParentActionViewContext != null
                && !ControllerContext.ParentActionViewContext.ViewData.ModelState.IsValid)
                ControllerContext.ParentActionViewContext.ViewData.ModelState.Clear();

            ModelState.Clear();
            return Json(personLicenseVm, JsonRequestBehavior.AllowGet);
        }

        public PersonLicensVm GetPersonLicensesRecord(int personLicenseId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            PersonLicensVm personLicenseVm = clientDbContext.PersonLicenses
                .Include(x => x.DdlLicenseType.Description)
                .Include(x => x.DdlState.Title)
                .Include(x => x.DdlCountry.Description)
                .Include(x => x.Person.Lastname).Include(x => x.Person.Firstname).Where(x => x.PersonLicenseId == personLicenseId)
                .Select(x => new PersonLicensVm
                {
                    PersonLicenseId = x.PersonLicenseId,
                    PersonId = x.PersonId,
                    PersonName = x.Person.Lastname + ", " + x.Person.Firstname,
                    LicenseTypeId = x.LicenseTypeId,
                    LicenseDescription = x.DdlLicenseType.Description,
                    LicenseNumber = x.LicenseNumber,
                    StateId = x.StateId,
                    StateTitle = x.DdlState.Title,
                    CountryId = x.DdlCountry.CountryId,
                    CountryDescription = x.DdlCountry.Description,
                    ExpirationDate = x.ExpirationDate,
                    RevokedDate = x.RevokedDate,
                    ReinstatedDate = x.ReinstatedDate,
                    Notes = x.Notes,
                })
                .FirstOrDefault();

            return personLicenseVm;
        }

        // the record for the same person has changed ; drop down on top right of ADA record
        public ActionResult PersonLicensesIndexChangedAjax(int personLicenseIdDdl, int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            //// this code is duplicated from get PersonAda() action
            PersonLicensVm personLicenseVm;

            if (personLicenseIdDdl < 1)
            {
                personLicenseVm = new PersonLicensVm();
                personLicenseVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personLicenseVm.PersonId = personId;
                personLicenseVm.PersonLicenseId = 0;
            }
            else
                personLicenseVm = GetPersonLicensesRecord(personLicenseIdDdl);

            ModelState.Clear();

            return Json(personLicenseVm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PersonLicensesSaveAjax(PersonLicensVm personLicenseVm)
        {

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            bool recordIsNew = false;
            if (requestType.ToLower() != "isselfservice" && personLicenseVm.PersonLicenseId == 0)
            {
                if (SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID) == null)
                {
                    ModelState.AddModelError("", "Cannot determine the id of the person the License record is for.");
                    return View(personLicenseVm);
                }
                else
                {
                    personLicenseVm.PersonId = Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));
                }
            }

            if (ModelState.IsValid)
            {
                PersonLicens personLicense = clientDbContext.PersonLicenses.Include(x => x.Person).Where(x => x.PersonLicenseId == personLicenseVm.PersonLicenseId).SingleOrDefault();
                
                if (personLicense == null)
                {
                    personLicense = new PersonLicens();  
                    personLicense.EnteredBy = User.Identity.Name;
                    personLicense.EnteredDate = DateTime.UtcNow;
                    personLicense.PersonId = personLicenseVm.PersonId;
                    recordIsNew = true;
                }
                else
                {
                    personLicense.ModifiedBy = User.Identity.Name;
                    personLicense.ModifiedDate = DateTime.UtcNow;
                }


                if (!string.IsNullOrEmpty(personLicenseVm.LicenseDescription))
                {
                    byte licenseInDb = clientDbContext.DdlLicenseTypes.Where(x => x.Description == personLicenseVm.LicenseDescription).Select(x => x.LicenseTypeId).SingleOrDefault();
                    if (licenseInDb <= 0)
                    {
                        return Json(new { succeed = false, Message = "The license type does not exist." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        personLicense.LicenseTypeId = licenseInDb;
                    }
                }

                if (recordIsNew)
                {
                    if (clientDbContext.PersonLicenses.Count(x => x.LicenseTypeId == personLicense.LicenseTypeId && x.PersonId == personLicenseVm.PersonId) > 0)
                    {
                        return Json(new { succeed = false, Message = string.Format(CustomErrorMessages.ERROR_DUPLICATE_RECORD, "License Type") }, JsonRequestBehavior.AllowGet);
                    }
                }
                else {
                    if (clientDbContext.PersonLicenses.Count(x => x.LicenseTypeId == personLicense.LicenseTypeId && x.PersonId == personLicenseVm.PersonId && x.PersonLicenseId != personLicenseVm.PersonLicenseId) > 0)
                    {
                        return Json(new { succeed = false, Message = string.Format(CustomErrorMessages.ERROR_DUPLICATE_RECORD, "License Type") }, JsonRequestBehavior.AllowGet);
                    }
                }


                if (!string.IsNullOrEmpty(personLicenseVm.StateTitle))
                {
                    int stateInDb = clientDbContext.DdlStates.Where(x => x.Title == personLicenseVm.StateTitle).Select(x => x.StateId).SingleOrDefault();

                    if (stateInDb <= 0)
                    {
                        return Json(new { succeed = false, Message = "The state does not exist." }, JsonRequestBehavior.AllowGet);
                    }
                    else personLicense.StateId = stateInDb;
                }

                if (!string.IsNullOrEmpty(personLicenseVm.CountryDescription))
                {
                    var countryInDb = clientDbContext.DdlCountries
                        .Where(x => x.Description == personLicenseVm.CountryDescription).Select(x => x.CountryId).SingleOrDefault();

                    if (countryInDb <= 0)
                    {
                        ModelState.AddModelError("", "The country does not exist.");
                        return Json(new { succeed = false, Message = "The country does not exist." }, JsonRequestBehavior.AllowGet);
                    }
                    else personLicense.CountryId = countryInDb;
                }
                if (!string.IsNullOrEmpty(personLicenseVm.LicenseNumber))
                {
                    var LicenseNumberInDb = clientDbContext.PersonLicenses.Where(n => n.LicenseNumber == personLicenseVm.LicenseNumber && n.PersonLicenseId != personLicenseVm.PersonLicenseId).ToList();

                    if (LicenseNumberInDb.Count > 0)
                    {
                        return Json(new { succeed = false, Message = "The entered License Number already exists!" }, JsonRequestBehavior.AllowGet);
                    }
                }

                personLicense.Notes = personLicenseVm.Notes;
                personLicense.LicenseNumber = personLicenseVm.LicenseNumber;
                personLicense.ExpirationDate = personLicenseVm.ExpirationDate;
                personLicense.RevokedDate = personLicenseVm.RevokedDate;
                personLicense.ReinstatedDate = personLicenseVm.ReinstatedDate;
                personLicense.LicenseTypeId = personLicenseVm.LicenseTypeId;
                personLicense.StateId = personLicenseVm.StateId;
                personLicense.CountryId = personLicenseVm.CountryId;


                if (recordIsNew)
                    clientDbContext.PersonLicenses.Add(personLicense);

                try
                {
                    clientDbContext.SaveChanges();
                    ViewBag.AlertMessage = recordIsNew == true ? "New Person License Record Added" : "Person License Record Updated";
                    personLicenseVm = GetPersonLicensesRecord(personLicense.PersonLicenseId);
                }
                catch (Exception err)
                {
                    ViewBag.AlertMessage = "";

                    IEnumerable<DbEntityValidationResult> errors = clientDbContext.GetValidationErrors();
                    if (errors.Count() == 0)
                        ModelState.AddModelError("", err.InnerException.Message);
                    else
                    {
                        foreach (DbEntityValidationResult error in errors)
                        {
                            foreach (var valError in error.ValidationErrors)
                            {
                                ModelState.AddModelError("", valError.ErrorMessage);
                            }
                        }
                    }
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

            return Json(new { personLicenseVm, succeed = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PersonLicensesDeleteAjax(int personLicenseIdDdl, int personId)
        {
            if (personLicenseIdDdl < 1)
                return Json("The person License Record does not exist", JsonRequestBehavior.AllowGet);

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            clientDbContext.Database.ExecuteSqlCommand(" " +
                                "DELETE FROM PersonLicenses " +
                                "WHERE PersonLicenseId = @PersonLicenseId ",
                                new SqlParameter("@PersonLicenseId", personLicenseIdDdl));

            //// this code is duplicated from get PersonAda() action
            PersonLicensVm personLicenseVm;
            int personLicenseId = clientDbContext.PersonLicenses.Where(x => x.PersonId == personId).Select(x => x.PersonLicenseId).FirstOrDefault();
            if (personLicenseId < 1)
            {
                personLicenseVm = new PersonLicensVm();
                personLicenseVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personLicenseVm.PersonId = personId;
                personLicenseVm.PersonLicenseId = 0;
            }
            else
                personLicenseVm = GetPersonLicensesRecord(personLicenseId);

            ModelState.Clear();
            return Json(personLicenseVm, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPersonLicensesList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name).Select(x => x.PersonId).SingleOrDefault()
                : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));

            var personLicenseList = clientDbContext.PersonLicenses.Include(x => x.DdlLicenseType.Description)
                .Where(x => x.PersonId == personId)
                .Select(m => new
                {
                    PersonLicenseId = m.PersonLicenseId,
                    PersonLicenseDescription = m.DdlLicenseType.Description
                }).OrderBy(x => x.PersonLicenseDescription).ToList();

            return Json(personLicenseList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PersonLicenseDelete(int PersonLicenseId)
        {

            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            var dbRecord = clientDbContext.PersonLicenses.Where(x => x.PersonLicenseId == PersonLicenseId).SingleOrDefault();
            if (dbRecord != null)
            {
                clientDbContext.PersonLicenses.Remove(dbRecord);
                try
                {
                    clientDbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(new { Message = ex.Message, succeed = false }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Message = "Record does not exist.", succeed = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Message = "", succeed = true }, JsonRequestBehavior.AllowGet);

        }
    }
}

    
