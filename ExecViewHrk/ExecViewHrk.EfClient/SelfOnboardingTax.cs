namespace ExecViewHrk.EfClient
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("SelfOnboardingTax")]
    public partial class SelfOnboardingTax
    {
        [Key]
        [ForeignKey("SelfOnboardingHire")]
        public int HireId { get; set; }

        public int? FilingStatusId { get; set; }

        public decimal? OtherIncomeAmount { get; set; }

        public decimal? DeductionsAmount { get; set; }

        public decimal? ExtraWithholdingAmount { get; set; }

        public decimal? ExtraWithholdingPercent { get; set; }

        public bool FederalExempt { get; set; }

        public bool CopyFromFederal { get; set; }

        public int? WorkingCountryId { get; set; }

        public int? WorkingStateId { get; set; }

        public int? StateTaxStatusId { get; set; }

        [StringLength(50)]
        public string StateExemptions { get; set; }

        public decimal? StateAdditionalWithholdingAmount { get; set; }

        public decimal? StateAdditionalWithholdingPercent { get; set; }

        public bool StateExempt { get; set; }

        public int? EmployeeDocumentId { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public virtual SelfOnboardingHire SelfOnboardingHire { get; set; }
    }
}
