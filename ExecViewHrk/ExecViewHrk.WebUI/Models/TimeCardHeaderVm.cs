using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class TimeCardHeaderVm
    {
        public int CompanyCodeId { get; set; }
        public string CompanyCodeDescription { get; set; }

        public short DepartmentId { get; set; }
        public string DepartmentDescription { get; set; }

        public int EmployeeId { get; set; }
        public string EmployeeFullName { get; set; }

        public int? PayPeriodId { get; set; }
        public string PayPeriod { get; set; }

        public bool Approved { get; set; }
    }
}