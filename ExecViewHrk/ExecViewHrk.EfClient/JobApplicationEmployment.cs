namespace ExecViewHrk.EfClient
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("JobApplicationEmployment")]
    public partial class JobApplicationEmployment
    {
        [Key]
        public int EmploymentId { get; set; }

        public int ApplicationId { get; set; }

        [Required]
        [StringLength(200)]
        public string EmployerName { get; set; }

        [StringLength(150)]
        public string JobTitle { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Duties { get; set; }

        [StringLength(300)]
        public string ReasonLeft { get; set; }

        public virtual JobApplication JobApplication { get; set; }
    }
}
