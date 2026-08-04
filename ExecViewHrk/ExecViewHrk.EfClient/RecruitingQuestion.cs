namespace ExecViewHrk.EfClient
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("RecruitingQuestions")]
    public partial class RecruitingQuestion
    {
        [Key]
        public int QuestionId { get; set; }

        [Required]
        [StringLength(500)]
        public string QuestionText { get; set; }

        [Required]
        [StringLength(30)]
        public string QuestionType { get; set; }

        [StringLength(1000)]
        public string Choices { get; set; }

        public int WizardPage { get; set; }

        public int SortOrder { get; set; }

        public bool IsRequired { get; set; }

        public bool IsActive { get; set; }
    }
}
