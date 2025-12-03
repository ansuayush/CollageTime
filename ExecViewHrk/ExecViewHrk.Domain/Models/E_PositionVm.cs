using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace ExecViewHrk.Models
{
    public class E_PositioVm
    {
        public int E_PositionId { get; set; }

        public int EmployeeId { get; set; }

        [Required]
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        //  [Required(ErrorMessage = "The Position field is required.")]
        public int PositionId { get; set; }
        public string PositionTitle { get; set; }
        public string BusinessLevel { get; set; }
        public string Job { get; set; }
        public decimal? FTE { get; set; }
        public DateTime? ProjectEndDate { get; set; }
        public string PositionDescription { get; set; }
        public string PositionCode { get; set; }
        // [Required(ErrorMessage = "The Pay Frequency field is required.")]
        public int? PayFrequencyId { get; set; }
        public int? PositionTypeID { get; set; }
        public string PositionTypeDesc { get; set; }
        public int? PositionCategoryID { get; set; }
        public string PositionCategoryDesc { get; set; }
        public int? PositionGradeID { get; set; }
        public string PositionGradeDesc { get; set; }
        public string PayFrequencyDescription { get; set; }

        public int? RateTypeId { get; set; }

        public int? EmployeeTypeId { get; set; }

        public int? PayGroupId { get; set; }

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

        [DataType(DataType.Date)]
        public DateTime? EffectiveDate { get; set; }
        public string FileNumber { get; set; }
        public string NewPositionId { get; set; }

        public List<DropDownModel> PositionDropDownList { get; set; }

        public IEnumerable<SelectListItem> EmployeeIdDropDownList { get; set; }
        public List<DropDownModel> PayFrequencyDropDownList { get; set; }
        public List<DropDownModel> RateTypeDropDownList { get; set; }
        public List<DropDownModel> PositionTypeDropDownList { get; set; }
        public List<DropDownModel> PositionCategoryDropDownList { get; set; }
        public List<DropDownModel> PositionGradeDropDownList { get; set; }
        public List<DropDownModel> EmployeeTypeDropDownList { get; set; }

        public List<DropDownModel> PayGroupDropDownList { get; set; }

        public List<DropDownModel> ReportsToIdDropDownList { get; set; }

        public List<ExecViewHrk.EfClient.DdlPositionTypes> PositionTypeList { get; set; }
        public List<ExecViewHrk.EfClient.DdlPositionCategory> PositionCategoryList { get; set; }
        public List<ExecViewHrk.EfClient.DdlPositionGrade> PositionGradeList { get; set; }

        public short SalaryGradeID { get; set; }

        public int? ReportsToID { get; set; }
        public int? AdpYear { get; set; }
        public bool? IsDeleted { get; set; }
        public string DeletedBy { get; set; }
        public string UserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public Nullable<decimal> AdpWSLimit { get; set; }
        public string CostNumber { get; set; }
        public DateTime? CostNumberEffectiveDate { get; set; }
        public decimal? CostNumber1Percent { get; set; }
        public string CostNumber2Account { get; set; }
        public decimal? CostNumber2Percent { get; set; }
        public string CostNumber3Account { get; set; }
        public decimal? CostNumber3Percent { get; set; }
        public string CostNumber4Account { get; set; }
        public decimal? CostNumber4Percent { get; set; }
        public string CostNumber5Account { get; set; }
        public decimal? CostNumber5Percent { get; set; }
        public string CostNumber6Account { get; set; }
        public decimal? CostNumber6Percent { get; set; }
        public int? EmployeeClassId { get; set; }
        public List<DropDownModel> EmployeeClassList { get; set; }
        public string Suffix { get; set; }
        public string EpositionId { get; set; }
        public DateTime? RetroHoursDate { get; set; }
    }
}

