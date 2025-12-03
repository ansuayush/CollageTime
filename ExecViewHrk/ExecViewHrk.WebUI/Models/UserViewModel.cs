using System;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using ExecViewHrk.Models;

namespace ExecViewHrk.WebUI.Models
{
    //public enum LoginTypes
    //{
    //    JOB_SEEKER, EMPLOYER, RESNAV, EMPLOYEE_ADMIN, EMPLOYEE
    //};

    public class UserViewModel
    {
        public string UserId { get; set; }
        
        [Required]
        [EmailAddress]
        public string UserName { get; set; }
        
        [Required]
        [Range(1, 1000000000000, ErrorMessage = "Must be between {2} and {1} characters long.")]
        public int EmployerId { get; set; }

        [StringLength(12, ErrorMessage = "Must be between {2} and {1} characters long.", MinimumLength = 4)]
        [Required]
        public string Password { get; set; }
        
        public DateTime LastPasswordChangeDate { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }
        public string EmployerName { get; set; }

        public List<DropDownModel> EmployerList { get; set; }
                
    }
}