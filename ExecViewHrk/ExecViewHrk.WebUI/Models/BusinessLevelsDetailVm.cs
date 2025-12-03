using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ExecViewHrk.Models;

namespace ExecViewHrk.WebUI.Models
{
    public class BusinessLevelsDetailVm
    {
        public int BusinessLevelNbr { get; set; }

        //[Required]
        //[Display(Name = "Buisness Code")]
        public string BusinessLevelCode { get; set; }
        public string BusinessLevelNotes { get; set; }

        //[Required]
        //[Display(Name = "Buisness Title")]
        public string BusinessLevelTitle { get; set; }
        public byte? BusinessLevelTypeNbr { get; set; }

       public int? FedralEINNbr { get; set; }
         public int? EEoFileStatusNbr { get; set; }
        public int? PayFrequencyId { get; set; }
        public int? ParentBULevelNbr { get; set; }
        public string ParentBULevelTitle { get; set; }
        public int ? LocationId { get; set; }
        public string LocationIdDesc { get; set; }
        public string EEoDesc { get; set; }
        public string EIN { get; set; }
        public string BusinessLevelTypeDescription { get; set; }
        public string LocationDescription { get; set; }
        public string PayFrequency { get; set; }
        public int SchedeuledHours { get; set; }
        public bool Active { get; set; }
        public string BudgetReported { get; set; }
        public DateTime? EnteredDate { get; set; }
        public string EnteredBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }

        public List<DropDownModel> ParentBULevelDropDownList { get; set; }
        public List<DropDownModel> LocationDropDownList { get; set; }
        public List<DropDownModel> EINDropDownList { get; set; }
        public List<DropDownModel> EEODropDownList { get; set; }
        public List<DropDownModel> PayFrequencyDropDownLiat { get; set; }

        public List<DropDownModel> BusinessLevelTypeDropDownLiat { get; set; }
    }
}