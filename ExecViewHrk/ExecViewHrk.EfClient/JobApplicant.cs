namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("JobApplicants")]
    public partial class JobApplicant
    {
        public JobApplicant()
        {
            JobApplications = new HashSet<JobApplication>();
        }

        [Key]
        public int ApplicantId { get; set; }

        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [StringLength(200)]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(100)]
        public string PasswordSalt { get; set; }

        [Required]
        [StringLength(200)]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public virtual ICollection<JobApplication> JobApplications { get; set; }
    }
}
