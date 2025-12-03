using ExecViewHrk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Domain.Interface
{
    public interface ITimeReportRepository : IDisposable, IBaseRepository
    {
        List<TimeReportVm> GetTimeReportList(DateTime startDate, DateTime endDate); 
       List<TimeReportVm> GetEmpTimeReportList(DateTime startDate, DateTime endDate);
    }
}
