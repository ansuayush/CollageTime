namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PersonAdditional
    {
        public int PersonAdditionalId { get; set; }

        public int PersonId { get; set; }

        [StringLength(50)]
        public string BirthPlace { get; set; }

        public int? EeoTypeId { get; set; }

        public int? ApplicantSourceId { get; set; }

        public int? CitizenshipId { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? CitizenshipDate { get; set; }

        [StringLength(50)]
        public string Veteran { get; set; }
        public bool? Other { get; set; }
        public bool? SpecialDisabled { get; set; }
        public bool? Vietnam { get; set; }

        public bool Disabled { get; set; }

        [Column(TypeName = "text")]
        public string DisabledComments { get; set; }

        public int? HospitalId { get; set; }

        [StringLength(50)]
        public string Doctor { get; set; }

        public bool BloodDonor { get; set; }

        public bool Smoker { get; set; }

        public bool AdvancedDirective { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? EarlyRetirementDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? NormalRetirementDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? ExpectedRetirementDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? DateOfDeath { get; set; }

        [Column(TypeName = "text")]
        public string Notes { get; set; }

        [StringLength(50)]
        public string EnteredBy { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? EnteredDate { get; set; }

        [StringLength(50)]
        public string ModifiedBy { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? ModifiedDate { get; set; }

        public virtual DdlCitizenship DdlCitizenship { get; set; }

        public virtual DdlEeoType DdlEeoType { get; set; }

        public virtual DdlHospital DdlHospital { get; set; }
        public virtual DdlApplicantSource DdlApplicantSource { get; set; }

        public virtual Person Person { get; set; }
    }
}
