namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    public class TimeCardSessionInOutConfigs
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        [Key]
        public int TimeCardSessionId { get; set; }
        public bool Toggle { get; set; }
        public string  Session { get; set; }
        public int? MaxHours { get; set; }
    }
}
