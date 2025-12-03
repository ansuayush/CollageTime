namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PersonADA")]
    public partial class PersonADA
    {
        public int PersonAdaId { get; set; }

        public int PersonId { get; set; }

        public int AccommodationTypeId { get; set; }

        public int AssociatedDisabilityId { get; set; }

        
        public DateTime? RequestedDate { get; set; }

        [Column(TypeName = "money")]
        public decimal? EstimatedCost { get; set; }

        public bool? AccommodationProvided { get; set; }

        
        public DateTime? ProvidedDate { get; set; }

        [Column(TypeName = "money")]
        public decimal? ActualCost { get; set; }

        [Column(TypeName = "text")]
        public string Notes { get; set; }

        [StringLength(50)]
        public string EnteredBy { get; set; }

        
        public DateTime? EnteredDate { get; set; }

        [StringLength(50)]
        public string ModifiedBy { get; set; }

        
        public DateTime? ModifiedDate { get; set; }

        public virtual DdlAccommodationType DdlAccommodationType { get; set; }

        public virtual DdlDisabilityType DdlDisabilityType { get; set; }

        public virtual Person Person { get; set; }
    }
}
