using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Infrastructure;
using ExecViewHrk.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace ExecViewHrk.WebUI.Controllers
{
    public class PersonWorkPermitsController : Controller
    {
        // GET: PersonWorkPermits
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult PersonWorkPermitsMatrixPartial(bool isSelectedIndex = false, int personWorkPermitIdParam = 0)
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

            PersonWorkPermitVm personWorkPermitVm;

            if (isSelectedIndex && personWorkPermitIdParam > 0)
            {
                personWorkPermitVm = GetPersonWorkPermitsRecord(personWorkPermitIdParam);
            }
            else
            {
                int personWorkPermitId = clientDbContext.PersonWorkPermits.Where(x => x.PersonId == personId).Select(x => x.PersonWorkPermitId).FirstOrDefault();
                if (personWorkPermitId == 0)
                {
                    personWorkPermitVm = new PersonWorkPermitVm();
                    personWorkPermitVm.PersonName = clientDbContext.Persons
                        .Where(x => x.PersonId == personId)
                        .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                    personWorkPermitVm.PersonId = personId;
                }
                else
                    personWorkPermitVm = GetPersonWorkPermitsRecord(personWorkPermitId);
            }


            return PartialView("PersonWorkPermitList", personWorkPermitVm);
        }

        // the selected person has changed to a different person
        public ActionResult PersonIndexChangedWorkPermitsAjax(int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            // this code is duplicated from get PersonWorkPermit() action; drop down on top left of employment screen
            PersonWorkPermitVm personWorkPermitVm;
            int personWorkPermitId = clientDbContext.PersonWorkPermits.Where(x => x.PersonId == personId).Select(x => x.PersonWorkPermitId).FirstOrDefault();
            if (personWorkPermitId == 0)
            {
                personWorkPermitVm = new PersonWorkPermitVm();
                personWorkPermitVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personWorkPermitVm.PersonId = personId;
            }
            else
                personWorkPermitVm = GetPersonWorkPermitsRecord(personWorkPermitId);

            SessionStateHelper.Set(SessionStateKeys.PERSON_SELECTED_ID, personId);

            if (ControllerContext.ParentActionViewContext != null
                && !ControllerContext.ParentActionViewContext.ViewData.ModelState.IsValid)
                ControllerContext.ParentActionViewContext.ViewData.ModelState.Clear();

            ModelState.Clear();
            return Json(personWorkPermitVm, JsonRequestBehavior.AllowGet);
        }

        public PersonWorkPermitVm GetPersonWorkPermitsRecord(int personWorkPermitId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            PersonWorkPermitVm personWorkPermitVm = clientDbContext.PersonWorkPermits
                .Include(x => x.DdlCountry.Description)
                .Include(x => x.Person.Lastname).Include(x => x.Person.Firstname).Where(x => x.PersonWorkPermitId == personWorkPermitId)
                .Select(x => new PersonWorkPermitVm
                {
                    PersonWorkPermitId = x.PersonWorkPermitId,
                    PersonId = x.PersonId,
                    PersonName = x.Person.Lastname + ", " + x.Person.Firstname,
                    CountryId = x.CountryId,
                    CountryDescription = x.DdlCountry.Description,
                    WorkPermitNumber = x.WorkPermitNumber,
                    WorkPermitType = x.WorkPermitType,
                    IssuingAuthority = x.IssuingAuthority,
                    IssueDate = x.IssueDate,
                    ExpirationDate = x.ExpirationDate,
                    ExtensionDate = x.ExtensionDate,
                    EnteredBy = x.EnteredBy,
                    EnteredDate = x.EnteredDate,
                    Notes = x.Notes,
                })
                .FirstOrDefault();

            return personWorkPermitVm;
        }

        // the record for the same person has changed ; drop down on top right of WorkPermit record
        public ActionResult PersonWorkPermitsIndexChangedAjax(int personWorkPermitIdDdl, int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            //// this code is duplicated from get PersonWorkPermit() action
            PersonWorkPermitVm personWorkPermitVm;

            if (personWorkPermitIdDdl < 1)
            {
                personWorkPermitVm = new PersonWorkPermitVm();
                personWorkPermitVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personWorkPermitVm.PersonId = personId;
                personWorkPermitVm.PersonWorkPermitId = 0;
            }
            else
                personWorkPermitVm = GetPersonWorkPermitsRecord(personWorkPermitIdDdl);

            ModelState.Clear();

            return Json(personWorkPermitVm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PersonWorkPermitsSaveAjax(PersonWorkPermitVm personWorkPermitVm)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            bool recordIsNew = false;
            if (requestType.ToLower() != "isselfservice" && personWorkPermitVm.PersonWorkPermitId == 0)
            {
                if (SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID) == null)
                {
                    ModelState.AddModelError("", "Cannot determine the id of the person the WorkPermit record is for.");
                    return View(personWorkPermitVm);
                }
                else
                {
                    personWorkPermitVm.PersonId = Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));
                }
            }

            if (ModelState.IsValid)
            {
                PersonWorkPermit personWorkPermit = clientDbContext.PersonWorkPermits.Include(x => x.Person)
                    .Where(x => x.PersonWorkPermitId == personWorkPermitVm.PersonWorkPermitId).SingleOrDefault();

                if (personWorkPermit == null)
                {
                    personWorkPermit = new PersonWorkPermit();
                    personWorkPermit.EnteredBy = User.Identity.Name;
                    personWorkPermit.EnteredDate = DateTime.UtcNow;
                    personWorkPermit.PersonId = personWorkPermitVm.PersonId;
                    recordIsNew = true;
                }              
                personWorkPermit.CountryId = personWorkPermitVm.CountryId;
                personWorkPermit.Notes = personWorkPermitVm.Notes;
                personWorkPermit.WorkPermitNumber = personWorkPermitVm.WorkPermitNumber;
                personWorkPermit.WorkPermitType = personWorkPermitVm.WorkPermitType;
                personWorkPermit.IssuingAuthority = personWorkPermitVm.IssuingAuthority;
                personWorkPermit.IssueDate = personWorkPermitVm.IssueDate;
                personWorkPermit.ExpirationDate = personWorkPermitVm.ExpirationDate;
                personWorkPermit.ExtensionDate = personWorkPermitVm.ExtensionDate;

                if (recordIsNew)
                    clientDbContext.PersonWorkPermits.Add(personWorkPermit);


                try
                {
                    clientDbContext.SaveChanges();
                    ViewBag.AlertMessage = recordIsNew == true ? "New Person Work Permit Record Added" : "Person Work Permit Record Saved";
                    personWorkPermitVm = GetPersonWorkPermitsRecord(personWorkPermit.PersonWorkPermitId); // refresh the view model
                }
                catch (Exception err)
                {
                    ViewBag.AlertMessage = "";
                    string _message = "";
                    IEnumerable<DbEntityValidationResult> errors = clientDbContext.GetValidationErrors();
                    if (errors.Count() == 0)
                    {
                        _message += err.InnerException.InnerException.Message;
                    }
                    else
                    {
                        foreach (DbEntityValidationResult error in errors)
                        {
                            foreach (var valError in error.ValidationErrors)
                            {
                                if (_message != "") _message += "\n";
                                _message += valError.ErrorMessage;
                            }
                        }
                    }
                    return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
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

            return Json(new { personWorkPermitVm, succeed = true }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult PersonWorkPermitsDeleteAjax(int personWorkPermitIdDdl)
        {          
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            var dbRecord = clientDbContext.PersonWorkPermits.Where(x => x.PersonWorkPermitId == personWorkPermitIdDdl).SingleOrDefault();
            if (dbRecord != null)
            {
                clientDbContext.PersonWorkPermits.Remove(dbRecord);
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

        public JsonResult GetPersonWorkPermitsList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name).Select(x => x.PersonId).SingleOrDefault()
                : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));
            var list = clientDbContext.PersonWorkPermits.Include(x => x.DdlCountry)
                .Where(x => x.PersonId == personId).ToList();

            var personWorkPermitList = clientDbContext.PersonWorkPermits.Include(x => x.DdlCountry)
                .Where(x => x.PersonId == personId)
                .Select(m => new
                {
                    PersonWorkPermitId = m.PersonWorkPermitId,
                    PersonWorkPermitCountryDescription = m.DdlCountry.Description
                });//.OrderBy(x => x.PersonWorkPermitDescription).ToList();

            return Json(personWorkPermitList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PersonWorkPermits_Read([DataSourceRequest]DataSourceRequest request)
        {
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            string connString = User.Identity.GetClientConnectionString();
            string requestType = User.Identity.GetRequestType();
            if (requestType == "NSS" && User.IsInRole("ClientEmployees")) throw new Exception("Client Employee trying to access NSS.");
            if (requestType != "IsSelfService") SessionStateHelper.CheckForPersonSelectedValue();
            int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name).Select(x => x.PersonId).SingleOrDefault()
               : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));
            var personWorkPermitList = clientDbContext.PersonWorkPermits.Include(x => x.DdlCountry.Description)
                .Where(x => x.PersonId == personId)
                .Select(m => new
                {
                    PersonWorkPermitId = m.PersonWorkPermitId,
                    PersonId = m.PersonId,
                    PersonName = m.Person.Lastname + ", " + m.Person.Firstname,
                    CountryId = m.CountryId,
                    CountryDescription = m.DdlCountry.Description,
                    WorkPermitNumber = m.WorkPermitNumber,
                    WorkPermitType = m.WorkPermitType,
                    IssuingAuthority = m.IssuingAuthority,
                    IssueDate = m.IssueDate,
                    ExpirationDate = m.ExpirationDate,
                    ExtensionDate = m.ExtensionDate,
                    Notes = m.Notes,
                }).ToList();
            return Json(personWorkPermitList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult PersonWorkPermitDetails(int personWorkPermitId, int personId)
        {
            return View(GetPersonWorkPermitDetail(personWorkPermitId, personId));
        }
        public PersonWorkPermitVm GetPersonWorkPermitDetail(int personWorkPermitId, int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            PersonWorkPermitVm personWorkPermitVm = new PersonWorkPermitVm();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            if (personWorkPermitId == 0)
            {
                personWorkPermitVm.PersonWorkPermitId = 0;
            }
            else
            {
                personWorkPermitVm = clientDbContext.PersonWorkPermits
                .Include(x => x.DdlCountry.Description)
                .Where(x => x.PersonWorkPermitId == personWorkPermitId)
                //.Include(x => x.Person.Lastname).Include(x => x.Person.Firstname).Where(x => x.PersonWorkPermitId == personWorkPermitId)
                .Select(x => new PersonWorkPermitVm
                {
                    PersonWorkPermitId = x.PersonWorkPermitId,
                    PersonId = x.PersonId,
                    PersonName = x.Person.Lastname + ", " + x.Person.Firstname,
                    CountryId = x.CountryId,
                    CountryDescription = x.DdlCountry.Description,
                    WorkPermitNumber = x.WorkPermitNumber,
                    WorkPermitType = x.WorkPermitType,
                    IssuingAuthority = x.IssuingAuthority,
                    IssueDate = x.IssueDate,
                    ExpirationDate = x.ExpirationDate,
                    ExtensionDate = x.ExtensionDate,
                    EnteredBy = x.EnteredBy,
                    EnteredDate = x.EnteredDate,
                    Notes = x.Notes,
                })
                .FirstOrDefault();
            }
            personWorkPermitVm.PersonId = personId;
            personWorkPermitVm.PersonWorkPermitId = 0;
            personWorkPermitVm.PersonName = clientDbContext.Persons.Where(x => x.PersonId == personId).Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();
            personWorkPermitVm.GetCountries = clientDbContext.DdlCountries.Where(x => x.Active == true).ToList();
            personWorkPermitVm.GetCountries.Insert(0, new DdlCountry { CountryId = 0, Description = "Select" });
            return personWorkPermitVm;
        }       
    }
}