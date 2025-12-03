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

namespace ExecViewHrk.WebUI.Controllers
{
    public class PersonAdditionalsController : Controller
    {
        // GET: PersonAdditionals
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult PersonAdditionalsMatrixPartial(bool isSelectedIndex = false, int personAdditionalIdParam = 0)
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

            PersonAdditionalVm personAdditionalVm;

            if (isSelectedIndex && personAdditionalIdParam > 0)
            {
                personAdditionalVm = GetPersonAdditionalsRecord(personAdditionalIdParam);
            }
            else
            {
                int personAdditionalId = clientDbContext.PersonAdditionals.Where(x => x.PersonId == personId).Select(x => x.PersonAdditionalId).FirstOrDefault();
                if (personAdditionalId == 0)
                {
                    personAdditionalVm = new PersonAdditionalVm();
                    personAdditionalVm.PersonName = clientDbContext.Persons
                        .Where(x => x.PersonId == personId)
                        .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                    personAdditionalVm.PersonId = personId;
                }
                else
                    personAdditionalVm = GetPersonAdditionalsRecord(personAdditionalId);
            }


            return PartialView(personAdditionalVm);
        }

        // the selected person has changed to a different person
        public ActionResult PersonIndexChangedAdditionalsAjax(int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            // this code is duplicated from get PersonAda() action; drop down on top left of employment screen
            PersonAdditionalVm personAdditionalVm;
            int personAdditionalId = clientDbContext.PersonAdditionals.Where(x => x.PersonId == personId).Select(x => x.PersonAdditionalId).FirstOrDefault();
            if (personAdditionalId == 0)
            {
                personAdditionalVm = new PersonAdditionalVm();
                personAdditionalVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personAdditionalVm.PersonId = personId;
            }
            else
                personAdditionalVm = GetPersonAdditionalsRecord(personAdditionalId);

            SessionStateHelper.Set(SessionStateKeys.PERSON_SELECTED_ID, personId);

            if (ControllerContext.ParentActionViewContext != null
                && !ControllerContext.ParentActionViewContext.ViewData.ModelState.IsValid)
                ControllerContext.ParentActionViewContext.ViewData.ModelState.Clear();

            ModelState.Clear();
            return Json(personAdditionalVm, JsonRequestBehavior.AllowGet);
        }

        public PersonAdditionalVm GetPersonAdditionalsRecord(int personAdditionalId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            PersonAdditionalVm personAdditionalVm = clientDbContext.PersonAdditionals
                .Include(x => x.DdlEeoType)
                .Include(x => x.DdlApplicantSource.Description)
                .Include(x => x.DdlHospital.Description)
                .Include(x => x.DdlCitizenship.Description)
                .Include(x => x.Person.Lastname).Include(x => x.Person.Firstname).Where(x => x.PersonAdditionalId == personAdditionalId)
                .Select(x => new PersonAdditionalVm
                {
                    PersonAdditionalId = x.PersonAdditionalId,
                    PersonId = x.PersonId,
                    PersonName = x.Person.Lastname + ", " + x.Person.Firstname,
                    EeoTypeId = x.EeoTypeId,
                    EeoDescription = x.DdlEeoType.Description,
                    ApplicantSourceId = x.ApplicantSourceId,
                    ApplicantDescription = x.DdlApplicantSource.Description,
                    HospitalId = x.HospitalId,
                    HospitalDescription = x.DdlHospital.Description,
                    CitizenshipId = x.CitizenshipId,
                    CitizenshipDescription = x.DdlCitizenship.Description,
                    BirthPlace = x.BirthPlace,
                    CitizenshipDate = x.CitizenshipDate,
                    Veteran = x.Veteran,
                    Vietnam = x.Vietnam.Value ?x.Vietnam.Value :false,
                    Other = x.Other.Value?x.Other.Value:false,
                    SpecialDisabled=x.SpecialDisabled.Value?x.SpecialDisabled.Value:false,
                    Disabled = x.Disabled,
                    DisabledComments = x.DisabledComments,
                    Doctor = x.Doctor,
                    BloodDonor = x.BloodDonor,
                    Smoker = x.Smoker,
                    AdvancedDirective = x.AdvancedDirective,
                    EarlyRetirementDate = x.EarlyRetirementDate,
                    NormalRetirementDate = x.NormalRetirementDate,
                    ExpectedRetirementDate = x.ExpectedRetirementDate,
                    DateOfDeath = x.DateOfDeath,
                    EnteredBy = x.EnteredBy,
                    EnteredDate = x.EnteredDate,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedDate = x.ModifiedDate,
                    Notes = x.Notes,
                })
                .FirstOrDefault();

            return personAdditionalVm;
        }

