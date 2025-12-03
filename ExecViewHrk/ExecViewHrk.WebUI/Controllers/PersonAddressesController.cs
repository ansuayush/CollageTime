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
using ExecViewHrk.Domain.Interface;

namespace ExecViewHrk.WebUI.Controllers
{
    public class PersonAddressesController : Controller
    {
        readonly IPersonAddress _personAddressRepository;
        public PersonAddressesController(IPersonAddress personAddressRepository)
        {
            _personAddressRepository = personAddressRepository;
        }
        // GET: PersonAddresses
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult PersonAddressesMatrixPartial(bool isSelectedIndex = false, int personAddressIdParam = 0)
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

            PersonAddressVm personAddressVm;

            if (isSelectedIndex && personAddressIdParam > 0)
            {
                personAddressVm = GetPersonAddressesRecord(personAddressIdParam);
            }
            else
            {
                int personAddressId = clientDbContext.PersonAddresses.Where(x => x.PersonId == personId).Select(x => x.PersonAddressId).FirstOrDefault();
                if (personAddressId == 0)
                {
                    personAddressVm = new PersonAddressVm();
                    personAddressVm.PersonName = clientDbContext.Persons
                        .Where(x => x.PersonId == personId)
                        .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                    personAddressVm.PersonId = personId;
                }
                else
                    personAddressVm = GetPersonAddressesRecord(personAddressId);
            }


            return PartialView(personAddressVm);
        }

        // the selected person has changed to a different person
        public ActionResult PersonIndexChangedAddressesAjax(int PersonAddressIdDdl, int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            // this code is duplicated from get PersonAda() action; drop down on top left of employment screen
            PersonAddressVm personAddressVm;
            int personAddressId = clientDbContext.PersonAddresses.Where(x => x.PersonId == personId).Select(x => x.PersonAddressId).FirstOrDefault();
            if (personAddressId == 0)
            {
                personAddressVm = new PersonAddressVm();
                personAddressVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personAddressVm.PersonId = personId;
            }
            else
                personAddressVm = GetPersonAddressesRecord(personAddressId);

            SessionStateHelper.Set(SessionStateKeys.PERSON_SELECTED_ID, personId);

            if (ControllerContext.ParentActionViewContext != null
                && !ControllerContext.ParentActionViewContext.ViewData.ModelState.IsValid)
                ControllerContext.ParentActionViewContext.ViewData.ModelState.Clear();

            ModelState.Clear();
            return Json(personAddressVm, JsonRequestBehavior.AllowGet);
        }

        public PersonAddressVm GetPersonAddressesRecord(int personAddressId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var personAddressVm = _personAddressRepository.GetPersonAddressesDetails(personAddressId);

            return personAddressVm;
        }
        public ActionResult PersonAddressesIndexChangedAjax(int personAddressIdDdl, int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            //// this code is duplicated from get PersonAda() action
            PersonAddressVm personAddressVm;

            if (personAddressIdDdl < 1)
            {
                personAddressVm = new PersonAddressVm();
                personAddressVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personAddressVm.PersonId = personId;
                personAddressVm.PersonAddressId = 0;
            }
            else
                personAddressVm = GetPersonAddressesRecord(personAddressIdDdl);

            ModelState.Clear();

            return Json(personAddressVm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PersonAddressesSaveAjax(PersonAddressVm personAddressVm)
        {

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            //  bool b = bool.Parse("true");
            //PersonAddressVm result = (from p in clientDbContext.PersonAddresses
            //                          where !(p.CorrespondenceAddress)
            //                          select p).SingleOrDefault();
           //var stlist = clientDbContext.PersonAddresses.All(m => m.CorrespondenceAddress == true);
           // PersonAddress personAddresss = clientDbContext.PersonAddresses.Include(x => x.Person)
           //        .Where(x => x.CorrespondenceAddress == !personAddressVm.CorrespondenceAddress).SingleOrDefault();
           // //  PersonAddressVm phoneNumberInDb = clientDbContext.PersonAddresses.Include(x => x.Person).Where(x => x.CorrespondenceAddress).SingleOrDefault();
           // personAddresss.CorrespondenceAddress = false;
           // clientDbContext.SaveChanges();

            string requestType = User.Identity.GetRequestType();

            bool recordIsNew = false;
            if (requestType.ToLower() != "isselfservice" && personAddressVm.PersonAddressId == 0)
            {
                if (SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID) == null)
                {
                    ModelState.AddModelError("", "Cannot determine the id of the person the Address record is for.");
                    return View(personAddressVm);
                }
                else
                {
                    personAddressVm.PersonId = Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));
                }
            }

