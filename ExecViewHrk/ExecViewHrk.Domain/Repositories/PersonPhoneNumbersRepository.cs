using ExecViewHrk.Domain.Interface;
using System;
using System.Collections.Generic;
using ExecViewHrk.Models;
using System.Linq;
using System.Data.Entity;

namespace ExecViewHrk.Domain.Repositories
{
    public class PersonPhoneNumbersRepository : RepositoryBase, IPersonPhoneNumbersRepository
    {
        public List<DropDownModel> GetDdlPhoneTypeList()
        {

            var phopneTypeList = _context.DdlPhoneTypes
                .Select(m => new DropDownModel { keyvalue = m.PhoneTypeId.ToString(), keydescription = m.Description })
                .OrderBy(x => x.keydescription)
                .ToList();
            return phopneTypeList;
        }
        public List<DropDownModel> GetPhoneTypeList()
        {
            var phonetypeList = _context.DdlPhoneTypes.Where(x => x.Active == true).Select(m => new DropDownModel { keyvalue = m.PhoneTypeId.ToString(), keydescription = m.Description }).OrderBy(x => x.keydescription).ToList();
            return phonetypeList;
        }
        public List<DropDownModel> GetProviderList()
        {
            var providerList = _context.Providers.Select(m => new DropDownModel { keyvalue = m.ProviderId.ToString(), keydescription = m.ProviderName }).OrderBy(x => x.keydescription).ToList();
            return providerList;
        }

        public PersonPhoneNumberVm GetPersonPhoneNumberRecord(int personPhoneNumberId)
        {
            PersonPhoneNumberVm PersonPhoneNumberVm = _context.PersonPhoneNumbers
               .Include(x => x.DdlPhoneType.Description)
               .Include(x => x.Person.Lastname).Include(x => x.Person.Firstname).Where(x => x.PersonPhoneNumberId == personPhoneNumberId)
               .Select(x => new PersonPhoneNumberVm
               {
                   PersonPhoneNumberId = x.PersonPhoneNumberId,
                   PersonId = x.PersonId,
                   PersonName = x.Person.Lastname + ", " + x.Person.Firstname,
                   PhoneTypeId = x.PhoneTypeId,
                   PhoneTypeDescription = x.DdlPhoneType.Description,
                   PhoneNumber = x.PhoneNumber,
                   Extension = x.Extension,
                   IsPrimaryPhone = x.IsPrimaryPhone,
                   EnteredBy = x.EnteredBy,
                   EnteredDate = x.EnteredDate,
                   ModifiedBy = x.ModifiedBy,
                   ModifiedDate = x.ModifiedDate,
                   ProviderId=x.ProviderId
               }).FirstOrDefault();

            return PersonPhoneNumberVm;
        }

        //public List<PersonPhoneNumberVM> GetPersonPhoneNumberbypersonId(int personId)
        //{
        //    var PersonPhoneNumberVm  = _context.PersonPhoneNumbers
        //            .Where(x => x.PersonId == personId && x.IsPrimaryPhone == true)
        //            .Select(m => new PersonPhoneNumberVM
        //            {
        //                PersonId = m.PersonId,
        //                PersonPhoneNumberId = m.PersonPhoneNumberId,
        //                PhoneNumber = m.PhoneNumber,
        //                PhoneTypeDescription = m.DdlPhoneType.Description,
        //                Extension = m.Extension,
        //                EnteredBy = m.EnteredBy,
        //                EnteredDate = m.EnteredDate,
        //                ModifiedBy = m.ModifiedBy,
        //                ModifiedDate = m.ModifiedDate,
        //            }).OrderBy(x => x.PhoneNumber).ToList();
        //    return PersonPhoneNumberVm;
        // }


        public void PersonPhoneNumbersDelete(int personPhoneNumberIdDdl)
        {
            var dbRecord = _context.PersonPhoneNumbers.Where(x => x.PersonPhoneNumberId == personPhoneNumberIdDdl).SingleOrDefault();
            if (dbRecord != null)
            {
                _context.PersonPhoneNumbers.Remove(dbRecord);

                _context.SaveChanges();
            }

        }

        public List<PersonPhoneNumberVm> GetPersonPhoneNumbersList(int personId)
        {
            var personPhoneList = new List<PersonPhoneNumberVm>();
            try
            {
                 personPhoneList = _context.PersonPhoneNumbers
                    .Where(x => x.PersonId == personId && x.IsPrimaryPhone)
                    .Select(m => new PersonPhoneNumberVm
                    {
                        PersonId = m.PersonId,
                        PersonPhoneNumberId = m.PersonPhoneNumberId,
                        PhoneNumber = m.PhoneNumber,
                        PhoneTypeDescription = m.DdlPhoneType.Description,
                        Extension = m.Extension,
                        EnteredBy = m.EnteredBy,
                        EnteredDate = m.EnteredDate,
                        ModifiedBy = m.ModifiedBy,
                        ModifiedDate = m.ModifiedDate,
                    }).OrderBy(x => x.PhoneNumber).ToList();
            }
            catch(Exception exe)
            {
                throw exe;
            }
            return personPhoneList;
        }
    }
}
