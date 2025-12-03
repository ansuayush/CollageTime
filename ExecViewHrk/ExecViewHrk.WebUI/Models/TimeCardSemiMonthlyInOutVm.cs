using ExecViewHrk.EfClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class TimeCardSemiMonthlyInOutVm
    {
        public TimeCardHeaderVm TimeCardHeader { get; set; }

        public TimeCardSemiMonthlyInOutVm()
        {
            TimeCardHeader = new TimeCardHeaderVm();
        }

        public int TimeCardId { get; set; }

        public short? TempDeptId { get; set; }
        public string TempDepartmentCode { get; set; }

        public int? TempJobId { get; set; }
        public string TempJobCode { get; set; }

        public short? EarningsCodeId { get; set; }
        public double? EarningsAmount { get; set; }

        [DataType(DataType.Date)]
        public DateTime ActualDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime Day { get; set; }

        public int ProjectNumber { get; set; }

        [DataType(DataType.Time)]
        public DateTime? TimeIn { get; set; }

        [DataType(DataType.Time)]
        public DateTime? TimeOut { get; set; }

        [DataType(DataType.Time)]
        public DateTime? LunchOut { get; set; }

        [DataType(DataType.Time)]
        public DateTime? LunchBack { get; set; }

        public int? HoursCodeId { get; set; }
        public double? Hours { get; set; }

        public bool Approved { get; set; }

        public int WeekNum { get; set; }

        public string LineTotal { get; set; }

        public bool IsLineApproved { get; set; }

        public bool ShowLineApprovedActive { get; set; }

        public bool IsArchived { get; set; }

        public TimeCardDisplayColumn timeCardDislayColumns { get; set; } 
    }
}