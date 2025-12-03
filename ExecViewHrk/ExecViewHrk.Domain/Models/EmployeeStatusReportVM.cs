using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace ExecViewHrk.Models
{
    public class EmployeeStatusReportVM
    {
        public string EmployeeName { get; set; }
        public int PersonID { get; set; }
        public string CompanyCodeDescription { get; set; }
        public string Code { get; set; }
        public string filenumber { get; set; }
        public string Status { get; set; }
        public string PositionCode { get; set; }
        public string positionTitle { get; set; }
        public string BuTitle { get; set; }
        public string BuCode { get; set; }
        public string jobCode { get; set; }
        public string JobTitle { get; set; }
        public bool? primaryPosition { get; set; }
        public DateTime? StartDate { get; set; }
        // public int? rateTypeCode { get; set; }
        public DateTime? EndDate { get; set; }
        public string PayFrequencyCode { get; set; }
        public DateTime? EnteredDate { get; set; }
        public decimal? PayRate { get; set; }
        public decimal? HoursPerPayPeriod { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public decimal? AnnualSalary {get;set;}
    }
}