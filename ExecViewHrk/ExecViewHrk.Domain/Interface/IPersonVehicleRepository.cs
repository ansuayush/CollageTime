using System;
using System.Collections.Generic;
using ExecViewHrk.Models;

namespace ExecViewHrk.Domain.Interface
{
    public interface IPersonVehicleRepository : IDisposable, IBaseRepository
    {
        List<DropDownModel> GetStateList();
        List<PersonsVehicleVm> GetPersonVehicleList(int _personId);
        void DeletePersonVehicle(int _personVehicleId);
    }
}
