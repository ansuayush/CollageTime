namespace ExecViewHrk.EfClient
{
    using System;
    using System.ComponentModel.DataAnnotations;
    public class Funds
    {    [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Description { get; set; }
        [Required]
        [StringLength(10)]
        public string Code { get; set; }
        public bool Active { get; set; }
        public decimal? Amount  { get; set; }

        public decimal? FTE { get; set; }
        public DateTime? EffectiveStartDate { get; set; }
        public DateTime? EffectiveEndDate { get; set; }



    }
    public class FundHistory
    {    [Key]
        public  int ID { get; set; }
        public int FundID { get; set;}
        public DateTime? EffectiveDate { get; set; }
        public Decimal Amount { get; set; }
    }
}
