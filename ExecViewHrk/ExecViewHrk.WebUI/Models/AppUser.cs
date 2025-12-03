using System;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ExecViewHrk.WebUI.Models
{
    //public enum LoginTypes
    //{
    //    JOB_SEEKER, EMPLOYER, RESNAV, EMPLOYEE_ADMIN, EMPLOYEE
    //};

    public class AppUser : IdentityUser
    {
        // additional properties will go here
        public int EmployerId { get; set; }
        public DateTime LastPasswordChangeDate { get; set; }

        //public LoginTypes LoginType { get; set; }
        //public string EmployerEin { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        //public string MiddleName { get; set; }
        //public string NameSuffix { get; set; }
        //public string Password { get; set; }
    }
}