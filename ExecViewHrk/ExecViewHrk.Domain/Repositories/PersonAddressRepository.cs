using ExecViewHrk.Domain.Interface;
using ExecViewHrk.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using ExecViewHrk.Models;
using ExecViewHrk.EfClient;

namespace ExecViewHrk.Domain.Repositories
{
    public class PersonAddressRepository : RepositoryBase, IPersonAddress
    {
        public List<PersonAddressVm> GetPersonAddressesList(int personId)
        {
            var personAddressList = new List<PersonAddressVm>();
            try
            {
                personAddressList = _context.PersonAddresses.Include(x => x.DdlAddressType.Description)
                .Where(a => a.PersonId == personId)
                .Select(m => new PersonAddressVm
                {
                    PersonAddressId = m.PersonAddressId,
                    PersonId = m.PersonId,
                    PersonName = m.Person.Lastname + ", " + m.Person.Firstname,
                    AddressTypeId = m.AddressTypeId,
                    AddressDescription = m.DdlAddressType.Description,
                    StateId = m.StateId,
                    StateTitle = m.DdlState.Title,
                    CountryId = m.CountryId,
                    CountryDescription = m.DdlCountry.Description,
                    AddressLineOne = m.AddressLineOne,
                    AddressLineTwo = m.AddressLineTwo,
                    City = m.City,
                    ZipCode = m.ZipCode,
                    CheckPayrollAddress = m.CheckPayrollAddress == null ? false : m.CheckPayrollAddress,
                    CorrespondenceAddress = m.CorrespondenceAddress == null ? false : m.CorrespondenceAddress,
                    IsPrimaryAddress = m.IsPrimaryAddress == false ? false : m.IsPrimaryAddress,


                }).OrderBy(x => x.AddressDescription).ToList();
               
            }
            catch (Exception exe)
            {
                throw exe;
            }
            return personAddressList;
        }

        public List<PersonAddressVm> GetPersonIsPrimaryAddressList(int personId)
        {
            var personAddressList = new List<PersonAddressVm>();
            try
            {
                personAddressList = _context.PersonAddresses.Include(x => x.DdlAddressType.Description)
                .Where(a => a.PersonId == personId && a.IsPrimaryAddress)
                .Select(m => new PersonAddressVm
                {
                    PersonAddressId = m.PersonAddressId,
                    PersonId = m.PersonId,
                    PersonName = m.Person.Lastname + ", " + m.Person.Firstname,
                    AddressTypeId = m.AddressTypeId,
                    AddressDescription = m.DdlAddressType.Description,
                    StateId = m.StateId,
                    StateTitle = m.DdlState.Title,
                    CountryId = m.CountryId,
                    CountryDescription = m.DdlCountry.Description,
                    AddressLineOne = m.AddressLineOne,
                    AddressLineTwo = m.AddressLineTwo,
                    City = m.City,
                    ZipCode = m.ZipCode,
                    CheckPayrollAddress = m.CheckPayrollAddress == null ? false : m.CheckPayrollAddress,
                    CorrespondenceAddress = m.CorrespondenceAddress == null ? false : m.CorrespondenceAddress,
                    IsPrimaryAddress =  m.IsPrimaryAddress,


                }).OrderBy(x => x.AddressDescription).ToList();

            }
            catch (Exception exe)
            {
                throw exe;
            }
            return personAddressList;
        }
        public PersonAddressVm GetPersonAddressesDetails(int personAddressId)
        {
            try
            {
                var personAddressVm = _context.PersonAddresses
                   .Include(x => x.DdlAddressType.Description)
                   .Include(x => x.DdlCountry.Description)
                   .Include(x => x.DdlState.Title)
                   .Include(x => x.Person.Lastname).Include(x => x.Person.Firstname).Where(x => x.PersonAddressId == personAddressId)
                   .Select(x => new PersonAddressVm
                   {
                       PersonAddressId = x.PersonAddressId,
                       PersonId = x.PersonId,
                       PersonName = x.Person.Lastname + ", " + x.Person.Firstname,
                       AddressTypeId = x.AddressTypeId,
                       AddressDescription = x.DdlAddressType.Description,
                       StateId = x.StateId,
                       StateTitle = x.DdlState.Title,
                       CountryId = x.CountryId,
                       CountryDescription = x.DdlCountry.Description,
                       AddressLineOne = x.AddressLineOne,
                       AddressLineTwo = x.AddressLineTwo,
                       City = x.City,
                       ZipCode = x.ZipCode,
                       CheckPayrollAddress = x.CheckPayrollAddress,
                       CorrespondenceAddress = x.CorrespondenceAddress,
                       EnteredBy = x.EnteredBy,
                       EnteredDate = x.EnteredDate,
                       ModifiedBy = x.ModifiedBy,
                       ModifiedDate = x.ModifiedDate,
                       Notes = x.Notes,
                       IsPrimaryAddress = x.IsPrimaryAddress,
                   })
                   .FirstOrDefault();
                return personAddressVm;
            }
            catch(Exception exe)
            {
                throw exe;
            }
        }
        public  void PersonAddressDeleteAjax(int personAddressId)
        {
            try
            {
                var record = _context.PersonAddresses.Where(d => d.PersonAddressId == personAddressId).FirstOrDefault();
                if(record !=null)
                {
                    _context.PersonAddresses.Remove(record);
                    _context.SaveChanges();
                }
            }
            catch(Exception exe)
            {
                throw exe;
            }
        }
        public List<DdlAddressType> GetAddressTypesList()
        {
            var addressTypeList = new List<DdlAddressType>();
            try
            {
                 addressTypeList = _context.DdlAddressTypes.Where(a => a.Active).OrderBy(d=>d.Description).ToList();
            }
            catch(Exception exe)
            {
                throw exe;
            }
            return addressTypeList;
        }
        public List<DdlState> GetStatesList()
        {
            var lstStates = new List<DdlState>();
            try
            {
                lstStates = _context.DdlStates.OrderBy(t => t.Title).ToList();
            }
            catch(Exception exe)
            {
                throw exe;
            }
            return lstStates;
        }
        public List<DdlCountry> GetCountriesList()
        {
            var lstCountries = new List<DdlCountry>();
            try
            {
                lstCountries = _context.DdlCountries.Where(a => a.Active).OrderBy(d => d.Description).ToList();
            }
            catch(Exception exe)
            {
                throw exe;
            }
            return lstCountries;
        }
    }
}
