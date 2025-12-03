using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.EfClient
{
    public class ManagersPositions
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public int ManagerID { get; set; }
        [Required]
        public int PositionID { get; set; }
    }
}
