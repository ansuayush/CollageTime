namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    public partial class PositionBudgets
    {
        public int ID { get; set; }
        public int PositionID { get; set; }
        [Required]
        public int BudgetYear { get; set; }
        [Required]
        public byte BudgetMonth { get; set; }
        [Required]
        public decimal BudgetAmount { get; set; }
        public decimal BurdenPercent { get; set; }
        public int? EmployeeID { get; set; }
        [Required]
        public decimal FTE { get; set; }

    }
}
