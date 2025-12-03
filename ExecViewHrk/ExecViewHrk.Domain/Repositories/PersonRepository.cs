using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Models;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System;

namespace ExecViewHrk.Domain.Repositories
{
    public class PersonRepository : RepositoryBase, IPersonRepository
    {
        public List<PersonVm> GetPersonsList(string _type, string _search)
        {
            List<PersonVm> _personList = new List<PersonVm>();
            try
            {
                // _personList = _context.Persons.Select(c => new PersonVm { PersonId = c.PersonId, PersonName = c.Firstname + " " + c.Lastname }).Distinct().OrderBy(s => s.PersonName).ToList();
                _personList = Query<PersonVm>("GET_ROLODEX_USERS", new { type = _type, search = _search }).ToList();
            }
            catch { }
            return _personList;
        }
        public List<PersonVm> GetEmployeesList()
        {
            List<PersonVm> _employeeList = new List<PersonVm>();
            try
            {
                _employeeList = _context.Employees.Select(c => new PersonVm { PersonId = c.PersonId, PersonName = c.Person.Firstname + " " + c.Person.Lastname }).Distinct().OrderBy(s => s.PersonName).ToList();
            }
            catch { }
            return _employeeList;
        }
        public List<PersonVm> GetActiveEmployeesList()
        {
            List<PersonVm> _employeeList = new List<PersonVm>();
            try
            {
                _employeeList = _context.Employees.Where(x => x.TerminationDate == null).Select(c => new PersonVm { PersonId = c.PersonId, PersonName = c.Person.Firstname + " " + c.Person.Lastname }).Distinct().OrderBy(s => s.PersonName).ToList();
            }
            catch { }
            return _employeeList;
        }
        public PersonProfileVm GetRolodexData(int personId)
        {
            return Query<PersonProfileVm>("USP_GET_ROLODEX_DATA", new { PersonId = personId }).FirstOrDefault();

        }
        public List<DropDownModel> GetPersonList()
        {
            string msg = "";
            var list = new List<DropDownModel>();
            try
            {
                list = _context.Persons
                  .Select(m => new DropDownModel
                  {
                      keyvalue = m.PersonId.ToString(),
                      keydescription = m.Lastname + " " + m.Firstname
                  }).Distinct()
                  .OrderBy(x => x.keydescription)
                  .ToList();
            }
            catch (Exception ex) { msg = ex.Message; }
            return list;
        }
        public List<DropDownModel> GetPositionsList()
        {
            string msg = "";
            var list = new List<DropDownModel>();
            try
            {
                list = _context.Positions
                  .Select(m => new DropDownModel
                  {
                      keyvalue = m.PositionId.ToString(),
                      keydescription = m.PositionCode + "-" + m.PositionDescription
                  }).Distinct()
                  .OrderBy(x => x.keydescription)
                  .ToList();
            }
            catch (Exception ex) { msg = ex.Message; }
            return list;
        }
        public List<DropDownModel> GetPositionListbyPositionId(int managerId)
        {
            string msg = "";
            var list = new List<DropDownModel>();
            try
            {
                var itemIds = _context.ManagersPositions.Where(x => x.ManagerID == managerId).Select(m => m.PositionID).ToArray();
                list = _context.Positions.Where(m => itemIds.Contains(m.PositionId))
                  .Select(m => new DropDownModel
                  {
                      keyvalue = m.PositionId.ToString(),
                      keydescription = m.PositionCode + "-" + m.PositionDescription
                  }).Distinct()
                  .OrderBy(x => x.keydescription)
                  .ToList();
            }
            catch (Exception ex) { msg = ex.Message; }
            return list;
        }
        public List<DropDownModel> GetPositionFilterbyPositionId(int managerId)
        {
            string msg = "";
            var list = new List<DropDownModel>();
            try
            {
                var itemIds = _context.ManagersPositions.Where(x => x.ManagerID == managerId).Select(m => m.PositionID).ToArray();
                list = _context.Positions.Where(m => !itemIds.Contains(m.PositionId))
                  .Select(m => new DropDownModel
                  {
                      keyvalue = m.PositionId.ToString(),
                      keydescription = m.PositionCode + "-" + m.PositionDescription
                  }).Distinct()
                  .OrderBy(x => x.keydescription)
                  .ToList();
            }
            catch (Exception ex) { msg = ex.Message; }
            return list;
        }
        public List<PersonProfileVm> GetAllPersons_PositionReportsTo(int PositionID)
        {
            return Query<List<PersonProfileVm>>("SelectPersons_I_ReportTo", new { SubordinatePositionID = PositionID }).FirstOrDefault();

        }

        public PersonProfileVm GetRecordForIDFromPersonIDWithPersonInfo(int personId)
        {
            //    "SELECT Employees.ID, Persons.LastName, Persons.Firstname, Persons.email FROM Employees " +
            //"       INNER JOIN Persons ON Employees.PersonID = Persons.ID " +
            //"WHERE Persons.ID=@ID "
            var personProfiles = (from pn in _context.Persons
                                  join
     emp in _context.Employees on
     pn.PersonId equals emp.PersonId
                                  where pn.PersonId == personId
                                  select new PersonProfileVm
                                  {
                                      EmployeeId = emp.EmployeeId,
                                      LastName = pn.Lastname,
                                      FirstName = pn.Firstname,
                                      Email = pn.eMail,
                                  }).FirstOrDefault();
            return personProfiles;
        }
    }
}