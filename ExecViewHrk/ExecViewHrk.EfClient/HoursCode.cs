namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class HoursCode
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HoursCode()
        {
            TimeCards = new HashSet<TimeCard>();
        }

        public int HoursCodeId { get; set; }

        public int CompanyCodeId { get; set; }

        [Required]
        [StringLength(10)]
        public string HoursCodeCode { get; set; }

        [Required]
        [StringLength(50)]
        public string HoursCodeDescription { get; set; }

        public int ADPFieldMappingId { get; set; }

        public int? ADPAccNumberId { get; set; }

        public decimal? RateOverride { get; set; }

        public decimal? RateMultiplier { get; set; }

        public bool ExcludeFromOT { get; set; }

        public bool SubtractFromRegular { get; set; }

        public bool NonPayCode { get; set; }

        public bool IsHoursCodeActive { get; set; }

        public virtual ADPAccNumber ADPAccNumber { get; set; }

        public virtual ADPFieldMapping ADPFieldMapping { get; set; }

        public virtual CompanyCode CompanyCode { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TimeCard> TimeCards { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsRetro { get; set; }

    }
}
