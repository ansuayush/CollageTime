using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ExecViewHrk.Models;

namespace ExecViewHrk.WebUI.Models
{
    public class SkillVM
    {
        public int SkillId { get; set; }

        [Required]
        [StringLength(10)]
        public string Code { get; set; }
        [Required]
        [StringLength(50)]
        public string Description { get; set; }

        public int? SkillTypeId { get; set; }


        public bool Active { get; set; }

        public bool isNewRecord { get; set; }

        public List<DropDownModel> SkillTypeDrop { get; set; }
    
    }
}