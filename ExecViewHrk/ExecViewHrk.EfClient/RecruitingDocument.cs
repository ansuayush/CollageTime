namespace ExecViewHrk.EfClient
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("RecruitingDocuments")]
    public partial class RecruitingDocument
    {
        [Key]
        public int DocumentSetupId { get; set; }

        [Required]
        [StringLength(200)]
        public string DocumentName { get; set; }

        [StringLength(500)]
        public string Instructions { get; set; }

        public bool IsRequired { get; set; }

        public bool RequiresSignature { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; }
    }
}
