
namespace ExecViewHrk.EfClient
{
    using System.ComponentModel.DataAnnotations;
   public class PerformanceProfiles
    {
        [Key]
        public int PerProfileID { get; set; }

        [Required]
        [StringLength(50)]
        public string Description { get; set; }

        [Required]
        [StringLength(10)]
        public string Code { get; set; }

        public bool Active { get; set; }
    }
}
