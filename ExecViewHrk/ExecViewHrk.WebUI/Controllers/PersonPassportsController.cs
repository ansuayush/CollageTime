using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using ExecViewHrk.Models;
using ExecViewHrk.Domain.Interface;

namespace ExecViewHrk.WebUI.Controllers
{
    public class PersonPassportsController : Controller
    {
        IPersonPassportRepository _personpassportRepository;

        public PersonPassportsController(IPersonPassportRepository personpassportRepository)
        {
            _personpassportRepository = personpassportRepository;
        }
        // GET: PersonPassports
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PersonPassportList_Read([DataSourceRequest]DataSourceRequest request)
        {
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            string connString = User.Identity.GetClientConnectionString();
            string requestType = User.Identity.GetRequestType();

            if (requestType == "NSS" && User.IsInRole("ClientEmployees")) throw new Exception("Client Employee trying to access NSS.");

            if (requestType != "IsSelfService") SessionStateHelper.CheckForPersonSelectedValue();

            int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name)
                .Select(x => x.PersonId).SingleOrDefault() : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));

            var personPassportList = _personpassportRepository.GetPersonPassportList(personId);

            return Json(personPassportList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult PersonPassportsDelete(int personPassportId)
        {
            try
            {
                _personpassportRepository.DeletePersonPassport(personPassportId);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, succeed = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Message = "", succeed = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PersonPassportDetail(int personPassportId, int personId)
        {
            return View(GetPersonPassportRecord(personPassportId, personId));
        }
        public PersonsPassportVm GetPersonPassportRecord(int personPassportId, int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            PersonsPassportVm personPassportVm = new PersonsPassportVm();

            if (personPassportId != 0)
            {
                personPassportVm = clientDbContext.PersonPassports
               .Include(x => x.DdlCountry.Description)
               .Where(x => x.PersonPassportId == personPassportId)
               .Select(x => new PersonsPassportVm
               {
                   PersonPassportId = x.PersonPassportId,
                   PersonId = x.PersonId,
                   PersonName = x.Person.Lastname + ", " + x.Person.Firstname,
                   CountryId = x.CountryId,
                   CountryDescription = x.DdlCountry.Description,
                   ExpirationDate = x.ExpirationDate,
                   PassportNumber = x.PassportNumber,
                   PassportStorage = x.PassportStorage,
                   EnteredBy = x.EnteredBy,
                   IssueDate = x.IssueDate,
                   EnteredDate = x.EnteredDate,
               })
               .FirstOrDefault();
            }

            personPassportVm.PersonId = personId;
            personPassportVm.PersonPassportId = 0;
            personPassportVm.PersonName = clientDbContext.Persons.Where(x => x.PersonId == personId).Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();
            personPassportVm.CountryDropDownList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetCountryList());
            return personPassportVm;
        }
       
        public string GetCountryList()
        {
            var _list = _personpassportRepository.GetCountryList().CleanUp();

            return JsonConvert.SerializeObject(_list);
          }
      
        public PartialViewResult PersonPassportsMatrixPartial(bool isSelectedIndex = false, int personPassportIdParam = 0)
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

            PersonsPassportVm personPassportVm;

            if (isSelectedIndex && personPassportIdParam > 0)
            {
                personPassportVm = GetPersonPassportsRecord(personPassportIdParam);
            }
            else
            {
                int personPassportId = clientDbContext.PersonPassports.Where(x => x.PersonId == personId).Select(x => x.PersonPassportId).FirstOrDefault();
                if (personPassportId == 0)
                {
                    personPassportVm = new PersonsPassportVm();
                    personPassportVm.PersonName = clientDbContext.Persons
                        .Where(x => x.PersonId == personId)
                        .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                    personPassportVm.PersonId = personId;
                }
                else
                    personPassportVm = GetPersonPassportsRecord(personPassportId);
            }

            return PartialView(personPassportVm);
        }

        // the selected person has changed to a different person
        public ActionResult PersonIndexChangedPassportsAjax(int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            // this code is duplicated from get PersonPassport() action; drop down on top left of employment screen
            PersonsPassportVm personPassportVm;
            int personPassportId = clientDbContext.PersonPassports.Where(x => x.PersonId == personId).Select(x => x.PersonPassportId).FirstOrDefault();
            if (personPassportId == 0)
            {
                personPassportVm = new PersonsPassportVm();
                personPassportVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personPassportVm.PersonId = personId;
            }
            else
                personPassportVm = GetPersonPassportsRecord(personPassportId);

            SessionStateHelper.Set(SessionStateKeys.PERSON_SELECTED_ID, personId);

            if (ControllerContext.ParentActionViewContext != null
                && !ControllerContext.ParentActionViewContext.ViewData.ModelState.IsValid)
                ControllerContext.ParentActionViewContext.ViewData.ModelState.Clear();

            ModelState.Clear();
            return Json(personPassportVm, JsonRequestBehavior.AllowGet);
        }

        public PersonsPassportVm GetPersonPassportsRecord(int personPassportId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            PersonsPassportVm personPassportVm = clientDbContext.PersonPassports
                .Include(x => x.DdlCountry.Description)
                .Include(x => x.Person.Lastname).Include(x => x.Person.Firstname).Where(x => x.PersonPassportId == personPassportId)
                .Select(x => new PersonsPassportVm
                {
                    PersonPassportId = x.PersonPassportId,
                    PersonId = x.PersonId,
                    PersonName = x.Person.Lastname + ", " + x.Person.Firstname,                   
                    CountryId = x.CountryId,
                    CountryDescription = x.DdlCountry.Description,
                    PassportNumber = x.PassportNumber,
                    PassportStorage = x.PassportStorage,
                    IssueDate = x.IssueDate,
                    ExpirationDate = x.ExpirationDate,                                
                })
                .FirstOrDefault();

            return personPassportVm;
        }

        // the record for the same person has changed ; drop down on top right of Passport record
        public ActionResult PersonPassportsIndexChangedAjax(int personPassportIdDdl, int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            //// this code is duplicated from get PersonPassport() action
            PersonsPassportVm personPassportVm;

            if (personPassportIdDdl < 1)
            {
                personPassportVm = new PersonsPassportVm();
                personPassportVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personPassportVm.PersonId = personId;
                personPassportVm.PersonPassportId = 0;
            }
            else
                personPassportVm = GetPersonPassportsRecord(personPassportIdDdl);

            ModelState.Clear();

            return Json(personPassportVm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PersonPassportsSaveAjax(PersonsPassportVm personPassportVm)
        {

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            bool recordIsNew = false;
            if (requestType.ToLower() != "isselfservice" && personPassportVm.PersonPassportId == 0)
            {
                if (SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID) == null)
                {
                    ModelState.AddModelError("", "Cannot determine the id of the person the Passport record is for.");
                    return View(personPassportVm);
                }
                else
                {
                    personPassportVm.PersonId = Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));
                }
            }

            if (ModelState.IsValid)
            {
                PersonPassport personPassport = clientDbContext.PersonPassports.Include(x => x.Person)
                    .Where(x => x.PersonPassportId == personPassportVm.PersonPassportId).SingleOrDefault();

                if (personPassport == null)
                {
                    personPassport = new PersonPassport();
                    personPassport.EnteredBy = User.Identity.Name;
                    personPassport.EnteredDate = DateTime.UtcNow;
                    personPassport.PersonId = personPassportVm.PersonId;
                    recordIsNew = true;
                }
                else
                {
                    personPassport.ModifiedBy = User.Identity.Name;
                    personPassport.ModifiedDate = DateTime.UtcNow;
                }

                if (!string.IsNullOrEmpty(personPassportVm.CountryDescription))
                {
                    var countryInDb = clientDbContext.DdlCountries
                        .Where(x => x.Description == personPassportVm.CountryDescription).Select(x => x.CountryId).SingleOrDefault();

                    if (countryInDb <= 0)
                    {
                        ModelState.AddModelError("", "The country  field is required.");
                        return Json(new { succeed = false, Message = "The country  field is required." }, JsonRequestBehavior.AllowGet);
                    }
                    else personPassport.CountryId = countryInDb;
                }

                if (!string.IsNullOrEmpty(personPassportVm.PassportNumber))
                {
                    var passportNumberInDb = clientDbContext.PersonPassports.Where(n => (n.PassportNumber == personPassportVm.PassportNumber && n.PersonPassportId!=personPassportVm.PersonPassportId && n.CountryId == personPassportVm.CountryId)).ToList();

                    if (passportNumberInDb.Count > 0)
                    {
                        return Json(new { succeed = false, Message = "The entered Passport Number already exists for country." }, JsonRequestBehavior.AllowGet);
                    }
                }
                if ((personPassportVm.IssueDate !=null) && personPassportVm.CountryId !=0)
                 {
                   var issueDateInDb = clientDbContext.PersonPassports.Where(n => (n.IssueDate == personPassportVm.IssueDate && n.PersonPassportId != personPassportVm.PersonPassportId && n.CountryId == personPassportVm.CountryId)).ToList();

                    if (issueDateInDb.Count > 0)
                   {
                       return Json(new { succeed = false, Message = "The entered Issue Date already exists for country." }, JsonRequestBehavior.AllowGet);
                   }
                }
                if ((personPassportVm.ExpirationDate != null) && personPassportVm.CountryId != 0)
                {
                    var expirationDateInDb = clientDbContext.PersonPassports.Where(n => (n.ExpirationDate == personPassportVm.ExpirationDate && n.PersonPassportId != personPassportVm.PersonPassportId && n.CountryId == personPassportVm.CountryId)).ToList();

                    if (expirationDateInDb.Count > 0)
                    {
                        return Json(new { succeed = false, Message = "The entered Expiration Date already exists for country." }, JsonRequestBehavior.AllowGet);
                    }
                }


                personPassport.PassportNumber = personPassportVm.PassportNumber;
                personPassport.PassportStorage = personPassportVm.PassportStorage;
                personPassport.IssueDate = personPassportVm.IssueDate;
                personPassport.ExpirationDate = personPassportVm.ExpirationDate;
                personPassport.CountryId = personPassportVm.CountryId;

                if (recordIsNew)
                    clientDbContext.PersonPassports.Add(personPassport);
                try
                {
                    clientDbContext.SaveChanges();
                    ViewBag.AlertMessage = recordIsNew == true ? "New Person Passport Record Added" : "Person Passport Record Saved";
                    personPassportVm = GetPersonPassportsRecord(personPassport.PersonPassportId); // refresh the view model
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
                _message += "\n\nRecord could not be save at this time.";
                return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { personPassportVm, succeed = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PersonPassportsDeleteAjax(int personPassportIdDdl, int personId)
        {
            if (personPassportIdDdl < 1)
                return Json("The person Passport Record does not exist", JsonRequestBehavior.AllowGet);

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            clientDbContext.Database.ExecuteSqlCommand(" " +
                                "DELETE FROM PersonPassports " +
                                "WHERE PersonPassportId = @PersonPassportId ",
                                new SqlParameter("@PersonPassportId", personPassportIdDdl));

            //// this code is duplicated from get PersonPassport() action
            PersonsPassportVm personPassportVm;
            int personPassportId = clientDbContext.PersonPassports.Where(x => x.PersonId == personId).Select(x => x.PersonPassportId).FirstOrDefault();
            if (personPassportIdDdl < 1)
            {
                personPassportVm = new PersonsPassportVm();
                personPassportVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personPassportVm.PersonId = personId;
                personPassportVm.PersonPassportId = 0;
            }
            else
                personPassportVm = GetPersonPassportsRecord(personPassportId);

            ModelState.Clear();           
            return Json(personPassportVm, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPersonPassportsList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name).Select(x => x.PersonId).SingleOrDefault()
                : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));

            var personPassportList = clientDbContext.PersonPassports.Include(x => x.DdlCountry.Description)
               .Where(x => x.PersonId == personId)
                .Select(m => new
                {
                    PersonPassportId = m.PersonPassportId,
                    PersonPassportDescription = m.DdlCountry.Description
                }).OrderBy(x => x.PersonPassportDescription).ToList();

            return Json(personPassportList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PersonPassportDelete(int PersonPassportId)
        {
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            var dbRecord = clientDbContext.PersonPassports.Where(x => x.PersonPassportId == PersonPassportId).SingleOrDefault();
            if (dbRecord != null)
            {
                clientDbContext.PersonPassports.Remove(dbRecord);
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
