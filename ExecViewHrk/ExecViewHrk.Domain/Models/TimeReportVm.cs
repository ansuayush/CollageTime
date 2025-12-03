using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Models
{
    public class TimeReportVm
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int EmployeeId { get; set; }
        [DataType(DataType.Time)]
        public DateTime ActualDate { get; set; }
        [DataType(DataType.Time)]
        public DateTime? TimeIn { get; set; }
        [DataType(DataType.Time)]
        public DateTime? TimeOut { get; set; }
        [DataType(DataType.Time)]
        public DateTime? LunchOut { get; set; }
        [DataType(DataType.Time)]
        public DateTime? LunchBack { get; set; }
        public string UserId { get; set; }
        public string FileNumber { get; set; }
        public string PositionDescription { get; set; }
        public double? DailyHours { get; set; }
        public string eMail { get; set; }
        public string SSN { get; set; }
        public bool? IsStudent { get; set; }
        public string Supervisor { get; set; }
        public string HomeDepartmentCode { get; set; }
        public string HomeDepartmentDescription { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool? IsApproved { get; set; }
        public string ReportstoID { get; set; }
        public string ManagerName { get; set; }
        public string CreatedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string DisApprovedBy { get; set; }
        public string DeletedBy { get; set; }
        public int CompanyCodeId { get; set; }
        public string CompanyCodeDescription { get; set; }
    }
}
