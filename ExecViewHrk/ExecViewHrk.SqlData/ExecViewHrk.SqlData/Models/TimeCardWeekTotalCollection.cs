using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.SqlData.Models
{
    public class TimeCardWeekTotalCollection
    {
        public int WeekNum { get; set; }
        public double? RegularHours { get; set; }
        public double? CodedHours { get; set; }
        public double? OverTime { get; set; }
        public double? WeeklyTotal { get; set; }
    }
}
