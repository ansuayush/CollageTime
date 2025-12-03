using System.Collections.Generic;
using System.Linq;
using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Models;
using System.Data.Entity;

namespace ExecViewHrk.Domain.Repositories
{
    public class PersonVehicleRepository : RepositoryBase, IPersonVehicleRepository
    {
        public void DeletePersonVehicle(int _personVehicleId)
        {
            var dbRecord = _context.PersonVehicles.Where(x => x.PersonVehicleId == _personVehicleId).FirstOrDefault();
            if (dbRecord != null)
            {
                _context.PersonVehicles.Remove(dbRecord);

                _context.SaveChanges();
            }
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
        public List<PersonsVehicleVm> GetPersonVehicleList(int _personId)
        {
            var list = _context.PersonVehicles
               .Include(x => x.DdlState)
               .Include(x => x.Person)
               .Where(x => x.PersonId == _personId)
                               .Select(x => new PersonsVehicleVm
                               {
                                   PersonVehicleId = x.PersonVehicleId,
                                   PersonId = x.PersonId,
                                   PersonName = x.Person.Lastname + ", " + x.Person.Firstname,
                                   StateId = x.StateId,
                                   StateTitle = x.DdlState.Title,
                                   AcquisitionDate = x.AcquisitionDate,
                                   SoldDate = x.SoldDate,
                                   Make = x.Make,
                                   Model = x.Model,
                                   Color = x.Color,
                                   LicenseNumber = x.LicenseNumber,
                                   Notes = x.Notes,
                               }).ToList();
            return list;
        }
    }
}
