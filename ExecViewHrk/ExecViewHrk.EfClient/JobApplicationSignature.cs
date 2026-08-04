namespace ExecViewHrk.EfClient
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("JobApplicationSignatures")]
    public partial class JobApplicationSignature
    {
        [Key]
        public int SignatureId { get; set; }

        public int ApplicationId { get; set; }

        [Required]
        [StringLength(50)]
        public string SignatureType { get; set; }

        public int? DocumentSetupId { get; set; }

        [Required]
        [StringLength(150)]
        public string SignerName { get; set; }

        public DateTime SignedDate { get; set; }

        [StringLength(500)]
        public string SignatureImagePath { get; set; }

        public virtual JobApplication JobApplication { get; set; }
    }
}
