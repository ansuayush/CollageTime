using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Domain.Repositories
{
   public class FinancialRepository : RepositoryBase, IFinancialRepository
    {
        public List<FinancialCustomVm> GetEmpFinancialReportList()
        {
            return Query<FinancialCustomVm>("GetFinancialAidCustomReport");
        }
        public List<FinancialCustomVm> GetEmpFinancialReportList1(int payperiodId)
        {
            return Query<FinancialCustomVm>("GetFinancialAidCustomReportbypayperiod", new { @PayPeriodId = payperiodId });
        }
        public List<PayPeriodVM> GetPayPeriodsList()
        {
            var payPeriodsList = _context.PayPeriods.Where(x=>x.PayGroupCode==1)
               .Select(m => new PayPeriodVM
               {
                   PayPeriodId = m.PayPeriodId,
                   EndDate = m.EndDate,
                   PayPeriod = SqlFunctions.StringConvert((double)SqlFunctions.DatePart("mm", m.StartDate)).Trim() + "/" + SqlFunctions.DateName("DAY", m.StartDate) + "/" + SqlFunctions.DateName("YYYY", m.StartDate)
                              + " - " + SqlFunctions.StringConvert((double)SqlFunctions.DatePart("mm", m.EndDate)).Trim() + "/" + SqlFunctions.DateName("DAY", m.EndDate) + "/" + SqlFunctions.DateName("YYYY", m.EndDate)
               }).OrderByDescending(m => m.EndDate).ToList();//.Take(6).ToList();

            return payPeriodsList;
        }
        public int GetPayperiodidlist()
        {
            var date = Convert.ToDateTime(DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day);
            var payperiodid = _context.PayPeriods.Where(x => x.PayGroupCode == 1 && ((x.StartDate <=date) && (x.EndDate >= date))).Select(x => x.PayPeriodId).FirstOrDefault();
            return payperiodid;

        }
    }
}
