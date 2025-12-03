using System;

namespace ExecViewHrk.Models
{
    public class ContractEarnBurnReport
    {
        public string FundingSource { get; set; }
        public string FundingSourceDescription { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string FundingSourceComments { get; set; }
        public string BudgetCenter { get; set; }
        public string BudgetCenterDescription { get; set; }
        public decimal ContractAmount { get; set; }
        public decimal AmountSpent { get; set; }
        public decimal BalanceRemaining { get; set; }
        public string TimeElapsed { get; set; }
        public string ContractSpent { get; set; }
        public string BudgetComments { get; set; }
    }
}
