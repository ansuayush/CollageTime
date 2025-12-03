using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExecViewHrk.Domain.Repositories
{
    public class TimeCardUnApprovedReportRepository : RepositoryBase, ITimeCardUnApprovedReportRepository
    {
        public List<TimeCardUnApprovedReportDM> GetTimeCardUnApprovedReportList(int? companyCodeId, int? payPeriodId)
        {
            var timeCardList = new List<TimeCardUnApprovedReportDM>();
            try
            {
                timeCardList = Query<TimeCardUnApprovedReportDM>("sp_GetTimeCardUnApprovalReportForAdministrator", new { @CompCodeId = companyCodeId, @PayPeriodId = payPeriodId, @IsApproved = 0 }).ToList();
            }
            catch (Exception e)
            {
                string error = e.ToString();
            }
            return timeCardList;
        }
    }
}