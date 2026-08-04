namespace ExecViewHrk.EfClient
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("JobApplicationFiles")]
    public partial class JobApplicationFile
    {
        [Key]
        public int FileId { get; set; }

        public int ApplicationId { get; set; }

        public int? DocumentSetupId { get; set; }

        [Required]
        [StringLength(50)]
        public string FileCategory { get; set; }

        [Required]
        [StringLength(260)]
        public string FileName { get; set; }

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; }

        public DateTime UploadedDate { get; set; }

        public virtual JobApplication JobApplication { get; set; }
    }
}
