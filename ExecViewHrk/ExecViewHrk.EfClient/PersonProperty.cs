namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PersonProperty
    {
        [Key]
        public int PersonPropertyTypeId { get; set; }

        public int PersonId { get; set; }

        public int PropertyTypeId { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? AcquiredDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? ReleaseDate { get; set; }

        [Column(TypeName = "money")]
        public decimal? EstimatedValue { get; set; }

        [StringLength(50)]
        public string AssetNumber { get; set; }

        [Column(TypeName = "text")]
        public string PropertyDescription { get; set; }

        [Column(TypeName = "text")]
        public string Notes { get; set; }

        [StringLength(50)]
        public string EnteredBy { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? EnteredDate { get; set; }

        [StringLength(50)]
        public string ModifiedBy { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? ModifiedDate { get; set; }

        public virtual DdlPropertyType DdlPropertyType { get; set; }

        public virtual Person Person { get; set; }
    }
}
