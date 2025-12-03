using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Models
{
    public class PositionsVM
    {
        public int PositionId { get; set; }
        public string PositionCode { get; set; }
        public string PositionDescription { get; set; }
        public bool IsPositionActive { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
}
}
