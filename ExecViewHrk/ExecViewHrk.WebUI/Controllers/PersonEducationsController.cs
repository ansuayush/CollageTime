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
    public class PersonEducationsController : Controller
    {
        // GET: PersonEducations
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult PersonEducationsMatrixPartial(bool isSelectedIndex = false, int personEducationIdParam = 0)
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

            PersonEducationVm personEducationVm;

            if (isSelectedIndex && personEducationIdParam > 0)
            {
                personEducationVm = GetPersonEducationsRecord(personEducationIdParam);
            }
            else
            {
                int personEducationId = clientDbContext.PersonEducations.Where(x => x.PersonId == personId).Select(x => x.PersonEducationId).FirstOrDefault();
                if (personEducationId == 0)
                {
                    personEducationVm = new PersonEducationVm();
                    personEducationVm.PersonName = clientDbContext.Persons
                        .Where(x => x.PersonId == personId)
                        .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                    personEducationVm.PersonId = personId;
                }
                else
                    personEducationVm = GetPersonEducationsRecord(personEducationId);
            }


            return PartialView(personEducationVm);
        }

        // the selected person has changed to a different person
        public ActionResult PersonIndexChangedEducationsAjax(int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            // this code is duplicated from get PersonAda() action; drop down on top left of employment screen
            PersonEducationVm personEducationVm;
            int personEducationId = clientDbContext.PersonEducations.Where(x => x.PersonId == personId).Select(x => x.PersonEducationId).FirstOrDefault();
            if (personEducationId == 0)
            {
                personEducationVm = new PersonEducationVm();
                personEducationVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personEducationVm.PersonId = personId;
            }
            else
                personEducationVm = GetPersonEducationsRecord(personEducationId);

            SessionStateHelper.Set(SessionStateKeys.PERSON_SELECTED_ID, personId);

            if (ControllerContext.ParentActionViewContext != null
                && !ControllerContext.ParentActionViewContext.ViewData.ModelState.IsValid)
                ControllerContext.ParentActionViewContext.ViewData.ModelState.Clear();

            ModelState.Clear();
            return Json(personEducationVm, JsonRequestBehavior.AllowGet);
        }

        public PersonEducationVm GetPersonEducationsRecord(int personEducationId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            PersonEducationVm personEducationVm = clientDbContext.PersonEducations
                .Include(x => x.DdlQualificationType.Description)
                .Include(x => x.DdlDegreeType.Description)
                .Include(x => x.DdlDegreeType1)
                .Include(x => x.DdlEducationLevel)
                .Include(x => x.DdlEducationEstablishment)
                .Include(x => x.Person.Lastname).Include(x => x.Person.Firstname).Where(x => x.PersonEducationId == personEducationId)
                .Select(x => new PersonEducationVm
                {
                    PersonEducationId = x.PersonEducationId,
                    PersonId = x.PersonId,
                    PersonName = x.Person.Lastname + ", " + x.Person.Firstname,
                    QualificationTypeId = x.QualificationTypeId,
                    QualificationTypeDescription = x.DdlQualificationType.Description,
                    MajorId = x.MajorId,
                    MajorDescription = x.DdlDegreeType.Description,
                    MinorId = x.MinorId,
                    MinorDescription = x.DdlDegreeType1.Description,
                    EducationEstablishmentId = x.EducationEstablishmentId,
                    EducationEstablishmentDescription = x.DdlEducationEstablishment.Description,
                    LevelAchievedId = x.LevelAchievedId,
                    LevelDescription = x.DdlEducationLevel.Description,
                    Grade = x.Grade,
                    Gpa = x.Gpa,
                    CreditsEarned = x.CreditsEarned,
                    PlannedStart = x.PlannedStart,
                    PlannedCompletion = x.PlannedCompletion,
                    ActualCompletion = x.ActualCompletion,
                    EnteredBy = x.EnteredBy,
                    EnteredDate = x.EnteredDate,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedDate = x.ModifiedDate,
                    Notes = x.Notes,
                })
                .FirstOrDefault();

            return personEducationVm;
        }

        // the record for the same person has changed ; drop down on top right of ADA record
        public ActionResult PersonEducationsIndexChangedAjax(int personEducationIdDdl, int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            //// this code is duplicated from get PersonAda() action
            PersonEducationVm personEducationVm;

            if (personEducationIdDdl < 1)
            {
                personEducationVm = new PersonEducationVm();
                personEducationVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personEducationVm.PersonId = personId;
                personEducationVm.PersonEducationId = 0;
            }
            else
                personEducationVm = GetPersonEducationsRecord(personEducationIdDdl);

            ModelState.Clear();

            return Json(personEducationVm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PersonEducationsSaveAjax(PersonEducationVm personEducationVm)
        {

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            bool recordIsNew = false;
            if (requestType.ToLower() != "isselfservice" && personEducationVm.PersonEducationId == 0)
            {
                if (SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID) == null)
                {
                    ModelState.AddModelError("", "Cannot determine the id of the person the Education record is for.");
                    return View(personEducationVm);
                }
                else
                {
                    personEducationVm.PersonId = Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));
                }
            }

            if (ModelState.IsValid)
            {
                PersonEducation personEducation = clientDbContext.PersonEducations.Include(x => x.Person)
                    .Where(x => x.PersonEducationId == personEducationVm.PersonEducationId).SingleOrDefault();

                if (personEducation == null)
                {
                    personEducation = new PersonEducation();
                    personEducation.EnteredBy = User.Identity.Name;
                    personEducation.EnteredDate = DateTime.Now;
                    personEducation.PersonId = personEducationVm.PersonId;
                    recordIsNew = true;
                }
                else
                {
                    personEducation.ModifiedBy = User.Identity.Name;
                    personEducation.ModifiedDate = DateTime.Now;
                }

                int qualificationTypeId = 0;
                if (!int.TryParse(personEducationVm.QualificationTypeDescription, out qualificationTypeId))
                {
                    var qualificationTypeInDb = clientDbContext.DdlQualificationTypes
                        .Where(x => x.Description == personEducationVm.QualificationTypeDescription).SingleOrDefault();

                    if (qualificationTypeInDb == null)
                    {
                        ModelState.AddModelError("", "The Qualification type does not exist.");
                        return View(personEducationVm);
                    }
                    else qualificationTypeId = qualificationTypeInDb.QualificationTypeId;
                }
                personEducation.QualificationTypeId = qualificationTypeId;


                int majorId = 0;
                if (!int.TryParse(personEducationVm.MajorDescription, out majorId))
                {
                    var majorInDb = clientDbContext.DdlDegreeTypes
                        .Where(x => x.Description == personEducationVm.MajorDescription).SingleOrDefault();

                    if (majorInDb == null)
                    {
                        ModelState.AddModelError("", "The Major does not exist.");
                        return View(personEducationVm);
                    }
                    else majorId = majorInDb.DegreeTypeId;
                }
                personEducation.MajorId = majorId;


                int minorId = 0;
                if (!int.TryParse(personEducationVm.MinorDescription, out minorId))
                {
                    var minorInDb = clientDbContext.DdlDegreeTypes
                        .Where(x => x.Description == personEducationVm.MinorDescription).SingleOrDefault();

                    if (minorInDb == null)
                    {
                        ModelState.AddModelError("", "The Minor does not exist.");
                        return View(personEducationVm);
                    }
                    else minorId = minorInDb.DegreeTypeId;
                }
                personEducation.MinorId = minorId;


                int educationEstablishmentId = 0;
                if (!int.TryParse(personEducationVm.EducationEstablishmentDescription, out educationEstablishmentId))
                {
                    var educationEstablishmentInDb = clientDbContext.DdlEducationEstablishments
                        .Where(x => x.Description == personEducationVm.EducationEstablishmentDescription).SingleOrDefault();

                    if (educationEstablishmentInDb == null)
                    {
                        ModelState.AddModelError("", "The Education Establishment does not exist.");
                        return View(personEducationVm);
                    }
                    else educationEstablishmentId = educationEstablishmentInDb.EducationEstablishmentId;
                }
                personEducation.EducationEstablishmentId = educationEstablishmentId;


                int levelAchievedId = 0;
                if (!int.TryParse(personEducationVm.LevelDescription, out levelAchievedId))
                {
                    var levelAchievedInDb = clientDbContext.DdlEducationLevels
                        .Where(x => x.Description == personEducationVm.LevelDescription).SingleOrDefault();

                    if (levelAchievedInDb == null)
                    {
                        ModelState.AddModelError("", "The level does not exist.");
                        return View(personEducationVm);
                    }
                    else levelAchievedId = levelAchievedInDb.EducationLevelId;
                }
                personEducation.LevelAchievedId = levelAchievedId;


                personEducation.Notes = personEducationVm.Notes;
                personEducation.Grade = personEducationVm.Grade;
                personEducation.Gpa = personEducationVm.Gpa;
                personEducation.CreditsEarned = personEducationVm.CreditsEarned;
                personEducation.PlannedStart = personEducationVm.PlannedStart;
                personEducation.PlannedCompletion = personEducationVm.PlannedCompletion;
                personEducation.ActualCompletion = personEducationVm.ActualCompletion;

                if (recordIsNew)
                    clientDbContext.PersonEducations.Add(personEducation);


                try
                {
                    clientDbContext.SaveChanges();
                    ViewBag.AlertMessage = recordIsNew == true ? "New Person Education Record Added" : "Person Education Record Saved";
                    personEducationVm = GetPersonEducationsRecord(personEducation.PersonEducationId); // refresh the view model
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
                ModelState.AddModelError("", "The record has been altered on transfer and could not be saved at this time.");
            }

            return Json(personEducationVm, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PersonEducationsDeleteAjax(int personEducationIdDdl, int personId)
        {
            if (personEducationIdDdl < 1)
                return Json("The person Education Record does not exist", JsonRequestBehavior.AllowGet);

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            clientDbContext.Database.ExecuteSqlCommand(" " +
                                "DELETE FROM PersonEducations " +
                                "WHERE PersonEducationId = @PersonEducationId ",
                                new SqlParameter("@PersonEducationId", personEducationIdDdl));

            //// this code is duplicated from get PersonAda() action
            PersonEducationVm personEducationVm;
            int personEducationId = clientDbContext.PersonEducations.Where(x => x.PersonId == personId).Select(x => x.PersonEducationId).FirstOrDefault();
            if (personEducationIdDdl < 1)
            {
                personEducationVm = new PersonEducationVm();
                personEducationVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personEducationVm.PersonId = personId;
                personEducationVm.PersonEducationId = 0;
            }
            else
                personEducationVm = GetPersonEducationsRecord(personEducationId);

            ModelState.Clear();

            return Json(personEducationVm, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPersonEducationsList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name).Select(x => x.PersonId).SingleOrDefault()
                : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));

            var personEducationList = clientDbContext.PersonEducations.Include(x => x.DdlQualificationType.Description)
                .Where(x => x.PersonId == personId)
                .Select(m => new
                {
                    PersonEducationId = m.PersonEducationId,
                    PersonEducationDescription = m.DdlQualificationType.Description
                }).OrderBy(x => x.PersonEducationDescription).ToList();

            return Json(personEducationList, JsonRequestBehavior.AllowGet);
        }
    }
}