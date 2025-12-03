using System;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace ExecViewHrk.WebUI.Models
{
    public class UserRoleAssignmentViewModel
    {
        //public string UserId { get; set; }
        public string UserName { get; set; }
        //public string RoleId { get; set; }
        public string RoleName { get; set; }
    }
}