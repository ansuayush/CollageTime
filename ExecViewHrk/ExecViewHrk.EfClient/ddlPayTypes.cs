using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ExecViewHrk.EfClient
{
  public class ddlPayTypes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ddlPayTypes()
        {
            EmployeeSalaryComponents = new HashSet<employeeSalaryComponents>();
        }
        [System.ComponentModel.DataAnnotations.Key]
        public int payTypeID { get; set; }

        public string description { get; set; }

        public string code { get; set; }
      
        public bool? active { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<employeeSalaryComponents> EmployeeSalaryComponents { get; set; }
    }
}
