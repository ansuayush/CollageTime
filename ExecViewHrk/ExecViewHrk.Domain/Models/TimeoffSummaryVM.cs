using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Models
{
    public class TimeoffSummaryVM
    {
        public int EmployeesAllowedTakenId { set; get; }
        public int EmployeeId { set; get; }
        public int CompanyCodeId { set; get; }
        public string FileNumber { set; get; }
        public string Typecode { set; get; }
        public int TypeId { set; get; }
        public decimal? Allowed { set; get; }
        public decimal? Taken { set; get; }
        public decimal? Remainder { set; get; }
        public string Position { get; set; }
    }
}
