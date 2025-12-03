using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace ExecViewHrk.WebUI.Models
{
    public class CompanyCodeVm
    {

        [Key]
        public int CompanyCodeId { get; set; }
        public string CompanyCodeDescription { get; set; }

        
        [Required]
        [StringLength(50)]
        public string Description { get; set; }

        [Required]
        [StringLength(10)]
        public string Code { get; set; }



        public bool Active { get; set; }
    }
}