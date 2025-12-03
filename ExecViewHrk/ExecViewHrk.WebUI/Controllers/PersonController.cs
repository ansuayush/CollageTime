using ExecViewHrk.Domain.Interface;
using ExecViewHrk.EfAdmin;
using ExecViewHrk.EfClient;
using ExecViewHrk.Models;
using ExecViewHrk.WebUI.Infrastructure;
using ExecViewHrk.WebUI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    [Authorize]
    public class PersonController : Controller
    {
        readonly IPersonRepository _personRepository;
        readonly IPersonAddress _personAddressRepository;
        readonly IPersonPhoneNumbersRepository _personPhoneNumbersRepository;
        public PersonController(IPersonRepository personRepository, IPersonAddress personAddressRepository, IPersonPhoneNumbersRepository personPhoneNumbersRepository)
        {
            _personRepository = personRepository;
            _personAddressRepository = personAddressRepository;
            _personPhoneNumbersRepository = personPhoneNumbersRepository;
        }
        public PersonBasicVm GetPersonRecord(int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            PersonBasicVm personVm;

            if (personId > 0)
            {
                personVm = clientDbContext.Persons
                 .Where(x => x.PersonId == personId)
                    .Select(x => new PersonBasicVm
                    {
                        LastName = x.Lastname,
                        FirstName = x.Firstname,
                        DateOfBirth = x.DOB,
                        Email = x.eMail,
                        AlternateEmail = x.AlternateEMail,
                        GenderId = x.GenderId,
                        IsDependent = x.IsDependent,
                        MaidenName = x.MaidenName,
                        MaritalStatusId = x.ActualMaritalStatusId,
                        MiddleName = x.MiddleName,
                        PersonId = x.PersonId,
                        PreferredName = x.PreferredName,
                        PrefixId = x.PrefixId,
                        Ssn = x.SSN,
                        SuffixId = x.SuffixId,
                        EnteredBy = x.EnteredBy,
                        EnteredDate = x.EnteredDate,
                        ModifiedBy = x.ModifiedBy,
                        ModifiedDate = x.ModifiedDate,
                        IsApplicant = x.IsApplicant,
                        IsStudent = x.IsStudent,
                        IsTrainer = x.IsTrainer
                    })
                    .FirstOrDefault();
                if (personVm != null)
                {
                    personVm.personPhoneNumberVm = _personPhoneNumbersRepository.GetPersonPhoneNumbersList(personId);
                    personVm.personAddressVm = _personAddressRepository.GetPersonIsPrimaryAddressList(personId);
                }

                PersonImage personImages = clientDbContext.PersonImages.FirstOrDefault(x => x.PersonId == personId);
                if (personImages != null)
                {
                    personVm.PersonImage = (byte[])personImages.PersonImageData;
                }
                if (personVm == null)
                {
                    personVm = new PersonBasicVm();
                }
                else
                {
                    if (personVm.GenderId != null)
                    {
                        personVm.Gender = clientDbContext.DdlGenders.Where(x => x.GenderId == personVm.GenderId).Count() > 0 ?
                            clientDbContext.DdlGenders.Where(x => x.GenderId == personVm.GenderId).FirstOrDefault().Description : null;
                    }
                    if (personVm.MaritalStatusId != null)
                    {
                        personVm.MaritialStatus = clientDbContext.DdlMaritalStatuses.Where(x => x.MaritalStatusId == personVm.MaritalStatusId).Count() > 0 ?
                            clientDbContext.DdlMaritalStatuses.Where(x => x.MaritalStatusId == personVm.MaritalStatusId).FirstOrDefault().Description : null;
                    }
                }
            }
            else
            {
                personVm = new PersonBasicVm { PersonId = personId };
            }
            return personVm;
        }

        public PartialViewResult Person()
        {
            string requestType = User.Identity.GetRequestType();

            if (requestType == "NSS" && User.IsInRole("ClientEmployees"))
            {
                throw new Exception("Client Employee trying to access NSS.");
            }
            if (requestType != "IsSelfService") SessionStateHelper.CheckForPersonSelectedValue();

            string connString = User.Identity.GetClientConnectionString();

            ClientDbContext clientDbContext = new ClientDbContext(connString);

            int personId = requestType == "IsSelfService" ? clientDbContext.Persons.Where(x => x.eMail == User.Identity.Name).Select(x => x.PersonId).SingleOrDefault() : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));

            if ((personId == 0) && (requestType == "IsSelfService"))
            {
                throw new Exception("Self service person id is 0 - Personal record cannot be displayed. It doesn't exist.");
            }

            PersonBasicVm personVm = GetPersonRecord(personId);

            return PartialView(personVm);
        }

        public ActionResult PersonClearAjax()
        {
            PersonBasicVm personVm = new PersonBasicVm();
            SessionStateHelper.Set(SessionStateKeys.PERSON_SELECTED_ID, 0);
            return Json(personVm, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PersonDetail(int personId)
        {
            var objPolicyController = DependencyResolver.Current.GetService<LookupTablesController>();
            objPolicyController.ControllerContext = new ControllerContext(Request.RequestContext, objPolicyController);

            var model = GetPersonRecord(personId);
            model.GenderList = ((List<DdlGender>)objPolicyController.GetDdlGenderList().Data).Select(m => new DropDownModel { keyvalue = m.GenderId.ToString(), keydescription = m.Description }).OrderBy(x => x.keydescription).ToList().CleanUp();
            model.PrefixList = ((List<DdlPrefix>)objPolicyController.GetDdlPrefixList().Data).Select(m => new DropDownModel { keyvalue = m.PrefixId.ToString(), keydescription = m.Description }).OrderBy(x => x.keydescription).ToList().CleanUp();
            model.SuffixList = ((List<DdlSuffix>)objPolicyController.GetDdlSuffixList().Data).Select(m => new DropDownModel { keyvalue = m.SuffixId.ToString(), keydescription = m.Description }).OrderBy(x => x.keydescription).ToList().CleanUp();
            model.SuffixList.Insert(0, new DropDownModel { keyvalue = "0", keydescription = "00" });
            model.MaritailStatusList = ((List<DdlMaritalStatus>)objPolicyController.GetDdlMaritalStatusList().Data).Select(m => new DropDownModel { keyvalue = m.MaritalStatusId.ToString(), keydescription = m.Description }).OrderBy(x => x.keydescription).ToList().CleanUp();

            return View(model);
        }

        public ActionResult PersonIndexChangedAjax(int personId)
        {
            SessionStateHelper.Set(SessionStateKeys.PERSON_SELECTED_ID, personId);
            PersonBasicVm personVm = GetPersonRecord(personId);
            return Json(personVm, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PersonSaveAjax(PersonBasicVm personVm)
        {
            var message = "";

            string requestType = User.Identity.GetRequestType();

            if (requestType == "NSS" && User.IsInRole("ClientEmployees")) throw new Exception("Client Employee trying to access NSS.");

            if (requestType.ToLower() != "isselfservice" && personVm.PersonId > 0) SessionStateHelper.Set(SessionStateKeys.PERSON_SELECTED_ID, 0); // this must be a save from add with person list visible

            if (ModelState.IsValid)
            {
                var connString = User.Identity.GetClientConnectionString();
                var clientDbContext = new ClientDbContext(connString);
                Person person = clientDbContext.Persons.FirstOrDefault(x => x.PersonId == personVm.PersonId);

                bool isNewPerson = false;
                if (person == null)
                {
                    AdminDbContext adminDbContext = new AdminDbContext();
                    var prefferedName = clientDbContext.Persons.Where(x => x.Firstname == personVm.FirstName && x.Lastname == personVm.LastName).Count();
                    if (prefferedName >= 1)
                    {
                        return Json(new { succeed = false, Message = " This person already exists as a relation" }, JsonRequestBehavior.AllowGet);
                    }


                    var aspNetUser = adminDbContext.AspNetUsers.FirstOrDefault(x => x.UserName == personVm.Email);
                    if (aspNetUser != null)
                    {
                        //_message += "Username " + personVm.Email + " has already been taken. Record cannot be saved.";
                        message += "Email Id should be unique for Person. Record cannot be saved.";

                        return Json(new { Message = message, succeed = false }, JsonRequestBehavior.AllowGet);
                    }

                    var employerId = 0;

                    // I think this can be condensed - only one session state variable needed - the EmployerId
                    if (User.IsInRole("HrkAdministrators") || User.IsInRole("HrkAccountManagers"))
                        employerId = Convert.ToInt32(User.Identity.GetSelectedClientID());
                    else if (User.IsInRole("ExternalUserAdmins"))
                        employerId = Convert.ToInt32(User.Identity.GetExternalUserSelectedClientID());
                    else if (User.IsInRole("ClientAdminsMultipleCompanies"))
                        employerId = Convert.ToInt32(User.Identity.GetUserMultipleCompaniesSelectedCompanyID());
                    else if (User.IsInRole("ClientAdministrators"))
                        employerId = Convert.ToInt32(User.Identity.GetClientAdminEmployerID());
                    else if (User.IsInRole("ClientManagers"))
                        employerId = Convert.ToInt32(User.Identity.GetClientAdminEmployerID());
                    else if (User.IsInRole("ClientEmployees"))
                        employerId = Convert.ToInt32(User.Identity.GetClientAdminEmployerID());

                    var ssn = personVm.Ssn;
                    var last4DigitSSN = ssn.Substring(ssn.Length - 4, 4);
                    var passwordHash = HttpContext.GetOwinContext().GetUserManager<AppUserManager>().PasswordHasher.HashPassword(last4DigitSSN);

                    var aspNeUser = new EfClient.AspNetUser();
                    aspNeUser.Id = Guid.NewGuid().ToString();
                    aspNeUser.EmployerId = 3;
                    aspNeUser.LastPasswordChangeDate = DateTime.Today;
                    aspNeUser.Email = personVm.Email;
                    aspNeUser.PasswordHash = passwordHash;
                    aspNeUser.SecurityStamp = last4DigitSSN;
                    aspNeUser.UserName = personVm.Email;
                    aspNeUser.LastName = personVm.LastName;
                    aspNeUser.FirstName = personVm.FirstName;
                    clientDbContext.AspNetUsers.Add(aspNeUser);
                    clientDbContext.SaveChanges();
                    var aspNeUserId = aspNeUser.Id;

                    var aspNetRoles = clientDbContext.AspNetRoles.Where(x => x.Name == "ClientEmployees").Select(x => x.Id).FirstOrDefault();
                    var aspNetUserRoles = new EfClient.AspNetUserRole();
                    aspNetUserRoles.UserId = aspNeUserId;
                    aspNetUserRoles.RoleId = aspNetRoles;
                    clientDbContext.AspNetUserRoles.Add(aspNetUserRoles);
                    clientDbContext.SaveChanges();

                    person = new Person
                    {
                        EnteredBy = User.Identity.Name,
                        EnteredDate = DateTime.Now
                    };
                    isNewPerson = true;
                }

                person.ActualMaritalStatusId = personVm.MaritalStatusId;
                person.AlternateEMail = personVm.AlternateEmail;
                person.DOB = personVm.DateOfBirth;
                //update email aspnetusers table also.
                if (personVm.Email != person.eMail & personVm.PersonId > 0)
                {
                    var aspNetUser = clientDbContext.AspNetUsers.FirstOrDefault(x => x.Email == person.eMail);
                    if (aspNetUser != null)
                    {
                        // var aspNetUsrs = new EfClient.AspNetUser();
                        aspNetUser.Email = personVm.Email;
                        aspNetUser.UserName = personVm.Email;
                        clientDbContext.SaveChanges();
                        person.eMail = personVm.Email;
                    }

                }
                else
                {
                    person.eMail = isNewPerson == true ? personVm.Email : person.eMail; // can't update the email because it is also username
                }
                //person.GenderId = personVm.GenderId == null ? null : (personVm.GenderId == 1 ? "M" : "F"); // use this if to store M or F
                person.GenderId = personVm.GenderId;
                person.MiddleName = personVm.MiddleName;
                person.ModifiedBy = User.Identity.Name;
                person.ModifiedDate = DateTime.Now;
                person.PreferredName = personVm.PreferredName;
                person.PrefixId = personVm.PrefixId;
                person.SSN = personVm.Ssn != null ? personVm.Ssn.Replace("-", string.Empty) : null;
                person.SuffixId = personVm.SuffixId;
                person.Lastname = personVm.LastName;
                person.Firstname = personVm.FirstName;
                person.MaidenName = personVm.MaidenName;
                person.ActualMaritalStatusId = personVm.MaritalStatusId;
                person.IsDependent = personVm.IsDependent;
                person.IsTrainer = personVm.IsTrainer;
                person.IsStudent = personVm.IsStudent;
                person.IsApplicant = personVm.IsApplicant;

                if (personVm.PersonId == 0)
                    clientDbContext.Persons.Add(person);

                try
                {
                    clientDbContext.SaveChanges();

                    ViewBag.AlertMessage = personVm.PersonId == 0 ? "New Person Record Added" : "Person Record Saved";

                    //personVm.PersonId = person.PersonId;
                    personVm = GetPersonRecord(person.PersonId);
                    SessionStateHelper.Set(SessionStateKeys.PERSON_SELECTED_ID, person.PersonId);
                }
                catch (Exception err)
                {
                    ViewBag.AlertMessage = "";

                    IEnumerable<DbEntityValidationResult> errors = clientDbContext.GetValidationErrors();
                    if (errors.Count() == 0)
                    {
                        // see if it is a duplicate key error

                        if (clientDbContext.Persons.Where(x => x.SSN == person.SSN).FirstOrDefault() != null)
                        {
                            // ModelState.AddModelError("", "Duplicate Ssn");
                            message += " Duplicate Ssn ";
                        }
                        else
                        //   ModelState.AddModelError("", err.InnerException.Message);
                        if (err.InnerException != null) message += " " + err.InnerException.Message + " ";

                        if (personVm.PersonId == 0)
                            SessionStateHelper.Set(SessionStateKeys.PERSON_SELECTED_ID, 0);
                    }
                    else
                    {
                        foreach (DbEntityValidationResult error in errors)
                        {
                            foreach (var valError in error.ValidationErrors)
                            {
                                if (err.InnerException != null) message += " " + err.InnerException.Message + " ";
                            }
                        }
                    }
                }
            }
            else
            {
                message = Utils.GetErrorString(null, null, ModelState);
                return Json(new { Message = message, succeed = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { personVm, succeed = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditImage()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            int personId = 0;
            personId = Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));
            PersonImage personImages = clientDbContext.PersonImages.Where(x => x.PersonId == personId).SingleOrDefault();
            var personImage = new PersonImage();
            personImage.PersonId = personId;
            if (personImages != null)
            {
                personImage.PersonImageData = (byte[])personImages.PersonImageData;
            }
            return View(personImage);
        }

        private static byte[] ReadFully(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        [HttpPost]
        public ActionResult DeletePersonImage()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            int personId = 0;
            personId = Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));
            PersonImage personImages = clientDbContext.PersonImages.Where(x => x.PersonId == personId).SingleOrDefault();
            if (personImages != null)
            {
                clientDbContext.PersonImages.Remove(personImages);
                TempData["message"] = "";
                TempData["message"] = "Record deleted successfully!";
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
                TempData["Recorddelete"] = "Record does not exist.";
                return Json(new { Message = "Record does not exist.", succeed = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Message = "", succeed = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadPersonImages()
        {
            for (int i = 0; i < Request.Files.Count; i++)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                int personId = Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));
                var file = Request.Files[i];

                if (file.ContentType.ToLower() == "image/jpeg" || file.ContentType.ToLower() == "image/png")
                {
                    PersonImage personImage = clientDbContext.PersonImages.Where(x => x.PersonId == personId).SingleOrDefault();
                    bool isNewImage = false;
                    if (personImage == null)
                    {
                        personImage = new PersonImage();
                        personImage.PersonId = personId;
                        isNewImage = true;
                    }
                    var filebytes = ReadFully(file.InputStream);
                    byte[] data = filebytes;

                    personImage.PersonImageMimeType = file.ContentType;
                    personImage.PersonImageData = data;

                    if (isNewImage)
                        clientDbContext.PersonImages.Add(personImage);

                    try
                    {
                        clientDbContext.SaveChanges();
                        TempData["message"] = "";
                        TempData["message"] = "File Uploaded Successfully!";
                    }
                    catch (NotSupportedException)
                    {
                        return Json("Please upload file of type JPeg / jpg format");
                    }
                    catch
                    {
                        IEnumerable<DbEntityValidationResult> errors = clientDbContext.GetValidationErrors();

                        if (errors.Count() == 0)
                        {
                            ModelState.AddModelError("", "The default database could not be accessed. The requested action cannot be made at this time.");
                        }
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
                    return Json("Please upload file of type JPeg / jpg format");
                }
            }
            return Json("File Uploaded Successfully!");
        }

        public JsonResult PersonsList_Read(string text)
        {

            SessionStateHelper.CheckForPersonSelectedValue();

            List<PersonVm> PersonsList = _personRepository.GetPersonsList("PERSONS", text.ToLower());
            // var peronsList = _personRepository.GetPersonsList().Where(x => x.PersonName.ToLower().Contains(text.ToLower())||x.eMail.ToLower().Contains(text.ToLower())).ToList();

            return Json(PersonsList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EmployeeList_Read(string text)
        {

            SessionStateHelper.CheckForPersonSelectedValue();

            List<PersonVm> EmployeeList = _personRepository.GetPersonsList("EMPLOYEES", text.ToLower());
            // var employeeList = _personRepository.GetEmployeesList().Where(x => x.PersonName.ToLower().Contains(text.ToLower())).ToList();

            return Json(EmployeeList, JsonRequestBehavior.AllowGet);
        }



        public ActionResult EmployeeRoloDex()
        {
            if (User.Identity.GetRequestType() == null)
            {
                return RedirectToActionPermanent("Login", "Account");
            }
            SessionStateHelper.CheckForEmployeeSelectedValue();

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            int employeeId = requestType == "IsSelfService" ? _personRepository.GetEmployeesList().Select(x => x.PersonId).SingleOrDefault()
                : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.EMPLOYEE_PERSON_SELECTED_ID));

            PersonProfileVm personProfileVm = GetEmployeeProfile(employeeId);
            ViewBag.title = personProfileVm.PositionDescription;
            return PartialView("EmployeeRoloDex", personProfileVm);
        }

        public ActionResult PersonRoloDex()
        {
            if (User.Identity.GetRequestType() == null)
            {
                return RedirectToActionPermanent("Login", "Account");
            }
            SessionStateHelper.CheckForPersonSelectedValue();

            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            int personId = requestType == "IsSelfService" ? _personRepository.GetPersonsList("PERSONS", "").Select(x => x.PersonId).SingleOrDefault()
                : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));

            PersonProfileVm personProfileVm = GetPersonProfile(personId);

            int compCodeFromDb = Convert.ToInt32(personProfileVm.CompanyCode);

            var compCode = clientDbContext.CompanyCodes.Where(x => x.CompanyCodeId == compCodeFromDb).Select(x => x.CompanyCodeDescription).FirstOrDefault();

            personProfileVm.CompanyCode = compCode;

            return PartialView("PersonRoloDex", personProfileVm);
        }

        public ActionResult GetPersonProfileAjax(int personId, string personType)
        {
            PersonProfileVm personProfileVm = new PersonProfileVm();
            if (personType == "P")
            {
                SessionStateHelper.Set(SessionStateKeys.PERSON_SELECTED_ID, personId);
                personProfileVm = GetPersonProfile(personId);
            }
            else if (personType == "E")
            {
                SessionStateHelper.Set(SessionStateKeys.EMPLOYEE_PERSON_SELECTED_ID, personId);
                personProfileVm = GetEmployeeProfile(personId);
            }

            return Json(personProfileVm, JsonRequestBehavior.AllowGet);
        }
        public PersonProfileVm GetEmployeeProfile(int personId)
        {
            var connString = User.Identity.GetClientConnectionString();
            var clientDbContext = new ClientDbContext(connString);

            var personProfileVm = _personRepository.GetRolodexData(personId);
            var companydesc = (from cc in clientDbContext.CompanyCodes
                               join e in clientDbContext.Employees on cc.CompanyCodeId equals e.CompanyCodeId
                               join ep in clientDbContext.E_Positions on e.EmployeeId equals ep.EmployeeId
                               where e.PersonId == personId && (ep.actualEndDate > DateTime.Now || ep.actualEndDate == null)
                               select new PersonProfileVm
                               {
                                   CompanyCode = cc.CompanyCodeId.ToString()
                               }).FirstOrDefault();
            if (companydesc == null)
            {
                int compCodeFromDb = Convert.ToInt32(personProfileVm.CompanyCode);
                var compCode = clientDbContext.CompanyCodes.Where(x => x.CompanyCodeId == compCodeFromDb).Select(x => x.CompanyCodeDescription).FirstOrDefault();
                personProfileVm.CompanyCode = compCode;
            }
            else
            {
                var comdesc = clientDbContext.CompanyCodes.Where(c => c.CompanyCodeId.ToString() == companydesc.CompanyCode).Select(x => x.CompanyCodeDescription).FirstOrDefault();
                if (comdesc != null)
                {
                    personProfileVm.CompanyCode = comdesc;
                }
            }
            //int compCodeFromDb = Convert.ToInt32(personProfileVm.CompanyCode);
            //var compCode = clientDbContext.CompanyCodes.Where(x => x.CompanyCodeId == compCodeFromDb).Select(x => x.CompanyCodeDescription).FirstOrDefault();
            //personProfileVm.CompanyCode = compCode;

            if (!string.IsNullOrEmpty(personProfileVm.HireDate))
            {
                personProfileVm.HireDate = Convert.ToDateTime(personProfileVm.HireDate).ToShortDateString();
            }
            PersonImage personImage = clientDbContext.PersonImages.FirstOrDefault(x => x.PersonId == personId);

            if (personImage != null)
            {
                byte[] data = new byte[(int)personImage.PersonImageData.Length];
                data = personImage.PersonImageData;
                personProfileVm.PersonImageBase64 = Convert.ToBase64String(data);
            }
            else
            {
                string fileNameAndPath = Server.MapPath("~/images/PersonSilhouette.jpg");
                using (Image image = Image.FromFile(fileNameAndPath))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();

                        // Convert byte[] to Base64 String
                        var base64String = Convert.ToBase64String(imageBytes);
                        if (personProfileVm != null)
                        {
                            personProfileVm.PersonImageBase64 = base64String;
                        }
                        else
                        {
                            personProfileVm = new PersonProfileVm
                            {
                                PersonImageBase64 = base64String
                            };
                        }
                    }
                }
            }


            personProfileVm.PersonsList = _personRepository.GetEmployeesList();


            int index = personProfileVm.PersonsList.FindIndex(x => x.PersonId == personId);
            personProfileVm.SearchIndex = index + 1;
            if (index < (personProfileVm.PersonsList.Count() - 1))
            {
                personProfileVm.NextPersonId = personProfileVm.PersonsList[index + 1].PersonId.ToString();
            }
            else
            {
                personProfileVm.NextPersonId = "";
            }
            if (index > 0)
            {
                personProfileVm.PreviousPersonId = personProfileVm.PersonsList[index - 1].PersonId.ToString();
            }
            else
            {
                personProfileVm.PreviousPersonId = "";
            }

            return personProfileVm;
        }
        public PersonProfileVm GetPersonProfile(int personId)
        {
            var connString = User.Identity.GetClientConnectionString();
            var clientDbContext = new ClientDbContext(connString);

            var personProfileVm = _personRepository.GetRolodexData(personId);
            //var personProfileVm = clientDbContext.E_Positions
            //.Include("Positions")
            //.Include("Employees")
            ////.Include("E_PositionSalaryHistories")
            //.OrderByDescending(x => x.E_PositionId)
            //.Where(x => x.Employee.PersonId == personId)
            //.Select(x => new PersonProfileVm
            //{
            //    PersonId = personId,
            //    PositionTitle = x.Position.PositionDescription,
            //    LastName = x.Employee.Person.Lastname,
            //    FirstName = x.Employee.Person.Firstname,
            //    CompanyCode = x.Employee.CompanyCode,
            //    HireDate = DbFunctions.TruncateTime(x.Employee.HireDate).ToString(),
            //    Email = x.Employee.Person.eMail,
            //    PhoneNumber = x.Employee.Person.PersonPhoneNumbers.FirstOrDefault().PhoneNumber,
            //    FileNumber = x.Employee.FileNumber
            //}).FirstOrDefault();

            //if (personProfileVm == null && personId > 0)
            //{
            //    personProfileVm = clientDbContext.Persons.Where(x => x.PersonId == personId)
            //        .Select(x => new PersonProfileVm
            //        {
            //            PersonId = personId,
            //            LastName = x.Lastname,
            //            FirstName = x.Firstname,
            //            CompanyCode = "",
            //            FileNumber = "",
            //            PositionTitle = "",
            //            Salary = "",
            //            PhoneNumber = x.PersonPhoneNumbers.FirstOrDefault().PhoneNumber,
            //            Email = x.eMail,
            //            HireDate = ""
            //        }).FirstOrDefault();
            //}

            if (!string.IsNullOrEmpty(personProfileVm.HireDate))
            {
                personProfileVm.HireDate = Convert.ToDateTime(personProfileVm.HireDate).ToShortDateString();
            }
            PersonImage personImage = clientDbContext.PersonImages.FirstOrDefault(x => x.PersonId == personId);

            if (personImage != null)
            {
                byte[] data = new byte[(int)personImage.PersonImageData.Length];
                data = personImage.PersonImageData;
                personProfileVm.PersonImageBase64 = Convert.ToBase64String(data);
            }
            else
            {
                string fileNameAndPath = Server.MapPath("~/images/PersonSilhouette.jpg");
                using (Image image = Image.FromFile(fileNameAndPath))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();

                        // Convert byte[] to Base64 String
                        string base64String = Convert.ToBase64String(imageBytes);
                        if (personProfileVm != null)
                        {
                            personProfileVm.PersonImageBase64 = base64String;
                        }
                        else
                        {
                            personProfileVm = new PersonProfileVm();
                            personProfileVm.PersonImageBase64 = base64String;
                        }
                    }
                }
            }


            personProfileVm.PersonsList = _personRepository.GetPersonsList("PERSONS", "");

            int index = personProfileVm.PersonsList.FindIndex(x => x.PersonId == personId);
            personProfileVm.SearchIndex = index + 1;
            if (index < (personProfileVm.PersonsList.Count() - 1))
            {
                personProfileVm.NextPersonId = personProfileVm.PersonsList[index + 1].PersonId.ToString();
            }
            else
            {
                personProfileVm.NextPersonId = "";
            }
            if (index > 0)
            {
                personProfileVm.PreviousPersonId = personProfileVm.PersonsList[index - 1].PersonId.ToString();
            }
            else
            {
                personProfileVm.PreviousPersonId = "";
            }

            return personProfileVm;
        }

        public JsonResult GetBase64Image()
        {
            var connString = User.Identity.GetClientConnectionString();
            var clientDbContext = new ClientDbContext(connString);

            int personId = Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));

            PersonImage personImage = clientDbContext.PersonImages.FirstOrDefault(x => x.PersonId == personId);

            byte[] data = new byte[(int)personImage.PersonImageData.Length];
            data = personImage.PersonImageData;

            return Json(new { base64image = Convert.ToBase64String(data) }
                , JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void UploadPersonImage(int personId)
        {
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var connString = User.Identity.GetClientConnectionString();
                var clientDbContext = new ClientDbContext(connString);

                var file = Request.Files[i];

                if (file.ContentType == "image/jpeg" || file.ContentLength <= 4000)
                {
                    //ModelState.AddModelError("", "System support only JPeg type image");

                    PersonImage personImage = clientDbContext.PersonImages.FirstOrDefault(x => x.PersonId == personId);
                    bool isNewImage = false;
                    if (personImage == null)
                    {
                        personImage = new PersonImage
                        {
                            PersonId = personId
                        };
                        isNewImage = true;
                    }

                    personImage.PersonImageMimeType = file.ContentType;
                    personImage.PersonImageData = new byte[file.ContentLength];
                    file.InputStream.Read(personImage.PersonImageData, 0, file.ContentLength);

                    if (isNewImage)
                        clientDbContext.PersonImages.Add(personImage);

                    try
                    {
                        clientDbContext.SaveChanges();
                    }
                    catch (NotSupportedException)
                    {
                        ModelState.AddModelError("", "System support only JPeg type image");
                    }
                    catch
                    {
                        IEnumerable<DbEntityValidationResult> errors = clientDbContext.GetValidationErrors();

                        if (errors.Count() == 0)
                        {
                            ModelState.AddModelError("", "The default database could not be accessed. The requested action cannot be made at this time.");
                        }
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
                //else
                //{
                //    ModelState.AddModelError("", "System support only jpg type image");
                //}
            }
        }

        public FileContentResult GetImage()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            int personId = Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));

            PersonImage personImage = clientDbContext.PersonImages.FirstOrDefault(x => x.PersonId == personId);
            if (personImage == null)
                return null;
            else
                return File(personImage.PersonImageData, personImage.PersonImageMimeType);
        }

        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}