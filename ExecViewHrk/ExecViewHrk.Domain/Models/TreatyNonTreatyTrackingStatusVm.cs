using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Domain.Models
{
    public class TreatyNonTreatyTrackingStatusVm
    {
        public int PayperiodNumber { get; set; }
        public int Paygroup { get; set; }
        public DateTime ExportedDate { get; set; }
        public bool Isexported { get; set; }
        public DateTime PayperiodStartDate { get; set; }
        public DateTime PayperiodEndDate { get; set; }
        public string FileNumber { get; set; }
        public decimal RegHours { get; set; }
        public decimal PayRate { get; set; }
    }
}
