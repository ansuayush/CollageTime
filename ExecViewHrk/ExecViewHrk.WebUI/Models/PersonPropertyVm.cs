using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ExecViewHrk.Models;

namespace ExecViewHrk.WebUI.Models
{
    public class PersonPropertyVm
    {
      
        public int PersonPropertyTypeId { get; set; }

        [Required]
        public int PersonId { get; set; }
        public string PersonName { get; set; }

        public int PropertyTypeId { get; set; }

        //[Required]
        public string PropertyTypeDescription { get; set; }
   
        public DateTime? AcquiredDate { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public decimal? EstimatedValue { get; set; }
        public string AssetNumber { get; set; }   
        public string PropertyDescription { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }     
        public string EnteredBy { get; set; }     
        public DateTime? EnteredDate { get; set; }      
        public string ModifiedBy { get; set; }       
        public DateTime? ModifiedDate { get; set; }
        public List<DropDownModel> PropertyDropDownList { get; set; }
    }
}