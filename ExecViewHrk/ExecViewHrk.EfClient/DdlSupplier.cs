namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DdlSupplier
    {
        [Key]
        public int SupplierId { get; set; }

        [Required]
        [StringLength(10)]
        public string Code { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [StringLength(50)]
        public string AddressLine1 { get; set; }

        [StringLength(50)]
        public string AddressLine2 { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        public int? StateId { get; set; }

        [StringLength(9)]
        public string ZipCode { get; set; }

        public int? CountryId { get; set; }

        [StringLength(10)]
        public string Phone { get; set; }

        [StringLength(10)]
        public string Fax { get; set; }

        [StringLength(50)]
        public string Contact { get; set; }

        [StringLength(100)]
        public string WebPage { get; set; }

        [StringLength(50)]
        public string AccountNumber { get; set; }

        [Column(TypeName = "text")]
        public string Notes { get; set; }

        public virtual DdlCountry DdlCountry { get; set; }

        public virtual DdlState DdlState { get; set; }
    }
}
