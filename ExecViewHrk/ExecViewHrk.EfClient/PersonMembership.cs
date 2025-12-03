namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PersonMembership
    {
        public int PersonMembershipId { get; set; }

        public int PersonId { get; set; }

        public int ProfessionalBodyId { get; set; }

        
        public DateTime? StartDate { get; set; }

        
        public DateTime? RenewalDate { get; set; }

        [StringLength(50)]
        public string Number { get; set; }

        [Column(TypeName = "money")]
        public decimal? Fee { get; set; }

        
        public DateTime? FeePaidDate { get; set; }

        [StringLength(50)]
        public string ProfessionalTitle { get; set; }

        [Column(TypeName = "text")]
        public string Notes { get; set; }

        public int? RegionalChapterId { get; set; }

        [StringLength(50)]
        public string EnteredBy { get; set; }

        
        public DateTime? EnteredDate { get; set; }

        [StringLength(100)]
        public string RegionalChapter { get; set; }

        [StringLength(50)]
        public string ModifiedBy { get; set; }

        
        public DateTime? ModifiedDate { get; set; }

        public virtual DdlProfessionalBody DdlProfessionalBody { get; set; }

        public virtual DdlRegionalChapter DdlRegionalChapter { get; set; }

        public virtual Person Person { get; set; }
    }
}
