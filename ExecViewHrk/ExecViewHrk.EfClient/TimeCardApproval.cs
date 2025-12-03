namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TimeCardApproval
    {
        public int TimeCardApprovalId { get; set; }

        public int EmployeeId { get; set; }

        public int PayPeriodId { get; set; }

        public bool Approved { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual PayPeriod PayPeriod { get; set; }
        public string ManagerId { get; set; }
    }
}
