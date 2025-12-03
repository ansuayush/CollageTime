using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ExecViewHrk.EfClient;
using ExecViewHrk.Models;

namespace ExecViewHrk.WebUI.Models
{
    public class PersonBasicVm
    {

        public int PersonId { get; set; }

        //[Required]
        [StringLength(9, MinimumLength = 9,ErrorMessage="SSN must be 9 digits")]
        [Display(Name = "SSN")]
        public string Ssn { get; set; }
        
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string PreferredName { get; set; }

        public string MaidenName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [EmailAddress]
        public string AlternateEmail { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public int? PrefixId { get; set; }
        public int? SuffixId { get; set; }
        public int? GenderId { get; set; }
        public int? MaritalStatusId { get; set; }

        public bool? IsDependent { get; set; }

        public string EnteredBy { get; set; }
        public DateTime? EnteredDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }


        public List<DropDownModel> GenderList { get; set; }
        public List<DropDownModel> PrefixList { get; set; }
        public List<DropDownModel> SuffixList { get; set; }
        public List<DropDownModel> MaritailStatusList { get; set; }

        public string Gender { get; set; }
        public string MaritialStatus { get; set; }

        public List<PersonPhoneNumberVm> personPhoneNumberVm { get; set; }

        public List<PersonAddressVm> personAddressVm { get; set; }
        public byte[] PersonImage { get; set; }

        public bool? IsStudent { get; set; }
        public bool? IsTrainer { get; set; }
        public bool? IsApplicant { get; set; }
    }
}