using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.EfClient
{
   public partial class PositionBudgetMonths
    {

        public int ID { get; set; }

        public int PositionBudgetsID { get; set; }

        public byte BudgetMonth { get; set; }

        public decimal BudgetAmount { get; set; }

        public byte DisplayPosition { get; set; }
    }
}
