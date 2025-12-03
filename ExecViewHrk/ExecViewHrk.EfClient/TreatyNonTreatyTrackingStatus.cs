using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace ExecViewHrk.EfClient
{
  public class TreatyNonTreatyTrackingStatus
    {
        [Key]
        public int PayperiodNumber { get; set; }
        public int Paygroup { get; set; }               
        public DateTime ExportedDate { get; set; }
        public bool Isexported { get; set; }
        public DateTime PayperiodStartDate { get; set; }
        public DateTime PayperiodEndDate { get; set; }       
        public string FileNumber { get; set; }
        public decimal RegHours { get; set; }
        public decimal PayRate { get; set; }
    }
}
