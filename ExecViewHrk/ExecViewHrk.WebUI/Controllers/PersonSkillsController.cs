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
    public class PersonSkillsController : Controller
    {
        // GET: PersonSkills
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult PersonSkillsMatrixPartial(bool isSelectedIndex = false, int personSkillIdParam = 0)
        {
            PopulateSkillTypes();
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

            PersonSkillVm personSkillVm;

            if (isSelectedIndex && personSkillIdParam > 0)
            {
                personSkillVm = GetPersonSkillsRecord(personSkillIdParam);
            }
            else
            {
                int personSkillId = clientDbContext.PersonSkills.Where(x => x.PersonId == personId).Select(x => x.PersonSkillId).FirstOrDefault();
                if (personSkillId == 0)
                {
                    personSkillVm = new PersonSkillVm();
                    personSkillVm.PersonName = clientDbContext.Persons
                        .Where(x => x.PersonId == personId)
                        .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                    personSkillVm.PersonId = personId;
                }
                else
                    personSkillVm = GetPersonSkillsRecord(personSkillId);
            }


            return PartialView(personSkillVm);
        }

        // the selected person has changed to a different person
        public ActionResult PersonIndexChangedSkillsAjax(int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            // this code is duplicated from get PersonAda() action; drop down on top left of employment screen
            PersonSkillVm personSkillVm;
            int personSkillId = clientDbContext.PersonSkills.Where(x => x.PersonId == personId).Select(x => x.PersonSkillId).FirstOrDefault();
            if (personSkillId == 0)
            {
                personSkillVm = new PersonSkillVm();
                personSkillVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personSkillVm.PersonId = personId;
            }
            else
                personSkillVm = GetPersonSkillsRecord(personSkillId);

            SessionStateHelper.Set(SessionStateKeys.PERSON_SELECTED_ID, personId);

            if (ControllerContext.ParentActionViewContext != null
                && !ControllerContext.ParentActionViewContext.ViewData.ModelState.IsValid)
                ControllerContext.ParentActionViewContext.ViewData.ModelState.Clear();

            ModelState.Clear();
            return Json(personSkillVm, JsonRequestBehavior.AllowGet);
        }

        public PersonSkillVm GetPersonSkillsRecord(int personSkillId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            PersonSkillVm personSkillVm = clientDbContext.PersonSkills
                .Include(x => x.DdlSkill.Description)
                .Include(x => x.DdlSkillLevel.Description)
                .Include(x => x.Person.Lastname).Include(x => x.Person.Firstname).Where(x => x.PersonSkillId == personSkillId)
                .Select(x => new PersonSkillVm
                {
                    PersonSkillId = x.PersonSkillId,
                    PersonId = x.PersonId,
                    PersonName = x.Person.Lastname + ", " + x.Person.Firstname,
                    SkillId = x.SkillId,
                    SkillDescription = x.DdlSkill.Description,
                    SkillLevelId = x.SkillLevelId,
                    SkillLevelDescription = x.DdlSkillLevel.Description,
                    AttainedDate = x.AttainedDate,
                    ExpirationDate = x.ExpirationDate,
                    LastUsedDate = x.LastUsedDate,
                    EffectiveDate = x.EffectiveDate,
                    EnteredBy = x.EnteredBy,
                    EnteredDate = x.EnteredDate,
                    Notes = x.Notes,
                })
                .FirstOrDefault();

            return personSkillVm;
        }

        // the record for the same person has changed ; drop down on top right of ADA record
        public ActionResult PersonSkillsIndexChangedAjax(int personSkillIdDdl, int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            //// this code is duplicated from get PersonAda() action
            PersonSkillVm personSkillVm;

            if (personSkillIdDdl < 1)
            {
                personSkillVm = new PersonSkillVm();
                personSkillVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personSkillVm.PersonId = personId;
                personSkillVm.PersonSkillId = 0;
            }
            else
                personSkillVm = GetPersonSkillsRecord(personSkillIdDdl);

            ModelState.Clear();

            return Json(personSkillVm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PersonSkillsSaveAjax(PersonSkillVm personSkillVm)
        {

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            bool recordIsNew = false;
            if (requestType.ToLower() != "isselfservice" && personSkillVm.PersonSkillId == 0)
            {
                if (SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID) == null)
                {
                    ModelState.AddModelError("", "Cannot determine the id of the person the Skill record is for.");
                    return View(personSkillVm);
                }
                else
                {
                    personSkillVm.PersonId = Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));
                }
            }

            if (ModelState.IsValid)
            {
                PersonSkill personSkill = clientDbContext.PersonSkills.Include(x => x.Person)
                    .Where(x => x.PersonSkillId == personSkillVm.PersonSkillId).SingleOrDefault();

                if (personSkill == null)
                {
                    personSkill = new PersonSkill();
                    personSkill.EnteredBy = User.Identity.Name;
                    personSkill.EnteredDate = DateTime.Now;
                    personSkill.PersonId = personSkillVm.PersonId;
                    recordIsNew = true;
                }
                //else
                //{
                //    personSkill.ModifiedBy = User.Identity.Name;
                //    personExamination.ModifiedDate = DateTime.Now;
                //}

                
                if (!string.IsNullOrEmpty(personSkillVm.SkillDescription))
                {
                    int skillInDb = clientDbContext.DdlSkills
                        .Where(x => x.Description == personSkillVm.SkillDescription).Select(x => x.SkillId).SingleOrDefault();

                    if (skillInDb <= 0)
                    {
                        return Json(new { succeed = false, Message = "The Skill does not exist." }, JsonRequestBehavior.AllowGet);
                    }
                    else personSkill.SkillId = skillInDb;
                }
                 

                if (!string.IsNullOrEmpty(personSkillVm.SkillDescription))
                {
                    int skillLevelInDb = clientDbContext.DdlSkillLevels
                        .Where(x => x.Description == personSkillVm.SkillLevelDescription).Select(x => x.SkillLevelId).SingleOrDefault();

                    //if (skillLevelInDb <= 0)
                    //{
                    //    return Json(new { succeed = false, Message = "The Skill Level does not exist." }, JsonRequestBehavior.AllowGet);
                    //}
                    //else 
                    personSkill.SkillLevelId = skillLevelInDb;
                }


                personSkill.Notes = personSkillVm.Notes;
                personSkill.AttainedDate = personSkillVm.AttainedDate;
                personSkill.ExpirationDate = personSkillVm.ExpirationDate;
                personSkill.LastUsedDate = personSkillVm.LastUsedDate;
                personSkill.EffectiveDate = personSkillVm.EffectiveDate;

                if (recordIsNew)
                    clientDbContext.PersonSkills.Add(personSkill);


                try
                {
                    clientDbContext.SaveChanges();
                    ViewBag.AlertMessage = recordIsNew == true ? "New Person Skill Record Added" : "Person Skill Record Saved";
                    personSkillVm = GetPersonSkillsRecord(personSkill.PersonSkillId); // refresh the view model
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

            return Json(new { personSkillVm, succeed = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PersonSkillsDeleteAjax(int personSkillIdDdl, int personId)
        {
            if (personSkillIdDdl < 1)
                return Json("The person Skill Record does not exist", JsonRequestBehavior.AllowGet);

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            clientDbContext.Database.ExecuteSqlCommand(" " +
                                "DELETE FROM PersonSkills " +
                                "WHERE PersonSkillId = @PersonSkillId ",
                                new SqlParameter("@PersonSkillId", personSkillIdDdl));

            //// this code is duplicated from get PersonAda() action
            PersonSkillVm personSkillVm;
            int personSkillId = clientDbContext.PersonSkills.Where(x => x.PersonId == personId).Select(x => x.PersonSkillId).FirstOrDefault();
            if (personSkillIdDdl < 1)
            {
                personSkillVm = new PersonSkillVm();
                personSkillVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personSkillVm.PersonId = personId;
                personSkillVm.PersonSkillId = 0;
            }
            else
                personSkillVm = GetPersonSkillsRecord(personSkillId);

            ModelState.Clear();
            return Json(new { personSkillVm, succeed = true, Message = "Record deleted successfully!" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPersonSkillsList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name).Select(x => x.PersonId).SingleOrDefault()
                : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));

            var personSkillList = clientDbContext.PersonSkills.Include(x => x.DdlSkill.Description)
                .Where(x => x.PersonId == personId)
                .Select(m => new
                {
                    SkillId=m.SkillId,
                    PersonSkillId = m.PersonSkillId,
                    PersonSkillDescription = m.DdlSkill.Description
                }).OrderBy(x => x.PersonSkillDescription).ToList();

            return Json(personSkillList, JsonRequestBehavior.AllowGet);
        }

        private void PopulateSkillTypes()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var skillTypesList = new ClientDbContext(connString).DdlSkillTypes
                        .Select(s => new SkillTypeVm
                        {
                            SkillTypeId = s.SkillTypeId,
                            SkillTypeDescription = s.Description //+ "-" + e.SkillId.ToString()
                        })
                        .OrderBy(s => s.SkillTypeDescription).ToList();

                skillTypesList.Insert(0, new SkillTypeVm { SkillTypeId = 0, SkillTypeDescription = "--select one--" });

                ViewData["skillTypesList"] = skillTypesList;
                // ViewData["defaultSkillType"] = skillTypesList.First();
            }
        }
    }
}