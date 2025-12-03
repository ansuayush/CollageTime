using ExecViewHrk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Models;
namespace ExecViewHrk.Domain.Interface
{
    public interface ILookupTablesRepository : IDisposable, IBaseRepository
    {
        List<DdlRelationshipTypeVm> getDdlRelationshipTypesList();
        DdlRelationshipTypeVm getDdlRelationshipTypesDetails(int relationshipTypeId);
        DdlRelationshipTypeVm updateDdlRelationshipTypes(DdlRelationshipTypeVm dlRelationshipTypeVm);
        void deleteDdlRelationshipTypes(int relationshipTypeId);

        List<DdlEducationEstablishmentViewModel> getDdlEducationEstablishmentList();
        List<DropDownModel> GetStateList();
        List<DropDownModel> GetCountryList();
        List<CompanyCodeVM> GetCompanyCodes();
        //DdlEducationEstablishmentViewModel updateDdlEducationEstablishmentsList(DdlEducationEstablishmentViewModel ddlEducationEstablishmentViewModel);

        #region  TimeCards Binding

        List<ADPFieldMappingVM> GetADPFieldMappings();
        List<ADPAccNumberVM> GetADPAccNumbers();
        List<HoursCodeVm> GetHourCodes();
        List<EarningCodeVm> GetEarningCodes();
        List<DepartmentVm> GetTempDepartmentCodes();
        List<JobsVM> GetTempJobCodes();
        TimeCardDisplayColumn TimeCardInOutDisplayColumns(short typeId);
        List<DepartmentVm> GetAdminDepartmentsList(short? CompanyCodeIdDdl);
        List<DepartmentVm> GetManagersDepartmentsList(short? CompanyCodeIdDdl, string useridentityname);
        List<EmployeesVM> GetEmployeesList(short? DepartmentIdDdl, bool isActive);
        List<EmployeesVM> GetEmployeesList();
        List<PayPeriodVM> GetPayPeriodsList(int? EmployeeIdDdl, bool IsArchived);
        List<PayPeriodVM> GetPayPeriodsList(int? CompanyCodeIdDdl);
        List<HoursCodeVm> ValidateHoursCodes(int? CompanyCodeIdDdl);
        List<EarningCodeVm> ValidEarningCodes(int? CompanyCodeIdDdl);
        List<DepartmentVm> ValidTempDeptCodes(int? CompanyCodeIdDdl);
        List<JobsVM> ValidateTempJobCodes(int? CompanyCodeIdDdl);
        List<PositionsVM> GetEmployeePositionList();
        //List<PositionsVM> ValidateEmployeePositions(int? EmployeeId);
        List<PositionsVM> ValidateEmployeePositionManager(int? EmployeeId, int payPeriodId, string loggedInUserId);
        List<PositionsVM> ValidateEmployeePositionsByPayPeriod(int? EmployeeId, int payPeriodId);
      //  bool getTimeCardApproval(TimeCardVm timeCardVm, int empId, string userrole,string userName);
        bool GetTimeCard_Approved(TimeCardVm timeCardVm, string username, string userrole);
        string GetPersonName(int employeeId);

        List<EmployeesVM> GetEmployeeDropdownList();

        string GetEmpFilenumber(int empid);
        PayPeriodVM GetEmployeeCurrentPayPeriod(int? EmployeeIdDdl, bool IsArchived);

        string GetEmployeeEmailId(int? employeeIdDdl);
        PersonPhoneNumberVm GetEmployeeMobileNumber(int? employeeIdDdl);
        TimeCardsNotesVM GetTimecardNotes(int? timecardId);
        bool TimecardNotes_SaveAjax(TimeCardsNotesVM timeCardsNotesVM);
        List<EmployeesVM> GetManagerEmployeesList(short? DepartmentIdDdl, string useridentityname, bool isActive);

        List<EmployeesVM> GetManagerEmployeesListByCompanyId(int companycodeId, string useridentityname);

        bool getTimeCardApproval(int EmployeeId, int PayPeriodId, string roleName, string userName);//,bool getTimeCardApproval);

        int Getsessionvalue();
        bool TimeCardApproveAll(TimeCardVm timeCardVm, string userName, string roleName);
        List<TimeCardCollectionVm> TimeCardApproveAllList(TimeCardVm timeCardVm, string userName, string roleName);
        #endregion

        /// Student Login: #2915: Returns multi companies which student belongs to...
        List<CompanyCodeVM> GetStudentCompanyCodes(string email);
        //Student Login: #2915: Returns Departments of Students based on assigned Positions
        List<DepartmentVm> GetStudentDepartmentsList(short? CompanyCodeIdDdl, string useridentityname);
        /// Student Login: #1915: Returns the only Student/Employee
        List<EmployeesVM> GetStudent(short? DepartmentIdDdl, string useridentityname);
        //#2959: Loads payperiods regardless of company   
        List<PayPeriodVM> GetGlobalPayPeriodsList();
        List<EmployeesVM> GetEmployees(int CompanyCodeId);
        List<Positions> GetPositionList(int Employeeid);
        List<Positions> GetManagerPositionByEmployeeId(int Employeeid, string useridentityname);
        List<HoursCodeVm> GetHoursList(int CompanyCodeIdDdl);
        List<PayPeriodVM> GetHourPayPeriodsList(int? EmployeeIdDdl);
        
    }
}
