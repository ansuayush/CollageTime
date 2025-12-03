using ExecViewHrk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Domain.Interface
{
    public interface IDepartmetsRepository : IDisposable, IBaseRepository
    {
        List<DepartmentVm> GetDepartmentList();
        DepartmentVm GetDepartmentDetails(int departmentId);
        bool DepartmentAddUpdate(DepartmentVm departmentVM, string UserId);
        bool DepartmentDestroy(int departmentID, string UserId);
    }
}
