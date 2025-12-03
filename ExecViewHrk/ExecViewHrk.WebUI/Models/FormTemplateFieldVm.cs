using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ExecViewHrk.WebUI.Models
{
    public class FormTemplateFieldVm
    {

        public int FormTemplateFieldId { get; set; }

        //[Required]
        //[StringLength(9, MinimumLength = 9,ErrorMessage="SSN must be 9 digits")]
        //public string Ssn { get; set; }
        

        public int FormTemplateId { get; set; }

        [Required]
        public string HtmlId { get; set; }

        [Required]
        public string Type { get; set; }


        public string Value { get; set; }


        public string Label { get; set; }

        public int VisualWidth { get; set; }

        [Required]
        [UIHint("DecimalSpin")]
        public Decimal Position { get; set; }

        [Required]
        public bool Required { get; set; }

        [UIHint("Integer")]
        [Range(1, 2,
        ErrorMessage = "Value must be 1 or 2.")]
        public int ColumnNumber { get; set; }

        public string SelectionGroup { get; set; }

        public string CheckBoxRadioGroupName { get; set; }

        
    }
}