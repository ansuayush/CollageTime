namespace ExecViewHrk.EfAdmin
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ExternalUserClient
    {
        public int ExternalUserClientId { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        public int EmployerId { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual Employer Employer { get; set; }
    }
}
