namespace ExecViewHrk.EfClient
{
    using System.ComponentModel.DataAnnotations;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    public partial class PositionFundingSource
    {
        [Key]
        public int PositionFundingSourceID { get; set; }
        public int FundCodeID { get; set; }
        public DateTime EffectiveDate { get; set; }
        public int Percentage { get; set; }
        public int PositionId { get; set; }
        public string FundingGroup { get; set; }
       
    }
}
