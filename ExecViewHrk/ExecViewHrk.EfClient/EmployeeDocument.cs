namespace ExecViewHrk.EfClient
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("EmployeeDocuments")]
    public partial class EmployeeDocument
    {
        [Key]
        public int DocumentId { get; set; }

        public int EmployeeId { get; set; }

        [Required]
        [StringLength(260)]
        public string FileName { get; set; }

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; }

        [Required]
        [StringLength(100)]
        public string UploadedBy { get; set; }

        public DateTime UploadedDate { get; set; }

        public bool IsSigned { get; set; }

        [StringLength(100)]
        public string SignedBy { get; set; }

        public DateTime? SignedDate { get; set; }

        [StringLength(20)]
        public string SignerRole { get; set; }

        [StringLength(150)]
        public string SignatureName { get; set; }

        [StringLength(500)]
        public string SignatureImagePath { get; set; }

        public virtual Employee Employee { get; set; }
    }
}
