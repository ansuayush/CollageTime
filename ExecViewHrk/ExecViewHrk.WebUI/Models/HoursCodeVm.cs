using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class HoursCodeVm
    {
        public int HoursCodeId { get; set; }

        [UIHint("GridForeignKey")]
        public int CompanyCodeId { get; set; }

        [Required]
        public string HoursCodeCode { get; set; }

        [Required]
        public string HoursCodeDescription { get; set; }

        [UIHint("GridForeignKey")]
        public int ADPFieldMappingId { get; set; }

        [UIHint("GridForeignKey")]
        public int? ADPAccNumberId { get; set; }

        public decimal? RateOverride { get; set; }

        public decimal? RateMultiplier { get; set; }

        public bool ExcludeFromOT { get; set; }

        public bool SubtractFromRegular { get; set; }

        public bool NonPayCode { get; set; }

        public bool IsHoursCodeActive { get; set; }

    }
}