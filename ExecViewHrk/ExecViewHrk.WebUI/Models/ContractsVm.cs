using ExecViewHrk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class ContractsVm
    {
        public int Id { get; set; }

        public int? PayPeriod { get; set; }

        public int? Status1FlagCode { get; set; }

        public string Status1FlagCodeDesc { get; set; }

        public string EarningCodes { get; set; }

        public string MemoCode1 { get; set; }

        public decimal? MemoCode1Amount { get; set; }

        public string MemoCode2 { get; set; }

        public decimal? MemoCode2Amount { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public decimal? Amounts { get; set; }

        public string Department { get; set; }

        public decimal? AmountsInDollar { get; set; }

        public string Job { get; set; }

        public string Notes { get; set; }

        public string AddNewContract { get; set; }
        public Nullable<int> SemisterId { set; get; }

         public string Semister { get; set; }
        public bool TreatyCode { get; set; }

        public string EarningsCode { get; set; }

        public Nullable<int> EarningsCodeId { get; set; }

        public Nullable<decimal> ADJlimits { get; set; }

        public int EpositionId { get; set; }
        public string GLCode { get; set; }

        public Nullable<int> GLCodeId { get; set; }

    }
}