using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.EfClient
{
    public class PositionImportVM
    {
        //position
        public virtual BusinessUnit BusinessUnit { get; set; }
        public virtual Department Department { get; set; }       
        public virtual ICollection<E_Positions> E_Positions { get; set; }
        public virtual Job Job { get; set; }
        public virtual PositionBusinessLevels PositionBusinessLevels { get; set; }
        public virtual DdlPositionCategory DdlPositionCategories { get; set; }
        public virtual DdlPositionTypes DdlPositionType { get; set; }
        public virtual DdlPositionGrade DdlPositionGrades { get; set; }
        public virtual UserDefinedSegment1s UserDefinedSegment1s { get; set; }
        public virtual UserDefinedSegment2s UserDefinedSegment2s { get; set; }
        //extra fields
        public string code { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string jobcode { get; set; }
        public string JobDescription { get; set; }
        public string BuCode { get; set; }
        public string BuTitle { get; set; }
        public string JobTitle { get; set; }

        //e_positions

        public int E_PositionId { get; set; }
        public int EmployeeId { get; set; }
        public int PositionId { get; set; }
        public int? PayFrequencyId { get; set; }
        public int? RateTypeId { get; set; }
       public bool? PrimaryPosition { get; set; }       
        public DateTime? StartDate { get; set; }        
        public DateTime? EndDate { get; set; }        
        public string Notes { get; set; }        
        public decimal? salary { get; set; }        
        public string EnteredBy { get; set; }        
        public DateTime? EnteredDate { get; set; }        
        public string ModifiedBy { get; set; }        
        public DateTime? ModifiedDate { get; set; }
        public virtual DdlPayFrequency DdlPayFrequency { get; set; }
        public virtual DdlRateType DdlRateType { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Position Position { get; set; }
        public int? PositionTypeID { get; set; }
        public int? PositionCategoryID { get; set; }
        public int? PositionGradeID { get; set; }
        public decimal? FTE { get; set; }
        public DateTime? actualEndDate { get; set; }
        public DateTime? ProjectedEndDate { get; set; }
        public virtual DdlPositionCategory DdlPositionCategory { get; set; }
        public virtual DdlPositionGrade DdlPositionGrade { get; set; }
        public virtual DdlPositionTypes DdlPositionTypes { get; set; }       
        public virtual ICollection<E_PositionSalaryHistories> E_PositionSalaryHistories { get; set; }

        //E_PositionSalaryHistories       
        public int E_PositionSalaryHistoryId { get; set; }               
        public DateTime? EffectiveDate { get; set; }
        public decimal? PayRate { get; set; }
        public decimal? HoursPerPayPeriod { get; set; }     
    }
}
