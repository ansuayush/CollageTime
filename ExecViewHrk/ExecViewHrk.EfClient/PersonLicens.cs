namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PersonLicenses")]
    public partial class PersonLicens
    {
        [Key]
        public int PersonLicenseId { get; set; }

        public int PersonId { get; set; }

        public byte LicenseTypeId { get; set; }

        [StringLength(50)]
        public string LicenseNumber { get; set; }

        public int StateId { get; set; }

        public int CountryId { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? ExpirationDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? RevokedDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? ReinstatedDate { get; set; }

        [Column(TypeName = "text")]
        public string Notes { get; set; }

        [StringLength(50)]
        public string EnteredBy { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? EnteredDate { get; set; }

        [StringLength(50)]
        public string ModifiedBy { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? ModifiedDate { get; set; }

        public virtual DdlCountry DdlCountry { get; set; }

        public virtual DdlLicenseType DdlLicenseType { get; set; }

        public virtual DdlState DdlState { get; set; }

        public virtual Person Person { get; set; }
    }
}
