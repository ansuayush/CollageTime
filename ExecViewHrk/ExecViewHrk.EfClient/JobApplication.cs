namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("JobApplications")]
    public partial class JobApplication
    {
        public JobApplication()
        {
            Answers = new HashSet<JobApplicationAnswer>();
            Files = new HashSet<JobApplicationFile>();
            References = new HashSet<JobApplicationReference>();
            Employments = new HashSet<JobApplicationEmployment>();
            Educations = new HashSet<JobApplicationEducation>();
            Signatures = new HashSet<JobApplicationSignature>();
            Profiles = new HashSet<JobApplicationProfile>();
        }

        [Key]
        public int ApplicationId { get; set; }

        public int RequisitionId { get; set; }

        public int? ApplicantId { get; set; }

        public int? EmployeeId { get; set; }

        [Required]
        [StringLength(30)]
        public string Status { get; set; }

        public int CurrentStep { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string AdminComment { get; set; }

        [StringLength(100)]
        public string ReviewedBy { get; set; }

        public DateTime? ReviewedDate { get; set; }

        public virtual JobRequisition JobRequisition { get; set; }

        public virtual JobApplicant JobApplicant { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual ICollection<JobApplicationAnswer> Answers { get; set; }

        public virtual ICollection<JobApplicationFile> Files { get; set; }

        public virtual ICollection<JobApplicationReference> References { get; set; }

        public virtual ICollection<JobApplicationEmployment> Employments { get; set; }

        public virtual ICollection<JobApplicationEducation> Educations { get; set; }

        public virtual ICollection<JobApplicationSignature> Signatures { get; set; }

        public virtual ICollection<JobApplicationProfile> Profiles { get; set; }
    }
}
