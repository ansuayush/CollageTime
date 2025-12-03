namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DdlEducationEstablishment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DdlEducationEstablishment()
        {
            PersonEducations = new HashSet<PersonEducation>();
        }

        [Key]
        public int EducationEstablishmentId { get; set; }

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

        public int? StateId { get; set; }

        [StringLength(9)]
        public string ZipCode { get; set; }

        public int? CountryId { get; set; }

        [StringLength(14)]
        public string PhoneNumber { get; set; }

        [StringLength(10)]
        public string FaxNumber { get; set; }

        public int? EducationTypeId { get; set; }

        [StringLength(50)]
        public string AccountNumber { get; set; }

        [StringLength(50)]
        public string Contact { get; set; }

        [StringLength(100)]
        public string WebAddress { get; set; }

        [Column(TypeName = "text")]
        public string Notes { get; set; }

        public bool Active { get; set; }

        public virtual DdlCountry DdlCountry { get; set; }

        public virtual DdlEducationType DdlEducationType { get; set; }

        public virtual DdlState DdlState { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonEducation> PersonEducations { get; set; }
    }
}
