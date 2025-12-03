using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Models
{
    public class TreatyReportVm
    {
        public string EmployeeName { get; set; }
        public int PersonID { get; set; }
        public string companyCode { get; set; }
        public string filenumber { get; set; }
        public decimal? TreatyLimit { get; set; }
        public decimal? UsedAmount { get; set; }
        public decimal? RemainingAmount { get; set; }
    }

}
