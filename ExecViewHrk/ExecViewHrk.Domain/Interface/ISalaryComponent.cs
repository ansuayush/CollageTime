using ExecViewHrk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ExecViewHrk.Domain.Interface
{
  public   interface ISalaryComponent : IDisposable, IBaseRepository
    {
        List<SalaryComponentViewModel> getSalaryComponentList(int empId);
        SalaryComponentViewModel GetSalaryComponentDetail(int id);

        SalaryComponentViewModel SaveSalaryComponent(SalaryComponentViewModel salaryComponentViewModel);
        void SalaryComponentsDelete(int id);
    }
}
