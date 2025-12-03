using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExecViewHrk.WebUI.Models
{

    public class PasswordViewModel
    {
        [Required]
        public string Password { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }

    }

    
}
