using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Models
{
   public class StduentPositionsReportVM
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string SSN { get; set; }
        public string Email { get; set; }
        public string FileNumber { get; set; }
        public string PositionDescription { get; set; }
        public string PositionCode { get; set; }
        public string HomeDepartmentDescription { get; set; }
        public string JobCode { get; set; }
        public string Supervisor { get; set; }
        public double? TotalTimes { get; set; }          
        public DateTime? PositionStartDate { get; set; }       
        public DateTime? PositionEndDate { get; set; }
    }
}
