using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ExecViewHrk.Models
{
    public class PositionBudgetSchedulesVM
    {
        public int ID { get; set; }
        [Required]
        public DateTime? EffectiveDate { get; set; }
        [Required]
        public DateTime? StartDate { get; set; }
        [Required]
        public DateTime? EndDate { get; set; }
        [Required]
        public DateTime? EligibleDate { get; set; }
        [Required]
        public byte? IncreaseType { get; set; }
        [Required]
        public decimal? IncreaseAmount { get; set; }
        [Required]
        public decimal? WashoutRule { get; set; }
        [Required]
        public decimal? WashoutRuleSalary { get; set; }    
        public bool? AutoFill { get; set; }
        [Required]
        public byte? ScheduleType { get; set; }
        public string AutoFillYesNo { get; set; }
        public string IncreaseTypeText { get; set; }
        public string ScheduleTypeText { get; set; }
       // public DateTime Year { get; set; }


    }
}