using ExecViewHrk.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExecViewHrk.Models;
using System.Data.Entity;
using System.Web.Mvc;
using ExecViewHrk.Domain.Models;

namespace ExecViewHrk.Domain.Repositories
{
    public class PersonEmployeeRepository : RepositoryBase, IPersonEmployeeRepository
    {
        public List<DropDownModel> GetMaritalStatusList()
        {
            var MaritalStatusList = _context.DdlMaritalStatuses.Where(x => x.Active == true)
                .Select(m => new DropDownModel
                {
                    keyvalue = m.MaritalStatusId.ToString(),
                    keydescription = m.Description
                })
                .OrderBy(x => x.keydescription)
                .ToList();
            return MaritalStatusList;
        }
        public List<DropDownModel> GetEmployeeTypeList()
        {
            var EmployeeTypeList = _context.DdlEmployeeTypes.Where(x => x.Active == true)
                .Select(m => new DropDownModel
                {
                    keyvalue = m.EmployeeTypeId.ToString(),
                    keydescription = m.Description
                })
                .OrderBy(x => x.keydescription)
                .ToList();
            return EmployeeTypeList;
        }

        public List<DropDownModel> GetEmployeeStatusList()
        {
            var employeeStatusList = _context.DdlEmploymentStatuses.Where(x => x.Active == true)
                .Select(m => new DropDownModel
                {
                    keyvalue = m.Code,
                    keydescription = m.Description
                })
                .OrderBy(x => x.keydescription)
                .ToList();
            return employeeStatusList;
        }
        public List<DropDownModel> GetEmployeePayFrequencyList()
        {
            var PayFrequncies = _context.DdlPayFrequencies.Where(x => x.Active == true)
                .Select(m => new DropDownModel
                {
                    keyvalue = m.PayFrequencyId.ToString(),
                    keydescription = m.Description
                })
                .OrderBy(x => x.keydescription)
                .ToList();
            return PayFrequncies;
        }


        public List<DropDownModel> GetPersonsList()
        {

            var personsList = _context.Persons
                .Select(m => new DropDownModel { keyvalue = m.PersonId.ToString(), keydescription = m.Firstname + " " + m.Lastname })
                .OrderBy(x => x.keydescription)
                .ToList();
            return personsList;
        }

        public List<DropDownModel> GetBusinessLevelCodeList()
        {
            var BusinessLevelCodeList = _context.PositionBusinessLevels.Where(x => x.Active == true)
              .Select(m => new DropDownModel
              {
                  keyvalue = m.BusinessLevelNbr.ToString(),
                  keydescription = m.BusinessLevelTitle
              })
              .OrderBy(x => x.keydescription)
              .ToList();
            return BusinessLevelCodeList;
        }

        public List<DropDownModel> GetWorkedStateList()
        {
            var WorkedStateList = _context.DdlStates
               .Select(m => new DropDownModel { keyvalue = m.StateId.ToString(), keydescription = m.Title })
              .OrderBy(x => x.keydescription)
              .ToList();
            return WorkedStateList;
        }
        public List<DropDownModel> GetTimeCardTypesList()
        {
            var timeCardTypesList = _context.DdlTimeCardTypes
                .Select(T => new DropDownModel
                {
                    keyvalue = T.TimeCardTypeId.ToString(),
                    keydescription = T.TimeCardTypeCode + "-" + T.TimeCardTypeDescription
                })
                .OrderBy(x => x.keydescription)
                .ToList();
            return timeCardTypesList;
        }


        public void PersonEmployeesDeleteAjax(int personTestId)
        {
            var dbRecord = _context.Employees.Where(x => x.EmployeeId == personTestId).FirstOrDefault();
            if (dbRecord != null)
            {
                _context.Employees.Remove(dbRecord);

                _context.SaveChanges();
            }


        }


