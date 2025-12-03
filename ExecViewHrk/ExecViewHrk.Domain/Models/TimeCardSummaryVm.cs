using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Models
{
    public class TimeCardSummaryVm
    {
        public int TimeCardId { get; set; }

        public int EmployeeId { get; set; }

        public DateTime ActualDate { get; set; }

        public DateTime? TimeIn { get; set; }

        public DateTime? TimeOut { get; set; }

        public DateTime? LunchOut { get; set; }

        public DateTime? LunchBack { get; set; }

        public double DailyHours { get; set; }

        public double Hours { get; set; }

        public string Title { get; set; }

        public int PayPeriodId { get; set; }

        public int PositionId { get; set; }
    }
}
