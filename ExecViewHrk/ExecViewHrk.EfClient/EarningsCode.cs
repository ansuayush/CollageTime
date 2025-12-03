namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class EarningsCode
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EarningsCode()
        {
            TimeCards = new HashSet<TimeCard>();
        }

        public int EarningsCodeId { get; set; }

        public int CompanyCodeId { get; set; }

        [Required]
        [StringLength(20)]
        public string EarningsCodeCode { get; set; }

        [Required]
        [StringLength(20)]
        public string EarningsCodeDescription { get; set; }

        public int ADPFieldMappingId { get; set; }

        public bool IsEarningsCodeActive { get; set; }

        [StringLength(20)]
        public string EarningsCodeOffset { get; set; }

        [StringLength(20)]
        public string DeductionCodeOffset { get; set; }

        public virtual ADPFieldMapping ADPFieldMapping { get; set; }

        public virtual CompanyCode CompanyCode { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TimeCard> TimeCards { get; set; }

        public bool TreatyCode { set; get; }
        public bool IsDefault { get; set; }
    }
}
