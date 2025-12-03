using ExecViewHrk.Domain.Models;
using ExecViewHrk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Domain.Interface
{
    public interface ITimeCardConfigurationsRepository: IBaseRepository
    {
        List<TimeCardDisplayColumnVM> GetTimeCardColumnsList();
        TimeCardDisplayColumnVM GetTimeCardColumnsById(int timeCardTypeId);
        List<TimeCardTypeVM> PopulateTimeCardTypes();
        bool TimeCardColumnsList_Update(TimeCardDisplayColumnVM timecardColumn);
    }
}
