using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class PosionFundingSource
    {
        public List<PositionFundingSourceGroupVM> posionFundingSourceList { get; set; }
        public List<PosionFundingSourceHistoryListVM> posionFundingSourceHistoryList { get; set; }
    }
}