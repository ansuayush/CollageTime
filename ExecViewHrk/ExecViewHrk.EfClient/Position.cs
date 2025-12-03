namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Position : Positions
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Position()
        {
            E_Positions = new HashSet<E_Positions>();
        }

        public virtual BusinessUnit BusinessUnit { get; set; }

        public virtual Department Department { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<E_Positions> E_Positions { get; set; }

        public virtual Job Job { get; set; }
        public virtual PositionBusinessLevels PositionBusinessLevels { get; set; }
        public virtual DdlPositionCategory DdlPositionCategories { get; set; }
        public virtual DdlPositionTypes DdlPositionType { get; set; }
        public virtual DdlPositionGrade DdlPositionGrades { get; set; }
        public virtual UserDefinedSegment1s UserDefinedSegment1s { get; set; }

        public virtual UserDefinedSegment2s UserDefinedSegment2s { get; set; }
    }
}
