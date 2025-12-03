using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExecViewHrk.EfClient
{
   public class ManagerLockouts
    {
        public int ManagerLockoutsId { get; set; }

        public int PayPeriodId { get; set; }

        public string ManagerUserName { get; set; }

        public virtual PayPeriod PayPeriod { get; set; }
    }
}
