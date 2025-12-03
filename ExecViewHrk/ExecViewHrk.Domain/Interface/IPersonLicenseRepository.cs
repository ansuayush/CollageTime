using ExecViewHrk.Models;
using System;
using System.Collections.Generic;

namespace ExecViewHrk.Domain.Interface
{
    public interface IPersonLicenseRepository : IDisposable, IBaseRepository
    {
        List<DropDownModel> GetPersonLicenseList();
        List<DropDownModel> GetStateList();
        List<DropDownModel> GetCountryList();
        List<PersonLicensVm> GetPersonLicenseList(int _personId);

        void DeletePersonLicense(int _personLicenseId);
    }
}
