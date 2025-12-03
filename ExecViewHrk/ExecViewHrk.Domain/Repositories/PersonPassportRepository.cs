using ExecViewHrk.Domain.Interface;
using System.Collections.Generic;
using System.Linq;
using ExecViewHrk.Models;
using System.Data.Entity;


namespace ExecViewHrk.Domain.Repositories
{
    public class PersonPassportRepository : RepositoryBase, IPersonPassportRepository
    {
        public void DeletePersonPassport(int _personPassportId)
        {
            var dbRecord = _context.PersonPassports.Where(x => x.PersonPassportId == _personPassportId).FirstOrDefault();
            if (dbRecord != null)
            {
                _context.PersonPassports.Remove(dbRecord);

                _context.SaveChanges();
            }
        }
        public List<DropDownModel> GetCountryList()
        {
            var list = _context.DdlCountries.Where(x => x.Active == true)
               .Select(m => new DropDownModel
               {
                   keyvalue = m.CountryId.ToString(),
                   keydescription = m.Description
               })
               .OrderBy(x => x.keydescription)
               .ToList();
            return list;
        }
        public List<PersonsPassportVm> GetPersonPassportList(int _personId)
        {
            var list = _context.PersonPassports
              .Include(x => x.DdlCountry)
              .Include(x => x.Person)
              .Where(x => x.PersonId == _personId)
                              .Select(x => new PersonsPassportVm
                              {
                                  PersonPassportId = x.PersonPassportId,
                                  PersonId = x.PersonId,
                                  PersonName = x.Person.Lastname + ", " + x.Person.Firstname,
                                  PassportNumber = x.PassportNumber,
                                  PassportStorage = x.PassportStorage,
                                  CountryDescription = x.DdlCountry.Description,
                                  ExpirationDate = x.ExpirationDate,
                                  EnteredBy = x.EnteredBy,
                                  IssueDate = x.IssueDate,
                                  EnteredDate = x.EnteredDate
                              }).ToList();
            return list;
        }
    }
}
