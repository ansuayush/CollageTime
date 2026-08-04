namespace ExecViewHrk.EfClient
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("JobApplicationEducation")]
    public partial class JobApplicationEducation
    {
        [Key]
        public int EducationId { get; set; }

        public int ApplicationId { get; set; }

        [Required]
        [StringLength(200)]
        public string SchoolName { get; set; }

        [StringLength(150)]
        public string Degree { get; set; }

        [StringLength(150)]
        public string FieldOfStudy { get; set; }

        [StringLength(20)]
        public string GraduationYear { get; set; }

        public virtual JobApplication JobApplication { get; set; }
    }
}
