using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ExecViewHrk.EfClient;

namespace ExecViewHrk.Models
{
    public class PayPeriodVM
    {
        public int PayPeriodId { get; set; }
        [Required]
        [Display(Name = "Company Code")]
        public int? CompanyCodeId { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyCodeDescription { get; set; }
        [DataType(DataType.Date)]
        [Required]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        
       
        public string ModifiedStartDate
        {
            get
            {
                return StartDate == null ? string.Empty : this.StartDate.ToString("MM/dd/yyyy");
            }
            set
            {
                this.StartDate = Convert.ToDateTime(value);
            }
        }

        [DataType(DataType.Date)]
        [Required]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
        public string ModifiedEndDate
        {
            get
            {
                return EndDate == null ? string.Empty : this.EndDate.ToString("MM/dd/yyyy");
            }
            set
            {
                this.EndDate = Convert.ToDateTime(value);
            }
        }

        [Required]
        [Display(Name = "Pay Frequency")]
        public int PayFrequencyId { get; set; }
        public string PayFrequencyName { get; set; }
        public bool IsPayPeriodActive { get; set; }
        public string Description { get; set; }
        [Display(Name = "Lockout All Employees")]
        public bool LockoutEmployees { get; set; }
        [Display(Name = "Lockout All Managers Override")]
        public bool LockoutManagers { get; set; }

        public bool IsArchived { get; set; }

        public int? PayGroupCode { get; set; }

        public int? PayPeriodNumber { get; set; }

        public List<DropDownModel> CompanyCodeDropdown { get; set; }
        public List<DropDownModel> PayFrequencyDropdown { get; set; }
        [Display(Name = "Managers Locked In/Out List")]
        public List<ManagerLockoutsVM> ManagersList { get; set; }
        public string managerslockedlist { get; set; }

        public List<ManagerLockoutsVM> ManagersdonthaveLockeList { get; set; }

        public List<DropDownModel> PayPeriodDropdown { get; set; }

        public string EarningCodes { get; set; }
        public string MemoCode1 { get; set; }
        public decimal MemoCode1Amount { get; set; }
        public string MemoCode2 { get; set; }
        public decimal MemoCode2Amount { get; set; }
        public string PayPeriod { get; set; }
        public string ManagersCount { get; set; }
        public override string ToString()
        {
            return string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\""
                             , EarningCodes, MemoCode1, MemoCode1Amount, MemoCode2, MemoCode2Amount);

        }
        public string Export { get; set; }
        public string PayPeriodStartDate { get; set; }
        public string PayPeriodEndDate { get; set; }
        public int TimecardUnApprovedCount { get; set; }
        public int TimecardCount { get; set; }
        public bool IsEndDate { get; set; }
        public DateTime? PayDate { get; set; }
        //SOFT DELETE
        public bool IsDeleted { get; set; }
        public string DeletedBy { get; set; }
        public string UserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        //conrtact 

        public decimal? Amounts { get; set; }
        public Nullable<decimal> ADJlimits { get; set; }
        public int? numberofpayperiods { get; set; }
        public bool Isexported { get; set; }
    }
}
