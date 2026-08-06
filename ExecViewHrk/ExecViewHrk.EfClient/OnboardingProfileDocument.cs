namespace ExecViewHrk.EfClient
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("OnboardingProfileDocuments")]
    public partial class OnboardingProfileDocument
    {
        [Key]
        public int ProfileDocumentId { get; set; }

        public int ProfileId { get; set; }

        [Required]
        [StringLength(200)]
        public string DocumentName { get; set; }

        public int? DocumentTypeId { get; set; }

        [StringLength(260)]
        public string FileName { get; set; }

        [StringLength(500)]
        public string FilePath { get; set; }

        public bool RequiresSignature { get; set; }

        public bool EnableUpload { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; }

        public virtual OnboardingProfile OnboardingProfile { get; set; }
    }
}
