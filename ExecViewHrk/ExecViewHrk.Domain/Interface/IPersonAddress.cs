using ExecViewHrk.EfClient;
using ExecViewHrk.Models;
using ExecViewHrk.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Domain.Interface
{
   public interface IPersonAddress:IDisposable,IBaseRepository
    {
        List< PersonAddressVm> GetPersonAddressesList(int personId);
        List<PersonAddressVm> GetPersonIsPrimaryAddressList(int personId);
        PersonAddressVm GetPersonAddressesDetails(int personAddressId);
        void PersonAddressDeleteAjax(int addressTypeId);
        List<DdlAddressType> GetAddressTypesList();
        List<DdlState> GetStatesList();
        List<DdlCountry> GetCountriesList();
    }
}
