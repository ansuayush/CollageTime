using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ExecViewHrk.Models
{
    public class TimeOffTotalRequestsVM
    {
        [DataType(DataType.Date)]
        public DateTime timeOffRequestDate { get; set; }

        public int totalRequests { get; set; }
        public int pendingRequests { get; set; }
        public int approvedRequests { get; set; }
        public int disapprovedRequests { get; set; }
    }
}
