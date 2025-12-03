using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExecViewHrk.Models;

namespace ExecViewHrk.Domain.Interface
{
   public interface ITimeCardArchiveRerpository
    {
        List<TimeCardsArchiveVM> GetTimaCardarchiveList(int EmployeeId, int payperiodId);
      
        List<PayPeriodVM> GetPayPeriodsList(int? EmployeeIdDdl, bool IsArchived);
        List<TimeCardWeekTotalCollectionVm> GetWeeklyTotalTimaCardArchiveList(int EmployeeId, int payperiodId);
        List<TimeoffSummaryVM> GetTimeoffSummaryList(int? employeeIdDdl);
        TimeCardsNotesVM GetTimecardArchiveNotes(int? timecardId);
    }
}
