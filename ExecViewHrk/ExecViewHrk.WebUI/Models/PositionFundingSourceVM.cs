using ExecViewHrk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class PositionFundingSourceVM
    {
        public int EditPositionFundingSourceID { get; set; }
        public byte EditFundCodeID { get; set; }

        public DateTime EditEffectiveDate { get; set; }
        public byte EditPercentage { get; set; }
        public List<DropDownModel> DDlFundCode { get; set; }
        public List<PosionFundingSourceListVM> EditposionFundingSourceList { get; set; }
    }

    public class EditPositionSalaryGrad
    {
        public string EditPositionSalaryDescription { get; set; }
        public Decimal? EditPositionSalarySalaryMax { get; set; }
        public Decimal? EditPositionSalarySalaryMin { get; set; }
        public Decimal? EditPositionSalarySalaryMad { get; set; }
        public DateTime? EditPositionSalaryVadidDate { get; set; }
        public List<DropDownModel> DDlSalaryGrad { get; set; }
        public short EditSalaryGradeGridID { get; set; }
    }

    // Modification
    public class PositionFundingSourceGroupVM
    {
        public DateTime? EffectiveDate { get; set; }
        public string EditFundCodeID { get; set; }
        public DateTime? EditEffectiveDate { get;set; }
        public byte EditPercentage { get; set; }
        public List<DropDownModel> FundCodes { get; set; }
        public string FundingGroup { get; set; }
        public List<PosionFundingSourceListVM> EditposionFundingSourceList { get; set; }
        public int Percentage { get; internal set; }
        public short PositionId { get; set; }
    }

}