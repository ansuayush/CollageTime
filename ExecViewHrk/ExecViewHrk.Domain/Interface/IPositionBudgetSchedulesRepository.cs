using System;
using ExecViewHrk.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Domain.Interface
{
    public interface IPositionBudgetSchedulesRepository : IDisposable,IBaseRepository
    {
        List<PositionBudgetSchedulesVM> getpositionBudgetSchedulesList();

        PositionBudgetSchedulesVM getpositionBudgetSchedulesDetails(int ID);

        void deletepositionBudgetSchedule(int ID);

        PositionBudgetSchedulesVM positionBudgetSchedulesSave(PositionBudgetSchedulesVM positionBudgetSchedulesVM);
        PositionBudgetSchedulesVM GetRecordForEffectiveDateAndScheduleType(DateTime? dateEffective,byte? scheduleType);
       
    }
}
