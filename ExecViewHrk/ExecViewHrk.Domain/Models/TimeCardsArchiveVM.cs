using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using ExecViewHrk.EfClient;

namespace ExecViewHrk.Models
{
    public class TimeCardsArchiveVM
    {
        public int TimeCardsArchiveId { get; set; }
        public int? TimeCardId { get; set; }
        public int CompanyCodeId { get; set; }

        public string FileNumber { get; set; }

        [Column(TypeName = "date")]
        public DateTime ActualDate { get; set; }
        public string ActualDateString
        {
            get
            {
                return ActualDate == null ? string.Empty : this.ActualDate.ToString("MM-dd-yyyy");
            }
            set
            {
                
            }
        }
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

        public double? EarningsAmount { get; set; }

        public int? DepartmentId { get; set; }
        public int? JobId { get; set; }

        public int? TempDeptId { get; set; }

        public int? TempJobId { get; set; }

        public string Project { get; set; }
        public string Task { get; set; }
        public double? OT { get; set; }

        public int? MealsTaken { get; set; }
        public double? Rate { get; set; }
        public double? HoursCodeRate { get; set; }
        public string EnteredBy { get; set; }
        public DateTime? EnterDate { get; set; }
        public bool IsApproved { get; set; }

        [StringLength(50)]
        public string ApprovedBy { get; set; }

        [DataType(DataType.Date)]
        public DateTime Day { get; set; }

        public int WeekNum { get; set; }
        public double? LineTotal { get; set; }
        public bool ShowLineApprovedActive { get; set; }
        public bool IsLineApproved { get; set; }
        public TimeCardDisplayColumn timeCardDislayColumns { get; set; }

        public int EmployeeId { get; set; }
        public string PayPeriod { get; set; }
        public int? PayPeriodId { get; set; }
        public bool Approved { get; set; }
        public bool IsArchived { get; set; }
        public int? PositionId { get; set; }
        public string UserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
