using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExecViewHrk.WebUI.Infrastructure;
using ExecViewHrk.WebUI.Models;
using ExecViewHrk.EfAdmin;
using ExecViewHrk.Domain.Helper;
using System.Configuration;
using ExecViewHrk.EfClient;
using System.Data.Entity.Validation;
using Newtonsoft.Json;
using ExecViewHrk.Models;
using ExecViewHrk.Domain.Interface;

namespace ExecViewHrk.WebUI.Controllers
{
    [Authorize]
    public class HrkAdminController : Controller
    {
        // GET: HrkAdmin
        private readonly IHrkAdminRepository _hrkAdminRepository;
        public HrkAdminController(IHrkAdminRepository hrkAdminRepository)
        {
            _hrkAdminRepository = hrkAdminRepository;
        }
        public ActionResult Index()
        {
            return View();
        }

        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser { UserName = model.Name, Email = model.Email, LastPasswordChangeDate = DateTime.Today };
                IdentityResult result = await UserManager.CreateAsync(user,
                    model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            AppUser user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View("Error", result.Errors);
                }
            }
            else
            {
                return View("Error", new string[] { "User Not Found" });
            }
        }


        [HttpPost]
        public async Task<ActionResult> Edit(string id, string email, string password)
        {
            AppUser user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                user.Email = email;

                IdentityResult validEmail
                    = await UserManager.UserValidator.ValidateAsync(user);
                if (!validEmail.Succeeded)
                {
                    AddErrorsFromResult(validEmail);
                }
                IdentityResult validPass = null;
                if (password != string.Empty)
                {
                    validPass
                        = await UserManager.PasswordValidator.ValidateAsync(password);
                    if (validPass.Succeeded)
                    {
                        user.PasswordHash =
                            UserManager.PasswordHasher.HashPassword(password);
                    }
                    else
                    {
                        AddErrorsFromResult(validPass);
                    }
                }
                if ((validEmail.Succeeded && validPass == null) || (validEmail.Succeeded
                        && password != string.Empty && validPass.Succeeded))
                {
                    IdentityResult result = await UserManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        AddErrorsFromResult(result);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            return View(user);
        }

        public ActionResult ViewErrorLog()
        {
            return View();
        }

        public ActionResult ErrorLog_Read([DataSourceRequest]DataSourceRequest request)
        {
            AdminDbContext adminDbContext = new AdminDbContext();

            var errorLogList = adminDbContext.ErrorLogs.OrderByDescending(e => e.LogDate).ToList();

            return Json(errorLogList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewMessageLog()
        {
            return View();
        }
        public ActionResult MessageLog_Read([DataSourceRequest]DataSourceRequest request)
        {
            AdminDbContext adminDbContext = new AdminDbContext();

            var messageLogList = adminDbContext.MessageLogs.OrderByDescending(e => e.DateTime).ToList();

            return Json(messageLogList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SendEmailFromMessageLog(EmailMessageLogModel emailMessageLogModel)
        {
            EmailProcessor emailProcessor = new EmailProcessor();
            emailProcessor.Send("webtimeadmin@resnav.com", emailMessageLogModel.To, emailMessageLogModel.Subject, emailMessageLogModel.Body);
            return Json("Email message has been sent.", JsonRequestBehavior.AllowGet);
        }

        public ActionResult EmployerListMaintenance()
        {
            return View();
        }

        // use this for Grid
        public ActionResult EmployerList_Read([DataSourceRequest]DataSourceRequest request)
        {
            AdminDbContext adminDbContext = new AdminDbContext();

            var employerList = adminDbContext.Employers.OrderBy(e => e.EmployerName).ToList();

            return Json(employerList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        // use this for ddl
        public JsonResult EmployersDdlList_Read(bool showAll = true)
        {
            AdminDbContext adminDbContext = new AdminDbContext();

            if (User.IsInRole("HrkAccountManagers"))
                showAll = false;

            string userId = User.Identity.GetUserId();
            var reqEmployerList = adminDbContext.Employers.Where(x => x.EmployerName == "HR Knowledge").ToList();
            reqEmployerList[0].EmployerName = "Drew University";
            //var reqEmployerList = adminDbContext.Employers.Where(x=>x.EmployerName.Replace("HR Knowledge", "Drew University")== "Drew University").ToList()
            var employerList = showAll ? reqEmployerList
                : adminDbContext.AspNetUsers.Where(x => x.Id == userId)
                        .Include(x => x.Employers).SelectMany(x => x.Employers).ToList();

            if (User.Identity.GetSelectedClientID() == null || User.Identity.GetSelectedClientID() == "0")
            {
                User.Identity.AddUpdateClaim(SessionStateKeys.SELECTED_CLIENT_ID.ToString(), employerList[0].EmployerId.ToString());
            }

            return Json(employerList, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployerList_Create([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfAdmin.Employer employer)
        {

            if (employer != null && ModelState.IsValid)
            {
                AdminDbContext adminDbContext = new AdminDbContext();
                var employerInDb = adminDbContext.Employers
                    .Where(x => x.EmployerName == employer.EmployerName)
                    .SingleOrDefault();

                if (employerInDb != null)
                {
                    ModelState.AddModelError("", "The Employer name is already defined.");
                }
                else
                {
                    var newEmployer = new Employer
                    {
                        EmployerName = employer.EmployerName
                        ,
                        DatabaseName = employer.DatabaseName,
                        CreatedDate = DateTime.Now,
                        IsClient = employer.IsClient,
                        CreatedBy = User.Identity.Name
                    };
                    adminDbContext.Employers.Add(newEmployer);
                    adminDbContext.SaveChanges();
                    employer.EmployerId = newEmployer.EmployerId;
                }
            }

            return Json(new[] { employer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployerList_Update([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfAdmin.Employer employer)
        {
            if (employer != null && ModelState.IsValid)
            {
                AdminDbContext adminDbContext = new AdminDbContext();
                var employerInDb = adminDbContext.Employers
                    .Where(x => x.EmployerId == employer.EmployerId)
                    .SingleOrDefault();

                if (employerInDb != null)
                {
                    employerInDb.EmployerName = employer.EmployerName;
                    employerInDb.DatabaseName = employer.DatabaseName;
                    employerInDb.IsClient = employer.IsClient;
                    employer.ModifiedBy = User.Identity.Name;
                    employer.ModifiedDate = DateTime.Now;
                    adminDbContext.SaveChanges();
                }
            }

            return Json(new[] { employer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployerList_Destroy([DataSourceRequest] DataSourceRequest request, Employer employer)
        {
            if (employer != null)
            {
                AdminDbContext adminDbContext = new AdminDbContext();
                Employer employerInDb = adminDbContext.Employers.Where(x => x.EmployerId == employer.EmployerId).SingleOrDefault();

                if (employerInDb != null)
                {
                    adminDbContext.Employers.Remove(employerInDb);
                    adminDbContext.SaveChanges();
                }
            }

            return Json(new[] { employer }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult UsersListMaintenance()
        {
            PopulateEmployers();
            return View();
        }
        public ActionResult UsersList_Read([DataSourceRequest]DataSourceRequest request)
        {
            AdminDbContext adminDbContext = new AdminDbContext();
            //int EmployerId = 0;
            var employerId = adminDbContext.Employers.Where(x => x.EmployerName == "HR Knowledge").Select(x => x.EmployerId).SingleOrDefault();

            //var employeeIdList = adminDbContext.AspNetUsers
            //                        .ToList();
            var employeeIdList = adminDbContext.AspNetUsers.Where(x => x.EmployerId == employerId).ToList();
            // var userEmployerNameList = adminDbContext.Employers.ToList();
            var userEmployerNameList = adminDbContext.Employers.Where(x => x.EmployerId == employerId).ToList();
            //var list1 = adminDbContext.AspNetUsers.ToList();
            var list1 = adminDbContext.AspNetUsers.Where(x => x.EmployerId == employerId).ToList();
            var employeeList = (from x in userEmployerNameList
                                join emp1 in list1 on x.EmployerId equals (int)emp1.EmployerId
                                select new
                                {
                                    emp1.UserName,
                                    emp1.Id,
                                    emp1.LastName,
                                    emp1.FirstName,
                                    emp1.EmployerId,
                                    emp1.LastPasswordChangeDate,
                                    x.EmployerName
                                });
            //return Json(employeeList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            return Json(employeeList.ToList().OrderBy(x => x.LastName).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);


        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> UsersList_Create([DataSourceRequest] DataSourceRequest request, UserViewModel userViewModel)
        {
            //AdminDbContext adminDbContext = new AdminDbContext();
            ////var person = adminDbContext.AspNetUsers.Where(x => x.UserName == userViewModel.UserName).FirstOrDefault();
            //var employer = adminDbContext.Employers.Where(x => x.EmployerId == userViewModel.EmployerId).SingleOrDefault();

            //if (employer.IsClient)
            //{

            //    string adminConnString = ConfigurationManager.ConnectionStrings["AdminDbContext"].ConnectionString;
            //    if (!adminConnString.Contains("ExecViewHrk")) // sanity check
            //    {
            //        ModelState.AddModelError("", "Admin Database name not set to ExecViewHrk. Request cannot be made.");
            //        return Json(new[] { userViewModel }.ToDataSourceResult(request, ModelState));
            //    }

            //    string clientConnString = adminConnString.Replace("ExecViewHrk", employer.DatabaseName);

            //    ClientDbContext clientDbContext = new ClientDbContext(clientConnString);
            //    if (!clientDbContext.Database.Exists())
            //    {
            //        ModelState.AddModelError("", "Database does not exist. for database = " + employer.DatabaseName);
            //        return Json(new[] { userViewModel }.ToDataSourceResult(request, ModelState));
            //    }
            //    else
            //    {
            //        try
            //        {
            //            var clientDbPerson = clientDbContext.Persons.Where(x => x.eMail == userViewModel.UserName).FirstOrDefault(); // check to see we can access the database
            //            if (clientDbPerson != null)
            //            {
            //                ModelState.AddModelError("", "The person with username " + userViewModel.UserName + " already exists in the for database = " + employer.DatabaseName);
            //                return Json(new[] { userViewModel }.ToDataSourceResult(request, ModelState));
            //            }
            //        }
            //        catch (Exception err)
            //        {
            //            ModelState.AddModelError("", "Cannot access database = " + employer.DatabaseName);
            //            return Json(new[] { userViewModel }.ToDataSourceResult(request, ModelState));
            //        }
            //    }

            //}






            //return Json(new[] { userViewModel }.ToDataSourceResult(request, ModelState));

            try
            {
                if (ModelState.IsValid)
                {
                    AppUser user = new AppUser
                    {
                        UserName = userViewModel.UserName,
                        Email = userViewModel.UserName
                        ,
                        LastPasswordChangeDate = DateTime.Today,
                        EmployerId = userViewModel.EmployerId
                        ,
                        LastName = userViewModel.LastName,
                        FirstName = userViewModel.FirstName
                    };

                    IdentityResult result = await UserManager.CreateAsync(user,
                        userViewModel.Password);

                    if (!result.Succeeded)
                    {
                        AddErrorsFromResult(result);
                    }
                }
                else
                    ModelState.AddModelError("", "The data returned is invalid. The requested action cannot be made at this time.");

            }
            catch (Exception err)
            {
                ModelState.AddModelError("", "The requested action cannot be made at this time." + err.Message);
            }

            return Json(new[] { userViewModel }.ToDataSourceResult(request, ModelState));

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> UsersList_Update([DataSourceRequest] DataSourceRequest request, UserViewModel userViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AppUser user = await UserManager.FindByIdAsync(userViewModel.UserId);
                    if (user != null)
                    {
                        user.Email = userViewModel.UserName;
                        user.LastName = userViewModel.LastName;
                        user.FirstName = userViewModel.FirstName;
                        user.UserName = userViewModel.UserName;
                        IdentityResult validEmail = await UserManager.UserValidator.ValidateAsync(user);
                        if (!validEmail.Succeeded)
                        {
                            AddErrorsFromResult(validEmail);
                        }

                        IdentityResult validPass = null;
                        validPass = await UserManager.PasswordValidator.ValidateAsync(userViewModel.Password);
                        if (validPass.Succeeded)
                        {
                            user.PasswordHash = UserManager.PasswordHasher.HashPassword(userViewModel.Password);
                        }
                        else
                        {
                            AddErrorsFromResult(validPass);
                        }

                        if ((validEmail.Succeeded && validPass.Succeeded))
                        {
                            IdentityResult result = await UserManager.UpdateAsync(user);
                            if (!result.Succeeded)
                            {
                                AddErrorsFromResult(result);
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "User Not Found");
                    }
                }
                else
                    ModelState.AddModelError("", "The data returned is invalid. The requested action cannot be made at this time.");

            }
            catch (Exception err)
            {
                ModelState.AddModelError("", "The requested action cannot be made at this time." + err.Message);
            }

            return Json(new[] { userViewModel }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> UsersList_Destroy([DataSourceRequest] DataSourceRequest request, UserViewModel userViewModel)
        {
            try
            {
                ModelState.Clear();
                if (ModelState.IsValid)
                {
                    AppUser user = await UserManager.FindByIdAsync(userViewModel.UserId);
                    if (user != null)
                    {
                        IdentityResult result = await UserManager.DeleteAsync(user);
                        if (!result.Succeeded)
                        {
                            AddErrorsFromResult(result);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "User not found.");
                    }
                }
                else
                    ModelState.AddModelError("", "The data returned is invalid. The requested action cannot be made at this time.");
            }
            catch (Exception err)
            {
                ModelState.AddModelError("", "The requested action cannot be made at this time." + err.Message);
            }

            return Json(new[] { userViewModel }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult UserRolesAssignmentListMaintenance()
        {
            PopulateRoles();
            PopulateUsers();
            return View();
        }

        public ActionResult UserRolesAssignmentList_Read([DataSourceRequest]DataSourceRequest request)
        {
            AdminDbContext adminDbContext = new AdminDbContext();

            var userWithRoles = adminDbContext.AspNetUsers.Include(x => x.AspNetRoles);
            List<UserRoleAssignmentViewModel> uraList = new List<UserRoleAssignmentViewModel>();
            foreach (var user in userWithRoles)
            {
                foreach (var role in user.AspNetRoles)
                {
                    UserRoleAssignmentViewModel ura = new UserRoleAssignmentViewModel
                    { UserName = user.UserName, RoleName = role.Name };

                    uraList.Add(ura);
                }
            }

            return Json(uraList.ToList().OrderBy(x => x.UserName).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> UserRolesAssignmentList_Create([DataSourceRequest] DataSourceRequest request, UserRoleAssignmentViewModel userRoleAssignmentViewModel)
        {
            try
            {
                IdentityResult result;
                if (ModelState.IsValid)
                {
                    AppUser user = await UserManager.FindByNameAsync(userRoleAssignmentViewModel.UserName);
                    if (user != null)
                    {
                        result = await UserManager.AddToRoleAsync(user.Id, userRoleAssignmentViewModel.RoleName);
                        if (!result.Succeeded)
                        {
                            AddErrorsFromResult(result);
                        }
                        else
                        {
                            if (userRoleAssignmentViewModel.RoleName == "ClientManagers")
                            {
                                string strConnection = ConfigurationManager.ConnectionStrings["execView1"].ConnectionString;
                                var clientDbContext = new ClientDbContext(strConnection);
                                var personId = clientDbContext.Persons.Where(x => x.eMail == userRoleAssignmentViewModel.UserName).Select(x => x.PersonId).FirstOrDefault();
                                var managerId = clientDbContext.Managers.Where(x => x.PersonId == personId).Select(x => x.ManagerId).FirstOrDefault();
                                if (managerId == 0)
                                {
                                    var manager = new Manager();
                                    manager.PersonId = personId;
                                    clientDbContext.Managers.Add(manager);
                                    clientDbContext.SaveChanges();
                                }
                                var managerDepartmentList = (from ee in clientDbContext.E_Positions
                                                             join emp in clientDbContext.Employees on ee.EmployeeId equals emp.EmployeeId
                                                             join per in clientDbContext.Persons on emp.PersonId equals per.PersonId
                                                             where ee.ReportsToID == personId
                                                             select new
                                                             {
                                                                 ee.DepartmentId,
                                                                 per.eMail,
                                                                 emp.CompanyCodeId
                                                             }).ToList();
                                if (managerDepartmentList.Count > 0)
                                {
                                    foreach (var item in managerDepartmentList)
                                    {
                                        var chkAspNetUsersExist = clientDbContext.AspNetUsers.Where(x => x.UserName == item.eMail).FirstOrDefault();
                                        if (chkAspNetUsersExist != null)
                                        {
                                            var chkDepartmentExist = clientDbContext.Departments.Where(x => x.DepartmentId == item.DepartmentId).FirstOrDefault();
                                            if (chkDepartmentExist != null)
                                            {
                                                var chkManagerDepartmentExist = clientDbContext.ManagerDepartments.Where(x => x.ManagerId == managerId && x.DepartmentId == item.DepartmentId && x.CompanyCodeId == item.CompanyCodeId).FirstOrDefault();
                                                if (chkManagerDepartmentExist == null)
                                                {
                                                    var managerDepartment = new ManagerDepartment();
                                                    managerDepartment.ManagerId = managerId;
                                                    managerDepartment.DepartmentId = item.DepartmentId;
                                                    managerDepartment.CompanyCodeId = Convert.ToInt32(item.CompanyCodeId);
                                                    clientDbContext.ManagerDepartments.Add(managerDepartment);
                                                    clientDbContext.SaveChanges();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "User not found.");
                    }
                }
                else
                    ModelState.AddModelError("", "The data returned is invalid. The requested action cannot be made at this time.");
            }
            catch (Exception err)
            {
                ModelState.AddModelError("", "The requested action cannot be made at this time." + err.Message);
            }

            return Json(new[] { userRoleAssignmentViewModel }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> UserRolesAssignmentList_Update([DataSourceRequest] DataSourceRequest request, UserRoleAssignmentViewModel userRoleAssignmentViewModel)
        {
            ModelState.AddModelError("", "Edit's are not allowed at this time. To make a change, delete the record and add a new one.");
            return Json(new[] { userRoleAssignmentViewModel }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> UserRolesAssignmentList_Destroy([DataSourceRequest] DataSourceRequest request, UserRoleAssignmentViewModel userRoleAssignmentViewModel)
        {
            try
            {
                IdentityResult result;
                if (ModelState.IsValid)
                {
                    AppUser user = await UserManager.FindByNameAsync(userRoleAssignmentViewModel.UserName);
                    if (user != null)
                    {
                        result = await UserManager.RemoveFromRoleAsync(user.Id, userRoleAssignmentViewModel.RoleName);
                        if (!result.Succeeded)
                        {
                            AddErrorsFromResult(result);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "User not found.");
                    }
                }
                else
                    ModelState.AddModelError("", "The data returned is invalid. The requested action cannot be made at this time.");
            }
            catch (Exception err)
            {
                ModelState.AddModelError("", "The requested action cannot be made at this time." + err.Message);
            }

            return Json(new[] { userRoleAssignmentViewModel }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult ClientAssignments()
        {
            PopulateEmployers();
            PopulateUsers();

            return View();
        }

        public ActionResult ClientAssignmentList_Read([DataSourceRequest]DataSourceRequest request)
        {
            AdminDbContext adminDbContext = new AdminDbContext();

            var usersWithClients = adminDbContext.AspNetUsers.Include(x => x.Employers).Where(x => x.Employers.Count > 0);

            List<ClientAssignmentViewModel> clientAssignmentList = new List<ClientAssignmentViewModel>();
            foreach (var user in usersWithClients)
            {
                foreach (var client in user.Employers)
                {
                    ClientAssignmentViewModel caVm = new ClientAssignmentViewModel { UserName = user.UserName, EmployerName = client.EmployerName };

                    clientAssignmentList.Add(caVm);
                }
            }

            return Json(clientAssignmentList.ToList().OrderBy(x => x.UserName).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> ClientAssignmentList_Create([DataSourceRequest] DataSourceRequest request
            , ClientAssignmentViewModel clientAssignmentViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AdminDbContext adminDbContext = new AdminDbContext();

                    EfAdmin.AspNetUser user = adminDbContext.AspNetUsers.Where(x => x.UserName == clientAssignmentViewModel.UserName).SingleOrDefault();
                    Employer employer = adminDbContext.Employers.Where(x => x.EmployerName == clientAssignmentViewModel.EmployerName).SingleOrDefault();
                    user.Employers.Add(employer);
                    adminDbContext.SaveChanges();

                }
                else
                    ModelState.AddModelError("", "The data returned is invalid. The requested action cannot be made at this time.");

            }
            catch (Exception err)
            {
                ModelState.AddModelError("", "The requested action cannot be made at this time." + err.Message);
            }

            return Json(new[] { clientAssignmentViewModel }.ToDataSourceResult(request, ModelState));

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> ClientAssignmentList_Update([DataSourceRequest] DataSourceRequest request,
            ClientAssignmentViewModel clientAssignmentViewModel)
        {

            ModelState.AddModelError("", "Edit's are not allowed at this time. To make a change, delete the record and add a new one.");
            return Json(new[] { clientAssignmentViewModel }.ToDataSourceResult(request, ModelState));

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> ClientAssignmentList_Destroy(
            [DataSourceRequest] DataSourceRequest request, ClientAssignmentViewModel clientAssignmentViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AdminDbContext adminDbContext = new AdminDbContext();

                    EfAdmin.AspNetUser user = adminDbContext.AspNetUsers.Include(x => x.Employers)
                        .Where(x => x.UserName == clientAssignmentViewModel.UserName).SingleOrDefault();

                    var employer = user.Employers.Where(x => x.EmployerName == clientAssignmentViewModel.EmployerName).SingleOrDefault();

                    user.Employers.Remove(employer);

                    adminDbContext.SaveChanges();
                }
                else
                    ModelState.AddModelError("", "The data returned is invalid. The requested action cannot be made at this time.");

            }
            catch (Exception err)
            {
                ModelState.AddModelError("", "The requested action cannot be made at this time." + err.Message);
            }

            return Json(new[] { clientAssignmentViewModel }.ToDataSourceResult(request, ModelState));

        }
        public ActionResult EmployerCompanyAssignments()
        {
            PopulateEmployers();
            PopulateUsers();

            return View();
        }

        public ActionResult EmployerCompanyAssignmentList_Read([DataSourceRequest]DataSourceRequest request)
        {
            AdminDbContext adminDbContext = new AdminDbContext();

            var usersWithCompanies = adminDbContext.AspNetUsers
                .Include(x => x.UserCompanies.Select(y => y.Employer)).Where(z => z.UserCompanies.Count > 0);

            List<EmployerCompanyAssignmentViewModel> companyAssignmentList = new List<EmployerCompanyAssignmentViewModel>();
            foreach (var user in usersWithCompanies)
            {
                foreach (var company in user.UserCompanies)
                {
                    EmployerCompanyAssignmentViewModel caVm = new EmployerCompanyAssignmentViewModel
                    { UserName = user.UserName, EmployerName = company.Employer.EmployerName, UserId = user.Id, EmployerId = company.EmployerId };

                    companyAssignmentList.Add(caVm);
                }
            }

            return Json(companyAssignmentList.ToList().OrderBy(x => x.UserName).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> EmployerCompanyAssignmentList_Create([DataSourceRequest] DataSourceRequest request
            , EmployerCompanyAssignmentViewModel companyAssignmentViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AdminDbContext adminDbContext = new AdminDbContext();

                    EfAdmin.AspNetUser user = adminDbContext.AspNetUsers.Where(x => x.UserName == companyAssignmentViewModel.UserName).SingleOrDefault();
                    Employer employer = adminDbContext.Employers.Where(x => x.EmployerName == companyAssignmentViewModel.EmployerName).SingleOrDefault();

                    EfAdmin.UserCompany userCompany = new EfAdmin.UserCompany { UserId = user.Id, EmployerId = employer.EmployerId };
                    user.UserCompanies.Add(userCompany);
                    adminDbContext.SaveChanges();

                }
                else
                    ModelState.AddModelError("", "The data returned is invalid. The requested action cannot be made at this time.");

            }
            catch (Exception err)
            {
                ModelState.AddModelError("", "The requested action cannot be made at this time." + err.Message);
            }

            return Json(new[] { companyAssignmentViewModel }.ToDataSourceResult(request, ModelState));

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> EmployerCompanyAssignmentList_Update([DataSourceRequest] DataSourceRequest request,
            EmployerCompanyAssignmentViewModel employerCompanyAssignmentViewModel)
        {

            ModelState.AddModelError("", "Edit's are not allowed at this time. To make a change, delete the record and add a new one.");
            return Json(new[] { employerCompanyAssignmentViewModel }.ToDataSourceResult(request, ModelState));

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> EmployerCompanyAssignmentList_Destroy(
            [DataSourceRequest] DataSourceRequest request, EmployerCompanyAssignmentViewModel employerCompanyAssignmentViewModel)
        {
            AdminDbContext adminDbContext = new AdminDbContext();
            try
            {
                if (ModelState.IsValid)
                {
                    EfAdmin.UserCompany userCompany = adminDbContext.UserCompanies
                        .Where(x => x.EmployerId == employerCompanyAssignmentViewModel.EmployerId &&
                            x.UserId == employerCompanyAssignmentViewModel.UserId).SingleOrDefault();

                    adminDbContext.UserCompanies.Remove(userCompany);

                    adminDbContext.SaveChanges();

                }
                else
                    ModelState.AddModelError("", "The data returned is invalid. The requested action cannot be made at this time.");

            }
            catch (Exception err)
            {

                IEnumerable<DbEntityValidationResult> errors = adminDbContext.GetValidationErrors();
                if (errors.Count() == 0)
                {

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

                ModelState.AddModelError("", "The requested action cannot be made at this time." + err.Message);
            }

            return Json(new[] { employerCompanyAssignmentViewModel }.ToDataSourceResult(request, ModelState));

        }
        public ActionResult ExternalUserClientAssignments()
        {
            PopulateEmployers();
            PopulateUsers();

            return View();
        }

        public ActionResult ExternalUserClientAssignmentList_Read([DataSourceRequest]DataSourceRequest request)
        {
            AdminDbContext adminDbContext = new AdminDbContext();

            var externalUsersWithClients = adminDbContext.AspNetUsers
                .Include(x => x.ExternalUserClients.Select(y => y.Employer)).Where(z => z.ExternalUserClients.Count > 0);

            List<ExternalUserClientAssignmentViewModel> externalUserAssignmentList = new List<ExternalUserClientAssignmentViewModel>();
            foreach (var user in externalUsersWithClients)
            {
                foreach (var client in user.ExternalUserClients)
                {
                    ExternalUserClientAssignmentViewModel caVm =
                        new ExternalUserClientAssignmentViewModel
                        { UserName = user.UserName, EmployerName = client.Employer.EmployerName, UserId = user.Id, EmployerId = client.EmployerId };

                    externalUserAssignmentList.Add(caVm);
                }
            }

            return Json(externalUserAssignmentList.ToList().OrderBy(x => x.UserName).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> ExternalUserClientAssignmentList_Create([DataSourceRequest] DataSourceRequest request
            , ExternalUserClientAssignmentViewModel clientAssignmentViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AdminDbContext adminDbContext = new AdminDbContext();

                    EfAdmin.AspNetUser user = adminDbContext.AspNetUsers.Where(x => x.UserName == clientAssignmentViewModel.UserName).SingleOrDefault();
                    Employer employer = adminDbContext.Employers.Where(x => x.EmployerName == clientAssignmentViewModel.EmployerName).SingleOrDefault();

                    EfAdmin.ExternalUserClient userClient = new EfAdmin.ExternalUserClient { UserId = user.Id, EmployerId = employer.EmployerId };
                    user.ExternalUserClients.Add(userClient);
                    adminDbContext.SaveChanges();

                }
                else
                    ModelState.AddModelError("", "The data returned is invalid. The requested action cannot be made at this time.");

            }
            catch (Exception err)
            {
                ModelState.AddModelError("", "The requested action cannot be made at this time." + err.Message);
            }

            return Json(new[] { clientAssignmentViewModel }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> ExternalUserClientAssignmentList_Update([DataSourceRequest] DataSourceRequest request,
            ExternalUserClientAssignmentViewModel clientAssignmentViewModel)
        {

            ModelState.AddModelError("", "Edit's are not allowed at this time. To make a change, delete the record and add a new one.");
            return Json(new[] { clientAssignmentViewModel }.ToDataSourceResult(request, ModelState));

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> ExternalUserClientAssignmentList_Destroy(
            [DataSourceRequest] DataSourceRequest request, ExternalUserClientAssignmentViewModel externalUserClientAssignmentViewModel)
        {
            AdminDbContext adminDbContext = new AdminDbContext();
            try
            {
                if (ModelState.IsValid)
                {
                    EfAdmin.ExternalUserClient userClient = adminDbContext.ExternalUserClients
                        .Where(x => x.EmployerId == externalUserClientAssignmentViewModel.EmployerId &&
                            x.UserId == externalUserClientAssignmentViewModel.UserId).SingleOrDefault();

                    adminDbContext.ExternalUserClients.Remove(userClient);

                    adminDbContext.SaveChanges();

                }
                else
                    ModelState.AddModelError("", "The data returned is invalid. The requested action cannot be made at this time.");

            }
            catch (Exception err)
            {

                IEnumerable<DbEntityValidationResult> errors = adminDbContext.GetValidationErrors();
                if (errors.Count() == 0)
                {

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

                ModelState.AddModelError("", "The requested action cannot be made at this time." + err.Message);
            }

            return Json(new[] { externalUserClientAssignmentViewModel }.ToDataSourceResult(request, ModelState));

        }


        public ActionResult UpdateClientDatabaseConnectionAjax(int employerId)
        {

            AdminDbContext adminDbContext = new AdminDbContext();

            //returnUrl = "/HrkAdmin";
            string databaseName = adminDbContext.Employers.Where(x => x.EmployerId == employerId)
                .Select(x => x.DatabaseName).FirstOrDefault();

            //string adminConnString = adminDbContext.Database.Connection.ConnectionString;
            string adminConnString = ConfigurationManager.ConnectionStrings["AdminDbContext"].ConnectionString;
            if (!adminConnString.Contains("ExecViewHrk")) // sanity check
                throw new Exception("Admin Database name not set to ExecViewHrk");

            string clientConnString = adminConnString.Replace("ExecViewHrk", databaseName);



            ClientDbContext clientDbContext = new ClientDbContext(clientConnString);
            if (!clientDbContext.Database.Exists())
            {
                return Json("database doesn't exist", JsonRequestBehavior.AllowGet);
            }

            else
            {
                SessionStateHelper.Set(SessionStateKeys.PERSON_SELECTED_ID, null);

                User.Identity.AddUpdateClaim(SessionStateKeys.CLIENT_DB_CONNECT_STRING.ToString(), clientConnString.ToString());
                User.Identity.AddUpdateClaim(SessionStateKeys.SELECTED_CLIENT_ID.ToString(), employerId.ToString());


                return Json("success", JsonRequestBehavior.AllowGet);
            }

        }


        public JsonResult UsersMultipleCompaniesDdlList_Read()
        {
            AdminDbContext adminDbContext = new AdminDbContext();

            string userId = User.Identity.GetUserId();

            var userClientList = adminDbContext.AspNetUsers.Where(x => x.Id == userId)
                        .Include(x => x.UserCompanies)
                        .SelectMany(x => x.UserCompanies.Select(y => y.Employer))
                        .OrderBy(x => x.DatabaseName).ToList();

            if (User.Identity.GetUserMultipleCompaniesSelectedCompanyID() == null || User.Identity.GetUserMultipleCompaniesSelectedCompanyID() == "0")
            {
                User.Identity.AddUpdateClaim(SessionStateKeys.USER_MULTIPLE_COMPANIES_SELECTED_COMPANY_ID.ToString(), userClientList[0].EmployerId.ToString());
            }

            return Json(userClientList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExternalUserClientsDdlList_Read()
        {
            AdminDbContext adminDbContext = new AdminDbContext();

            string userId = User.Identity.GetUserId();

            var userClientList = adminDbContext.AspNetUsers.Where(x => x.Id == userId)
                        .Include(x => x.ExternalUserClients)
                        .SelectMany(x => x.ExternalUserClients.Select(y => y.Employer))
                        .OrderBy(x => x.DatabaseName).ToList();

            if (User.Identity.GetExternalUserSelectedClientID() == null || User.Identity.GetExternalUserSelectedClientID() == "0")
            {
                User.Identity.AddUpdateClaim(SessionStateKeys.EXTERNAL_USER_SELECTED_ID_CLIENT.ToString(), userClientList[0].EmployerId.ToString());
            }

            return Json(userClientList, JsonRequestBehavior.AllowGet);
        }


        private void PopulateEmployers()
        {
            var employersList = new AdminDbContext().Employers
                        .Select(e => new EmployerDdlEntryViewModel
                        {
                            EmployerId = e.EmployerId,
                            EmployerName = e.EmployerName //+ "-" + e.SkillId.ToString()
                        })
                        .OrderBy(e => e.EmployerName).ToList();

            employersList.Insert(0, new EmployerDdlEntryViewModel { EmployerId = 0, EmployerName = "--select one--" });

            ViewData["employersList"] = employersList;
            ViewData["defaultEmployer"] = employersList.First();
        }

        private void PopulateRoles()
        {
            var rolesList = new AdminDbContext().AspNetRoles
                        .Select(e => new RoleDdlEntryViewModel
                        {
                            RoleId = e.Id,
                            RoleName = e.Name
                        })
                        .OrderBy(e => e.RoleName)
                        .Where(x => x.RoleName.Contains("ClientEmployees") || x.RoleName.Contains("ClientAdministrators") || x.RoleName.Contains("ClientManagers"))
                        .ToList();

            rolesList.Insert(0, new RoleDdlEntryViewModel { RoleId = "0", RoleName = "--select one--" });

            ViewData["rolesList"] = rolesList;
            ViewData["defaultRole"] = rolesList.First();
        }

        private void PopulateUsers()
        {
            var usersList = new AdminDbContext().AspNetUsers
                        .Select(e => new UserDdlEntryViewModel
                        {
                            UserId = e.Id,
                            UserName = e.UserName //+ "-" + e.SkillId.ToString()
                        })
                        .OrderBy(e => e.UserName).ToList();

            usersList.Insert(0, new UserDdlEntryViewModel { UserId = "0", UserName = "--select one--" });

            ViewData["usersList"] = usersList;
            ViewData["defaultUser"] = usersList.First();
        }


        private AppRoleManager RoleManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppRoleManager>();
            }
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        public async Task<ActionResult> UserDeleteajax(string UserId)
        {
            AppUser user = await UserManager.FindByIdAsync(UserId);
            if (user != null)
            {
                IdentityResult result = await UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View("Error", result.Errors);
                }
            }
            else
            {
                return View("Error", new string[] { "User Not Found" });
            }
        }
        public async Task<ActionResult> Edit(string UserId)
        {
            ViewBag.isEdit = true;
            AdminDbContext adminDbContext = new AdminDbContext();
            AppUser user = await UserManager.FindByIdAsync(UserId);
            UserViewModel userViewModel = new UserViewModel();
            var Employer = adminDbContext.Employers.Where(n => n.EmployerId == user.EmployerId).Select(s => s.EmployerName).FirstOrDefault();
            userViewModel.EmployerList = JsonConvert.DeserializeObject<List<DropDownModel>>(GetEmployerList());
            ViewBag.EmployerName = Employer;
            ViewBag.EmployerList = new SelectList(userViewModel.EmployerList, "keyvalue", "keydescription", ViewBag.EmployerName);
            if (user != null)
            {
                return View(user);
            }
            else
            {
                return RedirectToAction("Index");
            }

        }
        public string GetEmployerList()
        {
            string connString = User.Identity.GetClientConnectionString();
            AdminDbContext adminDbContext = new AdminDbContext();

            var EmployerList = adminDbContext.Employers
                .Select(m => new DropDownModel { keyvalue = m.EmployerId.ToString(), keydescription = m.EmployerName })
                .OrderBy(x => x.keydescription)
                .ToList()
                .CleanUp();

            return JsonConvert.SerializeObject(EmployerList);
        }
        public ActionResult EditEmployer(int employerId)
        {
            string connString = User.Identity.GetClientConnectionString();
            var adminDbContext = new AdminDbContext();
            var objEmployer = new Employer();
            if (employerId != 0)
            {
                objEmployer = adminDbContext.Employers.Where(e => e.EmployerId == employerId).FirstOrDefault();
            }
            return View(objEmployer);
        }
        public ActionResult EmployerListUpdateAjax(Employer employer)
        {
            string connString = User.Identity.GetClientConnectionString();
            AdminDbContext adminDbContext = new AdminDbContext();

            string requestType = User.Identity.GetRequestType();

            bool recordIsNew = false;
            if (requestType.ToLower() != "isselfservice" && employer.EmployerId == 0)
            {
                if (SessionStateHelper.Get(SessionStateKeys.EMPLOYEE_PERSON_SELECTED_ID) == null)
                {
                    ModelState.AddModelError("", "Cannot determine the id of the person the Employer record is for.");
                    return View(employer);
                }
                else
                {
                    employer.EmployerId = Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID));
                }
            }

            if (ModelState.IsValid)
            {
                var employers = adminDbContext.Employers.Where(x => x.EmployerId == employer.EmployerId).SingleOrDefault();

                if (employers == null)
                {
                    employer = new Employer();
                    employer.CreatedBy = User.Identity.Name;
                    employer.CreatedDate = DateTime.UtcNow;
                    employer.EmployerId = employer.EmployerId;
                    employer.EmployerName = employer.EmployerName;
                    employer.DatabaseName = employer.DatabaseName;
                    employer.IsClient = employer.IsClient;

                    recordIsNew = true;
                }
                else
                {
                    employer.ModifiedBy = User.Identity.Name;
                    employer.ModifiedDate = DateTime.UtcNow;
                }
            }
            employer.EmployerName = employer.EmployerName;
            employer.DatabaseName = employer.DatabaseName;
            employer.IsClient = employer.IsClient;
            employer.ModifiedBy = User.Identity.Name;
            employer.ModifiedDate = DateTime.Now;


            if (recordIsNew)
                adminDbContext.Employers.Add(employer);

            try
            {
                adminDbContext.SaveChanges();
                ViewBag.AlertMessage = recordIsNew == true ? "New Employer Record Added" : "Person Employer Record Updated";
                var employers = GetEmployerList();
            }
            catch (Exception err)
            {
                ViewBag.AlertMessage = "";

                IEnumerable<DbEntityValidationResult> errors = adminDbContext.GetValidationErrors();
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
            return Json(new { employer, succeed = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EmployerDeleteAjax(int EmployerId)
        {
            string connString = User.Identity.GetClientConnectionString();
            AdminDbContext adminDbContext = new AdminDbContext();
            try
            {
                var employer = adminDbContext.Employers.Where(d => d.EmployerId == EmployerId).FirstOrDefault();
                if (employer != null)
                {
                    adminDbContext.Employers.Remove(employer);
                    adminDbContext.SaveChanges();
                }
            }
            catch (Exception exe)
            {
                return Json(new { Message = exe.Message, succeed = false }, JsonRequestBehavior.AllowGet);
            }
            ModelState.Clear();
            return Json(new { succeed = true, Message = "Record deleted successfully!" }, JsonRequestBehavior.AllowGet);
        }
        public string UpdatingAspNetUserNamePassword(string UserId)
        {
            var pSSn = "";
            try
            {
                var connString = User.Identity.GetClientConnectionString();
                var clientDbContext = new ClientDbContext(connString);
                string ssn = null, last4DigitSSN = "", passwordHash = "";
                var emails = clientDbContext.AspNetUsers.Where(x => x.Id == UserId).FirstOrDefault().Email;
                ssn = clientDbContext.Persons.Where(x => x.eMail == emails).FirstOrDefault().SSN;
                if (!string.IsNullOrEmpty(ssn))
                {
                    last4DigitSSN = ssn.Substring(ssn.Length - 4, 4);
                    passwordHash = HttpContext.GetOwinContext().GetUserManager<AppUserManager>().PasswordHasher.HashPassword(last4DigitSSN);
                    var aspNetUser = clientDbContext.AspNetUsers.FirstOrDefault(x => x.Email == emails);
                    if (aspNetUser != null)
                    {
                        aspNetUser.PasswordHash = passwordHash;
                        aspNetUser.SecurityStamp = last4DigitSSN;
                        clientDbContext.SaveChanges();
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return pSSn;
        }

        public ActionResult ResetTreatyLimit()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ValidateAdmin(string name)
        {
            try
            {
                if (name != null)
                {
                    string userPass = name;

                    string adminPass = ConfigurationManager.AppSettings["AdminPassword"].ToString();

                    if (adminPass.Equals(userPass, StringComparison.OrdinalIgnoreCase))
                    {
                        return Json(new { Message = "SUCCESS", succeed = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Message = "Invalid Password", succeed = false }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = Utils.GetErrorString(ex, null, null);
                return Json(new { Message = "Invalid Password", succeed = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Message = "SUCCESS", succeed = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PartialReset()
        {
            try
            {


                _hrkAdminRepository.PartialReset();

                SessionStateHelper.Set(SessionStateKeys.PERSON_SELECTED_ID, null);

                SessionStateHelper.Set(SessionStateKeys.EMPLOYEE_PERSON_SELECTED_ID, null);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, succeed = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Message = "SUCCESS", succeed = true }, JsonRequestBehavior.AllowGet);
        }

        #region Treatylimithistory
        public ActionResult Treatylimithistory()
        {
            var Year = DateTime.Now.Year;
            if (Convert.ToInt32(TempData["SeletedYear"]) != 0)
            {
                 Year = Convert.ToInt32(TempData["SeletedYear"]);
            }           
            var treatylimithistoriesVm = new TreatylimithistoriesVm
            {
                lstTreatyyears = GetYears(),
                Treatyyear = Year
            };
            return View(treatylimithistoriesVm);
        }     
        public ActionResult TreatyLimitHistory_Read([DataSourceRequest]DataSourceRequest request, int treatyyear)
        {

            var connString = User.Identity.GetClientConnectionString();
            var clientDbContext = new ClientDbContext(connString);
            var treatyList = clientDbContext.Treatylimithistories.Where(x=>x.CreatedDate.Year== treatyyear).ToList();
            return Json(treatyList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult TreatyLimitYearchange(int treatyyear)
        {
            TempData["SeletedYear"] = treatyyear;
            return RedirectToAction("Treatylimithistory");
        }
        
        public List<DropDownModel> GetYears()
        {
            var lstDropDownYears = new List<DropDownModel>
                {
                new DropDownModel { keyvalue = Convert.ToString(DateTime.Now.Year), keydescription = Convert.ToString(DateTime.Now.Year) },
                new DropDownModel { keyvalue = Convert.ToString(DateTime.Now.Year+1), keydescription = Convert.ToString(DateTime.Now.Year+1) },
            };
            return lstDropDownYears;
        }
        #endregion
    }
}



