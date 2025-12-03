using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ExecViewHrk.WebUI.Models
{
    public class PersonEducationVm
    {
        public int PersonEducationId { get; set; }

        [Required]
        public int PersonId { get; set; }
        public string PersonName { get; set; }

        public int? QualificationTypeId { get; set; }
        [Required]
        public string QualificationTypeDescription { get; set; }

        public int? MajorId { get; set; }
        [Required]
        public string MajorDescription { get; set; }

        public int? MinorId { get; set; }
        [Required]
        public string MinorDescription { get; set; }
        
        public int? EducationEstablishmentId { get; set; }
        [Required]
        public string EducationEstablishmentDescription { get; set; }

        public int? LevelAchievedId { get; set; }
        public string LevelDescription { get; set; }
        
        public string Grade { get; set; }
        public string Gpa { get; set; }
        public string CreditsEarned { get; set; }

        public DateTime? PlannedStart { get; set; }
        public DateTime? PlannedCompletion { get; set; }
        public DateTime? ActualCompletion { get; set; }


        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        public string EnteredBy { get; set; }
        public DateTime? EnteredDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

    }
}