using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace ExecViewHrk.Domain.Models
{
   public class TimeCradsNotesArchiveVM
    {
        public int TimeCardsNotesId { get; set; }
        public int CompanyCodeId { get; set; }
        public string FileNumber { get; set; }
        [Column(TypeName = "date")]
        public DateTime ActualDate { get; set; }

        public string Notes { get; set; }
    }
}
