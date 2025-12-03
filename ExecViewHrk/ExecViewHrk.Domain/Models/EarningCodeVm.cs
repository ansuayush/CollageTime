using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExecViewHrk.Models
{
    //EarningCodeVm for data filter
    public class EarningCodeVm
    {
        [Required]
        public int EarningsCodeId { get; set; }

        [Required(ErrorMessage = "Company Code is Required")]
        public int CompanyCodeId { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Code cannot be longer than 2 characters.")]
        public string EarningsCode { get; set; }

        [Required]
        [StringLength(20)]
        public string EarningsCodeDescription { get; set; }

        public int ADPFieldMappingId { get; set; }

        public bool Active { get; set; }

        [StringLength(20)]
        public string EarningsCodeOffset { get; set; }

        [StringLength(20)]
        public string DeductionCodeOffset { get; set; }
        public string EarningsCodeCode { get; set; } //Created for Time Card EarningCode Dropdown. Property should be as per Database Entity.

        public bool TreatyCode { set; get; }
        public bool IsDefault { get; set; }
        public int? OldEarningsCodeId { get; set; }
    }
}