using ExecViewHrk.Models;
using System.Collections.Generic;

namespace ExecViewHrk.Models
{
    public class BudgetToActualsReportVm
    {
        public bool PrimaryPosition { get; set; }
        public string PositionCode { get; set; }
        public string PositionTitle { get; set; }
        public string PositionStatus { get; set; }
        public decimal BudgetAmount { get; set; }
        public decimal BudgetToDate { get; set; }
        public decimal ActualPay { get; set; }
        public decimal Variance { get; set; }
        public string BusinessUnitCode { get; set; }
        public string BusinessUnitTitle { get; set; }
        public string JobCode { get; set; }
        public string JobTitle { get; set; }
        public int budgetMonth { get; set; }
        public int budgetYear { get; set; }
        public string Code { get; set; }
        public List<DropDownModel> lstBudgetMonths { get; set; }
        public List<DropDownModel> lstBudgetYears { get; set; }
    }
}