        public PersonEmployeeVm GetPersonEmployeesRecord(int personEmployeeId)
        {
            //string connString = User.Identity.GetClientConnectionString();
            //ClientDbContext clientDbContext = new ClientDbContext(connString);

            PersonEmployeeVm personEmployeeVm = _context.Employees
                .Include(x => x.DdlEmployeeType.Description)
                .Include(x => x.DdlMaritalStatus.Description)
                .Include(x => x.DdlState.Title)
                .Include(x => x.DdlPayFrequency.Description)
                .Include(x => x.DdlRateType.Description)
                .Include(x => x.DdlEmploymentStatus.Description)
                .Include(x => x.DdlTimeCardType)
                .Include(x => x.Person.Lastname).Include(x => x.Person.Firstname).Where(x => x.EmployeeId == personEmployeeId)
                .Select(x => new PersonEmployeeVm
                {
                    EmployeeId = x.EmployeeId,
                    PersonId = x.PersonId,
                    PersonName = x.Person.Lastname + ", " + x.Person.Firstname,
                    EmploymentStatusId = x.EmploymentStatusId,
                    EmploymentStatusDescription = x.DdlEmploymentStatus.Description,
                    EmployeeTypeID = x.EmployeeTypeID,
                    EmployeeTypeDescription = x.DdlEmployeeType.Description,
                    PayFrequencyId = x.PayFrequencyId,
                    PayFrequencyDescription = x.DdlPayFrequency.Description,
                    MaritalStatusID = x.MaritalStatusID,
                    MaritalStatusDescription = x.DdlMaritalStatus.Description,
                    WorkedStateTaxCodeId = x.WorkedStateTaxCodeId,
                    WorkedStateTitle = x.DdlState.Title,
                    RateTypeId = x.RateTypeId,
                    RateTypeDescription = x.DdlRateType.Description,
                    TimeCardTypeId = x.TimeCardTypeId,
                    TimeCardTypeDescription = x.TimeCardTypeId > 0 ? x.DdlTimeCardType.TimeCardTypeCode + "-" + x.DdlTimeCardType.TimeCardTypeDescription : null,
                    CompanyCode = x.CompanyCode,
                    Rate = x.Rate,
                    Hours = x.Hours,
                    FileNumber = x.FileNumber,
                    EmploymentNumber = x.EmploymentNumber,
                    FedExemptions = x.FedExemptions,
                    HireDate = x.HireDate,
                    TerminationDate = x.TerminationDate,
                    PlannedServiceStartDate = x.PlannedServiceStartDate,
                    ActualServiceStartDate = x.ActualServiceStartDate,
                    ProbationEndDate = x.ProbationEndDate,
                    TrainingEndDate = x.TrainingEndDate,
                    SeniorityDate = x.SeniorityDate,
                    EnteredBy = x.EnteredBy,
                    EnteredDate = x.EnteredDate,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedDate = x.ModifiedDate,
                    BusinessLevelNbr = (int)x.CompanyCodeId,
                    DepartmentCodeId = x.DepartmentId,
                    //  CompanyCode=x.CompanyCode
                })
                .FirstOrDefault();


            return personEmployeeVm;
        }

        public PersonEmployeeVm GetPersonTestsRecord(int employeeId, int personId, string businessLevelCode)
        {
            PersonEmployeeVm personEmployeeVm;
            personEmployeeVm = _context.Employees
               .Include("DdlPayFrequencies.Description")
               .Include("DdlMaritalStatuses.Description")
               .Include("DdlEmployeeTypes.Description")
               .Include("DdlEmploymentStatuses.Description")
               .Include(x => x.PersonId)
               .Include(x => x.EmployeeId)
               .Include(x => x.Person.Lastname).Include(x => x.Person.Firstname).Where(x => x.EmployeeId == employeeId)
               .Select(x => new PersonEmployeeVm
               {
                   EmployeeId = x.EmployeeId,
                   PersonId = x.PersonId,
                   EmploymentNumber = x.EmploymentNumber,
                   EmployeeTypeID = x.EmployeeTypeID,
                   EmploymentStatusId = x.EmploymentStatusId,
                   EmploymentStatusDescription = x.DdlEmploymentStatus.Description,
                   MaritalStatusDescription = x.DdlMaritalStatus.Description,
                   PayFrequencyDescription = x.DdlPayFrequency.Description,
                   EmployeeTypeDescription = x.DdlEmployeeType.Description,
                   PayFrequencyId = x.PayFrequencyId,
                   MaritalStatusID = x.MaritalStatusID,
                   Hours = x.Hours,
                   Rate = x.Rate,
                   FedExemptions = x.FedExemptions,
                   FileNumber = x.FileNumber,
                   CompanyCodeId = (short)x.CompanyCodeId,
                   CompanyCode = businessLevelCode,
                   SeniorityDate = x.SeniorityDate,
                   TrainingEndDate = x.TrainingEndDate,
                   ProbationEndDate = x.ProbationEndDate,
                   ActualServiceStartDate = x.ActualServiceStartDate,
                   PlannedServiceStartDate = x.PlannedServiceStartDate,
                   TerminationDate = x.TerminationDate,
                   HireDate = x.HireDate,
                   WorkedStateTaxCodeId = x.WorkedStateTaxCodeId
               })
               .FirstOrDefault();
            return personEmployeeVm;
        }
        public List<PersonEmployeeVm> PersonEmployeeList(int personId)
        {
            var personEmployeeList = _context.Employees
                .Include(x => x.Person)
                 .Include("DdlPayFrequencies.Description")
                  .Include("DdlMaritalStatuses.Description")
                   .Include("DdlEmployeeTypes.Description")
                    .Include("DdlEmploymentStatuses.Description")
                .Where(x => x.PersonId == personId)
                                .Select(x => new PersonEmployeeVm
                                {
                                    EmploymentNumber = x.EmploymentNumber,
                                    EmployeeTypeID = x.EmployeeTypeID,
                                    EmployeeId = x.EmployeeId,
                                    PersonId = x.PersonId,
                                    PersonName = x.Person.Firstname + " " + x.Person.Lastname,
                                    BusinessLevelNbr = (int)x.CompanyCodeId,
                                    CompanyCode = x.CompanyCode,
                                    HireDate = x.HireDate,
                                    TerminationDate = x.TerminationDate,
                                    EmploymentStatusId = x.EmploymentStatusId,
                                    EmploymentStatusDescription = x.DdlEmploymentStatus.Description,
                                    MaritalStatusDescription = x.DdlMaritalStatus.Description,
                                    PayFrequencyDescription = x.DdlPayFrequency.Description,
                                    EmployeeTypeDescription = x.DdlEmployeeType.Description,
                                    PayFrequencyId = x.PayFrequencyId,
                                    MaritalStatusID = x.MaritalStatusID,
                                    TimeCardTypeId = x.TimeCardTypeId,
                                    TimeCardTypeDescription = x.DdlTimeCardType.TimeCardTypeDescription,
                                    CompanyCodeId = (int)x.CompanyCodeId,
                                    IsStudent = x.IsStudent
                                }).ToList();
            return personEmployeeList;
        }

