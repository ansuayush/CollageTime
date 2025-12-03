namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ManagerDepartment
    {
        public int ManagerDepartmentId { get; set; }

        public int ManagerId { get; set; }

        public int DepartmentId { get; set; }

        public bool? IsDesignated { get; set; }

        public int CompanyCodeId { get; set; }

        public virtual Department Department { get; set; }

        public virtual Manager Manager { get; set; }
    }
}
