namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PersonInnoculation
    {
        public int PersonInnoculationId { get; set; }

        public int PersonId { get; set; }

        public int InnoculationTypeId { get; set; }

       
        public DateTime? InnoculationDate { get; set; }

        
        public DateTime? ExpirationDate { get; set; }

        [Column(TypeName = "text")]
        public string Notes { get; set; }

        [StringLength(50)]
        public string EnteredBy { get; set; }

        
        public DateTime? EnteredDate { get; set; }

        [StringLength(50)]
        public string ModifiedBy { get; set; }

        
        public DateTime? ModifiedDate { get; set; }

        public virtual DdlInnoculationType DdlInnoculationType { get; set; }

        public virtual Person Person { get; set; }
    }
}
