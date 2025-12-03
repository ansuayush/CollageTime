using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class TimeCardRecordVm
    {
        //public DateTime TimeIn { get {return DateTime.Now ;} }
        public int TimeCardId { get; set; }
        public short? CompanyCodeId { get; set; }
        public int EmployeeId { get; set; }
        public int ProjectNumber { get; set; }
        public DateTime? TimeIn { get; set; }
        public DateTime? LunchOut { get; set; }
        public DateTime? LunchBack { get; set; }
        public DateTime? TimeOut { get; set; }
        public DateTime ActualDate { get; set; }
    }
}