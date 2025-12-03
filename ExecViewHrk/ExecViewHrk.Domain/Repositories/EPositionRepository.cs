using ExecViewHrk.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExecViewHrk.Models;
using System.Data.Entity;
using ExecViewHrk.Domain.Models;
using ExecViewHrk.EfClient;

namespace ExecViewHrk.Domain.Repositories
{
    public class EPositionRepository : RepositoryBase, IEPositionRepository
    {
        public void DeleteEPosition(int _e_PositionId)
        {
            var dbRecord = _context.E_Positions.Where(x => x.E_PositionId == _e_PositionId).FirstOrDefault();
            if (dbRecord != null)
            {
                _context.E_Positions.Remove(dbRecord);

                _context.SaveChanges();
            }
        }
        public void DeleteContract(int Id)
        {
            var dbRecord = _context.Contracts.Where(x => x.Id == Id).FirstOrDefault();
            if (dbRecord != null)
            {
                _context.Contracts.Remove(dbRecord);

                _context.SaveChanges();
            }
        }
        public List<DropDownModel> GetPositionList()
        {
            var list = _context.Positions.Where(x => x.Status == 1).Select(m => new DropDownModel { keyvalue = m.PositionId.ToString(), keydescription = m.Title + "-" + m.Code + m.Suffix }).OrderBy(x => x.keydescription).ToList();
            return list;
        }
        public List<DropDownModel> GetPayFrequencyList()
        {
            var list = _context.DdlPayFrequencies.Where(x => x.Active == true).Select(m => new DropDownModel { keyvalue = m.PayFrequencyId.ToString(), keydescription = m.Description }).OrderBy(x => x.keydescription).ToList();
            return list;
        }
        public List<DropDownModel> GetRateTypeList()
        {
            var list = _context.DdlRateTypes.Where(x => x.Active == true).Select(m => new DropDownModel { keyvalue = m.RateTypeId.ToString(), keydescription = m.Description }).OrderBy(x => x.keydescription).ToList();
            return list;
        }
        public List<DropDownModel> GetPositionCategoryList()
        {
            var list = _context.DdlPositionCategory.Where(m => m.active == true).Select(m => new DropDownModel { keyvalue = m.PositionCategoryID.ToString(), keydescription = m.description }).OrderBy(x => x.keydescription).ToList();
            return list;
        }
        public List<DropDownModel> GetPositionGradeList()
        {
            var list = _context.DdlPositionGrade.Where(m => m.Active == true).Select(m => new DropDownModel { keyvalue = m.PositionGradeID.ToString(), keydescription = m.Description }).OrderBy(x => x.keydescription).ToList();
            return list;
        }
        public List<DropDownModel> GetPositionTypeList()
        {
            var list = _context.DdlPositionTypes.Where(m => m.active == true).Select(m => new DropDownModel { keyvalue = m.PositionTypeId.ToString(), keydescription = m.description }).OrderBy(x => x.keydescription).ToList();
            return list;
        }

        public List<DropDownModel> GetEmployeeTypeList()
        {
            var list = _context.DdlEmployeeTypes.Where(m => m.Active == true).Select(m => new DropDownModel { keyvalue = m.EmployeeTypeId.ToString(), keydescription = m.Description }).OrderBy(x => x.keydescription).ToList();
            return list;
        }

        public List<DropDownModel> GetPayGroupList()
        {
            var list = _context.DdlPayGroups.Where(m => m.Active == true).Select(m => new DropDownModel { keyvalue = m.PayGroupId.ToString(), keydescription = m.Description }).OrderBy(x => x.keydescription).ToList();
            return list;
        }

