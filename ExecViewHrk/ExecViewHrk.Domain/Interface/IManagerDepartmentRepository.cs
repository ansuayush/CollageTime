using ExecViewHrk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Domain.Interface
{
    public interface IManagerDepartmentRepository
    {
        List<ManagerVm> PopulateManagers();
        List<DepartmentVm> PopulateDepartments();
        List<ManagerDepartmentVM> GetManagerDepartmentList();
        ManagerDepartmentVM GetManagerDepartmentDetails(int managerDepartmentId);
        bool ManagerDepartmentsSaveAjax(ManagerDepartmentVM managerDepartmentVM);
        bool ManagerDepartmentsList_Destroy(int managerDepartmentId);
    }
}
