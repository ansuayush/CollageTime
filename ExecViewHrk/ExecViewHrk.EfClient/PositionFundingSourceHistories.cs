namespace ExecViewHrk.EfClient
{
    using System.ComponentModel.DataAnnotations;
    using System;
    public partial class PositionFundingSourceHistories
    {
        [Key]
        public int FundingSourceHistoriesID { get; set; }
        public DateTime EffectiveDate { get; set; }
        public int FundCodeID { get; set; }
        public int Percentage { get; set; }
        public DateTime ChangeEffectiveDate { get; set; }
        public int PositionFundingSourceID { get; set; }
    }
}
