namespace ExecViewHrk.EfClient
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("SelfOnboardingSignatures")]
    public partial class SelfOnboardingSignature
    {
        [Key]
        public int SignatureId { get; set; }

        public int HireId { get; set; }

        [Required]
        [StringLength(100)]
        public string DocumentKey { get; set; }

        public int? ProfileDocumentId { get; set; }

        public bool IsSigned { get; set; }

        [StringLength(150)]
        public string SignedName { get; set; }

        public DateTime? SignedDate { get; set; }

        [StringLength(50)]
        public string SignedIp { get; set; }

        [StringLength(50)]
        public string TransactionId { get; set; }

        [StringLength(500)]
        public string FilePath { get; set; }

        public int? EmployeeDocumentId { get; set; }

        public virtual SelfOnboardingHire SelfOnboardingHire { get; set; }
    }
}
