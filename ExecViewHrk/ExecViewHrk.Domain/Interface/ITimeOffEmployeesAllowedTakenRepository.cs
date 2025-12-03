using ExecViewHrk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Domain.Interface
{
    public interface ITimeOffEmployeesAllowedTakenRepository : IDisposable, IBaseRepository
    {
        bool InsertEmployeesAllowedTaken(List<TimeOffEmployeesAllowedTakenVM> timeOffEmployeesAllowedTakenVM);
        List<TimeoffSummaryVM> UpdateEmployeesAllowedTaken(int? employeeIdDdl);
        List<TimeoffSummaryVM> GetTimeoffSummaryList();
    }
}
