namespace ExecViewHrk.EfClient
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("RecruitingConfig")]
    public partial class RecruitingConfig
    {
        [Key]
        public int ConfigId { get; set; }

        public string HomePageHtml { get; set; }

        public string IntroductionHtml { get; set; }

        public string ReviewSubmitHtml { get; set; }

        public string AttestationHtml { get; set; }

        [StringLength(100)]
        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}
