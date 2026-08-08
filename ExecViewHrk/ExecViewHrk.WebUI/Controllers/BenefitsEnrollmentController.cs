using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Helpers;
using ExecViewHrk.WebUI.Infrastructure;
using ExecViewHrk.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ExecViewHrk.WebUI.Controllers
{
    [Authorize]
    public class BenefitsEnrollmentController : Controller
    {
        public PartialViewResult MyBenefitsPartial()
        {
            string conn = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(conn))
            {
                BenefitsEnrollmentSchemaHelper.EnsureSchema(db);
                int? employeeId = ResolveEmployeeId(db);
                var portal = BuildPortal(db, employeeId);
                return PartialView(portal);
            }
        }

        public ActionResult Index()
        {
            return RedirectToAction("MyBenefitsPartial");
        }

        [HttpGet]
        public JsonResult GetPortalData()
        {
            string conn = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(conn))
            {
                BenefitsEnrollmentSchemaHelper.EnsureSchema(db);
                int? employeeId = ResolveEmployeeId(db);
                return Json(new { success = true, data = BuildPortal(db, employeeId) }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult StartEnrollment()
        {
            string conn = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(conn))
            {
                BenefitsEnrollmentSchemaHelper.EnsureSchema(db);
                int? employeeId = ResolveEmployeeId(db);
                if (!employeeId.HasValue)
                    return Json(new { success = false, message = "No employee record linked to this login." });

                var portal = BuildPortal(db, employeeId);
                if (!portal.IsEligible)
                    return Json(new { success = false, message = portal.Message ?? "You are currently not eligible." });

                var existing = db.BenEnrollments.FirstOrDefault(e =>
                    e.EmployeeId == employeeId.Value &&
                    e.EnrollmentPeriodId == portal.ActivePeriod.EnrollmentPeriodId &&
                    (e.Status == "InProgress" || e.Status == "Submitted" || e.Status == "Approved"));

                if (existing != null && (existing.Status == "Submitted" || existing.Status == "Approved"))
                    return Json(new { success = false, message = "Enrollment already submitted.", enrollmentId = existing.EnrollmentId });

                if (existing == null)
                {
                    existing = new BenEnrollment
                    {
                        EmployeeId = employeeId.Value,
                        EnrollmentPeriodId = portal.ActivePeriod.EnrollmentPeriodId,
                        BenefitClassId = portal.BenefitClass != null ? portal.BenefitClass.BenefitClassId : (int?)null,
                        Status = "InProgress",
                        CreatedDate = DateTime.Now
                    };
                    db.BenEnrollments.Add(existing);
                    db.BenAudits.Add(new BenAudit
                    {
                        EmployeeId = employeeId.Value,
                        Action = "StartEnrollment",
                        PerformedBy = User.Identity.Name,
                        PerformedDate = DateTime.Now,
                        IpAddress = Request.UserHostAddress
                    });
                    db.SaveChanges();
                }

                return Json(new { success = true, enrollmentId = existing.EnrollmentId, plans = portal.Plans });
            }
        }

        [HttpPost]
        public JsonResult SaveElections(int enrollmentId, string electionsJson)
        {
            string conn = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(conn))
            {
                BenefitsEnrollmentSchemaHelper.EnsureSchema(db);
                int? employeeId = ResolveEmployeeId(db);
                var en = db.BenEnrollments.FirstOrDefault(e => e.EnrollmentId == enrollmentId);
                if (en == null || (employeeId.HasValue && en.EmployeeId != employeeId.Value))
                    return Json(new { success = false, message = "Enrollment not found." });
                if (en.Status == "Submitted" || en.Status == "Approved")
                    return Json(new { success = false, message = "Enrollment is locked." });

                var serializer = new JavaScriptSerializer();
                var elections = serializer.Deserialize<List<BenElectionSaveVm>>(electionsJson ?? "[]") ?? new List<BenElectionSaveVm>();

                var existing = db.BenElections.Where(x => x.EnrollmentId == enrollmentId).ToList();
                foreach (var e in existing) db.BenElections.Remove(e);

                var period = db.BenEnrollmentPeriods.FirstOrDefault(p => p.EnrollmentPeriodId == en.EnrollmentPeriodId);
                DateTime? effective = period != null ? period.CoverageEffectiveDate : DateTime.Today;

                foreach (var el in elections)
                {
                    var plan = db.BenPlans.FirstOrDefault(p => p.PlanId == el.PlanId);
                    if (plan == null) continue;
                    double empCost = 0, erCost = 0;
                    if (!el.IsWaived && el.CoverageOptionId.HasValue)
                    {
                        var opt = db.BenCoverageOptions.FirstOrDefault(o => o.CoverageOptionId == el.CoverageOptionId.Value);
                        if (opt != null) { empCost = opt.EmployeeCost; erCost = opt.EmployerCost; }
                    }
                    else if (!el.IsWaived)
                    {
                        empCost = plan.EmployeeCost;
                        erCost = plan.EmployerCost;
                    }
                    db.BenElections.Add(new BenElection
                    {
                        EnrollmentId = enrollmentId,
                        PlanId = el.PlanId,
                        CoverageOptionId = el.IsWaived ? null : el.CoverageOptionId,
                        IsWaived = el.IsWaived,
                        EmployeeCost = empCost,
                        EmployerCost = erCost,
                        EffectiveDate = effective
                    });
                }
                en.ModifiedDate = DateTime.Now;
                db.SaveChanges();
                return Json(new { success = true });
            }
        }

        [HttpPost]
        public JsonResult SaveDependents(int enrollmentId, string dependentsJson)
        {
            string conn = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(conn))
            {
                BenefitsEnrollmentSchemaHelper.EnsureSchema(db);
                int? employeeId = ResolveEmployeeId(db);
                var en = db.BenEnrollments.FirstOrDefault(e => e.EnrollmentId == enrollmentId);
                if (en == null || (employeeId.HasValue && en.EmployeeId != employeeId.Value))
                    return Json(new { success = false, message = "Enrollment not found." });

                var serializer = new JavaScriptSerializer();
                var deps = serializer.Deserialize<List<BenDependentVm>>(dependentsJson ?? "[]") ?? new List<BenDependentVm>();
                var existing = db.BenDependents.Where(d => d.EnrollmentId == enrollmentId).ToList();
                foreach (var d in existing) db.BenDependents.Remove(d);
                foreach (var d in deps.Where(x => !string.IsNullOrWhiteSpace(x.FirstName)))
                {
                    db.BenDependents.Add(new BenDependent
                    {
                        EnrollmentId = enrollmentId,
                        ElectionId = d.ElectionId,
                        FirstName = d.FirstName.Trim(),
                        LastName = (d.LastName ?? "").Trim(),
                        Relationship = d.Relationship ?? "Dependent",
                        DateOfBirth = d.DateOfBirth,
                        Gender = d.Gender,
                        SSN = d.SSN
                    });
                }
                db.SaveChanges();
                return Json(new { success = true });
            }
        }

        [HttpPost]
        public JsonResult SaveBeneficiaries(int enrollmentId, string beneficiariesJson)
        {
            string conn = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(conn))
            {
                BenefitsEnrollmentSchemaHelper.EnsureSchema(db);
                int? employeeId = ResolveEmployeeId(db);
                var en = db.BenEnrollments.FirstOrDefault(e => e.EnrollmentId == enrollmentId);
                if (en == null || (employeeId.HasValue && en.EmployeeId != employeeId.Value))
                    return Json(new { success = false, message = "Enrollment not found." });

                var serializer = new JavaScriptSerializer();
                var bens = serializer.Deserialize<List<BenBeneficiaryVm>>(beneficiariesJson ?? "[]") ?? new List<BenBeneficiaryVm>();
                double total = bens.Sum(b => b.Percentage);
                if (bens.Count > 0 && Math.Abs(total - 100) > 0.01)
                    return Json(new { success = false, message = "Beneficiary percentages must total 100%." });

                var existing = db.BenBeneficiaries.Where(b => b.EnrollmentId == enrollmentId).ToList();
                foreach (var b in existing) db.BenBeneficiaries.Remove(b);
                foreach (var b in bens.Where(x => !string.IsNullOrWhiteSpace(x.Name)))
                {
                    db.BenBeneficiaries.Add(new BenBeneficiary
                    {
                        EnrollmentId = enrollmentId,
                        ElectionId = b.ElectionId,
                        Name = b.Name.Trim(),
                        Relationship = b.Relationship ?? "Beneficiary",
                        Percentage = b.Percentage
                    });
                }
                db.SaveChanges();
                return Json(new { success = true });
            }
        }

        [HttpPost]
        public JsonResult SubmitEnrollment(int enrollmentId, string signedName, bool termsAccepted)
        {
            if (!termsAccepted)
                return Json(new { success = false, message = "You must accept the benefit terms." });
            if (string.IsNullOrWhiteSpace(signedName))
                return Json(new { success = false, message = "Electronic signature name is required." });

            string conn = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(conn))
            {
                BenefitsEnrollmentSchemaHelper.EnsureSchema(db);
                int? employeeId = ResolveEmployeeId(db);
                var en = db.BenEnrollments.FirstOrDefault(e => e.EnrollmentId == enrollmentId);
                if (en == null || (employeeId.HasValue && en.EmployeeId != employeeId.Value))
                    return Json(new { success = false, message = "Enrollment not found." });
                if (en.Status == "Submitted" || en.Status == "Approved")
                    return Json(new { success = false, message = "Already submitted." });

                string conf = BenefitsEnrollmentSchemaHelper.NewConfirmationNumber();
                en.Status = "Submitted";
                en.SubmittedDate = DateTime.Now;
                en.ConfirmationNumber = conf;
                en.TermsAccepted = true;
                en.SignedName = signedName.Trim();
                en.SignedDate = DateTime.Now;
                en.SignedIp = Request.UserHostAddress;
                en.ModifiedDate = DateTime.Now;

                db.BenAudits.Add(new BenAudit
                {
                    EnrollmentId = en.EnrollmentId,
                    EmployeeId = en.EmployeeId,
                    Action = "Submit",
                    Details = conf,
                    PerformedBy = User.Identity.Name,
                    PerformedDate = DateTime.Now,
                    IpAddress = Request.UserHostAddress
                });
                db.SaveChanges();

                return Json(new
                {
                    success = true,
                    message = "Enrollment successful.",
                    confirmationNumber = conf,
                    submittedDate = en.SubmittedDate.Value.ToString("MM/dd/yyyy HH:mm")
                });
            }
        }

        private int? ResolveEmployeeId(ClientDbContext db)
        {
            string userName = User.Identity.Name ?? "";
            var asp = db.AspNetUsers.FirstOrDefault(u => u.UserName == userName);
            string email = asp != null ? asp.Email : userName;
            var person = db.Persons.FirstOrDefault(p => p.eMail == email || p.eMail == userName);
            if (person == null) return null;
            var emp = db.Employees.Where(e => e.PersonId == person.PersonId && e.TerminationDate == null)
                .OrderByDescending(e => e.HireDate)
                .FirstOrDefault();
            return emp != null ? emp.EmployeeId : (int?)null;
        }

        private BenPortalVm BuildPortal(ClientDbContext db, int? employeeId)
        {
            var vm = new BenPortalVm { IsEligible = false, Plans = new List<BenPlanVm>() };
            if (!employeeId.HasValue)
            {
                vm.Message = "No employee record is linked to this login.";
                return vm;
            }

            var today = DateTime.Today;
            var period = db.BenEnrollmentPeriods
                .Where(p => p.Status == "Active" && p.StartDate <= today && p.EndDate >= today)
                .OrderByDescending(p => p.StartDate)
                .FirstOrDefault();

            if (period == null)
            {
                vm.Message = "There is no active open enrollment period.";
                return vm;
            }

            vm.ActivePeriod = new BenEnrollmentPeriodVm
            {
                EnrollmentPeriodId = period.EnrollmentPeriodId,
                EnrollmentName = period.EnrollmentName,
                StartDate = period.StartDate,
                EndDate = period.EndDate,
                CoverageEffectiveDate = period.CoverageEffectiveDate,
                AllowChangesUntil = period.AllowChangesUntil,
                Status = period.Status,
                EnrollmentMessage = period.EnrollmentMessage,
                ReminderEmails = period.ReminderEmails
            };
            vm.Deadline = period.AllowChangesUntil ?? period.EndDate;

            var assign = db.BenEmployeeClasses.Where(a => a.EmployeeId == employeeId.Value)
                .OrderByDescending(a => a.AssignedDate).FirstOrDefault();
            if (assign == null)
            {
                vm.Message = "You are currently not eligible. No benefit class has been assigned.";
                return vm;
            }

            var benClass = db.BenClasses.FirstOrDefault(c => c.BenefitClassId == assign.BenefitClassId && c.IsActive);
            if (benClass == null)
            {
                vm.Message = "You are currently not eligible.";
                return vm;
            }

            var employee = db.Employees.FirstOrDefault(e => e.EmployeeId == employeeId.Value);
            string eligibilityError = ValidateEligibility(db, employee, benClass);
            if (eligibilityError != null)
            {
                vm.Message = eligibilityError;
                return vm;
            }

            vm.IsEligible = true;
            vm.Message = period.EnrollmentMessage;
            vm.BenefitClass = new BenClassVm
            {
                BenefitClassId = benClass.BenefitClassId,
                ClassName = benClass.ClassName,
                Description = benClass.Description,
                WaitingPeriodId = benClass.WaitingPeriodId,
                EligibilityRuleId = benClass.EligibilityRuleId,
                IsActive = benClass.IsActive
            };

            var planIds = db.BenClassPlans.Where(cp => cp.BenefitClassId == benClass.BenefitClassId)
                .OrderBy(cp => cp.SortOrder).Select(cp => cp.PlanId).ToList();
            var cats = db.BenCategories.ToDictionary(c => c.CategoryId, c => c.CategoryName);
            var options = db.BenCoverageOptions.Where(o => o.IsActive).ToList();
            var plans = db.BenPlans.Where(p => planIds.Contains(p.PlanId) && p.IsActive).ToList();
            vm.Plans = planIds.Select(id => plans.FirstOrDefault(p => p.PlanId == id)).Where(p => p != null)
                .Select(p => new BenPlanVm
                {
                    PlanId = p.PlanId,
                    PlanName = p.PlanName,
                    PlanCode = p.PlanCode,
                    CategoryId = p.CategoryId,
                    CategoryName = cats.ContainsKey(p.CategoryId) ? cats[p.CategoryId] : null,
                    Carrier = p.Carrier,
                    Description = p.Description,
                    EmployeeCost = p.EmployeeCost,
                    EmployerCost = p.EmployerCost,
                    RequireDependents = p.RequireDependents,
                    RequireBeneficiary = p.RequireBeneficiary,
                    WaiveAllowed = p.WaiveAllowed,
                    IsActive = p.IsActive,
                    CoverageOptions = options.Where(o => o.PlanId == p.PlanId).OrderBy(o => o.SortOrder)
                        .Select(o => new BenCoverageOptionVm
                        {
                            CoverageOptionId = o.CoverageOptionId,
                            PlanId = o.PlanId,
                            OptionCode = o.OptionCode,
                            OptionName = o.OptionName,
                            EmployeeCost = o.EmployeeCost,
                            EmployerCost = o.EmployerCost,
                            RequiresDependent = o.RequiresDependent,
                            SortOrder = o.SortOrder,
                            IsActive = o.IsActive
                        }).ToList()
                }).ToList();

            var enrollment = db.BenEnrollments
                .Where(e => e.EmployeeId == employeeId.Value && e.EnrollmentPeriodId == period.EnrollmentPeriodId)
                .OrderByDescending(e => e.CreatedDate)
                .FirstOrDefault();
            if (enrollment != null)
            {
                vm.EnrollmentId = enrollment.EnrollmentId;
                vm.EnrollmentStatus = enrollment.Status;
                vm.ConfirmationNumber = enrollment.ConfirmationNumber;
            }

            return vm;
        }

        private static string ValidateEligibility(ClientDbContext db, Employee employee, BenClass benClass)
        {
            if (employee == null) return "Employee record not found.";

            if (benClass.WaitingPeriodId.HasValue)
            {
                var wait = db.BenWaitingPeriods.FirstOrDefault(w => w.WaitingPeriodId == benClass.WaitingPeriodId.Value);
                if (wait != null && wait.IsActive)
                {
                    DateTime eligibleOn;
                    var h = employee.HireDate.Date;
                    if (string.Equals(wait.CalculationType, "FirstDayNextMonth", StringComparison.OrdinalIgnoreCase))
                        eligibleOn = new DateTime(h.Year, h.Month, 1).AddMonths(1);
                    else
                        eligibleOn = h.AddDays(wait.Days);
                    if (DateTime.Today < eligibleOn)
                        return "You are currently not eligible. Waiting period ends on " + eligibleOn.ToString("MM/dd/yyyy") + ".";
                }
            }

            if (benClass.EligibilityRuleId.HasValue)
            {
                var rule = db.BenEligibilityRules.FirstOrDefault(r => r.EligibilityRuleId == benClass.EligibilityRuleId.Value && r.IsActive);
                if (rule != null)
                {
                    if (rule.MinServiceDays.HasValue)
                    {
                        int days = (DateTime.Today - employee.HireDate.Date).Days;
                        if (days < rule.MinServiceDays.Value)
                            return "You are currently not eligible. Minimum service days not met.";
                    }
                    if (rule.MinHours.HasValue && employee.Hours.HasValue && (double)employee.Hours.Value < rule.MinHours.Value)
                        return "You are currently not eligible. Minimum hours not met.";
                    if (!string.IsNullOrWhiteSpace(rule.EmploymentStatusIds) && employee.EmploymentStatusId.HasValue)
                    {
                        var ids = rule.EmploymentStatusIds.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => { int n; return int.TryParse(s.Trim(), out n) ? n : 0; }).Where(n => n > 0).ToList();
                        if (ids.Any() && !ids.Contains(employee.EmploymentStatusId.Value))
                            return "You are currently not eligible based on employment status.";
                    }
                }
            }

            return null;
        }
    }
}
