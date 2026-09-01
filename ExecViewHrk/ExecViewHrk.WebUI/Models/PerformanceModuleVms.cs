using System;
using System.Collections.Generic;

namespace ExecViewHrk.WebUI.Models
{
    public class PrScoreContentVm
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public double ItemValue { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class PrCriteriaTypeVm
    {
        public int ReviewCriteriaTypeId { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class PrResponseTypeVm
    {
        public int ResponseTypeId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
    }

    public class PrSectionVm
    {
        public int SectionId { get; set; }
        public string SectionName { get; set; }
        public string Description { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class PrCriteriaVm
    {
        public int ReviewCriteriaId { get; set; }
        public int? CriteriaTypeId { get; set; }
        public string CriteriaTypeName { get; set; }
        public int? SectionId { get; set; }
        public string SectionName { get; set; }
        public string Description { get; set; }
        public int ResponseTypeId { get; set; }
        public string ResponseTypeName { get; set; }
        public string ResponseTypeCode { get; set; }
        public int SequenceNumber { get; set; }
        public bool IsActive { get; set; }
        public string Caption1 { get; set; }
    }

    public class PrReviewVm
    {
        public int ReviewId { get; set; }
        public string ReviewName { get; set; }
        public string RevieweeMode { get; set; }
        public string Status { get; set; }
        public string HrOwner { get; set; }
        public string Notes { get; set; }
        public string IntervalType { get; set; }
        public string FromSchedule { get; set; }
        public string FromDate { get; set; }
        public int DaysToComplete { get; set; }
        public int RepeatDays { get; set; }
        public bool IsAverageOfAllQuestions { get; set; }
        public bool IsSumOfAllQuestions { get; set; }
        public bool WeightedAverage { get; set; }
        public List<int> EmployeeIds { get; set; }
        public List<string> EmployeeNames { get; set; }
        public List<PrStepVm> Steps { get; set; }
        public List<int> CriteriaIds { get; set; }
    }

    public class PrStepVm
    {
        public int Id { get; set; }
        public int StepOrder { get; set; }
        public string ReviewerRole { get; set; }
        public int? OtherPersonId { get; set; }
        public List<int> OtherPersonIds { get; set; }
        public List<string> OtherPersonNames { get; set; }
        public bool IsViewPriorResponses { get; set; }
        public List<int> CriteriaIds { get; set; }
    }

    public class PrTaskVm
    {
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public string ReviewName { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string ManagerName { get; set; }
        public string ReviewDate { get; set; }
        public string ReviewStatusLabel { get; set; }
        public string ReviewerLabel { get; set; }
        public string ReviewerName { get; set; }
        public string ReviewerRole { get; set; }
        public int StepOrder { get; set; }
        public string Status { get; set; }
        public double? Score { get; set; }
        public string DueDate { get; set; }
        public string Comments { get; set; }
        public bool CanEdit { get; set; }
        public bool IsHrFinal { get; set; }
        public bool CanReject { get; set; }
        public bool CollapsePriorSteps { get; set; }
        public string ReworkReason { get; set; }
        public string ReworkComments { get; set; }
        public List<PrRejectTargetVm> RejectTargets { get; set; }
        public List<PrTaskCriteriaVm> Criteria { get; set; }
        public List<PrPriorStepVm> PriorSteps { get; set; }
    }

    public class PrRejectTargetVm
    {
        public string Value { get; set; }
        public string Label { get; set; }
    }

    public class PrRejectRequestVm
    {
        public string TargetRole { get; set; }
        public string Comments { get; set; }
        public string Reason { get; set; }
    }

    public class PrPriorStepVm
    {
        public string ReviewerRole { get; set; }
        public int StepOrder { get; set; }
        public string ReviewerName { get; set; }
        public double? Score { get; set; }
        public string Comments { get; set; }
        public string SubmittedDate { get; set; }
        public string Status { get; set; }
        public List<PrTaskCriteriaVm> Criteria { get; set; }
    }

    public class PrTaskCriteriaVm
    {
        public int ReviewCriteriaId { get; set; }
        public string Description { get; set; }
        public string CriteriaTypeName { get; set; }
        public string SectionName { get; set; }
        public int SequenceNumber { get; set; }
        public int ResponseTypeId { get; set; }
        public string ResponseTypeCode { get; set; }
        public string Answer { get; set; }
        public string Comments { get; set; }
        public List<PrScoreContentVm> ScaleOptions { get; set; }
    }

    public class PrDashboardVm
    {
        public int DraftReviews { get; set; }
        public int InProgress { get; set; }
        public int AwaitingHr { get; set; }
        public int Completed { get; set; }
        public int MyOpenTasks { get; set; }
    }

    public class PrAnswerItemVm
    {
        public int ReviewCriteriaId { get; set; }
        public string Answer { get; set; }
        public string Comments { get; set; }
    }

    public class PrEmployeeOptionVm
    {
        public int id { get; set; }
        public int personId { get; set; }
        public string text { get; set; }
        public string name { get; set; }
        public string fileNumber { get; set; }
    }

    public class PrNotificationVm
    {
        public int Id { get; set; }
        public int? TaskId { get; set; }
        public string EmployeeName { get; set; }
        public string Description { get; set; }
        public string ReviewName { get; set; }
        public string ReviewerRole { get; set; }
        public string ReceivedOn { get; set; }
        public bool IsRead { get; set; }
    }

    public class PrEmployeeCompletedReviewVm
    {
        public int ReviewEmployeeId { get; set; }
        public int ReviewId { get; set; }
        public string ReviewName { get; set; }
        public string ReviewType { get; set; }
        public string InitiatedDate { get; set; }
        public string ReviewerSummary { get; set; }
        public string CompletionDate { get; set; }
        public string Status { get; set; }
        public double? Score { get; set; }
        public List<PrEmployeeCompletedStepVm> Steps { get; set; }
    }

    public class PrEmployeeCompletedStepVm
    {
        public string ReviewerRole { get; set; }
        public string ReviewerLabel { get; set; }
        public string ReviewerName { get; set; }
        public string Status { get; set; }
        public string SubmittedDate { get; set; }
        public double? Score { get; set; }
        public string Comments { get; set; }
    }

    public class PrEmployeeCompletedReviewDetailVm
    {
        public int ReviewEmployeeId { get; set; }
        public string ReviewName { get; set; }
        public string EmployeeName { get; set; }
        public string ManagerName { get; set; }
        public string ReviewDate { get; set; }
        public string Status { get; set; }
        public double? FinalScore { get; set; }
        public List<PrPriorStepVm> Steps { get; set; }
    }
}
