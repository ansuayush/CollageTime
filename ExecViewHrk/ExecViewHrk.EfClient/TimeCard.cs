namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TimeCard
    {
        public int TimeCardId { get; set; }

        public int CompanyCodeId { get; set; }

        public int EmployeeId { get; set; }

        [Column(TypeName = "date")]
        public DateTime ActualDate { get; set; }

        public int ProjectNumber { get; set; }

        public double? DailyHours { get; set; }

        public int? HoursCodeId { get; set; }

        public double? Hours { get; set; }

        public int? EarningsCodeId { get; set; }

        public double? EarningsAmount { get; set; }

        public int? TempDeptId { get; set; }

        public int? TempJobId { get; set; }

        public DateTime? TimeIn { get; set; }

        public DateTime? TimeOut { get; set; }

        public DateTime? LunchOut { get; set; }

        public DateTime? LunchBack { get; set; }

        public bool IsApproved { get; set; }

        [StringLength(50)]
        public string ApprovedBy { get; set; }
        public string DisApprovedBy { get; set; }
        public int HoursCodeReasonId { get; set; }

        public string FileNumber { get; set; }

        public string Project { get; set; }
        public string Task { get; set; }
        public double? OT { get; set; }
        public int? MealsTaken { get; set; }
        public double? Rate { get; set; }
        public double? HoursCodeRate { get; set; }
        public string EnteredBy { get; set; }
        public DateTime? EnteredDate { get; set; }
        public int? DepartmentId { get; set; }
        public int? JobId { get; set; }

        public int? PositionId { get; set; }
        public bool IsDeleted { get; set; }
        public string DeletedBy { get; set; }
        public virtual CompanyCode CompanyCode { get; set; }

        public virtual Department Department { get; set; }

        public virtual EarningsCode EarningsCode { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual HoursCode HoursCode { get; set; }

        public virtual Job Job { get; set; }
        // public virtual Positions Positions { get; set; }
        //  public virtual Department Department1 { get; set; }
        // public virtual Job Job1 { get; set; }
        public string UserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? E_PositionId { get; set; }
    }
}
