namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PositionBusinessLevels
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PositionBusinessLevels()
        {
            this.Positions = new HashSet<Position>();
        }
        [Key]
        public int BusinessLevelNbr { get; set; }
        public string BusinessLevelNotes { get; set; }
        public string BusinessLevelTitle { get; set; }
        public Nullable<byte> BusinessLevelTypeNbr { get; set; }
        public Nullable<int> ParentBULevelNbr { get; set; }
        public Nullable<int> LocationId { get; set; }
        public Nullable<int> EEoFileStatusNbr { get; set; }
        public Nullable<int> FedralEINNbr { get; set; }
        public Nullable<int> PayFrequencyId { get; set; }
        public int SchedeuledHours { get; set; }
        public bool Active { get; set; }
        public Nullable<System.DateTime> EnteredDate { get; set; }
        public string EnteredBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string BusinessLevelCode { get; set; }
        public string BudgetReported { get; set; }

        public virtual DdlBusinessLevelTypes DdlBusinessLevelTypes { get; set; }
        public virtual DdlEEOFileStatuses DdlEEOFileStatuses { get; set; }
        public virtual DdlEINs DdlEINs { get; set; }
        public virtual DdlPayFrequency DdlPayFrequency { get; set; }
        public virtual Location Location { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Position> Positions { get; set; }
    }
}