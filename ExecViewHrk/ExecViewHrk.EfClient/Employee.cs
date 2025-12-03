namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Employee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Employee()
        {
            E_Positions = new HashSet<E_Positions>();
            EmployeeAllocations = new HashSet<EmployeeAllocation>();
            EmployeeI9Documents = new HashSet<EmployeeI9Documents>();
            TimeCardApprovals = new HashSet<TimeCardApproval>();
            TimeCards = new HashSet<TimeCard>();
            TimeOffRequests = new HashSet<TimeOffRequest>();
        }

        public int EmployeeId { get; set; }

        public int PersonId { get; set; }

        [StringLength(50)]
        public string CompanyCode { get; set; }

        public decimal? Rate { get; set; }

        public int? RateTypeId { get; set; }

        [StringLength(50)]
        public string FileNumber { get; set; }

        public int EmploymentNumber { get; set; }

        public int? EmploymentStatusId { get; set; }


        public DateTime HireDate { get; set; }


        public DateTime? TerminationDate { get; set; }


        public DateTime? PlannedServiceStartDate { get; set; }


        public DateTime? ActualServiceStartDate { get; set; }

        public int? EmployeeTypeID { get; set; }


        public DateTime? ProbationEndDate { get; set; }


        public DateTime? TrainingEndDate { get; set; }


        public DateTime? SeniorityDate { get; set; }

        public int? PayFrequencyId { get; set; }

        [StringLength(2)]
        public string FedExemptions { get; set; }

        public int? WorkedStateTaxCodeId { get; set; }

        public int? MaritalStatusID { get; set; }

        public decimal? Hours { get; set; }

        [StringLength(50)]
        public string EnteredBy { get; set; }


        public DateTime? EnteredDate { get; set; }

        [StringLength(50)]
        public string ModifiedBy { get; set; }


        public DateTime? ModifiedDate { get; set; }

        public int? DepartmentId { get; set; }

        public int? CompanyCodeId { get; set; }

        public int? TimeCardTypeId { get; set; }

        public virtual CompanyCode CompanyCode1 { get; set; }

        public virtual DdlEmployeeType DdlEmployeeType { get; set; }

        public virtual DdlEmploymentStatus DdlEmploymentStatus { get; set; }

        public virtual DdlMaritalStatus DdlMaritalStatus { get; set; }

        public virtual DdlPayFrequency DdlPayFrequency { get; set; }

        public virtual DdlRateType DdlRateType { get; set; }

        public virtual DdlState DdlState { get; set; }

        public virtual DdlTimeCardType DdlTimeCardType { get; set; }

        public virtual Department Department { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<E_Positions> E_Positions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeAllocation> EmployeeAllocations { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeI9Documents> EmployeeI9Documents { get; set; }

        public virtual Person Person { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TimeCardApproval> TimeCardApprovals { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TimeCard> TimeCards { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TimeOffRequest> TimeOffRequests { get; set; }


        public Nullable<int> EarningsCodeId { set; get; }
        public Nullable<decimal> Amount { set; get; }
        public Nullable<decimal> TreatyLimit { get; set; }
        public Nullable<decimal> NonTreatyLimit { get; set; }
        public DateTime? AdpYear { get; set; }

        public Nullable<decimal> AdpWSLimit { get; set; }

        public bool IsStudent { get; set; }

        public int? ReportToPersonId { get; set; }
        public bool IsManager { get; set; }
        public Nullable<decimal> UsedAmount { set; get; }

    }
}
