using ExecViewHrk.EfAdmin;
using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Helpers;
using ExecViewHrk.WebUI.Models;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    [Authorize]
    public class RequisitionController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("RequisitionDashboardPartial");
        }

        public PartialViewResult RequisitionDashboardPartial()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                JobRecruitingSchemaHelper.EnsureSchema(db);
                var list = db.JobRequisitions
                    .OrderByDescending(r => r.RequisitionDate)
                    .Select(r => new JobRequisitionVm
                    {
                        RequisitionId = r.RequisitionId,
                        RequisitionNumber = r.RequisitionNumber,
                        PositionTitle = r.PositionTitle,
                        Division = r.Division,
                        Department = r.Department,
                        PositionId = r.PositionId,
                        ReportToPositionId = r.ReportToPositionId,
                        Description = r.Description,
                        RequisitionDate = r.RequisitionDate,
                        OpenDate = r.OpenDate,
                        ClosedDate = r.ClosedDate,
                        Status = r.Status,
                        // Live counts: Applicants = Submitted; Candidates = Candidate or Hire
                        ApplicantCount = db.JobApplications.Count(a =>
                            a.RequisitionId == r.RequisitionId && a.Status == "Submitted"),
                        CandidateCount = db.JobApplications.Count(a =>
                            a.RequisitionId == r.RequisitionId
                            && (a.Status == "Candidate" || a.Status == "Hire")),
                        IsPublished = r.IsPublished
                    })
                    .ToList();
                return PartialView(list);
            }
        }

        [HttpGet]
        public JsonResult GetRequisitions()
        {
            try
            {
                string connString = User.Identity.GetClientConnectionString();
                using (var db = new ClientDbContext(connString))
                {
                    JobRecruitingSchemaHelper.EnsureSchema(db);
                    var list = db.JobRequisitions
                        .OrderByDescending(r => r.RequisitionDate)
                        .Select(r => new
                        {
                            r.RequisitionId,
                            r.RequisitionNumber,
                            r.PositionTitle,
                            r.Division,
                            r.Department,
                            r.RequisitionDate,
                            r.OpenDate,
                            r.ClosedDate,
                            r.Status,
                            ApplicantCount = db.JobApplications.Count(a =>
                                a.RequisitionId == r.RequisitionId && a.Status == "Submitted"),
                            CandidateCount = db.JobApplications.Count(a =>
                                a.RequisitionId == r.RequisitionId
                                && (a.Status == "Candidate" || a.Status == "Hire")),
                            r.IsPublished
                        })
                        .ToList();
                    return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetFormLookups()
        {
            try
            {
                string connString = User.Identity.GetClientConnectionString();
                using (var db = new ClientDbContext(connString))
                {
                    JobRecruitingSchemaHelper.EnsureSchema(db);

                    var positions = db.Positions
                        .Where(p => p.IsPositionActive)
                        .OrderBy(p => p.PositionCode)
                        .ToList()
                        .Select(p => new
                        {
                            id = p.PositionId,
                            code = p.PositionCode,
                            title = string.IsNullOrWhiteSpace(p.Title) ? p.PositionDescription : p.Title,
                            text = (p.PositionCode ?? "") + " - " + (string.IsNullOrWhiteSpace(p.Title) ? p.PositionDescription : p.Title),
                            departmentId = p.DepartmentId,
                            businessUnitId = p.BusinessUnitId,
                            reportToPositionId = p.ReportsToPositionId
                        })
                        .ToList();

                    var departments = db.Departments
                        .Where(d => d.IsDepartmentActive && (d.IsDeleted == null || d.IsDeleted == false))
                        .OrderBy(d => d.DepartmentCode)
                        .ToList()
                        .Select(d => new
                        {
                            id = d.DepartmentId,
                            text = (d.DepartmentCode ?? "") + " - " + (d.DepartmentDescription ?? ""),
                            name = d.DepartmentDescription
                        })
                        .ToList();

                    var divisions = db.BusinessUnits
                        .OrderBy(b => b.BusinessUnitCode)
                        .ToList()
                        .Select(b => new
                        {
                            id = b.BusinessUnitId,
                            text = (b.BusinessUnitCode ?? "") + " - " + (b.BusinessUnitDescription ?? ""),
                            name = b.BusinessUnitDescription
                        })
                        .ToList();

                    return Json(new
                    {
                        success = true,
                        positions = positions,
                        departments = departments,
                        divisions = divisions,
                        reportToPositions = positions
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetRequisition(int id)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                JobRecruitingSchemaHelper.EnsureSchema(db);
                var r = db.JobRequisitions.FirstOrDefault(x => x.RequisitionId == id);
                if (r == null) return Json(new { success = false, message = "Not found" }, JsonRequestBehavior.AllowGet);

                int? departmentId = null;
                int? businessUnitId = null;
                if (r.PositionId.HasValue)
                {
                    var pos = db.Positions.FirstOrDefault(p => p.PositionId == r.PositionId.Value);
                    if (pos != null)
                    {
                        departmentId = pos.DepartmentId;
                        businessUnitId = pos.BusinessUnitId;
                    }
                }
                if (!departmentId.HasValue && !string.IsNullOrWhiteSpace(r.Department))
                {
                    var dept = db.Departments.FirstOrDefault(d => d.DepartmentDescription == r.Department);
                    if (dept != null) departmentId = dept.DepartmentId;
                }
                if (!businessUnitId.HasValue && !string.IsNullOrWhiteSpace(r.Division))
                {
                    var bu = db.BusinessUnits.FirstOrDefault(b => b.BusinessUnitDescription == r.Division);
                    if (bu != null) businessUnitId = bu.BusinessUnitId;
                }

                return Json(new
                {
                    success = true,
                    data = new JobRequisitionVm
                    {
                        RequisitionId = r.RequisitionId,
                        RequisitionNumber = r.RequisitionNumber,
                        PositionTitle = r.PositionTitle,
                        Division = r.Division,
                        Department = r.Department,
                        PositionId = r.PositionId,
                        ReportToPositionId = r.ReportToPositionId,
                        DepartmentId = departmentId,
                        BusinessUnitId = businessUnitId,
                        Description = r.Description,
                        RequisitionDate = r.RequisitionDate,
                        OpenDate = r.OpenDate,
                        ClosedDate = r.ClosedDate,
                        Status = r.Status,
                        ApplicantCount = r.ApplicantCount,
                        IsPublished = r.IsPublished
                    }
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveRequisition(JobRequisitionVm model)
        {
            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.RequisitionNumber))
                    return Json(new { success = false, message = "Requisition number is required." });
                if (!model.PositionId.HasValue || model.PositionId.Value <= 0)
                    return Json(new { success = false, message = "Position is required." });

                string connString = User.Identity.GetClientConnectionString();
                using (var db = new ClientDbContext(connString))
                {
                    JobRecruitingSchemaHelper.EnsureSchema(db);

                    var position = db.Positions.FirstOrDefault(p => p.PositionId == model.PositionId.Value);
                    if (position == null)
                        return Json(new { success = false, message = "Selected position was not found." });

                    string positionTitle = !string.IsNullOrWhiteSpace(position.Title)
                        ? position.Title.Trim()
                        : (position.PositionDescription ?? "").Trim();
                    if (string.IsNullOrWhiteSpace(positionTitle))
                        positionTitle = position.PositionCode ?? "Position";

                    string divisionName = model.Division;
                    if (model.BusinessUnitId.HasValue && model.BusinessUnitId.Value > 0)
                    {
                        var bu = db.BusinessUnits.FirstOrDefault(b => b.BusinessUnitId == model.BusinessUnitId.Value);
                        if (bu != null) divisionName = bu.BusinessUnitDescription;
                    }

                    string departmentName = model.Department;
                    if (model.DepartmentId.HasValue && model.DepartmentId.Value > 0)
                    {
                        var dept = db.Departments.FirstOrDefault(d => d.DepartmentId == model.DepartmentId.Value);
                        if (dept != null) departmentName = dept.DepartmentDescription;
                    }

                    JobRequisition entity;
                    if (model.RequisitionId > 0)
                    {
                        entity = db.JobRequisitions.FirstOrDefault(x => x.RequisitionId == model.RequisitionId);
                        if (entity == null) return Json(new { success = false, message = "Not found" });
                        entity.ModifiedBy = User.Identity.Name;
                        entity.ModifiedDate = DateTime.Now;
                    }
                    else
                    {
                        entity = new JobRequisition
                        {
                            CreatedBy = User.Identity.Name,
                            CreatedDate = DateTime.Now,
                            ApplicantCount = 0
                        };
                        db.JobRequisitions.Add(entity);
                    }

                    entity.RequisitionNumber = model.RequisitionNumber.Trim();
                    entity.PositionTitle = positionTitle;
                    entity.Division = divisionName;
                    entity.Department = departmentName;
                    entity.PositionId = model.PositionId;
                    entity.ReportToPositionId = model.ReportToPositionId.HasValue && model.ReportToPositionId.Value > 0
                        ? model.ReportToPositionId
                        : null;
                    entity.Description = model.Description;
                    entity.RequisitionDate = model.RequisitionDate == default(DateTime) ? DateTime.Today : model.RequisitionDate;
                    entity.OpenDate = model.OpenDate;
                    entity.ClosedDate = model.ClosedDate;
                    entity.Status = string.IsNullOrWhiteSpace(model.Status) ? "Open" : model.Status;
                    entity.IsPublished = model.IsPublished;

                    db.SaveChanges();
                    return Json(new { success = true, id = entity.RequisitionId });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteRequisition(int id)
        {
            try
            {
                string connString = User.Identity.GetClientConnectionString();
                using (var db = new ClientDbContext(connString))
                {
                    JobRecruitingSchemaHelper.EnsureSchema(db);
                    var entity = db.JobRequisitions.FirstOrDefault(x => x.RequisitionId == id);
                    if (entity == null) return Json(new { success = false, message = "Not found" });
                    if (db.JobApplications.Any(a => a.RequisitionId == id))
                        return Json(new { success = false, message = "Cannot delete a requisition that has applications. Close it instead." });
                    db.JobRequisitions.Remove(entity);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetApplicants(int requisitionId)
        {
            try
            {
                string connString = User.Identity.GetClientConnectionString();
                using (var db = new ClientDbContext(connString))
                {
                    JobRecruitingSchemaHelper.EnsureSchema(db);
                    var apps = db.JobApplications
                        .Where(a => a.RequisitionId == requisitionId)
                        .OrderByDescending(a => a.CreatedDate)
                        .ToList()
                        .Select(a =>
                        {
                            string name = "";
                            if (a.ApplicantId.HasValue)
                            {
                                var ap = db.JobApplicants.FirstOrDefault(x => x.ApplicantId == a.ApplicantId.Value);
                                if (ap != null) name = ap.FirstName + " " + ap.LastName;
                            }
                            else if (a.EmployeeId.HasValue)
                            {
                                var emp = db.Employees.Include("Person").FirstOrDefault(e => e.EmployeeId == a.EmployeeId.Value);
                                if (emp != null && emp.Person != null)
                                    name = ((emp.Person.Firstname ?? "") + " " + (emp.Person.Lastname ?? "")).Trim();
                            }
                            return new
                            {
                                ApplicationId = a.ApplicationId,
                                RequisitionId = a.RequisitionId,
                                ApplicantName = string.IsNullOrWhiteSpace(name) ? "(unknown)" : name,
                                Status = a.Status,
                                ApplicantType = a.Status == "Candidate" || a.Status == "Hire" ? "Candidate" : "Applicant",
                                CreatedDate = a.CreatedDate,
                                SubmittedDate = a.SubmittedDate,
                                CurrentStep = a.CurrentStep,
                                AdminComment = a.AdminComment,
                                CanHire = a.Status == "Candidate"
                            };
                        })
                        .ToList();
                    return Json(new { success = true, data = apps }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetApplicantDetails(int applicationId)
        {
            try
            {
                string connString = User.Identity.GetClientConnectionString();
                using (var db = new ClientDbContext(connString))
                {
                    JobRecruitingSchemaHelper.EnsureSchema(db);
                    var app = db.JobApplications
                        .Include("JobRequisition")
                        .FirstOrDefault(a => a.ApplicationId == applicationId);
                    if (app == null)
                        return Json(new { success = false, message = "Application not found." }, JsonRequestBehavior.AllowGet);

                    string accountName = "";
                    string accountEmail = "";
                    if (app.ApplicantId.HasValue)
                    {
                        var applicant = db.JobApplicants.FirstOrDefault(a => a.ApplicantId == app.ApplicantId.Value);
                        if (applicant != null)
                        {
                            accountName = (applicant.FirstName + " " + applicant.LastName).Trim();
                            accountEmail = applicant.Email;
                        }
                    }
                    else if (app.EmployeeId.HasValue)
                    {
                        var employee = db.Employees.Include("Person").FirstOrDefault(e => e.EmployeeId == app.EmployeeId.Value);
                        if (employee != null && employee.Person != null)
                        {
                            accountName = ((employee.Person.Firstname ?? "") + " " + (employee.Person.Lastname ?? "")).Trim();
                            accountEmail = employee.Person.eMail;
                        }
                    }

                    var profile = db.JobApplicationProfiles.FirstOrDefault(p => p.ApplicationId == applicationId);
                    var questions = db.RecruitingQuestions.ToList();
                    var answers = db.JobApplicationAnswers
                        .Where(a => a.ApplicationId == applicationId)
                        .ToList()
                        .Select(a => new
                        {
                            Question = questions.Where(q => q.QuestionId == a.QuestionId)
                                .Select(q => q.QuestionText).FirstOrDefault() ?? ("Question #" + a.QuestionId),
                            Answer = a.AnswerText
                        })
                        .ToList();

                    var files = db.JobApplicationFiles
                        .Where(f => f.ApplicationId == applicationId)
                        .OrderBy(f => f.FileCategory)
                        .Select(f => new { f.FileId, f.FileCategory, f.FileName, f.UploadedDate })
                        .ToList();
                    var references = db.JobApplicationReferences
                        .Where(r => r.ApplicationId == applicationId)
                        .Select(r => new { r.FullName, r.Relationship, r.Company, r.Phone, r.Email, r.YearsKnown })
                        .ToList();
                    var employment = db.JobApplicationEmployments
                        .Where(e => e.ApplicationId == applicationId)
                        .Select(e => new { e.EmployerName, e.JobTitle, e.StartDate, e.EndDate, e.Duties, e.ReasonLeft })
                        .ToList();
                    var education = db.JobApplicationEducations
                        .Where(e => e.ApplicationId == applicationId)
                        .Select(e => new { e.SchoolName, e.Degree, e.FieldOfStudy, e.GraduationYear })
                        .ToList();
                    var signatures = db.JobApplicationSignatures
                        .Where(s => s.ApplicationId == applicationId)
                        .Select(s => new { s.SignatureType, s.SignerName, s.SignedDate })
                        .ToList();

                    return Json(new
                    {
                        success = true,
                        data = new
                        {
                            app.ApplicationId,
                            PositionTitle = app.JobRequisition != null ? app.JobRequisition.PositionTitle : "",
                            RequisitionNumber = app.JobRequisition != null ? app.JobRequisition.RequisitionNumber : "",
                            ApplicantName = !string.IsNullOrWhiteSpace(accountName) ? accountName :
                                (profile != null ? (profile.FirstName + " " + profile.LastName).Trim() : "(unknown)"),
                            Email = profile != null ? profile.Email : accountEmail,
                            Phone = profile != null ? profile.Phone : "",
                            Address = profile != null
                                ? string.Join(", ", new[] { profile.StreetAddress, profile.City, profile.ZipCode }.Where(x => !string.IsNullOrWhiteSpace(x)))
                                : "",
                            app.Status,
                            app.CreatedDate,
                            app.SubmittedDate,
                            app.AdminComment,
                            app.ReviewedBy,
                            app.ReviewedDate,
                            Answers = answers,
                            Files = files,
                            References = references,
                            Employment = employment,
                            Education = education,
                            Signatures = signatures
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateApplicant(int applicationId, string actionType, string comment)
        {
            try
            {
                string connString = User.Identity.GetClientConnectionString();
                using (var db = new ClientDbContext(connString))
                {
                    JobRecruitingSchemaHelper.EnsureSchema(db);
                    var app = db.JobApplications.FirstOrDefault(a => a.ApplicationId == applicationId);
                    if (app == null)
                        return Json(new { success = false, message = "Application not found." });

                    actionType = (actionType ?? "").Trim();
                    if (actionType == "Candidate")
                    {
                        if (app.Status != "Submitted")
                            return Json(new { success = false, message = "Only a submitted applicant can be converted to Candidate." });
                        app.Status = "Candidate";
                    }
                    else if (actionType == "Hire")
                    {
                        if (app.Status != "Candidate")
                            return Json(new { success = false, message = "Convert this applicant to Candidate before hiring." });
                        app.Status = "Hire";
                    }
                    else if (actionType == "Reject")
                    {
                        if (app.Status == "Hire")
                            return Json(new { success = false, message = "A hired applicant cannot be rejected." });
                        if (string.IsNullOrWhiteSpace(comment))
                            return Json(new { success = false, message = "Enter a rejection reason." });
                        app.Status = "Rejected";
                        app.AdminComment = comment.Trim();
                    }
                    else
                    {
                        return Json(new { success = false, message = "Select a valid action." });
                    }

                    app.ReviewedBy = User.Identity.Name;
                    app.ReviewedDate = DateTime.Now;
                    app.ModifiedDate = DateTime.Now;
                    db.SaveChanges();

                    return Json(new { success = true, status = app.Status });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult DownloadApplicantFile(int fileId)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                JobRecruitingSchemaHelper.EnsureSchema(db);
                var file = db.JobApplicationFiles.FirstOrDefault(f => f.FileId == fileId);
                if (file == null || string.IsNullOrWhiteSpace(file.FilePath))
                    return HttpNotFound();

                string root = Path.GetFullPath(Server.MapPath("~/App_Data/" + JobRecruitingSchemaHelper.StorageFolderName))
                    .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
                string physical = Path.GetFullPath(Server.MapPath("~/App_Data/" + file.FilePath));
                if (!physical.StartsWith(root, StringComparison.OrdinalIgnoreCase) || !System.IO.File.Exists(physical))
                    return HttpNotFound();

                return File(physical, "application/octet-stream", Path.GetFileName(file.FileName));
            }
        }

        [HttpGet]
        public JsonResult GetQuickApplyUrl()
        {
            try
            {
                int employerId = 0;
                int.TryParse(User.Identity.GetClientAdminEmployerID(), out employerId);
                if (employerId <= 0)
                {
                    using (var admin = new AdminDbContext())
                    {
                        string clientId = User.Identity.GetSelectedClientID();
                        int cid;
                        if (int.TryParse(clientId, out cid))
                        {
                            var emp = admin.Employers.FirstOrDefault(e => e.EmployerId == cid);
                            if (emp != null) employerId = emp.EmployerId;
                        }
                    }
                }

                if (employerId <= 0)
                    return Json(new { success = false, message = "Employer could not be resolved for this login." }, JsonRequestBehavior.AllowGet);

                // One permanent careers URL per employer. The applicant signs in once,
                // sees every published open job, and can apply to multiple requisitions.
                string url = Url.Action("Index", "Apply", new { employerId = employerId }, Request.Url.Scheme);
                return Json(new { success = true, url = url, employerId = employerId }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
