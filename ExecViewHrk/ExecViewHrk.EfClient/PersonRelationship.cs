namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PersonRelationship
    {
        public int PersonRelationshipId { get; set; }

        public int PersonId { get; set; }
        
        public int RelationshipTypeId { get; set; }

        public int RelationPersonId { get; set; }

        public bool? Dependent { get; set; }

        public bool? EmergencyContact { get; set; }

        public bool? Garnishment { get; set; }

        [StringLength(50)]
        public string EnteredBy { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? EnteredDate { get; set; }

        [StringLength(50)]
        public string ModifiedBy { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? ModifiedDate { get; set; }

        public virtual DdlRelationshipType DdlRelationshipType { get; set; }

      

        public virtual Person Person { get; set; }

        public virtual Person Person1 { get; set; }
    }
}
