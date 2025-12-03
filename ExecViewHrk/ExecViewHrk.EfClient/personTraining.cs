namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PersonTraining
    {
        public int PersonTrainingId { get; set; }

        public int PersonId { get; set; }

        public byte TrainingCourseId { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? RecommendationDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? RequiredByDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? ScheduledDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? CompletedDate { get; set; }

        public bool? PositionCurriculum { get; set; }

        [Column(TypeName = "text")]
        public string Notes { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? StartDate { get; set; }

        [Column(TypeName = "money")]
        public decimal? TravelCost { get; set; }

        [Column(TypeName = "money")]
        public decimal? HotelMealsExpense { get; set; }

        [Column(TypeName = "money")]
        public decimal? ActualCourseCost { get; set; }

        public bool? QualityRelated { get; set; }

        public bool? OshaRelated { get; set; }

        [StringLength(50)]
        public string Venue { get; set; }

        public bool? ManagerApproved { get; set; }

        public int? ApprovingManager { get; set; }

        [StringLength(50)]
        public string EnteredBy { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? EnteredDate { get; set; }

        [StringLength(50)]
        public string CompleteStatus { get; set; }

        [StringLength(3)]
        public string Grade { get; set; }

        [StringLength(50)]
        public string EnrollStatus { get; set; }

        public virtual DdlTrainingCours DdlTrainingCours { get; set; }

        public virtual Person Person { get; set; }
    }
}
