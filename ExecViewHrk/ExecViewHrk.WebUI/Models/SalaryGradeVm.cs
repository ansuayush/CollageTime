using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExecViewHrk.EfClient;
namespace ExecViewHrk.WebUI.Models
{
    public class SalaryGradeVm
    {
        public int ID { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public DateTime? validFrom { get; set; }
        public Decimal? salaryMinimum { get; set; }
        public Decimal? salaryMidpoint { get; set; }
        public Decimal? salaryMaximum { get; set; }
        public bool active { get; internal set; }
        public int? SalaryGradeID { get; internal set; }
    }
}