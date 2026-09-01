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
    public class PerformanceReviewController : Controller
    {
        public PartialViewResult MyReviewsPartial()
        {
            string conn = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(conn))
                PerformanceModuleSchemaHelper.EnsureSchema(db);
            return PartialView();
        }

        public PartialViewResult TasksPartial()
        {
            string conn = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(conn))
                PerformanceModuleSchemaHelper.EnsureSchema(db);
            return PartialView();
        }

        public PartialViewResult CompletedReviewsPartial()
        {
            string requestType = User.Identity.GetRequestType();
            if (requestType != "IsSelfService")
                SessionStateHelper.CheckForPersonSelectedValue();
            return CompletedReviewsView(isSelfService: false);
        }

        public PartialViewResult MyCompletedReviewsPartial()
        {
            return CompletedReviewsView(isSelfService: true);
        }

        private PartialViewResult CompletedReviewsView(bool isSelfService)
        {
            string conn = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(conn))
            {
                PerformanceModuleSchemaHelper.EnsureSchema(db);
                int? employeeId = ResolveTargetEmployeeId(db, isSelfService);
                ViewBag.IsSelfService = isSelfService;
                ViewBag.EmployeeId = employeeId;
                if (!employeeId.HasValue)
                {
                    ViewBag.IdentityMessage = isSelfService
                        ? "No employee record is linked to this login."
                        : "Select an employee on Personal Profile first, then open Performance.";
                }
            }
            return PartialView("CompletedReviewsPartial");
        }

        [HttpGet]
        public JsonResult GetCompletedReviews(int? employeeId)
        {
            using (var db = OpenDb())
            {
                int? targetId = employeeId;
                if (!targetId.HasValue)
                    targetId = ResolveTargetEmployeeId(db, User.Identity.GetRequestType() == "IsSelfService");
                if (!targetId.HasValue)
                    return Json(new { success = false, message = "No employee selected." }, JsonRequestBehavior.AllowGet);

                var rows = db.PrReviewEmployees
                    .Where(re => re.EmployeeId == targetId.Value)
                    .OrderByDescending(re => re.CreatedDate)
                    .ToList();
                if (!rows.Any())
                    return Json(new { success = true, data = new List<PrEmployeeCompletedReviewVm>() }, JsonRequestBehavior.AllowGet);

                var reviewIds = rows.Select(r => r.ReviewId).Distinct().ToList();
                var reviews = db.PrReviews.Where(r => reviewIds.Contains(r.ReviewId))
                    .ToDictionary(r => r.ReviewId, r => r);

                var list = rows.Select(re =>
                {
                    PrReview review;
                    reviews.TryGetValue(re.ReviewId, out review);
                    var steps = db.PrReviewReviewerEmployees
                        .Where(t => t.ReviewEmployeeId == re.Id)
                        .OrderBy(t => t.StepOrder)
                        .ToList();
                    var submitted = steps.Where(s => s.Status == "Submitted").ToList();
                    double? finalScore = ResolveFinalScoreFromApprovers(db, steps);
                    DateTime? completed = submitted.Any()
                        ? submitted.Max(s => s.SubmittedDate)
                        : (DateTime?)null;
                    string status = review != null && string.Equals(review.Status, "Completed", StringComparison.OrdinalIgnoreCase)
                        ? "Complete"
                        : steps.Any(s => s.Status == "Pending" || s.Status == "InProgress")
                            ? "In Progress"
                            : (submitted.Any() ? "In Progress" : "Not Started");

                    var stepVms = steps.Select(s => new PrEmployeeCompletedStepVm
                    {
                        ReviewerRole = s.ReviewerRole,
                        ReviewerLabel = PerformanceAdminController.FormatReviewerDisplayLabel(s.ReviewerRole),
                        ReviewerName = PerformanceAdminController.ResolveReviewerPersonName(db, s),
                        Status = s.Status,
                        SubmittedDate = s.SubmittedDate.HasValue ? s.SubmittedDate.Value.ToString("MM/dd/yyyy") : null,
                        Score = s.Score,
                        Comments = s.Comments
                    }).ToList();

                    var reviewerNames = stepVms
                        .Where(s => s.Status == "Submitted" && !string.IsNullOrWhiteSpace(s.ReviewerName))
                        .Select(s => s.ReviewerName)
                        .Distinct()
                        .ToList();

                    return new PrEmployeeCompletedReviewVm
                    {
                        ReviewEmployeeId = re.Id,
                        ReviewId = re.ReviewId,
                        ReviewName = review != null ? review.ReviewName : ("Review #" + re.ReviewId),
                        ReviewType = review != null && !string.IsNullOrWhiteSpace(review.RevieweeMode) ? review.RevieweeMode : "Performance Review",
                        InitiatedDate = re.CreatedDate.ToString("MM/dd/yyyy"),
                        ReviewerSummary = reviewerNames.Any() ? string.Join(", ", reviewerNames) : "—",
                        CompletionDate = completed.HasValue ? completed.Value.ToString("MM/dd/yyyy") : "—",
                        Status = status,
                        Score = finalScore,
                        Steps = stepVms
                    };
                }).ToList();

                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetCompletedReviewDetail(int reviewEmployeeId)
        {
            using (var db = OpenDb())
            {
                var re = db.PrReviewEmployees.FirstOrDefault(x => x.Id == reviewEmployeeId);
                if (re == null) return Json(new { success = false, message = "Review not found." }, JsonRequestBehavior.AllowGet);

                int? targetId = ResolveTargetEmployeeId(db, User.Identity.GetRequestType() == "IsSelfService");
                if (!targetId.HasValue || re.EmployeeId != targetId.Value)
                    return Json(new { success = false, message = "Not authorized." }, JsonRequestBehavior.AllowGet);

                var review = db.PrReviews.FirstOrDefault(r => r.ReviewId == re.ReviewId);
                var emp = db.Employees.FirstOrDefault(e => e.EmployeeId == re.EmployeeId);
                string empName = PerformanceAdminController.GetEmployeeDisplayName(db, re.EmployeeId);

                var stepTasks = db.PrReviewReviewerEmployees
                    .Where(t => t.ReviewEmployeeId == reviewEmployeeId
                        && (t.Status == "Submitted" || t.Status == "OnHold"
                            || t.Status == "InProgress" || t.Status == "Pending"))
                    .OrderBy(t => t.StepOrder)
                    .ToList()
                    .Where(t => t.Status == "Submitted" || t.Status == "OnHold"
                        || db.PrReviewScoreDetails.Any(d => d.ReviewReviewerEmployeeId == t.Id)
                        || !string.IsNullOrWhiteSpace(t.Comments))
                    .ToList();

                var priorSteps = stepTasks.Select(t => MapTaskToPriorStep(db, t)).ToList();

                var vm = new PrEmployeeCompletedReviewDetailVm
                {
                    ReviewEmployeeId = re.Id,
                    ReviewName = review != null ? review.ReviewName : ("Review #" + re.ReviewId),
                    EmployeeName = empName,
                    ManagerName = PerformanceAdminController.GetManagerNameForEmployee(db, re.EmployeeId),
                    ReviewDate = re.CreatedDate.ToString("MM/dd/yyyy"),
                    Status = review != null ? review.Status : "In Progress",
                    FinalScore = ResolveFinalScoreFromApprovers(db, stepTasks),
                    Steps = priorSteps
                };
                return Json(new { success = true, data = vm }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetNotificationCount()
        {
            using (var db = OpenDb())
            {
                int? personId = ResolvePersonId(db);
                int count = GetOpenTasksForUser(db, personId).Count;
                return Json(new { success = true, count }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetNotifications()
        {
            using (var db = OpenDb())
            {
                var list = MapNotifications(db, QueryUserNotifications(db));
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult DismissNotification(int id)
        {
            using (var db = OpenDb())
            {
                int? personId = ResolvePersonId(db);
                string userName = User.Identity.Name ?? "";
                var n = db.PrNotifications.FirstOrDefault(x => x.Id == id);
                if (n == null) return Json(new { success = false, message = "Not found." });
                if (n.PersonId.HasValue && personId.HasValue && n.PersonId != personId)
                    return Json(new { success = false, message = "Not authorized." });
                if (!string.IsNullOrWhiteSpace(n.UserName) && !string.Equals(n.UserName, userName, StringComparison.OrdinalIgnoreCase)
                    && (!personId.HasValue || n.PersonId != personId))
                    return Json(new { success = false, message = "Not authorized." });
                n.IsHidden = true;
                n.IsRead = true;
                db.SaveChanges();
                return Json(new { success = true });
            }
        }

        [HttpPost]
        public JsonResult MarkNotificationRead(int id)
        {
            using (var db = OpenDb())
            {
                int? personId = ResolvePersonId(db);
                var n = db.PrNotifications.FirstOrDefault(x => x.Id == id);
                if (n == null) return Json(new { success = false });
                if (personId.HasValue && n.PersonId.HasValue && n.PersonId != personId)
                    return Json(new { success = false, message = "Not authorized." });
                n.IsRead = true;
                db.SaveChanges();
                return Json(new { success = true });
            }
        }

        [HttpGet]
        public JsonResult GetMyTasks()
        {
            using (var db = OpenDb())
            {
                int? personId = ResolvePersonId(db);
                if (!personId.HasValue)
                    return Json(new { success = true, data = new object[0], message = "No person linked to this login." }, JsonRequestBehavior.AllowGet);

                var tasks = db.PrReviewReviewerEmployees
                    .Where(t => (t.Status == "Pending" || t.Status == "InProgress") && t.AssignedPersonId == personId.Value)
                    .OrderBy(t => t.DueDate)
                    .ToList();

                // HR final steps are often unassigned; include those where this user is listed.
                var hrPending = db.PrReviewReviewerEmployees
                    .Where(t => t.ReviewerRole == "HR" && (t.Status == "Pending" || t.Status == "InProgress")
                        && (t.AssignedPersonId == null || t.AssignedPersonId == personId.Value))
                    .ToList();
                foreach (var t in hrPending)
                {
                    if (tasks.Any(x => x.Id == t.Id)) continue;
                    var step = db.PrReviewNotificationSteps.FirstOrDefault(s => s.Id == t.NotificationStepId);
                    var hrPeople = step != null ? PerformanceAdminController.ParsePersonIdList(step.OtherPersonIds, step.OtherPersonId) : new List<int>();
                    if (!hrPeople.Any() || hrPeople.Contains(personId.Value))
                        tasks.Add(t);
                }
                tasks = tasks.OrderBy(t => t.DueDate).ToList();

                return Json(new { success = true, data = PerformanceAdminController.MapTasks(db, tasks, true), personId }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetTask(int id)
        {
            using (var db = OpenDb())
            {
                int? personId = ResolvePersonId(db);
                var task = db.PrReviewReviewerEmployees.FirstOrDefault(t => t.Id == id);
                if (task == null) return Json(new { success = false, message = "Task not found." }, JsonRequestBehavior.AllowGet);

                bool isHr = string.Equals(task.ReviewerRole, "HR", StringComparison.OrdinalIgnoreCase);
                PerformanceAdminController.SyncTaskStepAndCriteria(db, task);
                var step = PerformanceAdminController.ResolveNotificationStepForTask(db, task);
                var hrPeople = step != null ? PerformanceAdminController.ParsePersonIdList(step.OtherPersonIds, step.OtherPersonId) : new List<int>();
                bool isAssignee = personId.HasValue && task.AssignedPersonId == personId.Value;
                bool isListedHr = isHr && personId.HasValue && hrPeople.Contains(personId.Value);
                // Open HR queue: any listed HR (or anyone on Approvals if no HR people configured).
                bool isOpenHrQueue = isHr && (task.Status == "Pending" || task.Status == "InProgress")
                    && (!hrPeople.Any() || isListedHr);

                if (!isAssignee && !isOpenHrQueue)
                    return Json(new { success = false, message = "Not authorized for this task." }, JsonRequestBehavior.AllowGet);

                db.SaveChanges();

                var vm = PerformanceAdminController.MapTasks(db, new List<PrReviewReviewerEmployee> { task }, true).First();
                PerformanceAdminController.EnrichTaskDisplay(db, vm, task, personId);
                vm.IsHrFinal = isHr;
                if (isHr)
                {
                    vm.Criteria = new List<PrTaskCriteriaVm>();
                    vm.PriorSteps = LoadPriorSteps(db, task);
                    vm.CollapsePriorSteps = false;
                }
                else
                {
                    vm.Criteria = LoadCriteriaForTask(db, task);
                    vm.PriorSteps = step != null && step.IsViewPriorResponses ? LoadPriorSteps(db, task) : new List<PrPriorStepVm>();
                    vm.CollapsePriorSteps = vm.PriorSteps != null && vm.PriorSteps.Any();
                }
                vm.ReworkReason = task.ReworkReason;
                vm.ReworkComments = task.ReworkComments;
                vm.RejectTargets = BuildRejectTargets(db, task, isHr);
                vm.CanReject = vm.CanEdit && vm.RejectTargets != null && vm.RejectTargets.Any();
                return Json(new { success = true, data = vm }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult RejectTask(int id, string targetRole, string comments, string reason)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(targetRole))
                    return Json(new { success = false, message = "Select who to send the review back to." });
                if (string.IsNullOrWhiteSpace(reason))
                    return Json(new { success = false, message = "Reason for rejection is required." });

                using (var db = OpenDb())
                {
                    int? personId = ResolvePersonId(db);
                    var task = db.PrReviewReviewerEmployees.FirstOrDefault(t => t.Id == id);
                    if (task == null) return Json(new { success = false, message = "Task not found." });
                    if (task.Status != "Pending" && task.Status != "InProgress")
                        return Json(new { success = false, message = "Task is locked." });

                    bool isHr = string.Equals(task.ReviewerRole, "HR", StringComparison.OrdinalIgnoreCase);
                    bool isApprover2 = string.Equals(task.ReviewerRole, "Approver2", StringComparison.OrdinalIgnoreCase);
                    if (!isHr && !isApprover2)
                        return Json(new { success = false, message = "Reject is not available for this step." });

                    var step = PerformanceAdminController.ResolveNotificationStepForTask(db, task);
                    var hrPeople = step != null ? PerformanceAdminController.ParsePersonIdList(step.OtherPersonIds, step.OtherPersonId) : new List<int>();
                    bool isAssignee = personId.HasValue && task.AssignedPersonId == personId.Value;
                    bool isOpenHrQueue = isHr && (!hrPeople.Any() || (personId.HasValue && hrPeople.Contains(personId.Value)));
                    if (!isAssignee && !isOpenHrQueue)
                        return Json(new { success = false, message = "Not authorized." });

                    var targets = BuildRejectTargets(db, task, isHr);
                    if (!targets.Any(t => PerformanceAdminController.RolesEqual(t.Value, targetRole)
                        || string.Equals(t.Value, targetRole, StringComparison.OrdinalIgnoreCase)))
                        return Json(new { success = false, message = "Invalid reject target." });

                    ProcessRejection(db, task, targetRole.Trim(), comments, reason);
                    db.SaveChanges();

                    string loginUrl = null;
                    try { loginUrl = Url.Action("Login", "Account", null, Request.Url.Scheme); }
                    catch { loginUrl = ConfigurationManager.AppSettings["ApplicationBaseUrl"] ?? "/Account/Login"; }

                    var review = db.PrReviews.FirstOrDefault(r => r.ReviewId == task.ReviewId);
                    string reviewName = review != null ? review.ReviewName : "Performance Review";
                    NotifyReworkAssignees(db, task, targetRole.Trim(), reviewName, loginUrl);

                    var notifs = db.PrNotifications.Where(n => n.ReviewReviewerEmployeeId == task.Id && !n.IsHidden).ToList();
                    foreach (var n in notifs) { n.IsHidden = true; n.IsRead = true; }
                    db.SaveChanges();

                    return Json(new { success = true, message = "Review sent back for rework. The selected approver has been notified." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Reject failed: " + ex.GetBaseException().Message });
            }
        }

        private static List<PrRejectTargetVm> BuildRejectTargets(ClientDbContext db, PrReviewReviewerEmployee task, bool isHr)
        {
            var targets = new List<PrRejectTargetVm>();
            if (task == null) return targets;

            bool hasA1 = HasSubmittedPriorStep(db, task, "Approver1");
            bool hasA2 = HasSubmittedPriorStep(db, task, "Approver2");

            if (isHr)
            {
                if (hasA1) targets.Add(new PrRejectTargetVm { Value = "Approver1", Label = "Approver 1 (Manager)" });
                if (hasA2) targets.Add(new PrRejectTargetVm { Value = "Approver2", Label = "Approver 2 (Manager 2)" });
                if (hasA1 && hasA2) targets.Add(new PrRejectTargetVm { Value = "Both", Label = "Both Approvers" });
            }
            else if (string.Equals(task.ReviewerRole, "Approver2", StringComparison.OrdinalIgnoreCase) && hasA1)
            {
                targets.Add(new PrRejectTargetVm { Value = "Approver1", Label = "Approver 1 (Manager)" });
            }
            return targets;
        }

        private static bool HasSubmittedPriorStep(ClientDbContext db, PrReviewReviewerEmployee current, string role)
        {
            return db.PrReviewReviewerEmployees
                .Where(t => t.ReviewEmployeeId == current.ReviewEmployeeId
                    && t.StepOrder < current.StepOrder
                    && t.Status == "Submitted")
                .ToList()
                .Any(t => PerformanceAdminController.RolesEqual(t.ReviewerRole, role));
        }

        private static PrReviewReviewerEmployee FindStepTask(ClientDbContext db, int reviewEmployeeId, string role)
        {
            return db.PrReviewReviewerEmployees
                .Where(t => t.ReviewEmployeeId == reviewEmployeeId)
                .OrderByDescending(t => t.Id)
                .ToList()
                .FirstOrDefault(t => PerformanceAdminController.RolesEqual(t.ReviewerRole, role));
        }

        private void ProcessRejection(ClientDbContext db, PrReviewReviewerEmployee rejectingTask, string targetRole, string comments, string reason)
        {
            rejectingTask.RejectionReason = reason;
            rejectingTask.RejectionComments = comments;
            rejectingTask.Status = "OnHold";

            if (string.Equals(targetRole, "Both", StringComparison.OrdinalIgnoreCase)
                || string.Equals(targetRole, "Approver1", StringComparison.OrdinalIgnoreCase))
            {
                var a1 = FindStepTask(db, rejectingTask.ReviewEmployeeId, "Approver1");
                if (a1 != null)
                {
                    a1.Status = "InProgress";
                    a1.SubmittedDate = null;
                    a1.ReworkReason = reason;
                    a1.ReworkComments = comments;
                }
                PauseDownstreamExcept(db, rejectingTask.ReviewEmployeeId, rejectingTask.Id, "Approver1");
            }
            else if (string.Equals(targetRole, "Approver2", StringComparison.OrdinalIgnoreCase))
            {
                var a2 = FindStepTask(db, rejectingTask.ReviewEmployeeId, "Approver2");
                if (a2 != null)
                {
                    a2.Status = "InProgress";
                    a2.SubmittedDate = null;
                    a2.ReworkReason = reason;
                    a2.ReworkComments = comments;
                }
            }
        }

        private static void PauseDownstreamExcept(ClientDbContext db, int reviewEmployeeId, int rejectingTaskId, string reopenedRole)
        {
            var reopened = FindStepTask(db, reviewEmployeeId, reopenedRole);
            int minStep = reopened != null ? reopened.StepOrder : 0;
            var downstream = db.PrReviewReviewerEmployees
                .Where(t => t.ReviewEmployeeId == reviewEmployeeId && t.StepOrder > minStep && t.Id != rejectingTaskId)
                .ToList();
            foreach (var t in downstream)
            {
                if (t.Status == "Submitted" || t.Status == "Pending" || t.Status == "InProgress")
                    t.Status = "OnHold";
            }
        }

        private void NotifyReworkAssignees(ClientDbContext db, PrReviewReviewerEmployee rejectingTask, string targetRole, string reviewName, string loginUrl)
        {
            var roles = new List<string>();
            if (string.Equals(targetRole, "Both", StringComparison.OrdinalIgnoreCase))
                roles.Add("Approver1");
            else
                roles.Add(targetRole);

            foreach (var role in roles)
            {
                var reopened = FindStepTask(db, rejectingTask.ReviewEmployeeId, role);
                if (reopened == null || !reopened.AssignedPersonId.HasValue) continue;
                PerformanceAdminController.NotifyAssignee(db, reopened, reviewName,
                    "Performance review rework required", loginUrl);
            }
        }

        [HttpPost]
        public JsonResult SaveTask(int id, string comments, string answersJson, bool submit)
        {
            try
            {
                List<PrAnswerItemVm> answers = null;
                if (!string.IsNullOrWhiteSpace(answersJson))
                {
                    try { answers = new JavaScriptSerializer().Deserialize<List<PrAnswerItemVm>>(answersJson); }
                    catch { return Json(new { success = false, message = "Invalid answers." }); }
                }

                using (var db = OpenDb())
                {
                    int? personId = ResolvePersonId(db);
                    var task = db.PrReviewReviewerEmployees.FirstOrDefault(t => t.Id == id);
                    if (task == null) return Json(new { success = false, message = "Task not found." });
                    if (task.Status != "Pending" && task.Status != "InProgress")
                        return Json(new { success = false, message = "Task is locked." });

                    bool isHr = string.Equals(task.ReviewerRole, "HR", StringComparison.OrdinalIgnoreCase);
                    var step = PerformanceAdminController.ResolveNotificationStepForTask(db, task);
                    if (step != null && step.Id != task.NotificationStepId)
                    {
                        task.NotificationStepId = step.Id;
                        db.SaveChanges();
                    }
                    var hrPeople = step != null ? PerformanceAdminController.ParsePersonIdList(step.OtherPersonIds, step.OtherPersonId) : new List<int>();
                    bool isAssignee = personId.HasValue && task.AssignedPersonId == personId.Value;
                    bool isListedHr = isHr && personId.HasValue && hrPeople.Contains(personId.Value);
                    bool isOpenHrQueue = isHr && (!hrPeople.Any() || isListedHr);
                    if (!isAssignee && !isOpenHrQueue)
                        return Json(new { success = false, message = "Not authorized." });

                    task.Comments = comments;
                    task.Status = "InProgress";
                    if (isHr && personId.HasValue && !task.AssignedPersonId.HasValue)
                        task.AssignedPersonId = personId; // claim for audit

                    if (!isHr)
                    {
                        PerformanceAdminController.SyncTaskStepAndCriteria(db, task);
                        var allowedCriteria = new HashSet<int>(PerformanceAdminController.GetCriteriaIdsForTask(db, task));
                        var existing = db.PrReviewScoreDetails.Where(d => d.ReviewReviewerEmployeeId == task.Id).ToList();
                        foreach (var e in existing) db.PrReviewScoreDetails.Remove(e);
                        if (answers != null)
                        {
                            foreach (var a in answers.Where(x => x.ReviewCriteriaId > 0 && allowedCriteria.Contains(x.ReviewCriteriaId)))
                            {
                                db.PrReviewScoreDetails.Add(new PrReviewScoreDetail
                                {
                                    ReviewReviewerEmployeeId = task.Id,
                                    ReviewCriteriaId = a.ReviewCriteriaId,
                                    Answer = a.Answer,
                                    Comments = a.Comments
                                });
                            }
                        }
                    }
                    db.SaveChanges();

                    if (!submit)
                        return Json(new { success = true, message = "Saved." });

                    if (!isHr)
                        task.Score = CalculateScore(db, task);
                    task.Status = "Submitted";
                    task.SubmittedDate = DateTime.Now;
                    task.ReworkReason = null;
                    task.ReworkComments = null;
                    db.SaveChanges();

                    var notifs = db.PrNotifications.Where(n => n.ReviewReviewerEmployeeId == task.Id && !n.IsHidden).ToList();
                    foreach (var n in notifs) { n.IsHidden = true; n.IsRead = true; }
                    db.SaveChanges();

                    AdvanceWorkflow(db, task);
                    return Json(new { success = true, message = isHr ? "HR final approval submitted." : ("Submitted. Score: " + (task.Score.HasValue ? task.Score.Value.ToString("0.##") : "n/a")) });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Save failed: " + ex.GetBaseException().Message });
            }
        }

        private List<PrPriorStepVm> LoadPriorSteps(ClientDbContext db, PrReviewReviewerEmployee current)
        {
            var prior = db.PrReviewReviewerEmployees
                .Where(t => t.ReviewEmployeeId == current.ReviewEmployeeId && t.StepOrder < current.StepOrder && t.Status == "Submitted")
                .OrderBy(t => t.StepOrder)
                .ToList();
            if (!prior.Any()) return new List<PrPriorStepVm>();

            var assigneeIds = prior.Where(t => t.AssignedPersonId.HasValue).Select(t => t.AssignedPersonId.Value).Distinct().ToList();
            var people = assigneeIds.Any()
                ? db.Persons.Where(p => assigneeIds.Contains(p.PersonId)).ToDictionary(p => p.PersonId, p => (p.Firstname + " " + p.Lastname).Trim())
                : new Dictionary<int, string>();
            var scale = db.PrReviewScoreContents.Where(s => s.IsActive).ToDictionary(s => s.Id, s => s.ItemName + " (" + s.ItemValue + ")");
            var respMap = db.PrResponseTypes.ToDictionary(r => r.ResponseTypeId, r => r);
            var types = db.PrReviewCriteriaTypes.ToDictionary(x => x.ReviewCriteriaTypeId, x => x.Description);

            return prior.Select(t =>
            {
                var details = db.PrReviewScoreDetails.Where(d => d.ReviewReviewerEmployeeId == t.Id).ToList();
                var critIds = details.Select(d => d.ReviewCriteriaId).Distinct().ToList();
                var crits = db.PrReviewCriterias.Where(c => critIds.Contains(c.ReviewCriteriaId)).ToList();
                var sections = db.PrCriteriaSections.ToDictionary(s => s.SectionId, s => s.SectionName);
                return new PrPriorStepVm
                {
                    ReviewerRole = t.ReviewerRole,
                    StepOrder = t.StepOrder,
                    ReviewerName = t.AssignedPersonId.HasValue && people.ContainsKey(t.AssignedPersonId.Value)
                        ? people[t.AssignedPersonId.Value]
                        : PerformanceAdminController.GetRoleManagerNameForEmployee(db, t.EmployeeId, t.ReviewerRole),
                    Score = t.Score,
                    Comments = t.Comments,
                    SubmittedDate = t.SubmittedDate.HasValue ? t.SubmittedDate.Value.ToString("MM/dd/yyyy") : null,
                    Status = t.Status,
                    Criteria = details.Select(d =>
                    {
                        var c = crits.FirstOrDefault(x => x.ReviewCriteriaId == d.ReviewCriteriaId);
                        string display = d.Answer;
                        int scaleId;
                        if (int.TryParse(d.Answer, out scaleId) && scale.ContainsKey(scaleId))
                            display = scale[scaleId];
                        string code = c != null && respMap.ContainsKey(c.ResponseTypeId) ? respMap[c.ResponseTypeId].Code : null;
                        return new PrTaskCriteriaVm
                        {
                            ReviewCriteriaId = d.ReviewCriteriaId,
                            Description = c != null ? c.Description : ("#" + d.ReviewCriteriaId),
                            CriteriaTypeName = c != null && c.CriteriaTypeId.HasValue && types.ContainsKey(c.CriteriaTypeId.Value) ? types[c.CriteriaTypeId.Value] : null,
                            SectionName = c != null && c.SectionId.HasValue && sections.ContainsKey(c.SectionId.Value) ? sections[c.SectionId.Value] : null,
                            SequenceNumber = c != null ? c.SequenceNumber : 0,
                            ResponseTypeId = c != null ? c.ResponseTypeId : 0,
                            ResponseTypeCode = code,
                            Answer = display,
                            Comments = d.Comments
                        };
                    }).ToList()
                };
            }).ToList();
        }

        private void AdvanceWorkflow(ClientDbContext db, PrReviewReviewerEmployee completed)
        {
            var review = db.PrReviews.FirstOrDefault(r => r.ReviewId == completed.ReviewId);
            if (review == null) return;

            var steps = db.PrReviewNotificationSteps.Where(s => s.ReviewId == completed.ReviewId).OrderBy(s => s.StepOrder).ToList();
            var next = steps.FirstOrDefault(s => s.StepOrder > completed.StepOrder);
            var re = db.PrReviewEmployees.FirstOrDefault(e => e.Id == completed.ReviewEmployeeId);
            var sched = db.PrReviewSchedules.FirstOrDefault(s => s.ReviewId == completed.ReviewId);
            int days = sched != null && sched.DaysToComplete > 0 ? sched.DaysToComplete : 14;

            if (next == null)
            {
                // All steps done for this employee
                var siblingPending = db.PrReviewReviewerEmployees.Any(t =>
                    t.ReviewId == completed.ReviewId &&
                    t.ReviewEmployeeId == completed.ReviewEmployeeId &&
                    (t.Status == "Pending" || t.Status == "InProgress"));
                if (!siblingPending)
                {
                    // Mark employee path done; if all employees done, complete review
                    var allEmpIds = db.PrReviewEmployees.Where(e => e.ReviewId == completed.ReviewId).Select(e => e.Id).ToList();
                    bool allDone = allEmpIds.All(eid =>
                        !db.PrReviewReviewerEmployees.Any(t => t.ReviewEmployeeId == eid && (t.Status == "Pending" || t.Status == "InProgress")));
                    if (allDone)
                    {
                        review.Status = "Completed";
                        review.ModifiedDate = DateTime.Now;
                        review.ModifiedBy = User.Identity.Name;
                        db.SaveChanges();
                    }
                }
                return;
            }

            // Reactivate an on-hold downstream task after rework resubmission.
            var onHold = db.PrReviewReviewerEmployees
                .Where(t => t.ReviewEmployeeId == completed.ReviewEmployeeId
                    && t.StepOrder > completed.StepOrder
                    && t.Status == "OnHold")
                .OrderBy(t => t.StepOrder)
                .FirstOrDefault();
            if (onHold != null)
            {
                onHold.Status = "Pending";
                onHold.ReworkReason = null;
                onHold.ReworkComments = null;
                db.SaveChanges();
                string loginUrlHold = null;
                try { loginUrlHold = Url.Action("Login", "Account", null, Request.Url.Scheme); }
                catch { loginUrlHold = ConfigurationManager.AppSettings["ApplicationBaseUrl"] ?? "/Account/Login"; }
                PerformanceAdminController.NotifyAssignee(db, onHold, review.ReviewName, "Performance review rework completed — your action required", loginUrlHold);
                return;
            }

            // Skip optional approver steps when Manager 2 / Manager 3 is not configured on the position.
            while (next != null)
            {
                int? assignedCheck = PerformanceAdminController.ResolveApproverPersonId(db, completed.EmployeeId, next);
                bool isOptionalMgr = string.Equals(next.ReviewerRole, "Approver2", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(next.ReviewerRole, "Approver3", StringComparison.OrdinalIgnoreCase);
                if (!assignedCheck.HasValue && isOptionalMgr)
                {
                    var skipped = next;
                    next = steps.FirstOrDefault(s => s.StepOrder > skipped.StepOrder);
                    continue;
                }
                break;
            }

            if (next == null)
            {
                var siblingPendingAfterSkip = db.PrReviewReviewerEmployees.Any(t =>
                    t.ReviewId == completed.ReviewId &&
                    t.ReviewEmployeeId == completed.ReviewEmployeeId &&
                    (t.Status == "Pending" || t.Status == "InProgress"));
                if (!siblingPendingAfterSkip)
                {
                    var allEmpIds = db.PrReviewEmployees.Where(e => e.ReviewId == completed.ReviewId).Select(e => e.Id).ToList();
                    bool allDone = allEmpIds.All(eid =>
                        !db.PrReviewReviewerEmployees.Any(t => t.ReviewEmployeeId == eid && (t.Status == "Pending" || t.Status == "InProgress")));
                    if (allDone)
                    {
                        review.Status = "Completed";
                        review.ModifiedDate = DateTime.Now;
                        review.ModifiedBy = User.Identity.Name;
                        db.SaveChanges();
                    }
                }
                return;
            }

            int? assigned = PerformanceAdminController.ResolveApproverPersonId(db, completed.EmployeeId, next);
            var existingTask = db.PrReviewReviewerEmployees
                .FirstOrDefault(t => t.ReviewEmployeeId == completed.ReviewEmployeeId && t.NotificationStepId == next.Id);
            if (existingTask != null)
            {
                if (existingTask.Status == "OnHold" || existingTask.Status == "Pending")
                {
                    existingTask.Status = "Pending";
                    existingTask.AssignedPersonId = assigned ?? existingTask.AssignedPersonId;
                    db.SaveChanges();
                    string loginUrlExisting = null;
                    try { loginUrlExisting = Url.Action("Login", "Account", null, Request.Url.Scheme); }
                    catch { loginUrlExisting = ConfigurationManager.AppSettings["ApplicationBaseUrl"] ?? "/Account/Login"; }
                    PerformanceAdminController.NotifyAssignee(db, existingTask, review.ReviewName, "Performance review next step", loginUrlExisting);
                }
                return;
            }

            var nextCritIds = PerformanceAdminController.GetCriteriaIdsForStep(db, next.Id);
            var task = new PrReviewReviewerEmployee
            {
                ReviewId = completed.ReviewId,
                ReviewEmployeeId = completed.ReviewEmployeeId,
                EmployeeId = completed.EmployeeId,
                NotificationStepId = next.Id,
                StepOrder = next.StepOrder,
                ReviewerRole = next.ReviewerRole,
                AssignedPersonId = assigned,
                AssignedCriteriaIds = PerformanceAdminController.BuildCriteriaIdsCsv(nextCritIds),
                Status = "Pending",
                DueDate = DateTime.Today.AddDays(days),
                CreatedDate = DateTime.Now
            };
            db.PrReviewReviewerEmployees.Add(task);
            db.SaveChanges();
            string loginUrl = null;
            try { loginUrl = Url.Action("Login", "Account", null, Request.Url.Scheme); }
            catch { loginUrl = ConfigurationManager.AppSettings["ApplicationBaseUrl"] ?? "/Account/Login"; }
            PerformanceAdminController.NotifyAssignee(db, task, review.ReviewName, "Performance review next step", loginUrl);
        }

        private static bool IsApproverReviewerRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role)) return false;
            return string.Equals(role.Trim(), "Approver1", StringComparison.OrdinalIgnoreCase)
                || string.Equals(role.Trim(), "Approver2", StringComparison.OrdinalIgnoreCase)
                || string.Equals(role.Trim(), "Approver3", StringComparison.OrdinalIgnoreCase);
        }

        private double? ResolveFinalScoreFromApprovers(ClientDbContext db, IEnumerable<PrReviewReviewerEmployee> steps)
        {
            var approverSteps = steps
                .Where(s => s.Status == "Submitted" && IsApproverReviewerRole(s.ReviewerRole))
                .OrderBy(s => s.StepOrder)
                .ToList();
            if (!approverSteps.Any()) return null;

            double total = 0;
            bool hasAny = false;
            foreach (var step in approverSteps)
            {
                double? score = step.Score;
                if (!score.HasValue && db.PrReviewScoreDetails.Any(d => d.ReviewReviewerEmployeeId == step.Id))
                    score = CalculateScore(db, step);
                if (!score.HasValue) continue;
                total += score.Value;
                hasAny = true;
            }
            return hasAny ? Math.Round(total, 2) : (double?)null;
        }

        private static double CalculateScore(ClientDbContext db, PrReviewReviewerEmployee task)
        {
            var rule = db.PrReviewScoringRules.FirstOrDefault(r => r.ReviewId == task.ReviewId);
            var details = db.PrReviewScoreDetails.Where(d => d.ReviewReviewerEmployeeId == task.Id).ToList();
            if (!details.Any()) return 0;

            var criteriaIds = details.Select(d => d.ReviewCriteriaId).Distinct().ToList();
            var criteria = db.PrReviewCriterias.Where(c => criteriaIds.Contains(c.ReviewCriteriaId)).ToList();
            var respMap = db.PrResponseTypes.ToDictionary(r => r.ResponseTypeId, r => (r.Code ?? "").Trim());
            var scale = db.PrReviewScoreContents.Where(s => s.IsActive).ToList();

            var values = new List<double>();
            foreach (var d in details)
            {
                var c = criteria.FirstOrDefault(x => x.ReviewCriteriaId == d.ReviewCriteriaId);
                if (c == null) continue;
                string code = respMap.ContainsKey(c.ResponseTypeId) ? respMap[c.ResponseTypeId] : "";
                // Legacy: id 3 was Numeric before lookup table
                if (string.Equals(code, "Numeric", StringComparison.OrdinalIgnoreCase) || (string.IsNullOrEmpty(code) && c.ResponseTypeId == 3))
                {
                    double n;
                    if (double.TryParse(d.Answer, out n)) values.Add(n);
                }
                else if (string.Equals(code, "Text", StringComparison.OrdinalIgnoreCase))
                {
                    // Text responses do not contribute to numeric score
                    continue;
                }
                else
                {
                    int scaleId;
                    if (int.TryParse(d.Answer, out scaleId))
                    {
                        var item = scale.FirstOrDefault(s => s.Id == scaleId);
                        if (item != null) values.Add(item.ItemValue);
                    }
                    else
                    {
                        var byName = scale.FirstOrDefault(s => string.Equals(s.ItemName, d.Answer, StringComparison.OrdinalIgnoreCase));
                        if (byName != null) values.Add(byName.ItemValue);
                    }
                }
            }
            if (!values.Any()) return 0;
            if (rule != null && rule.IsSumOfAllQuestions) return values.Sum();
            return values.Average();
        }

        private static List<PrTaskCriteriaVm> LoadCriteriaForTask(ClientDbContext db, PrReviewReviewerEmployee task)
        {
            var linkIds = PerformanceAdminController.GetCriteriaIdsForTask(db, task);
            if (!linkIds.Any())
                return new List<PrTaskCriteriaVm>();

            var criterias = db.PrReviewCriterias.Where(c => linkIds.Contains(c.ReviewCriteriaId) && c.IsActive)
                .OrderBy(c => c.SequenceNumber).ToList();
            var sections = db.PrCriteriaSections.ToDictionary(s => s.SectionId, s => s.SectionName);
            var types = db.PrReviewCriteriaTypes.ToDictionary(t => t.ReviewCriteriaTypeId, t => t.Description);
            var respMap = db.PrResponseTypes.ToDictionary(r => r.ResponseTypeId, r => r);
            var scale = db.PrReviewScoreContents.Where(s => s.IsActive).OrderBy(s => s.SortOrder)
                .Select(s => new PrScoreContentVm { Id = s.Id, ItemName = s.ItemName, ItemValue = s.ItemValue, SortOrder = s.SortOrder, IsActive = s.IsActive })
                .ToList();
            var answers = db.PrReviewScoreDetails.Where(d => d.ReviewReviewerEmployeeId == task.Id).ToList();

            return criterias.Select(c =>
            {
                var ans = answers.FirstOrDefault(a => a.ReviewCriteriaId == c.ReviewCriteriaId);
                string code = respMap.ContainsKey(c.ResponseTypeId) ? (respMap[c.ResponseTypeId].Code ?? "") : (c.ResponseTypeId == 3 ? "Numeric" : "Rating");
                bool isRating = string.Equals(code, "Rating", StringComparison.OrdinalIgnoreCase) || code == "1" || (c.ResponseTypeId == 1 && !respMap.ContainsKey(c.ResponseTypeId));
                return new PrTaskCriteriaVm
                {
                    ReviewCriteriaId = c.ReviewCriteriaId,
                    Description = c.Description,
                    CriteriaTypeName = c.CriteriaTypeId.HasValue && types.ContainsKey(c.CriteriaTypeId.Value) ? types[c.CriteriaTypeId.Value] : null,
                    SectionName = c.SectionId.HasValue && sections.ContainsKey(c.SectionId.Value) ? sections[c.SectionId.Value] : null,
                    SequenceNumber = c.SequenceNumber,
                    ResponseTypeId = c.ResponseTypeId,
                    ResponseTypeCode = code,
                    Answer = ans != null ? ans.Answer : null,
                    Comments = ans != null ? ans.Comments : null,
                    ScaleOptions = isRating ? scale : null
                };
            }).ToList();
        }

        private List<PrNotification> QueryUserNotifications(ClientDbContext db)
        {
            int? personId = ResolvePersonId(db);
            string userName = User.Identity.Name ?? "";
            if (!personId.HasValue && string.IsNullOrWhiteSpace(userName))
                return new List<PrNotification>();

            var openTasks = GetOpenTasksForUser(db, personId);
            if (!openTasks.Any())
            {
                // Hide stale notifications tied to tasks that are no longer open.
                var stale = db.PrNotifications
                    .Where(n => !n.IsHidden)
                    .Where(n => (personId.HasValue && n.PersonId == personId) || n.UserName == userName)
                    .ToList();
                bool changed = false;
                foreach (var n in stale)
                {
                    if (!n.ReviewReviewerEmployeeId.HasValue) continue;
                    n.IsHidden = true;
                    n.IsRead = true;
                    changed = true;
                }
                if (changed) db.SaveChanges();
                return new List<PrNotification>();
            }

            var openTaskIds = new HashSet<int>(openTasks.Select(t => t.Id));
            var notifs = db.PrNotifications
                .Where(n => !n.IsHidden && n.ReviewReviewerEmployeeId.HasValue && openTaskIds.Contains(n.ReviewReviewerEmployeeId.Value))
                .Where(n => (personId.HasValue && n.PersonId == personId) || n.UserName == userName)
                .OrderByDescending(n => n.ReceivedOn)
                .ToList();

            var result = new List<PrNotification>();
            foreach (var task in openTasks.OrderByDescending(t => t.CreatedDate))
            {
                var existing = notifs
                    .Where(n => n.ReviewReviewerEmployeeId == task.Id)
                    .OrderByDescending(n => n.ReceivedOn)
                    .FirstOrDefault();
                if (existing != null)
                {
                    result.Add(existing);
                    continue;
                }

                result.Add(new PrNotification
                {
                    Id = 0,
                    PersonId = personId,
                    UserName = userName,
                    ReviewId = task.ReviewId,
                    ReviewReviewerEmployeeId = task.Id,
                    EventName = "Performance review assigned",
                    Contents = PerformanceAdminController.BuildNotificationDescription(task.ReviewerRole),
                    ReceivedOn = task.CreatedDate,
                    IsRead = false,
                    IsHidden = false
                });
            }

            return result.OrderByDescending(n => n.ReceivedOn).ToList();
        }

        private List<PrReviewReviewerEmployee> GetOpenTasksForUser(ClientDbContext db, int? personId)
        {
            if (!personId.HasValue) return new List<PrReviewReviewerEmployee>();
            var tasks = db.PrReviewReviewerEmployees
                .Where(t => (t.Status == "Pending" || t.Status == "InProgress") && t.AssignedPersonId == personId.Value)
                .ToList();
            var hrPending = db.PrReviewReviewerEmployees
                .Where(t => t.ReviewerRole == "HR" && (t.Status == "Pending" || t.Status == "InProgress")
                    && (t.AssignedPersonId == null || t.AssignedPersonId == personId.Value))
                .ToList();
            foreach (var t in hrPending)
            {
                if (tasks.Any(x => x.Id == t.Id)) continue;
                var step = db.PrReviewNotificationSteps.FirstOrDefault(s => s.Id == t.NotificationStepId);
                var hrPeople = step != null ? PerformanceAdminController.ParsePersonIdList(step.OtherPersonIds, step.OtherPersonId) : new List<int>();
                if (!hrPeople.Any() || hrPeople.Contains(personId.Value))
                    tasks.Add(t);
            }
            return tasks;
        }

        private List<PrNotificationVm> MapNotifications(ClientDbContext db, List<PrNotification> notifs)
        {
            var taskIds = notifs.Where(n => n.ReviewReviewerEmployeeId.HasValue).Select(n => n.ReviewReviewerEmployeeId.Value).Distinct().ToList();
            var tasks = taskIds.Any()
                ? db.PrReviewReviewerEmployees.Where(t => taskIds.Contains(t.Id)).ToList()
                : new List<PrReviewReviewerEmployee>();
            var reviewIds = tasks.Select(t => t.ReviewId).Distinct().ToList();
            var reviews = reviewIds.Any()
                ? db.PrReviews.Where(r => reviewIds.Contains(r.ReviewId)).ToDictionary(r => r.ReviewId, r => r.ReviewName)
                : new Dictionary<int, string>();
            var empIds = tasks.Select(t => t.EmployeeId).Distinct().ToList();
            var emps = empIds.Any() ? db.Employees.Where(e => empIds.Contains(e.EmployeeId)).ToList() : new List<Employee>();
            var personIds = emps.Select(e => e.PersonId).Distinct().ToList();
            var people = personIds.Any()
                ? db.Persons.Where(p => personIds.Contains(p.PersonId)).ToDictionary(p => p.PersonId, p => (p.Firstname + " " + p.Lastname).Trim())
                : new Dictionary<int, string>();

            return notifs.Select(n =>
            {
                var task = n.ReviewReviewerEmployeeId.HasValue
                    ? tasks.FirstOrDefault(t => t.Id == n.ReviewReviewerEmployeeId.Value)
                    : null;
                string empName = null;
                string role = task != null ? task.ReviewerRole : null;
                string reviewName = n.ReviewId.HasValue && reviews.ContainsKey(n.ReviewId.Value) ? reviews[n.ReviewId.Value] : null;
                if (task != null)
                {
                    reviewName = reviews.ContainsKey(task.ReviewId) ? reviews[task.ReviewId] : reviewName;
                    role = task.ReviewerRole;
                    var emp = emps.FirstOrDefault(e => e.EmployeeId == task.EmployeeId);
                    if (emp != null && people.ContainsKey(emp.PersonId))
                        empName = people[emp.PersonId];
                }
                return new PrNotificationVm
                {
                    Id = n.Id,
                    TaskId = n.ReviewReviewerEmployeeId,
                    EmployeeName = empName,
                    Description = !string.IsNullOrWhiteSpace(role)
                        ? PerformanceAdminController.BuildNotificationDescription(role)
                        : (!string.IsNullOrWhiteSpace(n.Contents) ? n.Contents : "Complete Performance Review"),
                    ReviewName = reviewName,
                    ReviewerRole = role,
                    ReceivedOn = n.ReceivedOn.ToString("MM/dd/yyyy"),
                    IsRead = n.IsRead
                };
            }).ToList();
        }

        private void MarkTaskNotificationsRead(ClientDbContext db, int? personId, int taskId)
        {
            string userName = User.Identity.Name ?? "";
            var rows = db.PrNotifications
                .Where(n => n.ReviewReviewerEmployeeId == taskId && !n.IsHidden
                    && ((personId.HasValue && n.PersonId == personId) || n.UserName == userName))
                .ToList();
            foreach (var n in rows) n.IsRead = true;
            if (rows.Any()) db.SaveChanges();
        }

        private PrPriorStepVm MapTaskToPriorStep(ClientDbContext db, PrReviewReviewerEmployee t)
        {
            var details = db.PrReviewScoreDetails.Where(d => d.ReviewReviewerEmployeeId == t.Id).ToList();
            var critIds = details.Select(d => d.ReviewCriteriaId).Distinct().ToList();
            var crits = critIds.Any() ? db.PrReviewCriterias.Where(c => critIds.Contains(c.ReviewCriteriaId)).ToList() : new List<PrReviewCriteria>();
            var sections = db.PrCriteriaSections.ToDictionary(s => s.SectionId, s => s.SectionName);
            var scale = db.PrReviewScoreContents.Where(s => s.IsActive).ToDictionary(s => s.Id, s => s.ItemName + " (" + s.ItemValue + ")");
            var respMap = db.PrResponseTypes.ToDictionary(r => r.ResponseTypeId, r => r);
            var types = db.PrReviewCriteriaTypes.ToDictionary(x => x.ReviewCriteriaTypeId, x => x.Description);

            return new PrPriorStepVm
            {
                ReviewerRole = t.ReviewerRole,
                StepOrder = t.StepOrder,
                ReviewerName = PerformanceAdminController.ResolveReviewerPersonName(db, t),
                Score = t.Score,
                Comments = t.Comments,
                SubmittedDate = t.SubmittedDate.HasValue ? t.SubmittedDate.Value.ToString("MM/dd/yyyy") : null,
                Status = t.Status,
                Criteria = details.Select(d =>
                {
                    var c = crits.FirstOrDefault(x => x.ReviewCriteriaId == d.ReviewCriteriaId);
                    string display = d.Answer;
                    int scaleId;
                    if (int.TryParse(d.Answer, out scaleId) && scale.ContainsKey(scaleId))
                        display = scale[scaleId];
                    string code = c != null && respMap.ContainsKey(c.ResponseTypeId) ? respMap[c.ResponseTypeId].Code : null;
                    return new PrTaskCriteriaVm
                    {
                        ReviewCriteriaId = d.ReviewCriteriaId,
                        Description = c != null ? c.Description : ("#" + d.ReviewCriteriaId),
                        CriteriaTypeName = c != null && c.CriteriaTypeId.HasValue && types.ContainsKey(c.CriteriaTypeId.Value) ? types[c.CriteriaTypeId.Value] : null,
                        SectionName = c != null && c.SectionId.HasValue && sections.ContainsKey(c.SectionId.Value) ? sections[c.SectionId.Value] : null,
                        SequenceNumber = c != null ? c.SequenceNumber : 0,
                        ResponseTypeId = c != null ? c.ResponseTypeId : 0,
                        ResponseTypeCode = code,
                        Answer = display,
                        Comments = d.Comments
                    };
                }).ToList()
            };
        }

        private int? ResolveTargetEmployeeId(ClientDbContext db, bool isSelfService)
        {
            int? personId = null;
            if (isSelfService)
            {
                personId = ResolvePersonId(db);
            }
            else
            {
                object selected = SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID);
                if (selected != null)
                {
                    try { personId = Convert.ToInt32(selected); }
                    catch { personId = null; }
                }
            }
            if (!personId.HasValue || personId.Value <= 0) return null;
            var emp = db.Employees
                .Where(e => e.PersonId == personId.Value)
                .OrderByDescending(e => e.EmploymentNumber)
                .FirstOrDefault();
            return emp != null ? emp.EmployeeId : (int?)null;
        }

        private ClientDbContext OpenDb()
        {
            string conn = User.Identity.GetClientConnectionString();
            var db = new ClientDbContext(conn);
            PerformanceModuleSchemaHelper.EnsureSchema(db);
            return db;
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
    }
}
