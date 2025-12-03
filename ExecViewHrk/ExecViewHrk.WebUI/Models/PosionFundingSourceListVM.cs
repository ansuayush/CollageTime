using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class PosionFundingSourceListVM
    {
        public string ID { get; set; }//for Edit the record
        public DateTime EffectiveDate { get; set; }
        public string FundCode { get; set; }
        public decimal FundPercentage { get; set; }
        public int FundCodeID { get; set; }
        public int PositionId { get; set; }
        public string FundingGroup { get; set; }
    }
}