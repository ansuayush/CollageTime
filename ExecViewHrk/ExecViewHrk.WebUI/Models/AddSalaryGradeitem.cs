using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class AddSalaryGradeitem
    {
        public string  description { get; set; }
        public System.DateTime? validDate { get; set; }
        public decimal salaryMin { get; set; }
        public decimal salaryMid { get; set; }
        public decimal salaryMax { get; set; }
        public short GridID { get; set; }
    }
}