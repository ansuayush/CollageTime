using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text.RegularExpressions;
using ExecViewHrk.Models;
namespace ExecViewHrk.Models
{
    public class PersonPhoneNumberVm
    {
        public int PersonPhoneNumberId { get; set; }

        [Required]
        public int PersonId { get; set; }
        public string PersonName { get; set; }

        [Required]
        [Display(Name = "Phone Number Type")]
        public int PhoneTypeId { get; set; }

        public string PhoneTypeDescription { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        //[RegularExpression("([1-9][0-9]*)", ErrorMessage = "Enter only numeric number")]
        public string Extension { get; set; }
        public string Gateway { get; set; }
        public bool IsPrimaryPhone { get; set; }

        [DataType(DataType.MultilineText)]
        public string EnteredBy { get; set; }
        public DateTime EnteredDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<DropDownModel> GetPersonPhoneNumberList { get; set; }
        public List<DropDownModel> GetPersonPhoneTypeList { get; set; }
        public Nullable<int> ProviderId { get; set; }
        public string ProviderName { get; set; }
        public List<DropDownModel> GetProvidersList { get; set; }
    }
}

