namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PersonSkill
    {
        public int PersonSkillId { get; set; }

        public int PersonId { get; set; }

        public int SkillId { get; set; }

        public int? SkillLevelId { get; set; }

        
        public DateTime? AttainedDate { get; set; }

        
        public DateTime? ExpirationDate { get; set; }

        
        public DateTime? LastUsedDate { get; set; }

        
        public DateTime? EffectiveDate { get; set; }

        [Column(TypeName = "text")]
        public string Notes { get; set; }

        [Required]
        [StringLength(50)]
        public string EnteredBy { get; set; }

        
        public DateTime EnteredDate { get; set; }

        public virtual DdlSkillLevel DdlSkillLevel { get; set; }

        public virtual DdlSkill DdlSkill { get; set; }

        public virtual Person Person { get; set; }
    }
}
