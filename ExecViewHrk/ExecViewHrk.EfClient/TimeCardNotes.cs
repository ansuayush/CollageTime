using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace ExecViewHrk.EfClient
{
   public class TimeCardNotes
    {
        [Key]
        public int TimeCardsNotesId { get; set; }
        public int CompanyCodeId { get; set; }
        public int? EmployeeId { get; set; }
        public string FileNumber { get; set; }
        public int? TimeCardId { get; set; }
        public DateTime ActualDate { get; set; }

        public string Notes { get; set; }

        public virtual CompanyCode CompanyCode { get; set; }

        public virtual Employee Employee { get; set; }
    }
}
