using System.Collections.Generic;

namespace ExecViewHrk.Models
{
    public class PositionClosedReportVm
    {

        public string PositionCode { get; set; }
        public string Code { get; set; }

        public string PositionTitle { get; set; }

        public string PositionStatus { get; set; }
        public string JobCode { get; set; }
        public string JobTitle { get; set; }
        public string BusinessUnitCode { get; set; }
        public string BusinessUnitTitle { get; set; }        
        public string Notes { get; set; }
        public string BudgetAmount { get; set; }      
        public List<DropDownModel> lstBudgetYears { get; set; }
        public int budgetYear { get; set; }
    }
}