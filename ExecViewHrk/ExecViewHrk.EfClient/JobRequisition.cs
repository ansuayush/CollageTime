namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("JobRequisitions")]
    public partial class JobRequisition
    {
        public JobRequisition()
        {
            JobApplications = new HashSet<JobApplication>();
        }

        [Key]
        public int RequisitionId { get; set; }

        [Required]
        [StringLength(50)]
        public string RequisitionNumber { get; set; }

        [Required]
        [StringLength(200)]
        public string PositionTitle { get; set; }

        [StringLength(100)]
        public string Division { get; set; }

        [StringLength(100)]
        public string Department { get; set; }

        public int? PositionId { get; set; }

        public int? ReportToPositionId { get; set; }

        public string Description { get; set; }

        public DateTime RequisitionDate { get; set; }

        public DateTime? OpenDate { get; set; }

        public DateTime? ClosedDate { get; set; }

        [Required]
        [StringLength(30)]
        public string Status { get; set; }

        public int ApplicantCount { get; set; }

        public bool IsPublished { get; set; }

        [StringLength(100)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [StringLength(100)]
        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public virtual ICollection<JobApplication> JobApplications { get; set; }
    }
}
