using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ExecViewHrk.Models;

namespace ExecViewHrk.WebUI.Models
{
    public class PersonLicenseVm
    {

        public int PersonLicenseId { get; set; }

        [Required]
        public int PersonId { get; set; }
        public string PersonName { get; set; }

        public byte LicenseTypeId { get; set; }


        public string LicenseDescription { get; set; }

        [Display(Name = "License Number")]
        public string LicenseNumber { get; set; }

        public byte StateId { get; set; }


        [Display(Name = "State Title")]
        public string StateTitle { get; set; }

        public int CountryId { get; set; }


        [Display(Name = "Country Description")]
        public string CountryDescription { get; set; }


        [Required]
        [Display(Name = "Expiration Date")]
        public DateTime? ExpirationDate { get; set; }

        [Display(Name = "Revoked Date")]
        public DateTime? RevokedDate { get; set; }

        [Display(Name = "Reinstated Date")]
        public DateTime? ReinstatedDate { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        public string EnteredBy { get; set; }
        public DateTime? EnteredDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<DropDownModel> LicenseDropDownList { get; set; }
        public List<DropDownModel> StateDropDownList { get; set; }
        public List<DropDownModel> CountryDropDownList { get; set; }
    }
}