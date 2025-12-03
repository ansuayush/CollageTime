namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PersonEducation
    {
        public int PersonEducationId { get; set; }

        public int PersonId { get; set; }

        public int? QualificationTypeId { get; set; }

        public int? MajorId { get; set; }

        public int? MinorId { get; set; }

        public int? LevelAchievedId { get; set; }

        [StringLength(10)]
        public string Grade { get; set; }

        [StringLength(10)]
        public string Gpa { get; set; }

        [StringLength(10)]
        public string CreditsEarned { get; set; }

        
        public DateTime? PlannedStart { get; set; }

        
        public DateTime? PlannedCompletion { get; set; }

        
        public DateTime? ActualCompletion { get; set; }

        public int? EducationEstablishmentId { get; set; }

        [Column(TypeName = "text")]
        public string Notes { get; set; }

        [StringLength(50)]
        public string EnteredBy { get; set; }

        
        public DateTime? EnteredDate { get; set; }

        [StringLength(50)]
        public string ModifiedBy { get; set; }

        
        public DateTime? ModifiedDate { get; set; }

        public virtual DdlDegreeType DdlDegreeType { get; set; }

        public virtual DdlDegreeType DdlDegreeType1 { get; set; }

        public virtual DdlEducationEstablishment DdlEducationEstablishment { get; set; }

        public virtual DdlEducationLevel DdlEducationLevel { get; set; }

        public virtual DdlQualificationType DdlQualificationType { get; set; }

        public virtual Person Person { get; set; }
    }
}
