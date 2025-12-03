using ExecViewHrk.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class PersonExaminationVm
    {
        public int PersonExaminationId { get; set; }

        [Required]
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        [Required(ErrorMessage ="The Medical Examination Type field is required.")]
        public int MedicalExaminationTypeId { get; set; }

         public string MedicalExaminationDescription { get; set; }
        [Required(ErrorMessage ="The Examination Date field is required.")]
        public DateTime? ExaminationDate { get; set; }
        public string Examiner { get; set; }

        public DateTime? NextScheduledExamination { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        public string EnteredBy { get; set; }
        public DateTime? EnteredDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<DropDownModel> MedicalExaminationDropDownList { get; set; }
    }
}