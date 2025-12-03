using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using ExecViewHrk.WebUI.Helpers;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Newtonsoft.Json;
using ExecViewHrk.Models;
using ExecViewHrk.Domain.Interface;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System.Data.SqlClient;

namespace ExecViewHrk.WebUI.Controllers
{
    public class PersonEmployeesController : BaseController
    {
        IPersonEmployeeRepository _personEmployeeRepository;

        public PersonEmployeesController(IPersonEmployeeRepository personEmployeeRepository)
        {
            _personEmployeeRepository = personEmployeeRepository;
        }
        string _message = "";
        // GET: PersonEmployees
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult PersonEmployeesMatrixPartial()
        {
            SessionStateHelper.CheckForEmployeeSelectedValue();

            string connString = User.Identity.GetClientConnectionString();

            ClientDbContext clientDbContext = new ClientDbContext(connString);

            int personId = User.Identity.GetRequestType() == "IsSelfService" ? clientDbContext.Employees.Where(x => x.Person.eMail == User.Identity.Name).Select(x => x.PersonId).SingleOrDefault()
                 : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.EMPLOYEE_PERSON_SELECTED_ID));



            PersonEmployeeVm personEmployeeVm = new PersonEmployeeVm();
            personEmployeeVm.EmployeeId = personId;

