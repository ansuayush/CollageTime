using System;

namespace ExecViewHrk.Domain.Models
{
    public class TimeCardPayload
    {
        public int EmployeeId { get; set; }

        public int PositionId { get; set; }

        public int PunchType { get; set; }

        public DateTime PunchTime { get; set; }

        public int CompanyCode { get; set; }

        public string UserName { get; set; }

        public string FileName { get; set; }

    }
}
