using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.SqlData.Models
{
    public class TimeCardApprovalReportCollection
    {
        public int EmployeeId { get; set; }
        public double? RegularHours { get; set; }
        public double? CodedHours { get; set; }
        public double? OverTime { get; set; }
        public double? Emp_PayPeriodTotal { get; set; }
        public bool Approved { get; set; }
        public string PersonName { get; set; }

        public override string ToString()
        {
            return string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\""
                             , PersonName, RegularHours, CodedHours, OverTime, Emp_PayPeriodTotal, Approved);

        }
    }
}
