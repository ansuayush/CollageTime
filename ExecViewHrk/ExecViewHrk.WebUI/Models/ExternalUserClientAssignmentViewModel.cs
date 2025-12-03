using System;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace ExecViewHrk.WebUI.Models
{
    public class ExternalUserClientAssignmentViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int EmployerId { get; set; }
        public string EmployerName { get; set; }
    }
}