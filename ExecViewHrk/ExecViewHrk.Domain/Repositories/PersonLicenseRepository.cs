using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Models;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace ExecViewHrk.Domain.Repositories
{
    public class PersonLicenseRepository : RepositoryBase, IPersonLicenseRepository
    {
        public void DeletePersonLicense(int _personLicenseId)
        {
            var dbRecord = _context.PersonLicenses.Where(x => x.PersonLicenseId == _personLicenseId).FirstOrDefault();
            if (dbRecord != null)
            {
                _context.PersonLicenses.Remove(dbRecord);
                _context.SaveChanges();
            }
        }
        public List<DropDownModel> GetPersonLicenseList()
        {
            var list = _context.DdlLicenseTypes.Where(x => x.Active == true)
               .Select(m => new DropDownModel
               {
                   keyvalue = m.LicenseTypeId.ToString(),
                   keydescription = m.Description
               })
               .OrderBy(x => x.keydescription)
               .ToList();
            return list;
        }
        public List<DropDownModel> GetStateList()
        {
            var list = _context.DdlStates
               .Select(m => new DropDownModel
               {
                   keyvalue = m.StateId.ToString(),
                   keydescription = m.Title
               })
               .OrderBy(x => x.keydescription)
               .ToList();
            return list;
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

        public List<PersonLicensVm> GetPersonLicenseList(int _personId)
        {
            var list = _context.PersonLicenses
               .Include(x => x.DdlLicenseType)
               .Include(x => x.DdlState)
               .Include(x => x.DdlCountry)
               .Include(x => x.Person)
               .Where(x => x.PersonId == _personId)
                               .Select(x => new PersonLicensVm
                               {
                                   PersonLicenseId = x.PersonLicenseId,
                                   PersonId = x.PersonId,
                                   PersonName = x.Person.Lastname + ", " + x.Person.Firstname,
                                   LicenseTypeId = x.LicenseTypeId,
                                   LicenseDescription = x.DdlLicenseType.Description,
                                   StateTitle = x.DdlState.Title,
                                   CountryDescription = x.DdlCountry.Description,
                                   ExpirationDate = x.ExpirationDate,
                                   RevokedDate = x.RevokedDate,
                                   ReinstatedDate = x.ReinstatedDate,
                                   LicenseNumber = x.LicenseNumber,
                                   Notes = x.Notes,
                               }).ToList();
            return list;

        }
    }
}