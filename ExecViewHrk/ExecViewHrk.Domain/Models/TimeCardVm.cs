using ExecViewHrk.EfClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExecViewHrk.Models
{
    public class TimeCardVm
    {
        public int TimeCardId { get; set; }

        public int CompanyCodeId { get; set; }
        public string CompanyCodeDescription { get; set; }

        public int? DepartmentId { get; set; }
        public string DepartmentDescription { get; set; }

        public int EmployeeId { get; set; }
        public string EmployeeFullName { get; set; }

        public int? PayPeriodId { get; set; }
        public string PayPeriod { get; set; }

        public int? TempDeptId { get; set; }
        //public short? TempDeptId { get; set; } //Dropdown
        public string TempDepartmentCode { get; set; }

        public int? TempJobId { get; set; }
        public string TempJobCode { get; set; }

        //[UIHint("EarningCodeEditor")]
        [Display(Name = "Earning Code")]
        public int? EarningsCodeId { get; set; }
        //public short? EarningsCodeId { get; set; }
        public List<EarningCodeVm> EarningCodeEditor { get; set; }

        public double? EarningsAmount { get; set; }

        [DataType(DataType.Date)]
        public DateTime ActualDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime Day { get; set; }

        public int ProjectNumber { get; set; }

        public double? DailyHours { get; set; }

        public int? HoursCodeId { get; set; }

        public double? Hours { get; set; }

        public bool Approved { get; set; }

        public bool IsLineApproved { get; set; }

        public bool ShowLineApprovedActive { get; set; }

        public int WeekNum { get; set; }

        public double? LineTotal { get; set; }

        public bool IsArchived { get; set; }
        [DataType(DataType.Time)]
        public DateTime? TimeIn { get; set; }
        [DataType(DataType.Time)]
        public DateTime? TimeOut { get; set; }
        [DataType(DataType.Time)]
        public DateTime? LunchOut { get; set; }
        [DataType(DataType.Time)]
        public DateTime? LunchBack { get; set; }
        public string ApprovedBy { get; set; }
        public bool IsApproved { get; set; }
        public int? HoursCodeReasonId { get; set; }

        public string FileNumber { get; set; }

        public string Project { get; set; }
        public string Task { get; set; }
        public double? OT { get; set; }
        public int? MealsTaken { get; set; }
        public double? Rate { get; set; }
        public double? HoursCodeRate { get; set; }
        public string EnteredBy { get; set; }
        public DateTime? EnteredDate { get; set; }

        public int? JobId { get; set; }

        public string WeekDay { get; set; }

        //[UIHint("TypeDropDown")]
        [Display(Name = "Hour Code")]
        public byte? Type { get; set; }
        public List<HoursCodeDropDownVM> TypeDropDown { get; set; }
        public string TypeName { get; set; }
        //[UIHint("DepartmentEditor")]
        public byte? Deptid { get; set; }
        public byte? DeptName { get; set; }
        public int? FundsId { get; set; }
        public int? ProjectsId { get; set; }
        public int? TaskId { get; set; }
        public string Falsacode { get; set; }
        public List<HoursCodeDropDownVM> HoursCodelist { get; set; }
        public List<EarningCodeVm> EarningcodeList { get; set; }
        public List<DepartmentVm> Departmentslist { get; set; }
        public List<JobsVM> JobsList { get; set; }
        public int? PositionId { get; set; }
        public TimeCardDisplayColumn timeCardDislayColumns { get; set; }


        public string EarningsCodeDescription { get; set; }

        public int NotesId { get; set; }

        public string UserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public bool isChild { get; set; }

        public bool LockEmployee { get; set; }
        public bool LockManger { get; set; }
        public bool LockAssignedManager { get; set; }
        public bool IsDeleted { get; set; }
        public string DeletedBy { get; set; }
        public int? E_PositionId { get; set; }
    }
}