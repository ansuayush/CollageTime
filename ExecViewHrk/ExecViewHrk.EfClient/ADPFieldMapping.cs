namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ADPFieldMapping
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ADPFieldMapping()
        {
            EarningsCodes = new HashSet<EarningsCode>();
            HoursCodes = new HashSet<HoursCode>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ADPFieldMappingId { get; set; }

        [Required]
        [StringLength(50)]
        public string ADPFieldMappingCode { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EarningsCode> EarningsCodes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HoursCode> HoursCodes { get; set; }
    }
}
