using ExecViewHrk.Domain.Models;
using ExecViewHrk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Domain.Interface
{
    public interface ITimeCardsRepository : IBaseRepository
    {
        List<PersonVm> GetEmployeeDropdownList();
    }
}
