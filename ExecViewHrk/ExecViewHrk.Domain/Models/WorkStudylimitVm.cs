using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Models
{
   public class WorkStudylimitVm
    {
        public string EmployeeName { get; set; }
        public int PersonID { get; set; }
        public string companyCode { get; set; }
        public string filenumber { get; set; }
        public string PositionCode { get; set; }
        public string PositionTitle { get; set; }
        public decimal? UsedAmount { get; set; }
        public decimal? RemainingBalance { get; set; }
        public Nullable<decimal> workstudylimit { get; set; }
       
    }
}
