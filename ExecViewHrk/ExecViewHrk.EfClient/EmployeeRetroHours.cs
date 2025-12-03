using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.EfClient
{
    public partial class EmployeeRetroHours
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int EPositionId { get; set; }
        public int PositionSalaryHistoryId { get; set; }
        public decimal RetroHours { get; set; }
        public DateTime? RetroHoursDate { get; set; }
        public int PayperiodId { get; set; }
        public string EnteredBy { get; set; }
        public DateTime? EnteredDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int CompanyCodeId { get; set; }
        public int HoursCodeId { get; set; }


    }
}
