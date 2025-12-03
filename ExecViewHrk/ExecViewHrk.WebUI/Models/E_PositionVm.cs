using ExecViewHrk.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//E_Position View Model
namespace ExecViewHrk.WebUI.Models
{
    public class E_PositionVm
    {
        public int E_PositionId { get; set; }

        public int EmployeeId { get; set; }

        [Required]
        public int PersonId { get; set; }
        public string PersonName { get; set; }
      //  [Required(ErrorMessage = "The Position field is required.")]
        public short PositionId { get; set; }
        public string PositionTitle { get; set; }
        public string BusinessLevel { get; set; }
        public string Job { get; set; }
        public decimal? FTE { get; set; }
        public DateTime? ProjectEndDate { get; set; }
        public string PositionDescription { get; set; }
       // [Required(ErrorMessage = "The Pay Frequency field is required.")]
        public byte? PayFrequencyId { get; set; }
        public short? PositionTypeID { get; set; }
        public string PositionTypeDesc { get; set; }
        public int? PositionCategoryID { get; set; }
        public string PositionCategoryDesc { get; set; }
        public short? PositionGradeID { get; set; }
        public string PositionGradeDesc { get; set; }
        public string PayFrequencyDescription { get; set; }
        
        public int? RateTypeId { get; set; }

        public string RateTypeDescription { get; set; }

        public bool? PrimaryPosition { get; set; }
      //  [Required(ErrorMessage = "The Start Date field is required.")]
        public DateTime? StartDate { get; set; }
    //    [Required(ErrorMessage = "The End Date field is required.")]
        public DateTime? EndDate { get; set; }
        public DateTime? projectedEndDate { get; set; }
        public DateTime? actualEndDate { get; set; }
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        public decimal? salary { get; set; }
        public string EnteredBy { get; set; }
        public DateTime? EnteredDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int E_PositionSalaryHistoryId { get; set; }
        public decimal? PayRate { get; set; }
        public decimal? HoursPerPayPeriod { get; set; }
        public decimal? AnnualSalary { get; set; }
        public string FileNumber { get; set; }
        [DataType(DataType.Date)]
        public DateTime? EffectiveDate { get; set; }

        public List<DropDownModel> PositionDropDownList { get; set; }

        public IEnumerable<SelectListItem> EmployeeIdDropDownList { get; set; }
        public List<DropDownModel> PayFrequencyDropDownList { get; set; }
        public List<DropDownModel> RateTypeDropDownList { get; set; }
        public List<DropDownModel> PositionTypeDropDownList { get; set; }
        public List<DropDownModel> PositionCategoryDropDownList { get; set; }
        public List<DropDownModel> PositionGradeDropDownList { get; set; }
        public List<ExecViewHrk.EfClient.DdlPositionTypes> PositionTypeList { get; set; }
        public List<ExecViewHrk.EfClient.DdlPositionCategory> PositionCategoryList { get; set; }
        public List<ExecViewHrk.EfClient.DdlPositionGrade> PositionGradeList { get; set; }

        public short SalaryGradeID { get; set; }
    }
}