namespace ExecViewHrk.EfAdmin
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MessageLog")]
    public partial class MessageLog
    {
        public int MessageLogId { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; }

        [Required]
        public string Message { get; set; }

        public DateTime DateTime { get; set; }

        [StringLength(100)]
        public string Source { get; set; }
    }
}
