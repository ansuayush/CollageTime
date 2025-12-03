using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ExecViewHrk.WebUI.Models
{
    public class EmployeeI9DocumentVm
    {
        public int EmployeeI9DocumentId { get; set; }

        [Required]
        public int EmployeeId { get; set; }
        public int EmploymentNumber { get; set; }

        public int PersonId { get; set; }
        [Required]
        public string PersonName { get; set; }

        public byte? I9DocumentTypeId { get; set; }
        [Required (ErrorMessage="The I9 Document Type Description field is required.")] 
        public string I9DocumentTypeDescription { get; set; }
       
        public DateTime? PresentedDate { get; set; }
        public DateTime? ExpirationDate { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        public string EnteredBy { get; set; }
        public DateTime EnteredDate { get; set; }

    }
}