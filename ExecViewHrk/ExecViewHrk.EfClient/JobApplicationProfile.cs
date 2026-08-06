namespace ExecViewHrk.EfClient
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("JobApplicationProfiles")]
    public partial class JobApplicationProfile
    {
        [Key]
        public int ProfileId { get; set; }

        public int ApplicationId { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(100)]
        public string MiddleName { get; set; }

        [StringLength(100)]
        public string PreferredName { get; set; }

        [StringLength(250)]
        public string StreetAddress { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        [StringLength(20)]
        public string ZipCode { get; set; }

        public int? CountryId { get; set; }

        public int? StateId { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        [StringLength(200)]
        public string Email { get; set; }

        public virtual JobApplication JobApplication { get; set; }
    }
}
