using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class FundHistoryVm
    {
        public DateTime? EffectiveDate { get; internal set; }
        public int? FundID { get; internal set; }
        public int ID { get; internal set; }
        public decimal Amount { get; internal set; }
        public string FundDescription { get; internal set; }
        public string FundCode { get; internal set; }
        public IEnumerable<FundHistoryVm> FundDefinitionList { get; set; }


    }

    public class FundHistoryAddVm
    {
        [Required]
        public DateTime? EffectiveDate { get; internal set; }
        [Required]
        public decimal? Amount { get; internal set; }
        [Required]
        public string FundDescription { get; internal set; }
        [Required]
        public string FundCode { get; internal set; }
    }
}