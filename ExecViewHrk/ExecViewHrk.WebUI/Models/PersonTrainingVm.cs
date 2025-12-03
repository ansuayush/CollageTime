using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ExecViewHrk.WebUI.Models
{
    public class PersonTrainingVm
    {

        public int PersonTrainingId { get; set; }

        [Required]
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        
        public byte TrainingCourseId { get; set; }
        [Required]
        public string TrainingCourseDescription { get; set; }
       
        public DateTime? RecommendationDate { get; set; }
        public DateTime? RequiredByDate { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        

        public string Grade { get; set; }
        public decimal? TravelCost { get; set; }
        public decimal? HotelMealsExpense { get; set; }
        public decimal? ActualCourseCost { get; set; }
        public bool? QualityRelated { get; set; }
        public bool? OshaRelated { get; set; }
        public string Venue { get; set; }

        public DateTime? StartDate { get; set; }
        public bool? PositionCurriculum { get; set; }
        public bool? ManagerApproved { get; set; }
        public int? ApprovingManager { get; set; }
        public string CompleteStatus { get; set; }
        public string EnrollStatus { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        public string EnteredBy { get; set; }
        public DateTime? EnteredDate { get; set; }

    }
}