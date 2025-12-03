namespace ExecViewHrk.EfClient
{
    using System.ComponentModel.DataAnnotations;

    public class DdlPayGroup
    {
        [Key]
        public int PayGroupId { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        [Required]
        [StringLength(50)]
        public string Description { get; set; }

        public bool Active { get; set; }
    }
}