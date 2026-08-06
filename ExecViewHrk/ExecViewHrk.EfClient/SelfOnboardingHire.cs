namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("SelfOnboardingHires")]
    public partial class SelfOnboardingHire
    {
        public SelfOnboardingHire()
        {
            Signatures = new HashSet<SelfOnboardingSignature>();
            BankAccounts = new HashSet<SelfOnboardingBankAccount>();
            Uploads = new HashSet<SelfOnboardingUpload>();
        }

        [Key]
        public int HireId { get; set; }

        public int? PositionId { get; set; }

        [StringLength(200)]
        public string PositionTitle { get; set; }

        public int? ProfileId { get; set; }

        public int? ApplicationId { get; set; }

        public int? ApplicantId { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [StringLength(200)]
        public string HomeEmail { get; set; }

        [StringLength(200)]
        public string WorkEmail { get; set; }

        [StringLength(50)]
        public string FileNumber { get; set; }

        public int? OfferLetterId { get; set; }

        [StringLength(100)]
        public string GeneratedUserName { get; set; }

        [StringLength(128)]
        public string AspNetUserId { get; set; }

        [Required]
        [StringLength(30)]
        public string Status { get; set; }

        public int CurrentStep { get; set; }

        [StringLength(50)]
        public string TransactionId { get; set; }

        public DateTime? NoticeSentDate { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public DateTime? ConfirmationDate { get; set; }

        [StringLength(100)]
        public string ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public int? EmployeeId { get; set; }

        [StringLength(1000)]
        public string RejectionReason { get; set; }

        [StringLength(200)]
        public string RejectedFormName { get; set; }

        [StringLength(100)]
        public string RejectedBy { get; set; }

        public DateTime? RejectedDate { get; set; }

        [StringLength(100)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [StringLength(100)]
        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public virtual SelfOnboardingPersonal Personal { get; set; }

        public virtual SelfOnboardingI9 I9 { get; set; }

        public virtual SelfOnboardingTax Tax { get; set; }

        public virtual ICollection<SelfOnboardingSignature> Signatures { get; set; }

        public virtual ICollection<SelfOnboardingBankAccount> BankAccounts { get; set; }

        public virtual ICollection<SelfOnboardingUpload> Uploads { get; set; }
    }
}
