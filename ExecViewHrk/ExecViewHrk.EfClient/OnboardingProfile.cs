namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("OnboardingProfiles")]
    public partial class OnboardingProfile
    {
        public OnboardingProfile()
        {
            Documents = new HashSet<OnboardingProfileDocument>();
        }

        [Key]
        public int ProfileId { get; set; }

        [Required]
        [StringLength(200)]
        public string ProfileName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        [StringLength(100)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [StringLength(100)]
        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public virtual ICollection<OnboardingProfileDocument> Documents { get; set; }
    }
}
