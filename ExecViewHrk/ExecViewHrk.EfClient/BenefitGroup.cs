namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class BenefitGroup
    {
        public int BenefitGroupId { get; set; }

        public int CompanyCodeId { get; set; }

        [Required]
        [StringLength(10)]
        public string BenefitGroupCode { get; set; }

        [Required]
        [StringLength(50)]
        public string BenefitGroupDescription { get; set; }

        [Required]
        [StringLength(10)]
        public string DeductionCode { get; set; }

        public int DisplayPosition { get; set; }

        public bool CanBeDeclined { get; set; }

        [StringLength(20)]
        public string ScheduleGroup { get; set; }

        public virtual CompanyCode CompanyCode { get; set; }
    }
}
