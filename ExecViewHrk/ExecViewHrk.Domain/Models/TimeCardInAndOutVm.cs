using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel.DataAnnotations;
using ExecViewHrk.EfClient;

namespace ExecViewHrk.Models
{
  public class TimeCardInAndOutVm
    {
        public int TimeCardId { get; set; }

        public int CompanyCodeId { get; set; }
        public string CompanyCodeDescription { get; set; }

        public short DepartmentId { get; set; }
        public string DepartmentDescription { get; set; }

        public int EmployeeId { get; set; }
        public string EmployeeFullName { get; set; }

        public int? PayPeriodId { get; set; }
        public string PayPeriod { get; set; }

        public short? TempDeptId { get; set; } //Dropdown
        public string TempDepartmentCode { get; set; }
        public int? FundsId { get; set; }
        public int? ProjectsId { get; set; }
        public int? TaskId { get; set; }
        public int? TempJobId { get; set; }
        public string TempJobCode { get; set; }

        public short? EarningsCodeId { get; set; }
        public double? EarningsAmount { get; set; }

        [DataType(DataType.Date)]
        public DateTime ActualDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime Day { get; set; }

        public int ProjectNumber { get; set; }

        public double? DailyHours { get; set; }

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
