using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class PositionsFundHistoryVm
    {
        public decimal PositionAmount { get; internal set; }
        public string BudgetTitle { get; internal set; }
        public string FundCode { get; internal set; }
        public string FundDescription { get; internal set; }
        public int FundHistoryID { get; internal set; }
        public int ID { get; internal set; }
        public int PositionBudgetID { get; internal set; }
    }
}