using ExecViewHrk.Models;
using System.Collections.Generic;

namespace ExecViewHrk.Domain.Interface
{
    public interface IPersonRepository : IBaseRepository
    {
        List<PersonVm> GetPersonsList(string _type, string _search);
        List<PersonVm> GetEmployeesList();
        //List<PersonVm> GetActiveEmployeesList();
        PersonProfileVm GetRolodexData(int personId);
        List<DropDownModel> GetPersonList();
        List<DropDownModel> GetPositionsList();
        List<DropDownModel> GetPositionListbyPositionId(int managerId);
        List<DropDownModel> GetPositionFilterbyPositionId(int mnagaerId);
        List<PersonProfileVm> GetAllPersons_PositionReportsTo(int PositionID);
        PersonProfileVm GetRecordForIDFromPersonIDWithPersonInfo(int personId);
    }
}