            if (ModelState.IsValid)
            {


                PersonAddress personAddress = clientDbContext.PersonAddresses.Include(x => x.Person)
                    .Where(x => x.PersonAddressId == personAddressVm.PersonAddressId).SingleOrDefault();

                if (personAddress == null)
                {
                    personAddress = new PersonAddress();
                    personAddress.EnteredBy = User.Identity.Name;
                    personAddress.EnteredDate = DateTime.Now;
                    personAddress.PersonId = personAddressVm.PersonId;
                    recordIsNew = true;
                }
                else
                {
                    personAddress.ModifiedBy = User.Identity.Name;
                    personAddress.ModifiedDate = DateTime.Now;
                }            
                personAddress.AddressTypeId = personAddressVm.AddressTypeId;
                personAddress.CountryId = personAddressVm.CountryId;
                personAddress.StateId = personAddressVm.StateId;
                personAddress.Notes = personAddressVm.Notes;
                personAddress.AddressLineOne = personAddressVm.AddressLineOne;
                personAddress.AddressLineTwo = personAddressVm.AddressLineTwo;
                personAddress.City = personAddressVm.City;
                personAddress.ZipCode = personAddressVm.ZipCode;
                personAddress.CheckPayrollAddress = personAddressVm.CheckPayrollAddress;
                personAddress.CorrespondenceAddress = personAddressVm.CorrespondenceAddress;
                personAddress.IsPrimaryAddress = personAddressVm.IsPrimaryAddress;
                
                if (recordIsNew)
                    clientDbContext.PersonAddresses.Add(personAddress);

                var IsPrimaryAddress = clientDbContext.PersonAddresses.Where(n => (n.IsPrimaryAddress && n.PersonAddressId!= personAddressVm.PersonAddressId && n.PersonId == personAddressVm.PersonId)).FirstOrDefault();
                if (IsPrimaryAddress != null && IsPrimaryAddress.IsPrimaryAddress && personAddressVm.IsPrimaryAddress)
                {
                    return Json(new { succeed = false, Message = "Person Primary Address already exists" }, JsonRequestBehavior.AllowGet);
                }
                try
                {
                    clientDbContext.SaveChanges();
                    ViewBag.AlertMessage = recordIsNew ? "New Person Address Record Added" : "Person Address Record Saved";
                    personAddressVm = GetPersonAddressesRecord(personAddress.PersonAddressId); // refresh the view model
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

            return Json(new { succeed = true, personAddressVm }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PersonAddressesDeleteAjax(int personAddressIdDdl, int personId)
        {
            if (personAddressIdDdl < 1)
                return Json("The person Address Record does not exist", JsonRequestBehavior.AllowGet);

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
                try
                {
                _personAddressRepository.PersonAddressDeleteAjax(personAddressIdDdl);
                clientDbContext.SaveChanges();
                }
                catch(Exception exe)
                {
                    return Json(new { Message = exe.Message, succeed = false }, JsonRequestBehavior.AllowGet);
                }
            PersonAddressVm personAddressVm;
            int personAddressId = clientDbContext.PersonAddresses.Where(x => x.PersonId == personId).Select(x => x.PersonAddressId).FirstOrDefault();
            if (personAddressIdDdl < 1)
            {
                personAddressVm = new PersonAddressVm();
                personAddressVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personAddressVm.PersonId = personId;
                personAddressVm.PersonAddressId = 0;
            }
            else
                personAddressVm = GetPersonAddressesRecord(personAddressId);

            ModelState.Clear();
            return Json(new { personAddressVm, succeed = true, Message = "Record deleted successfully!" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PersonAddressChangedAjax(int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            PersonAddressVm personAddressVm;
            int personAddressId = clientDbContext.PersonAddresses.Where(x => x.PersonId == personId).Select(x => x.PersonAddressId).FirstOrDefault();
            if (personAddressId == 0)
            {
                personAddressVm = new PersonAddressVm();
                personAddressVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personAddressVm.PersonId = personId;
            }
            else
                personAddressVm = GetPersonAddressesRecord(personAddressId);

            SessionStateHelper.Set(SessionStateKeys.PERSON_SELECTED_ID, personId);

            if (ControllerContext.ParentActionViewContext != null
                && !ControllerContext.ParentActionViewContext.ViewData.ModelState.IsValid)
                ControllerContext.ParentActionViewContext.ViewData.ModelState.Clear();

            ModelState.Clear();
            return Json(personAddressVm, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPersonAddressesList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name).Select(x => x.PersonId).SingleOrDefault()
                : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));

            var personAddressList = clientDbContext.PersonAddresses.Include(x => x.DdlAddressType.Description)
                .Where(x => x.PersonId == personId)
                .Select(m => new
                {
                    AddressTypeId = m.PersonAddressId,
                    AddressDescription = m.DdlAddressType.Description
                }).OrderBy(x => x.AddressDescription).ToList();

            return Json(personAddressList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PersonIndexChangedAddressesAjax(int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            // this code is duplicated from get PersonAda() action; drop down on top left of employment screen
            PersonAddressVm personAddressVm;
            int personAdaId = clientDbContext.PersonAddresses.Where(x => x.PersonId == personId).Select(x => x.PersonAddressId).FirstOrDefault();
            if (personAdaId == 0)
            {
                personAddressVm = new PersonAddressVm();
                personAddressVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personAddressVm.PersonId = personId;
            }
            else
                personAddressVm = GetPersonAddressesRecord(personAdaId);

            SessionStateHelper.Set(SessionStateKeys.PERSON_SELECTED_ID, personId);

            if (ControllerContext.ParentActionViewContext != null
                && !ControllerContext.ParentActionViewContext.ViewData.ModelState.IsValid)
                ControllerContext.ParentActionViewContext.ViewData.ModelState.Clear();

            ModelState.Clear();
            return Json(personAddressVm, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PersonAddressesListPartial()
        {
            return View("PersonAddressesMatrixPartial");
        }

        public ActionResult PersonAddressList_Read([DataSourceRequest]DataSourceRequest request)
        {
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            string connString = User.Identity.GetClientConnectionString();
            string requestType = User.Identity.GetRequestType();
            if (requestType == "NSS" && User.IsInRole("ClientEmployees")) throw new Exception("Client Employee trying to access NSS.");
            if (requestType != "IsSelfService") SessionStateHelper.CheckForPersonSelectedValue();
            int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name).Select(x => x.PersonId).SingleOrDefault()
               : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));
            
            var personAddressList = _personAddressRepository.GetPersonAddressesList(personId);

            return Json(personAddressList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        //public ActionResult PersonAddressDetail(int personAddressId)
        //{
        //    var ObjPolicyController = DependencyResolver.Current.GetService<LookupTablesController>();
        //    ObjPolicyController.ControllerContext = new ControllerContext(Request.RequestContext, ObjPolicyController);
        //    var ObjDdlStatesController = DependencyResolver.Current.GetService<DdlStatesController>();
        //    ObjDdlStatesController.ControllerContext = new ControllerContext(Request.RequestContext, ObjDdlStatesController);
        //    var ObjDdlCountriesController = DependencyResolver.Current.GetService<DdlCountriesController>();
        //    ObjDdlStatesController.ControllerContext = new ControllerContext(Request.RequestContext, ObjDdlCountriesController);

        //    SessionStateHelper.Set(SessionStateKeys.PERSON_SELECTED_ID, personAddressId);
        //    var model = GetPersonAddressesRecord(personAddressId);
        //    model.GetAddressTypes = ((List<DdlAddressType>)ObjPolicyController.GetAddressTypes("").Data).Select(m => new DropDownModel { keyvalue = m.AddressTypeId.ToString(), keydescription = m.Description }).OrderBy(x => x.keydescription).ToList().CleanUp();
        //    model.GetStates = ((List<DdlState>)ObjDdlStatesController.GetStates("").Data).Select(m => new DropDownModel { keyvalue = m.StateId.ToString(), keydescription = m.Title }).OrderBy(x => x.keydescription).ToList().CleanUp();
        //    model.GetCountries = ((List<DdlCountry>)ObjDdlCountriesController.GetCountries("").Data).Select(m => new DropDownModel { keyvalue = m.CountryId.ToString(), keydescription = m.Description }).OrderBy(x => x.keydescription).ToList().CleanUp();
        //    return View(model);
        //}       

        public ActionResult PersonAddressDetail(int personAddressId)
        {
            string connString = User.Identity.GetClientConnectionString();
            PersonAddressVm personAddressVm = new PersonAddressVm();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            if (personAddressId == 0)
            {
                personAddressVm.PersonAddressId = 0;
            }
            else
            {
               
                personAddressVm = _personAddressRepository.GetPersonAddressesDetails(personAddressId);
            }

            personAddressVm.GetAddressTypes = _personAddressRepository.GetAddressTypesList();
            personAddressVm.GetStates = _personAddressRepository.GetStatesList();
            personAddressVm.GetCountries = _personAddressRepository.GetCountriesList();
            PersonAddressCleanUp(personAddressVm);
            return View("PersonAddressDetails", personAddressVm);
        }
        public void PersonAddressCleanUp(PersonAddressVm personAddressVm)
        {
            personAddressVm.GetAddressTypes.Insert(0, new DdlAddressType { AddressTypeId = 0, Description = "Select" });
            personAddressVm.GetStates.Insert(0, new DdlState { StateId = 0, Title = "Select" });
            personAddressVm.GetCountries.Insert(0, new DdlCountry { CountryId = 0, Description = "Select" });
            
        }
        public ActionResult PersonAddressDelete(int personAddressId)
        {
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            var dbRecord = clientDbContext.PersonAddresses.Where(x => x.PersonAddressId == personAddressId).SingleOrDefault();
            if (dbRecord != null)
            {
                _personAddressRepository.PersonAddressDeleteAjax(personAddressId);
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