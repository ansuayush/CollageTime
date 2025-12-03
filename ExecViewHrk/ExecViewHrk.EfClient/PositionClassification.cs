namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PositionClassification
    {
        [Key]
        [StringLength(50)]
        public string ClassificationCriteria { get; set; }

        public bool IsCriteriaApplicable { get; set; }
    }
}
