namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Department
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Department()
        {
            EmployeeAllocations = new HashSet<EmployeeAllocation>();
            Employees = new HashSet<Employee>();
            ManagerDepartments = new HashSet<ManagerDepartment>();
            Positions = new HashSet<Position>();
            TimeCards = new HashSet<TimeCard>();
        }

        public int DepartmentId { get; set; }

        public int CompanyCodeId { get; set; }

        [Required]
        [StringLength(50)]
        public string DepartmentDescription { get; set; }

        [Required]
        [StringLength(10)]
        public string DepartmentCode { get; set; }

        public bool IsDepartmentActive { get; set; }
        public bool? IsDeleted { get; set; }
        public string  DeletedBy { get; set; }
        public string UserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public virtual CompanyCode CompanyCode { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeAllocation> EmployeeAllocations { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Employee> Employees { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ManagerDepartment> ManagerDepartments { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Position> Positions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TimeCard> TimeCards { get; set; }
    }
}
