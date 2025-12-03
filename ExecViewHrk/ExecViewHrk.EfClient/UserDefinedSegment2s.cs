namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserDefinedSegment2s
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserDefinedSegment2s()
        {
            Positions = new HashSet<Position>();
        }

        [Key]
        public int UserDefinedSegment2Id { get; set; }

        [Required]
        [StringLength(10)]
        public string UserDefinedSegment2Code { get; set; }

        [Required]
        [StringLength(50)]
        public string UserDefinedSegment2Description { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Position> Positions { get; set; }
    }
}
