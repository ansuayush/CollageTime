using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.EfClient
{
    public partial class EmployeesAllowedTakens
    {
        [Key]
        public int EmployeesAllowedTakenId { set; get; }
        public int EmployeeId { set; get; }
        public int CompanyCodeId { set; get; }
        public string FileNumber { set; get; }
        public int TypeId { set; get; }
        public decimal Allowed { set; get; }
        public decimal Taken { set; get; }
        public decimal Remainder { set; get; }

        //public virtual HoursCode HoursCode { get; set; }
    }
}
