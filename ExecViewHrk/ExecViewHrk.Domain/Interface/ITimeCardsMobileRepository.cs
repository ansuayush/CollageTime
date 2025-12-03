using ExecViewHrk.EfClient;
using ExecViewHrk.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExecViewHrk.Domain.Interface
{
    public interface ITimeCardsMobileRepository : IDisposable, IBaseRepository
    {
        List<E_PositioVm> GetEmployeePositions(int employeeId, int personId);

        //List<TimeCardMobileVm> GetEmployeePunchesbyPosition(int employeeId, int positionId);

        //List<TimeCard> GetEmployeeTimeCardByDate(int employeeId, int? positionId, DateTime punchTime, int companyCodeId);
        List<TimeCard> GetEmployeeTimeCardByDate(int? employeeId, int? positionId, DateTime punchTime, int? companyCodeId, int? personId);
        
        //bool InsertEmployeePositionPunch(int employeeId, int positionId, int punchType, DateTime punchTime, int companyCode, string userName, string fileName);

        //bool InsertEmployeePositionPunch(int employeeId, int positionId, int punchType, DateTime punchTime, int companyCode, string userName, string fileName, string nightShiftTCId);

        bool InsertEmployeePositionPunch(int employeeId, int positionId, int punchType, DateTime punchTime, int companyCode, string userName, string fileName, string nightShiftTCId, int personId, int epositionid);

        List<TimeCardMobileVm> GetEmployeeTimeCards(int employeeId, int positionId, DateTime startDate, DateTime endDate);

        List<TimeCardSummaryVm> GetEmployeeTimeCardsByPayPeriod(int? employeeId, int payPeriodId, bool IsArchived);

        List<CompanyCode> GetAllCompanyCodes(string email);

        List<TimeCardSummaryVm> GetEmployeeTimeCardsByPayPeriodbyCompanycodeId(int? employeeId, int payPeriodId, bool IsArchived, int CompanyCodeId);
    }
}