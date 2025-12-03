using System;
using System.Collections.Generic;
using ExecViewHrk.Models;

namespace ExecViewHrk.Domain.Interface
{
    public interface IPersonPassportRepository : IDisposable, IBaseRepository
    {
        List<DropDownModel> GetCountryList();
        List<PersonsPassportVm> GetPersonPassportList(int _personId);
        void DeletePersonPassport(int _personPassportId);        
    }
}
