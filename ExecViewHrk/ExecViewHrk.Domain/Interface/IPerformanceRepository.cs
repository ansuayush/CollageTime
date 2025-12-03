using ExecViewHrk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Domain.Interface
{
    public interface IPerformanceRepository : IDisposable, IBaseRepository
    {
        List<DropDownModel> GetPerformanceList();
    }
}
