using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class PayPeriodVm
    {
        public int PayPeriodId { get; set; }

        public short? CompanyCodeId { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public byte PayFrequencyId { get; set; }

        public bool IsPayPeriodActive { get; set; }

        public bool LockoutEmployees { get; set; }

        public bool LockoutManagers { get; set; }

        public bool IsArchived { get; set; }

    }
}