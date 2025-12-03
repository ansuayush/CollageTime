namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PersonPassport
    {
        public int PersonPassportId { get; set; }

        public int PersonId { get; set; }

        public int CountryId { get; set; }

        [StringLength(50)]
        public string PassportNumber { get; set; }

        [StringLength(50)]
        public string PassportStorage { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? IssueDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? ExpirationDate { get; set; }

        [StringLength(50)]
        public string EnteredBy { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? EnteredDate { get; set; }

        [StringLength(50)]
        public string ModifiedBy { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? ModifiedDate { get; set; }

        public virtual DdlCountry DdlCountry { get; set; }

        public virtual Person Person { get; set; }
    }
}
