using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class TimeOffEmpDetailsVm
    {
        public TimeOffEmpDetailsVm()
        { }

        [DataType(DataType.Date)]
        public DateTime TimeOffRequest { get; set; }
        public int TimeOffRequestId { get; set; }
        public int EmployeeId {get; set;}
        public string PersonName { get; set;}
        public byte RequestStatus { get; set; }
        //public EnumRequestStatusTypes StatusType { get; set; }
        //public int StatusType { get; set; }
        //public List<RequestStatus> RequestStatusTypeList { get; set; }
    }
}