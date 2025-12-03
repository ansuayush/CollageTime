using System;
using System.Collections.Generic;
using System.Linq;


namespace ExecViewHrk.EfClient
{
    using System;
    using System.ComponentModel.DataAnnotations;
    public partial class PositionFund
    {   [Key]
        public int PositionFundID { get; set; }
        public int PositionBudgetID { get; set; }
        public int FundID { get; set; }
        public decimal Amount { get; set; }
    }
}

