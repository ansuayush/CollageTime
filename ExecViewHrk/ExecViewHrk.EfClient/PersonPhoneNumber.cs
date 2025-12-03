namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PersonPhoneNumber
    {
        public int PersonPhoneNumberId { get; set; }

        public int PersonId { get; set; }

        public int PhoneTypeId { get; set; }

        [Required]
        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [StringLength(5)]
        public string Extension { get; set; }

        public bool IsPrimaryPhone { get; set; }

        [Required]
        [StringLength(50)]
        public string EnteredBy { get; set; }

        public DateTime EnteredDate { get; set; }

        [StringLength(50)]
        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public virtual DdlPhoneType DdlPhoneType { get; set; }

        public virtual Person Person { get; set; }
        public virtual Providers Providers { get; set; }
        public Nullable<int> ProviderId { get; set; }

    }
}
