namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class EmployeeI9Documents
    {
        [Key]
        public int EmployeeI9DocumentId { get; set; }

        public int EmployeeId { get; set; }

        public byte? I9DocumentTypeId { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? PresentedDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? ExpirationDate { get; set; }

        [Column(TypeName = "text")]
        public string Notes { get; set; }

        [Required]
        [StringLength(50)]
        public string EnteredBy { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime EnteredDate { get; set; }

        public virtual DdlI9DocumentTypes DdlI9DocumentTypes { get; set; }

        public virtual Employee Employee { get; set; }
    }
}
