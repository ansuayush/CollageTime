using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Models
{
    public class NonStandardSalaryPayment
    {
        public int SalaryPaymentId { get; set; }

        public string SalaryPaymentCode { get; set; }

        public string SALMemoCode { get; set; }

        public decimal? SALMemoAmount { get; set; }
        public string SalMemoAmountDescription { get; set; }

        public string DPAMemoCode { get; set; }

        public decimal? DPAMemoAmount { get; set; }

        public bool CancelPay { get; set; }
        public string CancelPayDescription { get; set; }
    }
}
