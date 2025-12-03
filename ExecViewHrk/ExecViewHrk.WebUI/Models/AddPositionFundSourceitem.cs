using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class AddPositionFundSourceitem
    {
        public int PositionFundingSourceID { get; set; }
        public System.DateTime EffectiveDate { get; set; }
        public byte FundCodeID { get; set; }
        public byte Percentage { get; set; }
    }
}