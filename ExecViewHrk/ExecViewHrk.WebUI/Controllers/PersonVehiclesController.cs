using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
    public class PersonVehiclesController : Controller
    {
        IPersonVehicleRepository _personvehicleRepository;
        public PersonVehiclesController(IPersonVehicleRepository personvehicleRepository)
        {
            _personvehicleRepository = personvehicleRepository;
        }
        // GET: PersonVehicles
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PersonVehicleList_Read([DataSourceRequest]DataSourceRequest request)
        {
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            string connString = User.Identity.GetClientConnectionString();
            string requestType = User.Identity.GetRequestType();

            if (requestType == "NSS" && User.IsInRole("ClientEmployees")) throw new Exception("Client Employee trying to access NSS.");

            if (requestType != "IsSelfService") SessionStateHelper.CheckForPersonSelectedValue();

            int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name)
                .Select(x => x.PersonId).SingleOrDefault() : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));
            var personVehicleList = _personvehicleRepository.GetPersonVehicleList(personId);
            return Json(personVehicleList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }
        public ActionResult PersonVehiclesDelete(int personVehicleId)
        {
            try
            {
                _personvehicleRepository.DeletePersonVehicle(personVehicleId);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, succeed = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Message = "", succeed = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PersonVehicleDetail(int personVehicleId, int personId)
        {
            return View(GetPersonVehicleRecord(personVehicleId, personId));
        }
        public PersonsVehicleVm GetPersonVehicleRecord(int personVehicleId, int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            PersonsVehicleVm personVehicleVm = new PersonsVehicleVm();

            if (personVehicleId != 0)
            {
                personVehicleVm = clientDbContext.PersonVehicles
               .Include(x => x.DdlState.Title)
               .Where(x => x.PersonVehicleId == personVehicleId)
               .Select(x => new PersonsVehicleVm
               {
                   PersonVehicleId = x.PersonVehicleId,
                   PersonId = x.PersonId,
                   PersonName = x.Person.Lastname + ", " + x.Person.Firstname,
                   StateId = x.StateId,
                   StateTitle = x.DdlState.Title,
                   AcquisitionDate = x.AcquisitionDate,
                   SoldDate = x.SoldDate,
                   Make = x.Make,
                   Model = x.Model,
                   Color = x.Color,
                   LicenseNumber = x.LicenseNumber,
                   Notes = x.Notes,
               })
               .FirstOrDefault();
            }

            personVehicleVm.PersonId = personId;
            personVehicleVm.PersonVehicleId = 0;
            personVehicleVm.PersonName = clientDbContext.Persons.Where(x => x.PersonId == personId).Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();
            personVehicleVm.StateDropDownList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetStateList());
            return personVehicleVm;
        }
        public string GetStateList()
        {
            var _list = _personvehicleRepository.GetStateList().CleanUp();
            return JsonConvert.SerializeObject(_list);
        }
        public PartialViewResult PersonVehiclesMatrixPartial(bool isSelectedIndex = false, int personVehicleIdParam = 0)
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

            PersonsVehicleVm personVehicleVm;

            if (isSelectedIndex && personVehicleIdParam > 0)
            {
                personVehicleVm = GetPersonVehiclesRecord(personVehicleIdParam);
            }
            else
            {
                int personVehicleId = clientDbContext.PersonVehicles.Where(x => x.PersonId == personId).Select(x => x.PersonVehicleId).FirstOrDefault();
                if (personVehicleId == 0)
                {
                    personVehicleVm = new PersonsVehicleVm();
                    personVehicleVm.PersonName = clientDbContext.Persons
                        .Where(x => x.PersonId == personId)
                        .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                    personVehicleVm.PersonId = personId;
                }
                else
                    personVehicleVm = GetPersonVehiclesRecord(personVehicleId);
            }


            return PartialView(personVehicleVm);
        }

        // the selected person has changed to a different person
        public ActionResult PersonIndexChangedVehiclesAjax(int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            // this code is duplicated from get PersonVehicle() action; drop down on top left of employment screen
            PersonsVehicleVm personVehicleVm;
            int personVehicleId = clientDbContext.PersonVehicles.Where(x => x.PersonId == personId).Select(x => x.PersonVehicleId).FirstOrDefault();
            if (personVehicleId == 0)
            {
                personVehicleVm = new PersonsVehicleVm();
                personVehicleVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personVehicleVm.PersonId = personId;
            }
            else
                personVehicleVm = GetPersonVehiclesRecord(personVehicleId);

            SessionStateHelper.Set(SessionStateKeys.PERSON_SELECTED_ID, personId);

            if (ControllerContext.ParentActionViewContext != null
                && !ControllerContext.ParentActionViewContext.ViewData.ModelState.IsValid)
                ControllerContext.ParentActionViewContext.ViewData.ModelState.Clear();

            ModelState.Clear();
            return Json(personVehicleVm, JsonRequestBehavior.AllowGet);
        }

        public PersonsVehicleVm GetPersonVehiclesRecord(int personVehicleId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            PersonsVehicleVm personVehicleVm = clientDbContext.PersonVehicles
                .Include(x => x.DdlState.Title)
                .Include(x => x.Person.Lastname).Include(x => x.Person.Firstname).Where(x => x.PersonVehicleId == personVehicleId)
                .Select(x => new PersonsVehicleVm
                {
                    PersonVehicleId = x.PersonVehicleId,
                    PersonId = x.PersonId,
                    PersonName = x.Person.Lastname + ", " + x.Person.Firstname,
                    LicenseNumber = x.LicenseNumber,
                    StateId = x.StateId,
                    StateTitle = x.DdlState.Title,
                    Make = x.Make,
                    Model = x.Model,
                    Color = x.Color,
                    AcquisitionDate = x.AcquisitionDate,
                    SoldDate = x.SoldDate,
                    EnteredBy = x.EnteredBy,
                    EnteredDate = x.EnteredDate,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedDate = x.ModifiedDate,
                    Notes = x.Notes,
                })
                .FirstOrDefault();
            return personVehicleVm;
        }

        // the record for the same person has changed ; drop down on top right of ADA record
        public ActionResult PersonVehiclesIndexChangedAjax(int personVehicleIdDdl, int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            //// this code is duplicated from get PersonVehicle() action
            PersonsVehicleVm personVehicleVm;

            if (personVehicleIdDdl < 1)
            {
                personVehicleVm = new PersonsVehicleVm();
                personVehicleVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personVehicleVm.PersonId = personId;
                personVehicleVm.PersonVehicleId = 0;
            }
            else
                personVehicleVm = GetPersonVehiclesRecord(personVehicleIdDdl);

            ModelState.Clear();

            return Json(personVehicleVm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PersonVehiclesSaveAjax(PersonsVehicleVm personVehicleVm)
        {

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            bool recordIsNew = false;
            if (requestType.ToLower() != "isselfservice" && personVehicleVm.PersonVehicleId == 0)
            {
                if (SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID) == null)
                {
                    ModelState.AddModelError("", "Cannot determine the id of the person the Vehicle record is for.");
                    return View(personVehicleVm);
                }
                else
                {
                    personVehicleVm.PersonId = Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));
                }
            }

            if (ModelState.IsValid)
            {
                if (clientDbContext.PersonVehicles.Where(x => x.LicenseNumber == personVehicleVm.LicenseNumber).Count() <= 0)
                {


                    PersonVehicle personVehicle = clientDbContext.PersonVehicles.Include(x => x.Person)
                        .Where(x => x.PersonVehicleId == personVehicleVm.PersonVehicleId).SingleOrDefault();

                    if (personVehicle == null)
                    {
                        personVehicle = new PersonVehicle();
                        personVehicle.EnteredBy = User.Identity.Name;
                        personVehicle.EnteredDate = DateTime.UtcNow;
                        personVehicle.PersonId = personVehicleVm.PersonId;
                        recordIsNew = true;
                    }
                    else
                    {
                        personVehicle.ModifiedBy = User.Identity.Name;
                        personVehicle.ModifiedDate = DateTime.UtcNow;
                    }


                    if (!string.IsNullOrEmpty(personVehicleVm.StateTitle))
                    {
                        int stateInDb = clientDbContext.DdlStates
                            .Where(x => x.Title == personVehicleVm.StateTitle).Select(x => x.StateId).SingleOrDefault();

                        if (stateInDb <= 0)
                        {
                            return Json(new { succeed = false, Message = "The state does not exist." }, JsonRequestBehavior.AllowGet);
                        }
                        else personVehicle.StateId = stateInDb;
                    }


                    personVehicle.Notes = personVehicleVm.Notes;
                    personVehicle.LicenseNumber = personVehicleVm.LicenseNumber;
                    personVehicle.Make = personVehicleVm.Make;
                    personVehicle.Model = personVehicleVm.Model;
                    personVehicle.Color = personVehicleVm.Color;
                    personVehicle.AcquisitionDate = personVehicleVm.AcquisitionDate;
                    personVehicle.SoldDate = personVehicleVm.SoldDate;
                    personVehicle.StateId=personVehicleVm.StateId;

                    if (recordIsNew)
                        clientDbContext.PersonVehicles.Add(personVehicle);


                    try
                    {
                        clientDbContext.SaveChanges();
                        ViewBag.AlertMessage = recordIsNew == true ? "New Person Vehicle Record Added" : "Person Vehicle Record Updated";
                        personVehicleVm = GetPersonVehiclesRecord(personVehicle.PersonVehicleId); // refresh the view model
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
                    return Json(new
                    {
                        succeed = false,
                        Message = string.Format(CustomErrorMessages.ERROR_DUPLICATE_RECORD, "License Number")
                    }, JsonRequestBehavior.AllowGet);
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
                _message += "\n\nRecord could not be updated at this time.";
                ModelState.AddModelError("", _message + "\n\nRecord could not be updated at this time.");
                return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { personVehicleVm, succeed = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PersonVehiclesDeleteAjax(int personVehicleIdDdl, int personId)
        {
            if (personVehicleIdDdl < 1)
                return Json("The person Vehicle Record does not exist", JsonRequestBehavior.AllowGet);

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            clientDbContext.Database.ExecuteSqlCommand(" " +
                                "DELETE FROM PersonVehicles " +
                                "WHERE PersonVehicleId = @PersonVehicleId ",
                                new SqlParameter("@PersonVehicleId", personVehicleIdDdl));

            //// this code is duplicated from get PersonVehicle() action
            PersonsVehicleVm personVehicleVm;
            int personVehicleId = clientDbContext.PersonVehicles.Where(x => x.PersonId == personId).Select(x => x.PersonVehicleId).FirstOrDefault();
            if (personVehicleId < 1)
            {
                personVehicleVm = new PersonsVehicleVm();
                personVehicleVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personVehicleVm.PersonId = personId;
                personVehicleVm.PersonVehicleId = 0;
            }
            else
            {
                personVehicleVm = GetPersonVehiclesRecord(personVehicleId);
            }
            ModelState.Clear();

            return Json(personVehicleVm, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPersonVehiclesList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name).Select(x => x.PersonId).SingleOrDefault()
                : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));

            var personVehicleList = clientDbContext.PersonVehicles.Include(x => x.DdlState.Title)
                .Where(x => x.PersonId == personId)
                .Select(m => new
                {
                    PersonVehicleId = m.PersonVehicleId,
                    PersonVehicleDescription = m.DdlState.Title,
                }).OrderBy(x => x.PersonVehicleDescription).ToList();

            return Json(personVehicleList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PersonVehicleDelete(int PersonVehicleId)
        {

            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            var dbRecord = clientDbContext.PersonVehicles.Where(x => x.PersonVehicleId == PersonVehicleId).SingleOrDefault();
            if (dbRecord != null)
            {
                clientDbContext.PersonVehicles.Remove(dbRecord);
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