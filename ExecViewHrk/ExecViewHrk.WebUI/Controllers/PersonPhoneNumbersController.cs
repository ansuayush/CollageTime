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
using Newtonsoft.Json;
using ExecViewHrk.WebUI.Helpers;
using ExecViewHrk.Models;
using ExecViewHrk.Domain.Interface;

namespace ExecViewHrk.WebUI.Controllers
{
    public class PersonPhoneNumbersController : Controller
    {

        private IPersonPhoneNumbersRepository _IPersonPhoneNumbersRepository;

        public PersonPhoneNumbersController(IPersonPhoneNumbersRepository PersonPhoneNumbersRepository)
        {
            _IPersonPhoneNumbersRepository = PersonPhoneNumbersRepository;
        }
        // GET: PersonPhoneNumbers
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult PersonPhoneNumbersMatrixPartial(bool isSelectedIndex = false, int personPhoneNumberIdParam = 0)
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

            PersonPhoneNumberVm personPhoneNumberVm;

            if (isSelectedIndex && personPhoneNumberIdParam > 0)
            {
                personPhoneNumberVm = GetPersonPhoneNumbersRecord(personPhoneNumberIdParam);
            }
            else
            {
                int personPhoneNumberId = clientDbContext.PersonPhoneNumbers.Where(x => x.PersonId == personId).Select(x => x.PersonPhoneNumberId).FirstOrDefault();
                if (personPhoneNumberId == 0)
                {
                    personPhoneNumberVm = new PersonPhoneNumberVm();
                    personPhoneNumberVm.PersonName = clientDbContext.Persons
                        .Where(x => x.PersonId == personId)
                        .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                    personPhoneNumberVm.PersonId = personId;
                }
                else
                    personPhoneNumberVm = GetPersonPhoneNumbersRecord(personPhoneNumberId);
            }


            return PartialView(personPhoneNumberVm);
        }

        // the selected person has changed to a different person
        public ActionResult PersonIndexChangedPhoneNumbersAjax(int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            // this code is duplicated from get PersonAda() action; drop down on top left of employment screen
            PersonPhoneNumberVm personPhoneNumberVm;
            int personPhoneNumberId = clientDbContext.PersonPhoneNumbers.Where(x => x.PersonId == personId).Select(x => x.PersonPhoneNumberId).FirstOrDefault();
            if (personPhoneNumberId == 0)
            {
                personPhoneNumberVm = new PersonPhoneNumberVm();
                personPhoneNumberVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personPhoneNumberVm.PersonId = personId;
            }
            else
                personPhoneNumberVm = GetPersonPhoneNumbersRecord(personPhoneNumberId);

            SessionStateHelper.Set(SessionStateKeys.PERSON_SELECTED_ID, personId);

            if (ControllerContext.ParentActionViewContext != null
                && !ControllerContext.ParentActionViewContext.ViewData.ModelState.IsValid)
                ControllerContext.ParentActionViewContext.ViewData.ModelState.Clear();

            ModelState.Clear();
            return Json(personPhoneNumberVm, JsonRequestBehavior.AllowGet);
        }

        public PersonPhoneNumberVm GetPersonPhoneNumbersRecord(int personPhoneNumberId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            PersonPhoneNumberVm personPhoneNumberVm = clientDbContext.PersonPhoneNumbers
                .Include(x => x.DdlPhoneType.Description)
                .Include(x => x.Person.Lastname).Include(x => x.Person.Firstname).Where(x => x.PersonPhoneNumberId == personPhoneNumberId)
                .Select(x => new PersonPhoneNumberVm
                {
                    PersonPhoneNumberId = x.PersonPhoneNumberId,
                    PersonId = x.PersonId,
                    PersonName = x.Person.Lastname + ", " + x.Person.Firstname,
                    PhoneTypeId = x.PhoneTypeId,
                    PhoneTypeDescription = x.DdlPhoneType.Description,
                    PhoneNumber = x.PhoneNumber,
                    Extension = x.Extension,
                    IsPrimaryPhone = x.IsPrimaryPhone,
                    EnteredBy = x.EnteredBy,
                    EnteredDate = x.EnteredDate,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedDate = x.ModifiedDate,
                    ProviderId=x.ProviderId
                })
                .FirstOrDefault();

            return personPhoneNumberVm;
        }

