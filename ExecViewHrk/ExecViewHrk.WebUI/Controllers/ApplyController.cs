using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Helpers;
using ExecViewHrk.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    /// <summary>
    /// External careers / apply wizard (AllowAnonymous). Resolves client DB via employerId.
    /// </summary>
    [AllowAnonymous]
    public class ApplyController : Controller
    {
        private const string DefaultAttestation =
            "I certify that the information provided in this application is true and complete to the best of my knowledge.";

        public ActionResult Index(int employerId, int? requisitionId)
        {
            string conn;
            string err;
            if (!JobRecruitingSchemaHelper.TryResolveClientConnection(employerId, out conn, out err))
            {
                ViewBag.Error = err;
                return View("ApplyError");
            }
            JobRecruitingSchemaHelper.StoreApplySession(employerId, conn);
            using (var db = new ClientDbContext(conn))
            {
                JobRecruitingSchemaHelper.EnsureSchema(db);
                var cfg = JobRecruitingSchemaHelper.GetOrCreateConfig(db);
                ViewBag.EmployerId = employerId;
                ViewBag.RequisitionId = requisitionId;
                ViewBag.HomeHtml = cfg.HomePageHtml;
                ViewBag.IntroHtml = cfg.IntroductionHtml;
                ViewBag.ApplicantName = Session[JobRecruitingSchemaHelper.SessionApplicantName] as string;
                var jobs = GetOpenJobs(db);
                if (requisitionId.HasValue)
                    jobs = jobs.Where(j => j.RequisitionId == requisitionId.Value).ToList();
                return View(jobs);
            }
        }

        [HttpGet]
        public ActionResult Register(int employerId)
        {
            EnsureSession(employerId);
            return View(new ApplyRegisterVm { EmployerId = employerId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(ApplyRegisterVm model)
        {
            if (!ModelState.IsValid)
                return View(model);
            if (!EnsureSession(model.EmployerId))
            {
                ModelState.AddModelError("", "Invalid career portal session.");
                return View(model);
            }
            using (var db = OpenDb())
            {
                JobRecruitingSchemaHelper.EnsureSchema(db);
                if (db.JobApplicants.Any(a => a.UserName == model.UserName))
                {
                    ModelState.AddModelError("UserName", "Username already taken.");
                    return View(model);
                }
                var salt = JobRecruitingSchemaHelper.CreateSalt();
                var applicant = new JobApplicant
                {
                    UserName = model.UserName.Trim(),
                    Email = model.Email.Trim(),
                    FirstName = model.FirstName.Trim(),
                    LastName = model.LastName.Trim(),
                    Phone = model.Phone,
                    PasswordSalt = salt,
                    PasswordHash = JobRecruitingSchemaHelper.HashPassword(model.Password, salt),
                    CreatedDate = DateTime.Now
                };
                db.JobApplicants.Add(applicant);
                db.SaveChanges();
                JobRecruitingSchemaHelper.SetApplyApplicant(applicant.ApplicantId, applicant.FirstName + " " + applicant.LastName);
                return RedirectToAction("Index", new { employerId = model.EmployerId });
            }
        }

        [HttpGet]
        public ActionResult Login(int employerId, int? requisitionId)
        {
            EnsureSession(employerId);
            return View(new ApplyLoginVm { EmployerId = employerId, RequisitionId = requisitionId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(ApplyLoginVm model)
        {
            if (!ModelState.IsValid)
                return View(model);
            if (!EnsureSession(model.EmployerId))
            {
                ModelState.AddModelError("", "Invalid career portal session.");
                return View(model);
            }
            using (var db = OpenDb())
            {
                JobRecruitingSchemaHelper.EnsureSchema(db);
                var applicant = db.JobApplicants.FirstOrDefault(a => a.UserName == model.UserName);
                if (applicant == null ||
                    applicant.PasswordHash != JobRecruitingSchemaHelper.HashPassword(model.Password, applicant.PasswordSalt))
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                    return View(model);
                }
                applicant.LastLoginDate = DateTime.Now;
                db.SaveChanges();
                JobRecruitingSchemaHelper.SetApplyApplicant(applicant.ApplicantId, applicant.FirstName + " " + applicant.LastName);
                if (model.RequisitionId.HasValue)
                    return RedirectToAction("Start", new { employerId = model.EmployerId, requisitionId = model.RequisitionId.Value });
                return RedirectToAction("Index", new { employerId = model.EmployerId });
            }
        }

        public ActionResult Logout(int employerId)
        {
            JobRecruitingSchemaHelper.ClearApplyApplicant();
            return RedirectToAction("Index", new { employerId });
        }

        [HttpPost]
        public ActionResult Start(int employerId, int requisitionId)
        {
            if (!EnsureSession(employerId))
                return RedirectToAction("Index", new { employerId });
            var applicantId = JobRecruitingSchemaHelper.GetApplyApplicantId();
            if (!applicantId.HasValue)
                return RedirectToAction("Login", new { employerId, requisitionId });

            using (var db = OpenDb())
            {
                JobRecruitingSchemaHelper.EnsureSchema(db);
                var req = db.JobRequisitions.FirstOrDefault(r => r.RequisitionId == requisitionId && r.IsPublished && r.Status == "Open");
                if (req == null)
                {
                    TempData["ApplyError"] = "This job is no longer open.";
                    return RedirectToAction("Index", new { employerId });
                }

                var existing = db.JobApplications.FirstOrDefault(a =>
                    a.RequisitionId == requisitionId && a.ApplicantId == applicantId.Value && a.Status != "Withdrawn");
                if (existing != null)
                    return RedirectToAction("Wizard", new { employerId, applicationId = existing.ApplicationId, step = Math.Max(1, existing.CurrentStep) });

                var app = new JobApplication
                {
                    RequisitionId = requisitionId,
                    ApplicantId = applicantId.Value,
                    Status = "Draft",
                    CurrentStep = 1,
                    CreatedDate = DateTime.Now
                };
                db.JobApplications.Add(app);
                db.SaveChanges();
                return RedirectToAction("Wizard", new { employerId, applicationId = app.ApplicationId, step = 1 });
            }
        }

        /// <summary>Internal employee apply (authenticated portal) — creates draft and redirects to wizard.</summary>
        [Authorize]
        [HttpPost]
        public JsonResult StartForEmployee(int requisitionId)
        {
            try
            {
                string conn = User.Identity.GetClientConnectionString();
                using (var db = new ClientDbContext(conn))
                {
                    JobRecruitingSchemaHelper.EnsureSchema(db);
                    var person = db.Persons.FirstOrDefault(p => p.eMail == User.Identity.Name);
                    if (person == null)
                        return Json(new { success = false, message = "No person linked to login." });
                    var emp = db.Employees.FirstOrDefault(e => e.PersonId == person.PersonId);
                    if (emp == null)
                        return Json(new { success = false, message = "No employee record for login." });

                    var req = db.JobRequisitions.FirstOrDefault(r => r.RequisitionId == requisitionId && r.IsPublished && r.Status == "Open");
                    if (req == null)
                        return Json(new { success = false, message = "Job is not open." });

                    var existing = db.JobApplications.FirstOrDefault(a =>
                        a.RequisitionId == requisitionId && a.EmployeeId == emp.EmployeeId && a.Status != "Withdrawn");
                    int applicationId;
                    int step;
                    if (existing != null)
                    {
                        applicationId = existing.ApplicationId;
                        step = Math.Max(1, existing.CurrentStep);
                    }
                    else
                    {
                        var app = new JobApplication
                        {
                            RequisitionId = requisitionId,
                            EmployeeId = emp.EmployeeId,
                            Status = "Draft",
                            CurrentStep = 1,
                            CreatedDate = DateTime.Now
                        };
                        db.JobApplications.Add(app);
                        db.SaveChanges();
                        applicationId = app.ApplicationId;
                        step = 1;
                    }

                    int employerId = 0;
                    int.TryParse(User.Identity.GetClientAdminEmployerID(), out employerId);
                    if (employerId <= 0) int.TryParse(User.Identity.GetSelectedClientID(), out employerId);
                    JobRecruitingSchemaHelper.StoreApplySession(employerId, conn);

                    string url = Url.Action("Wizard", "Apply", new { employerId = employerId, applicationId = applicationId, step = step });
                    return Json(new { success = true, url = url });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult Wizard(int employerId, int applicationId, int step = 1)
        {
            if (!EnsureSession(employerId) && !User.Identity.IsAuthenticated)
            {
                ViewBag.Error = "Session expired. Open the career link again.";
                return View("ApplyError");
            }
            // If authenticated employee, use their claim connection
            if (User.Identity.IsAuthenticated && string.IsNullOrEmpty(JobRecruitingSchemaHelper.GetApplyConnectionString()))
            {
                var c = User.Identity.GetClientConnectionString();
                if (!string.IsNullOrEmpty(c))
                    JobRecruitingSchemaHelper.StoreApplySession(employerId, c);
            }

            using (var db = OpenDb())
            {
                JobRecruitingSchemaHelper.EnsureSchema(db);
                var app = db.JobApplications.Include("JobRequisition").FirstOrDefault(a => a.ApplicationId == applicationId);
                if (app == null)
                {
                    ViewBag.Error = "Application not found.";
                    return View("ApplyError");
                }
                if (!CanAccessApplication(db, app))
                {
                    ViewBag.Error = "You do not have access to this application.";
                    return View("ApplyError");
                }
                if (app.Status == "Submitted")
                    return RedirectToAction("Complete", new { employerId, applicationId });

                step = Math.Min(10, Math.Max(1, step));
                var cfg = JobRecruitingSchemaHelper.GetOrCreateConfig(db);
                ViewBag.EmployerId = employerId;
                ViewBag.ApplicationId = applicationId;
                ViewBag.Step = step;
                ViewBag.App = app;
                ViewBag.Config = cfg;
                ViewBag.DefaultAttestation = DefaultAttestation;

                if (step >= 2 && step <= 5)
                {
                    ViewBag.Questions = db.RecruitingQuestions
                        .Where(q => q.IsActive && q.WizardPage == step)
                        .OrderBy(q => q.SortOrder).ToList();
                    ViewBag.Answers = db.JobApplicationAnswers.Where(a => a.ApplicationId == applicationId).ToList();
                }
                if (step == 6)
                {
                    ViewBag.DocSetups = db.RecruitingDocuments.Where(d => d.IsActive).OrderBy(d => d.SortOrder).ToList();
                    ViewBag.Files = db.JobApplicationFiles.Where(f => f.ApplicationId == applicationId && f.FileCategory == "Additional").ToList();
                    ViewBag.Signatures = db.JobApplicationSignatures.Where(s => s.ApplicationId == applicationId).ToList();
                }
                if (step == 7)
                {
                    ViewBag.ResumeFiles = db.JobApplicationFiles.Where(f => f.ApplicationId == applicationId && (f.FileCategory == "Resume" || f.FileCategory == "Other")).ToList();
                }
                if (step == 8)
                    ViewBag.References = db.JobApplicationReferences.Where(r => r.ApplicationId == applicationId).ToList();
                if (step == 9)
                {
                    ViewBag.Employments = db.JobApplicationEmployments.Where(e => e.ApplicationId == applicationId).ToList();
                    ViewBag.Educations = db.JobApplicationEducations.Where(e => e.ApplicationId == applicationId).ToList();
                }
                if (step == 10)
                {
                    ViewBag.Answers = db.JobApplicationAnswers.Where(a => a.ApplicationId == applicationId).ToList();
                    ViewBag.AllQuestions = db.RecruitingQuestions.Where(q => q.IsActive).OrderBy(q => q.WizardPage).ThenBy(q => q.SortOrder).ToList();
                    ViewBag.References = db.JobApplicationReferences.Where(r => r.ApplicationId == applicationId).ToList();
                    ViewBag.Employments = db.JobApplicationEmployments.Where(e => e.ApplicationId == applicationId).ToList();
                    ViewBag.Educations = db.JobApplicationEducations.Where(e => e.ApplicationId == applicationId).ToList();
                    ViewBag.Files = db.JobApplicationFiles.Where(f => f.ApplicationId == applicationId).ToList();
                    ViewBag.Signatures = db.JobApplicationSignatures.Where(s => s.ApplicationId == applicationId).ToList();
                }

                return View();
            }
        }

        [HttpPost]
        public JsonResult SaveStep(int employerId, int applicationId, int step, string attestationName = null)
        {
            try
            {
                EnsureSession(employerId);
                using (var db = OpenDb())
                {
                    JobRecruitingSchemaHelper.EnsureSchema(db);
                    var app = db.JobApplications.FirstOrDefault(a => a.ApplicationId == applicationId);
                    if (app == null || !CanAccessApplication(db, app))
                        return Json(new { success = false, message = "Access denied." });
                    if (app.Status == "Submitted")
                        return Json(new { success = false, message = "Already submitted." });

                    if (step >= 2 && step <= 5)
                    {
                        var answers = ParseAnswers();
                        foreach (var item in answers)
                        {
                            var existing = db.JobApplicationAnswers.FirstOrDefault(a => a.ApplicationId == applicationId && a.QuestionId == item.QuestionId);
                            if (existing == null)
                            {
                                db.JobApplicationAnswers.Add(new JobApplicationAnswer
                                {
                                    ApplicationId = applicationId,
                                    QuestionId = item.QuestionId,
                                    AnswerText = item.AnswerText
                                });
                            }
                            else existing.AnswerText = item.AnswerText;
                        }
                    }
                    else if (step == 8)
                    {
                        var refs = ParseReferences();
                        var old = db.JobApplicationReferences.Where(r => r.ApplicationId == applicationId).ToList();
                        db.JobApplicationReferences.RemoveRange(old);
                        foreach (var r in refs.Where(x => !string.IsNullOrWhiteSpace(x.FullName)))
                        {
                            db.JobApplicationReferences.Add(new JobApplicationReference
                            {
                                ApplicationId = applicationId,
                                FullName = r.FullName,
                                Relationship = r.Relationship,
                                Company = r.Company,
                                Phone = r.Phone,
                                Email = r.Email,
                                YearsKnown = r.YearsKnown
                            });
                        }
                    }
                    else if (step == 9)
                    {
                        var emps = ParseEmployments();
                        var edus = ParseEducations();
                        db.JobApplicationEmployments.RemoveRange(db.JobApplicationEmployments.Where(e => e.ApplicationId == applicationId));
                        db.JobApplicationEducations.RemoveRange(db.JobApplicationEducations.Where(e => e.ApplicationId == applicationId));
                        foreach (var e in emps.Where(x => !string.IsNullOrWhiteSpace(x.EmployerName)))
                        {
                            db.JobApplicationEmployments.Add(new JobApplicationEmployment
                            {
                                ApplicationId = applicationId,
                                EmployerName = e.EmployerName,
                                JobTitle = e.JobTitle,
                                StartDate = e.StartDate,
                                EndDate = e.EndDate,
                                Duties = e.Duties,
                                ReasonLeft = e.ReasonLeft
                            });
                        }
                        foreach (var e in edus.Where(x => !string.IsNullOrWhiteSpace(x.SchoolName)))
                        {
                            db.JobApplicationEducations.Add(new JobApplicationEducation
                            {
                                ApplicationId = applicationId,
                                SchoolName = e.SchoolName,
                                Degree = e.Degree,
                                FieldOfStudy = e.FieldOfStudy,
                                GraduationYear = e.GraduationYear
                            });
                        }
                    }
                    else if (step == 10 && !string.IsNullOrWhiteSpace(attestationName))
                    {
                        var sig = db.JobApplicationSignatures.FirstOrDefault(s => s.ApplicationId == applicationId && s.SignatureType == "Attestation");
                        if (sig == null)
                        {
                            db.JobApplicationSignatures.Add(new JobApplicationSignature
                            {
                                ApplicationId = applicationId,
                                SignatureType = "Attestation",
                                SignerName = attestationName.Trim(),
                                SignedDate = DateTime.Now
                            });
                        }
                        else
                        {
                            sig.SignerName = attestationName.Trim();
                            sig.SignedDate = DateTime.Now;
                        }
                    }

                    app.CurrentStep = Math.Max(app.CurrentStep, step);
                    app.ModifiedDate = DateTime.Now;
                    db.SaveChanges();
                    return Json(new { success = true, nextStep = Math.Min(10, step + 1) });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UploadFile(int employerId, int applicationId, string fileCategory, int? documentSetupId, string signerName = null)
        {
            try
            {
                EnsureSession(employerId);
                if (Request.Files.Count == 0 || Request.Files[0] == null || Request.Files[0].ContentLength == 0)
                    return Json(new { success = false, message = "No file uploaded." });

                using (var db = OpenDb())
                {
                    JobRecruitingSchemaHelper.EnsureSchema(db);
                    var app = db.JobApplications.FirstOrDefault(a => a.ApplicationId == applicationId);
                    if (app == null || !CanAccessApplication(db, app))
                        return Json(new { success = false, message = "Access denied." });

                    var file = Request.Files[0];
                    string rel = SaveUploadedFile(applicationId, file);
                    var entity = new JobApplicationFile
                    {
                        ApplicationId = applicationId,
                        DocumentSetupId = documentSetupId,
                        FileCategory = string.IsNullOrWhiteSpace(fileCategory) ? "Other" : fileCategory,
                        FileName = Path.GetFileName(file.FileName),
                        FilePath = rel,
                        UploadedDate = DateTime.Now
                    };
                    db.JobApplicationFiles.Add(entity);

                    if (!string.IsNullOrWhiteSpace(signerName) && documentSetupId.HasValue)
                    {
                        db.JobApplicationSignatures.Add(new JobApplicationSignature
                        {
                            ApplicationId = applicationId,
                            SignatureType = "Document",
                            DocumentSetupId = documentSetupId,
                            SignerName = signerName.Trim(),
                            SignedDate = DateTime.Now
                        });
                    }

                    app.ModifiedDate = DateTime.Now;
                    db.SaveChanges();
                    return Json(new { success = true, fileId = entity.FileId, fileName = entity.FileName });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Submit(int employerId, int applicationId, string attestationName)
        {
            try
            {
                EnsureSession(employerId);
                using (var db = OpenDb())
                {
                    JobRecruitingSchemaHelper.EnsureSchema(db);
                    var app = db.JobApplications.FirstOrDefault(a => a.ApplicationId == applicationId);
                    if (app == null || !CanAccessApplication(db, app))
                        return Json(new { success = false, message = "Access denied." });
                    if (app.Status == "Submitted")
                        return Json(new { success = true, already = true });

                    if (string.IsNullOrWhiteSpace(attestationName))
                        return Json(new { success = false, message = "Signature name is required." });

                    var sig = db.JobApplicationSignatures.FirstOrDefault(s => s.ApplicationId == applicationId && s.SignatureType == "Attestation");
                    if (sig == null)
                    {
                        db.JobApplicationSignatures.Add(new JobApplicationSignature
                        {
                            ApplicationId = applicationId,
                            SignatureType = "Attestation",
                            SignerName = attestationName.Trim(),
                            SignedDate = DateTime.Now
                        });
                    }
                    else
                    {
                        sig.SignerName = attestationName.Trim();
                        sig.SignedDate = DateTime.Now;
                    }

                    bool wasSubmitted = app.Status == "Submitted";
                    app.Status = "Submitted";
                    app.SubmittedDate = DateTime.Now;
                    app.CurrentStep = 10;
                    app.ModifiedDate = DateTime.Now;

                    var req = db.JobRequisitions.FirstOrDefault(r => r.RequisitionId == app.RequisitionId);
                    if (req != null && !wasSubmitted)
                        req.ApplicantCount = db.JobApplications.Count(a => a.RequisitionId == req.RequisitionId && a.Status == "Submitted") + 1;

                    db.SaveChanges();

                    // Recalc accurate count
                    if (req != null)
                    {
                        req.ApplicantCount = db.JobApplications.Count(a => a.RequisitionId == req.RequisitionId && a.Status == "Submitted");
                        db.SaveChanges();
                    }

                    return Json(new { success = true });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult Complete(int employerId, int applicationId)
        {
            EnsureSession(employerId);
            ViewBag.EmployerId = employerId;
            ViewBag.ApplicationId = applicationId;
            return View();
        }

        private bool EnsureSession(int employerId)
        {
            var existing = JobRecruitingSchemaHelper.GetApplyConnectionString();
            var eid = JobRecruitingSchemaHelper.GetApplyEmployerId();
            if (!string.IsNullOrEmpty(existing) && eid == employerId)
                return true;
            string conn, err;
            if (!JobRecruitingSchemaHelper.TryResolveClientConnection(employerId, out conn, out err))
                return false;
            JobRecruitingSchemaHelper.StoreApplySession(employerId, conn);
            return true;
        }

        private ClientDbContext OpenDb()
        {
            var conn = JobRecruitingSchemaHelper.GetApplyConnectionString();
            if (string.IsNullOrEmpty(conn) && User.Identity.IsAuthenticated)
                conn = User.Identity.GetClientConnectionString();
            if (string.IsNullOrEmpty(conn))
                throw new InvalidOperationException("No client connection for apply session.");
            return new ClientDbContext(conn);
        }

        private bool CanAccessApplication(ClientDbContext db, JobApplication app)
        {
            var applicantId = JobRecruitingSchemaHelper.GetApplyApplicantId();
            if (applicantId.HasValue && app.ApplicantId == applicantId.Value)
                return true;
            if (User.Identity.IsAuthenticated)
            {
                var person = db.Persons.FirstOrDefault(p => p.eMail == User.Identity.Name);
                if (person != null)
                {
                    var emp = db.Employees.FirstOrDefault(e => e.PersonId == person.PersonId);
                    if (emp != null && app.EmployeeId == emp.EmployeeId)
                        return true;
                }
            }
            return false;
        }

        private static List<JobPortalOpenJobVm> GetOpenJobs(ClientDbContext db)
        {
            return db.JobRequisitions
                .Where(r => r.IsPublished && r.Status == "Open")
                .OrderByDescending(r => r.OpenDate ?? r.RequisitionDate)
                .Select(r => new JobPortalOpenJobVm
                {
                    RequisitionId = r.RequisitionId,
                    RequisitionNumber = r.RequisitionNumber,
                    PositionTitle = r.PositionTitle,
                    Division = r.Division,
                    Department = r.Department,
                    Description = r.Description,
                    OpenDate = r.OpenDate
                })
                .ToList();
        }

        private string SaveUploadedFile(int applicationId, HttpPostedFileBase file)
        {
            string root = Server.MapPath("~/App_Data/" + JobRecruitingSchemaHelper.StorageFolderName);
            string folder = Path.Combine(root, applicationId.ToString());
            Directory.CreateDirectory(folder);
            string unique = Path.GetFileNameWithoutExtension(file.FileName)
                + "_" + DateTime.Now.ToString("yyyyMMddHHmmss")
                + Path.GetExtension(file.FileName);
            string physical = Path.Combine(folder, unique);
            file.SaveAs(physical);
            return Path.Combine(JobRecruitingSchemaHelper.StorageFolderName, applicationId.ToString(), unique).Replace('\\', '/');
        }

        private List<ApplyAnswerItem> ParseAnswers()
        {
            var list = new List<ApplyAnswerItem>();
            foreach (var key in Request.Form.AllKeys.Where(k => k != null && k.StartsWith("ans_")))
            {
                int qid;
                if (int.TryParse(key.Substring(4), out qid))
                    list.Add(new ApplyAnswerItem { QuestionId = qid, AnswerText = Request.Form[key] });
            }
            return list;
        }

        private List<ApplyReferenceItem> ParseReferences()
        {
            var list = new List<ApplyReferenceItem>();
            int i = 0;
            while (Request.Form["ref_name_" + i] != null)
            {
                list.Add(new ApplyReferenceItem
                {
                    FullName = Request.Form["ref_name_" + i],
                    Relationship = Request.Form["ref_rel_" + i],
                    Company = Request.Form["ref_co_" + i],
                    Phone = Request.Form["ref_phone_" + i],
                    Email = Request.Form["ref_email_" + i],
                    YearsKnown = Request.Form["ref_years_" + i]
                });
                i++;
            }
            return list;
        }

        private List<ApplyEmploymentItem> ParseEmployments()
        {
            var list = new List<ApplyEmploymentItem>();
            int i = 0;
            while (Request.Form["emp_name_" + i] != null)
            {
                DateTime? start = null, end = null;
                DateTime tmp;
                if (DateTime.TryParse(Request.Form["emp_start_" + i], out tmp)) start = tmp;
                if (DateTime.TryParse(Request.Form["emp_end_" + i], out tmp)) end = tmp;
                list.Add(new ApplyEmploymentItem
                {
                    EmployerName = Request.Form["emp_name_" + i],
                    JobTitle = Request.Form["emp_title_" + i],
                    StartDate = start,
                    EndDate = end,
                    Duties = Request.Form["emp_duties_" + i],
                    ReasonLeft = Request.Form["emp_reason_" + i]
                });
                i++;
            }
            return list;
        }

        private List<ApplyEducationItem> ParseEducations()
        {
            var list = new List<ApplyEducationItem>();
            int i = 0;
            while (Request.Form["edu_school_" + i] != null)
            {
                list.Add(new ApplyEducationItem
                {
                    SchoolName = Request.Form["edu_school_" + i],
                    Degree = Request.Form["edu_degree_" + i],
                    FieldOfStudy = Request.Form["edu_field_" + i],
                    GraduationYear = Request.Form["edu_year_" + i]
                });
                i++;
            }
            return list;
        }
    }
}
