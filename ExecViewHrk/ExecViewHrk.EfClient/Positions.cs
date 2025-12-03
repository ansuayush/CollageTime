namespace ExecViewHrk.EfClient
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class Positions
    {
        public int PositionId { get; set; }
        public int? BusinessLevelNbr { get; set; }
        public int? BusinessUnitId { get; set; }
        public int? JobId { get; set; }
        public int? DepartmentId { get; set; }
        public int? LocationId { get; set; }
        public int? UserDefinedSegment1Id { get; set; }
        public int? UserDefinedSegment2Id { get; set; }
        [Required]
        [StringLength(100)]
        public string PositionDescription { get; set; }
        [Required]
        [StringLength(15)]
        public string PositionCode { get; set; }
        public bool IsPositionActive { get; set; }
        public int? ReportsToPositionId { get; set; }
        public string Title { get; set; }
        public DateTime? FrozenDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public int? Status { get; set; }
        public int? PositionCategoryID { get; set; }
        public int? PositionTypeID { get; set; }
        public int? PositionGradeID { get; set; }
        public string PayFrequencyCode { get; set; }
        public string ProbationPeriod { get; set; }
        public int? UnitsID { get; set; }
        public string ScheduledHours { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? AuthorizedByID { get; set; }
        public int? TotalSlots { get; set; }
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
        public string IncumbentADPID { get; set; }
        public string Division { get; set; }
        public decimal? SalaryAmount { get; set; }
        public string SalaryPlanCode { get; set; }
        public string SalaryStep { get; set; }
        public string SalaryType { get; set; }
        public string SalaryPayGroup { get; set; }
        public string CostNumber { get; set; }
        public string Suffix { get; set; }
    }
}
