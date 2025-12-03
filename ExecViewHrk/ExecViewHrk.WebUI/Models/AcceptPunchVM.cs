using ExecViewHrk.EfClient;
using ExecViewHrk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class AcceptPunchVM
    {

        public DateTime CurrentDate { get; set; }

        public int EmployeeId { get; set; }

        public string FullName { get; set; }

        public int? CompanyCodeId { get; set; }

        public double? DailyTotal { get; set; }

        public double? PayPeriodTotal { get; set; }

        public DateTime? PayPeriodStartDate { get; set; }

        public DateTime? PayPeriodEndDate { get; set; }

        #region Multiple Company Codes

        public List<CompanyCode> CompanyCodeList { get; set; }

        public int SelectedCompanyCode { get; set; }

        #endregion

        public List<E_PositioVm> PositionList { get; set; }

        public int SelectedPosition { get; set; }

        public int PunchType { get; set; }

        public bool IsInActive { get; set; }

        public string NotificationMessage { get; set; }

        public bool IsPayPeriodLocked { get; set; }

        public string FileName { get; set; }

        public string NightShiftOn { get; set; }

        public string NightShiftTimeCardId { get; set; }

        public int? MaxHours { get; set; }
        public double? Week1Hours { get; set; }
        public double? Week2Hours { get; set; }
        public double? TreatyTime { get; set; }
        public string EpositionId { get; set; }
       

    }
}