using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ExecViewHrk.Models
{
    public class TimeOffRequestStatusVM
    {
        public TimeOffRequestStatusVM()
        { }

        public TimeOffRequestStatusVM(DateTime requestedDate, short timeOffRequestStatus)
        {
            timeOffRequestDate = requestedDate;
            statusOfTimeOffRequest = timeOffRequestStatus;
        }

        [DataType(DataType.Date)]
        public DateTime timeOffRequestDate { get; set; }
        public short statusOfTimeOffRequest { get; set; }
    }
}