        // the record for the same person has changed ; drop down on top right of ADA record
        public ActionResult PersonAdditionalsIndexChangedAjax(int personAdditionalIdDdl, int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            //// this code is duplicated from get PersonAda() action
            PersonAdditionalVm personAdditionalVm;

            if (personAdditionalIdDdl < 1)
            {
                personAdditionalVm = new PersonAdditionalVm();
                personAdditionalVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personAdditionalVm.PersonId = personId;
                personAdditionalVm.PersonAdditionalId = 0;
            }
            else
                personAdditionalVm = GetPersonAdditionalsRecord(personAdditionalIdDdl);

            ModelState.Clear();

            return Json(personAdditionalVm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PersonAdditionalsSaveAjax(PersonAdditionalVm personAdditionalVm)
        {

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            bool recordIsNew = false;
            if (requestType.ToLower() != "isselfservice" && personAdditionalVm.PersonAdditionalId == 0)
            {
                if (SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID) == null)
                {
                    ModelState.AddModelError("", "Cannot determine the id of the person the Additional record is for.");
                    return View(personAdditionalVm);
                }
                else
                {
                    personAdditionalVm.PersonId = Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));
                }
            }

            PersonAdditional personAdditional = clientDbContext.PersonAdditionals.Include(x => x.Person)
                .Where(x => x.PersonAdditionalId == personAdditionalVm.PersonAdditionalId).SingleOrDefault();

            if (personAdditional == null)
            {
                personAdditional = new PersonAdditional();
                personAdditional.EnteredBy = User.Identity.Name;
                personAdditional.EnteredDate = DateTime.UtcNow;
                personAdditional.PersonId = personAdditionalVm.PersonId;
                recordIsNew = true;
            }
            else
            {
                personAdditional.ModifiedBy = User.Identity.Name;
                personAdditional.ModifiedDate = DateTime.UtcNow;
            }

            int eeoTypeId = 0;
            if (!int.TryParse(personAdditionalVm.EeoDescription, out eeoTypeId))
            {
                var eeoTypeInDb = clientDbContext.DdlEeoTypes
                    .Where(x => x.Description == personAdditionalVm.EeoDescription).SingleOrDefault();

                if (eeoTypeInDb == null)
                {
                    ModelState.AddModelError("", "The Eeo type does not exist.");
                    return View(personAdditionalVm);
                }
                else eeoTypeId = eeoTypeInDb.EeoTypeId;
            }
            personAdditional.EeoTypeId = eeoTypeId;

            int applicantSourceId = 0;
            if (!int.TryParse(personAdditionalVm.ApplicantDescription, out applicantSourceId))
            {
                var applicantSourceInDb = clientDbContext.DdlApplicantSources
                    .Where(x => x.Description == personAdditionalVm.ApplicantDescription).SingleOrDefault();

                if (applicantSourceInDb == null)
                {
                    ModelState.AddModelError("", "The Applicant source does not exist.");
                    return View(personAdditionalVm);
                }
                else applicantSourceId = applicantSourceInDb.ApplicantSourceId;
            }
            personAdditional.ApplicantSourceId = applicantSourceId;

