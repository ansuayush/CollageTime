using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class TimeOffTotalRequests
    {
        [DataType(DataType.Date)]
        public DateTime timeOffRequestDate { get; set; }

        public int totalRequests { get; set; }
        public int pendingRequests { get; set; }
        public int approvedRequests { get; set; }
        public int disapprovedRequests { get; set; }
        //public short statusOfTimeOffRequest { get; set; }
    }
}