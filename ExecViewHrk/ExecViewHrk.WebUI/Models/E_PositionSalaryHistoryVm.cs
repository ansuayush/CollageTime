using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

//E_PositionSalaryHistory View Model
namespace ExecViewHrk.WebUI.Models
{
    public class E_PositionSalaryHistoryVm
    {
        public int E_PositionSalaryHistoryId { get; set; }

        public int E_PositionId { get; set; }
        public int? RateTypeId { get; set; }
        public string RateTypeDescription { get; set; }
        public decimal? PayRate { get; set; }
        public decimal? HoursPerPayPeriod { get; set; }
        public decimal? AnnualSalary { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EffectiveDate { get; set; }

        public string Notes { get; set; }

        public string EnteredBy { get; set; }

        public DateTime? EnteredDate { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public decimal? AverageSalary { get; set; }

        public string EmpName { get; set; }

        public decimal? SalaryVariance { get; set; }

        public decimal? BudgetSalary { get; set; }
        public DateTime? EndDate { get; set; }
    }
}