        public List<DropDownModel> GetReportsToIdList()
        {
            List<DropDownModel> lstdrp = new List<DropDownModel>();
            try
            {
                var getManagerList = _context.Managers
                      .Include("Person")
                     .Select(m => new DropDownModel
                     {
                         keyvalue = m.PersonId.ToString(),
                         keydescription = m.Person.Firstname + " " + m.Person.Lastname
                     }).Distinct().OrderBy(s => s.keydescription).ToList();
                return getManagerList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<E_PositioVm> GetEPositionList(int _personId, int EmpId)
        {
            var lstEPositions = _context.E_Positions
                .Include("Employee.Person")
                .Include("Position.PositionDescription")
                .Include("DdlPayFrequencies.Description")
                .Include("DdlRateTypes.Description")
                .Include("DdlPositionCategory.Description")
                .Include("DdlPositionGrade.Description")
                .Include("DdlPositionTypes.Description")
                .Where(x => x.Employee.PersonId == _personId && x.Employee.EmployeeId == EmpId && x.IsDeleted == false)
                             .Select(x => new E_PositioVm
                             {
                                 E_PositionId = x.E_PositionId,
                                 EmployeeId = x.EmployeeId,
                                 PositionId = x.PositionId,
                                 PayFrequencyId = x.PayFrequencyId,
                                 RateTypeId = x.PositionTypeID,
                                 PersonId = x.Employee.PersonId,
                                 PersonName = x.Employee.Person.Lastname + ", " + x.Employee.Person.Firstname,
                                 PositionDescription = x.Position.PositionDescription + "-" + x.Position.Suffix,
                                 PayFrequencyDescription = x.DdlPayFrequency.Description,
                                 RateTypeDescription = x.DdlRateType.Description,
                                 PositionTypeDesc = x.DdlPositionTypes.description,
                                 StartDate = x.StartDate,
                                 EndDate = x.EndDate,
                                 actualEndDate = x.actualEndDate,
                                 projectedEndDate = x.ProjectedEndDate,
                                 Notes = x.Notes,
                                 PrimaryPosition = x.PrimaryPosition,
                                 EnteredBy = x.EnteredBy,
                                 EnteredDate = x.EnteredDate,
                                 PayGroupId = x.PayGroupId
                             }).ToList();
            return lstEPositions;
        }

        // Duplicated the code to filter positions based on *Company Code*
        public List<E_PositioVm> GetEPositionList_v2(int _personId, int EmpId, int companyCodeId)
        {
            var lstEPositions = _context.E_Positions
                .Include("Employee.Person")
                .Include("Position.PositionDescription")
                .Include("DdlPayFrequencies.Description")
                .Include("DdlRateTypes.Description")
                .Include("DdlPositionCategory.Description")
                .Include("DdlPositionGrade.Description")
                .Include("DdlPositionTypes.Description")
                .Include("Employees")
                .Where(x => x.Employee.PersonId == _personId && x.Employee.EmployeeId == EmpId && x.IsDeleted == false && x.Employee.CompanyCodeId == companyCodeId)
                             .Select(x => new E_PositioVm
                             {
                                 E_PositionId = x.E_PositionId,
                                 EmployeeId = x.EmployeeId,
                                 //  PositionId = x.PositionId,
                                 PayFrequencyId = x.PayFrequencyId,
                                 RateTypeId = x.PositionTypeID,
                                 PersonId = x.Employee.PersonId,
                                 PersonName = x.Employee.Person.Lastname + ", " + x.Employee.Person.Firstname,
                                 PositionDescription = x.Position.PositionDescription + "-" + x.Position.Suffix,
                                 PayFrequencyDescription = x.DdlPayFrequency.Description,
                                 RateTypeDescription = x.DdlRateType.Description,
                                 PositionTypeDesc = x.DdlPositionTypes.description,
                                 StartDate = x.StartDate,
                                 EndDate = x.EndDate,
                                 actualEndDate = x.actualEndDate,
                                 projectedEndDate = x.ProjectedEndDate,
                                 Notes = x.Notes,
                                 PrimaryPosition = x.PrimaryPosition,
                                 EnteredBy = x.EnteredBy,
                                 EnteredDate = x.EnteredDate,
                                 PayGroupId = x.PayGroupId,
                                 EpositionId = x.E_PositionId.ToString() + "-" + x.PositionId.ToString()
                             }).ToList();
            return lstEPositions;
        }

        public List<TreatyNonTreatyTrackingStatusVm> GetTreatyNonTreatyTrackingStatus(string Filenumber)
        {
            var lstTreatyNonTreatyTrackingStatus = _context.TreatyNonTreatyTrackingStatus.Where(m => m.FileNumber == Filenumber)

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

                  }).OrderByDescending(x => x.PayperiodNumber).SingleOrDefault();

            return lstTreatyNonTreatyTrackingStatus;
        }

        public List<DropDownModel> GetEmployeeClassList()
        {
            var list = _context.EmployeeClass.Where(m => m.IsActive == true).Select(m => new DropDownModel { keyvalue = m.EmployeeClassId.ToString(), keydescription = m.ClassName }).OrderBy(x => x.keydescription).ToList();
            return list;
        }



