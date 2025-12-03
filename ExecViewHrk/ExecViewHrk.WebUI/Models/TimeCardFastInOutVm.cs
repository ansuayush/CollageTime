using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class TimeCardFastInOutVm
    {        
        public IList<TimeCardRecordVm> EmpTimeCard_List { get; set; }
        public TimeCardRecordVm newTimeCardRecord {get;set;}
    }
}