using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Helpers;
using ExecViewHrk.WebUI.Infrastructure;
using ExecViewHrk.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ExecViewHrk.WebUI.Controllers
{
    [Authorize]
    public class PerformanceAdminController : Controller
    {
        public PartialViewResult SetupPartial()
        {
            Ensure();
            return PartialView();
        }

        public PartialViewResult DashboardPartial()
        {
            Ensure();
            return PartialView();
        }

        public PartialViewResult StartPartial()
        {
            Ensure();
            return PartialView();
        }

        public PartialViewResult ApprovalsPartial()
        {
            Ensure();
            return PartialView();
        }

        #region Lookups / Dashboard

        [HttpGet]
        public JsonResult GetDashboard()
        {
            using (var db = OpenDb())
            {
                int? personId = ResolvePersonId(db);
                var dash = new PrDashboardVm
                {
                    DraftReviews = db.PrReviews.Count(r => r.Status == "Draft"),
                    InProgress = db.PrReviews.Count(r => r.Status == "InProgress"),
                    AwaitingHr = db.PrReviewReviewerEmployees.Count(t => t.Status == "Pending" && t.ReviewerRole == "HR"),
                    Completed = db.PrReviews.Count(r => r.Status == "Completed"),
                    MyOpenTasks = personId.HasValue
                        ? db.PrReviewReviewerEmployees.Count(t => t.AssignedPersonId == personId.Value && t.Status == "Pending")
                        : 0
                };
                return Json(new { success = true, data = dash }, JsonRequestBehavior.AllowGet);
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

                var list = BuildEmployeeOptions(db)
                    .Where(x => (x.name ?? "").ToLowerInvariant().Contains(q) || (x.fileNumber ?? "").ToLowerInvariant().Contains(q))
                    .OrderBy(x => x.name)
                    .Take(50)
                    .ToList();
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>Full active-employee list for dropdown / listbox pickers.</summary>
        [HttpGet]
        public JsonResult ListEmployees()
        {
            using (var db = OpenDb())
            {
                var list = BuildEmployeeOptions(db).OrderBy(x => x.name).Take(1000).ToList();
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        private static List<PrEmployeeOptionVm> BuildEmployeeOptions(ClientDbContext db)
        {
            var emps = db.Employees.Where(e => e.TerminationDate == null).Take(1000).ToList();
            var personIds = emps.Select(e => e.PersonId).Distinct().ToList();
            var people = db.Persons.Where(p => personIds.Contains(p.PersonId)).ToList();
            return emps.Select(e =>
            {
                var p = people.FirstOrDefault(x => x.PersonId == e.PersonId);
                string name = p == null ? "" : (p.Firstname + " " + p.Lastname).Trim();
                return new PrEmployeeOptionVm
                {
                    id = e.EmployeeId,
                    personId = e.PersonId,
                    text = name + " (" + (e.FileNumber ?? "") + ")",
                    name = name,
                    fileNumber = e.FileNumber
                };
            }).ToList();
        }

        #endregion

        #region Rating scales / types / sections / criteria

        [HttpGet]
        public JsonResult GetScoreContents()
        {
            using (var db = OpenDb())
            {
                var list = db.PrReviewScoreContents.OrderBy(x => x.SortOrder).ThenBy(x => x.ItemName)
                    .ToList().Select(x => new PrScoreContentVm
                    {
                        Id = x.Id,
                        ItemName = x.ItemName,
                        ItemValue = x.ItemValue,
                        SortOrder = x.SortOrder,
                        IsActive = x.IsActive
                    }).ToList();
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveScoreContent(int id, string itemName, double itemValue, int sortOrder, bool isActive)
        {
            if (string.IsNullOrWhiteSpace(itemName))
                return Json(new { success = false, message = "Name is required." });
            using (var db = OpenDb())
            {
                PrReviewScoreContent entity;
                if (id > 0)
                {
                    entity = db.PrReviewScoreContents.FirstOrDefault(x => x.Id == id);
                    if (entity == null) return Json(new { success = false, message = "Not found." });
                }
                else
                {
                    entity = new PrReviewScoreContent();
                    db.PrReviewScoreContents.Add(entity);
                }
                entity.ItemName = itemName.Trim();
                entity.ItemValue = itemValue;
                entity.SortOrder = sortOrder;
                entity.IsActive = isActive;
                db.SaveChanges();
                return Json(new { success = true, id = entity.Id });
            }
        }

        [HttpGet]
        public JsonResult GetCriteriaTypes()
        {
            using (var db = OpenDb())
            {
                var list = db.PrReviewCriteriaTypes.OrderBy(x => x.Description).ToList()
                    .Select(x => new PrCriteriaTypeVm
                    {
                        ReviewCriteriaTypeId = x.ReviewCriteriaTypeId,
                        Description = x.Description,
                        IsActive = x.IsActive
                    }).ToList();
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveCriteriaType(int reviewCriteriaTypeId, string description, bool isActive)
        {
            if (string.IsNullOrWhiteSpace(description))
                return Json(new { success = false, message = "Description is required." });
            using (var db = OpenDb())
            {
                PrReviewCriteriaType entity;
                if (reviewCriteriaTypeId > 0)
                {
                    entity = db.PrReviewCriteriaTypes.FirstOrDefault(x => x.ReviewCriteriaTypeId == reviewCriteriaTypeId);
                    if (entity == null) return Json(new { success = false, message = "Not found." });
                }
                else
                {
                    entity = new PrReviewCriteriaType();
                    db.PrReviewCriteriaTypes.Add(entity);
                }
                entity.Description = description.Trim();
                entity.IsActive = isActive;
                db.SaveChanges();
                return Json(new { success = true, id = entity.ReviewCriteriaTypeId });
            }
        }

        [HttpGet]
        public JsonResult GetResponseTypes()
        {
            using (var db = OpenDb())
            {
                var list = db.PrResponseTypes.OrderBy(x => x.SortOrder).ThenBy(x => x.Description).ToList()
                    .Select(x => new PrResponseTypeVm
                    {
                        ResponseTypeId = x.ResponseTypeId,
                        Code = x.Code,
                        Description = x.Description,
                        SortOrder = x.SortOrder,
                        IsActive = x.IsActive,
                        IsDefault = x.IsDefault
                    }).ToList();
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveResponseType(int responseTypeId, string code, string description, int sortOrder, bool isActive, bool isDefault)
        {
            if (string.IsNullOrWhiteSpace(description))
                return Json(new { success = false, message = "Description is required." });
            using (var db = OpenDb())
            {
                string normalizedCode = string.IsNullOrWhiteSpace(code)
                    ? description.Trim().Replace(" ", "")
                    : code.Trim();

                PrResponseType entity;
                if (responseTypeId > 0)
                {
                    entity = db.PrResponseTypes.FirstOrDefault(x => x.ResponseTypeId == responseTypeId);
                    if (entity == null) return Json(new { success = false, message = "Not found." });
                }
                else
                {
                    if (db.PrResponseTypes.Any(x => x.Code == normalizedCode))
                        return Json(new { success = false, message = "A response type with this code already exists." });
                    entity = new PrResponseType();
                    db.PrResponseTypes.Add(entity);
                }

                if (isDefault)
                {
                    foreach (var row in db.PrResponseTypes.Where(x => x.IsDefault && x.ResponseTypeId != responseTypeId).ToList())
                        row.IsDefault = false;
                }

                entity.Code = normalizedCode;
                entity.Description = description.Trim();
                entity.SortOrder = sortOrder;
                entity.IsActive = isActive;
                entity.IsDefault = isDefault;
                db.SaveChanges();
                return Json(new { success = true, id = entity.ResponseTypeId });
            }
        }

        [HttpGet]
        public JsonResult GetSections()
        {
            using (var db = OpenDb())
            {
                var list = db.PrCriteriaSections.OrderBy(x => x.SortOrder).ThenBy(x => x.SectionName).ToList()
                    .Select(x => new PrSectionVm
                    {
                        SectionId = x.SectionId,
                        SectionName = x.SectionName,
                        Description = x.Description,
                        SortOrder = x.SortOrder,
                        IsActive = x.IsActive
                    }).ToList();
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveSection(int sectionId, string sectionName, string description, int sortOrder, bool isActive)
        {
            if (string.IsNullOrWhiteSpace(sectionName))
                return Json(new { success = false, message = "Section name is required." });
            using (var db = OpenDb())
            {
                PrCriteriaSection entity;
                if (sectionId > 0)
                {
                    entity = db.PrCriteriaSections.FirstOrDefault(x => x.SectionId == sectionId);
                    if (entity == null) return Json(new { success = false, message = "Not found." });
                }
                else
                {
                    entity = new PrCriteriaSection();
                    db.PrCriteriaSections.Add(entity);
                }
                entity.SectionName = sectionName.Trim();
                entity.Description = description;
                entity.SortOrder = sortOrder;
                entity.IsActive = isActive;
                db.SaveChanges();
                return Json(new { success = true, id = entity.SectionId });
            }
        }

        [HttpGet]
        public JsonResult GetCriteria()
        {
            using (var db = OpenDb())
            {
                var types = db.PrReviewCriteriaTypes.ToDictionary(t => t.ReviewCriteriaTypeId, t => t.Description);
                var sections = db.PrCriteriaSections.ToDictionary(s => s.SectionId, s => s.SectionName);
                var respTypes = db.PrResponseTypes.ToDictionary(r => r.ResponseTypeId, r => r);
                var defaultRespId = db.PrResponseTypes.Where(r => r.IsDefault && r.IsActive).Select(r => r.ResponseTypeId).FirstOrDefault();
                if (defaultRespId == 0)
                    defaultRespId = db.PrResponseTypes.Where(r => r.IsActive).OrderBy(r => r.SortOrder).Select(r => r.ResponseTypeId).FirstOrDefault();

                var list = db.PrReviewCriterias.OrderBy(c => c.SequenceNumber).ThenBy(c => c.Description).ToList()
                    .Select(c =>
                    {
                        PrResponseType rt = null;
                        if (c.ResponseTypeId > 0 && respTypes.ContainsKey(c.ResponseTypeId))
                            rt = respTypes[c.ResponseTypeId];
                        return new PrCriteriaVm
                        {
                            ReviewCriteriaId = c.ReviewCriteriaId,
                            CriteriaTypeId = c.CriteriaTypeId,
                            CriteriaTypeName = c.CriteriaTypeId.HasValue && types.ContainsKey(c.CriteriaTypeId.Value) ? types[c.CriteriaTypeId.Value] : null,
                            SectionId = c.SectionId,
                            SectionName = c.SectionId.HasValue && sections.ContainsKey(c.SectionId.Value) ? sections[c.SectionId.Value] : null,
                            Description = c.Description,
                            ResponseTypeId = c.ResponseTypeId > 0 ? c.ResponseTypeId : defaultRespId,
                            ResponseTypeName = rt != null ? rt.Description : null,
                            ResponseTypeCode = rt != null ? rt.Code : null,
                            SequenceNumber = c.SequenceNumber,
                            IsActive = c.IsActive,
                            Caption1 = c.Caption1
                        };
                    }).ToList();
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveCriteria(PrCriteriaVm vm)
        {
            if (vm == null || string.IsNullOrWhiteSpace(vm.Description))
                return Json(new { success = false, message = "Description is required." });
            using (var db = OpenDb())
            {
                PrReviewCriteria entity;
                if (vm.ReviewCriteriaId > 0)
                {
                    entity = db.PrReviewCriterias.FirstOrDefault(x => x.ReviewCriteriaId == vm.ReviewCriteriaId);
                    if (entity == null) return Json(new { success = false, message = "Not found." });
                }
                else
                {
                    entity = new PrReviewCriteria();
                    db.PrReviewCriterias.Add(entity);
                }
                entity.Description = vm.Description.Trim();
                entity.CriteriaTypeId = vm.CriteriaTypeId;
                entity.SectionId = vm.SectionId;
                if (vm.ResponseTypeId > 0)
                    entity.ResponseTypeId = vm.ResponseTypeId;
                else
                {
                    var def = db.PrResponseTypes.FirstOrDefault(r => r.IsDefault && r.IsActive)
                        ?? db.PrResponseTypes.Where(r => r.IsActive).OrderBy(r => r.SortOrder).FirstOrDefault();
                    entity.ResponseTypeId = def != null ? def.ResponseTypeId : 1;
                }
                entity.SequenceNumber = vm.SequenceNumber > 0 ? vm.SequenceNumber : 1;
                entity.IsActive = vm.IsActive;
                entity.Caption1 = vm.Caption1;
                db.SaveChanges();
                return Json(new { success = true, id = entity.ReviewCriteriaId });
            }
        }

        #endregion

        #region Review master

        [HttpGet]
        public JsonResult GetReviews()
        {
            using (var db = OpenDb())
            {
                var list = db.PrReviews.OrderByDescending(r => r.CreatedDate).ToList().Select(r => MapReview(db, r)).ToList();
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveReview(PrReviewVm vm, string employeeIds, string employeeIdsCsv, string stepsJson, string criteriaIds)
        {
            if (vm == null || string.IsNullOrWhiteSpace(vm.ReviewName))
                return Json(new { success = false, message = "Review name is required." });

            // Prefer dedicated CSV fields; also accept vm.EmployeeIds (model binder).
            var empIds = ParseIntList(!string.IsNullOrWhiteSpace(employeeIdsCsv) ? employeeIdsCsv : employeeIds);
            if (!empIds.Any() && vm.EmployeeIds != null)
                empIds = vm.EmployeeIds.Where(id => id > 0).Distinct().ToList();

            List<PrStepVm> steps = null;
            if (!string.IsNullOrWhiteSpace(stepsJson))
            {
                try { steps = new JavaScriptSerializer().Deserialize<List<PrStepVm>>(stepsJson); }
                catch { return Json(new { success = false, message = "Invalid steps payload." }); }
            }

            if (steps == null || !steps.Any())
            {
                steps = new List<PrStepVm>
                {
                    new PrStepVm { StepOrder = 1, ReviewerRole = "Approver1", IsViewPriorResponses = false },
                    new PrStepVm { StepOrder = 4, ReviewerRole = "HR", IsViewPriorResponses = true }
                };
            }

            steps = steps.Where(s => !IsSkippedStepRole(s.ReviewerRole)).ToList();

            var approver1Steps = steps.Where(s => string.Equals((s.ReviewerRole ?? "").Trim(), "Approver1", StringComparison.OrdinalIgnoreCase)).ToList();
            if (!approver1Steps.Any())
                return Json(new { success = false, message = "Approver1 step is required." });
            if (approver1Steps.Any(s => s.CriteriaIds == null || !s.CriteriaIds.Any()))
                return Json(new { success = false, message = "Approver1 must have at least one criteria assigned. Other steps are optional." });
            if (!empIds.Any())
                return Json(new { success = false, message = "Select at least one employee, then click Add before saving." });

            using (var db = OpenDb())
            {
                var empRows = db.Employees.Where(e => empIds.Contains(e.EmployeeId)).ToList();
                if (!empRows.Any())
                    return Json(new { success = false, message = "None of the selected employee IDs were found. Re-select employees and try again." });

                PrReview entity;
                if (vm.ReviewId > 0)
                {
                    entity = db.PrReviews.FirstOrDefault(r => r.ReviewId == vm.ReviewId);
                    if (entity == null) return Json(new { success = false, message = "Not found." });
                    entity.ModifiedBy = User.Identity.Name;
                    entity.ModifiedDate = DateTime.Now;
                }
                else
                {
                    entity = new PrReview
                    {
                        CreatedBy = User.Identity.Name,
                        CreatedDate = DateTime.Now,
                        Status = "Draft"
                    };
                    db.PrReviews.Add(entity);
                }
                entity.ReviewName = vm.ReviewName.Trim();
                entity.RevieweeMode = string.IsNullOrWhiteSpace(vm.RevieweeMode) ? "Employee" : vm.RevieweeMode;
                entity.HrOwner = vm.HrOwner ?? User.Identity.Name;
                entity.Notes = vm.Notes;
                if (!string.IsNullOrWhiteSpace(vm.Status)) entity.Status = vm.Status;
                db.SaveChanges();

                var sched = db.PrReviewSchedules.FirstOrDefault(s => s.ReviewId == entity.ReviewId);
                if (sched == null)
                {
                    sched = new PrReviewSchedule { ReviewId = entity.ReviewId };
                    db.PrReviewSchedules.Add(sched);
                }
                sched.IntervalType = vm.IntervalType;
                sched.FromSchedule = vm.FromSchedule;
                DateTime fromDate;
                if (TryParseDate(vm.FromDate, out fromDate)) sched.FromDate = fromDate;
                else if (string.Equals(vm.FromSchedule, "Custom Date", StringComparison.OrdinalIgnoreCase) == false)
                    sched.FromDate = null;
                sched.DaysToComplete = vm.DaysToComplete > 0 ? vm.DaysToComplete : 14;
                sched.RepeatDays = vm.RepeatDays;

                var rule = db.PrReviewScoringRules.FirstOrDefault(s => s.ReviewId == entity.ReviewId);
                if (rule == null)
                {
                    rule = new PrReviewScoringRule { ReviewId = entity.ReviewId };
                    db.PrReviewScoringRules.Add(rule);
                }
                rule.IsAverageOfAllQuestions = vm.IsAverageOfAllQuestions || (!vm.IsSumOfAllQuestions && !vm.WeightedAverage);
                rule.IsSumOfAllQuestions = vm.IsSumOfAllQuestions;
                rule.WeightedAverage = vm.WeightedAverage;

                var oldEmps = db.PrReviewEmployees.Where(e => e.ReviewId == entity.ReviewId).ToList();
                foreach (var o in oldEmps) db.PrReviewEmployees.Remove(o);
                foreach (var emp in empRows)
                {
                    db.PrReviewEmployees.Add(new PrReviewEmployee
                    {
                        ReviewId = entity.ReviewId,
                        EmployeeId = emp.EmployeeId,
                        PersonId = emp.PersonId,
                        CreatedBy = User.Identity.Name,
                        CreatedDate = DateTime.Now
                    });
                }

                var oldSteps = db.PrReviewNotificationSteps.Where(s => s.ReviewId == entity.ReviewId).ToList();
                var oldStepIds = oldSteps.Select(s => s.Id).ToList();
                var oldLinks = db.PrReviewReviewerCriterias.Where(c => c.ReviewId == entity.ReviewId || oldStepIds.Contains(c.NotificationStepId)).ToList();
                foreach (var l in oldLinks) db.PrReviewReviewerCriterias.Remove(l);
                foreach (var s in oldSteps) db.PrReviewNotificationSteps.Remove(s);
                db.SaveChanges();

                int order = 0;
                int firstOrder = steps.Min(s => s.StepOrder > 0 ? s.StepOrder : int.MaxValue);
                if (firstOrder == int.MaxValue) firstOrder = 1;
                var usedCriteria = new HashSet<int>();
                foreach (var st in steps.OrderBy(s => s.StepOrder))
                {
                    string role = string.IsNullOrWhiteSpace(st.ReviewerRole) ? "" : st.ReviewerRole.Trim();
                    if (IsSkippedStepRole(role)) continue;
                    var personIds = st.OtherPersonIds != null ? st.OtherPersonIds.Where(id => id > 0).Distinct().ToList() : new List<int>();
                    if (!personIds.Any() && st.OtherPersonId.HasValue && st.OtherPersonId.Value > 0)
                        personIds.Add(st.OtherPersonId.Value);

                    int stepOrder = st.StepOrder > 0 ? st.StepOrder : ++order;
                    bool viewPrior = st.IsViewPriorResponses && stepOrder != firstOrder;

                    var stepEntity = new PrReviewNotificationStep
                    {
                        ReviewId = entity.ReviewId,
                        StepOrder = stepOrder,
                        ReviewerRole = role,
                        OtherPersonId = personIds.Any() ? personIds.First() : (int?)null,
                        OtherPersonIds = personIds.Any() ? string.Join(",", personIds) : null,
                        IsViewPriorResponses = viewPrior
                    };
                    db.PrReviewNotificationSteps.Add(stepEntity);
                    db.SaveChanges();

                    // HR final step has no assigned criteria — they review prior approver answers only.
                    if (!string.Equals(role, "HR", StringComparison.OrdinalIgnoreCase))
                    {
                        var stepCrit = (st.CriteriaIds != null && st.CriteriaIds.Any()) ? st.CriteriaIds.Distinct().ToList() : new List<int>();
                        foreach (var cid in stepCrit)
                        {
                            if (usedCriteria.Contains(cid)) continue;
                            usedCriteria.Add(cid);
                            db.PrReviewReviewerCriterias.Add(new PrReviewReviewerCriteria
                            {
                                ReviewId = entity.ReviewId,
                                NotificationStepId = stepEntity.Id,
                                ReviewCriteriaId = cid
                            });
                        }
                    }
                }
                db.SaveChanges();

                var openTasks = db.PrReviewReviewerEmployees
                    .Where(t => t.ReviewId == entity.ReviewId && (t.Status == "Pending" || t.Status == "InProgress"))
                    .ToList();
                foreach (var t in openTasks)
                    SyncTaskStepAndCriteria(db, t);

                db.SaveChanges();
                return Json(new { success = true, reviewId = entity.ReviewId, employeeCount = empRows.Count });
            }
        }

        #endregion

        #region Start / Approvals

        [HttpPost]
        public JsonResult StartReview(int reviewId)
        {
            try
            {
                using (var db = OpenDb())
                {
                    var review = db.PrReviews.FirstOrDefault(r => r.ReviewId == reviewId);
                    if (review == null) return Json(new { success = false, message = "Review not found." });
                    if (review.Status == "Completed")
                        return Json(new { success = false, message = "Review already completed." });
                    if (string.Equals(review.Status, "InProgress", StringComparison.OrdinalIgnoreCase)
                        || db.PrReviewReviewerEmployees.Any(t => t.ReviewId == reviewId))
                        return Json(new { success = false, message = "This review has already been started. Track progress from Performance Dashboard or Approvals." });

                    var sched = db.PrReviewSchedules.FirstOrDefault(s => s.ReviewId == reviewId);
                    if (sched != null && sched.FromDate.HasValue && sched.FromDate.Value.Date > DateTime.Today)
                        return Json(new { success = false, message = "Custom start date is in the future (" + sched.FromDate.Value.ToString("yyyy-MM-dd") + ")." });

                    var employees = db.PrReviewEmployees.Where(e => e.ReviewId == reviewId).ToList();
                    if (!employees.Any())
                        return Json(new { success = false, message = "Add at least one employee to the review." });

                    var steps = db.PrReviewNotificationSteps.Where(s => s.ReviewId == reviewId).OrderBy(s => s.StepOrder).ToList();
                    if (!steps.Any())
                        return Json(new { success = false, message = "Configure approval steps first." });

                    var firstStep = steps.First();
                    int days = sched != null && sched.DaysToComplete > 0 ? sched.DaysToComplete : 14;
                    var due = DateTime.Today.AddDays(days);
                    int started = 0;
                    int missingApprover = 0;

                    foreach (var re in employees)
                    {
                        int? assignedPersonId = ResolveApproverPersonId(db, re.EmployeeId, firstStep);
                        if (!assignedPersonId.HasValue && !string.Equals(firstStep.ReviewerRole, "HR", StringComparison.OrdinalIgnoreCase))
                            missingApprover++;
                        var critIds = GetCriteriaIdsForStep(db, firstStep.Id);
                        var task = new PrReviewReviewerEmployee
                        {
                            ReviewId = reviewId,
                            ReviewEmployeeId = re.Id,
                            EmployeeId = re.EmployeeId,
                            NotificationStepId = firstStep.Id,
                            StepOrder = firstStep.StepOrder,
                            ReviewerRole = firstStep.ReviewerRole,
                            AssignedPersonId = assignedPersonId,
                            AssignedCriteriaIds = BuildCriteriaIdsCsv(critIds),
                            Status = "Pending",
                            DueDate = due,
                            CreatedDate = DateTime.Now
                        };
                        db.PrReviewReviewerEmployees.Add(task);
                        db.SaveChanges();
                        NotifyAssignee(db, task, review.ReviewName, "Performance review assigned", GetLoginUrl());
                        started++;
                    }

                    review.Status = "InProgress";
                    review.ModifiedBy = User.Identity.Name;
                    review.ModifiedDate = DateTime.Now;
                    db.SaveChanges();
                    var msg = "Started review for " + started + " employee(s). First approver notified.";
                    if (missingApprover > 0)
                        msg += " Warning: " + missingApprover + " employee(s) have no " + firstStep.ReviewerRole + " on their position (Report To / Manager 2 / Manager 3).";
                    return Json(new { success = true, message = msg });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Start failed: " + ex.GetBaseException().Message });
            }
        }

        [HttpGet]
        public JsonResult GetHrQueue()
        {
            using (var db = OpenDb())
            {
                int? personId = ResolvePersonId(db);
                var tasks = db.PrReviewReviewerEmployees
                    .Where(t => t.ReviewerRole == "HR" && (t.Status == "Pending" || t.Status == "InProgress"))
                    .OrderBy(t => t.DueDate)
                    .ToList();

                // If the step lists specific HR people, only those users see the task.
                var filtered = new List<PrReviewReviewerEmployee>();
                foreach (var t in tasks)
                {
                    var step = db.PrReviewNotificationSteps.FirstOrDefault(s => s.Id == t.NotificationStepId);
                    var hrPeople = step != null ? ParsePersonIdList(step.OtherPersonIds, step.OtherPersonId) : new List<int>();
                    if (!hrPeople.Any() || (personId.HasValue && hrPeople.Contains(personId.Value)))
                        filtered.Add(t);
                }

                return Json(new { success = true, data = MapTasks(db, filtered, true) }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetInProgressReviews()
        {
            using (var db = OpenDb())
            {
                var list = db.PrReviews.Where(r => r.Status == "Draft" || r.Status == "InProgress" || r.Status == "Ready")
                    .OrderByDescending(r => r.CreatedDate).ToList()
                    .Select(r => MapReview(db, r)).ToList();
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Helpers

        private ClientDbContext OpenDb()
        {
            string conn = User.Identity.GetClientConnectionString();
            var db = new ClientDbContext(conn);
            PerformanceModuleSchemaHelper.EnsureSchema(db);
            return db;
        }

        private void Ensure()
        {
            using (var db = OpenDb()) { }
        }

        private static List<int> ParseIntList(string csv)
        {
            var ids = new List<int>();
            if (string.IsNullOrWhiteSpace(csv)) return ids;
            foreach (var part in csv.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int id;
                if (int.TryParse(part.Trim(), out id) && id > 0 && !ids.Contains(id))
                    ids.Add(id);
            }
            return ids;
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

        private PrReviewVm MapReview(ClientDbContext db, PrReview r)
        {
            var sched = db.PrReviewSchedules.FirstOrDefault(s => s.ReviewId == r.ReviewId);
            var rule = db.PrReviewScoringRules.FirstOrDefault(s => s.ReviewId == r.ReviewId);
            var emps = db.PrReviewEmployees.Where(e => e.ReviewId == r.ReviewId).ToList();
            var personIds = emps.Where(e => e.PersonId.HasValue).Select(e => e.PersonId.Value).Distinct().ToList();
            var people = personIds.Any()
                ? db.Persons.Where(p => personIds.Contains(p.PersonId)).ToDictionary(p => p.PersonId, p => (p.Firstname + " " + p.Lastname).Trim())
                : new Dictionary<int, string>();
            var steps = db.PrReviewNotificationSteps.Where(s => s.ReviewId == r.ReviewId).OrderBy(s => s.StepOrder).ToList();
            var links = db.PrReviewReviewerCriterias.Where(c => c.ReviewId == r.ReviewId).ToList();
            var allStepPersonIds = steps.SelectMany(s => ParsePersonIdList(s.OtherPersonIds, s.OtherPersonId)).Distinct().ToList();
            var stepPeople = allStepPersonIds.Any()
                ? db.Persons.Where(p => allStepPersonIds.Contains(p.PersonId)).ToDictionary(p => p.PersonId, p => (p.Firstname + " " + p.Lastname).Trim())
                : new Dictionary<int, string>();

            return new PrReviewVm
            {
                ReviewId = r.ReviewId,
                ReviewName = r.ReviewName,
                RevieweeMode = r.RevieweeMode,
                Status = r.Status,
                HrOwner = r.HrOwner,
                Notes = r.Notes,
                IntervalType = sched != null ? sched.IntervalType : null,
                FromSchedule = sched != null ? sched.FromSchedule : null,
                FromDate = sched != null && sched.FromDate.HasValue ? sched.FromDate.Value.ToString("yyyy-MM-dd") : null,
                DaysToComplete = sched != null ? sched.DaysToComplete : 14,
                RepeatDays = sched != null ? sched.RepeatDays : 0,
                IsAverageOfAllQuestions = rule == null || rule.IsAverageOfAllQuestions,
                IsSumOfAllQuestions = rule != null && rule.IsSumOfAllQuestions,
                WeightedAverage = rule != null && rule.WeightedAverage,
                EmployeeIds = emps.Select(e => e.EmployeeId).ToList(),
                EmployeeNames = emps.Select(e => e.PersonId.HasValue && people.ContainsKey(e.PersonId.Value) ? people[e.PersonId.Value] : ("#" + e.EmployeeId)).ToList(),
                Steps = steps.Select(s =>
                {
                    var ids = ParsePersonIdList(s.OtherPersonIds, s.OtherPersonId);
                    return new PrStepVm
                    {
                        Id = s.Id,
                        StepOrder = s.StepOrder,
                        ReviewerRole = s.ReviewerRole,
                        OtherPersonId = s.OtherPersonId,
                        OtherPersonIds = ids,
                        OtherPersonNames = ids.Select(id => stepPeople.ContainsKey(id) ? stepPeople[id] : ("#" + id)).ToList(),
                        IsViewPriorResponses = s.IsViewPriorResponses,
                        CriteriaIds = string.Equals(s.ReviewerRole, "HR", StringComparison.OrdinalIgnoreCase)
                            ? new List<int>()
                            : links.Where(l => l.NotificationStepId == s.Id).Select(l => l.ReviewCriteriaId).ToList()
                    };
                }).ToList(),
                CriteriaIds = links.Select(l => l.ReviewCriteriaId).Distinct().ToList()
            };
        }

        internal static List<int> ParsePersonIdList(string csv, int? singleId)
        {
            var ids = new List<int>();
            if (!string.IsNullOrWhiteSpace(csv))
            {
                foreach (var part in csv.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    int id;
                    if (int.TryParse(part.Trim(), out id) && id > 0 && !ids.Contains(id))
                        ids.Add(id);
                }
            }
            if (singleId.HasValue && singleId.Value > 0 && !ids.Contains(singleId.Value))
                ids.Add(singleId.Value);
            return ids;
        }

        internal static List<PrTaskVm> MapTasks(ClientDbContext db, List<PrReviewReviewerEmployee> tasks, bool canEdit)
        {
            var reviewIds = tasks.Select(t => t.ReviewId).Distinct().ToList();
            var reviews = db.PrReviews.Where(r => reviewIds.Contains(r.ReviewId)).ToDictionary(r => r.ReviewId, r => r.ReviewName);
            var empIds = tasks.Select(t => t.EmployeeId).Distinct().ToList();
            var emps = db.Employees.Where(e => empIds.Contains(e.EmployeeId)).ToList();
            var personIds = emps.Select(e => e.PersonId).Distinct().ToList();
            var people = db.Persons.Where(p => personIds.Contains(p.PersonId)).ToDictionary(p => p.PersonId, p => (p.Firstname + " " + p.Lastname).Trim());

            return tasks.Select(t =>
            {
                var emp = emps.FirstOrDefault(e => e.EmployeeId == t.EmployeeId);
                string name = emp != null && people.ContainsKey(emp.PersonId) ? people[emp.PersonId] : ("#" + t.EmployeeId);
                return new PrTaskVm
                {
                    Id = t.Id,
                    ReviewId = t.ReviewId,
                    ReviewName = reviews.ContainsKey(t.ReviewId) ? reviews[t.ReviewId] : null,
                    EmployeeId = t.EmployeeId,
                    EmployeeName = name,
                    ReviewerRole = t.ReviewerRole,
                    StepOrder = t.StepOrder,
                    Status = t.Status,
                    Score = t.Score,
                    DueDate = t.DueDate.HasValue ? t.DueDate.Value.ToString("yyyy-MM-dd") : null,
                    Comments = t.Comments,
                    CanEdit = canEdit && (t.Status == "Pending" || t.Status == "InProgress")
                };
            }).ToList();
        }

        internal static int? ResolveApproverPersonId(ClientDbContext db, int employeeId, PrReviewNotificationStep step)
        {
            if (step == null) return null;
            var role = (step.ReviewerRole ?? "").Trim();
            var listed = ParsePersonIdList(step.OtherPersonIds, step.OtherPersonId);

            if (string.Equals(role, "Other", StringComparison.OrdinalIgnoreCase) || string.Equals(role, "Employee", StringComparison.OrdinalIgnoreCase))
                return listed.Any() ? listed.First() : (int?)null;

            // HR final: leave unassigned so any configured HR person (or HR queue) can open it.
            if (string.Equals(role, "HR", StringComparison.OrdinalIgnoreCase))
                return null;

            var pos = db.E_Positions.Where(p => p.EmployeeId == employeeId && (p.EndDate == null || p.EndDate > DateTime.Today))
                .OrderByDescending(p => p.PrimaryPosition == true)
                .ThenByDescending(p => p.StartDate)
                .FirstOrDefault();
            if (pos == null) return listed.Any() ? listed.First() : (int?)null;

            if (string.Equals(role, "Approver1", StringComparison.OrdinalIgnoreCase))
                return pos.ReportsToID ?? (listed.Any() ? listed.First() : (int?)null);
            if (string.Equals(role, "Approver2", StringComparison.OrdinalIgnoreCase))
                return pos.Manager2ID ?? (listed.Any() ? listed.First() : (int?)null);
            if (string.Equals(role, "Approver3", StringComparison.OrdinalIgnoreCase))
                return pos.Manager3ID ?? (listed.Any() ? listed.First() : (int?)null);
            return listed.Any() ? listed.First() : (int?)null;
        }

        internal static string BuildNotificationDescription(string reviewerRole)
        {
            if (string.IsNullOrWhiteSpace(reviewerRole)) return "Complete Performance Review";
            var role = reviewerRole.Trim();
            if (string.Equals(role, "Approver1", StringComparison.OrdinalIgnoreCase))
                return "Complete Performance Review (Manager review)";
            if (string.Equals(role, "Approver2", StringComparison.OrdinalIgnoreCase))
                return "Complete Performance Review (Manager 2 review)";
            if (string.Equals(role, "Approver3", StringComparison.OrdinalIgnoreCase))
                return "Complete Performance Review (Manager 3 review)";
            if (string.Equals(role, "HR", StringComparison.OrdinalIgnoreCase))
                return "Complete Performance Review (HR final review)";
            if (string.Equals(role, "Employee", StringComparison.OrdinalIgnoreCase))
                return "Complete Performance Review (Self review)";
            return "Complete Performance Review (" + PerformanceReviewEmailHelper.FormatApproverRoleLabel(role) + ")";
        }

        internal static bool RolesEqual(string a, string b)
        {
            return string.Equals((a ?? "").Trim(), (b ?? "").Trim(), StringComparison.OrdinalIgnoreCase);
        }

        internal static PrReviewNotificationStep ResolveNotificationStepForTask(ClientDbContext db, PrReviewReviewerEmployee task)
        {
            if (task == null) return null;

            var steps = db.PrReviewNotificationSteps
                .Where(s => s.ReviewId == task.ReviewId)
                .ToList();
            if (!steps.Any()) return null;

            var role = (task.ReviewerRole ?? "").Trim();
            PrReviewNotificationStep step = null;

            if (!string.IsNullOrEmpty(role))
            {
                step = steps
                    .Where(s => s.StepOrder == task.StepOrder && RolesEqual(s.ReviewerRole, role))
                    .OrderByDescending(s => s.Id)
                    .FirstOrDefault();
                if (step == null)
                {
                    step = steps
                        .Where(s => RolesEqual(s.ReviewerRole, role))
                        .OrderByDescending(s => s.StepOrder)
                        .ThenByDescending(s => s.Id)
                        .FirstOrDefault();
                }
            }
            if (step == null)
            {
                step = steps
                    .Where(s => s.StepOrder == task.StepOrder)
                    .OrderByDescending(s => s.Id)
                    .FirstOrDefault();
            }
            if (step == null && task.NotificationStepId > 0)
                step = steps.FirstOrDefault(s => s.Id == task.NotificationStepId);

            if (step != null && !string.IsNullOrEmpty(role) && !RolesEqual(step.ReviewerRole, role))
            {
                var byRole = steps
                    .Where(s => RolesEqual(s.ReviewerRole, role))
                    .OrderByDescending(s => s.StepOrder)
                    .ThenByDescending(s => s.Id)
                    .FirstOrDefault();
                if (byRole != null) step = byRole;
            }
            return step;
        }

        internal static List<int> GetCriteriaIdsForStep(ClientDbContext db, int notificationStepId)
        {
            return db.PrReviewReviewerCriterias
                .Where(c => c.NotificationStepId == notificationStepId)
                .Select(c => c.ReviewCriteriaId)
                .Distinct()
                .ToList();
        }

        internal static string BuildCriteriaIdsCsv(IEnumerable<int> ids)
        {
            if (ids == null) return null;
            var list = ids.Where(id => id > 0).Distinct().ToList();
            return list.Any() ? string.Join(",", list) : null;
        }

        internal static void SyncTaskStepAndCriteria(ClientDbContext db, PrReviewReviewerEmployee task)
        {
            if (task == null) return;
            var step = ResolveNotificationStepForTask(db, task);
            if (step == null) return;
            var ids = GetCriteriaIdsForStep(db, step.Id);
            task.NotificationStepId = step.Id;
            task.StepOrder = step.StepOrder;
            task.ReviewerRole = step.ReviewerRole;
            task.AssignedCriteriaIds = BuildCriteriaIdsCsv(ids);
        }

        internal static List<int> GetCriteriaIdsForTask(ClientDbContext db, PrReviewReviewerEmployee task)
        {
            if (task == null) return new List<int>();
            SyncTaskStepAndCriteria(db, task);
            return ParseIntList(task.AssignedCriteriaIds);
        }

        internal static bool IsSkippedStepRole(string reviewerRole)
        {
            if (string.IsNullOrWhiteSpace(reviewerRole)) return true;
            var role = reviewerRole.Trim();
            return string.Equals(role, "None", StringComparison.OrdinalIgnoreCase)
                || string.Equals(role, "(None)", StringComparison.OrdinalIgnoreCase)
                || string.Equals(role, "Skip", StringComparison.OrdinalIgnoreCase)
                || string.Equals(role, "(Skip)", StringComparison.OrdinalIgnoreCase);
        }

        internal static void HideNotificationsForTask(ClientDbContext db, int taskId, int? personId = null, string userName = null)
        {
            var rows = db.PrNotifications.Where(n => n.ReviewReviewerEmployeeId == taskId && !n.IsHidden).ToList();
            foreach (var n in rows)
            {
                if (personId.HasValue && n.PersonId.HasValue && n.PersonId != personId) continue;
                if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(n.UserName)
                    && !string.Equals(n.UserName, userName, StringComparison.OrdinalIgnoreCase)) continue;
                n.IsHidden = true;
                n.IsRead = true;
            }
        }

        internal static void NotifyAssignee(ClientDbContext db, PrReviewReviewerEmployee task, string reviewName, string eventName, string loginUrl = null)
        {
            string roleLabel = PerformanceReviewEmailHelper.FormatApproverRoleLabel(task.ReviewerRole);
            string contents = BuildNotificationDescription(task.ReviewerRole);
            var step = db.PrReviewNotificationSteps.FirstOrDefault(s => s.Id == task.NotificationStepId);
            var notifyPersonIds = new List<int>();
            if (task.AssignedPersonId.HasValue)
                notifyPersonIds.Add(task.AssignedPersonId.Value);
            else if (step != null && string.Equals(step.ReviewerRole, "HR", StringComparison.OrdinalIgnoreCase))
                notifyPersonIds.AddRange(ParsePersonIdList(step.OtherPersonIds, step.OtherPersonId));

            string employeeName = GetEmployeeDisplayName(db, task.EmployeeId);
            if (string.IsNullOrWhiteSpace(loginUrl))
                loginUrl = "#";

            if (!notifyPersonIds.Any())
            {
                db.PrNotifications.Add(new PrNotification
                {
                    PersonId = null,
                    UserName = null,
                    ReviewId = task.ReviewId,
                    ReviewReviewerEmployeeId = task.Id,
                    EventName = eventName,
                    Contents = contents + " (open HR Approvals queue)",
                    ReceivedOn = DateTime.Now,
                    IsRead = false,
                    IsHidden = false
                });
                db.SaveChanges();
                return;
            }

            foreach (var pid in notifyPersonIds.Distinct())
            {
                string email = null;
                string userName = null;
                string recipientName = null;
                var person = db.Persons.FirstOrDefault(p => p.PersonId == pid);
                if (person != null)
                {
                    recipientName = (person.Firstname + " " + person.Lastname).Trim();
                    email = person.eMail ?? person.AlternateEMail;
                    var link = db.UserNamesPersons.FirstOrDefault(u => u.PersonID == person.PersonId);
                    if (link != null) userName = link.UserName;
                }

                HideNotificationsForTask(db, task.Id, pid, userName);

                db.PrNotifications.Add(new PrNotification
                {
                    PersonId = pid,
                    UserName = userName,
                    ReviewId = task.ReviewId,
                    ReviewReviewerEmployeeId = task.Id,
                    EventName = eventName,
                    Contents = contents,
                    ReceivedOn = DateTime.Now,
                    IsRead = false,
                    IsHidden = false
                });

                if (!string.IsNullOrWhiteSpace(email))
                {
                    string approverName = recipientName ?? roleLabel;
                    string html = PerformanceReviewEmailHelper.BuildAssignmentEmail(
                        recipientName ?? "Colleague",
                        employeeName,
                        roleLabel,
                        approverName,
                        reviewName,
                        loginUrl);
                    MailSettingsEmailHelper.TrySend(email, PerformanceReviewEmailHelper.AssignmentSubject(reviewName), html);
                }
            }
            db.SaveChanges();
        }

        internal static void EnrichTaskDisplay(ClientDbContext db, PrTaskVm vm, PrReviewReviewerEmployee task, int? currentPersonId = null)
        {
            if (vm == null || task == null) return;
            var review = db.PrReviews.FirstOrDefault(r => r.ReviewId == task.ReviewId);
            vm.ReviewDate = (task.DueDate ?? task.CreatedDate).ToString("MM/dd/yyyy");
            vm.ReviewStatusLabel = FormatReviewDisplayStatus(review != null ? review.Status : null, task.Status);
            vm.ReviewerName = ResolveReviewerPersonName(db, task, currentPersonId);
            vm.ReviewerLabel = FormatReviewerDisplayLabel(task.ReviewerRole);
            if (!string.IsNullOrWhiteSpace(vm.ReviewerName))
                vm.ReviewerLabel = vm.ReviewerLabel + " — " + vm.ReviewerName;
            vm.ManagerName = GetManagerNameForEmployee(db, task.EmployeeId);
        }

        internal static string ResolveReviewerPersonName(ClientDbContext db, PrReviewReviewerEmployee task, int? currentPersonId = null)
        {
            if (task == null) return null;

            if (task.AssignedPersonId.HasValue)
            {
                var assigned = GetPersonDisplayName(db, task.AssignedPersonId);
                if (!string.IsNullOrWhiteSpace(assigned)) return assigned;
            }

            var fromPosition = GetRoleManagerNameForEmployee(db, task.EmployeeId, task.ReviewerRole);
            if (!string.IsNullOrWhiteSpace(fromPosition)) return fromPosition;

            if (currentPersonId.HasValue
                && task.AssignedPersonId.HasValue
                && task.AssignedPersonId.Value == currentPersonId.Value)
            {
                var current = GetPersonDisplayName(db, currentPersonId);
                if (!string.IsNullOrWhiteSpace(current)) return current;
            }

            var step = ResolveNotificationStepForTask(db, task);
            var personId = ResolveApproverPersonId(db, task.EmployeeId, step);
            return GetPersonDisplayName(db, personId);
        }

        internal static string GetRoleManagerNameForEmployee(ClientDbContext db, int employeeId, string reviewerRole)
        {
            var pos = db.E_Positions
                .Where(p => p.EmployeeId == employeeId && (p.EndDate == null || p.EndDate > DateTime.Today))
                .OrderByDescending(p => p.PrimaryPosition == true)
                .ThenByDescending(p => p.StartDate)
                .FirstOrDefault();
            if (pos == null) return null;

            int? personId = null;
            var role = (reviewerRole ?? "").Trim();
            if (string.Equals(role, "Approver1", StringComparison.OrdinalIgnoreCase))
                personId = pos.ReportsToID;
            else if (string.Equals(role, "Approver2", StringComparison.OrdinalIgnoreCase))
                personId = pos.Manager2ID;
            else if (string.Equals(role, "Approver3", StringComparison.OrdinalIgnoreCase))
                personId = pos.Manager3ID;

            return GetPersonDisplayName(db, personId);
        }

        internal static string GetPersonDisplayName(ClientDbContext db, int? personId)
        {
            if (!personId.HasValue) return null;
            var person = db.Persons.FirstOrDefault(p => p.PersonId == personId.Value);
            return person != null ? (person.Firstname + " " + person.Lastname).Trim() : null;
        }

        internal static string FormatReviewDisplayStatus(string reviewStatus, string taskStatus)
        {
            if (string.Equals(taskStatus, "Submitted", StringComparison.OrdinalIgnoreCase)) return "Submitted";
            if (string.Equals(reviewStatus, "InProgress", StringComparison.OrdinalIgnoreCase)) return "Review Launched";
            if (string.Equals(reviewStatus, "Draft", StringComparison.OrdinalIgnoreCase)) return "Draft";
            if (string.Equals(reviewStatus, "Completed", StringComparison.OrdinalIgnoreCase)) return "Completed";
            if (!string.IsNullOrWhiteSpace(taskStatus)) return taskStatus;
            return !string.IsNullOrWhiteSpace(reviewStatus) ? reviewStatus : "In Progress";
        }

        internal static string FormatReviewerDisplayLabel(string reviewerRole)
        {
            if (string.IsNullOrWhiteSpace(reviewerRole)) return "REVIEWER";
            var role = reviewerRole.Trim();
            if (string.Equals(role, "Employee", StringComparison.OrdinalIgnoreCase)) return "SELF";
            if (string.Equals(role, "Approver1", StringComparison.OrdinalIgnoreCase)) return "MANAGER";
            if (string.Equals(role, "Approver2", StringComparison.OrdinalIgnoreCase)) return "MANAGER 2";
            if (string.Equals(role, "Approver3", StringComparison.OrdinalIgnoreCase)) return "MANAGER 3";
            if (string.Equals(role, "HR", StringComparison.OrdinalIgnoreCase)) return "HR";
            return role.ToUpperInvariant();
        }

        internal static string GetManagerNameForEmployee(ClientDbContext db, int employeeId)
        {
            var pos = db.E_Positions
                .Where(p => p.EmployeeId == employeeId && (p.EndDate == null || p.EndDate > DateTime.Today))
                .OrderByDescending(p => p.PrimaryPosition == true)
                .ThenByDescending(p => p.StartDate)
                .FirstOrDefault();
            if (pos == null || !pos.ReportsToID.HasValue) return "—";
            var mgr = db.Persons.FirstOrDefault(p => p.PersonId == pos.ReportsToID.Value);
            return mgr != null ? (mgr.Firstname + " " + mgr.Lastname).Trim() : "—";
        }

        internal static string GetEmployeeDisplayName(ClientDbContext db, int employeeId)
        {
            var emp = db.Employees.FirstOrDefault(e => e.EmployeeId == employeeId);
            if (emp == null) return "#" + employeeId;
            var person = db.Persons.FirstOrDefault(p => p.PersonId == emp.PersonId);
            if (person == null) return "#" + employeeId;
            return (person.Firstname + " " + person.Lastname).Trim();
        }

        private string GetLoginUrl()
        {
            try
            {
                return Url.Action("Login", "Account", null, Request.Url.Scheme);
            }
            catch
            {
                return ConfigurationManager.AppSettings["ApplicationBaseUrl"] ?? "/Account/Login";
            }
        }

        private int? ResolvePersonId(ClientDbContext db)
        {
            string userName = User.Identity.Name ?? "";
            var byUser = db.UserNamesPersons.FirstOrDefault(u => u.UserName == userName);
            if (byUser != null) return byUser.PersonID;
            var asp = db.AspNetUsers.FirstOrDefault(u => u.UserName == userName);
            string email = asp != null ? asp.Email : userName;
            var person = db.Persons.FirstOrDefault(p => p.eMail == email || p.eMail == userName);
            return person != null ? person.PersonId : (int?)null;
        }

        #endregion
    }
}
