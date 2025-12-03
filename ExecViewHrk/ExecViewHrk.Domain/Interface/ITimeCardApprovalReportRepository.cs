using ExecViewHrk.EfClient;
using ExecViewHrk.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Domain.Interface
{
    public interface ITimeCardApprovalReportRepository : IDisposable, IBaseRepository
    {
        List<int> LoadEmpList(int compCodeId, int deptId, int? payPeriodId);

        List<int> GetTimecardApprovalEmpList(int compCodeId, int deptId, int? payPeriodId);

        IEnumerable<DbEntityValidationResult> GetValidationErrors();

        List<TimeCardApprovalReportCollectionVm> GetTimeCardApprovalReportList(int? companyCodeId,int? payPeriodId, string userId);

        TimeCardApproval GetTimeCardApprovals(int empId, int? payPeriodId);
    }
}
