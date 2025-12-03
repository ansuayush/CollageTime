using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExecViewHrk.EfClient;
using ExecViewHrk.Models;

namespace ExecViewHrk.Domain.Interface
{
    public interface IEmployeeForecast : IBaseRepository, IDisposable
    {
       List<EmployeeForecastVm> GetEmployeeForecast();
        EmployeeForecastVm GetEmployeeByFileNumber(int? filenumber);
        void UpdateIncreasePercent(int? nID, decimal? dIncreasePercent);
        void UpdateAllEmpForecast(decimal? dIncreasePercent);
        void ClearAllEmpForecast();
    }
}