            int citizenshipId = 0;
            if (!int.TryParse(personAdditionalVm.CitizenshipDescription, out citizenshipId))
            {
                var citizenshipInDb = clientDbContext.DdlCitizenships
                    .Where(x => x.Description == personAdditionalVm.CitizenshipDescription).SingleOrDefault();

                if (citizenshipInDb == null)
                {
                    ModelState.AddModelError("", "The Citizenship does not exist.");
                    return View(personAdditionalVm);
                }
                else citizenshipId = citizenshipInDb.CitizenshipId;
            }
            personAdditional.CitizenshipId = citizenshipId;

            int hospitalId = 0;
            if (!int.TryParse(personAdditionalVm.HospitalDescription, out hospitalId))
            {
                var hospitalInDb = clientDbContext.DdlHospitals
                    .Where(x => x.Description == personAdditionalVm.HospitalDescription).SingleOrDefault();

                if (hospitalInDb == null)
                {
                    ModelState.AddModelError("", "The Hospital does not exist.");
                    return View(personAdditionalVm);
                }
                else hospitalId = hospitalInDb.HospitalId;
            }
            personAdditional.HospitalId = hospitalId;


            personAdditional.Notes = personAdditionalVm.Notes;
            personAdditional.BirthPlace = personAdditionalVm.BirthPlace;
            personAdditional.CitizenshipDate = personAdditionalVm.CitizenshipDate;
            personAdditional.Veteran = personAdditionalVm.Veteran;
            personAdditional.Disabled = personAdditionalVm.Disabled;
            personAdditional.DisabledComments = personAdditionalVm.DisabledComments;
            personAdditional.Vietnam = personAdditionalVm.Vietnam;
            personAdditional.Other = personAdditionalVm.Other;
            personAdditional.SpecialDisabled = personAdditionalVm.SpecialDisabled;
            personAdditional.Doctor = personAdditionalVm.Doctor;
            personAdditional.BloodDonor = personAdditionalVm.BloodDonor;
            personAdditional.Smoker = personAdditionalVm.Smoker;
            personAdditional.AdvancedDirective = personAdditionalVm.AdvancedDirective;
            personAdditional.EarlyRetirementDate = personAdditionalVm.EarlyRetirementDate;
            personAdditional.NormalRetirementDate = personAdditionalVm.NormalRetirementDate;
            personAdditional.ExpectedRetirementDate = personAdditionalVm.ExpectedRetirementDate;
            personAdditional.DateOfDeath = personAdditionalVm.DateOfDeath;
            if (recordIsNew)
                clientDbContext.PersonAdditionals.Add(personAdditional);


