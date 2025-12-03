using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;



namespace ExecViewHrk.EfClient
{
  public  class employeeSalaryComponents
    {
        [Key]
        public int id { get; set; }

        public int employeeID { get; set; }

        public DateTime? startDate { get; set; }
        public DateTime? expirationDate { get; set; }
        public DateTime? enteredDate { get; set; }
      
        public Decimal? amount { get; set; }
        public  string enteredBy { get; set; }
        public string payFrequencyCode { get; set; }
        
        public bool? total { get; set; }
        public   bool? benefits { get; set; }

        public bool? linkToPayroll { get; set; }


        public int SalaryComponentID { get; set; }
        public int salaryComponentTypeID { get; set; }
        public int payTypeID { get; set; }
        public int PayFrequencyId { get; set; }
        public bool? Base { get; set; }
    public virtual ddlSalaryComponents DdlSalaryComponents { get; set; }
    }
}
