using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Models
{
    public class DropDownModel
    {
        public string keyvalue { get; set; }
        public string keydescription { get; set; }
        public string HoursCodeId { get; set; }
        public string HoursCodeCode { get; set; }
    }


    public class DropDownModelwithid
    {
        public int id { get; set; }
        public string keyvalue { get; set; }
        public string keydescription { get; set; }
    }
}
