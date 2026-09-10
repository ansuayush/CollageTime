using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExecViewHrk.EfClient
{
    [Table("TrainingCourseCode")]
    public partial class TrainingCourseCode
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string Code { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }

    [Table("TrainingCourseCategory")]
    public partial class TrainingCourseCategory
    {
        [Key]
        public int Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }

    [Table("TrainingType")]
    public partial class TrainingType
    {
        [Key]
        public int Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }

    [Table("TrainingStatus")]
    public partial class TrainingStatus
    {
        [Key]
        public int Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
    }

    [Table("TrainingClass")]
    public partial class TrainingClass
    {
        [Key]
        public int Id { get; set; }
        [StringLength(500)]
        public string Name { get; set; }
        [StringLength(2000)]
        public string Description { get; set; }
        public byte[] Image { get; set; }
        public decimal? Cost { get; set; }
        public decimal? Hours { get; set; }
        [StringLength(1000)]
        public string Location { get; set; }
        public int? CourseCodeId { get; set; }
        public int? CourseCategoryId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? EnrollmentStartDate { get; set; }
        public DateTime? EnrollmentEndDate { get; set; }
        public int? CompanyId { get; set; }
        [StringLength(100)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [StringLength(100)]
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    [Table("TrainingTrack")]
    public partial class TrainingTrack
    {
        [Key]
        public int Id { get; set; }
        [StringLength(500)]
        public string Name { get; set; }
        [StringLength(2000)]
        public string Description { get; set; }
        public string ClassInfo { get; set; }
        public decimal TotalHours { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? CompanyId { get; set; }
        [StringLength(100)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [StringLength(100)]
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    [Table("TrainingTrackClass")]
    public partial class TrainingTrackClass
    {
        [Key]
        public int Id { get; set; }
        public int TrainingTrackId { get; set; }
        public int TrainingClassId { get; set; }
        public int SortOrder { get; set; }
    }

    [Table("TrainingClassSchedule")]
    public partial class TrainingClassSchedule
    {
        [Key]
        public int Id { get; set; }
        public int TrainingClassId { get; set; }
        [StringLength(200)]
        public string ScheduleName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [StringLength(1000)]
        public string Location { get; set; }
        public bool IsActive { get; set; }
    }

    [Table("TrainingEmployee")]
    public partial class TrainingEmployee
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int? PersonId { get; set; }
        public int? CompanyId { get; set; }
        public int? TrainingTypeId { get; set; }
        public int? TrainingClassId { get; set; }
        [StringLength(500)]
        public string ClassName { get; set; }
        public int? TrainingClassScheduleId { get; set; }
        public int? TrainingTrackId { get; set; }
        [StringLength(100)]
        public string TrainingTrackName { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public DateTime? EnrollmentStartDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [StringLength(500)]
        public string Location { get; set; }
        public decimal? Hours { get; set; }
        public decimal? Cost { get; set; }
        public int? StatusId { get; set; }
        public DateTime? CompletionDate { get; set; }
        public int? CompletionStatusId { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Notes { get; set; }
        [StringLength(50)]
        public string Instructor { get; set; }
        public int? CourseCodeId { get; set; }
        public int? CourseCategoryId { get; set; }
        [StringLength(400)]
        public string DocumentName { get; set; }
        [StringLength(100)]
        public string ContentType { get; set; }
        public byte[] Attachment { get; set; }
        [StringLength(50)]
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
