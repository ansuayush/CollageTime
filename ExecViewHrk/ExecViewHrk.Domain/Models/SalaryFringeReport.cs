using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Models
{
    public class SalaryFringeReport
    {
        public string BudgetCenter { get; set; }
        public string BudgetCenterDescription { get; set; }
        public string FundingSource { get; set; }
        public string FundingSourceDescription { get; set; }
        public string Resource { get; set; }
        public Nullable<System.DateTime> PeriodStart { get; set; }
        public Nullable<System.DateTime> PeriodEnd { get; set; }
        public decimal Hours { get; set; }
        public decimal Salary { get; set; }
        public decimal FICA { get; set; }
        public decimal C401K { get; set; }
        public decimal Medical { get; set; }
        public decimal Dental { get; set; }
        public decimal C403b { get; set; }
        public decimal LifeIns { get; set; }
        public decimal Disability { get; set; }
        public decimal TotalEmployerCost { get; set; }
    }
}
