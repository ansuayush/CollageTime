namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserDefinedSegment1s
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserDefinedSegment1s()
        {
            Positions = new HashSet<Position>();
        }

        [Key]
        public int UserDefinedSegment1Id { get; set; }

        [Required]
        [StringLength(10)]
        public string UserDefinedSegment1Code { get; set; }

        [Required]
        [StringLength(50)]
        public string UserDefinedSegment1Description { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Position> Positions { get; set; }
    }
}
