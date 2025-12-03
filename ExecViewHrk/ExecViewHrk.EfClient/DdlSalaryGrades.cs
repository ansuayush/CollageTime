

namespace ExecViewHrk.EfClient
{
    using System.ComponentModel.DataAnnotations;
    public partial class DdlSalaryGrades
    {
        [Key] 
        public int SalaryGradeID { get; set; }

        public string code { get; set; }

        public string description { get; set; }

        public bool  active { get; set; }
    }
}
