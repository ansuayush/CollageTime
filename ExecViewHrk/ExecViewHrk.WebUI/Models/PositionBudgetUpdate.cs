using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class PositionBudgetUpdate
    {
        public int GridRowID { get; set; }
        public decimal GridRowFTE { get; set; }
        public int GridRowYear { get; set; }
        public byte GridRowMonth { get; set; }
        public decimal GridRowamount { get; set; }
    }
}