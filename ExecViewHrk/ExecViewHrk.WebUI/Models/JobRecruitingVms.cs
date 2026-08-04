using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExecViewHrk.WebUI.Models
{
    public class JobRequisitionVm
    {
        public int RequisitionId { get; set; }

        [Required]
        [StringLength(50)]
        public string RequisitionNumber { get; set; }

        [Required]
        [StringLength(200)]
        public string PositionTitle { get; set; }

        public string Division { get; set; }
        public string Department { get; set; }
        public int? PositionId { get; set; }
        public string Description { get; set; }
        public DateTime RequisitionDate { get; set; }
        public DateTime? OpenDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public string Status { get; set; }
        public int ApplicantCount { get; set; }
        public bool IsPublished { get; set; }
    }

    public class RecruitingQuestionVm
    {
        public int QuestionId { get; set; }
        [Required]
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public string Choices { get; set; }
        public int WizardPage { get; set; }
        public int SortOrder { get; set; }
        public bool IsRequired { get; set; }
        public bool IsActive { get; set; }
    }

    public class RecruitingDocumentVm
    {
        public int DocumentSetupId { get; set; }
        [Required]
        public string DocumentName { get; set; }
        public string Instructions { get; set; }
        public bool IsRequired { get; set; }
        public bool RequiresSignature { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class RecruitingConfigVm
    {
        public int ConfigId { get; set; }
        public string HomePageHtml { get; set; }
        public string IntroductionHtml { get; set; }
        public string ReviewSubmitHtml { get; set; }
        public string AttestationHtml { get; set; }
        public string ExternalApplyUrl { get; set; }
        public int EmployerId { get; set; }
    }

    public class JobApplicationListVm
    {
        public int ApplicationId { get; set; }
        public int RequisitionId { get; set; }
        public string PositionTitle { get; set; }
        public string RequisitionNumber { get; set; }
        public string ApplicantName { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public int CurrentStep { get; set; }
    }

    public class JobPortalOpenJobVm
    {
        public int RequisitionId { get; set; }
        public string RequisitionNumber { get; set; }
        public string PositionTitle { get; set; }
        public string Division { get; set; }
        public string Department { get; set; }
        public string Description { get; set; }
        public DateTime? OpenDate { get; set; }
        public bool AlreadyApplied { get; set; }
        public int? ExistingApplicationId { get; set; }
        public string ApplicationStatus { get; set; }
    }

    public class ApplyRegisterVm
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Phone { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        public int EmployerId { get; set; }
    }

    public class ApplyLoginVm
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public int EmployerId { get; set; }
        public int? RequisitionId { get; set; }
    }

    public class ApplyAnswerItem
    {
        public int QuestionId { get; set; }
        public string AnswerText { get; set; }
    }

    public class ApplyReferenceItem
    {
        public string FullName { get; set; }
        public string Relationship { get; set; }
        public string Company { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string YearsKnown { get; set; }
    }

    public class ApplyEmploymentItem
    {
        public string EmployerName { get; set; }
        public string JobTitle { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Duties { get; set; }
        public string ReasonLeft { get; set; }
    }

    public class ApplyEducationItem
    {
        public string SchoolName { get; set; }
        public string Degree { get; set; }
        public string FieldOfStudy { get; set; }
        public string GraduationYear { get; set; }
    }
}
