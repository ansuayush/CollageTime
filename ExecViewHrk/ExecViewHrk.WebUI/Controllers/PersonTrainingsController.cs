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
    public class PersonTrainingsController : Controller
    {
        // GET: PersonTrainings
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult PersonTrainingsMatrixPartial(bool isSelectedIndex = false, int personTrainingIdParam = 0)
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

            PersonTrainingVm personTrainingVm;

            if (isSelectedIndex && personTrainingIdParam > 0)
            {
                personTrainingVm = GetPersonTrainingsRecord(personTrainingIdParam);
            }
            else
            {
                int personTrainingId = clientDbContext.PersonTrainings.Where(x => x.PersonId == personId).Select(x => x.PersonTrainingId).FirstOrDefault();
                if (personTrainingId == 0)
                {
                    personTrainingVm = new PersonTrainingVm();
                    personTrainingVm.PersonName = clientDbContext.Persons
                        .Where(x => x.PersonId == personId)
                        .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                    personTrainingVm.PersonId = personId;
                }
                else
                    personTrainingVm = GetPersonTrainingsRecord(personTrainingId);
            }


            return PartialView(personTrainingVm);
        }

        // the selected person has changed to a different person
        public ActionResult PersonIndexChangedTrainingsAjax(int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            // this code is duplicated from get PersonAda() action; drop down on top left of employment screen
            PersonTrainingVm personTrainingVm;
            int personTrainingId = clientDbContext.PersonTrainings.Where(x => x.PersonId == personId).Select(x => x.PersonTrainingId).FirstOrDefault();
            if (personTrainingId == 0)
            {
                personTrainingVm = new PersonTrainingVm();
                personTrainingVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personTrainingVm.PersonId = personId;
            }
            else
                personTrainingVm = GetPersonTrainingsRecord(personTrainingId);

            SessionStateHelper.Set(SessionStateKeys.PERSON_SELECTED_ID, personId);

            if (ControllerContext.ParentActionViewContext != null
                && !ControllerContext.ParentActionViewContext.ViewData.ModelState.IsValid)
                ControllerContext.ParentActionViewContext.ViewData.ModelState.Clear();

            ModelState.Clear();
            return Json(personTrainingVm, JsonRequestBehavior.AllowGet);
        }

        public PersonTrainingVm GetPersonTrainingsRecord(int personTrainingId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            PersonTrainingVm personTrainingVm = clientDbContext.PersonTrainings
                .Include(x => x.DdlTrainingCours.Description)
                .Include(x => x.Person.Lastname).Include(x => x.Person.Firstname).Where(x => x.PersonTrainingId == personTrainingId)
                .Select(x => new PersonTrainingVm
                {
                    PersonTrainingId = x.PersonTrainingId,
                    PersonId = x.PersonId,
                    PersonName = x.Person.Lastname + ", " + x.Person.Firstname,
                    TrainingCourseId = x.TrainingCourseId,
                    TrainingCourseDescription = x.DdlTrainingCours.Description,
                    RecommendationDate = x.RecommendationDate,
                    RequiredByDate = x.RequiredByDate,
                    ScheduledDate = x.ScheduledDate,
                    CompletedDate = x.CompletedDate,
                    Grade = x.Grade,
                    TravelCost = x.TravelCost,
                    HotelMealsExpense = x.HotelMealsExpense,
                    ActualCourseCost = x.ActualCourseCost,
                    QualityRelated = x.QualityRelated,
                    OshaRelated = x.OshaRelated,
                    Venue = x.Venue,
                    EnteredBy = x.EnteredBy,
                    EnteredDate = x.EnteredDate,
                    Notes = x.Notes,
                })
                .FirstOrDefault();

            return personTrainingVm;
        }

        // the record for the same person has changed ; drop down on top right of ADA record
        public ActionResult PersonTrainingsIndexChangedAjax(int personTrainingIdDdl, int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            //// this code is duplicated from get PersonAda() action
            PersonTrainingVm personTrainingVm;

            if (personTrainingIdDdl < 1)
            {
                personTrainingVm = new PersonTrainingVm();
                personTrainingVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personTrainingVm.PersonId = personId;
                personTrainingVm.PersonTrainingId = 0;
            }
            else
                personTrainingVm = GetPersonTrainingsRecord(personTrainingIdDdl);

            ModelState.Clear();

            return Json(personTrainingVm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PersonTrainingsSaveAjax(PersonTrainingVm personTrainingVm)
        {

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            bool recordIsNew = false;
            if (requestType.ToLower() != "isselfservice" && personTrainingVm.PersonTrainingId == 0)
            {
                if (SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID) == null)
                {
                    ModelState.AddModelError("", "Cannot determine the id of the person the Training record is for.");
                    return View(personTrainingVm);
                }
                else
                {
                    personTrainingVm.PersonId = Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));
                }
            }

            if (ModelState.IsValid)
            {
                PersonTraining personTraining = clientDbContext.PersonTrainings.Include(x => x.Person)
                    .Where(x => x.PersonTrainingId == personTrainingVm.PersonTrainingId).SingleOrDefault();

                if (personTraining == null)
                {
                    personTraining = new PersonTraining();
                    personTraining.EnteredBy = User.Identity.Name;
                    personTraining.EnteredDate = DateTime.Now;
                    personTraining.PersonId = personTrainingVm.PersonId;
                    recordIsNew = true;
                }

                if (!string.IsNullOrEmpty(personTrainingVm.TrainingCourseDescription))
                {
                    byte trainingCourseInDb = clientDbContext.DdlTrainingCourses
                        .Where(x => x.Description == personTrainingVm.TrainingCourseDescription).Select(x => x.TrainingCourseId).SingleOrDefault();

                    if (trainingCourseInDb <= 0)
                    {
                        return Json(new { succeed = false, Message = "The training course does not exist." }, JsonRequestBehavior.AllowGet);
                    }
                    else personTraining.TrainingCourseId = trainingCourseInDb;
                }
                if ((personTrainingVm.CompletedDate != null) && (personTrainingVm.RecommendationDate != null) && (personTrainingVm.RequiredByDate != null) && (personTrainingVm.ScheduledDate != null))
                {
                    if ((personTrainingVm.CompletedDate < personTrainingVm.RecommendationDate) || (personTrainingVm.CompletedDate < personTrainingVm.RequiredByDate) || (personTrainingVm.CompletedDate < personTrainingVm.ScheduledDate))
                    {
                        return Json(new { succeed = false, Message = "The completed date can't be smaller than recommendation date or requiredby date or scheduled date." }, JsonRequestBehavior.AllowGet);
                    }
                }

                personTraining.Notes = personTrainingVm.Notes;
                personTraining.RecommendationDate = personTrainingVm.RecommendationDate;
                personTraining.RequiredByDate = personTrainingVm.RequiredByDate;
                personTraining.ScheduledDate = personTrainingVm.ScheduledDate;
                personTraining.Grade = personTrainingVm.Grade;
                personTraining.CompletedDate = personTrainingVm.CompletedDate;
                personTraining.TravelCost = personTrainingVm.TravelCost;
                personTraining.HotelMealsExpense = personTrainingVm.HotelMealsExpense;
                personTraining.ActualCourseCost = personTrainingVm.ActualCourseCost;
                personTraining.QualityRelated = personTrainingVm.QualityRelated;
                personTraining.OshaRelated = personTrainingVm.OshaRelated;
                personTraining.Venue = personTrainingVm.Venue;
                if (recordIsNew)
                    clientDbContext.PersonTrainings.Add(personTraining);
                try
                {
                    clientDbContext.SaveChanges();
                    ViewBag.AlertMessage = recordIsNew == true ? "New Person Training Record Added" : "Person Training Record Updated";
                    personTrainingVm = GetPersonTrainingsRecord(personTraining.PersonTrainingId);
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

            return Json(new { succeed = true, personTrainingVm }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PersonTrainingsDeleteAjax(int personTrainingIdDdl, int personId)
        {
            if (personTrainingIdDdl < 1)
                return Json("The person Training Record does not exist", JsonRequestBehavior.AllowGet);

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            clientDbContext.Database.ExecuteSqlCommand(" " +
                                "DELETE FROM PersonTrainings " +
                                "WHERE PersonTrainingId = @PersonTrainingId ",
                                new SqlParameter("@PersonTrainingId", personTrainingIdDdl));

            //// this code is duplicated from get PersonAda() action
            PersonTrainingVm personTrainingVm;
            int personTrainingId = clientDbContext.PersonTrainings.Where(x => x.PersonId == personId).Select(x => x.PersonTrainingId).FirstOrDefault();
            if (personTrainingIdDdl < 1)
            {
                personTrainingVm = new PersonTrainingVm();
                personTrainingVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personTrainingVm.PersonId = personId;
                personTrainingVm.PersonTrainingId = 0;
            }
            else
                personTrainingVm = GetPersonTrainingsRecord(personTrainingId);

            ModelState.Clear();
            return Json(new { personTrainingVm, succeed = true, Message = "Record deleted successfully!" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPersonTrainingsList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name).Select(x => x.PersonId).SingleOrDefault()
                : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));

            var personTrainingList = clientDbContext.PersonTrainings.Include(x => x.DdlTrainingCours.Description)
                .Where(x => x.PersonId == personId)
                .Select(m => new
                {
                    PersonTrainingId = m.PersonTrainingId,
                    PersonTrainingDescription = m.DdlTrainingCours.Description
                }).OrderBy(x => x.PersonTrainingDescription).ToList();

            return Json(personTrainingList, JsonRequestBehavior.AllowGet);
        }
    }
}