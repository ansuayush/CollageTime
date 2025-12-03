

namespace ExecViewHrk.EfClient
{
    using System;
    using System.ComponentModel.DataAnnotations;
    public partial class PositionSalaryGrades
    {
        [Key]
        public int id { get; set; }

        public int PositionId { get; set; }

        public int salaryGradeID { get; set; }

        public string enteredBy { get; set; }

        public DateTime? enteredDate { get; set; }

    }
}
