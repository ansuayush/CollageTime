using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class EmployeeActualsVM
    {
        public string EmployeeName { get; set; }
        public string CompanyCode { get; set; }
        public string FileNumber { get; set; }
        public DateTime PayPeriodDate { get; set; }
        public DateTime? PayCheckEndDate { get; set; }
        public decimal GrossAmount { get; set; }
        public string PayrollDepartmentNumber { get; set; }
        public string CostNumber { get; set; }
        public string PositionCode { get; set; }
        public string PositionTitle { get; set; }
    }
}