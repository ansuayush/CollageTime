using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ExecViewHrk.WebUI.Models
{
    public class FormTemplateVm
    {

        public int PersonId { get; set; }

        [Required]
        [StringLength(9, MinimumLength = 9,ErrorMessage="SSN must be 9 digits")]
        public string Ssn { get; set; }
        
        [Required]
        public string LastName { get; set; }

        [Required]
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
    }
}