using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Helpers;
using ExecViewHrk.WebUI.Infrastructure;
using ExecViewHrk.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    [Authorize]
    public class BenefitsAdminController : Controller
    {
        public PartialViewResult SetupPartial()
        {
            Ensure();
            return PartialView();
        }

        public PartialViewResult DashboardPartial()
        {
            string conn = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(conn))
            {
                BenefitsEnrollmentSchemaHelper.EnsureSchema(db);
                var dash = BuildDashboard(db);
                return PartialView(dash);
            }
        }

        public PartialViewResult AssignPartial()
        {
            Ensure();
            return PartialView();
        }

        public PartialViewResult EnrollmentsPartial()
        {
            Ensure();
            return PartialView();
        }

        #region Categories

        [HttpGet]
        public JsonResult GetCategories()
        {
            using (var db = OpenDb())
            {
                var list = db.BenCategories.OrderBy(c => c.DisplayOrder).ThenBy(c => c.CategoryName)
                    .ToList().Select(MapCategory).ToList();
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveCategory(int categoryId, string categoryName, string description, bool isActive, int displayOrder)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
                return Json(new { success = false, message = "Category name is required." });
            using (var db = OpenDb())
            {
                BenCategory entity;
                if (categoryId > 0)
                {
                    entity = db.BenCategories.FirstOrDefault(c => c.CategoryId == categoryId);
                    if (entity == null) return Json(new { success = false, message = "Not found." });
                    entity.ModifiedBy = User.Identity.Name;
                    entity.ModifiedDate = DateTime.Now;
                }
                else
                {
                    entity = new BenCategory { CreatedBy = User.Identity.Name, CreatedDate = DateTime.Now };
                    db.BenCategories.Add(entity);
                }
                entity.CategoryName = categoryName.Trim();
                entity.Description = description;
                entity.IsActive = isActive;
                entity.DisplayOrder = displayOrder;
                db.SaveChanges();
                return Json(new { success = true, data = MapCategory(entity) });
            }
        }

        #endregion

        #region Waiting / Eligibility

        [HttpGet]
        public JsonResult GetWaitingPeriods()
        {
            using (var db = OpenDb())
            {
                var list = db.BenWaitingPeriods.OrderBy(w => w.Days).ToList().Select(w => new BenWaitingPeriodVm
                {
                    WaitingPeriodId = w.WaitingPeriodId,
                    Name = w.Name,
                    Days = w.Days,
                    CalculationType = w.CalculationType,
                    Description = w.Description,
                    IsActive = w.IsActive
                }).ToList();
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveWaitingPeriod(int waitingPeriodId, string name, int days, string calculationType, string description, bool isActive)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Json(new { success = false, message = "Name is required." });
            using (var db = OpenDb())
            {
                BenWaitingPeriod entity;
                if (waitingPeriodId > 0)
                {
                    entity = db.BenWaitingPeriods.FirstOrDefault(w => w.WaitingPeriodId == waitingPeriodId);
                    if (entity == null) return Json(new { success = false, message = "Not found." });
                }
                else
                {
                    entity = new BenWaitingPeriod();
                    db.BenWaitingPeriods.Add(entity);
                }
                entity.Name = name.Trim();
                entity.Days = days;
                entity.CalculationType = string.IsNullOrWhiteSpace(calculationType) ? "Days" : calculationType.Trim();
                entity.Description = description;
                entity.IsActive = isActive;
                db.SaveChanges();
                return Json(new { success = true });
            }
        }

        [HttpGet]
        public JsonResult GetEligibilityRules()
        {
            using (var db = OpenDb())
            {
                var list = db.BenEligibilityRules.OrderBy(r => r.RuleName).ToList().Select(r => new BenEligibilityRuleVm
                {
                    EligibilityRuleId = r.EligibilityRuleId,
                    RuleName = r.RuleName,
                    Description = r.Description,
                    EmploymentStatusIds = r.EmploymentStatusIds,
                    EmployeeTypeIds = r.EmployeeTypeIds,
                    MinHours = r.MinHours,
                    MinServiceDays = r.MinServiceDays,
                    MinAge = r.MinAge,
                    RuleExpression = r.RuleExpression,
                    IsActive = r.IsActive
                }).ToList();
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveEligibilityRule(BenEligibilityRuleVm vm)
        {
            if (vm == null || string.IsNullOrWhiteSpace(vm.RuleName))
                return Json(new { success = false, message = "Rule name is required." });
            using (var db = OpenDb())
            {
                BenEligibilityRule entity;
                if (vm.EligibilityRuleId > 0)
                {
                    entity = db.BenEligibilityRules.FirstOrDefault(r => r.EligibilityRuleId == vm.EligibilityRuleId);
                    if (entity == null) return Json(new { success = false, message = "Not found." });
                }
                else
                {
                    entity = new BenEligibilityRule();
                    db.BenEligibilityRules.Add(entity);
                }
                entity.RuleName = vm.RuleName.Trim();
                entity.Description = vm.Description;
                entity.EmploymentStatusIds = vm.EmploymentStatusIds;
                entity.EmployeeTypeIds = vm.EmployeeTypeIds;
                entity.MinHours = vm.MinHours;
                entity.MinServiceDays = vm.MinServiceDays;
                entity.MinAge = vm.MinAge;
                entity.RuleExpression = vm.RuleExpression;
                entity.IsActive = vm.IsActive;
                db.SaveChanges();
                return Json(new { success = true });
            }
        }

        #endregion

        #region Plans

        [HttpGet]
        public JsonResult GetPlans()
        {
            using (var db = OpenDb())
            {
                var cats = db.BenCategories.ToDictionary(c => c.CategoryId, c => c.CategoryName);
                var options = db.BenCoverageOptions.Where(o => o.IsActive).ToList();
                var list = db.BenPlans.OrderBy(p => p.PlanName).ToList().Select(p => MapPlan(p, cats, options)).ToList();
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SavePlan(BenPlanVm vm, string coverageOptionsJson)
        {
            if (vm == null || string.IsNullOrWhiteSpace(vm.PlanName) || vm.CategoryId <= 0)
                return Json(new { success = false, message = "Plan name and category are required." });

            if (!string.IsNullOrWhiteSpace(coverageOptionsJson))
            {
                try
                {
                    var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    vm.CoverageOptions = serializer.Deserialize<List<BenCoverageOptionVm>>(coverageOptionsJson);
                }
                catch
                {
                    return Json(new { success = false, message = "Invalid coverage options." });
                }
            }

            using (var db = OpenDb())
            {
                BenPlan entity;
                if (vm.PlanId > 0)
                {
                    entity = db.BenPlans.FirstOrDefault(p => p.PlanId == vm.PlanId);
                    if (entity == null) return Json(new { success = false, message = "Not found." });
                    entity.ModifiedBy = User.Identity.Name;
                    entity.ModifiedDate = DateTime.Now;
                }
                else
                {
                    entity = new BenPlan { CreatedBy = User.Identity.Name, CreatedDate = DateTime.Now };
                    db.BenPlans.Add(entity);
                }
                entity.PlanName = vm.PlanName.Trim();
                entity.PlanCode = vm.PlanCode;
                entity.CategoryId = vm.CategoryId;
                entity.Carrier = vm.Carrier;
                entity.Description = vm.Description;
                entity.EffectiveDate = vm.EffectiveDate;
                entity.ExpirationDate = vm.ExpirationDate;
                entity.EmployeeCost = vm.EmployeeCost;
                entity.EmployerCost = vm.EmployerCost;
                entity.RequireDependents = vm.RequireDependents;
                entity.RequireBeneficiary = vm.RequireBeneficiary;
                entity.WaiveAllowed = vm.WaiveAllowed;
                entity.IsActive = vm.IsActive;
                db.SaveChanges();

                if (vm.CoverageOptions != null && vm.CoverageOptions.Any(x => !string.IsNullOrWhiteSpace(x.OptionName)))
                {
                    var existing = db.BenCoverageOptions.Where(o => o.PlanId == entity.PlanId).ToList();
                    foreach (var e in existing) db.BenCoverageOptions.Remove(e);
                    int sort = 0;
                    foreach (var o in vm.CoverageOptions.Where(x => !string.IsNullOrWhiteSpace(x.OptionName)))
                    {
                        db.BenCoverageOptions.Add(new BenCoverageOption
                        {
                            PlanId = entity.PlanId,
                            OptionCode = string.IsNullOrWhiteSpace(o.OptionCode) ? ("OPT" + (++sort)) : o.OptionCode.Trim(),
                            OptionName = o.OptionName.Trim(),
                            EmployeeCost = o.EmployeeCost,
                            EmployerCost = o.EmployerCost,
                            RequiresDependent = o.RequiresDependent,
                            SortOrder = o.SortOrder > 0 ? o.SortOrder : ++sort,
                            IsActive = true
                        });
                    }
                    db.SaveChanges();
                }
                else if (vm.PlanId == 0)
                {
                    SeedDefaultCoverage(db, entity);
                    db.SaveChanges();
                }

                return Json(new { success = true, planId = entity.PlanId });
            }
        }

        private static void SeedDefaultCoverage(ClientDbContext db, BenPlan plan)
        {
            var defaults = new[]
            {
                new { Code = "EE", Name = "Employee Only (EE)", Dep = false },
                new { Code = "ES", Name = "Employee + Spouse", Dep = true },
                new { Code = "EC", Name = "Employee + Child", Dep = true },
                new { Code = "EF", Name = "Employee + Family", Dep = true }
            };
            int i = 0;
            foreach (var d in defaults)
            {
                db.BenCoverageOptions.Add(new BenCoverageOption
                {
                    PlanId = plan.PlanId,
                    OptionCode = d.Code,
                    OptionName = d.Name,
                    EmployeeCost = plan.EmployeeCost,
                    EmployerCost = plan.EmployerCost,
                    RequiresDependent = d.Dep,
                    SortOrder = ++i,
                    IsActive = true
                });
            }
        }

        #endregion

        #region Classes / OE / Assign

        [HttpGet]
        public JsonResult GetClasses()
        {
            using (var db = OpenDb())
            {
                var waits = db.BenWaitingPeriods.ToDictionary(w => w.WaitingPeriodId, w => w.Name);
                var rules = db.BenEligibilityRules.ToDictionary(r => r.EligibilityRuleId, r => r.RuleName);
                var classPlans = db.BenClassPlans.ToList();
                var plans = db.BenPlans.ToDictionary(p => p.PlanId, p => p.PlanName);
                var list = db.BenClasses.OrderBy(c => c.ClassName).ToList().Select(c =>
                {
                    var pids = classPlans.Where(cp => cp.BenefitClassId == c.BenefitClassId).OrderBy(cp => cp.SortOrder).Select(cp => cp.PlanId).ToList();
                    return new BenClassVm
                    {
                        BenefitClassId = c.BenefitClassId,
                        ClassName = c.ClassName,
                        Description = c.Description,
                        WaitingPeriodId = c.WaitingPeriodId,
                        WaitingPeriodName = c.WaitingPeriodId.HasValue && waits.ContainsKey(c.WaitingPeriodId.Value) ? waits[c.WaitingPeriodId.Value] : null,
                        EligibilityRuleId = c.EligibilityRuleId,
                        EligibilityRuleName = c.EligibilityRuleId.HasValue && rules.ContainsKey(c.EligibilityRuleId.Value) ? rules[c.EligibilityRuleId.Value] : null,
                        IsActive = c.IsActive,
                        PlanIds = pids,
                        PlanNames = pids.Where(id => plans.ContainsKey(id)).Select(id => plans[id]).ToList()
                    };
                }).ToList();
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveClass(int benefitClassId, string className, string description, int? waitingPeriodId, int? eligibilityRuleId, bool isActive, string planIds)
        {
            if (string.IsNullOrWhiteSpace(className))
                return Json(new { success = false, message = "Class name is required." });
            using (var db = OpenDb())
            {
                BenClass entity;
                if (benefitClassId > 0)
                {
                    entity = db.BenClasses.FirstOrDefault(c => c.BenefitClassId == benefitClassId);
                    if (entity == null) return Json(new { success = false, message = "Not found." });
                    entity.ModifiedBy = User.Identity.Name;
                    entity.ModifiedDate = DateTime.Now;
                }
                else
                {
                    entity = new BenClass { CreatedBy = User.Identity.Name, CreatedDate = DateTime.Now };
                    db.BenClasses.Add(entity);
                }
                entity.ClassName = className.Trim();
                entity.Description = description;
                entity.WaitingPeriodId = waitingPeriodId;
                entity.EligibilityRuleId = eligibilityRuleId;
                entity.IsActive = isActive;
                db.SaveChanges();

                var existing = db.BenClassPlans.Where(cp => cp.BenefitClassId == entity.BenefitClassId).ToList();
                foreach (var e in existing) db.BenClassPlans.Remove(e);
                var ids = ParseIdList(planIds);
                int sort = 0;
                foreach (var pid in ids)
                {
                    db.BenClassPlans.Add(new BenClassPlan { BenefitClassId = entity.BenefitClassId, PlanId = pid, SortOrder = ++sort });
                }
                db.SaveChanges();
                return Json(new { success = true, benefitClassId = entity.BenefitClassId });
            }
        }

        [HttpGet]
        public JsonResult GetEnrollmentPeriods()
        {
            using (var db = OpenDb())
            {
                var list = db.BenEnrollmentPeriods.OrderByDescending(p => p.StartDate).ToList().Select(p => new
                {
                    p.EnrollmentPeriodId,
                    p.EnrollmentName,
                    StartDate = p.StartDate.ToString("yyyy-MM-dd"),
                    EndDate = p.EndDate.ToString("yyyy-MM-dd"),
                    CoverageEffectiveDate = p.CoverageEffectiveDate.HasValue ? p.CoverageEffectiveDate.Value.ToString("yyyy-MM-dd") : null,
                    AllowChangesUntil = p.AllowChangesUntil.HasValue ? p.AllowChangesUntil.Value.ToString("yyyy-MM-dd") : null,
                    p.Status,
                    p.EnrollmentMessage,
                    p.ReminderEmails
                }).ToList();
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveEnrollmentPeriod(BenEnrollmentPeriodVm vm, string startDate, string endDate, string coverageEffectiveDate)
        {
            if (vm == null || string.IsNullOrWhiteSpace(vm.EnrollmentName))
                return Json(new { success = false, message = "Enrollment name is required." });

            DateTime parsedStart, parsedEnd;
            DateTime? parsedEff = null;
            if (!TryParseDate(startDate, out parsedStart))
                return Json(new { success = false, message = "Start date is required (YYYY-MM-DD)." });
            if (!TryParseDate(endDate, out parsedEnd))
                return Json(new { success = false, message = "End date is required (YYYY-MM-DD)." });
            if (!string.IsNullOrWhiteSpace(coverageEffectiveDate))
            {
                DateTime eff;
                if (!TryParseDate(coverageEffectiveDate, out eff))
                    return Json(new { success = false, message = "Coverage effective date is invalid." });
                parsedEff = eff;
            }

            using (var db = OpenDb())
            {
                BenEnrollmentPeriod entity;
                if (vm.EnrollmentPeriodId > 0)
                {
                    entity = db.BenEnrollmentPeriods.FirstOrDefault(p => p.EnrollmentPeriodId == vm.EnrollmentPeriodId);
                    if (entity == null) return Json(new { success = false, message = "Not found." });
                    entity.ModifiedBy = User.Identity.Name;
                    entity.ModifiedDate = DateTime.Now;
                }
                else
                {
                    entity = new BenEnrollmentPeriod { CreatedBy = User.Identity.Name, CreatedDate = DateTime.Now };
                    db.BenEnrollmentPeriods.Add(entity);
                }
                entity.EnrollmentName = vm.EnrollmentName.Trim();
                entity.StartDate = parsedStart;
                entity.EndDate = parsedEnd;
                entity.CoverageEffectiveDate = parsedEff;
                entity.AllowChangesUntil = vm.AllowChangesUntil;
                entity.Status = string.IsNullOrWhiteSpace(vm.Status) ? "Draft" : vm.Status;
                entity.EnrollmentMessage = vm.EnrollmentMessage;
                entity.ReminderEmails = vm.ReminderEmails;
                db.SaveChanges();
                return Json(new { success = true, enrollmentPeriodId = entity.EnrollmentPeriodId });
            }
        }

        private static bool TryParseDate(string value, out DateTime result)
        {
            result = default(DateTime);
            if (string.IsNullOrWhiteSpace(value)) return false;
            return DateTime.TryParseExact(value.Trim(), new[] { "yyyy-MM-dd", "MM/dd/yyyy", "M/d/yyyy" },
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out result)
                || DateTime.TryParse(value, System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.AssumeLocal, out result);
        }

        [HttpGet]
        public JsonResult GetEmployeeAssignments()
        {
            try
            {
                using (var db = OpenDb())
                {
                    var classes = db.BenClasses.ToDictionary(c => c.BenefitClassId, c => c.ClassName);
                    var assigns = db.BenEmployeeClasses.OrderByDescending(a => a.AssignedDate).ToList();
                    var empIds = assigns.Select(a => a.EmployeeId).Distinct().ToList();
                    var emps = empIds.Any()
                        ? db.Employees.Where(e => empIds.Contains(e.EmployeeId)).ToList()
                        : new List<Employee>();
                    var personIds = emps.Select(e => e.PersonId).Distinct().ToList();
                    var people = personIds.Any()
                        ? db.Persons.Where(p => personIds.Contains(p.PersonId)).ToDictionary(p => p.PersonId, p => (p.Firstname + " " + p.Lastname).Trim())
                        : new Dictionary<int, string>();

                    var list = assigns.Select(a =>
                    {
                        var emp = emps.FirstOrDefault(e => e.EmployeeId == a.EmployeeId);
                        string name = emp != null && people.ContainsKey(emp.PersonId) ? people[emp.PersonId] : ("#" + a.EmployeeId);
                        return new
                        {
                            a.EmployeeBenefitClassId,
                            a.EmployeeId,
                            EmployeeName = name,
                            FileNumber = emp != null ? emp.FileNumber : null,
                            a.BenefitClassId,
                            BenefitClassName = classes.ContainsKey(a.BenefitClassId) ? classes[a.BenefitClassId] : null,
                            EffectiveDate = a.EffectiveDate.HasValue ? a.EffectiveDate.Value.ToString("yyyy-MM-dd") : null
                        };
                    }).ToList();
                    return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.GetBaseException().Message, data = new object[0] }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult SearchEmployees(string q)
        {
            using (var db = OpenDb())
            {
                q = (q ?? "").Trim().ToLowerInvariant();
                if (q.Length < 2)
                    return Json(new { success = true, data = new object[0] }, JsonRequestBehavior.AllowGet);

                var emps = db.Employees.Where(e => e.TerminationDate == null).Take(500).ToList();
                var personIds = emps.Select(e => e.PersonId).Distinct().ToList();
                var people = db.Persons.Where(p => personIds.Contains(p.PersonId)).ToList();
                var list = emps.Select(e =>
                {
                    var p = people.FirstOrDefault(x => x.PersonId == e.PersonId);
                    string name = p == null ? "" : (p.Firstname + " " + p.Lastname).Trim();
                    return new { id = e.EmployeeId, text = name + " (" + (e.FileNumber ?? "") + ")", name, fileNumber = e.FileNumber };
                })
                .Where(x => (x.name ?? "").ToLowerInvariant().Contains(q) || (x.fileNumber ?? "").ToLowerInvariant().Contains(q))
                .OrderBy(x => x.name)
                .Take(50)
                .ToList();
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AssignBenefitClass(string employeeIds, int? employeeId, string benefitClassId, string effectiveDate)
        {
            try
            {
                DateTime eff;
                if (!TryParseDate(effectiveDate, out eff))
                    eff = DateTime.Today;

                int classId;
                if (!int.TryParse((benefitClassId ?? "").Trim(), out classId) || classId <= 0)
                    return Json(new { success = false, message = "Select a benefit class." });

                var ids = new List<int>();
                if (!string.IsNullOrWhiteSpace(employeeIds))
                {
                    foreach (var part in employeeIds.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        int id;
                        if (int.TryParse(part.Trim(), out id) && id > 0 && !ids.Contains(id))
                            ids.Add(id);
                    }
                }
                else if (employeeId.HasValue && employeeId.Value > 0)
                {
                    ids.Add(employeeId.Value);
                }

                if (!ids.Any())
                    return Json(new { success = false, message = "Select at least one employee." });

                using (var db = OpenDb())
                {
                    if (!db.BenClasses.Any(c => c.BenefitClassId == classId && c.IsActive))
                        return Json(new { success = false, message = "Benefit class not found or inactive." });

                    var validEmpIds = db.Employees.Where(e => ids.Contains(e.EmployeeId)).Select(e => e.EmployeeId).ToList();
                    if (!validEmpIds.Any())
                        return Json(new { success = false, message = "No valid employees found." });

                    var existing = db.BenEmployeeClasses.Where(a => validEmpIds.Contains(a.EmployeeId)).ToList();
                    foreach (var row in existing)
                        db.BenEmployeeClasses.Remove(row);

                    var now = DateTime.Now;
                    var user = User.Identity != null ? User.Identity.Name : null;
                    foreach (var empId in validEmpIds)
                    {
                        db.BenEmployeeClasses.Add(new BenEmployeeClass
                        {
                            EmployeeId = empId,
                            BenefitClassId = classId,
                            EffectiveDate = eff,
                            AssignedBy = user,
                            AssignedDate = now
                        });
                    }
                    db.SaveChanges();

                    try
                    {
                        var ip = Request.UserHostAddress;
                        if (!string.IsNullOrEmpty(ip) && ip.Length > 50)
                            ip = ip.Substring(0, 50);
                        foreach (var empId in validEmpIds)
                        {
                            db.BenAudits.Add(new BenAudit
                            {
                                EmployeeId = empId,
                                Action = "AssignBenefitClass",
                                Details = "BenefitClassId=" + classId,
                                PerformedBy = user,
                                PerformedDate = now,
                                IpAddress = ip
                            });
                        }
                        db.SaveChanges();
                    }
                    catch
                    {
                        // Assignment already saved; ignore audit failures.
                    }

                    var skipped = ids.Count - validEmpIds.Count;
                    var msg = validEmpIds.Count == 1
                        ? "Benefit class assigned."
                        : ("Benefit class assigned to " + validEmpIds.Count + " employees.");
                    if (skipped > 0) msg += " (" + skipped + " skipped)";
                    return Json(new { success = true, message = msg, assigned = validEmpIds.Count });
                }
            }
            catch (Exception ex)
            {
                var detail = ex.GetBaseException().Message;
                return Json(new { success = false, message = "Assign failed: " + detail });
            }
        }

        [HttpGet]
        public JsonResult GetEnrollments()
        {
            using (var db = OpenDb())
            {
                return Json(new { success = true, data = MapEnrollments(db) }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetDashboard()
        {
            using (var db = OpenDb())
            {
                return Json(new { success = true, data = BuildDashboard(db) }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ApproveEnrollment(int enrollmentId)
        {
            using (var db = OpenDb())
            {
                var en = db.BenEnrollments.FirstOrDefault(e => e.EnrollmentId == enrollmentId);
                if (en == null) return Json(new { success = false, message = "Enrollment not found." });
                if (en.Status != "Submitted")
                    return Json(new { success = false, message = "Only submitted enrollments can be approved." });
                en.Status = "Approved";
                en.ApprovedBy = User.Identity.Name;
                en.ApprovedDate = DateTime.Now;
                en.ModifiedDate = DateTime.Now;
                db.BenAudits.Add(new BenAudit
                {
                    EnrollmentId = en.EnrollmentId,
                    EmployeeId = en.EmployeeId,
                    Action = "Approve",
                    PerformedBy = User.Identity.Name,
                    PerformedDate = DateTime.Now,
                    IpAddress = Request.UserHostAddress
                });
                db.SaveChanges();
                return Json(new { success = true, message = "Enrollment approved." });
            }
        }

        #endregion

        #region Helpers

        private ClientDbContext OpenDb()
        {
            string conn = User.Identity.GetClientConnectionString();
            var db = new ClientDbContext(conn);
            BenefitsEnrollmentSchemaHelper.EnsureSchema(db);
            return db;
        }

        private void Ensure()
        {
            string conn = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(conn))
                BenefitsEnrollmentSchemaHelper.EnsureSchema(db);
        }

        private static BenCategoryVm MapCategory(BenCategory c)
        {
            return new BenCategoryVm
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                Description = c.Description,
                IsActive = c.IsActive,
                DisplayOrder = c.DisplayOrder
            };
        }

        private static BenPlanVm MapPlan(BenPlan p, Dictionary<int, string> cats, List<BenCoverageOption> allOptions)
        {
            return new BenPlanVm
            {
                PlanId = p.PlanId,
                PlanName = p.PlanName,
                PlanCode = p.PlanCode,
                CategoryId = p.CategoryId,
                CategoryName = cats.ContainsKey(p.CategoryId) ? cats[p.CategoryId] : null,
                Carrier = p.Carrier,
                Description = p.Description,
                EffectiveDate = p.EffectiveDate,
                ExpirationDate = p.ExpirationDate,
                EmployeeCost = p.EmployeeCost,
                EmployerCost = p.EmployerCost,
                RequireDependents = p.RequireDependents,
                RequireBeneficiary = p.RequireBeneficiary,
                WaiveAllowed = p.WaiveAllowed,
                IsActive = p.IsActive,
                CoverageOptions = allOptions.Where(o => o.PlanId == p.PlanId).OrderBy(o => o.SortOrder)
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
            };
        }

        private static List<int> ParseIdList(string csv)
        {
            if (string.IsNullOrWhiteSpace(csv)) return new List<int>();
            return csv.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => { int n; return int.TryParse(s.Trim(), out n) ? n : 0; })
                .Where(n => n > 0).Distinct().ToList();
        }

        private static List<BenEnrollmentListVm> MapEnrollments(ClientDbContext db)
        {
            var periods = db.BenEnrollmentPeriods.ToDictionary(p => p.EnrollmentPeriodId, p => p.EnrollmentName);
            var classes = db.BenClasses.ToDictionary(c => c.BenefitClassId, c => c.ClassName);
            var enrollments = db.BenEnrollments.OrderByDescending(e => e.SubmittedDate ?? e.CreatedDate).Take(200).ToList();
            var empIds = enrollments.Select(e => e.EmployeeId).Distinct().ToList();
            var emps = db.Employees.Where(e => empIds.Contains(e.EmployeeId)).ToList();
            var personIds = emps.Select(e => e.PersonId).Distinct().ToList();
            var people = db.Persons.Where(p => personIds.Contains(p.PersonId)).ToDictionary(p => p.PersonId, p => (p.Firstname + " " + p.Lastname).Trim());

            return enrollments.Select(e =>
            {
                var emp = emps.FirstOrDefault(x => x.EmployeeId == e.EmployeeId);
                string name = emp != null && people.ContainsKey(emp.PersonId) ? people[emp.PersonId] : ("#" + e.EmployeeId);
                return new BenEnrollmentListVm
                {
                    EnrollmentId = e.EnrollmentId,
                    EmployeeId = e.EmployeeId,
                    EmployeeName = name,
                    FileNumber = emp != null ? emp.FileNumber : null,
                    BenefitClassName = e.BenefitClassId.HasValue && classes.ContainsKey(e.BenefitClassId.Value) ? classes[e.BenefitClassId.Value] : null,
                    EnrollmentPeriodName = periods.ContainsKey(e.EnrollmentPeriodId) ? periods[e.EnrollmentPeriodId] : null,
                    Status = e.Status,
                    SubmittedDate = e.SubmittedDate,
                    ConfirmationNumber = e.ConfirmationNumber,
                    HasSignature = e.TermsAccepted && !string.IsNullOrEmpty(e.SignedName)
                };
            }).ToList();
        }

        private static BenDashboardVm BuildDashboard(ClientDbContext db)
        {
            var assigned = db.BenEmployeeClasses.Select(a => a.EmployeeId).Distinct().Count();
            var enrollments = db.BenEnrollments.ToList();
            var completed = enrollments.Count(e => e.Status == "Submitted" || e.Status == "Approved");
            var pending = assigned - enrollments.Select(e => e.EmployeeId).Distinct().Count();
            if (pending < 0) pending = 0;
            var inProgress = enrollments.Count(e => e.Status == "InProgress");
            var waived = db.BenElections.Count(x => x.IsWaived);
            return new BenDashboardVm
            {
                TotalAssigned = assigned,
                Completed = completed,
                Pending = pending,
                InProgress = inProgress,
                Waived = waived,
                Recent = MapEnrollments(db).Take(20).ToList()
            };
        }

        #endregion
    }
}