            return PartialView(personEmployeeVm);
        }

        // the selected person has changed to a different person
        public ActionResult PersonIndexChangedEmployeesAjax(int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            // this code is duplicated from get PersonAda() action; drop down on top left of employment screen
            PersonEmployeeVm personEmployeeVm;
            int personEmployeeId = clientDbContext.Employees.Where(x => x.PersonId == personId).OrderByDescending(x => x.EmployeeId).Select(x => x.EmployeeId).FirstOrDefault();
            if (personEmployeeId == 0)
            {
                personEmployeeVm = new PersonEmployeeVm();
                personEmployeeVm.PersonName = clientDbContext.Persons
                    .Where(x => x.PersonId == personId)
                    .Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();

                personEmployeeVm.PersonId = personId;
                PersonEmployeeVm.MaxEmploymentNumber = 1;
                personEmployeeVm.EmploymentNumber = PersonEmployeeVm.MaxEmploymentNumber;
            }
            else
                personEmployeeVm = _personEmployeeRepository.GetPersonEmployeesRecord(personEmployeeId);

            SessionStateHelper.Set(SessionStateKeys.EMPLOYEE_PERSON_SELECTED_ID, personId);

            if (ControllerContext.ParentActionViewContext != null
                && !ControllerContext.ParentActionViewContext.ViewData.ModelState.IsValid)
                ControllerContext.ParentActionViewContext.ViewData.ModelState.Clear();

            ModelState.Clear();
            return Json(personEmployeeVm, JsonRequestBehavior.AllowGet);
        }
        public PersonEmployeeVm GetPersonEmployeesRecord(int personEmployeeId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            PersonEmployeeVm personEmployeeVm = new PersonEmployeeVm();

            personEmployeeVm = _personEmployeeRepository.GetPersonEmployeesRecord(personEmployeeId);

            LengthOfEmployment lengthOfEmployment = new LengthOfEmployment(personEmployeeVm.HireDate, Convert.ToDateTime(personEmployeeVm.TerminationDate));
            personEmployeeVm.LenghtOfEmployment = lengthOfEmployment.LengthOfEmploymentCalculation();
            return personEmployeeVm;
        }

        // the record for the same person has changed ; drop down on top right of ADA record
        public ActionResult PersonEmployeesIndexChangedAjax(int personEmployeeIdDdl, int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            //// this code is duplicated from get PersonAda() action
            PersonEmployeeVm personEmployeeVm;

            if (personEmployeeIdDdl < 1)
            {
                personEmployeeVm = new PersonEmployeeVm();
                personEmployeeVm.PersonName = clientDbContext.Persons.Where(x => x.PersonId == personId).Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();
                personEmployeeVm.PersonId = personId;
                personEmployeeVm.EmployeeId = 0;
                personEmployeeVm.EmploymentNumber = Convert.ToByte(Convert.ToInt32(PersonEmployeeVm.MaxEmploymentNumber) + 1);
            }
            else
            {
                personEmployeeVm = GetPersonEmployeesRecord(personEmployeeIdDdl);
            }
            ModelState.Clear();

            return Json(personEmployeeVm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PersonEmployeesSaveAjax(PersonEmployeeVm personEmployeeVm)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            string requestType = User.Identity.GetRequestType();
            bool recordIsNew = false;

            var empNumber = clientDbContext.Employees.Where(n => n.EmployeeId == personEmployeeVm.EmployeeId).Select(s => s.EmploymentNumber).FirstOrDefault();
            var maxTerminationDate = clientDbContext.Employees.Where(x => x.PersonId == personEmployeeVm.PersonId).Max(x => x.TerminationDate);

            var statusId = clientDbContext.DdlEmploymentStatuses.Where(x => x.Code == personEmployeeVm.code).Select(x => x.EmploymentStatusId).FirstOrDefault();
            if (statusId > 0)
            {
                personEmployeeVm.EmploymentStatusId = statusId;
            }
            if (personEmployeeVm.HireDate <= maxTerminationDate)
            {
                return Json(new { succeed = false, Message = "You cannot enter an employment record unless previous records have a termination date." }, JsonRequestBehavior.AllowGet);
            }

            if (personEmployeeVm.code == "T")
            {
                if (personEmployeeVm.TerminationDate == null)
                {
                    return Json(new { succeed = false, Message = "Please select Termination Date" }, JsonRequestBehavior.AllowGet);
                }
            }

            if (personEmployeeVm.TerminationDate != null)
            {
                if (personEmployeeVm.TerminationDate < DateTime.Now)
                {
                    if (personEmployeeVm.code != "T")
                    {
                        return Json(new { succeed = false, Message = "Please select Employee Status as Terminated" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            if (personEmployeeVm.HireDate <= maxTerminationDate)
            {
                return Json(new { succeed = false, Message = "You cannot enter an employment record unless previous records have a termination date." }, JsonRequestBehavior.AllowGet);
            }
            if(personEmployeeVm.code == "T")
            if (personEmployeeVm.TerminationDate > DateTime.Now)
            {
                return Json(new { succeed = false, Message = " You can not enter future Termination Date" }, JsonRequestBehavior.AllowGet);

            }

            if (empNumber > 0)
            {
                personEmployeeVm.EmploymentNumber = empNumber;
            }

            //if (personEmployeeVm.BusinessLevelNbr != 0 && personEmployeeVm.EmploymentNumber == 0)
            if (personEmployeeVm.BusinessLevelNbr != null && personEmployeeVm.EmploymentNumber == 0)
            {
                var employees = clientDbContext.Employees.Where(x => x.PersonId == personEmployeeVm.PersonId);

                var empList = employees.ToList();
                var employeePersoncount = empList.Count() + 1;
                personEmployeeVm.EmploymentNumber = (byte)employeePersoncount;
                //var numbers = clientDbContext.Employees.Select(x => x.FileNumber).ToList();

                // int temp;
                //int max = numbers.Select(n => int.TryParse(n, out temp) ? temp : 0).Max();
                //int FileNewNumber = max + 1;
                //personEmployeeVm.FileNumber = Convert.ToString(FileNewNumber);
                var filenumber1 = personEmployeeVm.FileNumber;
                var newfilenumber = clientDbContext.Employees.Where(x => x.FileNumber == filenumber1).FirstOrDefault();
                if (newfilenumber != null)
                {
                    ModelState.AddModelError("", "File Number is already exist.");
                }
            }

            if (personEmployeeVm.CompanyCodeId != 0)
            {
               // var companyCodeDesc = clientDbContext.CompanyCodes.Where(x => x.CompanyCodeId == personEmployeeVm.CompanyCodeId).Select(x => x.CompanyCodeCode).FirstOrDefault();
                // var businesslevelcodeid = clientDbContext.PositionBusinessLevels.Where(x => x.BusinessLevelNbr == personEmployeeVm.DepartmentCodeId).Select(x => x.BusinessLevelNbr).FirstOrDefault();
                //personEmployeeVm.BusinessLevelNbr = Convert.ToInt32(businesslevelcodeid);
               // var businessLevelId = clientDbContext.PositionBusinessLevels.Where(n => n.BusinessLevelNotes == companyCodeDesc).Select(s => s.BusinessLevelNbr).FirstOrDefault();
                personEmployeeVm.BusinessLevelNbr = personEmployeeVm.BusinessLevelNbr;
            }
            if (requestType.ToLower() != "isselfservice" && personEmployeeVm.EmployeeId != 0)
            {
                if (SessionStateHelper.Get(SessionStateKeys.EMPLOYEE_PERSON_SELECTED_ID) == null)
                {
                    ModelState.AddModelError("", "Cannot determine the id of the person the Employee record is for.");
                    return View(personEmployeeVm);
                }
                else
                {
                    personEmployeeVm.PersonId = Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.EMPLOYEE_PERSON_SELECTED_ID));
                }
            }
            //vaidates when adding new employee
            if (personEmployeeVm.PersonId == 0 || personEmployeeVm.CompanyCodeId == 0)
            {
                var modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);
                ModelState.AddModelError("", "The record has been altered on transfer and could not be saved at this time.");
            }
            var empPersonId = clientDbContext.Employees.Where(x => x.EmployeeId == personEmployeeVm.EmployeeId).Select(s => s.PersonId).FirstOrDefault();
            var personEmail = clientDbContext.Persons.Where(x => x.PersonId == empPersonId).Select(x => x.eMail).FirstOrDefault();
            var aspNetUsersId = clientDbContext.AspNetUsers.Where(x => x.UserName == personEmail).Select(s => s.Id).FirstOrDefault();
            if (personEmployeeVm.IsManager)
            {
                var aspNetRolesManagers = clientDbContext.AspNetRoles.Where(x => x.Name == "ClientManagers").Select(x => x.Id).FirstOrDefault();
                clientDbContext.Database.ExecuteSqlCommand("UPDATE AspNetUserRoles SET [RoleId] = {0} WHERE [UserId] = {1} and [RoleId]= {2}", aspNetRolesManagers, aspNetUsersId, aspNetRolesManagers);
                clientDbContext.SaveChanges();
            }
            else
            {
                var aspNetRolesEmployees = clientDbContext.AspNetRoles.Where(x => x.Name == "ClientEmployees").Select(x => x.Id).FirstOrDefault();
                clientDbContext.Database.ExecuteSqlCommand("UPDATE AspNetUserRoles SET [RoleId] = {0} WHERE [UserId] = {1} and [RoleId]= {2}", aspNetRolesEmployees, aspNetUsersId, aspNetRolesEmployees);
                clientDbContext.SaveChanges();
            }
            if (ModelState.IsValid || personEmployeeVm.EmployeeId == 0)
            {
                try
                {
                    Employee personEmployee = clientDbContext.Employees.Include(x => x.Person).Where(x => x.EmployeeId == personEmployeeVm.EmployeeId).SingleOrDefault();

                    List<Employee> existingRecords = new List<Employee>();
                    if (personEmployee == null)
                    {
                        personEmployee = new Employee();
                        personEmployee.EnteredBy = User.Identity.Name;
                        personEmployee.EnteredDate = DateTime.UtcNow;
                        personEmployee.PersonId = personEmployeeVm.PersonId;
                        personEmployee.CompanyCodeId = personEmployeeVm.CompanyCodeId;
                        var employees = clientDbContext.Employees.Where(x => x.PersonId == personEmployeeVm.PersonId);
                        if (employees.Any())
                        {
                            var empList = employees.ToList();
                            var employeePersoncount = empList.Count() + 1;
                            personEmployee.EmploymentNumber = (byte)employeePersoncount;
                            var noEndDateRecord = empList.Where(x => string.IsNullOrEmpty(Convert.ToString(x.TerminationDate)));
                            if (noEndDateRecord.Any())
                            {
                                ModelState.AddModelError("", "You cannot enter an employment record unless previous records have a termination date.");
                                return Json(new { succeed = false, Message = "You cannot enter an employment record unless previous records have a termination date." }, JsonRequestBehavior.AllowGet);
                            }
                        }

                        recordIsNew = true;
                        existingRecords = clientDbContext.Employees.Include(x => x.Person).Where(x => x.EmployeeId == personEmployeeVm.EmployeeId).ToList();
                    }
                    else
                    {
                        personEmployee.ModifiedBy = User.Identity.Name;
                        personEmployee.ModifiedDate = DateTime.UtcNow;
                        personEmployee.EmploymentNumber = personEmployeeVm.EmploymentNumber;
                        existingRecords = clientDbContext.Employees.Include(x => x.Person).Where(x => x.PersonId == personEmployeeVm.PersonId && x.EmployeeId != personEmployeeVm.EmployeeId).ToList();
                    }
                    //if (existingRecords.Count > 0)
                    //{
                    //    if (existingRecords.Count(x => x.HireDate >= personEmployeeVm.HireDate) > 0)
                    //    {
                    //        return Json(new { succeed = false, Message = "You cannot enter employment records that occur previous to current employment records." }, JsonRequestBehavior.AllowGet);
                    //    }
                    //    else if (existingRecords.Count(x => x.TerminationDate >= personEmployeeVm.TerminationDate) > 0)
                    //    {
                    //        return Json(new { succeed = false, Message = "You cannot enter an employment record unless previous records have a termination date." }, JsonRequestBehavior.AllowGet);
                    //    }
                    //    else if (existingRecords.Count(x => x.HireDate <= personEmployeeVm.HireDate && x.TerminationDate >= personEmployeeVm.HireDate) > 0)
                    //    {
                    //        return Json(new { succeed = false, Message = "New employment cannot fall between dates of an existing employment, current or planned." }, JsonRequestBehavior.AllowGet);
                    //    };
                    //}
                    if (!string.IsNullOrEmpty(personEmployeeVm.code))
                    {
                        int employmentStatusInDb = clientDbContext.DdlEmploymentStatuses.Where(x => x.Code == personEmployeeVm.code).Select(x => x.EmploymentStatusId).SingleOrDefault();

                        if (employmentStatusInDb <= 0)
                        {
                            return Json(new { succeed = false, Message = "The Employee Status does not exist." }, JsonRequestBehavior.AllowGet);
                        }
                        else personEmployee.EmploymentStatusId = employmentStatusInDb;
                    }

                    if (!string.IsNullOrEmpty(personEmployeeVm.EmployeeTypeDescription))
                    {
                        int employeeTypeInDb = clientDbContext.DdlEmployeeTypes.Where(x => x.Description == personEmployeeVm.EmployeeTypeDescription).Select(x => x.EmployeeTypeId).SingleOrDefault();

                        if (employeeTypeInDb <= 0)
                        {
                            return Json(new { succeed = false, Message = "The Employee Types does not exist." }, JsonRequestBehavior.AllowGet);
                        }
                        else personEmployee.EmployeeTypeID = employeeTypeInDb;
                    }

                    if (!string.IsNullOrEmpty(personEmployeeVm.PayFrequencyDescription))
                    {
                        int payFrequencyInDb = clientDbContext.DdlPayFrequencies.Where(x => x.Description == personEmployeeVm.PayFrequencyDescription).Select(x => x.PayFrequencyId).SingleOrDefault();

                        if (payFrequencyInDb <= 0)
                        {
                            return Json(new { succeed = false, Message = "The Pay Frequency does not exist." }, JsonRequestBehavior.AllowGet);
                        }
                        else personEmployee.PayFrequencyId = payFrequencyInDb;
                    }

                    if (!string.IsNullOrEmpty(personEmployeeVm.MaritalStatusDescription))
                    {
                        int maritalStatusInDb = clientDbContext.DdlMaritalStatuses.Where(x => x.Description == personEmployeeVm.MaritalStatusDescription).Select(x => x.MaritalStatusId).SingleOrDefault();

                        if (maritalStatusInDb <= 0)
                        {
                            return Json(new { succeed = false, Message = "The Marital Status does not exist." }, JsonRequestBehavior.AllowGet);
                        }
                        else personEmployee.MaritalStatusID = maritalStatusInDb;
                    }

                    if (!string.IsNullOrEmpty(personEmployeeVm.WorkedStateTitle))
                    {
                        int workedStateTaxCodeInDb = clientDbContext.DdlStates.Where(x => x.Title == personEmployeeVm.WorkedStateTitle).Select(x => x.StateId).SingleOrDefault();

                        if (workedStateTaxCodeInDb <= 0)
                        {
                            return Json(new { succeed = false, Message = "The Worked State Tax Code does not exist." }, JsonRequestBehavior.AllowGet);
                        }
                        else personEmployee.WorkedStateTaxCodeId = workedStateTaxCodeInDb;
                    }

                    if (!string.IsNullOrEmpty(personEmployeeVm.RateTypeDescription))
                    {
                        int rateTypeIdInDb = clientDbContext.DdlRateTypes.Where(x => x.Description == personEmployeeVm.RateTypeDescription).Select(x => x.RateTypeId).SingleOrDefault();

                        if (rateTypeIdInDb <= 0)
                        {
                            return Json(new { succeed = false, Message = "The Rate Type does not exist." }, JsonRequestBehavior.AllowGet);
                        }
                        else personEmployee.RateTypeId = rateTypeIdInDb;
                    }

                    int timeCardTypeId = 0;
                    if (!string.IsNullOrEmpty(personEmployeeVm.TimeCardTypeDescription))
                    {
                        if (!int.TryParse(personEmployeeVm.TimeCardTypeDescription, out timeCardTypeId))
                        {
                            string TimeCardTypeDescription = personEmployeeVm.TimeCardTypeDescription.Substring(personEmployeeVm.TimeCardTypeDescription.IndexOf("-") + 1, (personEmployeeVm.TimeCardTypeDescription.Length - 1) - personEmployeeVm.TimeCardTypeDescription.IndexOf("-"));
                            var timeCardTypeInDb = clientDbContext.DdlTimeCardTypes.Where(x => x.TimeCardTypeDescription == TimeCardTypeDescription.Trim()).SingleOrDefault();

                            if (timeCardTypeInDb == null)
                            {
                                ModelState.AddModelError("", "The Timecard Type does not exist.");
                                return View(personEmployeeVm);
                            }
                            else timeCardTypeId = timeCardTypeInDb.TimeCardTypeId;
                        }
                        personEmployee.TimeCardTypeId = timeCardTypeId;
                    }
                    int result = 0;
                    if (personEmployeeVm.TerminationDate != null && personEmployeeVm.HireDate != null)
                    {
                        result = DateTime.Compare((DateTime)personEmployeeVm.HireDate, (DateTime)personEmployeeVm.TerminationDate);
                    }

                    if (result > 0 && personEmployeeVm.TerminationDate != null && personEmployeeVm.HireDate != null)
                    {
                        return Json(new { succeed = false, Message = "Termination date is less than Hire date." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        personEmployee.HireDate = personEmployeeVm.HireDate;
                        personEmployee.TerminationDate = personEmployeeVm.TerminationDate;
                    }

                    personEmployee.Rate = personEmployeeVm.Rate;
                    personEmployee.Hours = personEmployeeVm.Hours;
                    personEmployee.FileNumber = personEmployeeVm.FileNumber;
                    personEmployee.FedExemptions = personEmployeeVm.FedExemptions;
                    personEmployee.HireDate = personEmployeeVm.HireDate;
                    personEmployee.TerminationDate = personEmployeeVm.TerminationDate;
                    personEmployee.PlannedServiceStartDate = personEmployeeVm.PlannedServiceStartDate;
                    personEmployee.ActualServiceStartDate = personEmployeeVm.ActualServiceStartDate;
                    personEmployee.ProbationEndDate = personEmployeeVm.ProbationEndDate;
                    personEmployee.TrainingEndDate = personEmployeeVm.TrainingEndDate;
                    personEmployee.SeniorityDate = personEmployeeVm.SeniorityDate;
                    personEmployee.EmployeeTypeID = personEmployeeVm.EmployeeTypeID;
                    personEmployee.EmploymentStatusId = recordIsNew==true?1:personEmployeeVm.EmploymentStatusId;
                    personEmployee.MaritalStatusID = personEmployeeVm.MaritalStatusID;
                    personEmployee.PayFrequencyId = personEmployeeVm.PayFrequencyId;
                    personEmployee.PersonId = personEmployeeVm.PersonId;
                    personEmployee.CompanyCodeId = personEmployeeVm.CompanyCodeId;
                    personEmployee.CompanyCode = personEmployeeVm.BusinessLevelNbr.ToString();
                    personEmployee.EmployeeId = personEmployeeVm.EmployeeId;
                    personEmployee.EmploymentNumber = personEmployeeVm.EmploymentNumber;
                    personEmployee.TimeCardTypeId = personEmployeeVm.TimeCardTypeId;
                    personEmployee.DepartmentId = personEmployeeVm.DepartmentCodeId;
                    personEmployee.EarningsCodeId = personEmployeeVm.EarningsCodeId;
                    personEmployee.Amount = personEmployeeVm.Amount;
                    personEmployee.ReportToPersonId = personEmployeeVm.ReportToPersonId;
                    personEmployee.IsManager = personEmployeeVm.IsManager;
                    personEmployee.TreatyLimit = personEmployeeVm.TreatyLimit;
                    personEmployee.UsedAmount = personEmployeeVm.UsedAmount;
                    personEmployee.IsStudent = personEmployeeVm.IsStudent;
                    if (personEmployeeVm.WorkedStateTaxCodeId != null)
                    {
                        personEmployee.WorkedStateTaxCodeId = (byte)personEmployeeVm.WorkedStateTaxCodeId;
                    }
                    if (recordIsNew) clientDbContext.Employees.Add(personEmployee);

                    clientDbContext.SaveChanges();
                    //Terminated all positions once employee got terminated
                    if (personEmployeeVm.code == "T")
                    {
                        if (personEmployeeVm.TerminationDate != null)
                        {
                            clientDbContext.Database.ExecuteSqlCommand("update E_Positions set actualEndDate=@actualEndDate,PrimaryPosition=@PrimaryPosition where Employeeid=@Employeeid and actualEndDate is null", new SqlParameter("@actualEndDate", personEmployeeVm.TerminationDate), new SqlParameter("@PrimaryPosition", false), new SqlParameter("@Employeeid", personEmployeeVm.EmployeeId));
                            clientDbContext.SaveChanges();
                        }
                    }
                    ViewBag.AlertMessage = recordIsNew == true ? "New Employee Record Added" : "Employee Record updated";

                    var personEMail = clientDbContext.Persons.Where(x => x.PersonId == personEmployeeVm.PersonId).Select(x => new { x.eMail, x.SSN, x.Lastname, x.Firstname }).FirstOrDefault();
                    var ssn = personEMail.SSN;
                    var last4DigitSSN = ssn.Substring(ssn.Length - 4, 4);
                    var passwordHash = HttpContext.GetOwinContext().GetUserManager<AppUserManager>().PasswordHasher.HashPassword(last4DigitSSN);
                    var aspNetRoles = clientDbContext.AspNetRoles.Where(x => x.Name == "ClientEmployees").Select(x => x.Id).FirstOrDefault();
                    if (recordIsNew)
                    {
                        var aspNetUsers = (from x in clientDbContext.AspNetUsers where x.UserName == personEMail.eMail select x).FirstOrDefault();
                        if (aspNetUsers == null)
                        {
                            var aspNeUser = new EfClient.AspNetUser();
                            aspNeUser.Id = Guid.NewGuid().ToString();
                            aspNeUser.EmployerId = 3;
                            aspNeUser.LastPasswordChangeDate = DateTime.Today;
                            aspNeUser.Email = personEMail.eMail;
                            aspNeUser.PasswordHash = passwordHash;
                            aspNeUser.SecurityStamp = last4DigitSSN;
                            aspNeUser.UserName = personEMail.eMail;
                            aspNeUser.LastName = personEMail.Lastname;
                            aspNeUser.FirstName = personEMail.Firstname;
                            clientDbContext.AspNetUsers.Add(aspNeUser);
                            clientDbContext.SaveChanges();
                            var aspNeUserId = aspNeUser.Id;

                            var aspNetUserRoles = new EfClient.AspNetUserRole();
                            aspNetUserRoles.UserId = aspNeUserId;
                            aspNetUserRoles.RoleId = aspNetRoles;
                            clientDbContext.AspNetUserRoles.Add(aspNetUserRoles);
                            clientDbContext.SaveChanges();
                        }
                        else
                        {
                            aspNetUsers.EmployerId = 3;
                            aspNetUsers.LastPasswordChangeDate = DateTime.Today;
                            aspNetUsers.Email = personEMail.eMail;
                            aspNetUsers.PasswordHash = passwordHash;
                            aspNetUsers.SecurityStamp = last4DigitSSN;
                            aspNetUsers.UserName = personEMail.eMail;
                            aspNetUsers.LastName = personEMail.Lastname;
                            aspNetUsers.FirstName = personEMail.Firstname;                            
                            clientDbContext.SaveChanges();
                        }                        
                    }
                    personEmployeeVm = _personEmployeeRepository.GetPersonEmployeesRecord(personEmployee.EmployeeId);
                    SessionStateHelper.Set(SessionStateKeys.EMPLOYEE_PERSON_SELECTED_ID, personEmployee.PersonId);
                }
                catch (Exception err)
                {
                    _message = Utils.GetErrorString(err, clientDbContext, null);
                    return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                _message = Utils.GetErrorString(null, null, this.ModelState);
                return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);

            }
            return Json(new { personEmployeeVm, succeed = true }, JsonRequestBehavior.AllowGet);
        }
        public string GetMaritalStatusList()
        {
            var _MaritalStatusList = _personEmployeeRepository.GetMaritalStatusList().CleanUp();

            return JsonConvert.SerializeObject(_MaritalStatusList);
        }

        public string GetEmployeeTypeList()
        {
            var _EmployeeTypeList = _personEmployeeRepository.GetEmployeeTypeList().CleanUp();

            return JsonConvert.SerializeObject(_EmployeeTypeList);
        }

        public string GetEmployeeStatusList()
        {
            var _EmployeeStatusList = _personEmployeeRepository.GetEmployeeStatusList().CleanUp();

            return JsonConvert.SerializeObject(_EmployeeStatusList);
        }

        public string GetEmployeePayFrequencyList()
        {
            var _EmployeePayFrequencyList = _personEmployeeRepository.GetEmployeePayFrequencyList();

            return JsonConvert.SerializeObject(_EmployeePayFrequencyList);
        }

        public string GetPersonsList()
        {
            var _EmployeePayFrequencyList = _personEmployeeRepository.GetPersonsList().CleanUp();

            return JsonConvert.SerializeObject(_EmployeePayFrequencyList);
        }
        public string GetBusinessLevelCodeList()
        {
            var _BusinessLevelCodeList = _personEmployeeRepository.GetBusinessLevelCodeList().CleanUp();

            return JsonConvert.SerializeObject(_BusinessLevelCodeList);
        }

        public string GetWorkedStateList()
        {
            var _WorkedStateList = _personEmployeeRepository.GetWorkedStateList().CleanUp();

            return JsonConvert.SerializeObject(_WorkedStateList);
        }

        public ActionResult PersonEmployeesDeleteAjax(int personTestId)
        {
            ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString());
            var dbRecord = clientDbContext.Employees.Where(x => x.EmployeeId == personTestId).SingleOrDefault();
            if (dbRecord != null)
            {
                _personEmployeeRepository.PersonEmployeesDeleteAjax(personTestId);
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

        public ActionResult GetPersonEmployeesList([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var personEmployeeListNew = new List<PersonEmployeeVm>();

            int personId = User.Identity.GetRequestType() == "IsSelfService" ? clientDbContext.Employees.Where(x => x.Person.eMail == User.Identity.Name).Select(x => x.PersonId).SingleOrDefault()
                : Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.EMPLOYEE_PERSON_SELECTED_ID));
            var companyCodeIdList = clientDbContext.Employees.Where(m => m.PersonId == personId).ToList();
            var PositionList = clientDbContext.PositionBusinessLevels.ToList();

            var businessLevelCodeList = (from cList in PositionList
                                         join emp in companyCodeIdList on cList.BusinessLevelNbr.ToString() equals emp.CompanyCode
                                         select new
                                         {
                                             cList.BusinessLevelCode,
                                             cList.BusinessLevelNbr,
                                             emp.EmploymentNumber
                                         }).ToList();
            var personEmployeeList = _personEmployeeRepository.PersonEmployeeList(personId);

            personEmployeeListNew = (from emp in personEmployeeList
                                     join cList in businessLevelCodeList on
                                     new { EmpNo = emp.EmploymentNumber, ComID = emp.CompanyCode }
                                     equals
                                     new { EmpNo = cList.EmploymentNumber, ComID = cList.BusinessLevelNbr.ToString() }
                                     join ccode in clientDbContext.CompanyCodes on emp.CompanyCodeId equals ccode.CompanyCodeId
                                     select new PersonEmployeeVm
                                     {
                                         EmploymentNumber = emp.EmploymentNumber,
                                         EmployeeTypeID = emp.EmployeeTypeID,
                                         EmployeeId = emp.EmployeeId,
                                         PersonId = emp.PersonId,
                                         PersonName = emp.PersonName,
                                         BusinessLevelNbr = emp.CompanyCodeId,
                                         CompanyCode = ccode.CompanyCodeDescription,
                                         HireDate = emp.HireDate,
                                         TerminationDate = emp.TerminationDate,
                                         EmploymentStatusId = emp.EmploymentStatusId,
                                         EmploymentStatusDescription = emp.EmploymentStatusDescription,
                                         MaritalStatusDescription = emp.MaritalStatusDescription,
                                         PayFrequencyDescription = emp.PayFrequencyDescription,
                                         EmployeeTypeDescription = emp.EmployeeTypeDescription,
                                         PayFrequencyId = emp.PayFrequencyId,
                                         MaritalStatusID = emp.MaritalStatusID,
                                         TimeCardTypeId = emp.TimeCardTypeId,
                                         TimeCardTypeDescription = emp.TimeCardTypeDescription,
                                         IsStudent = emp.IsStudent
                                     }).ToList();

            //  return Json(personEmployeeListNew.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            return KendoCustomResult(personEmployeeListNew.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult PersonEmployees(int employeeId, int personId)
        {
            return View(GetPersonTestsRecord(employeeId, personId));
        }
        public ActionResult PersonEmployeesList(int employeeId, int personId)
        {
            return View(GetPersonTestsRecord(employeeId, personId));
        }
        public PersonEmployeeVm GetPersonTestsRecord(int employeeId, int personId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            PersonEmployeeVm personEmployeeVm = new PersonEmployeeVm();
            var strFed = clientDbContext.Employees.Where(m => m.PersonId == personId && m.EmployeeId == employeeId).Select(m => m.FedExemptions.Trim()).FirstOrDefault();
            var strstatusId = clientDbContext.Employees.Where(m => m.PersonId == personId && m.EmployeeId == employeeId).Select(m => m.EmploymentStatusId).FirstOrDefault();
            var strstatucode = clientDbContext.DdlEmploymentStatuses.Where(m => m.EmploymentStatusId == strstatusId).Select(m => m.Code).FirstOrDefault();
            var companyCodeId = clientDbContext.Employees.Where(m => m.PersonId == personId && m.EmployeeId == employeeId).Select(m => m.CompanyCodeId).FirstOrDefault();
            var companycodeid = Convert.ToString(companyCodeId);
            var businessLevelCode = clientDbContext.PositionBusinessLevels.Where(n => n.BusinessLevelCode == companycodeid).Select(s => s.BusinessLevelNotes).FirstOrDefault();
            var businesslevelnbr= clientDbContext.Employees.Where(n => n.EmployeeId == employeeId).Select(n=>n.CompanyCode).FirstOrDefault();
            personEmployeeVm.PersonId = personId;
            ViewBag.isEdit = employeeId != 0;
            if (personId != 0)
            {
                personEmployeeVm = _personEmployeeRepository.GetPersonTestsRecord(employeeId, personId, businessLevelCode);
            }

            var timeCardTypeId = clientDbContext.Employees.Where(m => m.PersonId == personId && m.EmployeeId == employeeId).Select(m => m.TimeCardTypeId).FirstOrDefault();
            personEmployeeVm.TimeCardTypeId = timeCardTypeId;
            personEmployeeVm.EmployeeId = employeeId;
            personEmployeeVm.FedExemptions = strFed;
            personEmployeeVm.PersonId = personId;
            personEmployeeVm.code = strstatucode;
            var emp = clientDbContext.Employees.Where(m => m.PersonId == personId && m.EmployeeId == employeeId).FirstOrDefault();
            if (emp != null)
            {
                personEmployeeVm.CompanyCode = emp.CompanyCode;
                personEmployeeVm.IsStudent = emp.IsStudent;
                personEmployeeVm.BusinessLevelNbr = Convert.ToInt32(businesslevelnbr);
                personEmployeeVm.CompanyCodeId = emp.CompanyCodeId ?? 0;
                personEmployeeVm.DepartmentCodeId = emp.DepartmentId ?? 0;
                personEmployeeVm.EarningsCodeId = emp.EarningsCodeId ?? 0;
                personEmployeeVm.Amount = emp.Amount;
                personEmployeeVm.TreatyLimit = emp.TreatyLimit;
                personEmployeeVm.NonTreatyLimit = emp.NonTreatyLimit;
                personEmployeeVm.ReportToPersonId = emp.ReportToPersonId;
                personEmployeeVm.UsedAmount = emp.UsedAmount;
                var earnings = clientDbContext.EarningsCodes.Where(m => m.EarningsCodeId == personEmployeeVm.EarningsCodeId).FirstOrDefault();
                if (earnings != null)
                {
                    var treaty = (earnings.TreatyCode) ? "Treaty" : "Non Treaty";
                    personEmployeeVm.EarningsCode = treaty;
                    personEmployeeVm.EarningTreatyCode = earnings.TreatyCode;
                }
                personEmployeeVm.EarningsCodeList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetEarningsCodesList(emp.CompanyCodeId ?? 0));
                personEmployeeVm.DepartmentCodeList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetDepartmentList(emp.CompanyCodeId ?? 0));

            }

            personEmployeeVm.PersonName = clientDbContext.Persons.Where(x => x.PersonId == personId).Select(m => m.Lastname + ", " + m.Firstname).SingleOrDefault();
            personEmployeeVm.EmploymentStatusList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetEmployeeStatusList());
            personEmployeeVm.EmployeeTypeList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetEmployeeTypeList());
            personEmployeeVm.PayFrequencyList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetEmployeePayFrequencyList());
            personEmployeeVm.TimeCardTypeList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetTimeCardTypesList());
            personEmployeeVm.MaritalStatusList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetMaritalStatusList());
            personEmployeeVm.PersonsList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetPersonsList());
            personEmployeeVm.BusinessLevelCodeList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetBusinessLevelCodeList());
            personEmployeeVm.WorkedStateCodeList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetWorkedStateList());


            personEmployeeVm.ReportToList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetReportToList(personId));
            // personEmployeeVm.EarningsCodeList=JsonConvert.DeserializeObject<List<DropDownModel>>(GetEarningsCodesList());

            personEmployeeVm.CompanyCodeList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetCompanyCodeList());
            var earningisdefault = clientDbContext.EarningsCodes.Where(n => n.IsDefault == true && n.CompanyCodeId == companyCodeId).Select(n => n.EarningsCodeId).FirstOrDefault();
            if (employeeId != 0)
            {
                if (earningisdefault == emp.EarningsCodeId)
                {
                    personEmployeeVm.EarningsCodeId = earningisdefault;
                }
                else if (emp.EarningsCodeId == null)
                {
                    personEmployeeVm.EarningsCodeId = earningisdefault;
                }
                else
                {
                    personEmployeeVm.EarningsCodeId = emp.EarningsCodeId;
                }
            }
            return personEmployeeVm;
        }
        public JsonResult GetRateTypes(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var rateTypes = clientDbContext.DdlRateTypes
                    .Select(m => new
                    {
                        RateTypeId = m.RateTypeId,
                        RateTypeDescription = m.Description,
                    }).OrderBy(x => x.RateTypeDescription).ToList();

                return Json(rateTypes, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetTimeCardTypes(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var timeCardTypes = clientDbContext.DdlTimeCardTypes
                    .Select(m => new
                    {
                        TimeCardTypeId = m.TimeCardTypeId,
                        TimeCardTypeDescription = m.TimeCardTypeCode + "-" + m.TimeCardTypeDescription,
                    }).OrderBy(x => x.TimeCardTypeDescription).ToList();

                return Json(timeCardTypes, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddEmploymentStatus()
        {
            var model = new DdlEmploymentStatus() { Active = true };
            return View(model);
        }
        public JsonResult GetEmployeStatusList()
        {
            var connString = User.Identity.GetClientConnectionString();
            var clientDbContext = new ClientDbContext(connString);
            var employeeStatusList = clientDbContext.DdlEmploymentStatuses.Where(m => m.Active == true).ToList();

            return Json(employeeStatusList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DdlEmploymentStatusList_Create([DataSourceRequest] DataSourceRequest request, DdlEmploymentStatus employmentStatus)
        {
            if (employmentStatus != null && ModelState.IsValid)
            {
                var connString = User.Identity.GetClientConnectionString();
                var clientDbContext = new ClientDbContext(connString);
                var employmentStatusInDb = clientDbContext.DdlEmploymentStatuses.Where(x => x.Code == employmentStatus.Code || x.Description == employmentStatus.Description).SingleOrDefault();

                if (employmentStatusInDb != null)
                {
                    ModelState.AddModelError("", "Employment Status" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                }
                else
                {
                    var newemploymentStatus = new DdlEmploymentStatus { Description = employmentStatus.Description, Code = employmentStatus.Code, Active = true };
                    clientDbContext.DdlEmploymentStatuses.Add(newemploymentStatus);
                    clientDbContext.SaveChanges();
                    employmentStatus.EmploymentStatusId = newemploymentStatus.EmploymentStatusId;
                    employmentStatus.Code = newemploymentStatus.Code;
                    employmentStatus.Description = newemploymentStatus.Description;
                    employmentStatus.Active = newemploymentStatus.Active;
                }
            }

            return Json(new[] { employmentStatus }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult AddEmployeeType()
        {
            var model = new DdlEmployeeType() { Active = true };
            return View(model);
        }


        public ActionResult DdlEmployeeTypesList_Create([DataSourceRequest] DataSourceRequest request
           , ExecViewHrk.EfClient.DdlEmployeeType employeeType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (employeeType != null && ModelState.IsValid)
                {
                    var employeeTypeInDb = clientDbContext.DdlEmployeeTypes
                        .Where(x => x.Code == employeeType.Code || x.Description == employeeType.Description)
                        .SingleOrDefault();

                    if (employeeTypeInDb != null)
                    {
                        ModelState.AddModelError("", "The Employee type is already defined.");
                    }
                    else
                    {
                        var newEmployeeType = new DdlEmployeeType
                        {
                            Description = employeeType.Description,
                            Code = employeeType.Code,
                            Active = true
                        };

                        clientDbContext.DdlEmployeeTypes.Add(newEmployeeType);
                        clientDbContext.SaveChanges();
                        employeeType.EmployeeTypeId = newEmployeeType.EmployeeTypeId;
                        employeeType.Description = newEmployeeType.Description;
                        employeeType.Code = newEmployeeType.Code;
                        employeeType.Active = newEmployeeType.Active;
                    }
                }

                return Json(new[] { employeeType }.ToDataSourceResult(request, ModelState));
            }
        }
        public JsonResult GetEmployeTypeList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var employeeTypeList = clientDbContext.DdlEmployeeTypes.Where(m => m.Active == true)
                .Select(m => new DropDownModel { keyvalue = m.EmployeeTypeId.ToString(), keydescription = m.Description }).ToList();

            return Json(employeeTypeList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPayFrequencies()
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var payFrequencyTypes = clientDbContext.DdlPayFrequencies.Where(x => x.Active == true).ToList();

                return Json(payFrequencyTypes, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddPersons()
        {
            return View();
        }
        public ActionResult Persons_Create([DataSourceRequest]DataSourceRequest request, Person personList)
        {
            string constring = User.Identity.GetClientConnectionString();
            using (var clientDbContext = new ClientDbContext(constring))
            {
                if (personList != null && ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(personList.Firstname) && !string.IsNullOrEmpty(personList.Lastname) && !string.IsNullOrEmpty(personList.eMail))
                    {

                        var isExist = clientDbContext.Persons.Any(r => r.eMail == personList.eMail);
                        if (!isExist)
                        {
                            //var getMaxSsn = clientDbContext.Persons.Max(s => s.SSN);
                            var getMaxPersonId = clientDbContext.Persons.Max(PID => PID.PersonId);
                          //  int getMaxSn = Convert.ToInt32(getMaxSsn) + 1;
                            var setEmail = personList.eMail; //(personList.Firstname + "" + personList.Lastname + getMaxPersonId + "@execview.com").ToLower();
                            var newssn = personList.SSN.Replace("-", "").Trim();                      
                            var newPersonDetails = new Person { Firstname = personList.Firstname, Lastname = personList.Lastname, /*SSN = Convert.ToString(getMaxSn)*/SSN= newssn, eMail = setEmail };

                            var duplicatessn = clientDbContext.Persons.Where(x=>x.SSN== newssn).FirstOrDefault();
                            if(duplicatessn!=null)
                            {
                                ModelState.AddModelError("", "SSN is already exist.");
                            }
                            clientDbContext.Persons.Add(newPersonDetails);
                            clientDbContext.SaveChanges();
                            personList.PersonId = newPersonDetails.PersonId;
                            personList.Firstname = newPersonDetails.Firstname;
                            personList.Lastname = newPersonDetails.Lastname;
                            personList.eMail = newPersonDetails.eMail;
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Email id is already exist.");
                        }
                    }
                }
                return Json(new[] { personList }.ToDataSourceResult(request, ModelState));
            }
        }

        public string GetTimeCardTypesList()
        {
            var timecardTypesList = _personEmployeeRepository.GetTimeCardTypesList();
            return JsonConvert.SerializeObject(timecardTypesList);
        }

        public string GetDepartmentList(int companyCode)
        {
            var departmentsList = _personEmployeeRepository.GetDepartmentsList(companyCode).CleanUp();

            return JsonConvert.SerializeObject(departmentsList);
        }

        public string GetReportToList(int reportId)
        {
            var reportToList = _personEmployeeRepository.GetReportsToList(reportId).CleanUp();

            return JsonConvert.SerializeObject(reportToList);
        }


        [HttpGet]
        public JsonResult GetDepartments(int companycodeid)
        {
            var _DepartmentList = GetDepartmentList(companycodeid);
            var departments = JsonConvert.DeserializeObject<List<DropDownModel>>(_DepartmentList);

            var earningsCodesList = GetEarningsCodesList(companycodeid);
            var earnings = JsonConvert.DeserializeObject<List<DropDownModel>>(earningsCodesList);
            var list = new { departments, earnings };
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public string GetCompanyCodeList()
        {
            var departmentsList = _personEmployeeRepository.GetCompanyCodeList();

            return JsonConvert.SerializeObject(departmentsList);
        }

        public string GetEarningsCodesList(int comapnyCode)
        {
            var earningsCodesList = _personEmployeeRepository.GetEarningsCodesList(comapnyCode).CleanUp();

            return JsonConvert.SerializeObject(earningsCodesList);
        }
        public JsonResult OnChangeEarningsCodes(int earningCodeId)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var earningsCodes = clientDbContext.EarningsCodes.Where(r => r.EarningsCodeId == earningCodeId).Select(r=>r.EarningsCodeDescription).FirstOrDefault();
                return Json(earningsCodes, JsonRequestBehavior.AllowGet);
            }
        }
    }
}