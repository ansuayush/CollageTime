namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PersonVehicle
    {
        public int PersonVehicleId { get; set; }

        public int PersonId { get; set; }

        [Required]
        [StringLength(50)]
        public string LicenseNumber { get; set; }

        [StringLength(50)]
        public string Make { get; set; }

        [StringLength(50)]
        public string Model { get; set; }

        [StringLength(50)]
        public string Color { get; set; }

        public int? StateId { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? AcquisitionDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? SoldDate { get; set; }

        [Column(TypeName = "text")]
        public string Notes { get; set; }

        [Required]
        [StringLength(50)]
        public string EnteredBy { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime EnteredDate { get; set; }

        [StringLength(50)]
        public string ModifiedBy { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? ModifiedDate { get; set; }

        public virtual DdlState DdlState { get; set; }

        public virtual Person Person { get; set; }
    }
}
