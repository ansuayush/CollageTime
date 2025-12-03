namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class E_Positions
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public E_Positions()
        {
            E_PositionSalaryHistories = new HashSet<E_PositionSalaryHistories>();
        }

        [Key]
        public int E_PositionId { get; set; }

        public int EmployeeId { get; set; }

        public int PositionId { get; set; }

        public int? PayFrequencyId { get; set; }

        public int? ReportsToID { get; set; }

        public int? RateTypeId { get; set; }

        public string FileNumber { get; set; }

        public int? EmployeeTypeId { get; set; }

        public int? PayGroupId { get; set; }

        public int? companyID { get; set; }

        public bool? PrimaryPosition { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? StartDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? EndDate { get; set; }

        [Column(TypeName = "text")]
        public string Notes { get; set; }
        [Column(TypeName = "money")]
        public decimal? salary { get; set; }

        [StringLength(50)]
        public string EnteredBy { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? EnteredDate { get; set; }

        [StringLength(50)]
        public string ModifiedBy { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? ModifiedDate { get; set; }


        public virtual DdlPayFrequency DdlPayFrequency { get; set; }

        public virtual DdlRateType DdlRateType { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual Position Position { get; set; }
        public int? PositionTypeID { get; set; }
        public int? PositionCategoryID { get; set; }
        public int? PositionGradeID { get; set; }
        public decimal? FTE { get; set; }
        public DateTime? actualEndDate { get; set; }
        public DateTime? ProjectedEndDate { get; set; }
        public virtual DdlPositionCategory DdlPositionCategory { get; set; }
        public virtual DdlPositionGrade DdlPositionGrade { get; set; }
        public virtual DdlPositionTypes DdlPositionTypes { get; set; }

        public virtual DdlPayGroup DdlPayGroup { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<E_PositionSalaryHistories> E_PositionSalaryHistories { get; set; }

        public int? AdpYear { get; set; }

        public Nullable<decimal> AdpWSLimit { get; set; }

        public int DepartmentId { get; set; }

        public bool? IsDesignated { get; set; } // Need to set this value to true when designator is created
        public bool? IsDeleted { get; set; }
        public string DeletedBy { get; set; }
        public string UserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public DateTime? CostNumberEffectiveDate { get; set; }
        public string CostNumber { get; set; }
        public decimal? CostNumber1Percent { get; set; }
        public string CostNumber2Account { get; set; }
        public decimal? CostNumber2Percent { get; set; }
        public string CostNumber3Account { get; set; }
        public decimal? CostNumber3Percent { get; set; }
        public string CostNumber4Account { get; set; }
        public decimal? CostNumber4Percent { get; set; }
        public string CostNumber5Account { get; set; }
        public decimal? CostNumber5Percent { get; set; }
        public string CostNumber6Account { get; set; }
        public decimal? CostNumber6Percent { get; set; }

        public int? EmployeeClassId { get; set; }
    }
}
