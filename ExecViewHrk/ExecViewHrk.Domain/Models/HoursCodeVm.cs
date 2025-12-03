using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExecViewHrk.Models
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

        [DisplayName("Active")]
        public bool IsHoursCodeActive { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsRetro { get; set; }

    }
}