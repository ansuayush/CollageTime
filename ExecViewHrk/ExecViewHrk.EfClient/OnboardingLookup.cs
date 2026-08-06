namespace ExecViewHrk.EfClient
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("OnboardingLookups")]
    public partial class OnboardingLookup
    {
        [Key]
        public int LookupId { get; set; }

        [Required]
        [StringLength(50)]
        public string LookupType { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; }
    }
}
