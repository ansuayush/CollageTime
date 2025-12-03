using ExecViewHrk.EfClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class DdlEducationEstablishmentViewModel
    {
        [Key]
        public int EducationEstablishmentId { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Code { get; set; }

        public string AddressLineOne { get; set; }

        public string AddressLineTwo { get; set; }

        public string City { get; set; }

        [UIHint("GridForeignKey")]
        public int? StateId { get; set; }

        [UIHint("MaskedZipCode")]
        public string ZipCode { get; set; }

        [UIHint("GridForeignKey")]
        public int? CountryId { get; set; }

        [StringLength(15)]
        [MinLength(14, ErrorMessage = "The {0} must be of 10 digits")]
        [UIHint("MaskedPhoneNumber")]
        public string PhoneNumber { get; set; }

        public string FaxNumber { get; set; }

        [UIHint("GridForeignKey")]
        public int? EducationTypeId { get; set; }

        public string AccountNumber { get; set; }

        public string Contact { get; set; }

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