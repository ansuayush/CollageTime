using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExecViewHrk.Models;

namespace ExecViewHrk.Domain.Interface
{
    public interface IPersonPhoneNumbersRepository :IDisposable, IBaseRepository
    {
        List<DropDownModel> GetPhoneTypeList();
       PersonPhoneNumberVm GetPersonPhoneNumberRecord(int personPhoneNumberId);
      // List<PersonPhoneNumberVM> GetPersonPhoneNumberbypersonId(int personId);
        List<DropDownModel> GetDdlPhoneTypeList();

        void PersonPhoneNumbersDelete(int personPhoneNumberIdDdl);
        List<PersonPhoneNumberVm> GetPersonPhoneNumbersList(int personId);

        List<DropDownModel> GetProviderList();
    }
}
