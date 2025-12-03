using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExecViewHrk.EfClient
{
  public class TimeCardsArchive
    {
        [Key]
        public int TimeCardsArchiveId { get; set; }

        public int CompanyCodeId { get; set; }
        public int? EmployeeId { get; set; }
        public int? TimeCardId { get; set; }
        public string FileNumber { get; set; }

        [Column(TypeName = "date")]
        public DateTime ActualDate { get; set; }

        public int ProjectNumber { get; set; }

        public double? DailyHours { get; set; }
        public DateTime? TimeIn { get; set; }

        public DateTime? TimeOut { get; set; }

        public DateTime? LunchOut { get; set; }

        public DateTime? LunchBack { get; set; }


        public int? HoursCodeId { get; set; }

        public double? Hours { get; set; }
        public int? HoursCodeReasonId { get; set; }

        public int? EarningsCodeId { get; set; }
        //public short? EarningsCodeId { get; set; }

        public double? EarningsAmount { get; set; }

        public int? DepartmentId { get; set; }
        public int? JobId { get; set; }

        //public int? TempDeptId { get; set; }
        public short? TempDeptId { get; set; }

        public int? TempJobId { get; set; }

        public string Project { get; set; }
        public string Task { get; set; }
        public double? OT { get; set; }

        public int? MealsTaken { get; set; }
        public double? Rate { get; set; }
        public double? HoursCodeRate { get; set; }
        public string EnteredBy { get; set; }
        public DateTime? EnteredDate { get; set; }
        public bool IsApproved { get; set; }

        public string ApprovedBy { get; set; }
        public int? PositionId { get; set; }
        public string UserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool IsDeleted { get; set; }

        public virtual CompanyCode CompanyCode { get; set; }

        public virtual EarningsCode EarningsCode { get; set; }

        //public virtual Employee Employee { get; set; }

        public virtual HoursCode HoursCode { get; set; }
        public virtual Department Department { get; set; }
        public virtual Job Job { get; set; }
        //public virtual Job Job1 { get; set; }
        //public virtual Department Department1 { get; set; }

    }
}
