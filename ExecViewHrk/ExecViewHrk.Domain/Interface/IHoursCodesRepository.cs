using ExecViewHrk.Models;
using System;
using System.Collections.Generic;

namespace ExecViewHrk.Domain.Interface
{
    public interface IHoursCodesRepository : IDisposable, IBaseRepository
    {
        HoursCodeVm HoursEditMatrix(int HoursCodeId);
        List<HoursCodeVm> HoursCodesList_Read();
        bool HoursCodeSaveAjax(HoursCodeVm hourcodeVM);
        bool HourCodeDestroy(int HoursCodeId);
    }
}
