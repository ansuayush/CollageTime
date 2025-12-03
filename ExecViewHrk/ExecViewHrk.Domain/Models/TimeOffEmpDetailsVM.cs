using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ExecViewHrk.Models
{
    public class TimeOffEmpDetailsVM
    {
        public TimeOffEmpDetailsVM()
        { }

        [DataType(DataType.Date)]
        public DateTime TimeOffRequest { get; set; }
        public DateTime TimeOffDate { get; set; }
        public int TimeOffRequestId { get; set; }
        public int EmployeeId { get; set; }
        public int HoursCodeId { get; set; }
        public int CompanyCodeId { get; set; }
        public string HoursCodeCode { get; set; }
        public decimal TimeOfftHours { get; set; }
        public string PersonName { get; set; }
        public byte RequestStatus { get; set; }
    }
}
