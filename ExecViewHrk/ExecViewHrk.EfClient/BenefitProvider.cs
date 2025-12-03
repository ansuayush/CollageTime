namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class BenefitProvider
    {
        public int BenefitProviderId { get; set; }

        public int CompanyCodeId { get; set; }

        [Required]
        [StringLength(10)]
        public string BenefitProviderCode { get; set; }

        [Required]
        [StringLength(50)]
        public string BenefitProviderDescription { get; set; }

        public virtual CompanyCode CompanyCode { get; set; }
    }
}
