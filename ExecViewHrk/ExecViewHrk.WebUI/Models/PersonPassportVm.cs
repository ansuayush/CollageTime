using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ExecViewHrk.Models;

namespace ExecViewHrk.WebUI.Models
{
    public class PersonPassportVm
    {
        public int PersonPassportId { get; set; }

        [Required]
        public int PersonId { get; set; }
        public string PersonName { get; set; }

        public int CountryId { get; set; }

        
        public string CountryDescription { get; set; }

        //public short CountryId { get; set; }

        //[Required]
        //[Display(Name = "Country Description")]
        //public string CountryDescription { get; set; }

        public string PassportNumber { get; set; }
        public string PassportStorage { get; set; }

        [Display(Name = "Issue Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? IssueDate { get; set; }

        [Display(Name = "Expiration Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? ExpirationDate { get; set; }

        [DataType(DataType.MultilineText)]
        public string EnteredBy { get; set; }   
        public DateTime? EnteredDate { get; set; }       
        public string ModifiedBy { get; set; }        
        public DateTime? ModifiedDate { get; set; }
        public List<DropDownModel> CountryDropDownList { get; set; }


    }
}