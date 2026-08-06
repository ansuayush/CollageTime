using System;
using System.Collections.Generic;

namespace ExecViewHrk.WebUI.Models
{
    public class OnboardingProfileVm
    {
        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int DocumentCount { get; set; }
    }

    public class OnboardingProfileDocumentVm
    {
        public int ProfileDocumentId { get; set; }
        public int ProfileId { get; set; }
        public string DocumentName { get; set; }
        public int? DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public bool RequiresSignature { get; set; }
        public bool EnableUpload { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public bool IsSigned { get; set; }
    }

    public class OnboardingLookupVm
    {
        public int LookupId { get; set; }
        public string LookupType { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class SelfOnboardingHireVm
    {
        public int HireId { get; set; }
        public int? PositionId { get; set; }
        public string PositionTitle { get; set; }
        public int? ProfileId { get; set; }
        public string ProfileName { get; set; }
        public int? ApplicationId { get; set; }
        public int? ApplicantId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string HomeEmail { get; set; }
        public string WorkEmail { get; set; }
        public string FileNumber { get; set; }
        public int? OfferLetterId { get; set; }
        public string OfferLetterName { get; set; }
        public string GeneratedUserName { get; set; }
        public string Status { get; set; }
        public int CurrentStep { get; set; }
        public string TransactionId { get; set; }
        public DateTime? NoticeSentDate { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string ApprovedBy { get; set; }
        public int? EmployeeId { get; set; }
        public string RejectionReason { get; set; }
        public string RejectedFormName { get; set; }
        public string RejectedBy { get; set; }
        public DateTime? RejectedDate { get; set; }
    }

    public class SelfOnboardingPersonalVm
    {
        public int HireId { get; set; }
        public int? PrefixId { get; set; }
        public int? SuffixId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PreferredName { get; set; }
        public string WorkEmail { get; set; }
        public string HomeEmail { get; set; }
        public string Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string SSN { get; set; }
        public int? GenderId { get; set; }
        public int? MaritalStatusId { get; set; }
        public int? EthnicityId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public int? StateId { get; set; }
        public string Zip { get; set; }
        public int? CountryId { get; set; }
        public int? LicenseCountryId { get; set; }
        public string EmergencyName { get; set; }
        public string EmergencyPhone { get; set; }
        public int? RelationshipTypeId { get; set; }
        public int? FilingStatusId { get; set; }
        public int? WorkingCountryId { get; set; }
        public int? WorkingStateId { get; set; }
        public int? StateTaxStatusId { get; set; }
    }

    public class SelfOnboardingBankVm
    {
        public int BankAccountId { get; set; }
        public int HireId { get; set; }
        public int? AccountTypeId { get; set; }
        public string AccountTypeName { get; set; }
        public string BankName { get; set; }
        public string RoutingNumber { get; set; }
        public string AccountNumber { get; set; }
        public bool IsPrimary { get; set; }
    }

    public class SelfOnboardingI9Vm
    {
        public int HireId { get; set; }
        public int CitizenStatus { get; set; }
        public string AlienNumber { get; set; }
        public DateTime? PermanentResidentExpire { get; set; }
        public int? LawCitizenOfId { get; set; }
        public string LawCitizenOfText { get; set; }
        public DateTime? AlienAuthorizedUntil { get; set; }
        public int? AlienCitizenOfId { get; set; }
        public string AlienCitizenOfText { get; set; }
        public string AlienRegistrationNumber { get; set; }
        public string AdmissionNumber { get; set; }
        public string PassportNumber { get; set; }
        public int? CountryOfIssuanceId { get; set; }
        public string CountryOfIssuanceText { get; set; }
        public bool TranslatorNotUsed { get; set; }
        public bool TranslatorUsed { get; set; }
        public bool FederalLawAcknowledged { get; set; }
        public bool HideSsnOnForm { get; set; }
        public bool IsSigned { get; set; }
        public int? EmployeeDocumentId { get; set; }
    }

    public class SelfOnboardingTaxVm
    {
        public int HireId { get; set; }
        public int? FilingStatusId { get; set; }
        public string FilingStatusName { get; set; }
        // doubles (not decimal) so MVC JavaScriptSerializer can return GetWizardData JSON
        public double? OtherIncomeAmount { get; set; }
        public double? DeductionsAmount { get; set; }
        public double? ExtraWithholdingAmount { get; set; }
        public double? ExtraWithholdingPercent { get; set; }
        public bool FederalExempt { get; set; }
        public bool CopyFromFederal { get; set; }
        public int? WorkingCountryId { get; set; }
        public int? WorkingStateId { get; set; }
        public int? StateTaxStatusId { get; set; }
        public string StateTaxStatusName { get; set; }
        public string StateExemptions { get; set; }
        public double? StateAdditionalWithholdingAmount { get; set; }
        public double? StateAdditionalWithholdingPercent { get; set; }
        public bool StateExempt { get; set; }
        public bool IsSigned { get; set; }
        public int? EmployeeDocumentId { get; set; }
    }

    public class SelfOnboardingWizardVm
    {
        public SelfOnboardingHireVm Hire { get; set; }
        public SelfOnboardingPersonalVm Personal { get; set; }
        public SelfOnboardingI9Vm I9 { get; set; }
        public SelfOnboardingTaxVm Tax { get; set; }
        public List<OnboardingProfileDocumentVm> Documents { get; set; }
        public List<SelfOnboardingBankVm> BankAccounts { get; set; }
        public Dictionary<string, bool> SignatureFlags { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsHrReview { get; set; }
    }
}
