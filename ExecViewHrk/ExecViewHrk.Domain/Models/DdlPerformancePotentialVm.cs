using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Domain.Models
{
    public class DdlPerformancePotentialVm
    {
        [Key]
        public short Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Description { get; set; }
        [Required]
        [StringLength(10)]
        public string Code { get; set; }
        [Required]
        public bool Active { get; set; }
    }
}
