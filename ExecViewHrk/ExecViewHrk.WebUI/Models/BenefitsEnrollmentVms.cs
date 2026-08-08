using System;
using System.Collections.Generic;

namespace ExecViewHrk.WebUI.Models
{
    public class BenCategoryVm
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class BenWaitingPeriodVm
    {
        public int WaitingPeriodId { get; set; }
        public string Name { get; set; }
        public int Days { get; set; }
        public string CalculationType { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class BenEligibilityRuleVm
    {
        public int EligibilityRuleId { get; set; }
        public string RuleName { get; set; }
        public string Description { get; set; }
        public string EmploymentStatusIds { get; set; }
        public string EmployeeTypeIds { get; set; }
        public double? MinHours { get; set; }
        public int? MinServiceDays { get; set; }
        public int? MinAge { get; set; }
        public string RuleExpression { get; set; }
        public bool IsActive { get; set; }
    }

    public class BenCoverageOptionVm
    {
        public int CoverageOptionId { get; set; }
        public int PlanId { get; set; }
        public string OptionCode { get; set; }
        public string OptionName { get; set; }
        public double EmployeeCost { get; set; }
        public double EmployerCost { get; set; }
        public bool RequiresDependent { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class BenPlanVm
    {
        public int PlanId { get; set; }
        public string PlanName { get; set; }
        public string PlanCode { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Carrier { get; set; }
        public string Description { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public double EmployeeCost { get; set; }
        public double EmployerCost { get; set; }
        public bool RequireDependents { get; set; }
        public bool RequireBeneficiary { get; set; }
        public bool WaiveAllowed { get; set; }
        public bool IsActive { get; set; }
        public List<BenCoverageOptionVm> CoverageOptions { get; set; }
    }

    public class BenClassVm
    {
        public int BenefitClassId { get; set; }
        public string ClassName { get; set; }
        public string Description { get; set; }
        public int? WaitingPeriodId { get; set; }
        public string WaitingPeriodName { get; set; }
        public int? EligibilityRuleId { get; set; }
        public string EligibilityRuleName { get; set; }
        public bool IsActive { get; set; }
        public List<int> PlanIds { get; set; }
        public List<string> PlanNames { get; set; }
    }

    public class BenEnrollmentPeriodVm
    {
        public int EnrollmentPeriodId { get; set; }
        public string EnrollmentName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? CoverageEffectiveDate { get; set; }
        public DateTime? AllowChangesUntil { get; set; }
        public string Status { get; set; }
        public string EnrollmentMessage { get; set; }
        public bool ReminderEmails { get; set; }
    }

    public class BenEmployeeClassVm
    {
        public int EmployeeBenefitClassId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string FileNumber { get; set; }
        public int BenefitClassId { get; set; }
        public string BenefitClassName { get; set; }
        public DateTime? EffectiveDate { get; set; }
    }

    public class BenEnrollmentListVm
    {
        public int EnrollmentId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string FileNumber { get; set; }
        public string BenefitClassName { get; set; }
        public string EnrollmentPeriodName { get; set; }
        public string Status { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public string ConfirmationNumber { get; set; }
        public bool HasSignature { get; set; }
    }

    public class BenDashboardVm
    {
        public int TotalAssigned { get; set; }
        public int Completed { get; set; }
        public int Pending { get; set; }
        public int InProgress { get; set; }
        public int Waived { get; set; }
        public List<BenEnrollmentListVm> Recent { get; set; }
    }

    public class BenPortalVm
    {
        public bool IsEligible { get; set; }
        public string Message { get; set; }
        public BenEnrollmentPeriodVm ActivePeriod { get; set; }
        public BenClassVm BenefitClass { get; set; }
        public List<BenPlanVm> Plans { get; set; }
        public int? EnrollmentId { get; set; }
        public string EnrollmentStatus { get; set; }
        public string ConfirmationNumber { get; set; }
        public DateTime? Deadline { get; set; }
    }

    public class BenElectionSaveVm
    {
        public int PlanId { get; set; }
        public int? CoverageOptionId { get; set; }
        public bool IsWaived { get; set; }
    }

    public class BenDependentVm
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Relationship { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string SSN { get; set; }
        public int? ElectionId { get; set; }
    }

    public class BenBeneficiaryVm
    {
        public string Name { get; set; }
        public string Relationship { get; set; }
        public double Percentage { get; set; }
        public int? ElectionId { get; set; }
    }
}
