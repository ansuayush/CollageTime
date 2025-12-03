using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Infrastructure;
using ExecViewHrk.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Newtonsoft.Json;
using ExecViewHrk.Models;

namespace ExecViewHrk.WebUI.Controllers
{
    public class PersonExaminationsController : Controller
    {
        // GET: PersonExaminations
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PersonExaminationList_Read([DataSourceRequest]DataSourceRequest request)
        {
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            string connString = User.Identity.GetClientConnectionString();
            string requestType = User.Identity.GetRequestType();

            if (requestType == "NSS" && User.IsInRole("ClientEmployees")) throw new Exception("Client Employee trying to access NSS.");

            if (requestType != "IsSelfService") SessionStateHelper.CheckForPersonSelectedValue();

            int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name)
                .Select(x => x.PersonId).SingleOrDefault() : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));

            var personExamList = clientDbContext.PersonExaminations
                .Include(x => x.DdlMedicalExaminationType)
                .Include(x => x.Person)
                .Where(x => x.PersonId == personId)
                                .Select(x => new PersonExaminationVm
                                {
                                    PersonExaminationId = x.PersonExaminationId,
                                    PersonId = x.PersonId,
                                    PersonName = x.Person.Lastname + ", " + x.Person.Firstname,
                                    MedicalExaminationTypeId = x.MedicalExaminationTypeId,
                                    MedicalExaminationDescription = x.DdlMedicalExaminationType.Description,
                                    ExaminationDate = x.ExaminationDate,
                                    Examiner = x.Examiner,
                                    NextScheduledExamination = x.NextScheduledExamination,
                                    Notes = x.Notes,
                                    EnteredBy = x.EnteredBy,
                                    EnteredDate = x.EnteredDate
                                }).ToList();

            return Json(personExamList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult PersonExaminationsMatrixPartial(bool isSelectedIndex = false, int personExaminationIdParam = 0)
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

            if ((personId == 0) && requestType == "IsSelfService")
                throw new Exception("Self service person id is 0 - Personal record cannot be displayed. It doesn't exist.");

            PersonExaminationVm personExaminationVm;

            if (isSelectedIndex && personExaminationIdParam > 0)
            {
                personExaminationVm = GetPersonExaminationsRecord(personExaminationIdParam, personId);
            }
            else
            {
                int personExaminationId = clientDbContext.PersonExaminations.Where(x => x.PersonId == personId).Select(x => x.PersonExaminationId).FirstOrDefault();
                if (personExaminationId == 0)
                {
                    personExaminationVm = new PersonExaminationVm();
                    personExaminationVm.PersonName = clientDbContext.Persons
                        .Where(x => x.PersonId == personId)
                        .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                    personExaminationVm.PersonId = personId;
                }
                else
                    personExaminationVm = GetPersonExaminationsRecord(personExaminationId, personId);
            }

            return PartialView(personExaminationVm);
        }

        // the selected person has changed to a different person
        public ActionResult PersonIndexChangedExaminationsAjax(int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            PersonExaminationVm personExaminationVm;
            int personExaminationId = clientDbContext.PersonExaminations.Where(x => x.PersonId == personId).Select(x => x.PersonExaminationId).FirstOrDefault();
            if (personExaminationId == 0)
            {
                personExaminationVm = new PersonExaminationVm();
                personExaminationVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personExaminationVm.PersonId = personId;
            }
            else
                personExaminationVm = GetPersonExaminationsRecord(personExaminationId, personId);

            SessionStateHelper.Set(SessionStateKeys.PERSON_SELECTED_ID, personId);

            if (ControllerContext.ParentActionViewContext != null
                && !ControllerContext.ParentActionViewContext.ViewData.ModelState.IsValid)
                ControllerContext.ParentActionViewContext.ViewData.ModelState.Clear();

            ModelState.Clear();
            return Json(personExaminationVm, JsonRequestBehavior.AllowGet);
        }

        public PersonExaminationVm GetPersonExaminationsRecord(int personExaminationId, int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            PersonExaminationVm personExaminationVm = new PersonExaminationVm();
            if (personExaminationId != 0)
            {
                personExaminationVm = clientDbContext.PersonExaminations
               .Include(x => x.DdlMedicalExaminationType.Description)
               .Include(x => x.Person.Lastname).Include(x => x.Person.Firstname).Where(x => x.PersonExaminationId == personExaminationId)
               .Select(x => new PersonExaminationVm
               {
                   PersonExaminationId = x.PersonExaminationId,
                   PersonId = x.PersonId,
                   PersonName = x.Person.Lastname + ", " + x.Person.Firstname,
                   MedicalExaminationTypeId = x.MedicalExaminationTypeId,
                   MedicalExaminationDescription = x.DdlMedicalExaminationType.Description,
                   ExaminationDate = x.ExaminationDate,
                   Examiner = x.Examiner,
                   NextScheduledExamination = x.NextScheduledExamination,
                   EnteredBy = x.EnteredBy,
                   EnteredDate = x.EnteredDate,
                   ModifiedBy = x.ModifiedBy,
                   ModifiedDate = x.ModifiedDate,
                   Notes = x.Notes,
               })
               .FirstOrDefault();
            }
            personExaminationVm.PersonId = personId;
            personExaminationVm.PersonExaminationId = 0;
            personExaminationVm.PersonName = clientDbContext.Persons.Where(x => x.PersonId == personId).Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();
            personExaminationVm.MedicalExaminationDropDownList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetMedicalExaminationList());

            return personExaminationVm;
        }
        public string GetMedicalExaminationList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var MedicalExaminationList = clientDbContext.DdlMedicalExaminationTypes.Where(x => x.Active == true).Select(m => new DropDownModel { keyvalue = m.MedicalExaminationTypeId.ToString(), keydescription = m.Description }).OrderBy(x => x.keydescription).ToList().CleanUp();
            return JsonConvert.SerializeObject(MedicalExaminationList);
        }

        public ActionResult PersonExaminationsIndexChangedAjax(int personExaminationIdDdl, int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            PersonExaminationVm personExaminationVm;

            if (personExaminationIdDdl < 1)
            {
                personExaminationVm = new PersonExaminationVm();
                personExaminationVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personExaminationVm.PersonId = personId;
                personExaminationVm.PersonExaminationId = 0;
            }
            else
                personExaminationVm = GetPersonExaminationsRecord(personExaminationIdDdl, personId);

            ModelState.Clear();

            return Json(personExaminationVm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PersonExaminationsSaveAjax(PersonExaminationVm personExaminationVm)
        {

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            bool recordIsNew = false;
            if (requestType.ToLower() != "isselfservice" && personExaminationVm.PersonExaminationId == 0)
            {
                if (SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID) == null)
                {
                    ModelState.AddModelError("", "Cannot determine the id of the person the Examination record is for.");
                    return View(personExaminationVm);
                }
                else
                {
                    personExaminationVm.PersonId = Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));
                }
            }

            if (ModelState.IsValid)
            {
                PersonExamination personExamination = clientDbContext.PersonExaminations.Include(x => x.Person)
                    .Where(x => x.PersonExaminationId == personExaminationVm.PersonExaminationId).SingleOrDefault();

                if (personExaminationVm.MedicalExaminationTypeId != 0)
                {
                    if (personExaminationVm.PersonExaminationId == 0)
                    {
                        var isRecordExists = clientDbContext.PersonExaminations.Where(x => x.MedicalExaminationTypeId == personExaminationVm.MedicalExaminationTypeId
                                                                                && x.ExaminationDate == personExaminationVm.ExaminationDate
                                                                                && x.PersonId == personExaminationVm.PersonId
                        ).Select(a => a.PersonExaminationId).SingleOrDefault();


                        if (personExamination == null && isRecordExists != 0)
                        {
                            return Json(new { succeed = false, Message = "The Examination record  exists for the selected examination date." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    if (personExaminationVm.PersonExaminationId > 0)
                    {
                        var isSameExamExists = clientDbContext.PersonExaminations.Where(x => x.PersonExaminationId != personExaminationVm.PersonExaminationId
                                                            && x.ExaminationDate == personExaminationVm.ExaminationDate
                                                            && x.MedicalExaminationTypeId == personExaminationVm.MedicalExaminationTypeId
                                                            && x.PersonId == personExaminationVm.PersonId
                        ).Select(a => a.PersonExaminationId).SingleOrDefault();

                        if (isSameExamExists != 0)
                        {
                            return Json(new { succeed = false, Message = "The Examination record  exists for examination date." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    if (personExamination == null)
                    {
                        personExamination = new PersonExamination();
                        personExamination.EnteredBy = User.Identity.Name;
                        personExamination.EnteredDate = DateTime.UtcNow;
                        personExamination.PersonId = personExaminationVm.PersonId;
                        recordIsNew = true;
                    }
                    personExamination.MedicalExaminationTypeId = personExaminationVm.MedicalExaminationTypeId;
                    if (!string.IsNullOrEmpty(personExaminationVm.MedicalExaminationDescription))
                    {
                        int medicalExaminationInDb = clientDbContext.DdlMedicalExaminationTypes
                            .Where(x => x.Description == personExaminationVm.MedicalExaminationDescription).Select(x => x.MedicalExaminationTypeId).SingleOrDefault();

                        if (medicalExaminationInDb <= 0)
                        {
                            ModelState.AddModelError("", "The Medical Examination type does not exist.");
                            return Json(new { succeed = false, Message = "The Medical Examination type does not exist." }, JsonRequestBehavior.AllowGet);
                        }
                        else personExamination.MedicalExaminationTypeId = medicalExaminationInDb;
                    }
                }
                personExamination.Notes = personExaminationVm.Notes;
                personExamination.ExaminationDate = personExaminationVm.ExaminationDate;
                personExamination.Examiner = personExaminationVm.Examiner;
                personExamination.NextScheduledExamination = personExaminationVm.NextScheduledExamination;

                if (recordIsNew)
                    clientDbContext.PersonExaminations.Add(personExamination);
                try
                {
                    clientDbContext.SaveChanges();
                    ViewBag.AlertMessage = recordIsNew == true ? "New Person Examination Record Added" : "Person Examination Record Saved";
                    personExaminationVm = GetPersonExaminationsRecord(personExamination.PersonExaminationId, personExamination.PersonId); 
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
            return Json(new { personExaminationVm, succeed = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PersonExaminationsDelete(int personExaminationId)
        {
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            var dbRecord = clientDbContext.PersonExaminations.Where(x => x.PersonExaminationId == personExaminationId).SingleOrDefault();
            if (dbRecord != null)
            {
                clientDbContext.PersonExaminations.Remove(dbRecord);
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
        public ActionResult PersonExaminationsDeleteAjax(int personExaminationIdDdl, int personId)
        {
            if (personExaminationIdDdl < 1)
                return Json("Examination record does not exist", JsonRequestBehavior.AllowGet);

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            clientDbContext.Database.ExecuteSqlCommand(" " +
                                "DELETE FROM PersonExaminations " +
                                "WHERE PersonExaminationId = @PersonExaminationId ",
                                new SqlParameter("@PersonExaminationId", personExaminationIdDdl));
            PersonExaminationVm personExaminationVm;
            int personExaminationId = clientDbContext.PersonExaminations.Where(x => x.PersonId == personId).Select(x => x.PersonExaminationId).FirstOrDefault();
            if (personExaminationId < 1)
            {
                personExaminationVm = new PersonExaminationVm();
                personExaminationVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personExaminationVm.PersonId = personId;
                personExaminationVm.PersonExaminationId = 0;
            }
            else
                personExaminationVm = GetPersonExaminationsRecord(personExaminationId, personId);

            ModelState.Clear();

            return Json(personExaminationVm, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PersonExaminationsDetail(int personExaminationId, int personId)
        {
            return View(GetPersonExaminationsRecord(personExaminationId, personId));
        }
        public JsonResult GetPersonExaminationsList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name).Select(x => x.PersonId).SingleOrDefault()
                : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));

            var personExaminationList = clientDbContext.PersonExaminations.Include(x => x.DdlMedicalExaminationType.Description)
                .Where(x => x.PersonId == personId)
                .Select(m => new
                {
                    PersonExaminationId = m.PersonExaminationId,
                    PersonExaminationDescription = m.DdlMedicalExaminationType.Description
                }).OrderBy(x => x.PersonExaminationDescription).ToList();

            return Json(personExaminationList, JsonRequestBehavior.AllowGet);
        }
    }
}