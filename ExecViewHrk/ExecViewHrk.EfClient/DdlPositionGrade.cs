using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.EfClient
{
 public partial class DdlPositionGrade
    {
        [Key]
        public int PositionGradeID { get; set; }

        public string Description { get; set; }

        public string Code { get; set; }

        public bool Active { get; set; }
    }
}