            try
            {
                clientDbContext.SaveChanges();
                if (recordIsNew)
                {
                    PersonAdditional personAdditional1 = clientDbContext.PersonAdditionals 
                    .Where(x => x.PersonId == personAdditionalVm.PersonId).SingleOrDefault();
                    //Include(x => x.Person)
                    // .Where(x => x.PersonAdditionalId == personAdditionalVm.PersonAdditionalId).SingleOrDefault();
                    personAdditionalVm = GetPersonAdditionalsRecord(personAdditional1.PersonAdditionalId);
                }
                else
                {
                    personAdditionalVm = GetPersonAdditionalsRecord(personAdditionalVm.PersonAdditionalId);
                }
                ViewBag.AlertMessage = recordIsNew == true ? "New Person Additional Record Added" : "Person Additional Record Saved";
                //return Json(personAdditionalVm, JsonRequestBehavior.AllowGet); // refresh the view model
            }
            catch (Exception err)
            {
                ViewBag.AlertMessage = "";
                string _message = "";
                IEnumerable<DbEntityValidationResult> errors = clientDbContext.GetValidationErrors();
                if (errors.Count() == 0)
                    _message += err.InnerException.InnerException.Message;
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
            return Json(personAdditionalVm, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PersonAdditionalsDeleteAjax(int personAdditionalIdDdl, int personId)
        {
            if (personAdditionalIdDdl < 1)
                return Json("The person Additional Record does not exist", JsonRequestBehavior.AllowGet);

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            clientDbContext.Database.ExecuteSqlCommand(" " +
                                "DELETE FROM PersonAdditionals " +
                                "WHERE PersonAdditionalId = @PersonAdditionalId ",
                                new SqlParameter("@PersonAdditionalId", personAdditionalIdDdl));

            //// this code is duplicated from get PersonAda() action
            PersonAdditionalVm personAdditionalVm;
            int personAdditionalId = clientDbContext.PersonAdditionals.Where(x => x.PersonId == personId).Select(x => x.PersonAdditionalId).FirstOrDefault();
            if (personAdditionalIdDdl < 1)
            {
                personAdditionalVm = new PersonAdditionalVm();
                personAdditionalVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personAdditionalVm.PersonId = personId;
                personAdditionalVm.PersonAdditionalId = 0;
            }
            else
                personAdditionalVm = GetPersonAdditionalsRecord(personAdditionalId);

            ModelState.Clear();

            return Json(personAdditionalVm, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPersonAdditionalsList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name).Select(x => x.PersonId).SingleOrDefault()
                : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));

            var personAdditionalList = clientDbContext.PersonAdditionals//.Include(x => x.DdlMedicalAdditionalType.Description)
                .Where(x => x.PersonId == personId)
                .Select(m => new
                {
                    PersonAdditionalId = m.PersonAdditionalId,
                    //PersonAdditionalDescription = m.DdlMedicalAdditionalType.Description
                });//.OrderBy(x => x.PersonAdditionalDescription).ToList();

            return Json(personAdditionalList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPersonEmergencyRecord(int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            PersonEmergencyBasicDetail personAdditionalVm = new PersonEmergencyBasicDetail();
            /*
             * This code for Hospital in Emergency Detail
            personAdditionalVm = clientDbContext.PersonAdditionals
                .Include(x => x.DdlEeoType)
                .Include(x => x.DdlApplicantSource.Description)
                .Include(x => x.DdlHospital.Description).Include(x => x.DdlHospital.PhoneNumber)
                .Include(x => x.Person.Lastname).Include(x => x.Person.Firstname).Where(x => x.PersonId == personId)
                .Select(x => new PersonEmergencyBasicDetail
                {
                    Hospital = x.DdlHospital.Description==null?"": x.DdlHospital.Description,
                    PhoneNumber= x.DdlHospital.PhoneNumber==null?"": x.DdlHospital.PhoneNumber,
                    Doctor = x.Doctor==null?"":x.Doctor,
                }).FirstOrDefault();
                */
            var emergencyContactsList = clientDbContext.PersonRelationships
                        .Include("DdlRelationshipType")
                        .Include("Person1")
                        .Include("Person1.PersonPhoneNumbers")
                        .Where(m => (m.PersonId == personId) && (m.EmergencyContact==true))
                        .Select(m => new
                        {
                            RelationPersonId = m.RelationPersonId,
                            RelationshipType = m.DdlRelationshipType.Description,
                            PersonName = m.Person1.Firstname + " " + m.Person1.Lastname,
                            DOB = m.Person1.DOB,
                            PersonPhoneNumbers = m.Person1.PersonPhoneNumbers.FirstOrDefault()
                        }).FirstOrDefault();
            personAdditionalVm.FullName = emergencyContactsList==null?"N/A":emergencyContactsList.PersonName==null?"N/A": emergencyContactsList.PersonName;
            personAdditionalVm.Contact = emergencyContactsList == null ? "N/A" : emergencyContactsList.PersonPhoneNumbers==null?"N/A":emergencyContactsList.PersonPhoneNumbers.PhoneNumber==null?"N/A": emergencyContactsList.PersonPhoneNumbers.PhoneNumber;
            personAdditionalVm.Relationship = emergencyContactsList == null ? "N/A" : emergencyContactsList.RelationshipType==null?"N/A":emergencyContactsList.RelationshipType;
            return Json(personAdditionalVm, JsonRequestBehavior.AllowGet);
        }

    }
}