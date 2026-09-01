namespace ExecViewHrk.EfClient
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PR_ReviewScoreContent")]
    public partial class PrReviewScoreContent
    {
        [Key]
        public int Id { get; set; }
        [StringLength(100)]
        public string ItemName { get; set; }
        public double ItemValue { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }

    [Table("PR_ReviewCriteriaType")]
    public partial class PrReviewCriteriaType
    {
        [Key]
        public int ReviewCriteriaTypeId { get; set; }
        [Required, StringLength(200)]
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }

    [Table("PR_ResponseType")]
    public partial class PrResponseType
    {
        [Key]
        public int ResponseTypeId { get; set; }
        [Required, StringLength(50)]
        public string Code { get; set; }
        [Required, StringLength(100)]
        public string Description { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
    }

    [Table("PR_CriteriaSection")]
    public partial class PrCriteriaSection
    {
        [Key]
        public int SectionId { get; set; }
        [Required, StringLength(200)]
        public string SectionName { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }

    [Table("PR_ReviewCriteria")]
    public partial class PrReviewCriteria
    {
        [Key]
        public int ReviewCriteriaId { get; set; }
        public int? CriteriaTypeId { get; set; }
        public int? SectionId { get; set; }
        public int? PositionId { get; set; }
        [Required, StringLength(1000)]
        public string Description { get; set; }
        /// <summary>1 = Rating scale, 3 = Numeric</summary>
        public int ResponseTypeId { get; set; }
        public int? ScoreContentGroupId { get; set; }
        public int SequenceNumber { get; set; }
        public bool IsActive { get; set; }
        [StringLength(200)]
        public string Caption1 { get; set; }
    }

    [Table("PR_Review")]
    public partial class PrReview
    {
        [Key]
        public int ReviewId { get; set; }
        [Required, StringLength(200)]
        public string ReviewName { get; set; }
        /// <summary>Employee | Position | Department | Supervisor</summary>
        [StringLength(50)]
        public string RevieweeMode { get; set; }
        [StringLength(30)]
        public string Status { get; set; }
        [StringLength(100)]
        public string HrOwner { get; set; }
        [StringLength(1000)]
        public string Notes { get; set; }
        [StringLength(100)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [StringLength(100)]
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    [Table("PR_ReviewSchedule")]
    public partial class PrReviewSchedule
    {
        [Key]
        public int Id { get; set; }
        public int ReviewId { get; set; }
        [StringLength(20)]
        public string IntervalType { get; set; }
        [StringLength(50)]
        public string FromSchedule { get; set; }
        public DateTime? FromDate { get; set; }
        public int DaysToComplete { get; set; }
        public int RepeatDays { get; set; }
    }

    [Table("PR_ReviewScoringRule")]
    public partial class PrReviewScoringRule
    {
        [Key]
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public bool WeightedAverage { get; set; }
        public bool IsSumOfAllQuestions { get; set; }
        public bool IsAverageOfAllQuestions { get; set; }
        public bool IsAverageOfSections { get; set; }
        public bool IsSumOfAverageWithinSections { get; set; }
        public bool IsCustomRule { get; set; }
    }

    [Table("PR_ReviewEmployee")]
    public partial class PrReviewEmployee
    {
        [Key]
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public int EmployeeId { get; set; }
        public int? PersonId { get; set; }
        public int? PositionId { get; set; }
        [StringLength(100)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    [Table("PR_ReviewNotificationStep")]
    public partial class PrReviewNotificationStep
    {
        [Key]
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public int StepOrder { get; set; }
        /// <summary>Approver1 | Approver2 | Approver3 | HR | Other | Employee</summary>
        [Required, StringLength(50)]
        public string ReviewerRole { get; set; }
        public int? OtherPersonId { get; set; }
        /// <summary>Comma-separated PersonIds for multi-HR (or Other) assignees.</summary>
        [StringLength(500)]
        public string OtherPersonIds { get; set; }
        public bool IsViewPriorResponses { get; set; }
    }

    [Table("PR_ReviewReviewerCriteria")]
    public partial class PrReviewReviewerCriteria
    {
        [Key]
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public int NotificationStepId { get; set; }
        public int ReviewCriteriaId { get; set; }
    }

    [Table("PR_ReviewReviewerEmployee")]
    public partial class PrReviewReviewerEmployee
    {
        [Key]
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public int ReviewEmployeeId { get; set; }
        public int EmployeeId { get; set; }
        public int NotificationStepId { get; set; }
        public int StepOrder { get; set; }
        [StringLength(50)]
        public string ReviewerRole { get; set; }
        public int? AssignedPersonId { get; set; }
        [StringLength(30)]
        public string Status { get; set; }
        public double? Score { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? SubmittedDate { get; set; }
        [StringLength(1000)]
        public string Comments { get; set; }
        [StringLength(500)]
        public string AssignedCriteriaIds { get; set; }
        [StringLength(500)]
        public string RejectionReason { get; set; }
        [StringLength(2000)]
        public string RejectionComments { get; set; }
        [StringLength(500)]
        public string ReworkReason { get; set; }
        [StringLength(2000)]
        public string ReworkComments { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    [Table("PR_ReviewScoreDetails")]
    public partial class PrReviewScoreDetail
    {
        [Key]
        public int Id { get; set; }
        public int ReviewReviewerEmployeeId { get; set; }
        public int ReviewCriteriaId { get; set; }
        [StringLength(1000)]
        public string Answer { get; set; }
        [StringLength(2000)]
        public string Comments { get; set; }
    }

    [Table("PR_Notification")]
    public partial class PrNotification
    {
        [Key]
        public int Id { get; set; }
        public int? PersonId { get; set; }
        [StringLength(256)]
        public string UserName { get; set; }
        public int? ReviewId { get; set; }
        public int? ReviewReviewerEmployeeId { get; set; }
        [Required, StringLength(50)]
        public string EventName { get; set; }
        [StringLength(1000)]
        public string Contents { get; set; }
        public DateTime ReceivedOn { get; set; }
        public bool IsRead { get; set; }
        public bool IsHidden { get; set; }
    }
}
