namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PersonTest
    {
        public int PersonTestId { get; set; }

        public int PersonId { get; set; }

        public int? EvaluationTestId { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? TestDate { get; set; }

        [StringLength(10)]
        public string Score { get; set; }

        [StringLength(10)]
        public string Grade { get; set; }

        [StringLength(50)]
        public string Administrator { get; set; }

        [Column(TypeName = "text")]
        public string Notes { get; set; }

        [Required]
        [StringLength(50)]
        public string EnteredBy { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime EnteredDate { get; set; }

        public virtual DdlEvaluationTest DdlEvaluationTest { get; set; }

        public virtual Person Person { get; set; }
    }
}
