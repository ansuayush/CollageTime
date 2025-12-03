using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Models
{
    public class FinancialCustomVm
    {
        public string PositionID { get; set; }
        public int EmployeeId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Description { get; set; }
        
        public DateTime payperidenddate { get; set; }
        public int PayPeriodNumber { get; set; }
        public decimal RegularRatePaid { get; set; }
        public string JobTitleCode { get; set; }
        public decimal? GrossPay { get; set; }
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
        public DateTime? PayDate { get; set; }
        public int? PayPeriodId { get; set; }
        public double? CodedHours { get; set; }
        public double? RegularHours { get; set; }
        public string HoursCodeDescription { get; set; }
        public double? RetroHours { get; set; }
        public string RetroHourCode { get; set; }
    }
}
