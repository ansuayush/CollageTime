using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Models
{
    public class TimeOffEmployeesAllowedTakenVM
    {
        public int EmployeesAllowedTakenId { get; set; }
        public int CompanyCodeId { get; set; }
        public int EmployeeId { get; set; }
        [StringLength(50)]
        public string FileNumber { get; set; }
        public int TypeId { get; set; }
        public decimal Allowed { get; set; }
        public decimal Taken { get; set; }
        public decimal Remainder { get; set; }
        public double? Total { get; set; }
        public int? PositionId { get; set; }
        public decimal? PayRate { get; set; }
    }
}
