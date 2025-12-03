namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class E_PositionSalaryHistories
    {
        [Key]
        public int E_PositionSalaryHistoryId { get; set; }

        public int E_PositionId { get; set; }
        public int? RateTypeId { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? EffectiveDate { get; set; }

        public decimal? PayRate { get; set; }
        public decimal? AnnualSalary { get; set; }
        public decimal? HoursPerPayPeriod { get; set; }

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
        public DateTime? EndDate { get; set; }

        public virtual E_Positions E_Positions { get; set; }
        public decimal? IncreasePercent { get; set; }
        public bool IsDeleted { get; set; }
        public string DeletedBy { get; set; }
        public string UserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
