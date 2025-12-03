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
    public class PersonPropertiesController : Controller
    {
        // GET: PersonProperties
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PersonPropertyList_Read([DataSourceRequest]DataSourceRequest request)
        {
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            string connString = User.Identity.GetClientConnectionString();
            string requestType = User.Identity.GetRequestType();

            if (requestType == "NSS" && User.IsInRole("ClientEmployees")) throw new Exception("Client Employee trying to access NSS.");

            if (requestType != "IsSelfService") SessionStateHelper.CheckForPersonSelectedValue();

            int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name)
                .Select(x => x.PersonId).SingleOrDefault() : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));

            var personInnoculationList = clientDbContext.PersonProperties
                .Include(x => x.DdlPropertyType)
                .Include(x => x.Person)
                .Where(x => x.PersonId == personId)
                                .Select(x => new PersonPropertyVm
                                {
                                    PropertyTypeId = x.PropertyTypeId,
                                    PersonId = x.PersonId,
                                    PersonName = x.Person.Lastname + ", " + x.Person.Firstname,
                                    PersonPropertyTypeId = x.PersonPropertyTypeId,
                                    PropertyTypeDescription = x.DdlPropertyType.Description,
                                    AcquiredDate = x.AcquiredDate,
                                    ReleaseDate = x.ReleaseDate,
                                    EstimatedValue=x.EstimatedValue,
                                    AssetNumber=x.AssetNumber,
                                    PropertyDescription=x.PropertyDescription,
                                    Notes = x.Notes,
                                    EnteredBy = x.EnteredBy,
                                    EnteredDate = x.EnteredDate
                                }).ToList();

            return Json(personInnoculationList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult PersonPropertiesMatrixPartial(bool isSelectedIndex = false, int personPropertyTypeIdParam = 0)
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

            PersonPropertyVm personPropertyVm;

            if (isSelectedIndex && personPropertyTypeIdParam > 0)
            {
                personPropertyVm = GetPersonPropertiesRecord(personPropertyTypeIdParam, personId);
            }
            else
            {
                int personPropertyTypeId = clientDbContext.PersonProperties.Where(x => x.PersonId == personId).Select(x => x.PersonPropertyTypeId).FirstOrDefault();
                if (personPropertyTypeId == 0)
                {
                    personPropertyVm = new PersonPropertyVm();
                    personPropertyVm.PersonName = clientDbContext.Persons
                        .Where(x => x.PersonId == personId)
                        .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                    personPropertyVm.PersonId = personId;
                }
                else
                    personPropertyVm = GetPersonPropertiesRecord(personPropertyTypeId,personId);
            }


            return PartialView(personPropertyVm);
        }

        // the selected person has changed to a different person
        public ActionResult PersonIndexChangedPropertiesAjax(int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            // this code is duplicated from get PersonProperty() action; drop down on top left of employment screen
            PersonPropertyVm personPropertyVm;
            int personPropertyTypeId = clientDbContext.PersonProperties.Where(x => x.PersonId == personId).Select(x => x.PersonPropertyTypeId).FirstOrDefault();
            if (personPropertyTypeId == 0)
            {
                personPropertyVm = new PersonPropertyVm();
                personPropertyVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personPropertyVm.PersonId = personId;
            }
            else
                personPropertyVm = GetPersonPropertiesRecord(personPropertyTypeId, personId);

            SessionStateHelper.Set(SessionStateKeys.PERSON_SELECTED_ID, personId);

            if (ControllerContext.ParentActionViewContext != null
                && !ControllerContext.ParentActionViewContext.ViewData.ModelState.IsValid)
                ControllerContext.ParentActionViewContext.ViewData.ModelState.Clear();

            ModelState.Clear();
            return Json(personPropertyVm, JsonRequestBehavior.AllowGet);
        }

        public PersonPropertyVm GetPersonPropertiesRecord(int personPropertyTypeId,int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            PersonPropertyVm personPropertyVm = new PersonPropertyVm();
                if (personPropertyTypeId != 0)
            {
                personPropertyVm = clientDbContext.PersonProperties
                .Include(x => x.DdlPropertyType.Description)
                .Include(x => x.Person.Lastname).Include(x => x.Person.Firstname).Where(x => x.PersonPropertyTypeId == personPropertyTypeId)
                .Select(x => new PersonPropertyVm
                {
                    PersonPropertyTypeId = x.PersonPropertyTypeId,
                    PersonId = x.PersonId,
                    PersonName = x.Person.Lastname + ", " + x.Person.Firstname,
                    PropertyTypeId = x.PropertyTypeId,
                    PropertyTypeDescription = x.DdlPropertyType.Description,
                    AcquiredDate = x.AcquiredDate,
                    ReleaseDate = x.ReleaseDate,
                    EstimatedValue = x.EstimatedValue,
                    AssetNumber = x.AssetNumber,
                    PropertyDescription = x.PropertyDescription,
                    EnteredBy = x.EnteredBy,
                    EnteredDate = x.EnteredDate,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedDate = x.ModifiedDate,
                    Notes = x.Notes,
                })
                .FirstOrDefault();
            }
            personPropertyVm.PersonId = personId;
            personPropertyVm.PersonPropertyTypeId = 0;
            personPropertyVm.PersonName = clientDbContext.Persons.Where(x => x.PersonId == personId).Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();
            personPropertyVm.PropertyDropDownList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetPropertiesList());


            return personPropertyVm;
        }
        public string GetPropertiesList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var GetPropertiesList = clientDbContext.DdlPropertyTypes.Where(x => x.Active == true).Select(m => new DropDownModel { keyvalue = m.PropertyTypeId.ToString(), keydescription = m.Description }).OrderBy(x => x.keydescription).ToList().CleanUp();
            return JsonConvert.SerializeObject(GetPropertiesList);
        }
        // the record for the same person has changed ; drop down on top right of Property record
        public ActionResult PersonPropertiesIndexChangedAjax(int personPropertyTypeIdDdl, int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            //// this code is duplicated from get PersonProperty() action
            PersonPropertyVm personPropertyVm;

            if (personPropertyTypeIdDdl < 1)
            {
                personPropertyVm = new PersonPropertyVm();
                personPropertyVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personPropertyVm.PersonId = personId;
                personPropertyVm.PersonPropertyTypeId = 0;
            }
            else
                personPropertyVm = GetPersonPropertiesRecord(personPropertyTypeIdDdl, personId);

            ModelState.Clear();

            return Json(personPropertyVm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PersonPropertiesSaveAjax(PersonPropertyVm personPropertyVm)
        {

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            bool recordIsNew = false;
            if (requestType.ToLower() != "isselfservice" && personPropertyVm.PersonPropertyTypeId == 0)
            {
                if (SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID) == null)
                {
                    ModelState.AddModelError("", "Cannot determine the id of the person the Property record is for.");
                    return Json(new { succeed = false, Message = "Cannot determine the id of the person the Property record is for." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    personPropertyVm.PersonId = Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));
                }
            }

            if (ModelState.IsValid)
            {
                PersonProperty personProperty = clientDbContext.PersonProperties.Include(x => x.Person)
                    .Where(x => x.PersonPropertyTypeId == personPropertyVm.PersonPropertyTypeId).SingleOrDefault();

                if (personPropertyVm.PropertyTypeId != 0)
                {
                    if (personPropertyVm.PersonPropertyTypeId == 0)
                    {
                        var isRecordExists = clientDbContext.PersonProperties.Where(x => x.PropertyTypeId == personPropertyVm.PropertyTypeId
                                                                                && x.AcquiredDate == personPropertyVm.AcquiredDate
                                                                                && x.PersonId == personPropertyVm.PersonId
                        ).Select(a => a.PersonPropertyTypeId).SingleOrDefault();
                        if (personProperty == null && isRecordExists != 0)
                        {
                            return Json(new { succeed = false, Message = "The Properties record  exists for the selected Acquired date." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    if (personPropertyVm.PersonPropertyTypeId > 0)
                    {
                        var isSameExamExists = clientDbContext.PersonProperties.Where(x => x.PersonPropertyTypeId != personPropertyVm.PersonPropertyTypeId
                                                            && x.AcquiredDate == personPropertyVm.AcquiredDate
                                                            && x.PropertyTypeId == personPropertyVm.PropertyTypeId
                                                            && x.PersonId == personPropertyVm.PersonId
                        ).Select(a => a.PersonPropertyTypeId).SingleOrDefault();

                        if (isSameExamExists != 0)
                        {
                            return Json(new { succeed = false, Message = "The Properties record  exists for Acquired date." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    if (personProperty == null)
                    {
                        personProperty = new PersonProperty();
                        personProperty.EnteredBy = User.Identity.Name;
                        personProperty.EnteredDate = DateTime.Now;
                        personProperty.PersonId = personPropertyVm.PersonId;
                        recordIsNew = true;
                    }
                    //else
                    //{
                    //    personProperty.ModifiedBy = User.Identity.Name;
                    //    personProperty.ModifiedDate = DateTime.Now;
                    //}

                    personProperty.PropertyTypeId = personPropertyVm.PropertyTypeId;
                    if (!string.IsNullOrEmpty(personPropertyVm.PropertyTypeDescription))
                    {
                        int propertyTypeInDb = clientDbContext.DdlPropertyTypes
                            .Where(x => x.Description == personPropertyVm.PropertyTypeDescription).Select(x => x.PropertyTypeId).SingleOrDefault();

                        if (propertyTypeInDb <= 0)
                        {
                            ModelState.AddModelError("", "The property type does not exist.");
                            return Json(new { succeed = false, Message = "The property type does not exist." }, JsonRequestBehavior.AllowGet);
                        }
                        else personProperty.PropertyTypeId = propertyTypeInDb;
                    }
                }

                personProperty.Notes = personPropertyVm.Notes;
                personProperty.AcquiredDate = personPropertyVm.AcquiredDate;
                personProperty.ReleaseDate = personPropertyVm.ReleaseDate;
                personProperty.EstimatedValue = personPropertyVm.EstimatedValue;
                personProperty.AssetNumber = personPropertyVm.AssetNumber;
                personProperty.PropertyDescription = personPropertyVm.PropertyDescription;

                if (recordIsNew)
                    clientDbContext.PersonProperties.Add(personProperty);

                try
                {
                    clientDbContext.SaveChanges();
                    ViewBag.AlertMessage = recordIsNew == true ? "New Person Property Record Added" : "Person Property Record Updated";
                    personPropertyVm = GetPersonPropertiesRecord(personProperty.PersonPropertyTypeId, personProperty.PersonId); 
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

            return Json(new { personPropertyVm, succeed = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PersonPropertiesDelete(int PersonPropertyTypeId)
        {
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            var dbRecord = clientDbContext.PersonProperties.Where(x => x.PersonPropertyTypeId == PersonPropertyTypeId).SingleOrDefault();
            if (dbRecord != null)
            {
                clientDbContext.PersonProperties.Remove(dbRecord);
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
        public ActionResult PersonPropertiesDeleteAjax(int PersonPropertyTypeId, int personId)
        {
            if (PersonPropertyTypeId < 1)
                return Json("The person Property Record does not exist", JsonRequestBehavior.AllowGet);

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            clientDbContext.Database.ExecuteSqlCommand(" " +
                                "DELETE FROM PersonProperties " +
                                "WHERE PersonPropertyTypeId = @PersonPropertyTypeId ",
                                new SqlParameter("@PersonPropertyTypeId", PersonPropertyTypeId));

            //// this code is duplicated from get PersonProperty() action
            PersonPropertyVm personPropertyVm;
            int personPropertyTypeId = clientDbContext.PersonProperties.Where(x => x.PersonId == personId).Select(x => x.PersonPropertyTypeId).FirstOrDefault();
            if (personPropertyTypeId < 1)
            {
                personPropertyVm = new PersonPropertyVm();
                personPropertyVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personPropertyVm.PersonId = personId;
                personPropertyVm.PersonPropertyTypeId = 0;
            }
            else
                personPropertyVm = GetPersonPropertiesRecord(personPropertyTypeId, personId);

            ModelState.Clear();

            return Json(personPropertyVm, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PersonPropertiesDetail(int PersonPropertyTypeId, int personId)
        {
            return View(GetPersonPropertiesRecord(PersonPropertyTypeId, personId));
        }
        public JsonResult GetPersonPropertiesList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name).Select(x => x.PersonId).SingleOrDefault()
                : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));

            var personPropertyList = clientDbContext.PersonProperties.Include(x => x.DdlPropertyType.Description)
                .Where(x => x.PersonId == personId)
                .Select(m => new
                {
                    PersonPropertyTypeId = m.PersonPropertyTypeId,
                    PropertyDescription = m.DdlPropertyType.Description
                }).OrderBy(x => x.PropertyDescription).ToList();

            return Json(personPropertyList, JsonRequestBehavior.AllowGet);
        }
    }
}