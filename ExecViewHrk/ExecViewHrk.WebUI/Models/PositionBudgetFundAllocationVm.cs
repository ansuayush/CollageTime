using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class PositionBudgetFundAllocationVm
    {
        public decimal Budget { get; internal set; }
        public string FundCode { get; set; }
        public decimal PositionAllocation { get;  set; }
        public decimal TotalAllocation { get; set; }
        public decimal TotalFund { get;  set; }

        public int PositionFundID { get; set; }
    }
}