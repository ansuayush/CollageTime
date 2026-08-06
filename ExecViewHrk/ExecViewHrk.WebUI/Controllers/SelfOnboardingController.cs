using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Helpers;
using ExecViewHrk.WebUI.Infrastructure;
using ExecViewHrk.WebUI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    [Authorize]
    public class SelfOnboardingController : Controller
    {
        /// <summary>
        /// Full-page wizard for new-hire self onboarding (no employee portal tabs).
        /// </summary>
        public ActionResult Index()
        {
            string connString = User.Identity.GetClientConnectionString();
            if (string.IsNullOrWhiteSpace(connString))
                connString = ConfigurationManager.ConnectionStrings["execView1"].ConnectionString;

            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);
                var hire = ResolveHireForLogin(db);
                ViewBag.HasHire = hire != null;
                if (hire == null)
                {
                    ViewBag.IdentityMessage = "No self onboarding registration is linked to this login. Contact HR if you received a new hire notice.";
                    return View("Wizard", new SelfOnboardingWizardVm());
                }

                var wizard = BuildWizardVm(db, hire.HireId);
                return View("Wizard", wizard);
            }
        }

        #region HR - Rules / Send notice

        public PartialViewResult RulesPartial()
        {
            return PartialView();
        }

        public PartialViewResult HireReviewPartial()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);
                var list = MapHireList(db, null);
                return PartialView(list);
            }
        }

        [HttpGet]
        public JsonResult GetRuleLookups()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);
                JobRecruitingSchemaHelper.EnsureSchema(db);

                var positions = db.Positions
                    .Where(p => p.IsPositionActive)
                    .OrderBy(p => p.PositionCode)
                    .ToList()
                    .Select(p => new
                    {
                        id = p.PositionId,
                        text = (p.PositionCode ?? "") + " - " + (string.IsNullOrWhiteSpace(p.Title) ? p.PositionDescription : p.Title)
                    })
                    .ToList();

                var profiles = db.OnboardingProfiles
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.ProfileName)
                    .Select(p => new { id = p.ProfileId, text = p.ProfileName })
                    .ToList();

                var offerLetters = db.OnboardingLookups
                    .Where(l => l.LookupType == "OfferLetter" && l.IsActive)
                    .OrderBy(l => l.SortOrder)
                    .Select(l => new { id = l.LookupId, text = l.Description })
                    .ToList();

                string nextBadge = SelfOnboardingSchemaHelper.NextFileNumber(db);

                return Json(new { success = true, positions, profiles, offerLetters, nextBadge }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetCandidatesForPosition(int positionId)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                JobRecruitingSchemaHelper.EnsureSchema(db);
                SelfOnboardingSchemaHelper.EnsureSchema(db);

                var apps = (from a in db.JobApplications
                            join r in db.JobRequisitions on a.RequisitionId equals r.RequisitionId
                            where r.PositionId == positionId && (a.Status == "Hire" || a.Status == "Candidate")
                            select new { a.ApplicationId, a.ApplicantId, a.EmployeeId, a.Status, a.CreatedDate })
                    .OrderByDescending(x => x.CreatedDate)
                    .ToList();

                var list = apps.Select(a =>
                {
                    string first = "", last = "", email = "";
                    if (a.ApplicantId.HasValue)
                    {
                        var ap = db.JobApplicants.FirstOrDefault(x => x.ApplicantId == a.ApplicantId.Value);
                        if (ap != null) { first = ap.FirstName; last = ap.LastName; email = ap.Email; }
                    }
                    else if (a.EmployeeId.HasValue)
                    {
                        var emp = db.Employees.FirstOrDefault(e => e.EmployeeId == a.EmployeeId.Value);
                        if (emp != null)
                        {
                            var person = db.Persons.FirstOrDefault(p => p.PersonId == emp.PersonId);
                            if (person != null) { first = person.Firstname; last = person.Lastname; email = person.eMail; }
                        }
                    }
                    return new
                    {
                        applicationId = a.ApplicationId,
                        applicantId = a.ApplicantId,
                        text = (first + " " + last).Trim() + " (" + a.Status + ")",
                        firstName = first,
                        lastName = last,
                        homeEmail = email
                    };
                }).ToList();

                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SendNewHireNotice(int? positionId, int? profileId, int? applicationId, int? applicantId,
            string firstName, string lastName, string homeEmail, string fileNumber, int? offerLetterId)
        {
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(homeEmail))
                return Json(new { success = false, message = "First name, last name, and home email are required." });

            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);

                string badge = string.IsNullOrWhiteSpace(fileNumber)
                    ? SelfOnboardingSchemaHelper.NextFileNumber(db)
                    : fileNumber.Trim();

                string userName = SelfOnboardingSchemaHelper.BuildUserName(firstName, lastName, badge);
                string tempPassword = Guid.NewGuid().ToString("N").Substring(0, 8) + "Aa1!";

                if (db.AspNetUsers.Any(u => u.UserName == userName))
                    return Json(new { success = false, message = "A login already exists for this username." });

                int employerId = 0;
                int.TryParse(User.Identity.GetClientAdminEmployerID(), out employerId);
                if (employerId <= 0) int.TryParse(User.Identity.GetSelectedClientID(), out employerId);

                string positionTitle = null;
                if (positionId.HasValue)
                {
                    var pos = db.Positions.FirstOrDefault(p => p.PositionId == positionId.Value);
                    if (pos != null)
                        positionTitle = string.IsNullOrWhiteSpace(pos.Title) ? pos.PositionDescription : pos.Title;
                }

                var hasher = HttpContext.GetOwinContext().GetUserManager<AppUserManager>().PasswordHasher;
                var aspUser = new AspNetUser
                {
                    Id = Guid.NewGuid().ToString(),
                    EmployerId = employerId > 0 ? employerId : 0,
                    LastPasswordChangeDate = DateTime.Today,
                    Email = homeEmail.Trim(),
                    PasswordHash = hasher.HashPassword(tempPassword),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = userName,
                    FirstName = firstName.Trim(),
                    LastName = lastName.Trim()
                };
                db.AspNetUsers.Add(aspUser);
                db.SaveChanges();

                var roleId = db.AspNetRoles.Where(r => r.Name == "ClientEmployees").Select(r => r.Id).FirstOrDefault();
                if (!string.IsNullOrEmpty(roleId))
                {
                    db.AspNetUserRoles.Add(new AspNetUserRole { UserId = aspUser.Id, RoleId = roleId });
                    db.SaveChanges();
                }

                // Login requires a Persons row matching AspNetUsers.Email
                int? employeeId = EnsurePersonAndEmployeeForHire(db, firstName.Trim(), lastName.Trim(), homeEmail.Trim(), badge, User.Identity.Name);

                var hire = new SelfOnboardingHire
                {
                    PositionId = positionId,
                    PositionTitle = positionTitle,
                    ProfileId = profileId,
                    ApplicationId = applicationId,
                    ApplicantId = applicantId,
                    FirstName = firstName.Trim(),
                    LastName = lastName.Trim(),
                    HomeEmail = homeEmail.Trim(),
                    WorkEmail = homeEmail.Trim(),
                    FileNumber = badge,
                    OfferLetterId = offerLetterId,
                    GeneratedUserName = userName,
                    AspNetUserId = aspUser.Id,
                    EmployeeId = employeeId,
                    Status = "Invited",
                    CurrentStep = 1,
                    TransactionId = SelfOnboardingSchemaHelper.NewTransactionId(),
                    NoticeSentDate = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    CreatedDate = DateTime.Now
                };
                db.SelfOnboardingHires.Add(hire);
                db.SaveChanges();

                db.SelfOnboardingPersonals.Add(new SelfOnboardingPersonal
                {
                    HireId = hire.HireId,
                    FirstName = hire.FirstName,
                    LastName = hire.LastName,
                    HomeEmail = hire.HomeEmail,
                    WorkEmail = hire.WorkEmail,
                    ModifiedDate = DateTime.Now
                });
                db.SaveChanges();

                string loginUrl = Url.Action("Login", "Account", null, Request.Url.Scheme);
                string body = BuildHireNoticeEmail(hire.FirstName, hire.LastName, userName, tempPassword, loginUrl);
                string emailError = null;
                string savedPath = SaveHireNoticeEmailCopy(hire.HireId, hire.HomeEmail, body);
                try
                {
                    SendHireNoticeEmail(hire.HomeEmail, "Welcome - Self Onboarding Login", body);
                }
                catch (Exception ex)
                {
                    emailError = FormatSmtpError(ex);
                }

                return Json(new
                {
                    success = true,
                    hireId = hire.HireId,
                    userName,
                    fileNumber = badge,
                    message = emailError == null
                        ? "New hire notice sent to " + hire.HomeEmail
                        : "Login created, but Gmail rejected SMTP login. Use a 16-char App Password in Web.config (not erp@123#)."
                          + " Login now -> Username: " + userName + " / Password: " + tempPassword
                          + (savedPath != null ? " | Email copy: App_Data/" + savedPath : ""),
                    tempPassword = emailError != null ? tempPassword : null
                });
            }
        }

        [HttpPost]
        public JsonResult ResendNewHireNotice(int hireId)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);
                var hire = db.SelfOnboardingHires.FirstOrDefault(h => h.HireId == hireId);
                if (hire == null)
                    return Json(new { success = false, message = "Hire not found." });

                string tempPassword = Guid.NewGuid().ToString("N").Substring(0, 8) + "Aa1!";
                var aspUser = !string.IsNullOrEmpty(hire.AspNetUserId)
                    ? db.AspNetUsers.FirstOrDefault(u => u.Id == hire.AspNetUserId)
                    : db.AspNetUsers.FirstOrDefault(u => u.UserName == hire.GeneratedUserName || u.Email == hire.HomeEmail);

                if (aspUser == null)
                    return Json(new { success = false, message = "Login user not found for this hire. Send a new hire notice instead." });

                var hasher = HttpContext.GetOwinContext().GetUserManager<AppUserManager>().PasswordHasher;
                aspUser.PasswordHash = hasher.HashPassword(tempPassword);
                aspUser.SecurityStamp = Guid.NewGuid().ToString();
                // Keep Email = HomeEmail so Account.SetLoginPersonId can find Persons.eMail
                if (string.IsNullOrWhiteSpace(aspUser.Email))
                    aspUser.Email = hire.HomeEmail;
                hire.NoticeSentDate = DateTime.Now;
                hire.ModifiedBy = User.Identity.Name;
                hire.ModifiedDate = DateTime.Now;

                int? employeeId = EnsurePersonAndEmployeeForHire(db, hire.FirstName, hire.LastName, hire.HomeEmail, hire.FileNumber, User.Identity.Name);
                if (employeeId.HasValue)
                    hire.EmployeeId = employeeId;

                db.SaveChanges();

                string loginUrl = Url.Action("Login", "Account", null, Request.Url.Scheme);
                string body = BuildHireNoticeEmail(hire.FirstName, hire.LastName, hire.GeneratedUserName, tempPassword, loginUrl);
                string savedPath = SaveHireNoticeEmailCopy(hire.HireId, hire.HomeEmail, body);
                try
                {
                    SendHireNoticeEmail(hire.HomeEmail, "Welcome - Self Onboarding Login", body);
                    return Json(new { success = true, message = "Email resent to " + hire.HomeEmail });
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Gmail SMTP auth failed (" + FormatSmtpError(ex) + "). "
                            + "Create App Password for " + (ConfigurationManager.AppSettings["FromEmailAddressTraining"] ?? "the SMTP user")
                            + " at https://myaccount.google.com/apppasswords and put it in Web.config mailSettings password. "
                            + "Login now -> Username: " + hire.GeneratedUserName + " / Password: " + tempPassword
                            + (savedPath != null ? " | Email copy: App_Data/" + savedPath : ""),
                        tempPassword,
                        userName = hire.GeneratedUserName
                    });
                }
            }
        }

        /// <summary>
        /// Account login looks up Persons by AspNetUsers.Email. Create Person + stub Employee for new hires.
        /// </summary>
        private static int? EnsurePersonAndEmployeeForHire(ClientDbContext db, string firstName, string lastName,
            string homeEmail, string fileNumber, string enteredBy)
        {
            if (string.IsNullOrWhiteSpace(homeEmail))
                return null;

            string email = homeEmail.Trim();
            var person = db.Persons.FirstOrDefault(p => p.eMail == email);
            if (person == null)
            {
                person = new Person
                {
                    Firstname = TruncateName(firstName, 50),
                    Lastname = TruncateName(lastName, 50),
                    eMail = TruncateName(email, 100),
                    AlternateEMail = TruncateName(email, 100),
                    IsApplicant = true,
                    EnteredBy = TruncateName(enteredBy ?? "SelfOnboarding", 50),
                    EnteredDate = DateTime.Now
                };
                db.Persons.Add(person);
                db.SaveChanges();
            }

            var employee = db.Employees.FirstOrDefault(e => e.PersonId == person.PersonId && e.TerminationDate == null);
            if (employee == null)
            {
                int? companyCodeId = db.CompanyCodes
                    .Where(c => c.CompanyCodeCode == "ELL" || c.CompanyCodeDescription == "ELL")
                    .Select(c => (int?)c.CompanyCodeId)
                    .FirstOrDefault();

                employee = new Employee
                {
                    PersonId = person.PersonId,
                    FileNumber = string.IsNullOrWhiteSpace(fileNumber) ? null : fileNumber.Trim(),
                    EmploymentNumber = 1,
                    HireDate = DateTime.Today,
                    EnteredBy = TruncateName(enteredBy ?? "SelfOnboarding", 50),
                    EnteredDate = DateTime.Now,
                    IsStudent = false,
                    CompanyCode = "ELL",
                    CompanyCodeId = companyCodeId
                };
                db.Employees.Add(employee);
                db.SaveChanges();
            }
            else
            {
                bool changed = false;
                if (!string.IsNullOrWhiteSpace(fileNumber) && string.IsNullOrWhiteSpace(employee.FileNumber))
                {
                    employee.FileNumber = fileNumber.Trim();
                    changed = true;
                }
                if (string.IsNullOrWhiteSpace(employee.CompanyCode))
                {
                    employee.CompanyCode = "ELL";
                    changed = true;
                }
                if (!employee.CompanyCodeId.HasValue)
                {
                    int? companyCodeId = db.CompanyCodes
                        .Where(c => c.CompanyCodeCode == "ELL" || c.CompanyCodeDescription == "ELL")
                        .Select(c => (int?)c.CompanyCodeId)
                        .FirstOrDefault();
                    if (companyCodeId.HasValue)
                    {
                        employee.CompanyCodeId = companyCodeId;
                        changed = true;
                    }
                }
                if (changed)
                    db.SaveChanges();
            }

            return employee.EmployeeId;
        }

        private static string TruncateName(string value, int max)
        {
            if (string.IsNullOrEmpty(value)) return value;
            value = value.Trim();
            return value.Length <= max ? value : value.Substring(0, max);
        }

        private string SaveHireNoticeEmailCopy(int hireId, string to, string htmlBody)
        {
            try
            {
                string folder = Server.MapPath("~/App_Data/" + SelfOnboardingSchemaHelper.StorageFolderName + "/Emails");
                Directory.CreateDirectory(folder);
                string file = "hire_" + hireId + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".html";
                string physical = Path.Combine(folder, file);
                System.IO.File.WriteAllText(physical, "<!-- To: " + to + " -->\r\n" + htmlBody, Encoding.UTF8);
                return Path.Combine(SelfOnboardingSchemaHelper.StorageFolderName, "Emails", file).Replace('\\', '/');
            }
            catch
            {
                return null;
            }
        }

        private static string FormatSmtpError(Exception ex)
        {
            string err = ex.Message ?? "";
            if (ex.InnerException != null)
                err += " (" + ex.InnerException.Message + ")";
            return err;
        }

        /// <summary>
        /// Sends via Web.config system.net/mailSettings with explicit SMTP authentication.
        /// Gmail requires an App Password (not the normal Gmail password) when 2FA is on.
        /// </summary>
        private static void SendHireNoticeEmail(string to, string subject, string htmlBody)
        {
            var section = ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;
            if (section == null || section.Network == null)
                throw new InvalidOperationException("system.net/mailSettings/smtp is missing in Web.config.");

            string host = section.Network.Host;
            int port = section.Network.Port > 0 ? section.Network.Port : 587;
            string userName = section.Network.UserName;
            string password = section.Network.Password;
            bool enableSsl = section.Network.EnableSsl;

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("SMTP host, userName, and password must be set in Web.config mailSettings.");

            string from = userName;
            if (string.IsNullOrWhiteSpace(from))
                from = ConfigurationManager.AppSettings["FromEmailAddressTraining"];

            using (var mail = new MailMessage())
            {
                mail.From = new MailAddress(from.Trim());
                mail.To.Add(to.Trim());
                mail.Subject = subject;
                mail.Body = htmlBody;
                mail.IsBodyHtml = true;

                using (var smtp = new SmtpClient(host, port))
                {
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.EnableSsl = enableSsl || port == 587 || port == 465;
                    // Must be false or Gmail returns 5.7.0 Authentication Required
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(userName.Trim(), password);
                    smtp.Timeout = 60000;
                    smtp.Send(mail);
                }
            }
        }

        private static string BuildHireNoticeEmail(string first, string last, string userName, string password, string loginUrl)
        {
            var sb = new StringBuilder();
            sb.Append("<html><body style='font-family:Arial,sans-serif;font-size:14px;'>");
            sb.Append("<p>Dear ").Append(HttpUtility.HtmlEncode(first)).Append(" ").Append(HttpUtility.HtmlEncode(last)).Append(",</p>");
            sb.Append("<p>Welcome! Your self onboarding account has been created.</p>");
            sb.Append("<p><strong>Username:</strong> ").Append(HttpUtility.HtmlEncode(userName)).Append("<br/>");
            sb.Append("<strong>Temporary Password:</strong> ").Append(HttpUtility.HtmlEncode(password)).Append("</p>");
            sb.Append("<p><a href=\"").Append(HttpUtility.HtmlAttributeEncode(loginUrl)).Append("\">Click here</a> to log in and start your self onboarding wizard.</p>");
            sb.Append("<p>After login, open <strong>Self Service &gt; Self Onboarding</strong> and click <strong>Let's get started</strong>.</p>");
            sb.Append("<p>Thank you.</p>");
            sb.Append("</body></html>");
            return sb.ToString();
        }

        private static string BuildRejectionEmail(string first, string last, string userName, string password,
            string loginUrl, string formNames, string reason)
        {
            var sb = new StringBuilder();
            sb.Append("<html><body style='font-family:Arial,sans-serif;font-size:14px;'>");
            sb.Append("<p>Dear ").Append(HttpUtility.HtmlEncode(first)).Append(" ").Append(HttpUtility.HtmlEncode(last)).Append(",</p>");
            sb.Append("<p>Your self onboarding submission needs corrections before it can be approved.</p>");
            if (!string.IsNullOrWhiteSpace(formNames))
            {
                sb.Append("<p><strong>Form(s) to correct:</strong> ").Append(HttpUtility.HtmlEncode(formNames)).Append("</p>");
            }
            if (!string.IsNullOrWhiteSpace(reason))
            {
                sb.Append("<p><strong>Rejection reason:</strong><br/>").Append(HttpUtility.HtmlEncode(reason).Replace("\n", "<br/>")).Append("</p>");
            }
            sb.Append("<p>Please log in again, update the requested information, and resubmit for HR review.</p>");
            sb.Append("<p><strong>Username:</strong> ").Append(HttpUtility.HtmlEncode(userName)).Append("<br/>");
            sb.Append("<strong>Temporary Password:</strong> ").Append(HttpUtility.HtmlEncode(password)).Append("</p>");
            sb.Append("<p><a href=\"").Append(HttpUtility.HtmlAttributeEncode(loginUrl)).Append("\">Click here</a> to log in and open Self Onboarding.</p>");
            sb.Append("<p>After login, open <strong>Self Service &gt; Self Onboarding</strong>, correct the listed form(s), and submit again.</p>");
            sb.Append("<p>Thank you.</p>");
            sb.Append("</body></html>");
            return sb.ToString();
        }

        #endregion

        #region HR - Review / Approve

        public PartialViewResult ReviewWizardPartial(int hireId)
        {
            try
            {
                string connString = User.Identity.GetClientConnectionString();
                using (var db = new ClientDbContext(connString))
                {
                    SelfOnboardingSchemaHelper.EnsureSchema(db);
                    var wizard = BuildWizardVm(db, hireId);
                    if (wizard == null || wizard.Hire == null)
                    {
                        ViewBag.HasHire = false;
                        ViewBag.IdentityMessage = "Hire registration not found (HireId=" + hireId + ").";
                        return PartialView("MySelfOnboardingPartial", new SelfOnboardingWizardVm { IsHrReview = true });
                    }

                    wizard.IsHrReview = true;
                    wizard.IsReadOnly = true;
                    wizard.Hire.CurrentStep = 7;
                    ViewBag.HasHire = true;
                    return PartialView("MySelfOnboardingPartial", wizard);
                }
            }
            catch (Exception ex)
            {
                ViewBag.HasHire = false;
                ViewBag.IdentityMessage = "Unable to open approval wizard: " + ex.Message;
                return PartialView("MySelfOnboardingPartial", new SelfOnboardingWizardVm { IsHrReview = true });
            }
        }

        [HttpGet]
        public JsonResult GetHires(string status)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);
                return Json(new { success = true, data = MapHireList(db, status) }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetHireDetails(int hireId)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);
                var wizard = BuildWizardVm(db, hireId);
                if (wizard == null)
                    return Json(new { success = false, message = "Hire not found." }, JsonRequestBehavior.AllowGet);
                return Json(new { success = true, data = wizard }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ApproveHire(int hireId)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);
                var hire = db.SelfOnboardingHires.FirstOrDefault(h => h.HireId == hireId);
                if (hire == null)
                    return Json(new { success = false, message = "Hire not found." });
                if (hire.Status != "Submitted")
                    return Json(new { success = false, message = "Only submitted registrations can be approved." });

                hire.Status = "Hired";
                hire.ApprovedBy = User.Identity.Name;
                hire.ApprovedDate = DateTime.Now;
                hire.RejectionReason = null;
                hire.RejectedFormName = null;
                hire.RejectedBy = null;
                hire.RejectedDate = null;
                hire.ModifiedBy = User.Identity.Name;
                hire.ModifiedDate = DateTime.Now;
                db.SaveChanges();
                return Json(new { success = true, message = "Employee approved as Hired." });
            }
        }

        [HttpPost]
        public JsonResult RejectHire(int hireId, string reason, string formNames)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);
                var hire = db.SelfOnboardingHires.FirstOrDefault(h => h.HireId == hireId);
                if (hire == null)
                    return Json(new { success = false, message = "Hire not found." });
                if (hire.Status != "Submitted")
                    return Json(new { success = false, message = "Only submitted registrations can be rejected for correction." });

                reason = (reason ?? "").Trim();
                formNames = (formNames ?? "").Trim();
                if (string.IsNullOrWhiteSpace(reason))
                    return Json(new { success = false, message = "Please enter a rejection reason." });
                if (string.IsNullOrWhiteSpace(formNames))
                    return Json(new { success = false, message = "Please select at least one form to correct." });

                ClearSignaturesForRejectedForms(db, hire.HireId, formNames);

                string tempPassword = Guid.NewGuid().ToString("N").Substring(0, 8) + "Aa1!";
                var aspUser = !string.IsNullOrEmpty(hire.AspNetUserId)
                    ? db.AspNetUsers.FirstOrDefault(u => u.Id == hire.AspNetUserId)
                    : db.AspNetUsers.FirstOrDefault(u => u.UserName == hire.GeneratedUserName || u.Email == hire.HomeEmail);

                if (aspUser == null)
                    return Json(new { success = false, message = "Login user not found for this hire." });

                var hasher = HttpContext.GetOwinContext().GetUserManager<AppUserManager>().PasswordHasher;
                aspUser.PasswordHash = hasher.HashPassword(tempPassword);
                aspUser.SecurityStamp = Guid.NewGuid().ToString();
                if (string.IsNullOrWhiteSpace(aspUser.Email))
                    aspUser.Email = hire.HomeEmail;

                hire.Status = "ChangesRequested";
                hire.RejectionReason = reason.Length > 1000 ? reason.Substring(0, 1000) : reason;
                hire.RejectedFormName = formNames.Length > 200 ? formNames.Substring(0, 200) : formNames;
                hire.RejectedBy = User.Identity.Name;
                hire.RejectedDate = DateTime.Now;
                hire.CurrentStep = LowestStepForRejectedForms(formNames);
                hire.SubmittedDate = null;
                hire.ModifiedBy = User.Identity.Name;
                hire.ModifiedDate = DateTime.Now;
                hire.NoticeSentDate = DateTime.Now;
                db.SaveChanges();

                string loginUrl = Url.Action("Login", "Account", null, Request.Url.Scheme);
                string body = BuildRejectionEmail(hire.FirstName, hire.LastName, hire.GeneratedUserName, tempPassword,
                    loginUrl, formNames, reason);
                string savedPath = SaveHireNoticeEmailCopy(hire.HireId, hire.HomeEmail, body);
                try
                {
                    SendHireNoticeEmail(hire.HomeEmail, "Self Onboarding - Corrections Required", body);
                    return Json(new { success = true, message = "Rejected. Correction email sent to " + hire.HomeEmail });
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Rejected and unlocked for corrections, but email failed (" + FormatSmtpError(ex) + "). "
                            + "Share login manually -> Username: " + hire.GeneratedUserName + " / Password: " + tempPassword
                            + (savedPath != null ? " | Email copy: App_Data/" + savedPath : ""),
                        tempPassword,
                        userName = hire.GeneratedUserName
                    });
                }
            }
        }

        private static int LowestStepForRejectedForms(string formNames)
        {
            var forms = (formNames ?? "").Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(f => f.Trim())
                .Where(f => f.Length > 0)
                .ToList();
            int step = 7;
            foreach (var f in forms)
            {
                string key = f.ToLowerInvariant();
                if (key.Contains("personal")) step = Math.Min(step, 2);
                else if (key.Contains("i-9") || key == "i9") step = Math.Min(step, 3);
                else if (key.Contains("document")) step = Math.Min(step, 4);
                else if (key.Contains("tax") || key.Contains("w-4") || key.Contains("w4")) step = Math.Min(step, 5);
                else if (key.Contains("bank")) step = Math.Min(step, 6);
            }
            return step == 7 ? 2 : step;
        }

        private static void ClearSignaturesForRejectedForms(ClientDbContext db, int hireId, string formNames)
        {
            var forms = (formNames ?? "").Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(f => f.Trim().ToLowerInvariant())
                .Where(f => f.Length > 0)
                .ToList();
            if (!forms.Any()) return;

            bool clearI9 = forms.Any(f => f.Contains("i-9") || f == "i9");
            bool clearTax = forms.Any(f => f.Contains("tax") || f.Contains("w-4") || f.Contains("w4"));
            bool clearDocs = forms.Any(f => f.Contains("document"));

            var sigs = db.SelfOnboardingSignatures.Where(s => s.HireId == hireId && s.IsSigned).ToList();
            foreach (var sig in sigs)
            {
                bool clear = false;
                if (clearI9 && string.Equals(sig.DocumentKey, "I9", StringComparison.OrdinalIgnoreCase))
                    clear = true;
                if (clearTax && string.Equals(sig.DocumentKey, "W4", StringComparison.OrdinalIgnoreCase))
                    clear = true;
                if (clearDocs && sig.ProfileDocumentId.HasValue)
                    clear = true;
                if (!clear) continue;

                sig.IsSigned = false;
                sig.SignedName = null;
                sig.SignedDate = null;
                sig.SignedIp = null;
                if (sig.EmployeeDocumentId.HasValue)
                {
                    var empDoc = db.EmployeeDocuments.FirstOrDefault(d => d.DocumentId == sig.EmployeeDocumentId.Value);
                    if (empDoc != null)
                        empDoc.IsSigned = false;
                }
            }
        }

        private static List<SelfOnboardingHireVm> MapHireList(ClientDbContext db, string status)
        {
            var q = db.SelfOnboardingHires.AsQueryable();
            if (!string.IsNullOrWhiteSpace(status))
                q = q.Where(h => h.Status == status);

            var profiles = db.OnboardingProfiles.ToDictionary(p => p.ProfileId, p => p.ProfileName);
            var offers = db.OnboardingLookups.Where(l => l.LookupType == "OfferLetter").ToDictionary(l => l.LookupId, l => l.Description);

            return q.OrderByDescending(h => h.CreatedDate).ToList().Select(h => new SelfOnboardingHireVm
            {
                HireId = h.HireId,
                PositionId = h.PositionId,
                PositionTitle = h.PositionTitle,
                ProfileId = h.ProfileId,
                ProfileName = h.ProfileId.HasValue && profiles.ContainsKey(h.ProfileId.Value) ? profiles[h.ProfileId.Value] : null,
                ApplicationId = h.ApplicationId,
                ApplicantId = h.ApplicantId,
                FirstName = h.FirstName,
                LastName = h.LastName,
                HomeEmail = h.HomeEmail,
                WorkEmail = h.WorkEmail,
                FileNumber = h.FileNumber,
                OfferLetterId = h.OfferLetterId,
                OfferLetterName = h.OfferLetterId.HasValue && offers.ContainsKey(h.OfferLetterId.Value) ? offers[h.OfferLetterId.Value] : null,
                GeneratedUserName = h.GeneratedUserName,
                Status = h.Status,
                CurrentStep = h.CurrentStep,
                TransactionId = h.TransactionId,
                NoticeSentDate = h.NoticeSentDate,
                SubmittedDate = h.SubmittedDate,
                ConfirmationDate = h.ConfirmationDate,
                ApprovedDate = h.ApprovedDate,
                ApprovedBy = h.ApprovedBy,
                EmployeeId = h.EmployeeId,
                RejectionReason = h.RejectionReason,
                RejectedFormName = h.RejectedFormName,
                RejectedBy = h.RejectedBy,
                RejectedDate = h.RejectedDate
            }).ToList();
        }

        #endregion

        #region Self Service (My Documents pattern)

        public PartialViewResult MySelfOnboardingPartial()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);
                var hire = ResolveHireForLogin(db);
                ViewBag.HasHire = hire != null;
                if (hire == null)
                {
                    ViewBag.IdentityMessage = "No self onboarding registration is linked to this login. Contact HR if you received a new hire notice.";
                    return PartialView(new SelfOnboardingWizardVm());
                }

                var wizard = BuildWizardVm(db, hire.HireId);
                return PartialView(wizard);
            }
        }

        [HttpGet]
        public JsonResult GetWizardData(int? hireId = null)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);
                var hire = ResolveHireForRequest(db, hireId);
                if (hire == null)
                    return Json(new { success = false, message = "No onboarding registration found." }, JsonRequestBehavior.AllowGet);

                var wizard = BuildWizardVm(db, hire.HireId);
                if (hireId.HasValue)
                {
                    wizard.IsHrReview = true;
                    wizard.IsReadOnly = true;
                }
                return Json(new { success = true, data = wizard, lookups = GetWizardLookups(db) }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveStep(int step)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);
                var hire = ResolveHireForLogin(db);
                if (hire == null)
                    return Json(new { success = false, message = "No onboarding registration found." });
                if (IsLocked(hire))
                    return Json(new { success = false, message = "Registration is locked." });

                if (step == 2)
                    SavePersonal(db, hire, BindPersonalFromForm(hire.HireId));

                if (step == 5)
                {
                    var p = db.SelfOnboardingPersonals.FirstOrDefault(x => x.HireId == hire.HireId);
                    if (p == null)
                    {
                        p = new SelfOnboardingPersonal { HireId = hire.HireId };
                        db.SelfOnboardingPersonals.Add(p);
                    }
                    p.FilingStatusId = ParseNullableInt(Request["FilingStatusId"]);
                    p.WorkingCountryId = ParseNullableInt(Request["WorkingCountryId"]);
                    p.WorkingStateId = ParseNullableInt(Request["WorkingStateId"]);
                    p.StateTaxStatusId = ParseNullableInt(Request["StateTaxStatusId"]);
                    p.ModifiedDate = DateTime.Now;
                }

                if (hire.CurrentStep < step + 1)
                    hire.CurrentStep = Math.Min(step + 1, 7);
                hire.Status = hire.Status == "Invited" ? "InProgress" : hire.Status;
                hire.ModifiedDate = DateTime.Now;
                db.SaveChanges();
                return Json(new { success = true, currentStep = hire.CurrentStep });
            }
        }

        private SelfOnboardingPersonalVm BindPersonalFromForm(int hireId)
        {
            return new SelfOnboardingPersonalVm
            {
                HireId = hireId,
                PrefixId = ParseNullableInt(Request["PrefixId"]),
                SuffixId = ParseNullableInt(Request["SuffixId"]),
                FirstName = Request["FirstName"],
                MiddleName = Request["MiddleName"],
                LastName = Request["LastName"],
                PreferredName = Request["PreferredName"],
                WorkEmail = Request["WorkEmail"],
                HomeEmail = Request["HomeEmail"],
                Phone = Request["Phone"],
                DateOfBirth = ParseNullableDate(Request["DateOfBirth"]),
                SSN = Request["SSN"],
                GenderId = ParseNullableInt(Request["GenderId"]),
                MaritalStatusId = ParseNullableInt(Request["MaritalStatusId"]),
                EthnicityId = ParseNullableInt(Request["EthnicityId"]),
                Address1 = Request["Address1"],
                Address2 = Request["Address2"],
                City = Request["City"],
                StateId = ParseNullableInt(Request["StateId"]),
                Zip = Request["Zip"],
                CountryId = ParseNullableInt(Request["CountryId"]),
                LicenseCountryId = ParseNullableInt(Request["LicenseCountryId"]),
                EmergencyName = Request["EmergencyName"],
                EmergencyPhone = Request["EmergencyPhone"],
                RelationshipTypeId = ParseNullableInt(Request["RelationshipTypeId"])
            };
        }

        private static int? ParseNullableInt(string value)
        {
            int n;
            if (string.IsNullOrWhiteSpace(value)) return null;
            return int.TryParse(value, out n) ? (int?)n : null;
        }

        private static DateTime? ParseNullableDate(string value)
        {
            DateTime d;
            if (string.IsNullOrWhiteSpace(value)) return null;
            return DateTime.TryParse(value, out d) ? (DateTime?)d : null;
        }

        [HttpGet]
        public JsonResult GetI9Data(int? hireId = null)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);
                var hire = ResolveHireForRequest(db, hireId);
                if (hire == null)
                    return Json(new { success = false, message = "No onboarding registration found." }, JsonRequestBehavior.AllowGet);

                var countries = db.DdlCountries
                    .OrderBy(c => c.Description)
                    .Select(c => new { id = c.CountryId, text = c.Description })
                    .ToList();

                var i9 = db.SelfOnboardingI9s.FirstOrDefault(x => x.HireId == hire.HireId);
                var signed = db.SelfOnboardingSignatures.Any(s => s.HireId == hire.HireId && s.DocumentKey == "I9" && s.IsSigned);

                return Json(new
                {
                    success = true,
                    countries = countries,
                    data = MapI9Vm(hire.HireId, i9, signed)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveI9(
            int citizenStatus,
            string alienNumber,
            string permanentResidentExpire,
            int? lawCitizenOfId,
            string lawCitizenOfText,
            string alienAuthorizedUntil,
            int? alienCitizenOfId,
            string alienCitizenOfText,
            string alienRegistrationNumber,
            string admissionNumber,
            string passportNumber,
            int? countryOfIssuanceId,
            string countryOfIssuanceText,
            bool translatorNotUsed,
            bool translatorUsed,
            bool federalLawAcknowledged,
            bool hideSsnOnForm)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);
                var hire = ResolveHireForLogin(db);
                if (hire == null)
                    return Json(new { success = false, message = "No onboarding registration found." });
                if (IsLocked(hire))
                    return Json(new { success = false, message = "Registration is locked." });

                DateTime? lawExpire = ParseNullableDate(permanentResidentExpire);
                DateTime? alienUntil = ParseNullableDate(alienAuthorizedUntil);

                string validation = ValidateI9(citizenStatus, lawExpire, lawCitizenOfId, alienUntil, alienCitizenOfId,
                    alienRegistrationNumber, admissionNumber, passportNumber, countryOfIssuanceId, federalLawAcknowledged);
                if (validation != null)
                    return Json(new { success = false, message = validation });

                UpsertI9Fields(db, hire, citizenStatus, alienNumber, lawExpire, lawCitizenOfId, lawCitizenOfText,
                    alienUntil, alienCitizenOfId, alienCitizenOfText, alienRegistrationNumber, admissionNumber,
                    passportNumber, countryOfIssuanceId, countryOfIssuanceText, translatorNotUsed, translatorUsed,
                    federalLawAcknowledged, hideSsnOnForm);

                hire.ModifiedDate = DateTime.Now;
                if (hire.Status == "Invited") hire.Status = "InProgress";
                if (hire.CurrentStep < 3) hire.CurrentStep = 3;
                db.SaveChanges();

                return Json(new { success = true, message = "Form I-9 saved." });
            }
        }

        [HttpPost]
        public JsonResult SaveAndSignI9(
            int citizenStatus,
            string alienNumber,
            string permanentResidentExpire,
            int? lawCitizenOfId,
            string lawCitizenOfText,
            string alienAuthorizedUntil,
            int? alienCitizenOfId,
            string alienCitizenOfText,
            string alienRegistrationNumber,
            string admissionNumber,
            string passportNumber,
            int? countryOfIssuanceId,
            string countryOfIssuanceText,
            bool translatorNotUsed,
            bool translatorUsed,
            bool federalLawAcknowledged,
            bool hideSsnOnForm,
            string signedName)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);
                EnsureEmployeeDocumentsTable(db);
                var hire = ResolveHireForLogin(db);
                if (hire == null)
                    return Json(new { success = false, message = "No onboarding registration found." });
                if (IsLocked(hire))
                    return Json(new { success = false, message = "Registration is locked." });

                DateTime? lawExpire = ParseNullableDate(permanentResidentExpire);
                DateTime? alienUntil = ParseNullableDate(alienAuthorizedUntil);

                string validation = ValidateI9(citizenStatus, lawExpire, lawCitizenOfId, alienUntil, alienCitizenOfId,
                    alienRegistrationNumber, admissionNumber, passportNumber, countryOfIssuanceId, federalLawAcknowledged);
                if (validation != null)
                    return Json(new { success = false, message = validation });

                var i9 = UpsertI9Fields(db, hire, citizenStatus, alienNumber, lawExpire, lawCitizenOfId, lawCitizenOfText,
                    alienUntil, alienCitizenOfId, alienCitizenOfText, alienRegistrationNumber, admissionNumber,
                    passportNumber, countryOfIssuanceId, countryOfIssuanceText, translatorNotUsed, translatorUsed,
                    federalLawAcknowledged, hideSsnOnForm);

                string name = string.IsNullOrWhiteSpace(signedName)
                    ? (hire.FirstName + " " + hire.LastName).Trim()
                    : signedName.Trim();
                string ip = Request.UserHostAddress;
                string txn = SelfOnboardingSchemaHelper.NewTransactionId();
                DateTime when = DateTime.Now;

                byte[] pdf = CreateI9PdfBytes(hire, i9, name, when, ip, txn);
                pdf = StampSignatureText(pdf, name, when, ip, txn);
                string relative = SaveHireFile(hire.HireId, "I9_signed_" + when.ToString("yyyyMMddHHmmss") + ".pdf", pdf);

                var sig = db.SelfOnboardingSignatures.FirstOrDefault(s => s.HireId == hire.HireId && s.DocumentKey == "I9");
                if (sig == null)
                {
                    sig = new SelfOnboardingSignature { HireId = hire.HireId, DocumentKey = "I9" };
                    db.SelfOnboardingSignatures.Add(sig);
                }
                sig.IsSigned = true;
                sig.SignedName = name;
                sig.SignedDate = when;
                sig.SignedIp = ip;
                sig.TransactionId = txn;
                sig.FilePath = relative;

                int? employeeId = hire.EmployeeId;
                if (!employeeId.HasValue || employeeId.Value <= 0)
                    employeeId = EnsurePersonAndEmployeeForHire(db, hire.FirstName, hire.LastName, hire.HomeEmail, hire.FileNumber, User.Identity.Name);

                if (employeeId.HasValue && employeeId.Value > 0)
                {
                    hire.EmployeeId = employeeId;
                    string empRelative = SaveEmployeeDocumentFile(employeeId.Value, "Form_I9_" + (hire.FileNumber ?? hire.HireId.ToString()) + ".pdf", pdf);
                    EmployeeDocument empDoc = null;
                    if (i9.EmployeeDocumentId.HasValue)
                        empDoc = db.EmployeeDocuments.FirstOrDefault(d => d.DocumentId == i9.EmployeeDocumentId.Value);

                    if (empDoc == null)
                    {
                        empDoc = new EmployeeDocument
                        {
                            EmployeeId = employeeId.Value,
                            FileName = "Form I-9.pdf",
                            FilePath = empRelative,
                            UploadedBy = TruncateName(User.Identity.Name ?? "self-onboarding", 100),
                            UploadedDate = when,
                            IsSigned = true,
                            SignedBy = TruncateName(User.Identity.Name ?? name, 100),
                            SignedDate = when,
                            SignerRole = "Employee",
                            SignatureName = TruncateName(name, 150)
                        };
                        db.EmployeeDocuments.Add(empDoc);
                        db.SaveChanges();
                        i9.EmployeeDocumentId = empDoc.DocumentId;
                    }
                    else
                    {
                        empDoc.FilePath = empRelative;
                        empDoc.FileName = "Form I-9.pdf";
                        empDoc.IsSigned = true;
                        empDoc.SignedBy = TruncateName(User.Identity.Name ?? name, 100);
                        empDoc.SignedDate = when;
                        empDoc.SignerRole = "Employee";
                        empDoc.SignatureName = TruncateName(name, 150);
                    }
                }

                hire.ModifiedDate = when;
                if (hire.Status == "Invited") hire.Status = "InProgress";
                if (hire.CurrentStep < 3) hire.CurrentStep = 3;
                db.SaveChanges();

                return Json(new
                {
                    success = true,
                    message = "Form I-9 signed and saved to employee documents.",
                    signedName = name,
                    signedDate = when.ToString("MM/dd/yyyy HH:mm"),
                    signedIp = ip,
                    transactionId = txn,
                    employeeDocumentId = i9.EmployeeDocumentId,
                    buttonText = "View or Unsign I9"
                });
            }
        }

        private static SelfOnboardingI9 UpsertI9Fields(
            ClientDbContext db,
            SelfOnboardingHire hire,
            int citizenStatus,
            string alienNumber,
            DateTime? lawExpire,
            int? lawCitizenOfId,
            string lawCitizenOfText,
            DateTime? alienUntil,
            int? alienCitizenOfId,
            string alienCitizenOfText,
            string alienRegistrationNumber,
            string admissionNumber,
            string passportNumber,
            int? countryOfIssuanceId,
            string countryOfIssuanceText,
            bool translatorNotUsed,
            bool translatorUsed,
            bool federalLawAcknowledged,
            bool hideSsnOnForm)
        {
            var i9 = db.SelfOnboardingI9s.FirstOrDefault(x => x.HireId == hire.HireId);
            if (i9 == null)
            {
                i9 = new SelfOnboardingI9 { HireId = hire.HireId };
                db.SelfOnboardingI9s.Add(i9);
            }

            i9.CitizenStatus = citizenStatus;
            i9.AlienNumber = TruncateName(alienNumber, 50);
            i9.PermanentResidentExpire = citizenStatus == 1 ? lawExpire : null;
            i9.LawCitizenOfId = citizenStatus == 1 ? lawCitizenOfId : null;
            i9.LawCitizenOfText = citizenStatus == 1 ? TruncateName(lawCitizenOfText, 100) : null;
            i9.AlienAuthorizedUntil = citizenStatus == 2 ? alienUntil : null;
            i9.AlienCitizenOfId = citizenStatus == 2 ? alienCitizenOfId : null;
            i9.AlienCitizenOfText = citizenStatus == 2 ? TruncateName(alienCitizenOfText, 100) : null;
            i9.AlienRegistrationNumber = TruncateName(alienRegistrationNumber, 50);
            i9.AdmissionNumber = TruncateName(admissionNumber, 50);
            i9.PassportNumber = TruncateName(passportNumber, 50);
            i9.CountryOfIssuanceId = countryOfIssuanceId;
            i9.CountryOfIssuanceText = TruncateName(countryOfIssuanceText, 100);
            i9.TranslatorNotUsed = translatorNotUsed;
            i9.TranslatorUsed = translatorUsed;
            i9.FederalLawAcknowledged = federalLawAcknowledged;
            i9.HideSsnOnForm = hideSsnOnForm;
            i9.ModifiedDate = DateTime.Now;
            return i9;
        }

        [HttpGet]
        public JsonResult GetTaxData(int? hireId = null)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);
                var hire = ResolveHireForRequest(db, hireId);
                if (hire == null)
                    return Json(new { success = false, message = "No onboarding registration found." }, JsonRequestBehavior.AllowGet);

                var tax = db.SelfOnboardingTaxes.FirstOrDefault(x => x.HireId == hire.HireId);
                var personal = db.SelfOnboardingPersonals.FirstOrDefault(x => x.HireId == hire.HireId);
                var signed = db.SelfOnboardingSignatures.Any(s => s.HireId == hire.HireId && s.DocumentKey == "W4" && s.IsSigned);

                return Json(new
                {
                    success = true,
                    data = MapTaxVm(hire.HireId, tax, personal, signed, db)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveTax(
            int? filingStatusId,
            string otherIncomeAmount,
            string deductionsAmount,
            string extraWithholdingAmount,
            string extraWithholdingPercent,
            bool federalExempt,
            bool copyFromFederal,
            int? workingCountryId,
            int? workingStateId,
            int? stateTaxStatusId,
            string stateExemptions,
            string stateAdditionalWithholdingAmount,
            string stateAdditionalWithholdingPercent,
            bool stateExempt)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);
                var hire = ResolveHireForLogin(db);
                if (hire == null)
                    return Json(new { success = false, message = "No onboarding registration found." });
                if (IsLocked(hire))
                    return Json(new { success = false, message = "Registration is locked." });

                string validation = ValidateTax(filingStatusId, federalExempt, workingCountryId, workingStateId,
                    stateTaxStatusId, stateExemptions, stateExempt, stateAdditionalWithholdingAmount, stateAdditionalWithholdingPercent);
                if (validation != null)
                    return Json(new { success = false, message = validation });

                UpsertTaxFields(db, hire, filingStatusId, otherIncomeAmount, deductionsAmount, extraWithholdingAmount,
                    extraWithholdingPercent, federalExempt, copyFromFederal, workingCountryId, workingStateId,
                    stateTaxStatusId, stateExemptions, stateAdditionalWithholdingAmount, stateAdditionalWithholdingPercent, stateExempt);

                hire.ModifiedDate = DateTime.Now;
                if (hire.Status == "Invited") hire.Status = "InProgress";
                if (hire.CurrentStep < 5) hire.CurrentStep = 5;
                db.SaveChanges();
                return Json(new { success = true, message = "Tax election saved." });
            }
        }

        [HttpPost]
        public JsonResult SaveAndSignW4(
            int? filingStatusId,
            string otherIncomeAmount,
            string deductionsAmount,
            string extraWithholdingAmount,
            string extraWithholdingPercent,
            bool federalExempt,
            bool copyFromFederal,
            int? workingCountryId,
            int? workingStateId,
            int? stateTaxStatusId,
            string stateExemptions,
            string stateAdditionalWithholdingAmount,
            string stateAdditionalWithholdingPercent,
            bool stateExempt,
            string signedName)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);
                EnsureEmployeeDocumentsTable(db);
                var hire = ResolveHireForLogin(db);
                if (hire == null)
                    return Json(new { success = false, message = "No onboarding registration found." });
                if (IsLocked(hire))
                    return Json(new { success = false, message = "Registration is locked." });

                string validation = ValidateTax(filingStatusId, federalExempt, workingCountryId, workingStateId,
                    stateTaxStatusId, stateExemptions, stateExempt, stateAdditionalWithholdingAmount, stateAdditionalWithholdingPercent);
                if (validation != null)
                    return Json(new { success = false, message = validation });

                var tax = UpsertTaxFields(db, hire, filingStatusId, otherIncomeAmount, deductionsAmount, extraWithholdingAmount,
                    extraWithholdingPercent, federalExempt, copyFromFederal, workingCountryId, workingStateId,
                    stateTaxStatusId, stateExemptions, stateAdditionalWithholdingAmount, stateAdditionalWithholdingPercent, stateExempt);

                string name = string.IsNullOrWhiteSpace(signedName)
                    ? (hire.FirstName + " " + hire.LastName).Trim()
                    : signedName.Trim();
                string ip = Request.UserHostAddress;
                string txn = SelfOnboardingSchemaHelper.NewTransactionId();
                DateTime when = DateTime.Now;

                string filingName = ResolveLookupName(db, filingStatusId);
                string stateStatusName = ResolveLookupName(db, stateTaxStatusId);
                string stateName = ResolveStateName(db, workingStateId);
                string countryName = ResolveCountryName(db, workingCountryId);

                byte[] pdf = CreateW4PdfBytes(hire, tax, filingName, stateStatusName, stateName, countryName, name, when, ip, txn);
                pdf = StampSignatureText(pdf, name, when, ip, txn);
                string relative = SaveHireFile(hire.HireId, "W4_signed_" + when.ToString("yyyyMMddHHmmss") + ".pdf", pdf);

                var sig = db.SelfOnboardingSignatures.FirstOrDefault(s => s.HireId == hire.HireId && s.DocumentKey == "W4");
                if (sig == null)
                {
                    sig = new SelfOnboardingSignature { HireId = hire.HireId, DocumentKey = "W4" };
                    db.SelfOnboardingSignatures.Add(sig);
                }
                sig.IsSigned = true;
                sig.SignedName = name;
                sig.SignedDate = when;
                sig.SignedIp = ip;
                sig.TransactionId = txn;
                sig.FilePath = relative;

                int? employeeId = hire.EmployeeId;
                if (!employeeId.HasValue || employeeId.Value <= 0)
                    employeeId = EnsurePersonAndEmployeeForHire(db, hire.FirstName, hire.LastName, hire.HomeEmail, hire.FileNumber, User.Identity.Name);

                if (employeeId.HasValue && employeeId.Value > 0)
                {
                    hire.EmployeeId = employeeId;
                    string empRelative = SaveEmployeeDocumentFile(employeeId.Value, "Form_W4_" + (hire.FileNumber ?? hire.HireId.ToString()) + ".pdf", pdf);
                    EmployeeDocument empDoc = null;
                    if (tax.EmployeeDocumentId.HasValue)
                        empDoc = db.EmployeeDocuments.FirstOrDefault(d => d.DocumentId == tax.EmployeeDocumentId.Value);

                    if (empDoc == null)
                    {
                        empDoc = new EmployeeDocument
                        {
                            EmployeeId = employeeId.Value,
                            FileName = "Form W-4.pdf",
                            FilePath = empRelative,
                            UploadedBy = TruncateName(User.Identity.Name ?? "self-onboarding", 100),
                            UploadedDate = when,
                            IsSigned = true,
                            SignedBy = TruncateName(User.Identity.Name ?? name, 100),
                            SignedDate = when,
                            SignerRole = "Employee",
                            SignatureName = TruncateName(name, 150)
                        };
                        db.EmployeeDocuments.Add(empDoc);
                        db.SaveChanges();
                        tax.EmployeeDocumentId = empDoc.DocumentId;
                    }
                    else
                    {
                        empDoc.FilePath = empRelative;
                        empDoc.FileName = "Form W-4.pdf";
                        empDoc.IsSigned = true;
                        empDoc.SignedBy = TruncateName(User.Identity.Name ?? name, 100);
                        empDoc.SignedDate = when;
                        empDoc.SignerRole = "Employee";
                        empDoc.SignatureName = TruncateName(name, 150);
                    }
                }

                hire.ModifiedDate = when;
                if (hire.Status == "Invited") hire.Status = "InProgress";
                if (hire.CurrentStep < 5) hire.CurrentStep = 5;
                db.SaveChanges();

                return Json(new
                {
                    success = true,
                    message = "Form W-4 signed and saved to employee documents.",
                    signedName = name,
                    signedDate = when.ToString("MM/dd/yyyy HH:mm"),
                    signedIp = ip,
                    transactionId = txn,
                    employeeDocumentId = tax.EmployeeDocumentId,
                    buttonText = "View or Unsign W-4"
                });
            }
        }

        private static SelfOnboardingTax UpsertTaxFields(
            ClientDbContext db,
            SelfOnboardingHire hire,
            int? filingStatusId,
            string otherIncomeAmount,
            string deductionsAmount,
            string extraWithholdingAmount,
            string extraWithholdingPercent,
            bool federalExempt,
            bool copyFromFederal,
            int? workingCountryId,
            int? workingStateId,
            int? stateTaxStatusId,
            string stateExemptions,
            string stateAdditionalWithholdingAmount,
            string stateAdditionalWithholdingPercent,
            bool stateExempt)
        {
            var tax = db.SelfOnboardingTaxes.FirstOrDefault(x => x.HireId == hire.HireId);
            if (tax == null)
            {
                tax = new SelfOnboardingTax { HireId = hire.HireId };
                db.SelfOnboardingTaxes.Add(tax);
            }

            tax.FilingStatusId = filingStatusId;
            tax.OtherIncomeAmount = ParseNullableDecimal(otherIncomeAmount);
            tax.DeductionsAmount = ParseNullableDecimal(deductionsAmount);
            tax.ExtraWithholdingAmount = ParseNullableDecimal(extraWithholdingAmount);
            tax.ExtraWithholdingPercent = ParseNullableDecimal(extraWithholdingPercent);
            tax.FederalExempt = federalExempt;
            tax.CopyFromFederal = copyFromFederal;
            tax.WorkingCountryId = workingCountryId;
            tax.WorkingStateId = workingStateId;
            tax.StateTaxStatusId = stateTaxStatusId;
            tax.StateExemptions = TruncateName(stateExemptions, 50);
            tax.StateAdditionalWithholdingAmount = ParseNullableDecimal(stateAdditionalWithholdingAmount);
            tax.StateAdditionalWithholdingPercent = ParseNullableDecimal(stateAdditionalWithholdingPercent);
            tax.StateExempt = stateExempt;
            tax.ModifiedDate = DateTime.Now;

            // Keep personal tax keys in sync for review / legacy fields
            var personal = db.SelfOnboardingPersonals.FirstOrDefault(p => p.HireId == hire.HireId);
            if (personal == null)
            {
                personal = new SelfOnboardingPersonal { HireId = hire.HireId };
                db.SelfOnboardingPersonals.Add(personal);
            }
            personal.FilingStatusId = filingStatusId;
            personal.WorkingCountryId = workingCountryId;
            personal.WorkingStateId = workingStateId;
            personal.StateTaxStatusId = stateTaxStatusId;
            personal.ModifiedDate = DateTime.Now;

            return tax;
        }

        private static decimal? ParseNullableDecimal(string value)
        {
            decimal d;
            if (string.IsNullOrWhiteSpace(value)) return null;
            return decimal.TryParse(value, out d) ? (decimal?)d : null;
        }

        private static string ValidateTax(int? filingStatusId, bool federalExempt, int? workingCountryId, int? workingStateId,
            int? stateTaxStatusId, string stateExemptions, bool stateExempt,
            string stateAdditionalAmount, string stateAdditionalPercent)
        {
            if (!federalExempt && (!filingStatusId.HasValue || filingStatusId.Value <= 0))
                return "Please select Filing Status.";
            if (!workingCountryId.HasValue || workingCountryId.Value <= 0)
                return "Please select Work in Country.";
            if (!workingStateId.HasValue || workingStateId.Value <= 0)
                return "Please select Work in State.";
            if (!stateExempt && (!stateTaxStatusId.HasValue || stateTaxStatusId.Value <= 0))
                return "Please select State Taxes Withholding Status.";
            if (!stateExempt && string.IsNullOrWhiteSpace(stateExemptions))
                return "Please enter State Taxes Exemptions.";
            return null;
        }

        private static string ResolveLookupName(ClientDbContext db, int? lookupId)
        {
            if (!lookupId.HasValue) return "";
            return db.OnboardingLookups.Where(l => l.LookupId == lookupId.Value).Select(l => l.Description).FirstOrDefault() ?? "";
        }

        private static string ResolveStateName(ClientDbContext db, int? stateId)
        {
            if (!stateId.HasValue) return "";
            return db.DdlStates.Where(s => s.StateId == stateId.Value).Select(s => s.Title).FirstOrDefault() ?? "";
        }

        private static string ResolveCountryName(ClientDbContext db, int? countryId)
        {
            if (!countryId.HasValue) return "";
            return db.DdlCountries.Where(c => c.CountryId == countryId.Value).Select(c => c.Description).FirstOrDefault() ?? "";
        }

        [HttpPost]
        public JsonResult SignDocument(string documentKey, int? profileDocumentId, string signedName)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);
                EnsureEmployeeDocumentsTable(db);
                var hire = ResolveHireForLogin(db);
                if (hire == null)
                    return Json(new { success = false, message = "No onboarding registration found." });
                if (IsLocked(hire))
                    return Json(new { success = false, message = "Registration is locked." });

                string name = string.IsNullOrWhiteSpace(signedName)
                    ? (hire.FirstName + " " + hire.LastName).Trim()
                    : signedName.Trim();
                string ip = Request.UserHostAddress;
                string txn = SelfOnboardingSchemaHelper.NewTransactionId();
                DateTime when = DateTime.Now;

                string key = documentKey ?? "DOC";
                var sig = db.SelfOnboardingSignatures.FirstOrDefault(s =>
                    s.HireId == hire.HireId && s.DocumentKey == key &&
                    (profileDocumentId == null || s.ProfileDocumentId == profileDocumentId));

                if (sig == null)
                {
                    sig = new SelfOnboardingSignature
                    {
                        HireId = hire.HireId,
                        DocumentKey = key,
                        ProfileDocumentId = profileDocumentId
                    };
                    db.SelfOnboardingSignatures.Add(sig);
                }

                sig.IsSigned = true;
                sig.SignedName = name;
                sig.SignedDate = when;
                sig.SignedIp = ip;
                sig.TransactionId = txn;

                string displayName = "Signed Document.pdf";
                byte[] signedPdf = null;

                // Stamp PDF when library file exists
                string sourcePath = null;
                OnboardingProfileDocument profileDoc = null;
                if (profileDocumentId.HasValue)
                {
                    profileDoc = db.OnboardingProfileDocuments.FirstOrDefault(d => d.ProfileDocumentId == profileDocumentId.Value);
                    if (profileDoc != null)
                    {
                        displayName = string.IsNullOrWhiteSpace(profileDoc.DocumentName)
                            ? (profileDoc.FileName ?? "Onboarding Document.pdf")
                            : profileDoc.DocumentName + ".pdf";
                        if (!displayName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                            displayName += ".pdf";
                        if (!string.IsNullOrEmpty(profileDoc.FilePath))
                            sourcePath = Server.MapPath("~/App_Data/" + profileDoc.FilePath);
                    }
                }
                else if (string.Equals(key, "I9", StringComparison.OrdinalIgnoreCase) || string.Equals(key, "W4", StringComparison.OrdinalIgnoreCase))
                {
                    displayName = "Form " + key + ".pdf";
                    sourcePath = CreateBlankFormPdf(key, hire);
                }

                if (!string.IsNullOrEmpty(sourcePath) && System.IO.File.Exists(sourcePath))
                {
                    signedPdf = System.IO.File.ReadAllBytes(sourcePath);
                    signedPdf = StampSignatureText(signedPdf, name, when, ip, txn);
                    string relative = SaveHireFile(hire.HireId, key + "_signed_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf", signedPdf);
                    sig.FilePath = relative;
                }
                else if (string.Equals(key, "I9", StringComparison.OrdinalIgnoreCase) || string.Equals(key, "W4", StringComparison.OrdinalIgnoreCase))
                {
                    signedPdf = CreateBlankFormPdfBytes(key, hire);
                    signedPdf = StampSignatureText(signedPdf, name, when, ip, txn);
                    string relative = SaveHireFile(hire.HireId, key + "_signed_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf", signedPdf);
                    sig.FilePath = relative;
                }

                // Also store signed PDF in EmployeeDocuments so employee can see it under My Documents
                if (signedPdf != null && signedPdf.Length > 0)
                {
                    int? employeeId = hire.EmployeeId;
                    if (!employeeId.HasValue || employeeId.Value <= 0)
                        employeeId = EnsurePersonAndEmployeeForHire(db, hire.FirstName, hire.LastName, hire.HomeEmail, hire.FileNumber, User.Identity.Name);

                    if (employeeId.HasValue && employeeId.Value > 0)
                    {
                        hire.EmployeeId = employeeId;
                        string safeFile = MakeSafeFileName(displayName);
                        string empRelative = SaveEmployeeDocumentFile(employeeId.Value,
                            Path.GetFileNameWithoutExtension(safeFile) + "_" + when.ToString("yyyyMMddHHmmss") + ".pdf",
                            signedPdf);

                        EmployeeDocument empDoc = null;
                        if (sig.EmployeeDocumentId.HasValue)
                            empDoc = db.EmployeeDocuments.FirstOrDefault(d => d.DocumentId == sig.EmployeeDocumentId.Value);

                        if (empDoc == null)
                        {
                            empDoc = new EmployeeDocument
                            {
                                EmployeeId = employeeId.Value,
                                FileName = TruncateName(displayName, 260),
                                FilePath = empRelative,
                                UploadedBy = TruncateName(User.Identity.Name ?? "self-onboarding", 100),
                                UploadedDate = when,
                                IsSigned = true,
                                SignedBy = TruncateName(User.Identity.Name ?? name, 100),
                                SignedDate = when,
                                SignerRole = "Employee",
                                SignatureName = TruncateName(name, 150)
                            };
                            db.EmployeeDocuments.Add(empDoc);
                            db.SaveChanges();
                            sig.EmployeeDocumentId = empDoc.DocumentId;
                        }
                        else
                        {
                            empDoc.FileName = TruncateName(displayName, 260);
                            empDoc.FilePath = empRelative;
                            empDoc.IsSigned = true;
                            empDoc.SignedBy = TruncateName(User.Identity.Name ?? name, 100);
                            empDoc.SignedDate = when;
                            empDoc.SignerRole = "Employee";
                            empDoc.SignatureName = TruncateName(name, 150);
                        }
                    }
                }

                hire.ModifiedDate = DateTime.Now;
                if (hire.Status == "Invited") hire.Status = "InProgress";
                if (hire.CurrentStep < 4) hire.CurrentStep = 4;
                db.SaveChanges();

                return Json(new
                {
                    success = true,
                    message = "Signature saved to employee documents.",
                    signedName = name,
                    signedDate = when.ToString("MM/dd/yyyy HH:mm"),
                    signedIp = ip,
                    transactionId = txn,
                    employeeDocumentId = sig.EmployeeDocumentId,
                    buttonText = key == "DOC" ? "Unsign" : ("Unsign " + key)
                });
            }
        }

        private static string MakeSafeFileName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "document.pdf";
            foreach (char c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name.Trim();
        }

        [HttpPost]
        public JsonResult UnsignDocument(string documentKey, int? profileDocumentId)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);
                var hire = ResolveHireForLogin(db);
                if (hire == null)
                    return Json(new { success = false, message = "No onboarding registration found." });
                if (IsLocked(hire))
                    return Json(new { success = false, message = "Registration is locked." });

                string key = documentKey ?? "";
                var sig = db.SelfOnboardingSignatures.FirstOrDefault(s =>
                    s.HireId == hire.HireId && s.DocumentKey == key &&
                    (profileDocumentId == null || s.ProfileDocumentId == profileDocumentId));
                if (sig != null)
                {
                    sig.IsSigned = false;
                    sig.SignedName = null;
                    sig.SignedDate = null;
                    sig.SignedIp = null;
                    sig.TransactionId = null;

                    if (sig.EmployeeDocumentId.HasValue)
                    {
                        var linkedDoc = db.EmployeeDocuments.FirstOrDefault(d => d.DocumentId == sig.EmployeeDocumentId.Value);
                        if (linkedDoc != null)
                        {
                            linkedDoc.IsSigned = false;
                            linkedDoc.SignedBy = null;
                            linkedDoc.SignedDate = null;
                            linkedDoc.SignerRole = null;
                            linkedDoc.SignatureName = null;
                            linkedDoc.SignatureImagePath = null;
                        }
                    }
                    db.SaveChanges();
                }

                if (string.Equals(key, "I9", StringComparison.OrdinalIgnoreCase))
                {
                    var i9 = db.SelfOnboardingI9s.FirstOrDefault(x => x.HireId == hire.HireId);
                    if (i9 != null && i9.EmployeeDocumentId.HasValue)
                    {
                        var empDoc = db.EmployeeDocuments.FirstOrDefault(d => d.DocumentId == i9.EmployeeDocumentId.Value);
                        if (empDoc != null)
                        {
                            empDoc.IsSigned = false;
                            empDoc.SignedBy = null;
                            empDoc.SignedDate = null;
                            empDoc.SignerRole = null;
                            empDoc.SignatureName = null;
                            empDoc.SignatureImagePath = null;
                            db.SaveChanges();
                        }
                    }
                }
                else if (string.Equals(key, "W4", StringComparison.OrdinalIgnoreCase))
                {
                    var tax = db.SelfOnboardingTaxes.FirstOrDefault(x => x.HireId == hire.HireId);
                    if (tax != null && tax.EmployeeDocumentId.HasValue)
                    {
                        var empDoc = db.EmployeeDocuments.FirstOrDefault(d => d.DocumentId == tax.EmployeeDocumentId.Value);
                        if (empDoc != null)
                        {
                            empDoc.IsSigned = false;
                            empDoc.SignedBy = null;
                            empDoc.SignedDate = null;
                            empDoc.SignerRole = null;
                            empDoc.SignatureName = null;
                            empDoc.SignatureImagePath = null;
                            db.SaveChanges();
                        }
                    }
                }

                return Json(new { success = true, buttonText = key == "DOC" ? "Sign" : (key == "I9" ? "View & Sign I9" : (key == "W4" ? "View and Sign W-4" : ("Sign " + key))) });
            }
        }

        [HttpGet]
        public JsonResult GetBankAccounts(int? hireId = null)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);
                var hire = ResolveHireForRequest(db, hireId);
                if (hire == null)
                    return Json(new { success = false, message = "No onboarding registration found.", count = 0, data = new object[0] }, JsonRequestBehavior.AllowGet);

                var accountTypes = db.OnboardingLookups.Where(l => l.LookupType == "AccountType").ToList();
                var banks = db.SelfOnboardingBankAccounts.Where(b => b.HireId == hire.HireId).ToList()
                    .Select(b => new
                    {
                        BankAccountId = b.BankAccountId,
                        AccountTypeId = b.AccountTypeId,
                        AccountTypeName = accountTypes.Where(t => t.LookupId == b.AccountTypeId).Select(t => t.Description).FirstOrDefault() ?? "",
                        BankName = b.BankName ?? "",
                        RoutingNumber = b.RoutingNumber ?? "",
                        AccountNumber = b.AccountNumber ?? "",
                        IsPrimary = b.IsPrimary
                    }).ToList();

                return Json(new { success = true, count = banks.Count, data = banks }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveBankAccount(int bankAccountId, int? accountTypeId, string bankName,
            string routingNumber, string accountNumber, bool isPrimary)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);
                var hire = ResolveHireForLogin(db);
                if (hire == null)
                    return Json(new { success = false, message = "No onboarding registration found." });
                if (IsLocked(hire))
                    return Json(new { success = false, message = "Registration is locked." });

                SelfOnboardingBankAccount entity;
                if (bankAccountId > 0)
                {
                    entity = db.SelfOnboardingBankAccounts.FirstOrDefault(b => b.BankAccountId == bankAccountId && b.HireId == hire.HireId);
                    if (entity == null)
                        return Json(new { success = false, message = "Account not found." });
                }
                else
                {
                    entity = new SelfOnboardingBankAccount { HireId = hire.HireId };
                    db.SelfOnboardingBankAccounts.Add(entity);
                }

                if (isPrimary)
                {
                    foreach (var b in db.SelfOnboardingBankAccounts.Where(x => x.HireId == hire.HireId))
                        b.IsPrimary = false;
                }

                entity.AccountTypeId = accountTypeId;
                entity.BankName = bankName;
                entity.RoutingNumber = routingNumber;
                entity.AccountNumber = accountNumber;
                entity.IsPrimary = isPrimary;
                hire.ModifiedDate = DateTime.Now;
                if (hire.Status == "Invited") hire.Status = "InProgress";
                if (hire.CurrentStep < 7) hire.CurrentStep = Math.Max(hire.CurrentStep, 6);
                db.SaveChanges();

                string typeName = null;
                if (accountTypeId.HasValue)
                {
                    typeName = db.OnboardingLookups
                        .Where(l => l.LookupId == accountTypeId.Value)
                        .Select(l => l.Description)
                        .FirstOrDefault();
                }

                return Json(new
                {
                    success = true,
                    bankAccountId = entity.BankAccountId,
                    accountTypeName = typeName ?? "",
                    bankName = entity.BankName ?? "",
                    routingNumber = entity.RoutingNumber ?? "",
                    accountNumber = entity.AccountNumber ?? "",
                    isPrimary = entity.IsPrimary,
                    count = db.SelfOnboardingBankAccounts.Count(b => b.HireId == hire.HireId)
                });
            }
        }

        [HttpPost]
        public JsonResult DeleteBankAccount(int bankAccountId)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                var hire = ResolveHireForLogin(db);
                if (hire == null)
                    return Json(new { success = false, message = "No onboarding registration found." });
                if (IsLocked(hire))
                    return Json(new { success = false, message = "Registration is locked." });

                var entity = db.SelfOnboardingBankAccounts.FirstOrDefault(b => b.BankAccountId == bankAccountId && b.HireId == hire.HireId);
                if (entity == null)
                    return Json(new { success = false, message = "Account not found." });
                db.SelfOnboardingBankAccounts.Remove(entity);
                db.SaveChanges();
                return Json(new { success = true });
            }
        }

        [HttpPost]
        public JsonResult UploadDocument(int profileDocumentId)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);
                var hire = ResolveHireForLogin(db);
                if (hire == null)
                    return Json(new { success = false, message = "No onboarding registration found." });
                if (IsLocked(hire))
                    return Json(new { success = false, message = "Registration is locked." });

                if (Request.Files == null || Request.Files.Count == 0)
                    return Json(new { success = false, message = "Choose a file." });

                var file = Request.Files[0];
                if (file == null || file.ContentLength == 0)
                    return Json(new { success = false, message = "Choose a file." });

                string root = Server.MapPath("~/App_Data/" + SelfOnboardingSchemaHelper.StorageFolderName + "/" + hire.HireId);
                Directory.CreateDirectory(root);
                string safe = Path.GetFileName(file.FileName);
                string name = Path.GetFileNameWithoutExtension(safe) + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(safe);
                string physical = Path.Combine(root, name);
                file.SaveAs(physical);
                string relative = Path.Combine(SelfOnboardingSchemaHelper.StorageFolderName, hire.HireId.ToString(), name).Replace('\\', '/');

                var upload = new SelfOnboardingUpload
                {
                    HireId = hire.HireId,
                    ProfileDocumentId = profileDocumentId,
                    FileName = safe,
                    FilePath = relative,
                    UploadedDate = DateTime.Now,
                    IsSigned = false
                };
                db.SelfOnboardingUploads.Add(upload);
                db.SaveChanges();
                return Json(new { success = true, uploadId = upload.UploadId, fileName = safe });
            }
        }

        [HttpPost]
        public JsonResult SubmitOnboarding()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);
                var hire = ResolveHireForLogin(db);
                if (hire == null)
                    return Json(new { success = false, message = "No onboarding registration found." });
                if (hire.Status == "Submitted" || hire.Status == "Hired")
                    return Json(new { success = false, message = "Already submitted." });

                string txn = hire.TransactionId ?? SelfOnboardingSchemaHelper.NewTransactionId();
                hire.TransactionId = txn;
                hire.Status = "Submitted";
                hire.SubmittedDate = DateTime.Now;
                hire.ConfirmationDate = DateTime.Now;
                hire.CurrentStep = 7;
                hire.RejectionReason = null;
                hire.RejectedFormName = null;
                hire.RejectedBy = null;
                hire.RejectedDate = null;
                hire.ModifiedDate = DateTime.Now;
                db.SaveChanges();

                return Json(new
                {
                    success = true,
                    message = "Congratulations! You have completed your new hire documents.",
                    transactionId = txn,
                    confirmationDate = hire.ConfirmationDate.Value.ToString("MM/dd/yyyy HH:mm")
                });
            }
        }

        [HttpGet]
        public ActionResult DownloadDocument(int profileDocumentId, bool preferSigned = true, int? hireId = null)
        {
            string connString = User.Identity.GetClientConnectionString();
            if (string.IsNullOrWhiteSpace(connString))
                connString = ConfigurationManager.ConnectionStrings["execView1"].ConnectionString;

            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);
                var hire = ResolveHireForRequest(db, hireId);
                if (hire == null)
                    return new HttpUnauthorizedResult();

                // Prefer signed PDF for this hire when available
                if (preferSigned)
                {
                    var sig = db.SelfOnboardingSignatures.FirstOrDefault(s =>
                        s.HireId == hire.HireId &&
                        s.ProfileDocumentId == profileDocumentId &&
                        s.IsSigned &&
                        s.FilePath != null && s.FilePath != "");
                    if (sig != null)
                    {
                        string signedPhysical = Server.MapPath("~/App_Data/" + sig.FilePath);
                        if (System.IO.File.Exists(signedPhysical))
                        {
                            string signedName = Path.GetFileName(signedPhysical);
                            Response.AppendHeader("Content-Disposition", "inline; filename=\"" + signedName + "\"");
                            return File(signedPhysical, "application/pdf");
                        }
                    }
                }

                var doc = db.OnboardingProfileDocuments.FirstOrDefault(d => d.ProfileDocumentId == profileDocumentId && d.IsActive);
                if (doc != null)
                {
                    // Ensure document belongs to this hire's onboarding profile
                    if (hire.ProfileId.HasValue && doc.ProfileId != hire.ProfileId.Value)
                        return new HttpUnauthorizedResult();

                    if (!string.IsNullOrEmpty(doc.FilePath))
                    {
                        string physical = Server.MapPath("~/App_Data/" + doc.FilePath);
                        if (System.IO.File.Exists(physical))
                        {
                            string fileName = string.IsNullOrWhiteSpace(doc.FileName) ? Path.GetFileName(physical) : doc.FileName;
                            Response.AppendHeader("Content-Disposition", "inline; filename=\"" + fileName.Replace("\"", "") + "\"");
                            return File(physical, "application/pdf");
                        }
                    }
                }

                // Fall back to the hire's latest uploaded copy for this document
                var upload = db.SelfOnboardingUploads
                    .Where(u => u.HireId == hire.HireId && u.ProfileDocumentId == profileDocumentId)
                    .OrderByDescending(u => u.UploadedDate)
                    .FirstOrDefault();
                if (upload != null && !string.IsNullOrEmpty(upload.FilePath))
                {
                    string uploadPhysical = Server.MapPath("~/App_Data/" + upload.FilePath);
                    if (System.IO.File.Exists(uploadPhysical))
                    {
                        string uploadName = string.IsNullOrWhiteSpace(upload.FileName) ? Path.GetFileName(uploadPhysical) : upload.FileName;
                        Response.AppendHeader("Content-Disposition", "inline; filename=\"" + uploadName.Replace("\"", "") + "\"");
                        return File(uploadPhysical, "application/pdf");
                    }
                }

                return HttpNotFound("Document file not found.");
            }
        }

        [HttpGet]
        public ActionResult DownloadSigned(string documentKey, int? profileDocumentId, int? hireId)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingHire hire = null;
                if (hireId.HasValue && hireId.Value > 0)
                    hire = db.SelfOnboardingHires.FirstOrDefault(h => h.HireId == hireId.Value);
                if (hire == null)
                    hire = ResolveHireForLogin(db);
                if (hire == null)
                    return HttpNotFound();

                var sig = db.SelfOnboardingSignatures.FirstOrDefault(s =>
                    s.HireId == hire.HireId && s.DocumentKey == documentKey &&
                    (profileDocumentId == null || s.ProfileDocumentId == profileDocumentId));
                if (sig == null || string.IsNullOrEmpty(sig.FilePath))
                    return HttpNotFound();

                string physical = Server.MapPath("~/App_Data/" + sig.FilePath);
                if (!System.IO.File.Exists(physical))
                    return HttpNotFound();

                string fileName = Path.GetFileName(physical);
                Response.AppendHeader("Content-Disposition", "inline; filename=\"" + fileName + "\"");
                return File(physical, "application/pdf");
            }
        }

        #endregion

        #region Helpers

        private SelfOnboardingHire ResolveHireForRequest(ClientDbContext db, int? hireId)
        {
            if (hireId.HasValue && hireId.Value > 0)
                return db.SelfOnboardingHires.FirstOrDefault(h => h.HireId == hireId.Value);
            return ResolveHireForLogin(db);
        }

        private SelfOnboardingHire ResolveHireForLogin(ClientDbContext db)
        {
            string userName = (User.Identity.Name ?? "").Trim();
            if (string.IsNullOrEmpty(userName))
                return null;

            // Prefer exact username (emails may be shared across hires)
            var byUserName = db.SelfOnboardingHires
                .Where(h => h.GeneratedUserName == userName)
                .OrderByDescending(h => h.CreatedDate)
                .FirstOrDefault();
            if (byUserName != null)
                return byUserName;

            var asp = db.AspNetUsers.FirstOrDefault(u => u.UserName == userName);
            if (asp != null)
            {
                var byAspId = db.SelfOnboardingHires
                    .Where(h => h.AspNetUserId == asp.Id)
                    .OrderByDescending(h => h.CreatedDate)
                    .FirstOrDefault();
                if (byAspId != null)
                    return byAspId;
            }

            // Fallback: login value is an email (or matches HomeEmail)
            string aspEmail = asp != null ? asp.Email : null;
            var openStatuses = new[] { "Invited", "InProgress", "ChangesRequested", "Submitted" };
            var byEmail = db.SelfOnboardingHires
                .Where(h => h.HomeEmail == userName || (aspEmail != null && h.HomeEmail == aspEmail))
                .OrderByDescending(h => openStatuses.Contains(h.Status))
                .ThenByDescending(h => h.CreatedDate)
                .FirstOrDefault();
            if (byEmail != null)
                return byEmail;

            var person = db.Persons.FirstOrDefault(p => p.eMail == userName || (aspEmail != null && p.eMail == aspEmail));
            if (person == null || string.IsNullOrWhiteSpace(person.eMail))
                return null;

            return db.SelfOnboardingHires
                .Where(h => h.HomeEmail == person.eMail)
                .OrderByDescending(h => openStatuses.Contains(h.Status))
                .ThenByDescending(h => h.CreatedDate)
                .FirstOrDefault();
        }

        private static bool IsLocked(SelfOnboardingHire hire)
        {
            return hire.Status == "Submitted" || hire.Status == "Hired";
        }

        private void SavePersonal(ClientDbContext db, SelfOnboardingHire hire, SelfOnboardingPersonalVm vm)
        {
            var p = db.SelfOnboardingPersonals.FirstOrDefault(x => x.HireId == hire.HireId);
            if (p == null)
            {
                p = new SelfOnboardingPersonal { HireId = hire.HireId };
                db.SelfOnboardingPersonals.Add(p);
            }
            p.PrefixId = vm.PrefixId;
            p.SuffixId = vm.SuffixId;
            p.FirstName = string.IsNullOrWhiteSpace(vm.FirstName) ? hire.FirstName : vm.FirstName.Trim();
            p.MiddleName = vm.MiddleName;
            p.LastName = string.IsNullOrWhiteSpace(vm.LastName) ? hire.LastName : vm.LastName.Trim();
            p.PreferredName = vm.PreferredName;
            p.WorkEmail = string.IsNullOrWhiteSpace(vm.WorkEmail) ? hire.WorkEmail : vm.WorkEmail;
            p.HomeEmail = string.IsNullOrWhiteSpace(vm.HomeEmail) ? hire.HomeEmail : vm.HomeEmail;
            p.Phone = vm.Phone;
            p.DateOfBirth = vm.DateOfBirth;
            p.SSN = vm.SSN;
            p.GenderId = vm.GenderId;
            p.MaritalStatusId = vm.MaritalStatusId;
            p.EthnicityId = vm.EthnicityId;
            p.Address1 = vm.Address1;
            p.Address2 = vm.Address2;
            p.City = vm.City;
            p.StateId = vm.StateId;
            p.Zip = vm.Zip;
            p.CountryId = vm.CountryId;
            p.LicenseCountryId = vm.LicenseCountryId;
            p.EmergencyName = vm.EmergencyName;
            p.EmergencyPhone = vm.EmergencyPhone;
            p.RelationshipTypeId = vm.RelationshipTypeId;
            p.ModifiedDate = DateTime.Now;

            hire.FirstName = p.FirstName;
            hire.LastName = p.LastName;
            hire.WorkEmail = p.WorkEmail;
        }

        private SelfOnboardingWizardVm BuildWizardVm(ClientDbContext db, int hireId)
        {
            var hire = db.SelfOnboardingHires.FirstOrDefault(h => h.HireId == hireId);
            if (hire == null) return null;

            string profileName = null;
            if (hire.ProfileId.HasValue)
            {
                var pr = db.OnboardingProfiles.FirstOrDefault(p => p.ProfileId == hire.ProfileId.Value);
                if (pr != null) profileName = pr.ProfileName;
            }

            var personal = db.SelfOnboardingPersonals.FirstOrDefault(p => p.HireId == hireId);
            var sigs = db.SelfOnboardingSignatures.Where(s => s.HireId == hireId).ToList();
            var flags = sigs.Where(s => s.IsSigned)
                .GroupBy(s => s.DocumentKey + (s.ProfileDocumentId.HasValue ? (":" + s.ProfileDocumentId) : ""))
                .ToDictionary(g => g.Key, g => true);

            var docs = new List<OnboardingProfileDocumentVm>();
            if (hire.ProfileId.HasValue)
            {
                var types = db.OnboardingLookups.Where(l => l.LookupType == "DocumentType").ToList();
                docs = db.OnboardingProfileDocuments
                    .Where(d => d.ProfileId == hire.ProfileId.Value && d.IsActive)
                    .OrderBy(d => d.SortOrder)
                    .ToList()
                    .Select(d => new OnboardingProfileDocumentVm
                    {
                        ProfileDocumentId = d.ProfileDocumentId,
                        ProfileId = d.ProfileId,
                        DocumentName = d.DocumentName,
                        DocumentTypeId = d.DocumentTypeId,
                        DocumentTypeName = types.Where(t => t.LookupId == d.DocumentTypeId).Select(t => t.Description).FirstOrDefault(),
                        FileName = d.FileName,
                        FilePath = d.FilePath,
                        RequiresSignature = d.RequiresSignature,
                        EnableUpload = d.EnableUpload,
                        SortOrder = d.SortOrder,
                        IsActive = d.IsActive,
                        IsSigned = sigs.Any(s => s.ProfileDocumentId == d.ProfileDocumentId && s.IsSigned)
                    })
                    .ToList();
            }

            var accountTypes = db.OnboardingLookups.Where(l => l.LookupType == "AccountType").ToList();
            var banks = db.SelfOnboardingBankAccounts.Where(b => b.HireId == hireId).ToList()
                .Select(b => new SelfOnboardingBankVm
                {
                    BankAccountId = b.BankAccountId,
                    HireId = b.HireId,
                    AccountTypeId = b.AccountTypeId,
                    AccountTypeName = accountTypes.Where(t => t.LookupId == b.AccountTypeId).Select(t => t.Description).FirstOrDefault(),
                    BankName = b.BankName,
                    RoutingNumber = b.RoutingNumber,
                    AccountNumber = b.AccountNumber,
                    IsPrimary = b.IsPrimary
                }).ToList();

            var i9Entity = db.SelfOnboardingI9s.FirstOrDefault(x => x.HireId == hireId);
            bool i9Signed = sigs.Any(s => s.DocumentKey == "I9" && s.IsSigned);
            var taxEntity = db.SelfOnboardingTaxes.FirstOrDefault(x => x.HireId == hireId);
            bool w4Signed = sigs.Any(s => s.DocumentKey == "W4" && s.IsSigned);

            return new SelfOnboardingWizardVm
            {
                Hire = new SelfOnboardingHireVm
                {
                    HireId = hire.HireId,
                    PositionId = hire.PositionId,
                    PositionTitle = hire.PositionTitle,
                    ProfileId = hire.ProfileId,
                    ProfileName = profileName,
                    FirstName = hire.FirstName,
                    LastName = hire.LastName,
                    HomeEmail = hire.HomeEmail,
                    WorkEmail = hire.WorkEmail,
                    FileNumber = hire.FileNumber,
                    GeneratedUserName = hire.GeneratedUserName,
                    Status = hire.Status,
                    CurrentStep = hire.CurrentStep,
                    TransactionId = hire.TransactionId,
                    SubmittedDate = hire.SubmittedDate,
                    ConfirmationDate = hire.ConfirmationDate,
                    RejectionReason = hire.RejectionReason,
                    RejectedFormName = hire.RejectedFormName,
                    RejectedBy = hire.RejectedBy,
                    RejectedDate = hire.RejectedDate
                },
                Personal = personal == null ? new SelfOnboardingPersonalVm
                {
                    HireId = hire.HireId,
                    FirstName = hire.FirstName,
                    LastName = hire.LastName,
                    HomeEmail = hire.HomeEmail,
                    WorkEmail = hire.WorkEmail
                } : new SelfOnboardingPersonalVm
                {
                    HireId = personal.HireId,
                    PrefixId = personal.PrefixId,
                    SuffixId = personal.SuffixId,
                    FirstName = personal.FirstName,
                    MiddleName = personal.MiddleName,
                    LastName = personal.LastName,
                    PreferredName = personal.PreferredName,
                    WorkEmail = personal.WorkEmail,
                    HomeEmail = personal.HomeEmail,
                    Phone = personal.Phone,
                    DateOfBirth = personal.DateOfBirth,
                    SSN = personal.SSN,
                    GenderId = personal.GenderId,
                    MaritalStatusId = personal.MaritalStatusId,
                    EthnicityId = personal.EthnicityId,
                    Address1 = personal.Address1,
                    Address2 = personal.Address2,
                    City = personal.City,
                    StateId = personal.StateId,
                    Zip = personal.Zip,
                    CountryId = personal.CountryId,
                    LicenseCountryId = personal.LicenseCountryId,
                    EmergencyName = personal.EmergencyName,
                    EmergencyPhone = personal.EmergencyPhone,
                    RelationshipTypeId = personal.RelationshipTypeId,
                    FilingStatusId = personal.FilingStatusId,
                    WorkingCountryId = personal.WorkingCountryId,
                    WorkingStateId = personal.WorkingStateId,
                    StateTaxStatusId = personal.StateTaxStatusId
                },
                I9 = MapI9Vm(hire.HireId, i9Entity, i9Signed),
                Tax = MapTaxVm(hire.HireId, taxEntity, personal, w4Signed, db),
                Documents = docs,
                BankAccounts = banks,
                SignatureFlags = flags,
                IsReadOnly = IsLocked(hire)
            };
        }

        private static SelfOnboardingI9Vm MapI9Vm(int hireId, SelfOnboardingI9 i9, bool isSigned)
        {
            if (i9 == null)
            {
                return new SelfOnboardingI9Vm
                {
                    HireId = hireId,
                    CitizenStatus = 0,
                    TranslatorNotUsed = true,
                    IsSigned = isSigned
                };
            }
            return new SelfOnboardingI9Vm
            {
                HireId = i9.HireId,
                CitizenStatus = i9.CitizenStatus,
                AlienNumber = i9.AlienNumber,
                PermanentResidentExpire = i9.PermanentResidentExpire,
                LawCitizenOfId = i9.LawCitizenOfId,
                LawCitizenOfText = i9.LawCitizenOfText,
                AlienAuthorizedUntil = i9.AlienAuthorizedUntil,
                AlienCitizenOfId = i9.AlienCitizenOfId,
                AlienCitizenOfText = i9.AlienCitizenOfText,
                AlienRegistrationNumber = i9.AlienRegistrationNumber,
                AdmissionNumber = i9.AdmissionNumber,
                PassportNumber = i9.PassportNumber,
                CountryOfIssuanceId = i9.CountryOfIssuanceId,
                CountryOfIssuanceText = i9.CountryOfIssuanceText,
                TranslatorNotUsed = i9.TranslatorNotUsed,
                TranslatorUsed = i9.TranslatorUsed,
                FederalLawAcknowledged = i9.FederalLawAcknowledged,
                HideSsnOnForm = i9.HideSsnOnForm,
                IsSigned = isSigned,
                EmployeeDocumentId = i9.EmployeeDocumentId
            };
        }

        private static SelfOnboardingTaxVm MapTaxVm(int hireId, SelfOnboardingTax tax, SelfOnboardingPersonal personal, bool isSigned, ClientDbContext db)
        {
            if (tax == null)
            {
                return new SelfOnboardingTaxVm
                {
                    HireId = hireId,
                    FilingStatusId = personal != null ? personal.FilingStatusId : null,
                    WorkingCountryId = personal != null ? personal.WorkingCountryId : null,
                    WorkingStateId = personal != null ? personal.WorkingStateId : null,
                    StateTaxStatusId = personal != null ? personal.StateTaxStatusId : null,
                    ExtraWithholdingPercent = 0,
                    StateAdditionalWithholdingPercent = 0,
                    IsSigned = isSigned
                };
            }
            return new SelfOnboardingTaxVm
            {
                HireId = tax.HireId,
                FilingStatusId = tax.FilingStatusId,
                FilingStatusName = ResolveLookupName(db, tax.FilingStatusId),
                OtherIncomeAmount = ToDouble(tax.OtherIncomeAmount),
                DeductionsAmount = ToDouble(tax.DeductionsAmount),
                ExtraWithholdingAmount = ToDouble(tax.ExtraWithholdingAmount),
                ExtraWithholdingPercent = ToDouble(tax.ExtraWithholdingPercent),
                FederalExempt = tax.FederalExempt,
                CopyFromFederal = tax.CopyFromFederal,
                WorkingCountryId = tax.WorkingCountryId,
                WorkingStateId = tax.WorkingStateId,
                StateTaxStatusId = tax.StateTaxStatusId,
                StateTaxStatusName = ResolveLookupName(db, tax.StateTaxStatusId),
                StateExemptions = tax.StateExemptions,
                StateAdditionalWithholdingAmount = ToDouble(tax.StateAdditionalWithholdingAmount),
                StateAdditionalWithholdingPercent = ToDouble(tax.StateAdditionalWithholdingPercent),
                StateExempt = tax.StateExempt,
                IsSigned = isSigned,
                EmployeeDocumentId = tax.EmployeeDocumentId
            };
        }

        private static double? ToDouble(decimal? value)
        {
            return value.HasValue ? (double?)Convert.ToDouble(value.Value) : null;
        }

        private static string ValidateI9(int citizenStatus, DateTime? lawExpire, int? lawCitizenOfId,
            DateTime? alienUntil, int? alienCitizenOfId,
            string alienRegistration, string admission, string passport, int? countryOfIssuanceId,
            bool federalLaw)
        {
            if (!federalLaw)
                return "Please check that you agree to federal law.";

            if (citizenStatus == 1)
            {
                if (!lawExpire.HasValue)
                    return "Please select an expiration date.";
                if (lawExpire.Value.Date <= DateTime.Today)
                    return "Please enter an expiration date after the current date.";
                if (!lawCitizenOfId.HasValue || lawCitizenOfId.Value <= 0)
                    return "Please select Citizen of.";
            }
            if (citizenStatus == 2)
            {
                if (!alienUntil.HasValue)
                    return "Please select an expiration date.";
                if (alienUntil.Value.Date <= DateTime.Today)
                    return "Please enter an expiration date after the current date.";
                if (!alienCitizenOfId.HasValue || alienCitizenOfId.Value <= 0)
                    return "Please select Citizen of.";
            }

            bool hasAlienReg = !string.IsNullOrWhiteSpace(alienRegistration);
            bool hasAdmission = !string.IsNullOrWhiteSpace(admission);
            bool hasPassport = !string.IsNullOrWhiteSpace(passport) || (countryOfIssuanceId.HasValue && countryOfIssuanceId.Value > 0);
            if (hasAlienReg && (hasAdmission || hasPassport))
                return "Please enter either an Alien Registration Number/USCIS Number OR Form I-94 Admission Number.";

            return null;
        }

        private static void EnsureEmployeeDocumentsTable(ClientDbContext db)
        {
            db.Database.ExecuteSqlCommand(@"
IF OBJECT_ID(N'[dbo].[EmployeeDocuments]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[EmployeeDocuments] (
        [DocumentId]   INT            IDENTITY (1, 1) NOT NULL,
        [EmployeeId]   INT            NOT NULL,
        [FileName]     NVARCHAR (260) NOT NULL,
        [FilePath]     NVARCHAR (500) NOT NULL,
        [UploadedBy]   NVARCHAR (100) NOT NULL,
        [UploadedDate] DATETIME       NOT NULL,
        [IsSigned]     BIT            NOT NULL CONSTRAINT [DF_EmployeeDocuments_IsSigned_SO] DEFAULT (0),
        [SignedBy]     NVARCHAR (100) NULL,
        [SignedDate]   DATETIME       NULL,
        [SignerRole]   NVARCHAR (20)  NULL,
        [SignatureName] NVARCHAR (150) NULL,
        [SignatureImagePath] NVARCHAR (500) NULL,
        CONSTRAINT [PK_EmployeeDocuments] PRIMARY KEY CLUSTERED ([DocumentId] ASC)
    );
END
");
        }

        private string SaveEmployeeDocumentFile(int employeeId, string fileName, byte[] bytes)
        {
            const string folder = "EmployeeDocuments";
            string root = Server.MapPath("~/App_Data/" + folder + "/" + employeeId);
            Directory.CreateDirectory(root);
            string safe = Path.GetFileName(fileName);
            string physical = Path.Combine(root, safe);
            System.IO.File.WriteAllBytes(physical, bytes);
            return Path.Combine(folder, employeeId.ToString(), safe).Replace('\\', '/');
        }

        private static byte[] CreateW4PdfBytes(SelfOnboardingHire hire, SelfOnboardingTax tax,
            string filingName, string stateStatusName, string stateName, string countryName,
            string signedName, DateTime when, string ip, string txn)
        {
            using (var doc = new PdfDocument())
            {
                var page = doc.AddPage();
                using (var gfx = XGraphics.FromPdfPage(page))
                {
                    var title = new XFont("Arial", 16, XFontStyle.Bold);
                    var body = new XFont("Arial", 10, XFontStyle.Regular);
                    var bold = new XFont("Arial", 10, XFontStyle.Bold);
                    double y = 40;
                    gfx.DrawString("Form W-4 Employee's Withholding Certificate", title, XBrushes.Black, 40, y); y += 28;
                    gfx.DrawString("Employee: " + hire.FirstName + " " + hire.LastName, body, XBrushes.Black, 40, y); y += 16;
                    gfx.DrawString("Badge / File #: " + (hire.FileNumber ?? ""), body, XBrushes.Black, 40, y); y += 16;
                    gfx.DrawString("Email: " + (hire.HomeEmail ?? ""), body, XBrushes.Black, 40, y); y += 24;

                    gfx.DrawString("Federal Tax Election", bold, XBrushes.Black, 40, y); y += 18;
                    gfx.DrawString("Filing Status: " + (filingName ?? ""), body, XBrushes.Black, 40, y); y += 16;
                    gfx.DrawString("Other Income: " + FormatMoney(tax.OtherIncomeAmount), body, XBrushes.Black, 40, y); y += 16;
                    gfx.DrawString("Deductions: " + FormatMoney(tax.DeductionsAmount), body, XBrushes.Black, 40, y); y += 16;
                    gfx.DrawString("Extra Withholding: " + FormatMoney(tax.ExtraWithholdingAmount) + " / " + FormatPct(tax.ExtraWithholdingPercent), body, XBrushes.Black, 40, y); y += 16;
                    gfx.DrawString("Federal Exempt: " + (tax.FederalExempt ? "Yes" : "No"), body, XBrushes.Black, 40, y); y += 24;

                    gfx.DrawString("State Tax Election", bold, XBrushes.Black, 40, y); y += 18;
                    gfx.DrawString("Work Country / State: " + (countryName ?? "") + " / " + (stateName ?? ""), body, XBrushes.Black, 40, y); y += 16;
                    gfx.DrawString("State Withholding Status: " + (stateStatusName ?? ""), body, XBrushes.Black, 40, y); y += 16;
                    gfx.DrawString("State Exemptions: " + (tax.StateExemptions ?? ""), body, XBrushes.Black, 40, y); y += 16;
                    gfx.DrawString("State Additional: " + FormatMoney(tax.StateAdditionalWithholdingAmount) + " / " + FormatPct(tax.StateAdditionalWithholdingPercent), body, XBrushes.Black, 40, y); y += 16;
                    gfx.DrawString("State Exempt: " + (tax.StateExempt ? "Yes" : "No"), body, XBrushes.Black, 40, y); y += 24;
                    gfx.DrawString("Prepared for electronic signature.", body, XBrushes.DimGray, 40, y);
                }
                using (var ms = new MemoryStream())
                {
                    doc.Save(ms, false);
                    return ms.ToArray();
                }
            }
        }

        private static string FormatMoney(decimal? amount)
        {
            return amount.HasValue ? amount.Value.ToString("0.00") : "0.00";
        }

        private static string FormatPct(decimal? pct)
        {
            return (pct.HasValue ? pct.Value.ToString("0.##") : "0") + "%";
        }

        private static byte[] CreateI9PdfBytes(SelfOnboardingHire hire, SelfOnboardingI9 i9, string signedName, DateTime when, string ip, string txn)
        {
            using (var doc = new PdfDocument())
            {
                var page = doc.AddPage();
                using (var gfx = XGraphics.FromPdfPage(page))
                {
                    var title = new XFont("Arial", 16, XFontStyle.Bold);
                    var body = new XFont("Arial", 10, XFontStyle.Regular);
                    var bold = new XFont("Arial", 10, XFontStyle.Bold);
                    double y = 40;
                    gfx.DrawString("Form I-9 Employment Eligibility Verification", title, XBrushes.Black, 40, y); y += 28;
                    gfx.DrawString("Employee: " + hire.FirstName + " " + hire.LastName, body, XBrushes.Black, 40, y); y += 16;
                    gfx.DrawString("Badge / File #: " + (hire.FileNumber ?? ""), body, XBrushes.Black, 40, y); y += 16;
                    gfx.DrawString("Email: " + (hire.HomeEmail ?? ""), body, XBrushes.Black, 40, y); y += 24;

                    string statusText = i9.CitizenStatus == 1
                        ? "A lawful permanent resident"
                        : (i9.CitizenStatus == 2 ? "An alien authorized to work" : "A citizen or national of the United States");
                    gfx.DrawString("Attestation: " + statusText, bold, XBrushes.Black, 40, y); y += 18;

                    if (i9.CitizenStatus == 1)
                    {
                        gfx.DrawString("Alien Number: " + (i9.AlienNumber ?? ""), body, XBrushes.Black, 40, y); y += 16;
                        gfx.DrawString("Expiration: " + (i9.PermanentResidentExpire.HasValue ? i9.PermanentResidentExpire.Value.ToString("MM/dd/yyyy") : ""), body, XBrushes.Black, 40, y); y += 16;
                        gfx.DrawString("Citizen of: " + (i9.LawCitizenOfText ?? ""), body, XBrushes.Black, 40, y); y += 16;
                    }
                    else if (i9.CitizenStatus == 2)
                    {
                        gfx.DrawString("Authorized until: " + (i9.AlienAuthorizedUntil.HasValue ? i9.AlienAuthorizedUntil.Value.ToString("MM/dd/yyyy") : ""), body, XBrushes.Black, 40, y); y += 16;
                        gfx.DrawString("Citizen of: " + (i9.AlienCitizenOfText ?? ""), body, XBrushes.Black, 40, y); y += 16;
                    }

                    y += 8;
                    gfx.DrawString("Alien Registration / USCIS #: " + (i9.AlienRegistrationNumber ?? ""), body, XBrushes.Black, 40, y); y += 16;
                    gfx.DrawString("I-94 Admission #: " + (i9.AdmissionNumber ?? ""), body, XBrushes.Black, 40, y); y += 16;
                    gfx.DrawString("Foreign Passport #: " + (i9.PassportNumber ?? ""), body, XBrushes.Black, 40, y); y += 16;
                    gfx.DrawString("Country of Issuance: " + (i9.CountryOfIssuanceText ?? ""), body, XBrushes.Black, 40, y); y += 16;
                    gfx.DrawString("Translator: " + (i9.TranslatorNotUsed ? "Not used" : (i9.TranslatorUsed ? "Used" : "")), body, XBrushes.Black, 40, y); y += 16;
                    gfx.DrawString("Federal law acknowledgment: Yes", body, XBrushes.Black, 40, y); y += 16;
                    gfx.DrawString("Hide SSN on form: " + (i9.HideSsnOnForm ? "Yes" : "No"), body, XBrushes.Black, 40, y); y += 24;
                    gfx.DrawString("Prepared for electronic signature.", body, XBrushes.DimGray, 40, y);
                }
                using (var ms = new MemoryStream())
                {
                    doc.Save(ms, false);
                    return ms.ToArray();
                }
            }
        }

        private object GetWizardLookups(ClientDbContext db)
        {
            return new
            {
                prefixes = db.DdlPrefixes.OrderBy(x => x.Description).Select(x => new { id = x.PrefixId, text = x.Description }).ToList(),
                suffixes = db.DdlSuffixes.OrderBy(x => x.Description).Select(x => new { id = x.SuffixId, text = x.Description }).ToList(),
                marital = db.DdlMaritalStatuses.OrderBy(x => x.Description).Select(x => new { id = x.MaritalStatusId, text = x.Description }).ToList(),
                ethnicity = db.DdlEeoTypes.OrderBy(x => x.Description).Select(x => new { id = x.EeoTypeId, text = x.Description }).ToList(),
                genders = db.DdlGenders.OrderBy(x => x.Description).Select(x => new { id = x.GenderId, text = x.Description }).ToList(),
                countries = db.DdlCountries.OrderBy(x => x.Description).Select(x => new { id = x.CountryId, text = x.Description }).ToList(),
                states = db.DdlStates.OrderBy(x => x.Title).Select(x => new { id = x.StateId, text = x.Title }).ToList(),
                relationships = db.DdlRelationshipTypes.OrderBy(x => x.Description).Select(x => new { id = x.RelationshipTypeId, text = x.Description }).ToList(),
                filingStatus = db.OnboardingLookups.Where(l => l.LookupType == "FilingStatus" && l.IsActive).OrderBy(l => l.SortOrder).Select(l => new { id = l.LookupId, text = l.Description }).ToList(),
                stateTaxStatus = db.OnboardingLookups.Where(l => l.LookupType == "StateTaxStatus" && l.IsActive).OrderBy(l => l.SortOrder).Select(l => new { id = l.LookupId, text = l.Description }).ToList(),
                accountTypes = db.OnboardingLookups.Where(l => l.LookupType == "AccountType" && l.IsActive).OrderBy(l => l.SortOrder).Select(l => new { id = l.LookupId, text = l.Description }).ToList()
            };
        }

        private string SaveHireFile(int hireId, string fileName, byte[] bytes)
        {
            string root = Server.MapPath("~/App_Data/" + SelfOnboardingSchemaHelper.StorageFolderName + "/" + hireId);
            Directory.CreateDirectory(root);
            string physical = Path.Combine(root, fileName);
            System.IO.File.WriteAllBytes(physical, bytes);
            return Path.Combine(SelfOnboardingSchemaHelper.StorageFolderName, hireId.ToString(), fileName).Replace('\\', '/');
        }

        private string CreateBlankFormPdf(string key, SelfOnboardingHire hire)
        {
            byte[] bytes = CreateBlankFormPdfBytes(key, hire);
            string relative = SaveHireFile(hire.HireId, key + "_form.pdf", bytes);
            return Server.MapPath("~/App_Data/" + relative);
        }

        private static byte[] CreateBlankFormPdfBytes(string key, SelfOnboardingHire hire)
        {
            using (var doc = new PdfDocument())
            {
                var page = doc.AddPage();
                using (var gfx = XGraphics.FromPdfPage(page))
                {
                    var title = new XFont("Arial", 16, XFontStyle.Bold);
                    var body = new XFont("Arial", 11, XFontStyle.Regular);
                    gfx.DrawString(key + " Form", title, XBrushes.Black, 40, 50);
                    gfx.DrawString("Employee: " + hire.FirstName + " " + hire.LastName, body, XBrushes.Black, 40, 90);
                    gfx.DrawString("File #: " + (hire.FileNumber ?? ""), body, XBrushes.Black, 40, 110);
                    gfx.DrawString("Complete and sign this " + key + " form as part of self onboarding.", body, XBrushes.Black, 40, 140);
                }
                using (var ms = new MemoryStream())
                {
                    doc.Save(ms, false);
                    return ms.ToArray();
                }
            }
        }

        private static byte[] StampSignatureText(byte[] pdfBytes, string name, DateTime when, string ip, string txn)
        {
            using (var input = new MemoryStream(pdfBytes))
            using (var output = new MemoryStream())
            {
                PdfDocument document = PdfReader.Open(input, PdfDocumentOpenMode.Modify);
                PdfPage page = document.Pages[document.PageCount - 1];
                using (XGraphics gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Append))
                {
                    double y = page.Height.Point - 90;
                    var font = new XFont("Arial", 9, XFontStyle.Regular);
                    var bold = new XFont("Arial", 9, XFontStyle.Bold);
                    gfx.DrawRectangle(XBrushes.White, 30, y - 10, page.Width.Point - 60, 80);
                    gfx.DrawString("Electronic Signature", bold, XBrushes.DimGray, 40, y);
                    gfx.DrawString("Signed by: " + name, font, XBrushes.Black, 40, y + 16);
                    gfx.DrawString("Date/Time: " + when.ToString("MM/dd/yyyy HH:mm:ss"), font, XBrushes.Black, 40, y + 30);
                    gfx.DrawString("IP: " + ip, font, XBrushes.Black, 40, y + 44);
                    gfx.DrawString("Transaction ID: " + txn, font, XBrushes.Black, 40, y + 58);
                }
                document.Save(output, false);
                return output.ToArray();
            }
        }

        #endregion
    }
}
