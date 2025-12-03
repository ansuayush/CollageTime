using ExecViewHrk.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace ExecViewHrk.WebUI.Models
{
    public class PersonPhoneNumberVm
    {
        public int PersonPhoneNumberId { get; set; }

        [Required]
        public int PersonId { get; set; }
        public string PersonName { get; set; }

        [Required]
        [Display(Name = "Phone Number Type")]
        public short PhoneTypeId { get; set; }
       
        public string PhoneTypeDescription { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        //[RegularExpression("([1-9][0-9]*)", ErrorMessage = "Enter only numeric number")]
        public string Extension { get; set; }

        [DataType(DataType.MultilineText)]
        public string EnteredBy { get; set; }
        public DateTime EnteredDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public List<DropDownModel> GetPersonPhoneNumberList { get; set; }
        public List<DropDownModel> GetPersonPhoneTypeList { get; set; }
    }
}