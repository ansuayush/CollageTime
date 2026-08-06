namespace ExecViewHrk.EfClient
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("SelfOnboardingUploads")]
    public partial class SelfOnboardingUpload
    {
        [Key]
        public int UploadId { get; set; }

        public int HireId { get; set; }

        public int? ProfileDocumentId { get; set; }

        [Required]
        [StringLength(260)]
        public string FileName { get; set; }

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; }

        public DateTime UploadedDate { get; set; }

        public bool IsSigned { get; set; }

        public virtual SelfOnboardingHire SelfOnboardingHire { get; set; }
    }
}
