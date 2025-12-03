using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class PosionFundingSourceHistoryListVM
    {   
        public DateTime EffectiveDate { get; set; }
        public DateTime ChangeEffectiveDate { get; set; }
        public string FundCodeID { get; set; }
        public string Percentage { get; set; }
    }
}