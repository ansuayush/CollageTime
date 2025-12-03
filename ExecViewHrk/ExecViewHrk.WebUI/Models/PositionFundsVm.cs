using ExecViewHrk.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class PositionFundsVm
    {
        [Required]
        public int FundID { get;  set; }
        [Required]
        public decimal Amount { get;  set; }
        public int PositionBudgetID { get;  set; }
        public List<DropDownModel> FundCodes { get; set; }
        public string BudgetTitle { get; internal set; }
        public string FundCode { get; internal set; }
        public string FundDescription { get; internal set; }
        public int PositionID { get; internal set; }
        public int PositionFundID { get; set; }
    }
}