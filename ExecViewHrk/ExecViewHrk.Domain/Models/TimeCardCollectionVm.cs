using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Models
{
    public class TimeCardCollectionVm
    {
        public int TimeCardId { get; set; }
        public int CompanyCodeId { get; set; }
        //public short DepartmentId { get; set; }
        public int DepartmentId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime ActualDate { get; set; }
        public DateTime Day { get; set; }
        public int ProjectNumber { get; set; }
        public double? DailyHours { get; set; }
        public int? HoursCodeId { get; set; }
        public double? Hours { get; set; }
        //public short? TempDeptId { get; set; }
        public int? TempDeptId { get; set; }
        //public string TempDepartmentCode { get; set; }
        public int? TempJobId { get; set; }
        public string TempJobCode { get; set; }
        //public short? EarningsCodeId { get; set; }
        public int? EarningsCodeId { get; set; }
        public double? EarningsAmount { get; set; }
        public int WeekNum { get; set; }
        public double? LineTotal { get; set; }  //DailyHours + Hours
        public DateTime? TimeIn { get; set; }
        public DateTime? TimeOut { get; set; }
        public DateTime? LunchOut { get; set; }
        public DateTime? LunchBack { get; set; }
        public bool IsLineApproved { get; set; }
        public int? ProjectsId { get; set; }
        public int? FundsId { get; set; }
        public int? TaskId { get; set; }
        public int? PositionId { get; set; }
        public string Falsacode { get; set; }
        public bool ShowLineApprovedActive { get; set; }
        public int NotesId { get; set; }
        public string UserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string ApprovedBy { get; set; }
        public bool isChild { get; set; }
        public bool IsDeleted { get; set; }
        public int? E_PositionId { get; set; }
    }
}
