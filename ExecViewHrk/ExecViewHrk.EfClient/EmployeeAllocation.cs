namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class EmployeeAllocation
    {
        public int EmployeeAllocationId { get; set; }

        public int EmployeeId { get; set; }

        public int DepartmentId { get; set; }

        public decimal AllocationPercent { get; set; }

        public virtual Department Department { get; set; }

        public virtual Employee Employee { get; set; }
    }
}
