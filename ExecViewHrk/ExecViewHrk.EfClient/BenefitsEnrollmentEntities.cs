namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("BenCategories")]
    public partial class BenCategory
    {
        [Key]
        public int CategoryId { get; set; }
        [Required, StringLength(100)]
        public string CategoryName { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
        [StringLength(100)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [StringLength(100)]
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    [Table("BenWaitingPeriods")]
    public partial class BenWaitingPeriod
    {
        [Key]
        public int WaitingPeriodId { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        public int Days { get; set; }
        [Required, StringLength(50)]
        public string CalculationType { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }

    [Table("BenEligibilityRules")]
    public partial class BenEligibilityRule
    {
        [Key]
        public int EligibilityRuleId { get; set; }
        [Required, StringLength(150)]
        public string RuleName { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        [StringLength(200)]
        public string EmploymentStatusIds { get; set; }
        [StringLength(200)]
        public string EmployeeTypeIds { get; set; }
        public double? MinHours { get; set; }
        public int? MinServiceDays { get; set; }
        public int? MinAge { get; set; }
        [StringLength(1000)]
        public string RuleExpression { get; set; }
        public bool IsActive { get; set; }
    }

    [Table("BenPlans")]
    public partial class BenPlan
    {
        public BenPlan()
        {
            CoverageOptions = new HashSet<BenCoverageOption>();
        }

        [Key]
        public int PlanId { get; set; }
        [Required, StringLength(200)]
        public string PlanName { get; set; }
        [StringLength(50)]
        public string PlanCode { get; set; }
        public int CategoryId { get; set; }
        [StringLength(150)]
        public string Carrier { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public double EmployeeCost { get; set; }
        public double EmployerCost { get; set; }
        public bool RequireDependents { get; set; }
        public bool RequireBeneficiary { get; set; }
        public bool WaiveAllowed { get; set; }
        public bool IsActive { get; set; }
        [StringLength(100)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [StringLength(100)]
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual ICollection<BenCoverageOption> CoverageOptions { get; set; }
    }

    [Table("BenCoverageOptions")]
    public partial class BenCoverageOption
    {
        [Key]
        public int CoverageOptionId { get; set; }
        public int PlanId { get; set; }
        [Required, StringLength(50)]
        public string OptionCode { get; set; }
        [Required, StringLength(150)]
        public string OptionName { get; set; }
        public double EmployeeCost { get; set; }
        public double EmployerCost { get; set; }
        public bool RequiresDependent { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("PlanId")]
        public virtual BenPlan Plan { get; set; }
    }

    [Table("BenClasses")]
    public partial class BenClass
    {
        [Key]
        public int BenefitClassId { get; set; }
        [Required, StringLength(150)]
        public string ClassName { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        public int? WaitingPeriodId { get; set; }
        public int? EligibilityRuleId { get; set; }
        public bool IsActive { get; set; }
        [StringLength(100)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [StringLength(100)]
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    [Table("BenClassPlans")]
    public partial class BenClassPlan
    {
        [Key]
        public int BenefitClassPlanId { get; set; }
        public int BenefitClassId { get; set; }
        public int PlanId { get; set; }
        public int SortOrder { get; set; }
    }

    [Table("BenEnrollmentPeriods")]
    public partial class BenEnrollmentPeriod
    {
        [Key]
        public int EnrollmentPeriodId { get; set; }
        [Required, StringLength(200)]
        public string EnrollmentName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? CoverageEffectiveDate { get; set; }
        public DateTime? AllowChangesUntil { get; set; }
        [Required, StringLength(30)]
        public string Status { get; set; }
        [StringLength(1000)]
        public string EnrollmentMessage { get; set; }
        public bool ReminderEmails { get; set; }
        [StringLength(100)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [StringLength(100)]
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    [Table("BenEmployeeClasses")]
    public partial class BenEmployeeClass
    {
        [Key]
        public int EmployeeBenefitClassId { get; set; }
        public int EmployeeId { get; set; }
        public int BenefitClassId { get; set; }
        public DateTime? EffectiveDate { get; set; }
        [StringLength(100)]
        public string AssignedBy { get; set; }
        public DateTime AssignedDate { get; set; }
    }

    [Table("BenEnrollments")]
    public partial class BenEnrollment
    {
        [Key]
        public int EnrollmentId { get; set; }
        public int EmployeeId { get; set; }
        public int EnrollmentPeriodId { get; set; }
        public int? BenefitClassId { get; set; }
        [Required, StringLength(30)]
        public string Status { get; set; }
        [StringLength(50)]
        public string ConfirmationNumber { get; set; }
        public DateTime? SubmittedDate { get; set; }
        [StringLength(100)]
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        [StringLength(150)]
        public string SignedName { get; set; }
        public DateTime? SignedDate { get; set; }
        [StringLength(50)]
        public string SignedIp { get; set; }
        public bool TermsAccepted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    [Table("BenElections")]
    public partial class BenElection
    {
        [Key]
        public int ElectionId { get; set; }
        public int EnrollmentId { get; set; }
        public int PlanId { get; set; }
        public int? CoverageOptionId { get; set; }
        public bool IsWaived { get; set; }
        public double EmployeeCost { get; set; }
        public double EmployerCost { get; set; }
        public DateTime? EffectiveDate { get; set; }
    }

    [Table("BenDependents")]
    public partial class BenDependent
    {
        [Key]
        public int DependentId { get; set; }
        public int EnrollmentId { get; set; }
        public int? ElectionId { get; set; }
        [Required, StringLength(100)]
        public string FirstName { get; set; }
        [Required, StringLength(100)]
        public string LastName { get; set; }
        [Required, StringLength(50)]
        public string Relationship { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [StringLength(20)]
        public string Gender { get; set; }
        [StringLength(20)]
        public string SSN { get; set; }
    }

    [Table("BenBeneficiaries")]
    public partial class BenBeneficiary
    {
        [Key]
        public int BeneficiaryId { get; set; }
        public int EnrollmentId { get; set; }
        public int? ElectionId { get; set; }
        [Required, StringLength(150)]
        public string Name { get; set; }
        [Required, StringLength(50)]
        public string Relationship { get; set; }
        public double Percentage { get; set; }
    }

    [Table("BenDocuments")]
    public partial class BenDocument
    {
        [Key]
        public int DocumentId { get; set; }
        public int? PlanId { get; set; }
        [Required, StringLength(200)]
        public string DocumentName { get; set; }
        [StringLength(50)]
        public string DocumentType { get; set; }
        [StringLength(260)]
        public string FileName { get; set; }
        [StringLength(500)]
        public string FilePath { get; set; }
        public bool IsActive { get; set; }
    }

    [Table("BenAudit")]
    public partial class BenAudit
    {
        [Key]
        public int AuditId { get; set; }
        public int? EnrollmentId { get; set; }
        public int? EmployeeId { get; set; }
        [Required, StringLength(100)]
        public string Action { get; set; }
        [StringLength(1000)]
        public string Details { get; set; }
        [StringLength(100)]
        public string PerformedBy { get; set; }
        public DateTime PerformedDate { get; set; }
        [StringLength(50)]
        public string IpAddress { get; set; }
    }
}
