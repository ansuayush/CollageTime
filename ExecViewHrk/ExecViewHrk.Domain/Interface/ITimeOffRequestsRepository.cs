using ExecViewHrk.EfClient;
using ExecViewHrk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Domain.Interface
{
    public interface ITimeOffRequestsRepository : IDisposable, IBaseRepository
    {
        IList<TimeOffRequestStatusVM> GetEmpTimeOffRequest(Employee empDetails, int year, int month);

        bool GetTimeOffRequestDateInDb(Employee empDetails, DateTime timeOffDate);
        TimeOffRequestVM GetTimeOffRequestData(Employee empDetails, DateTime timeOffDate);

        bool AddTimeOffRequest(TimeOffRequestVM timeOffRequestVM, Employee empDetails, DateTime timeOffDate, int days);

        List<TimeOffRequest> DeleteTimeOffRequest(Employee empDetails, DateTime timeOffDate, int days);

        List<TimeOffTotalRequestsVM> GetAllEmpTimeOffRequestsList(int sYear, int sMonth);
        List<TimeOffEmpDetailsVM> GetTimeOffEmpDetailsList(DateTime selectedDate);
        bool AddResposeTimeOffRequest(TimeOffVM timeOffVM);
    }
}
