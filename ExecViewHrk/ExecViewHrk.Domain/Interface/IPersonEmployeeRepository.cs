using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExecViewHrk.Domain.Models;
using ExecViewHrk.Models;



namespace ExecViewHrk.Domain.Interface
{
    public interface IPersonEmployeeRepository : IDisposable, IBaseRepository
    {
        List<DropDownModel> GetMaritalStatusList();
        List<DropDownModel> GetEmployeeTypeList();
        List<DropDownModel> GetEmployeeStatusList();
        List<DropDownModel> GetEmployeePayFrequencyList();

        List<DropDownModel> GetPersonsList();
        List<DropDownModel> GetBusinessLevelCodeList();
        List<DropDownModel> GetWorkedStateList();

        void PersonEmployeesDeleteAjax(int personTestId);

        PersonEmployeeVm GetPersonEmployeesRecord(int personEmployeeId);
        //PersonEmployeeVm GetPersonTestsRecord(int employeeId, int personId);
        PersonEmployeeVm GetPersonTestsRecord(int employeeId, int personId,string businessLevelCode);

        List<PersonEmployeeVm> PersonEmployeeList(int personId);
        List<DropDownModel> GetTimeCardTypesList();

        List<DropDownModel> GetDepartmentsList(int companyCode);

        List<DropDownModel> GetCompanyCodeList();

        List<DropDownModel> GetEarningsCodesList(int earingCode);

        List<DropDownModel> GetReportsToList(int personId);

        List<TreatyNonTreatyTrackingStatusVm> GetTreatyNonTreatyTrackingStatus(string Filenumber);
        TreatyNonTreatyTrackingStatusVm GetTreatyNonTreaty(string Filenumber);
    }   
}
