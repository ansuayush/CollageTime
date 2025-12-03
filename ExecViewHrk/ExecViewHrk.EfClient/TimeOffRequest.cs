namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TimeOffRequest
    {
        public int TimeOffRequestId { get; set; }

        public int CompanyCodeId { get; set; }

        public int EmployeeId { get; set; }
        public int HoursCodeId { get; set; }

        public decimal TimeOfftHours { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime TimeOffDate { get; set; }
        public byte RequestStatus { get; set; }

        public virtual CompanyCode CompanyCode { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual HoursCode HoursCode { get; set; }
    }
}
