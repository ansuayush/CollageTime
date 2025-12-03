using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.EfClient
{
   public partial class DdlPositionCategory
    {
        [Key]
        public int PositionCategoryID { get; set; }
        public string description { get; set; }
        public string code { get; set; }
        public bool active { get; set; }
    }
}
