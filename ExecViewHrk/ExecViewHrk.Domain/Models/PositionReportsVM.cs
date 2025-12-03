using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExecViewHrk.Models
{
    public class PositionReportsVM
    {

        public string PositionCode { get; set; }
        public string positionTitle { get; set; }
        public string BuTitle { get; set; }
        public string BuCode { get; set; }
        public string jobCode { get; set; }
        public string JobTitle { get; set; }
        public string EmployeeName { get; set; }
        public string CompanyCodeDescription { get; set; }
        public string FileNumber { get; set; }
        public string Status { get; set; }
        public bool? primaryPosition { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? actualEndDate { get; set; }
        public string PayFrequencyCode { get; set; }
        public DateTime? EnteredDate { get; set; }
        public decimal? PayRate { get; set; }

        public DateTime? EffectiveDate { get; set; }

        public decimal? AnnualSalary { get; set; }

    }
}