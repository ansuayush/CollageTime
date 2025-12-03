using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Models
{
   public class TimeCardSessionInOutConfigsVm
    {
        public int TimeCardSessionId { get; set; }
        public bool Toggle { get; set; }
        public string Session { get; set; }
        public int? MaxHours { get; set; }
    }
}
