namespace ExecViewHrk.EfClient
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("JobApplicationReferences")]
    public partial class JobApplicationReference
    {
        [Key]
        public int ReferenceId { get; set; }

        public int ApplicationId { get; set; }

        [Required]
        [StringLength(150)]
        public string FullName { get; set; }

        [StringLength(100)]
        public string Relationship { get; set; }

        [StringLength(150)]
        public string Company { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        [StringLength(200)]
        public string Email { get; set; }

        [StringLength(50)]
        public string YearsKnown { get; set; }

        public virtual JobApplication JobApplication { get; set; }
    }
}
