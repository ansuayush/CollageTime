using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Models
{
    public class TimeOffVM
    {
        public IList<TimeOffEmpDetailsVM> TimeOffEmpDetailsList { get; set; }
        public IList<TimeOffTotalRequestsVM> TimeOffRequestsList { get; set; }
    }
}
