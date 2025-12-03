
namespace ExecViewHrk.EfClient
{
    using System;
    using System.ComponentModel.DataAnnotations;
    public partial class PositionsFundHistory
    {
        public int ID { get; set; }
        public int PositionBudgetID { get; set; }
        public int FundHistoryID { get; set; }
        public decimal PositionAmount { get; set; }
    }
}
