using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class TimeOffRequestVm
    {
        public TimeOffRequestVm()
        {
            RequestStatus = 0;
        }

        [DataType(DataType.Date)]
        public DateTime start { get; set; }

        [DataType(DataType.Date)]
        public DateTime end { get; set; }
        public string test { get; set; }
        public byte RequestStatus { get; set; }
        public IList<TimeOffRequestStatusVm> EmpTimeOffRequest { get; set; }

        //IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    var pEnd = new[] { "end" };
        //    if (end < start)
        //    {
        //        yield return new ValidationResult("End Date must be greater than Start Date", pEnd);
        //    }
        //}
    }
}