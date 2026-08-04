namespace ExecViewHrk.EfClient
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("JobApplicationAnswers")]
    public partial class JobApplicationAnswer
    {
        [Key]
        public int AnswerId { get; set; }

        public int ApplicationId { get; set; }

        public int QuestionId { get; set; }

        public string AnswerText { get; set; }

        public virtual JobApplication JobApplication { get; set; }
    }
}
