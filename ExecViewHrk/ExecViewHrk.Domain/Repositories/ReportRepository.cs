using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Domain.Models;
using ExecViewHrk.Models;
using System.Collections.Generic;

namespace ExecViewHrk.Domain.Repositories
{
    public class ReportRepository : RepositoryBase, IReportRepository
    {

        public List<PositionReportsVM> GetPositionReportList()
        {
            return Query<PositionReportsVM>("uspPositionReportNoBudgetYear");
        }
        public List<EmployeeStatusReportVM> GetEmployeePositionReportList()
        {
            return Query<EmployeeStatusReportVM>("uspEmployeePositionReportNoBudgetYear");
        }
        public List<OpenPositionReportVm> GetOpenPositionReportList()
        {
            return Query<OpenPositionReportVm>("uspOpenPositionReport");
        }
        public List<ContractsReportVm> GetContractsReportList()
        {
            return Query<ContractsReportVm>("uspContractsReport");
        }
        public List<PositionsInUseReportVm> GetPositionsInUseReportList()
        {
            return Query<PositionsInUseReportVm>("uspPositionInUseReport");
        }
        public List<PositionClosedReportVm> GetPositionClosedReportListList()
        {
            return Query<PositionClosedReportVm>("uspPositionClosedReport");
        }
        public List<BudgetToActualsReportVm> GetBudgetToActualsReportList(int budgetYear, int month)
        {
            return Query<BudgetToActualsReportVm>("uspBudgetToActualsV01", new { BudgetYear = budgetYear, Month = month });
        }
        public List<ContractEarnBurnReport> GetContractEarnBurnReportList()
        {
            return Query<ContractEarnBurnReport>("USP_Contract_Earn_Burn_Report");
        }
        public List<SalaryFringeReport> GetSalaryFringeReportList()
        {
            return Query<SalaryFringeReport>("USP_Salary_Fringe_Report");
        }


        public List<StduentPositionsReportVM> GetStudentPositionReportList()
        {
            return Query<StduentPositionsReportVM>("uspStudentPositionReport");
        }
        public List<TreatyReportVm> GetTreatyReportList()
        {
            return Query<TreatyReportVm>("uspTreatyReport");
        }
        public List<ContractReportVM> GetContractReportList()
        {
            return Query<ContractReportVM>("SPGetContractReport");
        }
        public List<WorkStudylimitVm> GetWorkStudylimitReportList()
        {
            return Query<WorkStudylimitVm>("SPGetWorkStudylimitReport");
        }

    }
}