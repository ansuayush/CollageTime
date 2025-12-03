using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ExecViewHrk.WebUI.Models
{
    public class PersonSkillVm
    {
        public int PersonSkillId { get; set; }

        [Required]
        public int PersonId { get; set; }
        public string PersonName { get; set; }

        public int SkillId { get; set; }
        [Required]
        public string SkillDescription { get; set; }

        public int? SkillLevelId { get; set; }
       
        public string SkillLevelDescription { get; set; }
        
        public DateTime? AttainedDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? LastUsedDate { get; set; }
        public DateTime? EffectiveDate { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        public string EnteredBy { get; set; }
        public DateTime EnteredDate { get; set; }

    }
}