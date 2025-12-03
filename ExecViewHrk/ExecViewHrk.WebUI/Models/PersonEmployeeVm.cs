using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text.RegularExpressions;
using ExecViewHrk.Models;

namespace ExecViewHrk.WebUI.Models
{
    public class PersonEmployeeVm // : IValidatableObject
    {
        public int EmployeeId { get; set; }
        
        [Required]
        public int PersonId { get; set; }
        public string PersonName { get; set; }

        public byte? EmploymentStatusId { get; set; }

        
        [Display(Name = "Employment Status Description")]
        public string EmploymentStatusDescription { get; set; }

        public short? EmployeeTypeID { get; set; }

        
        [Display(Name = "Employee Type Description")]
        public string EmployeeTypeDescription { get; set; }

        public byte? PayFrequencyId { get; set; }
       
       
        [Display(Name = "Pay Status Description")]
        public string PayFrequencyDescription { get; set; }

        public int? MaritalStatusID { get; set; }

        
        [Display(Name = "Marital Status Description")]
        public string MaritalStatusDescription { get; set; }

        public byte? WorkedStateTaxCodeId { get; set; }

        
        [Display(Name = "Worked State Title")]
        public string WorkedStateTitle { get; set; }

        public int? RateTypeId { get; set; }

        
        [Display(Name = "Rate Type Description")]
        public string RateTypeDescription { get; set; }

        public short? TimeCardTypeId { get; set; }
        //[Required]        
        public string TimeCardTypeDescription { get; set; }


        public decimal? Rate { get; set; }
        public string FileNumber { get; set; }

        [Display(Name = "Employment Number")]
        public byte EmploymentNumber { get; set; }

        
        [Display(Name = "Fed Exemptions")]
        [RegularExpression(@"^[0-9]{1,2}$", ErrorMessage = "Fed Exemptions Must be between 00-99 "), StringLength(2)]
        public string FedExemptions { get; set; }

        public decimal? Hours { get; set; }


        public static byte MaxEmploymentNumber;

        
        [Display(Name = "Hire Date")]
        public DateTime HireDate { get; set; }

        [Display(Name = "Termination Date")]
        public DateTime? TerminationDate { get; set; }

        [Display(Name = "Planned Service Start Date")]
        public DateTime? PlannedServiceStartDate { get; set; }

        [Display(Name = "Actual Service Start Date")]
        public DateTime? ActualServiceStartDate { get; set; }

        [Display(Name = "Probation End Date")]
        public DateTime? ProbationEndDate { get; set; }

        [Display(Name = "Training End Date")]
        public DateTime? TrainingEndDate { get; set; }

        [Display(Name = "Seniority Date")]
        public DateTime? SeniorityDate { get; set; }

        [DataType(DataType.MultilineText)]
        public string EnteredBy { get; set; }
        public DateTime? EnteredDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public string LenghtOfEmployment { get; set; }

        public short CompanyCodeId { get; set; }
        
        [Display(Name = "Company Code")]
        public string CompanyCode { get; set; }

        public int BusinessLevelNbr { get; set; }

        public List<DropDownModel> MaritalStatusList { get; set; }
        public List<DropDownModel> EmployeeTypeList { get; set; }
        public List<DropDownModel> EmploymentStatusList { get; set; }
        public List<DropDownModel> PayFrequencyList { get; set; }
        public List<DropDownModel> TimeCardTypeList{ get; set; }
        public List<DropDownModel> PersonsList { get; set; }
        public List<DropDownModel> BusinessLevelCodeList { get; set; }
        public List<DropDownModel> WorkedStateCodeList { get; set; }
    }
}