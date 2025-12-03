using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ExecViewHrk.EfClient;
using ExecViewHrk.Models;

namespace ExecViewHrk.WebUI.Models
{
    public class PositionBudgetsVM
    {
        public int ID { get; set; }
        [Required]
        [Display(Name ="Position Title")]
        public int PositionID { get; set; }
        [Required]
        [Display(Name = "Budget Year")]
        public int BudgetYear { get; set; }

        [Required]
        [Display(Name = "Budget Month")]
        public byte BudgetMonth { get; set; }
        [Required]
        [Display(Name = "Budget Amount")]
        public decimal? BudgetAmount { get; set; }
        public decimal? BurdenPercent { get; set; }
        public int? EmployeeID { get; set; }

        [Required]
        public decimal? FTE { get; set; }
        [Display(Name = "Budget Month")]
        public string BudgetMonthText { get; set; }
        public bool isNewRecord { get; set; }

        public string PositionTitle { get; set; }

        public List<DropDownModel> lstMonths { get; set; }
        public List<DropDownModel> lstyears { get; set; }
        public List<PositionBudgetFundAllocationVm> BudgetFundAllocations { get; set; }
        public List<DropDownModel> lstPositionIDs { get; set; }
        public List<DropDownModel> MonthList { get; internal set; }
        public List<DropDownModel> YearList { get; internal set; }
        public List<PositionBudgetMonthsVM> BudgetMonthList { get; internal set; }
        //  public List<PositionBudgetMonthsVM> lstPositionBudgetMonths { get; set; }

    }
}