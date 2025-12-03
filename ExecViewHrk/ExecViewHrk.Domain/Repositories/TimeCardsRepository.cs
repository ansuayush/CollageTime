using ExecViewHrk.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExecViewHrk.Domain.Models;
using ExecViewHrk.Models;

namespace ExecViewHrk.Domain.Repositories
{
    public class TimeCardsRepository : RepositoryBase, ITimeCardsRepository
    {
        public List<PersonVm> GetEmployeeDropdownList()
        {
            var employeeDropdownList = _context.Persons
                .Select(p => new PersonVm
                {
                    PersonId=p.PersonId,
                    PersonName=p.Lastname+" "+p.Firstname+" "+p.MiddleName
                }).ToList();
            return employeeDropdownList;
        }
    }
}
