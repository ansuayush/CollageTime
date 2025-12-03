using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class TimeOffRequestStatusVm
    {
        public TimeOffRequestStatusVm()
        { }

        public TimeOffRequestStatusVm(DateTime requestedDate, short timeOffRequestStatus)
        {
            timeOffRequestDate = requestedDate;
            statusOfTimeOffRequest = timeOffRequestStatus;
        }

        [DataType(DataType.Date)]
        public DateTime timeOffRequestDate { get; set; }
        public short statusOfTimeOffRequest { get; set; }
    }
}