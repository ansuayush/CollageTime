using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class SalaryGradeReloadGrid
    {
        public int ID { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public string validFrom { get; set; }
        public decimal? salaryMinimum { get; set; }
        public decimal? salaryMidpoint { get; set; }
        public decimal? salaryMaximum { get; set; }
        public bool active { get; internal set; }
    }
}