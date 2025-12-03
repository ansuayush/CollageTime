using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExecViewHrk.Models;

namespace ExecViewHrk.Domain.Interface
{
   public interface IPayPeriodRepository:IDisposable, IBaseRepository
    {
        List<PayPeriodVM> GetPayPeriodList();
        PayPeriodVM GetPayPeriodDetail(int _payPeriodId);
        //PayPeriodVM savePayPeriod(PayPeriodVM payperiodvm);
        PayPeriodVM savePayPeriod(PayPeriodVM payperiodvm, string userId);
        //void DeletePayPeriod(int payPeriodId);
        void DeletePayPeriod(int payPeriodId, string userId);
        bool ArchivePayperiod(short companyCodeId, int payPeriodId, DateTime startdate, DateTime enddate, int PayPeriodId);
        void LockoutemployeeUpdate(int payPeriodId, bool lockoutemployee);
        void LockoutManagersUpdate(int payPeriodId, bool lockoutManagers);
        List<TimeCardExportCollectionVM> Export_Employee_TimecardDetails(int companyCodeId, int payPeriodId);
        List<T> Query<T>(string storedProcName);
        int GetUnApprovedTimecards(int? companyCodeId, DateTime startdate, DateTime enddate);
        int GetTimecardCount(int? companyCodeId, DateTime startdate, DateTime enddate);

        //Global Archive for all Companies
        bool ArchiveGlobalPayperiod(int PayFrequencyid, DateTime startdate, DateTime enddate, int PayPeriodId);
        List<TimeCardVm> GetGlobalTimeCardsBetweendates(DateTime strtdate, DateTime enddate);
        TimeCardsNotesVM GetGlobalTimeCardsNotesBetweendates(int timeCardId);
        List<PayPeriodVM> GetPayPeriodListByCompanyCode(int CompanyCodeId);
        int GetGlobalUnApprovedTimecards(DateTime startdate, DateTime enddate);
        int GetGlobalTimecardCount(DateTime startdate, DateTime enddate);
        string AutoSendEmailTreatyLimit(int companyCode, int? payGroupId);
    }
}
