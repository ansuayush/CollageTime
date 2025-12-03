using System.Threading.Tasks;
using System.Web.Mvc;
using ExecViewHrk.WebUI.Models;
using Microsoft.Owin.Security;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ExecViewHrk.WebUI.Infrastructure;
using ExecViewHrk.EfAdmin;
using System.Web;
using System;
using System.Linq;
using System.Data.Entity;
using ExecViewHrk.Domain.Helper;
using System.Configuration;
using ExecViewHrk.EfClient;
using System.Data.Entity.Validation;
using System.Collections.Generic;
using ExecViewHrk.WebUI.Helpers;


namespace ExecViewHrk.WebUI.Controllers
{

    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult Error()
        {
            return View();

        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            //throw new Exception("AccountController forced exception test");
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                Session.Abandon();
                AuthManager.SignOut();
            }
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel loginModel, string returnUrl = "Home/Index")
        {
            AppIdentityDbContext context = new AppIdentityDbContext();
            var roles = context.Roles;

            if (ModelState.IsValid)
            {
                AppUser user = UserManager.Find(loginModel.Name.Trim(), loginModel.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid name or password.");
                    return View(loginModel);
                }
                

                ClaimsIdentity _claimsIdentity = UserManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                var userName = user.LastName + " " + user.FirstName;
                List<Claim> _identityClaimsList = new List<Claim>();
                _identityClaimsList.Add(new Claim(SessionStateKeys.BROWSER_LOGIN_TIME.ToString(), loginModel.Browser_Time.ToString()));
                _identityClaimsList.Add(new Claim(SessionStateKeys.LOGIN_PERSON_NAME.ToString(), userName));
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    Session.Abandon();
                    AuthManager.SignOut();
                }


                IList<string> userRoles = UserManager.GetRoles(user.Id);

                _identityClaimsList.Add(new Claim(SessionStateKeys.LOGGEDIN_USER_ROLE_NAME.ToString(), userRoles.FirstOrDefault()));

                string clientConnString = "";
                if (userRoles.Contains("HrkAdministrators"))
                {
                    AdminDbContext adminDbContext = new AdminDbContext();

                    returnUrl = "/HrkAdmin";
                    var employer = adminDbContext.Employers.Where(x => x.IsClient == true && x.EmployerId == 3).FirstOrDefault();//.Where(x => x.IsClient == true).Select(x => x).FirstOrDefault();
                    string databaseName = employer.DatabaseName;

                    if (!CanAccessDatabase(adminDbContext, databaseName, ref clientConnString))
                        return View();
                    
                    _identityClaimsList.Add(new Claim(SessionStateKeys.CLIENT_DB_CONNECT_STRING.ToString(), clientConnString));
                    _identityClaimsList.Add(new Claim(SessionStateKeys.SELECTED_CLIENT_ID.ToString(), employer.EmployerId.ToString()));
                    _identityClaimsList.Add(new Claim(SessionStateKeys.LOGIN_PERSON_ID.ToString(), "0"));
                    _identityClaimsList.Add(new Claim(SessionStateKeys.REQUEST_TYPE.ToString(), "NSS"));
                    _claimsIdentity.AddClaims(_identityClaimsList);


                    AuthManager.SignIn(new AuthenticationProperties { IsPersistent = false }, _claimsIdentity);

                    
                }
                else if (userRoles.Contains("HrkAccountManagers"))
                {
                    AdminDbContext adminDbContext = new AdminDbContext();
                    returnUrl = "/HrkAdmin";

                    var userClientList = adminDbContext.AspNetUsers.Where(x => x.Id == user.Id)
                        .Include(x => x.Employers).SelectMany(x => x.Employers).ToList();

                    if (userClientList == null || userClientList.Count == 0)
                    {
                        Session.Abandon();
                        AuthManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        ModelState.AddModelError("", "You do not have any clients assigned.");
                        return View();
                    }

                    string databaseName = userClientList.ElementAt(0).DatabaseName;
                    if (!CanAccessDatabase(adminDbContext, databaseName, ref clientConnString))
                        return View();
                    

                    _identityClaimsList.Add(new Claim(SessionStateKeys.CLIENT_DB_CONNECT_STRING.ToString(), clientConnString));
                    _identityClaimsList.Add(new Claim(SessionStateKeys.SELECTED_CLIENT_ID.ToString(), userClientList[0].EmployerId.ToString()));
                    _identityClaimsList.Add(new Claim(SessionStateKeys.LOGIN_PERSON_ID.ToString(), "0"));

                    _claimsIdentity.AddClaims(_identityClaimsList);

                    AuthManager.SignIn(new AuthenticationProperties { IsPersistent = false }, _claimsIdentity);
                }
                else if (userRoles.Contains("ClientAdminsMultipleCompanies"))
                {
                    AdminDbContext adminDbContext = new AdminDbContext();
                    returnUrl = "/HrkAdmin";

                    var userClientList = adminDbContext.AspNetUsers.Where(x => x.Id == user.Id)
                        .Include(x => x.UserCompanies).SelectMany(x => x.UserCompanies.Select(y => y.Employer)).ToList();

                    if (userClientList == null || userClientList.Count == 0)
                    {
                        Session.Abandon();
                        AuthManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        ModelState.AddModelError("", "You are in ClientAdminsMultipleCompanies role but there are no assignments set up.");
                        return View();
                    }

                    string databaseName = userClientList.ElementAt(0).DatabaseName;

                    if (!CanAccessDatabase(adminDbContext, databaseName, ref clientConnString))
                        return View();


                    _identityClaimsList.Add(new Claim(SessionStateKeys.CLIENT_DB_CONNECT_STRING.ToString(), clientConnString));
                    _identityClaimsList.Add(new Claim(SessionStateKeys.USER_MULTIPLE_COMPANIES_SELECTED_COMPANY_ID.ToString(), userClientList[0].EmployerId.ToString()));

                    _claimsIdentity.AddClaims(_identityClaimsList);

                    AuthManager.SignIn(new AuthenticationProperties { IsPersistent = false }, _claimsIdentity);

                    if (!SetLoginPersonId(loginModel.Name, ref _identityClaimsList))
                        return View();

                }
                else if (userRoles.Contains("ExternalUserAdmins"))
                {
                    AdminDbContext adminDbContext = new AdminDbContext();
                    returnUrl = "/HrkAdmin";

                    var userClientList = adminDbContext.AspNetUsers.Where(x => x.Id == user.Id)
                        .Include(x => x.ExternalUserClients)
                        .SelectMany(x => x.ExternalUserClients.Select(y => y.Employer))
                        .OrderBy(x => x.DatabaseName).ToList();

                    if (userClientList == null || userClientList.Count == 0)
                    {
                        Session.Abandon();
                        AuthManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        ModelState.AddModelError("", "You are in ExternalUserAdmins role but there are no client assignments set up.");
                        return View();
                    }

                    string databaseName = userClientList.ElementAt(0).DatabaseName;

                    if (!CanAccessDatabase(adminDbContext, databaseName, ref clientConnString))
                        return View();
                    

                    _identityClaimsList.Add(new Claim(SessionStateKeys.CLIENT_DB_CONNECT_STRING.ToString(), clientConnString));
                    _identityClaimsList.Add(new Claim(SessionStateKeys.SELECTED_CLIENT_ID.ToString(), userClientList[0].EmployerId.ToString()));
                    _identityClaimsList.Add(new Claim(SessionStateKeys.LOGIN_PERSON_ID.ToString(), "0"));

                    _claimsIdentity.AddClaims(_identityClaimsList);

                    AuthManager.SignIn(new AuthenticationProperties { IsPersistent = false }, _claimsIdentity);
                }
                else if (userRoles.Contains("ClientAdministrators"))
                {
                    AdminDbContext adminDbContext = new AdminDbContext();
                    returnUrl = "/HrkAdmin";

                    var userInDb = adminDbContext.AspNetUsers
                        .Include(x => x.Employer).Where(x => x.Id == user.Id).SingleOrDefault();

                    string databaseName = userInDb.Employer.DatabaseName;

                    if (!CanAccessDatabase(adminDbContext, databaseName, ref clientConnString))
                        return View();


                    _identityClaimsList.Add(new Claim(SessionStateKeys.CLIENT_DB_CONNECT_STRING.ToString(), clientConnString));
                    _identityClaimsList.Add(new Claim(SessionStateKeys.CLIENT_ADMIN_EMPLOYER_ID.ToString(), userInDb.EmployerId.ToString()));

                    _claimsIdentity.AddClaims(_identityClaimsList);

                    AuthManager.SignIn(new AuthenticationProperties { IsPersistent = false }, _claimsIdentity);

                    if (!SetLoginPersonId(loginModel.Name, ref _identityClaimsList))
                        return View();

                }
                else if (userRoles.Contains("ClientEmployees"))
                {
                    AdminDbContext adminDbContext = new AdminDbContext();
                    string connString = Convert.ToString(ConfigurationManager.ConnectionStrings["execView1"]);
                    ClientDbContext clientDbContext = new ClientDbContext(connString);
                    //returnUrl = "/HrkAdmin";
                    //returnUrl = "/EmployeeHome";
                    if (DetectBrowserCapabilities())
                        returnUrl = "/MobileHome";
                    else
                        returnUrl = "/HrkAdmin/Index";

                    var userInDb = adminDbContext.AspNetUsers
                        .Include(x => x.Employer).Where(x => x.Id == user.Id).SingleOrDefault();
                    var personid = clientDbContext.Persons.Where(x => x.eMail == userInDb.Email).Select(x => x.PersonId).FirstOrDefault();
                    var isStudent = clientDbContext.Employees.Where(x => x.PersonId == personid).Select(m => m.IsStudent).FirstOrDefault();
                    Session["isStudent"] = isStudent;
                    string databaseName = userInDb.Employer.DatabaseName;

                    if (!CanAccessDatabase(adminDbContext, databaseName, ref clientConnString))
                        return View();
                    

                    _identityClaimsList.Add(new Claim(SessionStateKeys.CLIENT_DB_CONNECT_STRING.ToString(), clientConnString));
                    _identityClaimsList.Add(new Claim(SessionStateKeys.CLIENT_ADMIN_EMPLOYER_ID.ToString(), userInDb.EmployerId.ToString()));
                    _identityClaimsList.Add(new Claim(SessionStateKeys.SELECTED_CLIENT_ID.ToString(), userInDb.EmployerId.ToString()));
                    _identityClaimsList.Add(new Claim(SessionStateKeys.EMPLOYEE_TIME_CARD_TYPE_CODE.ToString(), Emp_TimeCardsType(clientConnString, user.UserName)));

                    _claimsIdentity.AddClaims(_identityClaimsList);

                    AuthManager.SignIn(new AuthenticationProperties { IsPersistent = false }, _claimsIdentity);

                    if (!SetLoginPersonId(loginModel.Name, ref _identityClaimsList))
                        return View();
                }
                else if (userRoles.Contains("ClientManagers"))
                {
                    AdminDbContext adminDbContext = new AdminDbContext();
                    returnUrl = "/HrkAdmin";

                    var userInDb = adminDbContext.AspNetUsers
                        .Include(x => x.Employer).Where(x => x.Id == user.Id).SingleOrDefault();

                    string databaseName = userInDb.Employer.DatabaseName;

                    if (!CanAccessDatabase(adminDbContext, databaseName, ref clientConnString))
                        return View();

              

                    _identityClaimsList.Add(new Claim(SessionStateKeys.CLIENT_DB_CONNECT_STRING.ToString(), clientConnString));
                    _identityClaimsList.Add(new Claim(SessionStateKeys.CLIENT_ADMIN_EMPLOYER_ID.ToString(), userInDb.EmployerId.ToString()));

                    _claimsIdentity.AddClaims(_identityClaimsList);

                    AuthManager.SignIn(new AuthenticationProperties { IsPersistent = false }, _claimsIdentity);

                    if (!SetLoginPersonId(loginModel.Name, ref _identityClaimsList))
                        return View();
                }
                else
                {
                    ModelState.AddModelError("", "You are not assigned to any roles. Contact system administrator.");
                    Session.Abandon();
                    AuthManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    return View();
                }

                string baseUrl = Request.Url.ToString().Replace(Request.Url.AbsolutePath, "");

                return Redirect("~" + returnUrl);
            }
            return View(loginModel);
        }

        private bool SetLoginPersonId(string email, ref List<Claim> _identityClaimsList)
        {
            string connString = ConfigurationManager.ConnectionStrings["execView1"].ConnectionString;
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var aspNetUsersEmail = clientDbContext.AspNetUsers.Where(x => x.UserName == email.Trim()).Select(x => x.Email).FirstOrDefault();
            var person = clientDbContext.Persons.Where(x => x.eMail == aspNetUsersEmail).FirstOrDefault();
            if (person == null)
            {
                ModelState.AddModelError("", "You do not have a person record set up.");
                return false;
            }
            else
            {               
                _identityClaimsList.Add(new Claim(SessionStateKeys.LOGIN_PERSON_ID.ToString(), person.PersonId.ToString()));
                return true;
            }
        }

        private bool CanAccessDatabase(AdminDbContext adminDbContext, string databaseName, ref string clientConnString)
        {
            string adminConnString = ConfigurationManager.ConnectionStrings["AdminDbContext"].ConnectionString;
            //if (!adminConnString.Contains("DUBlankDB")) 
            //    throw new Exception("Admin Database name not set to ExecViewHrk");

            clientConnString = adminConnString.Replace("CommunityHealthNetwork", databaseName);

            ClientDbContext clientDbContext = new ClientDbContext(clientConnString);
            if (!clientDbContext.Database.Exists())
            {
                Session.Abandon();
                AuthManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                ModelState.AddModelError("", "Database does not exist. for database = " + databaseName);
                return false;
            }
            else
            {
                try
                {
                    var person = clientDbContext.DdlAddressTypes.Count(); // check to see we can access the database
                }
                catch (Exception ex)
                {
                    var message = ex.Message;
                    IEnumerable<DbEntityValidationResult> errors = adminDbContext.GetValidationErrors();
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
                    Session.Abandon();
                    AuthManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    return false;
                }
            }

            return true;
        }

        // example below from google serach to handle lockouts
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LoginLockout(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // find user by username first
                //var user = await UserManager.FindByNameAsync(model.Email);

                //if (user != null)
                //{
                //    var validCredentials = await UserManager.FindAsync(model.Email, model.Password);

                //    // When a user is lockedout, this check is done to ensure that even if the credentials are valid
                //    // the user can not login until the lockout duration has passed
                //    if (await UserManager.IsLockedOutAsync(user.Id))
                //    {
                //        ModelState.AddModelError("", string.Format("Your account has been locked out for {0} minutes due to multiple failed login attempts.", ConfigurationManager.AppSettings["DefaultAccountLockoutTimeSpan"].ToString()));
                //    }
                //    // if user is subject to lockouts and the credentials are invalid
                //    // record the failure and check if user is lockedout and display message, otherwise, 
                //    // display the number of attempts remaining before lockout
                //    else if (await UserManager.GetLockoutEnabledAsync(user.Id) && validCredentials == null)
                //    {
                //        // Record the failure which also may cause the user to be locked out
                //        await UserManager.AccessFailedAsync(user.Id);

                //        string message;

                //        if (await UserManager.IsLockedOutAsync(user.Id))
                //        {
                //            message = string.Format("Your account has been locked out for {0} minutes due to multiple failed login attempts.", ConfigurationManager.AppSettings["DefaultAccountLockoutTimeSpan"].ToString());
                //        }
                //        else
                //        {
                //            int accessFailedCount = await UserManager.GetAccessFailedCountAsync(user.Id);

                //            int attemptsLeft =
                //                Convert.ToInt32(
                //                    ConfigurationManager.AppSettings["MaxFailedAccessAttemptsBeforeLockout"].ToString()) -
                //                accessFailedCount;

                //            message = string.Format(
                //                "Invalid credentials. You have {0} more attempt(s) before your account gets locked out.", attemptsLeft);

                //        }

                //        ModelState.AddModelError("", message);
                //    }
                //    else if (validCredentials == null)
                //    {
                //        ModelState.AddModelError("", "Invalid credentials. Please try again.");
                //    }
                //    else
                //    {
                //        await SignInAsync(user, model.RememberMe);

                //        // When token is verified correctly, clear the access failed count used for lockout
                //        await UserManager.ResetAccessFailedCountAsync(user.Id);

                //        return RedirectToLocal(returnUrl);
                //    }
                //}
            }

            // If we got this far, something failed, redisplay form
            //return View(model);

            return View();
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            ViewBag.Response = "";
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(LoginModel loginModel, string returnUrl = "Home/Index")
        {
            AdminDbContext adminDbContext = new AdminDbContext();
            string connString = ConfigurationManager.ConnectionStrings["execView1"].ConnectionString;
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var aspNetUser = clientDbContext.AspNetUsers.Where(x => x.UserName == loginModel.Name.Trim()).FirstOrDefault();
            if (aspNetUser == null)
            {
                ViewBag.Response = "The username cannot be found. Please try again or contact support.";
            }
            else
            {                              
                string ssn = null, last4DigitSSN = "", passwordHash = "";
                var emails = clientDbContext.AspNetUsers.Where(x => x.UserName == loginModel.Name).Select(x => x.Email).FirstOrDefault();
                ssn = clientDbContext.Persons.Where(x => x.eMail == emails).Select(x => x.SSN).FirstOrDefault();
                if (!string.IsNullOrEmpty(ssn))
                {
                    last4DigitSSN = ssn.Substring(ssn.Length - 4, 4);
                    passwordHash = HttpContext.GetOwinContext().GetUserManager<AppUserManager>().PasswordHasher.HashPassword(last4DigitSSN);
                    var aspNetUsers = clientDbContext.AspNetUsers.FirstOrDefault(x => x.Email == emails);
                    if (aspNetUsers != null)
                    {
                        aspNetUsers.PasswordHash = passwordHash;
                        aspNetUsers.SecurityStamp = last4DigitSSN;
                        clientDbContext.SaveChanges();
                    }                    
                    string strFrom = ConfigurationManager.AppSettings["fromAddress"].ToString();                    
                    EmailProcessorCommunity.Send("", strFrom, aspNetUsers.Email, "Password Reset", "Your Password reset to last four digit of your SSN.",false);                    
                    ViewBag.Response = "Password Reset Email sent successfully.";
                }
            }
            if (aspNetUser != null)
            {
                TempData["message"] = "Password Reset Email sent successfully.";
                return RedirectToAction("Login", "Account");
            }
            else
            { return View(); }
        }


        [Authorize]
        public ActionResult Logout()
        {
            //Session.Clear();
            Session.Abandon();
            AuthManager.SignOut();


            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult LogoutSessionExpired()
        {
            Session.Abandon();
            AuthManager.SignOut();
            TempData["message"] = "Your session has expired ";
            return RedirectToAction("Index", "Home");
        }

        private IAuthenticationManager AuthManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
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

        //TimeCard Fast In Out Fetch records from database
        private string Emp_TimeCardsType(string connString, string username)
        {
            var emp_TimecardTypeId = "NS";      //Not set       


            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                try
                {
                    var empDetails = AccessEmployeeDetails.EmpDetails(clientDbContext, username);
                    if (empDetails != null)
                    {
                        emp_TimecardTypeId = clientDbContext.DdlTimeCardTypes
                            .Where(x => x.TimeCardTypeId == empDetails.TimeCardTypeId)
                            .Select(x => x.TimeCardTypeCode).FirstOrDefault();
                    }
                }
                catch// (Exception err)
                {
                    ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                }

            }

            return (emp_TimecardTypeId == null ? "" : emp_TimecardTypeId);
        }

        private bool DetectBrowserCapabilities()
        {
            return Request.Browser.IsMobileDevice;
        }
    }

}