using System;

namespace ExecViewHrk.Models
{
    public class TimeCardMobileVm
    {
        public int EmployeeId { get; set; }

        public int PositionId { get; set; }

        public string FileNumber { get; set; }

        public DateTime? ActualDate { get; set; }

        public DateTime? TimeIn { get; set; }

        public DateTime? TimeOut { get; set; }

        public DateTime? LunchOut { get; set; }

        public DateTime? LunchBack { get; set; }
    }
}
