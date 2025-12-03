using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExecViewHrk.EfClient;
using ExecViewHrk.Models;

namespace ExecViewHrk.Domain.Interface
{
   public interface IEPositionRepository : IDisposable, IBaseRepository
    {
        List<DropDownModel> GetPositionList();
        List<DropDownModel> GetPayFrequencyList();
        List<DropDownModel> GetRateTypeList();
        List<DropDownModel> GetPositionCategoryList();
        List<DropDownModel> GetPositionGradeList();
        List<DropDownModel> GetPositionTypeList();
        List<DropDownModel> GetEmployeeTypeList();
        List<DropDownModel> GetPayGroupList();
        List<DropDownModel> GetReportsToIdList();
        List<DropDownModel> GetEmployeeClassList();
        List<E_PositioVm> GetEPositionList(int _personId,int EmpId);
        List<E_PositioVm> GetEPositionList_v2(int _personId, int EmpId, int companyCodeId);
        void DeleteEPosition(int _e_PositionId);
        void DeleteContract(int Id);
        List<E_PositioVm> GetEPositionListbyRetrohourDate(int EmpId, int companyCodeId);
        List<E_PositioVm> GetEPositionListbyManagerId(int EmpId, int companyCodeId, string useridentityname);
       List<E_PositionSalaryHistorVm> GetSalaryHistroybyEpositionId(int epositionid);
    }
}
