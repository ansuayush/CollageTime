namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DdlApplicantSource
    {
        [Key]
        public int ApplicantSourceId { get; set; }

        [Required]
        [StringLength(50)]
        public string Description { get; set; }

        [Required]
        [StringLength(10)]
        public string Code { get; set; }

        [StringLength(50)]
        public string AddressLineOne { get; set; }

        [StringLength(50)]
        public string AddressLineTwo { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        public byte? StateId { get; set; }

        [StringLength(9)]
        public string ZipCode { get; set; }

        public int? CountryId { get; set; }

        [StringLength(10)]
        public string PhoneNumber { get; set; }

        [StringLength(10)]
        public string FaxNumber { get; set; }

        [StringLength(50)]
        public string Contact { get; set; }

        [StringLength(100)]
        public string WebAddress { get; set; }

        [StringLength(50)]
        public string AccountNumber { get; set; }

        [Column(TypeName = "text")]
        public string Notes { get; set; }

        public bool Active { get; set; }
    }
}
