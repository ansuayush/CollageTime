namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Job
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
      
        public int JobId { get; set; }

        public int CompanyCodeId { get; set; }        

        [Required]
        [StringLength(10)]
        public string JobCode { get; set; }

        public string Code { get; set; }

        public bool IsJobActive { get; set; }

        [StringLength(100)]
        public string JobDescription { get; set; }

        public virtual ddlJobClasses DdlJobClasses { get; set; }

        public virtual ddlWorkersCompensations DdlWorkersCompensations { get; set; }

        public virtual ddlEEOJobCodes DdlEEOJobCodes { get; set; }

        public virtual ddlFLSAs DdlFLSAs { get; set; }

        public virtual ddlEEOJobTrainingStatuses DdlEEOJobTrainingStatuses { get; set; }
        public virtual ddlUnions DdlUnions { get; set; }
        public virtual ddlJobFamilys DdlJobFamilys { get; set; }

        

        public virtual CompanyCode CompanyCode { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Position> Positions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TimeCard> TimeCards { get; set; }
        
        public short? jobClassID { get; set; }
        public short? workersCompensationID { get; set; }
        public short? eeoJobCodeID { get; set; }
        public short? eeoJobTrainingStatusID { get; set; }
        public short? FLSAID { get; set; }
        public short? unionID { get; set; }

        public DateTime? createdDate { get; set; }
        public DateTime? endDate { get; set; }
        public DateTime? lastEvaluationDate { get; set; }
        public DateTime? enteredDate { get; set; }
        public string salaryRange { get; set; }
        public string enteredBy { get; set; }
        [StringLength(100)]
        public string title { get; set; }
        public string requirements { get; set; }
        public string Notes { get; set; }
        public short? JobFamilyId { get; set; }

     
    }
}