        // the record for the same person has changed ; drop down on top right of ADA record
        public ActionResult PersonPhoneNumbersIndexChangedAjax(int personPhoneNumberIdDdl, int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            //// this code is duplicated from get PersonAda() action
            PersonPhoneNumberVm personPhoneNumberVm;

            if (personPhoneNumberIdDdl < 1)
            {
                personPhoneNumberVm = new PersonPhoneNumberVm();
                personPhoneNumberVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personPhoneNumberVm.PersonId = personId;
                personPhoneNumberVm.PersonPhoneNumberId = 0;
            }
            else
                personPhoneNumberVm = GetPersonPhoneNumbersRecord(personPhoneNumberIdDdl);

            ModelState.Clear();

            return Json(personPhoneNumberVm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PersonPhoneNumbersSaveAjax(PersonPhoneNumberVm personPhoneNumberVm)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            bool recordIsNew = false;
            if (requestType.ToLower() != "isselfservice" && personPhoneNumberVm.PersonPhoneNumberId == 0)
            {
                if (SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID) == null)
                {
                    ModelState.AddModelError("", "Cannot determine the id of the person the Test record is for.");
                    return View(personPhoneNumberVm);
                }
                else
                {
                    personPhoneNumberVm.PersonId = Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));
                }
            }
            if (ModelState.IsValid)
            {
                PersonPhoneNumber personPhoneNumber = clientDbContext.PersonPhoneNumbers.Include(x => x.Person)
                    .Where(x => x.PersonPhoneNumberId == personPhoneNumberVm.PersonPhoneNumberId).SingleOrDefault();

                if (personPhoneNumber == null)
                {
                    personPhoneNumber = new PersonPhoneNumber();
                    personPhoneNumber.EnteredBy = User.Identity.Name;
                    personPhoneNumber.EnteredDate = DateTime.Now;
                    personPhoneNumber.PersonId = personPhoneNumberVm.PersonId;
                    recordIsNew = true;
                }
                else
                {
                    personPhoneNumber.ModifiedBy = User.Identity.Name;
                    personPhoneNumber.ModifiedDate = DateTime.Now;
                }              
                personPhoneNumber.PhoneTypeId= personPhoneNumberVm.PhoneTypeId;
                personPhoneNumber.PhoneNumber = personPhoneNumberVm.PhoneNumber;
                personPhoneNumber.Extension = personPhoneNumberVm.Extension;
                personPhoneNumber.IsPrimaryPhone = personPhoneNumberVm.IsPrimaryPhone;
                personPhoneNumber.ProviderId = personPhoneNumberVm.ProviderId;
                if (recordIsNew)
                {
                    var phoneNumberInDb = clientDbContext.PersonPhoneNumbers.Where(x => x.PhoneNumber == personPhoneNumberVm.PhoneNumber).SingleOrDefault();
                    if (phoneNumberInDb != null)
                    {
                        ModelState.AddModelError("", "The Phone Number" + CustomErrorMessages.ERROR_ALREADY_DEFINED);
                        var modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);
                        string _message = "";
                        foreach (var item in modelStateErrors)
                        {
                            if (_message != "") _message += "<br />";
                            _message += item.ErrorMessage;
                        }
                        return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
                    }
                    else { 
                    clientDbContext.PersonPhoneNumbers.Add(personPhoneNumber);
                    }
                }
                var IsPrimary = clientDbContext.PersonPhoneNumbers.Where(n => (n.IsPrimaryPhone && n.PersonPhoneNumberId != personPhoneNumberVm.PersonPhoneNumberId && n.PersonId == personPhoneNumberVm.PersonId)).FirstOrDefault();
                if (IsPrimary != null && IsPrimary.IsPrimaryPhone && personPhoneNumber.IsPrimaryPhone)
                {
                    return Json(new { succeed = false, Message = "Primary Phone already exists" }, JsonRequestBehavior.AllowGet);
                }
                try
                {
                    clientDbContext.SaveChanges();
                    ViewBag.AlertMessage = recordIsNew? "New Person PhoneNumber Record Added" : "Person Phone number Record Saved";
                    personPhoneNumberVm = GetPersonPhoneNumbersRecord(personPhoneNumber.PersonPhoneNumberId); // refresh the view model
                }
                catch (Exception err)
                {
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
            return Json(new { personPhoneNumberVm, succeed = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PersonPhoneNumbersDeleteAjax(int personPhoneNumberIdDdl, int personId)
        {
            if (personPhoneNumberIdDdl < 1)
                return Json("The person phone number record does not exist", JsonRequestBehavior.AllowGet);

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            clientDbContext.Database.ExecuteSqlCommand(" " +
                                "DELETE FROM PersonPhoneNumbers " +
                                "WHERE PersonPhoneNumberId = @PersonPhoneNumberId ",
                                new SqlParameter("@PersonPhoneNumberId", personPhoneNumberIdDdl));

            //// this code is duplicated from get PersonAda() action
            PersonPhoneNumberVm personPhoneNumberVm;
            int personPhoneNumberId = clientDbContext.PersonPhoneNumbers.Where(x => x.PersonId == personId).Select(x => x.PersonPhoneNumberId).FirstOrDefault();
            if (personPhoneNumberIdDdl < 1)
            {
                personPhoneNumberVm = new PersonPhoneNumberVm();
                personPhoneNumberVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personPhoneNumberVm.PersonId = personId;
                personPhoneNumberVm.PersonPhoneNumberId = 0;
            }
            else
                personPhoneNumberVm = GetPersonPhoneNumbersRecord(personPhoneNumberId);

            ModelState.Clear();
            return Json(personPhoneNumberVm, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetPersonPhoneNumbersList()
        //{
        //    string connString = User.Identity.GetClientConnectionString();
        //    ClientDbContext clientDbContext = new ClientDbContext(connString);

        //    string requestType = User.Identity.GetRequestType();

        //    int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name).Select(x => x.PersonId).SingleOrDefault()
        //        : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));

        //    var personPhoneNumberList = clientDbContext.PersonPhoneNumbers //.Include(x => x.DdlPhoneType.Description)
        //        .Where(x => x.PersonId == personId)
        //        .Select(m => new
        //        {
        //            PersonPhoneNumberId = m.PersonPhoneNumberId,
        //            PersonPhoneNumber = m.PhoneNumber
        //        }).OrderBy(x => x.PersonPhoneNumber).ToList();

        //    return Json(personPhoneNumberList, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetPersonPhoneNumbersListByPersonId(int _personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            var personPhoneNumberList = clientDbContext.PersonPhoneNumbers.Include(x => x.DdlPhoneType.Description)
                .Where(x => x.PersonId == _personId)
                .Select(m => new
                {
                    PersonPhoneNumberId = m.PersonPhoneNumberId,
                    PersonPhoneNumber = m.PhoneNumber,
                    PersonPhoneType = m.DdlPhoneType.Description
                }).OrderBy(x => x.PersonPhoneNumber).ToList();

            return Json(personPhoneNumberList, JsonRequestBehavior.AllowGet);
        }
      
        public ActionResult PersonPhoneNumbersListMatrixPartial()
        {
            return View("PersonPhoneNumbersListMatrixPartial");
        }
        public ActionResult PersonPhoneNumbersList_Read([DataSourceRequest]DataSourceRequest request)
        {
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            string connString = User.Identity.GetClientConnectionString();
            string requestType = User.Identity.GetRequestType();
            if (requestType == "NSS" && User.IsInRole("ClientEmployees")) throw new Exception("Client Employee trying to access NSS.");
            if (requestType != "IsSelfService")SessionStateHelper.CheckForPersonSelectedValue();
            int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name).Select(x => x.PersonId).SingleOrDefault()
               : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));
            var personPhoneNumberList = clientDbContext.PersonPhoneNumbers.Include(x => x.DdlPhoneType.Description).Include(x=>x.Providers.ProviderId)
                .Where(x => x.PersonId == personId)
                .Select(m => new
                {
                    PersonId = m.PersonId,
                    PersonPhoneNumberId = m.PersonPhoneNumberId,
                    PhoneNumber = m.PhoneNumber,
                    PhoneTypeDescription = m.DdlPhoneType.Description,
                    Extension = m.Extension,
                    EnteredBy = m.EnteredBy,
                    EnteredDate = m.EnteredDate,
                    ModifiedBy = m.ModifiedBy,
                    ModifiedDate = m.ModifiedDate,
                    IsPrimaryPhone = m.IsPrimaryPhone,
                    ProviderName = m.Providers.ProviderName

                }).OrderBy(x => x.PhoneNumber).ToList();

            return Json(personPhoneNumberList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult PersonPhoneNumbersDetail(int personPhoneNumberId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            PersonPhoneNumberVm personPhoneNumberDatailVm = new PersonPhoneNumberVm();
            if(personPhoneNumberId !=0)
            { 
             //personPhoneNumberDatailVm = clientDbContext.PersonPhoneNumbers
             //   .Include(x => x.DdlPhoneType.Description)
             //   .Include(x => x.Person.Lastname).Include(x => x.Person.Firstname).Where(x => x.PersonPhoneNumberId == personPhoneNumberId)
             //   .Select(x => new PersonPhoneNumberVm
             //   {
             //       PersonPhoneNumberId = x.PersonPhoneNumberId,
             //       PersonId = x.PersonId,
             //       PersonName = x.Person.Lastname + ", " + x.Person.Firstname,
             //       PhoneTypeId = x.PhoneTypeId,
             //       PhoneTypeDescription = x.DdlPhoneType.Description,
             //       PhoneNumber = x.PhoneNumber,
             //       Extension = x.Extension,
             //       IsPrimaryPhone = x.IsPrimaryPhone,
             //       EnteredBy = x.EnteredBy,
             //       EnteredDate = x.EnteredDate,
             //       ModifiedBy = x.ModifiedBy,
             //       ModifiedDate = x.ModifiedDate,
             //   }).FirstOrDefault();

             personPhoneNumberDatailVm = _IPersonPhoneNumbersRepository.GetPersonPhoneNumberRecord(personPhoneNumberId);
            }

            personPhoneNumberDatailVm.PersonPhoneNumberId = 0;
            personPhoneNumberDatailVm.GetPersonPhoneNumberList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetPersonPhoneNumbersList());
            personPhoneNumberDatailVm.GetPersonPhoneTypeList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetPhoneTypeList());
            personPhoneNumberDatailVm.GetProvidersList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetProviderList());
           
            return View("PersonPhoneNumbersMatrixPartial", personPhoneNumberDatailVm);
        }

        public string GetPhoneTypeList()
        {
            var phonetypeList = _IPersonPhoneNumbersRepository.GetPhoneTypeList();
            return JsonConvert.SerializeObject(phonetypeList);
        }
        public string GetProviderList()
        {
            var phonetypeList = _IPersonPhoneNumbersRepository.GetProviderList();
            return JsonConvert.SerializeObject(phonetypeList);
        }
        
        public ActionResult AddPhoneType()
        {
            var model = new DdlPhoneType() { Active = true };
            return View(model);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlPhoneTypeList_Create([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfClient.DdlPhoneType phoneType)
        {

            if (phoneType != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var suffixInDb = clientDbContext.DdlPhoneTypes.Where(x => x.Code == phoneType.Code || x.Description == phoneType.Description).SingleOrDefault();

                if (suffixInDb != null)
                {
                    ModelState.AddModelError("", "The Phone Type CODE or Phone Type Description" + CustomErrorMessages.ERROR_ALREADY_DEFINED);
                }
                else
                {
                    var newPhoneType = new DdlPhoneType
                    {
                        Description = phoneType.Description,
                        Code = phoneType.Code,
                        Active = true
                    };
                    clientDbContext.DdlPhoneTypes.Add(newPhoneType);
                    clientDbContext.SaveChanges();
                    phoneType.PhoneTypeId = newPhoneType.PhoneTypeId;
                }

            }

            return Json(new[] { phoneType }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult GetDdlPhoneTypeList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var phopneTypeList = clientDbContext.DdlPhoneTypes.OrderBy(e => e.Description).ToList();

            return Json(phopneTypeList, JsonRequestBehavior.AllowGet);

        }

        public string GetPersonPhoneNumbersList()
        {
            string requestType = User.Identity.GetRequestType();
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name).Select(x => x.PersonId).SingleOrDefault()
                : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));

            var phonetypeList = clientDbContext.PersonPhoneNumbers .Where(x => x.PersonId == personId).Select(m => new DropDownModel { keyvalue = m.PersonPhoneNumberId.ToString(), keydescription = m.PhoneNumber }).OrderBy(x => x.keydescription).ToList().CleanUp();
            return JsonConvert.SerializeObject(phonetypeList);
        }
        public ActionResult PersonPhoneNumbersDelete(int personPhoneNumberIdDdl)
        {            
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            var dbRecord = clientDbContext.PersonPhoneNumbers.Where(x => x.PersonPhoneNumberId == personPhoneNumberIdDdl).SingleOrDefault();
            if (dbRecord != null)
            {
                _IPersonPhoneNumbersRepository.PersonPhoneNumbersDelete(personPhoneNumberIdDdl);
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