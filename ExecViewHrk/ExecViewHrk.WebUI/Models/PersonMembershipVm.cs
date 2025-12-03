using ExecViewHrk.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class PersonMembershipVm
    {
        public int PersonMembershipId { get; set; }

        [Required]
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        [Required(ErrorMessage = "The Professional Body field is required.")]
        public int ProfessionalBodyId { get; set; }

        public string ProfessionalBodyDescription { get; set; }
        [Required(ErrorMessage = "The Start Date field is required.")]
        public DateTime? StartDate { get; set; }
        [Required(ErrorMessage ="The Renewal Date field is required.")]
        public DateTime? RenewalDate { get; set; }
        [Required]
        public string Number { get; set; }

        public decimal? Fee { get; set; }
        [Required(ErrorMessage ="The Fee Paid Date field is required.")]
        public DateTime? FeePaidDate { get; set; }

        public string ProfessionalTitle { get; set; }
        [Required(ErrorMessage = "The Regional Chapter field is required.")]
        public int? RegionalChapterId { get; set; }
        public string RegionalChapterDescription { get; set; }

        public string RegionalChapter { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        public string EnteredBy { get; set; }
        public DateTime? EnteredDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<DropDownModel> ProfessionalBodyDropDownList { get; set; }
        public List<DropDownModel> RegionalChapterDropDownList { get; set; }
    }
}