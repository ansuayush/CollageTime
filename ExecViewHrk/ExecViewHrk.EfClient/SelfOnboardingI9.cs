namespace ExecViewHrk.EfClient
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("SelfOnboardingI9")]
    public partial class SelfOnboardingI9
    {
        [Key]
        [ForeignKey("SelfOnboardingHire")]
        public int HireId { get; set; }

        /// <summary>0 = US citizen, 1 = lawful permanent resident, 2 = alien authorized to work</summary>
        public int CitizenStatus { get; set; }

        [StringLength(50)]
        public string AlienNumber { get; set; }

        public DateTime? PermanentResidentExpire { get; set; }

        public int? LawCitizenOfId { get; set; }

        [StringLength(100)]
        public string LawCitizenOfText { get; set; }

        public DateTime? AlienAuthorizedUntil { get; set; }

        public int? AlienCitizenOfId { get; set; }

        [StringLength(100)]
        public string AlienCitizenOfText { get; set; }

        [StringLength(50)]
        public string AlienRegistrationNumber { get; set; }

        [StringLength(50)]
        public string AdmissionNumber { get; set; }

        [StringLength(50)]
        public string PassportNumber { get; set; }

        public int? CountryOfIssuanceId { get; set; }

        [StringLength(100)]
        public string CountryOfIssuanceText { get; set; }

        public bool TranslatorNotUsed { get; set; }

        public bool TranslatorUsed { get; set; }

        public bool FederalLawAcknowledged { get; set; }

        public bool HideSsnOnForm { get; set; }

        public int? EmployeeDocumentId { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public virtual SelfOnboardingHire SelfOnboardingHire { get; set; }
    }
}
