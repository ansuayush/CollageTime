namespace ExecViewHrk.EfClient
{
    using System.ComponentModel.DataAnnotations;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class DdlSalaryGradeHistory
    {
        [Key]
        public int ID { get; set; }
       
        public int? SalaryGradeID { get; set; }

        public DateTime? validFrom { get; set; }

        public Decimal? salaryMinimum { get; set; }
        public Decimal? salaryMidpoint { get; set; }

        public Decimal? salaryMaximum { get; set; }

        public virtual DdlSalaryGrades DdlSalaryGrades { get; set; }

    }
}
