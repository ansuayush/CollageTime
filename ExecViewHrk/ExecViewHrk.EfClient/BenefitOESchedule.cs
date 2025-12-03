namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class BenefitOESchedule
    {
        public int BenefitOEScheduleId { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime EndDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime EffectiveDate { get; set; }

        public int CompanyCodeId { get; set; }

        [StringLength(50)]
        public string ScheduleGroup { get; set; }

        public virtual CompanyCode CompanyCode { get; set; }
    }
}
