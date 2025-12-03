using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.EfClient
{
   public partial class PositionSalaryGradeSourceHistories
    {
        [Key]
        public int PositionSalaryGradeHistoriesID { get; set; }

        public int? SalaryGradeID { get; set; }

        public DateTime? ValidDate { get; set; }

        public Decimal? salaryMinimum { get; set; }
        public Decimal? salaryMidpoint { get; set; }

        public Decimal? salaryMaximum { get; set; }

        public DateTime ChangeEffectiveDate { get; set; }

        public int DdlSalaryGradeHistoriesID { get; set; }

    }
}
