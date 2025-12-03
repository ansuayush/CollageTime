using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.EfClient
{
   public  class PerformanceProfileSections
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [StringLength(50)]
        public string SectionName { get; set; }
        [Required]
        [StringLength(200)]
        [DataType(DataType.MultilineText)]
        public string Header { get; set; }
        [Required]
        public int PerProfileID { get; set; }
        [Required]
        public int NumRows { get; set; }
        [Required]
        public int MaxCharacters { get; set; }
        [Required]
        public decimal Weight { get; set; }
        [Required]
        public int Position { get; set; }
        [StringLength(50)]
        public string EnteredBy { get; set; }
        public DateTime? EnteredDate { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
