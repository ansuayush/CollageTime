using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Data.Entity.Validation;
using ExecViewHrk.EfClient;
using System.Data.Entity.SqlServer;

namespace ExecViewHrk.Domain.Repositories
{
    public class TimeCardApprovalReportRepository : RepositoryBase, ITimeCardApprovalReportRepository
    {
        public List<int> GetTimecardApprovalEmpList(int compCodeId, int deptId, int? payPeriodId)
        {
            var timecardApproval_empList = new List<int>();
            try
            {
                timecardApproval_empList = _context.TimeCardApprovals
                            .Include("Employees").Where(x => x.Employee.CompanyCodeId == compCodeId &&
                              x.PayPeriodId == payPeriodId && x.Employee.DepartmentId == deptId
                              && x.Approved == true).Select(x => x.EmployeeId).ToList();
            }
            catch (Exception e)
            {
                throw new Exception();
            }
            return timecardApproval_empList;
        }

        public TimeCardApproval GetTimeCardApprovals(int empId, int? payPeriodId)
        {
            TimeCardApproval timeCardApprovalRecordInDb = _context.TimeCardApprovals
           .Where(x => x.EmployeeId == empId && x.PayPeriodId == payPeriodId).SingleOrDefault();
            return timeCardApprovalRecordInDb;
        }

        public IEnumerable<DbEntityValidationResult> GetValidationErrors()
        {
            var errors = _context.GetValidationErrors();
            return errors;
        }

        public List<int> LoadEmpList(int compCodeId, int deptId, int? payPeriodId)
        {
            var employees_List = new List<int>();
            try
            {
                employees_List = Query<int>("sp_LoadEmpList", new { @compCodeId = compCodeId, @deptId = deptId, @payPeriodId = payPeriodId }).ToList();
            }
            catch (Exception e)
            {
                string error = e.ToString();
            }
            return employees_List;
        }

        public List<TimeCardApprovalReportCollectionVm> GetTimeCardApprovalReportList(int? companyCodeId,int? payPeriodId, string userId)
        {
            var employeeWeeklyTimeCardList = new List<TimeCardApprovalReportCollectionVm>();
            try
            {
                if (userId == null)
                {
                    employeeWeeklyTimeCardList = Query<TimeCardApprovalReportCollectionVm>("sp_GetTimeCardApprovalReportList", new { @compCodeId = companyCodeId, @payPeriodId = payPeriodId }).ToList();
                }
                else
                {
                    employeeWeeklyTimeCardList = Query<TimeCardApprovalReportCollectionVm>("sp_GetTimeCardApprovalReportManagerList", new { @compCodeId = companyCodeId, @payPeriodId = payPeriodId, @email = userId }).ToList();
                }
                foreach (var item in employeeWeeklyTimeCardList)
                {
                    item.PersonName = _context.Employees.Where(x => x.EmployeeId == item.EmployeeId).Select(x => x.Person.Firstname + " " + x.Person.Lastname).FirstOrDefault();
                    item.RegularHours = Math.Round(item.RegularHours.Value, 2);
                    item.CodedHours = Math.Round(item.CodedHours.Value, 2);
                    item.Emp_PayPeriodTotal = item.RegularHours.Value + item.OverTime.Value + item.CodedHours.Value;
                }
            }
            catch (Exception e)
            {
                string error = e.ToString();
            }
            return employeeWeeklyTimeCardList;
        }

    }
}
