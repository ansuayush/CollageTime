using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ExecViewHrk.EfClient;

namespace ExecViewHrk.Models
{
    public class PersonTestVm
    {
        public int PersonTestId { get; set; }

        [Required]
        public int PersonId { get; set; }
        public string PersonName { get; set; }

        [Required]
        [Display(Name = "Evaluation Test")]
        public int? EvaluationTestId { get; set; }
        
        public string EvaluationTestDescription { get; set; }
        [Required]
        [Display(Name = "Examination Date")]
        public DateTime? TestDate { get; set; }
        public string Score { get; set; }
        public string Grade { get; set; }
        public string Administrator { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        public string EnteredBy { get; set; }
        public DateTime EnteredDate { get; set; }
        public List<DropDownModel> EvaluationTestDropDownList { get; set; }
    }
}