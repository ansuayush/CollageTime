using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExecViewHrk.EfClient
{
    [Table("TimeCardNotesArchive")]
   public class TimeCardNotesArchive
    {
        [Key]
        public int TimeCardNotesArchiveId { get; set; }
        public int CompanyCodeId { get; set; }
        public string FileNumber { get; set; }
        [Column(TypeName = "date")]
        public DateTime ActualDate { get; set; }
        public int? EmployeeId { get; set; }
        public string Notes { get; set; }
        public int? TimeCardId { get; set; }
    }
}
