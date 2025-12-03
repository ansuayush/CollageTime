using ExecViewHrk.Domain.Interface;
using ExecViewHrk.EfClient;
using ExecViewHrk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Domain.Repositories
{
    public class EmployeeRetroHoursRepository : RepositoryBase, IEmployeeRetroHours
    {
        public void RetroDelete(int RetroHourId)
        {

            var EmployeeRetroHoursInDb = _context.EmployeeRetroHours
                .Where(x => x.Id == RetroHourId).FirstOrDefault();
            if (EmployeeRetroHoursInDb != null)
            {
                _context.EmployeeRetroHours.Remove(EmployeeRetroHoursInDb);
                _context.SaveChanges();
            }
        }   
    }
}
