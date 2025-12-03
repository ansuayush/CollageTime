using ExecViewHrk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Domain.Interface
{
   public interface IFinancialRepository : IDisposable, IBaseRepository
    {
        List<FinancialCustomVm> GetEmpFinancialReportList();
        List<FinancialCustomVm> GetEmpFinancialReportList1(int payperiodId);
        List<PayPeriodVM> GetPayPeriodsList();
        int GetPayperiodidlist();
    }
}
