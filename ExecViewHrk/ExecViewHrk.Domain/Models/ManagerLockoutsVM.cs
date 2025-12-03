using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Models
{
   public class ManagerLockoutsVM
    {
        public int ManagerLockoutsId { get; set; }

        public int PayPeriodId { get; set; }

        public string ManagerUserName { get; set; }
        public int? PersonId { get; set; }
    }
}
