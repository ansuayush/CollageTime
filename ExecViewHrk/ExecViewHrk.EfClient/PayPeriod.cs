namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PayPeriod
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PayPeriod()
        {
            TimeCardApprovals = new HashSet<TimeCardApproval>();
        }

        public int PayPeriodId { get; set; }

        public int? CompanyCodeId { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime EndDate { get; set; }

        public int PayFrequencyId { get; set; }

        public bool IsPayPeriodActive { get; set; }

        public bool LockoutEmployees { get; set; }

        public bool LockoutManagers { get; set; }

        public bool IsArchived { get; set; }

        public int? PayGroupCode { get; set; }

        public int? PayPeriodNumber { get; set; }

        public virtual CompanyCode CompanyCode { get; set; }

        public virtual DdlPayFrequency DdlPayFrequency { get; set; }
        public bool IsDeleted { get; set; }
        public string DeletedBy { get; set; }
        public string UserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public DateTime? PayDate { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TimeCardApproval> TimeCardApprovals { get; set; }
        public bool Isexported { get; set; }
    }
}
