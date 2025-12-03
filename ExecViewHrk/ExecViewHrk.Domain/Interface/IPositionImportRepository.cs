using ExecViewHrk.EfClient;
using ExecViewHrk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Domain.Interface
{
    public interface IPositionImportRepository : IDisposable, IBaseRepository
    {
        List<PositionImportVM> GetSalaryList(string strPositionCode);
        List<PersonEmployeeVm> GetPersonsList(string strPayGroup, string strFileNumber);
        List<PersonEmployeeVm> GetEmployeeList(string strPayGroup, string strFileNumber);
        List<PositionImportVM> GetEmployeePositions();
        List<PositionImportVM> GetSalaryhistory(string strPositionCode, decimal? dPayRate);
        List<PositionImportVM> GetEffective(int nEPositionID, decimal? dPayRate);
    }
}
