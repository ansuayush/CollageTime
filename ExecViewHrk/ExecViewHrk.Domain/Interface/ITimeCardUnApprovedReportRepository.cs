using ExecViewHrk.Models;
using System.Collections.Generic;

namespace ExecViewHrk.Domain.Interface
{
    public interface ITimeCardUnApprovedReportRepository
    {
        List<TimeCardUnApprovedReportDM> GetTimeCardUnApprovedReportList(int? companyCodeId, int? payPeriodId);
    }
}