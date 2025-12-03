using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Models
{
    public class ContractReportVM
    {
        public string EmployeeName { get; set; }
        public string FileNumber { get; set; }
        public string PositionCode { get; set; }
        public string PositionDescription { get; set; }
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public decimal? RemainingBalance { get; set; }
        public string GLCode { get; set; }
        public int? NoofPayPeriods { get; set; }

        public string EarningCode { get; set; }
        public decimal? ContractAmount { get; set; }
        public Nullable<decimal> OutsidePaymentAdjustment { get; set; }
        public string Semester { get; set; }
        public string Notes { get; set; }

    }
}
