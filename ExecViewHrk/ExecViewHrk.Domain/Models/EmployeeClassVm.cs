using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Models
{
    public class EmployeeClassVm
    {
        [Key]
        public int EmployeeClassId { get; set; }

        [StringLength(20)]
        public string ClassName { get; set; }

        [StringLength(50)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public bool Active { get; set; }
        public bool IsActive { get; set; }
    }
}
