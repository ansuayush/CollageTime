using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class TimeCardApprovalReportVm
    {
        public int CompanyCodeId { get; set; }
        public string CompanyCodeDescription { get; set; }

        public short DepartmentId { get; set; }
        public string DepartmentDescription { get; set; }

        public int EmployeeId { get; set; }
        public string PersonName { get; set; }

        public int? PayPeriodId { get; set; }
        public string PayPeriod { get; set; }
        
        public double? RegularHours { get; set; }
        public double? CodedHours { get; set; }
        public double? OverTime { get; set; }
        public double? Emp_PayPeriodTotal { get; set; }
        public bool Approved { get; set; }
    }
}