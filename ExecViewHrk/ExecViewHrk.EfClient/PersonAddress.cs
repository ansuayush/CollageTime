namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PersonAddress
    {
        public int PersonAddressId { get; set; }

        public int PersonId { get; set; }

        public int AddressTypeId { get; set; }

        [StringLength(50)]
        public string AddressLineOne { get; set; }

        [StringLength(50)]
        public string AddressLineTwo { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        public int? StateId { get; set; }

        [StringLength(10)]
        public string ZipCode { get; set; }

        public int? CountryId { get; set; }

        public bool? CheckPayrollAddress { get; set; }

        public bool? CorrespondenceAddress { get; set; }

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

        public virtual DdlAddressType DdlAddressType { get; set; }

        public virtual DdlCountry DdlCountry { get; set; }

        public virtual DdlState DdlState { get; set; }

        public virtual Person Person { get; set; }
        public bool IsPrimaryAddress { get; set; }
    }
}
