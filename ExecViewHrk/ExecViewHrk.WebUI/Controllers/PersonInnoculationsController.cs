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
    public class PersonInnoculationsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PersonInnoculationList_Read([DataSourceRequest]DataSourceRequest request)
        {
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            string connString = User.Identity.GetClientConnectionString();
            string requestType = User.Identity.GetRequestType();

            if (requestType == "NSS" && User.IsInRole("ClientEmployees")) throw new Exception("Client Employee trying to access NSS.");

            if (requestType != "IsSelfService") SessionStateHelper.CheckForPersonSelectedValue();

            int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name)
                .Select(x => x.PersonId).SingleOrDefault() : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));

            var personInnoculationList = clientDbContext.PersonInnoculations
                .Include(x => x.DdlInnoculationType)
                .Include(x => x.Person)
                .Where(x => x.PersonId == personId)
                                .Select(x => new PersonInnoculationVm
                                {
                                    PersonInnoculationId=x.PersonInnoculationId,
                                    PersonId = x.PersonId,
                                    PersonName = x.Person.Lastname + ", " + x.Person.Firstname,
                                    InnoculationTypeId = x.InnoculationTypeId,
                                    InnoculationDescription = x.DdlInnoculationType.Description,
                                    InnoculationDate = x.InnoculationDate,
                                    ExpirationDate=x.ExpirationDate,
                                    Notes = x.Notes,
                                    EnteredBy = x.EnteredBy,
                                    EnteredDate = x.EnteredDate
                                }).ToList();

            return Json(personInnoculationList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult PersonInnoculationsMatrixPartial(bool isSelectedIndex = false, int personInnoculationIdParam = 0)
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

            PersonInnoculationVm personInnoculationVm;

            if (isSelectedIndex && personInnoculationIdParam > 0)
            {
                personInnoculationVm = GetPersonInnoculationsRecord(personInnoculationIdParam, personId);
            }
            else
            {
                int personInnoculationId = clientDbContext.PersonInnoculations.Where(x => x.PersonId == personId).Select(x => x.PersonInnoculationId).FirstOrDefault();
                if (personInnoculationId == 0)
                {
                    personInnoculationVm = new PersonInnoculationVm();
                    personInnoculationVm.PersonName = clientDbContext.Persons
                        .Where(x => x.PersonId == personId)
                        .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                    personInnoculationVm.PersonId = personId;
                }
                else
                    personInnoculationVm = GetPersonInnoculationsRecord(personInnoculationId, personId);
            }


            return PartialView(personInnoculationVm);
        }

        public ActionResult PersonIndexChangedInnoculationsAjax(int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            // this code is duplicated from get PersonAda() action; drop down on top left of employment screen
            PersonInnoculationVm personInnoculationVm;
            int personInnoculationId = clientDbContext.PersonInnoculations.Where(x => x.PersonId == personId).Select(x => x.PersonInnoculationId).FirstOrDefault();
            if (personInnoculationId == 0)
            {
                personInnoculationVm = new PersonInnoculationVm();
                personInnoculationVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personInnoculationVm.PersonId = personId;
            }
            else
                personInnoculationVm = GetPersonInnoculationsRecord(personInnoculationId, personId);

            SessionStateHelper.Set(SessionStateKeys.PERSON_SELECTED_ID, personId);

            if (ControllerContext.ParentActionViewContext != null
                && !ControllerContext.ParentActionViewContext.ViewData.ModelState.IsValid)
                ControllerContext.ParentActionViewContext.ViewData.ModelState.Clear();

            ModelState.Clear();
            return Json(personInnoculationVm, JsonRequestBehavior.AllowGet);
        }

        public PersonInnoculationVm GetPersonInnoculationsRecord(int personInnoculationId, int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            PersonInnoculationVm PersonInnoculationVm = new PersonInnoculationVm();
            if (personInnoculationId != 0)
            {
                PersonInnoculationVm = clientDbContext.PersonInnoculations
                    .Include(x => x.DdlInnoculationType.Description)
                    .Include(x => x.Person.Lastname).Include(x => x.Person.Firstname).Where(x => x.PersonInnoculationId == personInnoculationId)
                    .Select(x => new PersonInnoculationVm
                    {
                        PersonInnoculationId = x.PersonInnoculationId,
                        PersonId = x.PersonId,
                        PersonName = x.Person.Lastname + ", " + x.Person.Firstname,
                        InnoculationTypeId = x.InnoculationTypeId,
                        InnoculationDescription = x.DdlInnoculationType.Description,
                        InnoculationDate = x.InnoculationDate,
                        ExpirationDate = x.ExpirationDate,
                        EnteredBy = x.EnteredBy,
                        EnteredDate = x.EnteredDate,
                        ModifiedBy = x.ModifiedBy,
                        ModifiedDate = x.ModifiedDate,
                        Notes = x.Notes,

                    })
                    .FirstOrDefault();
            }
            PersonInnoculationVm.PersonId = personId;
            PersonInnoculationVm.PersonInnoculationId = 0;
            PersonInnoculationVm.PersonName = clientDbContext.Persons.Where(x => x.PersonId == personId).Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();
            PersonInnoculationVm.InnoculationDropDownList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetInnoculationList());

            return PersonInnoculationVm;
        }
        public string GetInnoculationList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var GetInnoculationListList = clientDbContext.DdlInnoculationTypes.Where(x => x.Active == true).Select(m => new DropDownModel { keyvalue = m.InnoculationTypeId.ToString(), keydescription = m.Description }).OrderBy(x => x.keydescription).ToList().CleanUp();
            return JsonConvert.SerializeObject(GetInnoculationListList);
        }
        public ActionResult PersonInnoculationsIndexChangedAjax(int personInnoculationIdDdl, int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            //// this code is duplicated from get PersonAda() action
            PersonInnoculationVm personInnoculationVm;

            if (personInnoculationIdDdl < 1)
            {
                personInnoculationVm = new PersonInnoculationVm();
                personInnoculationVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personInnoculationVm.PersonId = personId;
                personInnoculationVm.PersonInnoculationId = 0;
            }
            else
                personInnoculationVm = GetPersonInnoculationsRecord(personInnoculationIdDdl, personId);

            ModelState.Clear();

            return Json(personInnoculationVm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PersonInnoculationsSaveAjax(PersonInnoculationVm personInnoculationVm)
        {

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            bool recordIsNew = false;
            if (requestType.ToLower() != "isselfservice" && personInnoculationVm.PersonInnoculationId == 0)
            {
                if (SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID) == null)
                {
                    ModelState.AddModelError("", "Cannot determine the id of the person the Innoculation record is for.");
                    return Json(new { succeed = false, Message = "Cannot determine the id of the person the Innoculation record is for." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    personInnoculationVm.PersonId = Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));
                }
            }

            if (ModelState.IsValid)
            {
                PersonInnoculation personInnoculation = clientDbContext.PersonInnoculations.Include(x => x.Person)
                    .Where(x => x.PersonInnoculationId == personInnoculationVm.PersonInnoculationId).SingleOrDefault();

                if (personInnoculationVm.InnoculationTypeId != 0)
                {
                    if (personInnoculationVm.PersonInnoculationId == 0)
                    {
                        var isRecordExists = clientDbContext.PersonInnoculations.Where(x => x.InnoculationTypeId == personInnoculationVm.InnoculationTypeId
                                                                                && x.InnoculationDate == personInnoculationVm.InnoculationDate
                                                                                && x.PersonId == personInnoculationVm.PersonId
                        ).Select(a => a.PersonInnoculationId).SingleOrDefault();
                        if (personInnoculation == null && isRecordExists != 0)
                        {
                            return Json(new { succeed = false, Message = "The Innoculation record  exists for the selected Innoculation date." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    if (personInnoculationVm.PersonInnoculationId > 0)
                    {
                        var isSameExamExists = clientDbContext.PersonInnoculations.Where(x => x.PersonInnoculationId != personInnoculationVm.PersonInnoculationId
                                                            && x.InnoculationDate == personInnoculationVm.InnoculationDate
                                                            && x.InnoculationTypeId == personInnoculationVm.InnoculationTypeId
                                                            && x.PersonId == personInnoculationVm.PersonId
                        ).Select(a => a.PersonInnoculationId).SingleOrDefault();

                        if (isSameExamExists != 0)
                        {
                            return Json(new { succeed = false, Message = "The Properties record  exists for Acquired date." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    if (personInnoculation == null)
                {
                        personInnoculation = new PersonInnoculation
                        {
                            EnteredBy = User.Identity.Name,
                            EnteredDate = DateTime.UtcNow,
                            PersonId = personInnoculationVm.PersonId
                        };
                        recordIsNew = true;
                }
                    //else
                    //{
                    //    personInnoculation.ModifiedBy = User.Identity.Name;
                    //    personInnoculation.ModifiedDate = DateTime.UtcNow;
                    //}

                    personInnoculation.InnoculationTypeId = personInnoculationVm.InnoculationTypeId;
                    if (!string.IsNullOrEmpty(personInnoculationVm.InnoculationDescription))
                {
                    int innoculationTypeInDb = clientDbContext.DdlInnoculationTypes.Where(x => x.Description == personInnoculationVm.InnoculationDescription).Select(x => x.InnoculationTypeId).SingleOrDefault();

                    if (innoculationTypeInDb <= 0)
                    {
                        ModelState.AddModelError("", "The innoculation type does not exist.");
                        return Json(new { succeed = false, Message = "The innoculation type does not exist." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        //if (personInnoculationVm.PersonInnoculationId == 0
                        //    && (clientDbContext.PersonInnoculations.Where(x => x.PersonId == personInnoculationVm.PersonId && x.InnoculationTypeId == innoculationTypeInDb).Count() > 0))
                        //{
                        //    return Json(new
                        //    {
                        //        succeed = false,
                        //        Message = string.Format(CustomErrorMessages.ERROR_DUPLICATE_RECORD, "innoculation type")
                        //    }, JsonRequestBehavior.AllowGet);
                        //}
                        //else if (personInnoculationVm.PersonInnoculationId > 0
                        //    && (clientDbContext.PersonInnoculations.Where(x => x.PersonId == personInnoculationVm.PersonId && x.InnoculationTypeId == innoculationTypeInDb
                        //                                                       && x.PersonInnoculationId != personInnoculationVm.PersonInnoculationId).Count() > 0))
                        //{
                        //    return Json(new
                        //    {
                        //        succeed = false,
                        //        Message = string.Format(CustomErrorMessages.ERROR_DUPLICATE_RECORD, "innoculation type")
                        //    }, JsonRequestBehavior.AllowGet);
                        //}
                        //else
                        //{
                            personInnoculation.InnoculationTypeId = innoculationTypeInDb;
                        //}
                    }
                }
            }
                personInnoculation.Notes = personInnoculationVm.Notes;
                personInnoculation.InnoculationDate = personInnoculationVm.InnoculationDate;
                personInnoculation.ExpirationDate = personInnoculationVm.ExpirationDate;

                if (recordIsNew)
                    clientDbContext.PersonInnoculations.Add(personInnoculation);

                try
                {
                    clientDbContext.SaveChanges();
                    ViewBag.AlertMessage = recordIsNew == true ? "New Person Innoculation Record Added" : "Person Innoculation Record Saved";
                    personInnoculationVm = GetPersonInnoculationsRecord(personInnoculation.PersonInnoculationId, personInnoculation.PersonId); // refresh the view model
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
                var modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);
                string _message = "";
                foreach (var item in modelStateErrors)
                {
                    if (_message != "") _message += "<br />";
                    _message += item.ErrorMessage;
                }
                return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { personInnoculationVm, succeed = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PersonInnoculationsDelete(int personInnoculationId)
        {
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            var dbRecord = clientDbContext.PersonInnoculations.Where(x => x.PersonInnoculationId == personInnoculationId).SingleOrDefault();
            if (dbRecord != null)
            {
                clientDbContext.PersonInnoculations.Remove(dbRecord);
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
        public ActionResult PersonInnoculationsDeleteAjax(int personInnoculationIdDdl, int personId)
        {
            if (personInnoculationIdDdl < 1)
                return Json("Innoculation record does not exist", JsonRequestBehavior.AllowGet);

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            clientDbContext.Database.ExecuteSqlCommand(" DELETE FROM PersonInnoculations WHERE PersonInnoculationId = @PersonInnoculationId ",
                                new SqlParameter("@PersonInnoculationId", personInnoculationIdDdl));

            //// this code is duplicated from get PersonAda() action
            PersonInnoculationVm personInnoculationVm;
            int personInnoculationId = clientDbContext.PersonInnoculations.Where(x => x.PersonId == personId).Select(x => x.PersonInnoculationId).FirstOrDefault();
            if (personInnoculationId < 1)
            {
                personInnoculationVm = new PersonInnoculationVm();
                personInnoculationVm.PersonName = clientDbContext.Persons.Where(x => x.PersonId == personId).Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personInnoculationVm.PersonId = personId;
                personInnoculationVm.PersonInnoculationId = 0;
            }
            else
                personInnoculationVm = GetPersonInnoculationsRecord(personInnoculationId, personId);

            ModelState.Clear();

            return Json(personInnoculationVm, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PersonInnoculationsDetail(int personInnoculationId, int personId)
        {
            return View(GetPersonInnoculationsRecord(personInnoculationId, personId));
        }
        public JsonResult GetPersonInnoculationsList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name).Select(x => x.PersonId).SingleOrDefault()
                : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));

            var personInnoculationList = clientDbContext.PersonInnoculations.Include(x => x.DdlInnoculationType.Description)
                .Where(x => x.PersonId == personId)
                .Select(m => new
                {
                    PersonInnoculationId = m.PersonInnoculationId,
                    PersonInnoculationDescription = m.DdlInnoculationType.Description
                }).OrderBy(x => x.PersonInnoculationDescription).ToList();

            return Json(personInnoculationList, JsonRequestBehavior.AllowGet);
        }

    }
}