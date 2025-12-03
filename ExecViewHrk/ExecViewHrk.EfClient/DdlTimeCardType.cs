namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DdlTimeCardType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DdlTimeCardType()
        {
            Employees = new HashSet<Employee>();
        }

        [Key]
        public int TimeCardTypeId { get; set; }

        [Required]
        [StringLength(10)]
        public string TimeCardTypeCode { get; set; }

        [Required]
        [StringLength(50)]
        public string TimeCardTypeDescription { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Employee> Employees { get; set; }

        public virtual TimeCardDisplayColumn TimeCardDisplayColumn { get; set; }
    }
}
