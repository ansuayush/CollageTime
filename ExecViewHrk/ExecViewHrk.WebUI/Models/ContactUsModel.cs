using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExecViewHrk.WebUI.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace EmployMeMatch.WebUI.Models
{
    public class ContactUsModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required]
        [DataType(DataType.MultilineText)] 
        public string Message { get; set; }

        
    }
}