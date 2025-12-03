using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.SqlData.Models
{
    public class TimeCardSemiMonthlyCollection
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

        public short? TempDeptId { get; set; }
        public string TempDepartmentCode { get; set; }

        public int? TempJobId { get; set; }
        public string TempJobCode { get; set; }

        public short? EarningsCodeId { get; set; }

        public double? EarningsAmount { get; set; }
       
        public DateTime ActualDate { get; set; }

        public DateTime Day { get; set; }

        public int ProjectNumber { get; set; }

        public double? DailyHours { get; set; }

        public int? HoursCodeId { get; set; }

        public double? Hours { get; set; }

        public bool Approved { get; set; }

        public bool IsLineApproved { get; set; }

        public bool ShowLineApprovedActive { get; set; }

        public int WeekNum { get; set; }

        public double? LineTotal { get; set; }

        public bool IsArchived { get; set; }

    }
}
