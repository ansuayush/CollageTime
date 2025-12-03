using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ExecViewHrk.WebUI.Models
{
    public class PositionBudgetMonthsVM
    {
        public int ID { get; set; }

        //public int PositionBudgetsID { get; set; }

        //public byte BudgetMonth { get; set; }

        public decimal BudgetAmount { get; set; }

        //public byte DisplayPosition { get; set; }
        public string Month { get;  set; }
    }
}

