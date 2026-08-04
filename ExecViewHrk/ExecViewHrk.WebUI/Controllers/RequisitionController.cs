using ExecViewHrk.EfAdmin;
using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Helpers;
using ExecViewHrk.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
                        Description = r.Description,
                        RequisitionDate = r.RequisitionDate,
                        OpenDate = r.OpenDate,
                        ClosedDate = r.ClosedDate,
                        Status = r.Status,
                        ApplicantCount = r.ApplicantCount,
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
                            r.ApplicantCount,
                            r.IsPublished,
                            CandidateCount = db.JobApplications.Count(a => a.RequisitionId == r.RequisitionId && a.Status == "Submitted")
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
        public JsonResult GetRequisition(int id)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                JobRecruitingSchemaHelper.EnsureSchema(db);
                var r = db.JobRequisitions.FirstOrDefault(x => x.RequisitionId == id);
                if (r == null) return Json(new { success = false, message = "Not found" }, JsonRequestBehavior.AllowGet);
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
                if (model == null || string.IsNullOrWhiteSpace(model.PositionTitle) || string.IsNullOrWhiteSpace(model.RequisitionNumber))
                    return Json(new { success = false, message = "Requisition number and position title are required." });

                string connString = User.Identity.GetClientConnectionString();
                using (var db = new ClientDbContext(connString))
                {
                    JobRecruitingSchemaHelper.EnsureSchema(db);
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
                    entity.PositionTitle = model.PositionTitle.Trim();
                    entity.Division = model.Division;
                    entity.Department = model.Department;
                    entity.PositionId = model.PositionId;
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
                            return new JobApplicationListVm
                            {
                                ApplicationId = a.ApplicationId,
                                RequisitionId = a.RequisitionId,
                                ApplicantName = string.IsNullOrWhiteSpace(name) ? "(unknown)" : name,
                                Status = a.Status,
                                CreatedDate = a.CreatedDate,
                                SubmittedDate = a.SubmittedDate,
                                CurrentStep = a.CurrentStep
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
        public JsonResult GetQuickApplyUrl(int requisitionId)
        {
            try
            {
                int employerId = 0;
                int.TryParse(User.Identity.GetClientAdminEmployerID(), out employerId);
                if (employerId <= 0)
                {
                    // Fall back: look up employer from selected client claim if available
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

                string url = Url.Action("Index", "Apply", new { employerId = employerId, requisitionId = requisitionId }, Request.Url.Scheme);
                return Json(new { success = true, url = url, employerId = employerId }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
