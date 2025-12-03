using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ExecViewHrk.EfClient;
namespace ExecViewHrk.Models
{
    public class SalaryComponentViewModel
    {
        public int id { get; set; }

        public int employeeID { get; set; }

        public int? SelectedEmployeeID { get; set; }

        public DateTime? startDate { get; set; }
        public DateTime? expirationDate { get; set; }
        public DateTime enteredDate { get; set; }

        public Decimal? amount { get; set; }
        public string enteredBy { get; set; }
        public string payFrequencyCode { get; set; }

        public bool? total { get; set; }
        public bool? benefits { get; set; }

        public bool? linkToPayroll { get; set; }
       

        public int SalaryComponentID { get; set; }
        public int SelectSalaryComponentID { get; set; }
        public int PayFrequencyId { get; set; }
        public int salaryComponentTypeID { get; set; }
        public int payTypeID { get; set; }
        public string SalaryComponentTitle { get; set; }
        public string salaryComponentTypeDescription { get; set; }
        public string payTypeDescription { get; set; }
        public bool? Base { get; set; }
        public List<ddlSalaryComponents> SalaryComponentsList { get; set; }
        public List<DdlPayFrequency> PayFrequencyList { get; set; }
        public List<ddlPayTypes> PayTypeyList { get; set; }
        public List<DropDownModel> EmployeeList { get; set; }

    }
}