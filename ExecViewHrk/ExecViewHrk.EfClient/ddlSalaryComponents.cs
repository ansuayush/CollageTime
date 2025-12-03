using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.EfClient
{
  public class ddlSalaryComponents
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ddlSalaryComponents()
        {
            EmployeeSalaryComponents = new HashSet<employeeSalaryComponents>();
        }
        [System.ComponentModel.DataAnnotations.Key]
        public int salaryComponentID { get; set; }

  

      
        public string description { get; set; }

        public string code { get; set; }
        public string payFrequencyCode { get; set; }

        public bool? total { get; set; }
        public bool? active { get; set; }

        public bool? benefits { get; set; }
        public bool? Base { get; set; }

        public int SalaryComponentID { get; set; }
        public int salaryComponentTypeID { get; set; }
        public int payTypeID { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<employeeSalaryComponents> EmployeeSalaryComponents { get; set; }
    }
}
