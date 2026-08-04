using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Helpers;
using ExecViewHrk.WebUI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    [Authorize]
    public class JobPortalController : Controller
    {
        public PartialViewResult JobPortalPartial()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                JobRecruitingSchemaHelper.EnsureSchema(db);

                int? employeeId = null;
                var person = db.Persons.FirstOrDefault(p => p.eMail == User.Identity.Name);
                if (person != null)
                {
                    var emp = db.Employees.FirstOrDefault(e => e.PersonId == person.PersonId);
                    if (emp != null) employeeId = emp.EmployeeId;
                }

                ViewBag.HasEmployee = employeeId.HasValue;
                ViewBag.EmployeeId = employeeId;
                if (!employeeId.HasValue)
                {
                    ViewBag.IdentityMessage = "No employee record is linked to this login.";
                    return PartialView(new List<JobPortalOpenJobVm>());
                }

                var myApps = db.JobApplications
                    .Where(a => a.EmployeeId == employeeId.Value)
                    .ToList();

                var jobs = db.JobRequisitions
                    .Where(r => r.IsPublished && r.Status == "Open")
                    .OrderByDescending(r => r.OpenDate ?? r.RequisitionDate)
                    .ToList()
                    .Select(r =>
                    {
                        var existing = myApps.FirstOrDefault(a => a.RequisitionId == r.RequisitionId && a.Status != "Withdrawn");
                        return new JobPortalOpenJobVm
                        {
                            RequisitionId = r.RequisitionId,
                            RequisitionNumber = r.RequisitionNumber,
                            PositionTitle = r.PositionTitle,
                            Division = r.Division,
                            Department = r.Department,
                            Description = r.Description,
                            OpenDate = r.OpenDate,
                            AlreadyApplied = existing != null,
                            ExistingApplicationId = existing != null ? (int?)existing.ApplicationId : null,
                            ApplicationStatus = existing != null ? existing.Status : null
                        };
                    })
                    .ToList();

                ViewBag.MyApplications = myApps
                    .OrderByDescending(a => a.CreatedDate)
                    .Select(a =>
                    {
                        var req = db.JobRequisitions.FirstOrDefault(r => r.RequisitionId == a.RequisitionId);
                        return new JobApplicationListVm
                        {
                            ApplicationId = a.ApplicationId,
                            RequisitionId = a.RequisitionId,
                            PositionTitle = req != null ? req.PositionTitle : "",
                            RequisitionNumber = req != null ? req.RequisitionNumber : "",
                            Status = a.Status,
                            CreatedDate = a.CreatedDate,
                            SubmittedDate = a.SubmittedDate,
                            CurrentStep = a.CurrentStep
                        };
                    })
                    .ToList();

                int employerId = 0;
                int.TryParse(User.Identity.GetClientAdminEmployerID(), out employerId);
                if (employerId <= 0) int.TryParse(User.Identity.GetSelectedClientID(), out employerId);
                ViewBag.EmployerId = employerId;
                ViewBag.StartUrl = Url.Action("StartForEmployee", "Apply");

                return PartialView(jobs);
            }
        }
    }
}
