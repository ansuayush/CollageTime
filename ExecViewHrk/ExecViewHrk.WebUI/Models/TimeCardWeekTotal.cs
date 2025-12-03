using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class TimeCardWeekTotal
    {
        public int WeekNum { get; set; }
        public double? RegularHours { get; set; }
        public double? CodedHours { get; set; }
        public double? OverTime { get; set; }
        public double? WeeklyTotal { get; set; }
    }
}