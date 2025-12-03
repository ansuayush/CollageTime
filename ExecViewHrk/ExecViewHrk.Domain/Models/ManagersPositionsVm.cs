using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Models
{
    public class ManagersPositionsVm
    {
        public int ID { get; set; }
        [Required]
        public int ManagerID { get; set; }
        [Required]
        public short PositionID { get; set; }
        public string PersonName { get; set; }
        public List<DropDownModel> PersonsList { get; set; }
        public List<DropDownModel> PositionsList { get; set; }
    }
}
