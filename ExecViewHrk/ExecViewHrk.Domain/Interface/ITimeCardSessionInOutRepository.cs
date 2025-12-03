using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExecViewHrk.Models;
namespace ExecViewHrk.Domain.Interface
{
   public interface ITimeCardSessionInOutRepository
    {
        List<TimeCardSessionInOutConfigsVm> GetTimeCardSessionList();
        TimeCardSessionInOutConfigsVm GetTimecradssessiondeatils(int timecardssessionId);
        bool updatetimecardsseiion(TimeCardSessionInOutConfigsVm tcsvm);
    }
}
