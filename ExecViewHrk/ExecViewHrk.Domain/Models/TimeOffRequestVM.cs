using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ExecViewHrk.Models
{
    public class TimeOffRequestVM
    {
        public TimeOffRequestVM()
        {
            RequestStatus = 0;
        }

        [DataType(DataType.Date)]
        public DateTime start { get; set; }

        [DataType(DataType.Date)]
        public DateTime end { get; set; }

        public int HoursCodeId { get; set; }
        public int CompanyCodeId { get; set; }
        public string HoursCodeCode { get; set; }
        public decimal TimeOfftHours { get; set; }
        public string test { get; set; }
        public byte RequestStatus { get; set; }
        public IList<TimeOffRequestStatusVM> EmpTimeOffRequest { get; set; }
    }
}
