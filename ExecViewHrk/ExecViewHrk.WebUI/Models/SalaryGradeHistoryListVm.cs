using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class SalaryGradeHistoryListVm
    {
        public string description { get; set; }
        public DateTime? validFrom { get; set; }
        public string salaryMinimum { get; set; }
        public string salaryMidpoint { get; set; }
        public string salaryMaximum { get; set; }
        public DateTime ChangeDate { get; set; }
    }
}