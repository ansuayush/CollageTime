using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ExecViewHrk.WebUI.Models
{
    public class PersonAdditionalVm
    {
        public int PersonAdditionalId { get; set; }

        [Required]
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        
        public int? EeoTypeId { get; set; }

        [Required]
        public string EeoDescription { get; set; }

        public int? ApplicantSourceId { get; set; }

        [Required]
        public string ApplicantDescription { get; set; }

        public int? CitizenshipId { get; set; }

        [Required]
        public string CitizenshipDescription { get; set; }

        public int? HospitalId { get; set; }
        [Required]
        public string HospitalDescription { get; set; }

        public string BirthPlace { get; set; }
        public DateTime? CitizenshipDate { get; set; }
        public string Veteran { get; set; }
        public bool Other { get; set; }
        public bool SpecialDisabled { get; set; }
        public bool Vietnam { get; set; }

        public bool Disabled { get; set; }
        [DataType(DataType.MultilineText)]
        public string DisabledComments { get; set; }
        public string Doctor { get; set; }
        public bool BloodDonor { get; set; }
        public bool Smoker { get; set; }
        public bool AdvancedDirective { get; set; }

      
        public DateTime? EarlyRetirementDate { get; set; }
        public DateTime? NormalRetirementDate { get; set; }
        public DateTime? ExpectedRetirementDate { get; set; }
        public DateTime? DateOfDeath { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }      
        public string EnteredBy { get; set; }       
        public DateTime? EnteredDate { get; set; }       
        public string ModifiedBy { get; set; }       
        public DateTime? ModifiedDate { get; set; }
    }
}