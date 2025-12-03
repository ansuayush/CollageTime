using ExecViewHrk.EfClient;
using ExecViewHrk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Domain.Interface
{
    public interface ITimeCardMatrixReposotory : IBaseRepository
    {
        List<TimeCardCollectionVm> GetTimeCardsList(int? employeeIdDdl, int? payPeriodId, bool? IsArchived);

        List<TimeCardCollectionVm> GetTimeCardsListReportToPositions(int? employeeIdDdl, int? payPeriodId, bool? IsArchived, string loggedInUserId);

      //  List<TimeCardCollectionVm> GetTimeCardsListReportToPositions(int? employeeIdDdl, int? payPeriodId, bool IsArchived, string loggedInUserId, int? DepartmentId);
        List<TimeCard> SaveTimeCardsList(IEnumerable<TimeCardVm> timeCardVmGridCollection, Dictionary<DateTime, int> Date_MaxProjectNum, int maxProjectNum, int employeeIdDdl, int? departmentId, int companyCodeIdDdl, string userId,int? payPeriodId);
        List<TimeCardVm> UpdateTimeCardsList(IEnumerable<TimeCardVm> timeCardVmGridCollection, Dictionary<DateTime, int> Date_MaxProjectNum, int maxProjectNum, int employeeIdDdl, int? departmentId, int companyCodeIdDdl, string userId, int? payPeriodId);
        bool DeleteTimeCards(int timecardId, string userId);
        Dictionary<DateTime, int> GetDateMaxProjectNum(int? employeeIdDdl, int? companyCodeIdDdl);
        PayPeriodVM GetPayPeriodDates(int? payPeriodId);
        List<TimeCardWeekTotalCollectionVm> GetTimeCardWeekTotalList(int empid, int? departmentId, int payperiodId, bool isArchive);
        bool checkEmployeeStatus(int departmentId, int employeeIdDdl);
        double gettotalhours(int empid, DateTime PPstartdate, DateTime PPenddate);
        EmployeesVM GetFlsaCode(int empId);
        decimal? GetNonExemptEmployeeFTE(int employeeId);
        List<TimeCardVm> Grid_ReadChild(int employeeIdDdl, DateTime ActualDate);

        List<TimeCardVm> Grid_UpdateChild(IEnumerable<TimeCardVm> timeCardVmGridCollection, Dictionary<DateTime, int> Date_MaxProjectNum, int maxProjectNum, int? employeeIdDdl, int? companyCodeIdDdl, int? departmentId, int? payPeriodId, bool isArchived, string UserName, DateTime ActualDate);
        List<TimeCardVm> Grid_CreateChild(IEnumerable<TimeCardVm> timeCardVmGridCollection, Dictionary<DateTime, int> Date_MaxProjectNum, int maxProjectNum, int? employeeIdDdl, int? companyCodeIdDdl, int? departmentId, int? payPeriodId, bool isArchived, string UserName, DateTime ActualDate);

        List<TimeoffSummaryVM> GetTimeoffSummaryList(int? employeeIdDdl);
        // List<TimeCardVm> Grid_ReadChildPoistionByManager(int employeeIdDdl, DateTime ActualDate, string loggedInUserId);
        //List<TimeCardVm> Grid_ReadChildPoistionByManager(int employeeIdDdl, int? payPeriodId, bool IsArchived, string loggedInUserId, DateTime actualDate);
        List<TimeCardVm> Grid_ReadChildPoistionByManager(int employeeIdDdl, int? payPeriodId, bool IsArchived, string loggedInUserId, DateTime actualDate);

        List<TimeCardCollectionVm> GetEpositionID(int? employeeIdDdl, int? PositionId, int? companycodeId, int? payPeriodId);

    }
}
