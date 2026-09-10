using System;
using System.Collections.Generic;

namespace ExecViewHrk.WebUI.Models
{
    public class TrainingLookupVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
    }

    public class TrainingClassVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? Cost { get; set; }
        public decimal? Hours { get; set; }
        public string Location { get; set; }
        public int? CourseCodeId { get; set; }
        public string CourseCode { get; set; }
        public int? CourseCategoryId { get; set; }
        public string CourseCategory { get; set; }
        public bool IsActive { get; set; }
        public string ExpirationDate { get; set; }
        public string EnrollmentStartDate { get; set; }
        public string EnrollmentEndDate { get; set; }
        public bool HasImage { get; set; }
    }

    public class TrainingTrackVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal TotalHours { get; set; }
        public string ExpirationDate { get; set; }
        public string ClassInfo { get; set; }
        public List<int> ClassIds { get; set; }
        public List<string> ClassNames { get; set; }
    }

    public class TrainingScheduleVm
    {
        public int Id { get; set; }
        public int TrainingClassId { get; set; }
        public string ClassName { get; set; }
        public string ScheduleName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Location { get; set; }
        public bool IsActive { get; set; }
    }

    public class TrainingEnrollmentVm
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int? TrainingTypeId { get; set; }
        public string TrainingType { get; set; }
        public int? TrainingClassId { get; set; }
        public string ClassName { get; set; }
        public int? TrainingClassScheduleId { get; set; }
        public string ScheduleName { get; set; }
        public int? TrainingTrackId { get; set; }
        public string TrainingTrackName { get; set; }
        public string EnrollmentDate { get; set; }
        public string EnrollmentStartDate { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Location { get; set; }
        public decimal? Hours { get; set; }
        public decimal? Cost { get; set; }
        public int? StatusId { get; set; }
        public string Status { get; set; }
        public string CompletionDate { get; set; }
        public int? CompletionStatusId { get; set; }
        public string CompletionStatus { get; set; }
        public string ExpirationDate { get; set; }
        public string Notes { get; set; }
        public string Instructor { get; set; }
        public int? CourseCodeId { get; set; }
        public string CourseCode { get; set; }
        public int? CourseCategoryId { get; set; }
        public string CourseCategory { get; set; }
        public bool IsEnrolled { get; set; }
        public bool IsCompleted { get; set; }
        public string DocumentName { get; set; }
    }

    public class TrainingEnrollmentSaveVm
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int? TrainingTypeId { get; set; }
        public int? TrainingClassId { get; set; }
        public int? TrainingClassScheduleId { get; set; }
        public int? TrainingTrackId { get; set; }
        public string EnrollmentDate { get; set; }
        public string EnrollmentStartDate { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Location { get; set; }
        public decimal? Hours { get; set; }
        public decimal? Cost { get; set; }
        public int? StatusId { get; set; }
        public string CompletionDate { get; set; }
        public int? CompletionStatusId { get; set; }
        public string ExpirationDate { get; set; }
        public string Notes { get; set; }
        public string Instructor { get; set; }
    }
}
