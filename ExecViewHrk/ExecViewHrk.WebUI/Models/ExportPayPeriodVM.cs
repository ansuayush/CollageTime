using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class ExportPayPeriodVM
    {
        public string CoCode { get; set; }
        public string BatchID { get; set; }
        public string FileNumber { get; set; }
        public string PayNumber { get; set; }
        public string RegHours { get; set; }
        public string OTHours { get; set; }
        public string Hours3Code { get; set; }
        public string Hours3Amount { get; set; }
        public string Hours4Code { get; set; }
        public string Hours4Amount { get; set; }
        public string RegEarnings { get; set; }
        public string OTEarnings { get; set; }
        public string Earnings3Code { get; set; }
        public string Earnings3Amount { get; set; }
        public string Earnings4Code { get; set; }
        public string Earnings4Amount { get; set; }
        public string Earnings5Code { get; set; }
        public string Earnings5Amount { get; set; }
        public string TempRate { get; set; }
        public string MemoCode1 { get; set; }
        public string MemoAmount1 { get; set; }
        public string MemoCode2 { get; set; }
        public string MemoAmount2 { get; set; }
        public string CancelPay { get; set; }
        public string CostNumber { get; set; }
        public string Department { get; set; }
    }
}