        public List<DropDownModel> GetDepartmentsList(int companyCode)
        {
            var departmentsList = _context.Departments.Where(r => r.CompanyCodeId == companyCode && r.IsDeleted == false)
                .Select(T => new DropDownModel
                {
                    keyvalue = T.DepartmentId.ToString(),
                    keydescription = T.DepartmentCode+"-"+T.DepartmentDescription
                })
                // .OrderBy(x => x.keydescription)
                .ToList();
            return departmentsList;
        }
        public List<DropDownModel> GetCompanyCodeList()
        {
            var departmentsList = _context.CompanyCodes.Where(r => r.IsCompanyCodeActive == true)
                .Select(T => new DropDownModel
                {
                    keyvalue = T.CompanyCodeId.ToString(),
                    keydescription = T.CompanyCodeDescription
                })
                // .OrderBy(x => x.keydescription)
                .ToList();
            return departmentsList;
        }
        public List<DropDownModel> GetEarningsCodesList(int companyCode)
        {
            var earningsCodes = _context.EarningsCodes.Where(r => r.CompanyCodeId == companyCode && r.IsEarningsCodeActive==true)
               .Select(T => new DropDownModel
               {
                   keyvalue = T.EarningsCodeId.ToString(),
                   keydescription = T.EarningsCodeCode
               }).OrderBy(x => x.keydescription).ToList();
            return earningsCodes;
        }

        public List<DropDownModel> GetReportsToList(int personId)
        {
            var reportsToList = _context.Persons.Where(r => r.PersonId != personId)
               .Select(T => new DropDownModel
               {
                   keyvalue = T.PersonId.ToString(),
                   keydescription = T.Firstname + " " + T.Lastname
               }).OrderBy(x => x.keydescription).Distinct().ToList();
            return reportsToList;
        }


        public List<TreatyNonTreatyTrackingStatusVm> GetTreatyNonTreatyTrackingStatus(string Filenumber)
        {
            var lstTreatyNonTreatyTrackingStatus = _context.TreatyNonTreatyTrackingStatus.Where(m => m.FileNumber == Filenumber )

                  .Select(x => new TreatyNonTreatyTrackingStatusVm
                  {
                      RegHours = x.RegHours,
                      PayRate = x.PayRate

                  })
                .ToList();

            return lstTreatyNonTreatyTrackingStatus;
        }


        public TreatyNonTreatyTrackingStatusVm GetTreatyNonTreaty(string Filenumber)
        {
            var lstTreatyNonTreatyTrackingStatus = _context.TreatyNonTreatyTrackingStatus.Where(m => m.FileNumber == Filenumber)

                  .Select(x => new TreatyNonTreatyTrackingStatusVm
                  {
                      RegHours = x.RegHours,
                      PayRate = x.PayRate

                  }).OrderByDescending(m => m.PayperiodNumber).SingleOrDefault();              

            return lstTreatyNonTreatyTrackingStatus;
        }


    }
}








