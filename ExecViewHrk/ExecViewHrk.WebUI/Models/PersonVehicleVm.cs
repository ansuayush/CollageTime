using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ExecViewHrk.Models;

namespace ExecViewHrk.WebUI.Models
{
    public class PersonVehicleVm
    {
        public int PersonVehicleId { get; set; }

        [Required]
        public int PersonId { get; set; }
        public string PersonName { get; set; }

        [Required]
        [Display(Name = "License Number")]
        public string LicenseNumber { get; set; }    
        public string Make { get; set; }     
        public string Model { get; set; }   
        public string Color { get; set; }

        public byte? StateId { get; set; }

        [Display(Name = "State Title")]
        public string StateTitle { get; set; }

        [Display(Name = "Acquisition Date")]
        public DateTime? AcquisitionDate { get; set; }

        [Display(Name = "Sold Date")]
        public DateTime? SoldDate { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        public string EnteredBy { get; set; }     
        public DateTime? EnteredDate { get; set; }
        public string ModifiedBy { get; set; }       
        public DateTime? ModifiedDate { get; set; }
        public List<DropDownModel> StateDropDownList { get; set; }
    }
}