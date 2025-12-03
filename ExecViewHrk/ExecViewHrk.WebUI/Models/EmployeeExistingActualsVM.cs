using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class EmployeeExistingActualsVM
    {
        public int ID { get; set; }
        public int EmployeeID { get; set; }
        public int PositionID { get; set; }
        public DateTime PayPeriodDate { get; set; }
        public decimal ActualPay { get; set; }
        public string CompanyCode { get; set; }
        public string FileNumber { get; set; }
        public DateTime PayCheckEndDate { get; set; }
        public decimal Overtime { get; set; }
        public string JobCode { get; set; }
        public string DepartmentCode { get; set; }
        public string LocationCode { get; set; }
        public decimal Bonus { get; set; }
        public decimal DetailPay { get; set; }
        public decimal AdditionalDuties { get; set; }
    }
}