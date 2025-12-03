using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Models
{
    public class JobsVM
    {
        public int JobId { get; set; }
        public string JobCode { get; set; }
        public int TempJobId { get; set; }
        public string TempJobCode { get; set; }
    }
}
