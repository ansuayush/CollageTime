using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Models
{
    public class TimeCardDisplayColumnVM
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TimeCardTypeId { get; set; }

        public bool Day { get; set; }

        public bool ActualDate { get; set; }

        public bool DailyHours { get; set; }

        public bool HoursCodeId { get; set; }

        public bool Hours { get; set; }

        public bool EarningsCodeId { get; set; }

        public bool EarningsAmount { get; set; }

        public bool TempDeptId { get; set; }

        public bool TempJobId { get; set; }

        public bool TimeIn { get; set; }

        public bool TimeOut { get; set; }

        public bool LunchOut { get; set; }

        public bool LunchBack { get; set; }

        public bool IsApproved { get; set; }

        public bool ApprovedBy { get; set; }

        public bool AutoFill { get; set; }

        public bool Total { get; set; }

    }
}
