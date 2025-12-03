using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.EfClient
{
    public partial class PositionBudgetSchedules
    {
        public int ID { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? EligibleDate { get; set; }
        public byte? IncreaseType { get; set; }
        public decimal? IncreaseAmount { get; set; }
        public decimal? WashoutRule { get; set; }
        public decimal? WashoutRuleSalary { get; set; }
        public bool? AutoFill { get; set; }
        public byte? ScheduleType { get; set; }

    }
}