        public List<E_PositioVm> GetEPositionListbyRetrohourDate(int EmpId, int companyCodeId)
        {
            var lstEPositions = _context.E_Positions
                .Include("Employee.Person")
                .Include("Position.PositionDescription")
                .Include("DdlPayFrequencies.Description")
                .Include("DdlRateTypes.Description")
                .Include("DdlPositionCategory.Description")
                .Include("DdlPositionGrade.Description")
                .Include("DdlPositionTypes.Description")
                .Include("Employees")
                .Where(x => x.Employee.EmployeeId == EmpId && x.IsDeleted == false && x.Employee.CompanyCodeId == companyCodeId)
                             .Select(x => new E_PositioVm
                             {
                                 E_PositionId = x.E_PositionId,
                                 EmployeeId = x.EmployeeId,
                                 //  PositionId = x.PositionId,
                                 PayFrequencyId = x.PayFrequencyId,
                                 RateTypeId = x.PositionTypeID,
                                 PersonId = x.Employee.PersonId,
                                 PersonName = x.Employee.Person.Lastname + ", " + x.Employee.Person.Firstname,
                                 PositionDescription = x.Position.PositionDescription + "-" + x.Position.Suffix,
                                 PayFrequencyDescription = x.DdlPayFrequency.Description,
                                 RateTypeDescription = x.DdlRateType.Description,
                                 PositionTypeDesc = x.DdlPositionTypes.description,
                                 StartDate = x.StartDate,
                                 EndDate = x.EndDate,
                                 actualEndDate = x.actualEndDate,
                                 projectedEndDate = x.ProjectedEndDate,
                                 Notes = x.Notes,
                                 PrimaryPosition = x.PrimaryPosition,
                                 EnteredBy = x.EnteredBy,
                                 EnteredDate = x.EnteredDate,
                                 PayGroupId = x.PayGroupId,
                                 EpositionId = x.E_PositionId.ToString() + "-" + x.PositionId.ToString()
                             }).ToList();
            return lstEPositions;
        }
                
        public List<E_PositioVm> GetEPositionListbyManagerId(int EmpId, int companyCodeId, string useridentityname)
        {
            var aspNetUsersEmail = _context.AspNetUsers.Where(x => x.UserName == useridentityname).Select(x => x.Email).FirstOrDefault();
            var ReportsToID = _context.Persons.Where(x => x.eMail == aspNetUsersEmail).Select(x => x.PersonId).FirstOrDefault();
            if (ReportsToID == 0)
            {
                ReportsToID = 0;
            }
            var lstEPositions = _context.E_Positions
               .Include("Employee.Person")
               .Include("Position.PositionDescription")
               .Include("DdlPayFrequencies.Description")
               .Include("DdlRateTypes.Description")
               .Include("DdlPositionCategory.Description")
               .Include("DdlPositionGrade.Description")
               .Include("DdlPositionTypes.Description")
               .Include("Employees")
               .Where(x => x.Employee.EmployeeId == EmpId && x.IsDeleted == false && x.Employee.CompanyCodeId == companyCodeId && x.ReportsToID == ReportsToID)
                            .Select(x => new E_PositioVm
                            {
                                E_PositionId = x.E_PositionId,
                                EmployeeId = x.EmployeeId,
                                 //  PositionId = x.PositionId,
                                 PayFrequencyId = x.PayFrequencyId,
                                RateTypeId = x.PositionTypeID,
                                PersonId = x.Employee.PersonId,
                                PersonName = x.Employee.Person.Lastname + ", " + x.Employee.Person.Firstname,
                                PositionDescription = x.Position.PositionDescription + "-" + x.Position.Suffix,
                                PayFrequencyDescription = x.DdlPayFrequency.Description,
                                RateTypeDescription = x.DdlRateType.Description,
                                PositionTypeDesc = x.DdlPositionTypes.description,
                                StartDate = x.StartDate,
                                EndDate = x.EndDate,
                                actualEndDate = x.actualEndDate,
                                projectedEndDate = x.ProjectedEndDate,
                                Notes = x.Notes,
                                PrimaryPosition = x.PrimaryPosition,
                                EnteredBy = x.EnteredBy,
                                EnteredDate = x.EnteredDate,
                                PayGroupId = x.PayGroupId,
                                EpositionId = x.E_PositionId.ToString() + "-" + x.PositionId.ToString()
                            }).ToList();
            return lstEPositions;
        }
        public List<E_PositionSalaryHistorVm> GetSalaryHistroybyEpositionId(int epositionid)
        {
            var listPositionSalaryHis = _context.E_PositionSalaryHistories.Where(x => x.E_PositionId == epositionid && x.EffectiveDate != null).ToList().Select(x => new E_PositionSalaryHistorVm
            {
                E_PositionSalaryHistoryId = x.E_PositionSalaryHistoryId,
                EffectiveDate = x.EffectiveDate,
                EndDate  = x.EndDate,
                E_PositionId = x.E_PositionId
            }).ToList();
            return listPositionSalaryHis;
        }

     }
}

