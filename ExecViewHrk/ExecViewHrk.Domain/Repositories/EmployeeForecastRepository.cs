using ExecViewHrk.Domain.Interface;
using ExecViewHrk.EfClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Domain.Repositories
{
    public class EmployeeForecastRepository : RepositoryBase , IEmployeeForecast
    {


        public List<EmployeeForecastVm> GetEmployeeForecast()
    {
        List<EmployeeForecastVm> objEmployeeForecastvm = new List<EmployeeForecastVm>();
        try
        {
            objEmployeeForecastvm = Query<EmployeeForecastVm>("uspEmployeeSalaryForecastWc").ToList();
            return objEmployeeForecastvm;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public EmployeeForecastVm GetEmployeeByFileNumber(int? filenumber)
    {

        EmployeeForecastVm empForecastobj = new EmployeeForecastVm();
        var objEmployeeForecastvm = Query<EmployeeForecastVm>("uspEmployeeSalaryForecastWc").ToList();
        empForecastobj = objEmployeeForecastvm.Where(m => m.FileNumber == filenumber).FirstOrDefault();
        return empForecastobj;
    }



    public void UpdateIncreasePercent(int? nID, decimal? dIncreasePercent)
    {
        var poshistory = _context.E_PositionSalaryHistories.Where(u => u.E_PositionSalaryHistoryId == nID).SingleOrDefault();
        poshistory.IncreasePercent = dIncreasePercent;
        _context.SaveChanges();

    }

    public void UpdateAllEmpForecast(decimal? dIncreasePercent)
    {
        //_context.E_PositionSalaryHistories.
        var AllEmpsalHistData = _context.E_PositionSalaryHistories.ToList();
        AllEmpsalHistData.ForEach(a => a.IncreasePercent = dIncreasePercent);
        _context.SaveChanges();

    }
    public void ClearAllEmpForecast()
    {
        var AllEmpsalHistData = _context.E_PositionSalaryHistories.ToList();
        AllEmpsalHistData.ForEach(a => a.IncreasePercent = null);
        _context.SaveChanges();
    }
}
}