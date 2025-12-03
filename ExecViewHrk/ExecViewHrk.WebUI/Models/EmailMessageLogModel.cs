using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ExecViewHrk.WebUI.Models
{
    public class EmailMessageLogModel
    {
        
        [Required]
        public string To { get; set; }
        
        [Required]
        public string Subject { get; set; }

        [Required]
        [DataType(DataType.MultilineText)] 
        public string Body { get; set; }
    }
}