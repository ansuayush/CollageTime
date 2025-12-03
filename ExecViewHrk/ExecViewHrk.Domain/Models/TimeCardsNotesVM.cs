using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Models
{
    public class TimeCardsNotesVM
    {
        public int TimeCardsNotesId { get; set; }
        public int CompanyCodeId { get; set; }
        public string FileNumber { get; set; }
        public int? TimeCardId { get; set; }
        [Column(TypeName = "date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0: d}")]
        [DataType(DataType.Date)]
        public DateTime ActualDate { get; set; }
        public int? EmployeeId { get; set; }
        public string Notes { get; set; }
    }
}
