namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DdlHospital
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DdlHospital()
        {
            PersonAdditionals = new HashSet<PersonAdditional>();
        }

        [Key]
        public int HospitalId { get; set; }

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

        public byte StateId { get; set; }

        [StringLength(9)]
        public string ZipCode { get; set; }

        public int CountryId { get; set; }

        [StringLength(10)]
        public string PhoneNumber { get; set; }

        [StringLength(10)]
        public string FaxNumber { get; set; }

        [StringLength(50)]
        public string Contact { get; set; }

        [StringLength(100)]
        public string WebAddress { get; set; }

        [Column(TypeName = "text")]
        public string Notes { get; set; }

        public bool Active { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonAdditional> PersonAdditionals { get; set; }
    }
}
