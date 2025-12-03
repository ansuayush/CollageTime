using ExecViewHrk.Domain.Models;
using ExecViewHrk.Models;
using System.Collections.Generic;

namespace ExecViewHrk.Domain.Interface
{
    public interface IReportRepository : IBaseRepository
    {
        List<PositionReportsVM> GetPositionReportList();
        List<EmployeeStatusReportVM> GetEmployeePositionReportList();
        List<OpenPositionReportVm> GetOpenPositionReportList();
        List<ContractsReportVm> GetContractsReportList();
        List<PositionsInUseReportVm> GetPositionsInUseReportList();
        List<PositionClosedReportVm> GetPositionClosedReportListList();
        List<BudgetToActualsReportVm> GetBudgetToActualsReportList(int budgetYear, int month);
        List<ContractEarnBurnReport> GetContractEarnBurnReportList();
        List<SalaryFringeReport> GetSalaryFringeReportList();
        List<StduentPositionsReportVM> GetStudentPositionReportList();
        List<TreatyReportVm> GetTreatyReportList();
        List<ContractReportVM> GetContractReportList();
        List<WorkStudylimitVm> GetWorkStudylimitReportList();
    }
}
