namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PersonWorkPermit
    {
        
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PersonWorkPermitId { get; set; }

        public int PersonId { get; set; }

        public int CountryId { get; set; }

        [StringLength(50)]
        public string WorkPermitNumber { get; set; }

        [StringLength(50)]
        public string WorkPermitType { get; set; }

        [StringLength(50)]
        public string IssuingAuthority { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? IssueDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? ExpirationDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? ExtensionDate { get; set; }

        [Column(TypeName = "text")]
        public string Notes { get; set; }

        [Required]
        [StringLength(50)]
        public string EnteredBy { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime EnteredDate { get; set; }

        public virtual DdlCountry DdlCountry { get; set; }

        public virtual Person Person { get; set; }
    }
}
