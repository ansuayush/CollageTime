using System;
using System.Collections.Generic;
using ExecViewHrk.EfClient;
using System.ComponentModel.DataAnnotations;
using ExecViewHrk.Models;

namespace ExecViewHrk.WebUI.Models
{
    public class PositionDetailsVm
    {

        public int PositionId { get; set; }
        public int? BusinessUnitId { get; set; }
        public string BusinessLevel { get; set; }
        [Required]
        public int? BusinessLevelNbr { get; set; }
        public string BUCode { get; set; }

        public string BUTitle { get; set; }
        [Required]
        public int? JobId { get; set; }


        public string JobTitle { get; set; }

        public string JobCode { get; set; }
        public int? DepartmentId { get; set; }

        public int? LocationId { get; set; }

        public int? UserDefinedSegment1Id { get; set; }

        public int? UserDefinedSegment2Id { get; set; }
        [Required]
        [StringLength(100)]
        public string PositionDescription { get; set; }     
        [StringLength(15)] 
        public string PositionCode { get; set; }
        public bool IsPositionActive { get; set; }

        public int? ReportsToPositionId { get; set; }
        public string Title { get; set; }
        public DateTime? FrozenDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public int? Status { get; set; }
        public string PositionCategory { get; set; }
        public int? PositionTypeID { get; set; }
        public string PositionGrade { get; set; }
        public int? PositionGradeID { get; set; }
        public string PayFrequencyCode { get; set; }
        public string ProbationPeriod { get; set; }
        public int? UnitsID { get; set; }

        public string ScheduledHours { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? AuthorizedByID { get; set; }
        public int? TotalSlots { get; set; }
        public int FilledSlots { get; set; }

        public DateTime? EnteredDate { get; set; }
        public string EnteredBy { get; set; }
        public string Requisitno { get; set; }
        public string TravPercent { get; set; }
        public string Shift { get; set; }
        public string PositReason { get; set; }
        public string PositLocation { get; set; }
        public DateTime? PositStatusActive { get; set; }
        public DateTime? PositStatusFrozen { get; set; }
        public DateTime? PositStatusClosed { get; set; }
        public DateTime? PositStatusOpen { get; set; }
        public DateTime? PositStatusPost { get; set; }
        public int? PerProfileID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ProjectEndDate { get; set; }
        public DateTime? ActualEnddate { get; set; }
        public bool? ThirdShift { get; set; }
        public string Description { get; set; }
        [Required]
        public string Code { get; set; }
        public string AccountNumber { get; set; }
        public string Group2 { get; set; }
        public string Group3 { get; set; }
        public string Group4 { get; set; }
        public string Group5 { get; set; }
        public string FLSA { get; set; }
        public int? LengthOfContract { get; set; }
        public int? WorkClassificationId { get; set; }

        public string AlternateTitle { get; set; }
        public int? FTE { get; set; }
        public int? PositionCategoryID { get; set; }
        public string PositionType { get;  set; }
        public string ReportsTo { get;  set; }
        public string CurrentStatus { get;  set; }

        public string IncumbentADPID { get; set; }
        public string Department { get;  set; }
        public string Division { get;  set; }
        public string PerProfile { get;  set; }
        public decimal? SalaryAmount { get; set; }
        public string SalaryPlanCode { get; set; }
        public string SalaryStep { get; set; }
        public string SalaryType { get; set; }
        public string SalaryPayGroup { get; set; }
        public string SalaryGrade { get; set; }
        public string oldval { get; set; }
        public string olddepartment { get; set; }
        public string oldsalarygrade { get; set; }
        public string oldpayfrequency { get; set; }
        public string oldsalaryplancode { get; set; }
        public string oldsalarystep { get; set; }
        public string positionnewcode { get; set; }

        public IEnumerable<DdlSalaryGrades> DdlSalaryGrades { get; set;}
      
        
        public IEnumerable<SalaryGradeVm> SalaryGradeHistory { get; set; }
        public List<DropDownModel> DdlPayFrequency { get; set; }
        public List<DdlEmployeeType> DdlEmployeeType { get; set; }

        public List<PerformanceProfiles> performanceProfiles { get; set; }
        public List<DdlPositionTypes> ddlPositionType { get; set; }
        public List<DdlPositionCategory> ddlPositionCategory { get; set; }

        public List<PositionBusinessLevels> positionBusinessLevelsList { get; set; }
        public List<DropDownModel> ReportToList { get; set; }
        public List<Job> JobList { get; set; }
        public DateTime? CreatedDate { get; set; }
        public IEnumerable<PositionBudgetsVM>PositionBudgetList { get; set; }
        public List<DropDownModel> DDEmpType { get; set; }
        public PosionFundingSource posionFundingSourceVM { get; set; }
        public List<Location> LocationList { get; set; }
        public List<Department> DepartmentList { get; set; }
        public List<DropDownModel> EmployeeList { get; set; }
        public List<DdlPayGroup> PayGroupList { get; set; }

        public List<E_PositionSalaryHistoryVm> EmpSalaryList { get; set; }
        public List<DropDownModel> DropDownSalaryGrade { get; set; }
        public IEnumerable<SalaryGradeHistoryListVm> SalaryGradeHistoryListVm { get; set; }
        public List<PositionFundingSourceGroupVM> positionFundingSourceList { get; set; }
        public IEnumerable<DropDownModel> DropDownWorkStatusType { get; set; }

        public SalaryGradeVm PositionSalaryGradeDetail { get; set; }

        public IEnumerable<ExecViewHrk.EfClient.PositionHistory> PositionHistory { get; set; }

        public string CostNumber { get; set; }
        public string Suffix { get; set; }
        public List<DropDownModel> ddlpositionBusinessLevels { get; set; }
        public List<DropDownModel> ddlJobs { get; set; }
    }
}