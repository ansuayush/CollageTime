namespace ExecViewHrk.EfClient
{
   
    using System.ComponentModel.DataAnnotations;
    public partial class Location
    {
        public int LocationId { get; set; }

        [Required]
        [StringLength(50)]
        public string LocationDescription { get; set; }
        [Required]
        [StringLength(10)]
        public string LocationCode { get; set; }

      

        public bool Active { get; set; }

    }
}
