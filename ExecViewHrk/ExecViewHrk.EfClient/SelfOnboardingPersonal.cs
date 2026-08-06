namespace ExecViewHrk.EfClient
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("SelfOnboardingPersonal")]
    public partial class SelfOnboardingPersonal
    {
        [Key]
        [ForeignKey("SelfOnboardingHire")]
        public int HireId { get; set; }

        public int? PrefixId { get; set; }
        public int? SuffixId { get; set; }

        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string MiddleName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(100)]
        public string PreferredName { get; set; }

        [StringLength(200)]
        public string WorkEmail { get; set; }

        [StringLength(200)]
        public string HomeEmail { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(20)]
        public string SSN { get; set; }

        public int? GenderId { get; set; }
        public int? MaritalStatusId { get; set; }
        public int? EthnicityId { get; set; }

        [StringLength(200)]
        public string Address1 { get; set; }

        [StringLength(200)]
        public string Address2 { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        public int? StateId { get; set; }

        [StringLength(20)]
        public string Zip { get; set; }

        public int? CountryId { get; set; }
        public int? LicenseCountryId { get; set; }

        [StringLength(150)]
        public string EmergencyName { get; set; }

        [StringLength(50)]
        public string EmergencyPhone { get; set; }

        public int? RelationshipTypeId { get; set; }
        public int? FilingStatusId { get; set; }
        public int? WorkingCountryId { get; set; }
        public int? WorkingStateId { get; set; }
        public int? StateTaxStatusId { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public virtual SelfOnboardingHire SelfOnboardingHire { get; set; }
    }
}
