using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ExecViewHrk.Models;

namespace ExecViewHrk.WebUI.Models
{
    public class PersonInnoculationVm
    {
        public int PersonInnoculationId { get; set; }

        [Required]
        public int PersonId { get; set; }
        public string PersonName { get; set; }

        public int InnoculationTypeId { get; set; }

        //[Required]
        //[Display(Name = "Innoculation Description")]
        public string InnoculationDescription { get; set; }

        [Display(Name = "Innoculation Date")]
        public DateTime? InnoculationDate { get; set; }

        [Display(Name = "Expiration Date")]
        public DateTime? ExpirationDate { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        public string EnteredBy { get; set; }
        public DateTime? EnteredDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<DropDownModel> InnoculationDropDownList { get; set; }
    }
}