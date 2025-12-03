using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Domain.Repositories
{
    public class PerformanceRepository : RepositoryBase, IPerformanceRepository
    {
        public List<DropDownModel> GetPerformanceList()
        {
            var performanceList = _context.PerformanceProfiles
              .Select(m => new DropDownModel { keyvalue = m.PerProfileID.ToString(), keydescription = m.Description })
              .OrderBy(x => x.keydescription)
              .ToList();
            return performanceList;
        }
    }
}
