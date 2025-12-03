using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Models;
using ExecViewHrk.EfClient;
using System.Data.Entity.SqlServer;
using ExecViewHrk.Domain;
using System.Data.Entity;

namespace ExecViewHrk.Domain.Repositories
{
    public class LookupTablesRepository : RepositoryBase, ILookupTablesRepository
    {
        public List<DdlRelationshipTypeVm> getDdlRelationshipTypesList()
        {
            var relationshipTypesList = _context.DdlRelationshipTypes.Select(m => new DdlRelationshipTypeVm
            {
                RelationshipTypeId = m.RelationshipTypeId,
                Code = m.Code,
                Description = m.Description,
                IsSpouse = m.IsSpouse,
                CobraEligible = m.CobraEligible,
                Active = m.Active,
            }).
            OrderByDescending(e => e.Description).ToList();
            return relationshipTypesList;
        }
        public DdlRelationshipTypeVm getDdlRelationshipTypesDetails(int relationshipTypeId)
        {
            DdlRelationshipTypeVm ddlRelationshipTypesVM = new DdlRelationshipTypeVm();
            if (relationshipTypeId != 0)
            {
                ddlRelationshipTypesVM = _context.DdlRelationshipTypes
                .Where(x => x.RelationshipTypeId == relationshipTypeId)
                .Select(m => new DdlRelationshipTypeVm
                {
                    RelationshipTypeId = m.RelationshipTypeId,
                    Code = m.Code,
                    Description = m.Description,
                    CobraEligible = m.CobraEligible,
                    Active = m.Active,
                    IsSpouse = m.IsSpouse,
                }).FirstOrDefault();
            }
            else
            {
                ddlRelationshipTypesVM.RelationshipTypeId = 0;
            }
            return ddlRelationshipTypesVM;
        }
        public DdlRelationshipTypeVm updateDdlRelationshipTypes(DdlRelationshipTypeVm dlRelationshipTypeVm)
        {
            DdlRelationshipType ddlRelationshipType = new DdlRelationshipType();
            DdlRelationshipType ddlRelationshipTypeRecord = _context.DdlRelationshipTypes.Where(x => x.RelationshipTypeId == dlRelationshipTypeVm.RelationshipTypeId).SingleOrDefault();
            if (ddlRelationshipTypeRecord == null)
            {
                var newRelationshipType = new DdlRelationshipType
                {
                    Description = dlRelationshipTypeVm.Description,
                    Code = dlRelationshipTypeVm.Code,
                    Active = dlRelationshipTypeVm.Active,
                    CobraEligible = dlRelationshipTypeVm.CobraEligible,
                    IsSpouse = dlRelationshipTypeVm.IsSpouse
                };

                _context.DdlRelationshipTypes.Add(newRelationshipType);
                _context.SaveChanges();
                dlRelationshipTypeVm.RelationshipTypeId = newRelationshipType.RelationshipTypeId;
            }
            else
            {

                ddlRelationshipTypeRecord.Code = dlRelationshipTypeVm.Code;
                ddlRelationshipTypeRecord.Description = dlRelationshipTypeVm.Description;
                ddlRelationshipTypeRecord.Active = dlRelationshipTypeVm.Active;
                ddlRelationshipTypeRecord.CobraEligible = dlRelationshipTypeVm.CobraEligible;
                ddlRelationshipTypeRecord.IsSpouse = dlRelationshipTypeVm.IsSpouse;
                _context.SaveChanges();
            }
            return dlRelationshipTypeVm;
        }
        public void deleteDdlRelationshipTypes(int relationshipTypeId)
        {
            var dbRecord = _context.DdlRelationshipTypes.Where(x => x.RelationshipTypeId == relationshipTypeId).SingleOrDefault();
            if (dbRecord != null)
            {
                _context.DdlRelationshipTypes.Remove(dbRecord);

                _context.SaveChanges();
            }
        }


        public List<DdlEducationEstablishmentViewModel> getDdlEducationEstablishmentList()
        {
            var educationEstablishmentList = new List<DdlEducationEstablishmentViewModel>();
            educationEstablishmentList = _context.DdlEducationEstablishments
              .Select(x => new DdlEducationEstablishmentViewModel
              {
                  EducationEstablishmentId = x.EducationEstablishmentId,
                  Description = x.Description,
                  Code = x.Code,
                  AddressLineOne = x.AddressLineOne,
                  AddressLineTwo = x.AddressLineTwo,
                  City = x.City,
                  StateId = x.StateId,
                  ZipCode = x.ZipCode,
                  CountryId = x.CountryId,
                  PhoneNumber = x.PhoneNumber,
                  FaxNumber = x.FaxNumber,
                  //EducationTypeId = x.EducationTypeId,
                  AccountNumber = x.AccountNumber,
                  Contact = x.Contact,
                  WebAddress = x.WebAddress,
                  Notes = x.Notes,
                  Active = x.Active,
                  //DdlCountry = x.DdlCountry,
                  //DdlEducationType = x.DdlEducationType,
                  //DdlState = x.DdlState,
                  //PersonEducations = x.PersonEducations,
              }).
          OrderByDescending(e => e.Description).ToList();
            return educationEstablishmentList;
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

        public List<CompanyCodeVM> GetCompanyCodes()
        {

            List<CompanyCodeVM> companyCodeslist = _context.CompanyCodes
                   .Where(x => x.IsCompanyCodeActive == true)
                   .Select(m => new CompanyCodeVM
                   {
                       CompanyCodeId = m.CompanyCodeId,
                       CompanyCodeDescription = m.CompanyCodeCode + " - " + m.CompanyCodeDescription
                   }).OrderBy(x => x.CompanyCodeDescription).ToList();
            return companyCodeslist;
        }

        /// <summary>
        /// Student Login: #2915:Returns multi companies which student belongs to...
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public List<CompanyCodeVM> GetStudentCompanyCodes(string email)
        {
            var employeeCompanyIDs = (from e in _context.Employees
                                      where e.PersonId == (from p in _context.Persons where p.eMail.Equals(email) select p.PersonId).FirstOrDefault()
                                      select e.CompanyCodeId
                         );

            List<CompanyCodeVM> companyCodeslist = _context.CompanyCodes
                   .Where(x => x.IsCompanyCodeActive == true)
                   .Select(m => new CompanyCodeVM
                   {
                       CompanyCodeId = m.CompanyCodeId,
                       CompanyCodeDescription = m.CompanyCodeCode + " - " + m.CompanyCodeDescription
                   }).OrderBy(x => x.CompanyCodeDescription).ToList();
            return companyCodeslist.Where(x => employeeCompanyIDs.Any(y => y.Value == x.CompanyCodeId)).ToList(); 
        }

        public string GetEmployeeEmailId(int? employeeIdDdl)
        {
            string emailId = "";

            try
            {
                emailId = _context.Employees.Include("Person").Where(e => e.EmployeeId == employeeIdDdl).Select(x => x.Person.eMail).SingleOrDefault();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
            return emailId;
        }

        public PersonPhoneNumberVm GetEmployeeMobileNumber(int? employeeIdDdl)
        {
            PersonPhoneNumberVm personPhoneNumberVm = new PersonPhoneNumberVm();

            try
            {
                personPhoneNumberVm = (from ph in _context.PersonPhoneNumbers
                                       join em in _context.Employees on ph.PersonId equals em.PersonId
                                       join pr in _context.Providers on ph.ProviderId equals pr.ProviderId
                                       where em.EmployeeId == employeeIdDdl
                                       select new PersonPhoneNumberVm()
                                       {
                                           PhoneNumber = /*ph.Extension +*/ ph.PhoneNumber,
                                           Gateway = pr.Gateway

                                       }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
            return personPhoneNumberVm;
        }

        public TimeCardsNotesVM GetTimecardNotes(int? timecardId)
        {
            TimeCardsNotesVM timeCardsNotesVM = new TimeCardsNotesVM();

            try
            {
                timeCardsNotesVM = _context.TimeCardsNotes.Where(m => m.TimeCardId == timecardId)
                    .Select(x => new TimeCardsNotesVM
                    {
                        TimeCardsNotesId = x.TimeCardsNotesId,
                        EmployeeId = x.EmployeeId,
                        CompanyCodeId = x.CompanyCodeId,
                        FileNumber = x.FileNumber,
                        TimeCardId = timecardId.Value,
                        ActualDate = x.ActualDate,
                        Notes = x.Notes
                    }).FirstOrDefault();
                if (timeCardsNotesVM == null)
                {
                    timeCardsNotesVM = _context.TimeCards.Include("Employee").Where(m => m.TimeCardId == timecardId)
                        .Select(x => new TimeCardsNotesVM
                        {
                            EmployeeId = x.EmployeeId,
                            CompanyCodeId = x.CompanyCodeId,
                            FileNumber = x.Employee.FileNumber,
                            TimeCardId = timecardId.Value,
                            ActualDate = x.ActualDate,
                        }).FirstOrDefault();
                }
            }

            catch (Exception ex)
            {
                string message = ex.Message;
            }

            return timeCardsNotesVM;
        }

        public bool TimecardNotes_SaveAjax(TimeCardsNotesVM timeCardsNotesVM)
        {
            bool result = false;
            var timecardNotes = _context.TimeCardsNotes.Where(m => m.TimeCardId == timeCardsNotesVM.TimeCardId).FirstOrDefault();
            try
            {
                if (timecardNotes == null)
                {
                    TimeCardNotes timeCardNotes = new TimeCardNotes()
                    {
                        TimeCardId = timeCardsNotesVM.TimeCardId,
                        EmployeeId = timeCardsNotesVM.EmployeeId,
                        CompanyCodeId = timeCardsNotesVM.CompanyCodeId,
                        FileNumber = timeCardsNotesVM.FileNumber,
                        ActualDate = timeCardsNotesVM.ActualDate,
                        Notes = timeCardsNotesVM.Notes
                    };
                    _context.TimeCardsNotes.Add(timeCardNotes);
                    result = _context.SaveChanges() > 0 ? true : false;
                }
                else
                {
                    //timecardNotes.CompanyCodeId = timeCardsNotesVM.CompanyCodeId;
                    //timecardNotes.FileNumber = timeCardsNotesVM.FileNumber;
                    //timecardNotes.ActualDate = timeCardsNotesVM.ActualDate;
                    timecardNotes.Notes = timeCardsNotesVM.Notes;
                    result = _context.SaveChanges() > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }

            return result;
        }

        public bool PostionStatus(int employeeId)
        {
            bool result = false;
            var payPeriod = (from ep in _context.E_Positions
                             join pf in _context.DdlPayFrequencies on ep.PayFrequencyId equals pf.PayFrequencyId
                             join pp in _context.PayPeriods on pf.PayFrequencyId equals pp.PayFrequencyId
                             where ep.PayGroupId == pp.PayGroupCode && ep.EmployeeId == employeeId && pp.StartDate <= System.DateTime.UtcNow &&
                             pp.IsPayPeriodActive == true
                             select pp
                             ).OrderByDescending(x => x.EndDate).FirstOrDefault();
            var ePostion = _context.E_Positions.FirstOrDefault(x => x.EmployeeId == employeeId);
            if (ePostion != null)
            {
                if (ePostion.StartDate.HasValue && !ePostion.actualEndDate.HasValue)
                {
                    return result;
                }
                else if (ePostion.StartDate.HasValue && ePostion.actualEndDate.HasValue
                                                       && (ePostion.actualEndDate >= payPeriod.StartDate && ePostion.actualEndDate <= payPeriod.EndDate))
                {
                    result = true;

                }
                else if (ePostion.StartDate.HasValue && ePostion.actualEndDate.HasValue
                                                    && (ePostion.StartDate <= payPeriod.EndDate && ePostion.actualEndDate >= payPeriod.StartDate
                                                                                            && ePostion.actualEndDate >= payPeriod.EndDate))
                {
                    result = true;
                }
            }
            return result;
        }

        #region TimeCards Binding Methods



        #region Grid Filters and Display Columns based on TimeCardConfiguration 

        /// <summary>
        /// Returns Admin Department List
        /// </summary>
        public List<DepartmentVm> GetAdminDepartmentsList(short? CompanyCodeIdDdl)
        {
            List<DepartmentVm> departmentList = _context.Departments
                    .Where(x => x.CompanyCodeId == CompanyCodeIdDdl && x.IsDeleted == false)
                    .Select(m => new DepartmentVm
                    {
                        DepartmentId = m.DepartmentId,
                        CompCode_DeptCode_DeptDescription = m.DepartmentCode + "-" + m.DepartmentDescription
                    }).Distinct().OrderBy(m => m.CompCode_DeptCode_DeptDescription).ToList();
            return departmentList;
        }

        /// <summary>
        /// Returns Managers Department List
        /// </summary>
        public List<DepartmentVm> GetManagersDepartmentsList(short? CompanyCodeIdDdl, string useridentityname)
        {
            var aspNetUsersEmail = _context.AspNetUsers.Where(x => x.UserName == useridentityname).Select(x => x.Email).FirstOrDefault();
            List<DepartmentVm> departmentList = _context.ManagerDepartments
                   .Include("Department.CompanyCode")
                   .Include("Manager.Person")
                   //.Where(x => x.Manager.Person.eMail == useridentityname && x.Department.CompanyCodeId == CompanyCodeIdDdl)
                   .Where(x => x.Manager.Person.eMail == aspNetUsersEmail && x.Department.CompanyCodeId == CompanyCodeIdDdl && x.Department.IsDeleted == false)
                   .Select(m => new DepartmentVm
                   {
                       DepartmentId = m.Department.DepartmentId,
                       CompCode_DeptCode_DeptDescription = m.Department.DepartmentCode + "-" + m.Department.DepartmentDescription
                   }).Distinct().OrderBy(m => m.CompCode_DeptCode_DeptDescription).ToList();
            return departmentList;         

        }

        /// <summary>
        /// Student Login: #2915: Returns Departments of Students based on assigned Positions
        /// </summary>
        /// <param name="CompanyCodeIdDdl"></param>
        /// <param name="useridentityname"></param>
        /// <returns></returns>
        public List<DepartmentVm> GetStudentDepartmentsList(short? CompanyCodeIdDdl, string useridentityname)
        {
            var employeeID = (from e in _context.Employees
                                      where e.CompanyCodeId == CompanyCodeIdDdl && 
                                            e.PersonId == (from p in _context.Persons where p.eMail.Equals(useridentityname) select p.PersonId).FirstOrDefault()
                                      select e.EmployeeId
                         ).FirstOrDefault();

            //var departmentList = (from d in _context.Departments
            //                      join p in _context.Positions on d.DepartmentId equals p.DepartmentId
            //                      join ep in _context.E_Positions on p.PositionId equals ep.PositionId
            //                      where ep.EmployeeId == employeeID && d.CompanyCodeId == CompanyCodeIdDdl && d.IsDeleted == false
            //                      select new DepartmentVm
            //                      {
            //                          DepartmentId = d.DepartmentId,
            //                          CompCode_DeptCode_DeptDescription = d.DepartmentCode + "-" + d.DepartmentDescription
            //                      }).Distinct().OrderBy(m => m.CompCode_DeptCode_DeptDescription).ToList();

            //Returns Departments based on assigned Positions departments...
            //&& ep.PrimaryPosition == true
            var departmentList = (from d in _context.Departments
                                  join ep in _context.E_Positions on d.DepartmentId equals ep.DepartmentId
                                  where ep.EmployeeId == employeeID && d.CompanyCodeId == CompanyCodeIdDdl
                                                                    && d.IsDeleted == false
                                  select new DepartmentVm
                                  {
                                      DepartmentId = d.DepartmentId,
                                      CompCode_DeptCode_DeptDescription = d.DepartmentCode + "-" + d.DepartmentDescription
                                  }).Distinct().OrderBy(m => m.CompCode_DeptCode_DeptDescription).ToList();

            return departmentList;
        }

        /// <summary>
        /// Returns Employee List 
        /// </summary>     
        public List<EmployeesVM> GetEmployeesList(short? DepartmentIdDdl, bool isActive)
        {
            //2857-Filter Employee by CompanyId
            var companycodeId = _context.Departments.Where(x => x.DepartmentId == (int)DepartmentIdDdl).Select(d => d.CompanyCodeId).FirstOrDefault();

            var employeeList = (from x in _context.Employees
                                join per in _context.Persons on x.PersonId equals per.PersonId
                                join ep in _context.E_Positions on x.EmployeeId equals ep.EmployeeId
                                where (ep.DepartmentId == DepartmentIdDdl && ep.IsDeleted == false
                                                                          && x.CompanyCodeId == companycodeId) //2857-Filter Employee by CompanyId
                                select new EmployeesVM
                                {
                                    //deptid = dp.DepartmentId,                                    
                                    EmployeeId = x.EmployeeId,
                                    EmployeeFullName = per.Firstname + " " + per.Lastname,
                                    EmployeeRole = "Permanent",
                                    //FileNumber = x.FileNumber,
                                    StatusCode = x.DdlEmploymentStatus.Code,
                                }).OrderBy(m => m.EmployeeFullName).Distinct().ToList();
            if (isActive)
            {
                //foreach (var item in employeeList)
                //{
                //    item.IsPostionEnded = PostionStatus(item.EmployeeId);
                //}
                //employeeList = employeeList.Where(x => x.StatusCode == "A" && x.IsPostionEnded == false).ToList();
                //added for #4401
                employeeList = employeeList.Where(x => x.StatusCode == "A").ToList();
            }          

            return employeeList.Distinct().ToList();
        }

        public List<EmployeesVM> GetEmployees(int CompanyCodeId)
        {
            var employeeList = (from x in _context.Employees
                                join per in _context.Persons on x.PersonId equals per.PersonId
                                join Cd in _context.CompanyCodes on x.CompanyCodeId equals Cd.CompanyCodeId
                                where (x.CompanyCodeId == CompanyCodeId)
                                select new EmployeesVM
                                {                                     
                                    EmployeeId = x.EmployeeId,
                                    EmployeeFullName = per.Firstname + " " + per.Lastname,
                                    EmployeeRole = "Permanent",
                                    StatusCode = x.DdlEmploymentStatus.Code,
                                }).OrderBy(m => m.EmployeeFullName).Distinct().ToList();
            employeeList = employeeList.Where(x => x.StatusCode == "A").ToList();
            return employeeList.Distinct().ToList();

        }
        public List<HoursCodeVm> GetHoursList(int CompanyCodeId)
        {
            List<HoursCodeVm> hourscodelist = new List<HoursCodeVm>();
            var HoursList = (from x in _context.HoursCodes
                                where (x.CompanyCodeId == CompanyCodeId && x.IsRetro == true && x.StartDate == null && x.EndDate == null )
                                select new HoursCodeVm
                                {                               
                                    HoursCodeId=x.HoursCodeId,
                                    HoursCodeCode= x.HoursCodeCode
                                }).OrderBy(m => m.HoursCodeId).Distinct().ToList();      
            return HoursList;
        }


        public List<Positions> GetPositionList(int Employeeid)
        {

            var PositionList = (from x in _context.Employees
                                join EP in _context.E_Positions on x.EmployeeId equals EP.EmployeeId
                                join po in _context.Positions on EP.PositionId equals po.PositionId
                                where (EP.EmployeeId == Employeeid)
                                select new Positions
                                {
                                    PositionId = EP.E_PositionId,
                                    PositionCode = po.PositionDescription,
                                }).OrderBy(m => m.PositionCode).ToList();
            return PositionList.Distinct().ToList();

        }
        public List<PayPeriodVM> GetHourPayPeriodsList(int? EmployeeIdDdl)
        {
            var payGroupId = _context.E_Positions.Where(x => x.EmployeeId == EmployeeIdDdl).Select(X => X.PayGroupId).First();

            List<PayPeriodVM> payPeriodsList = new List<PayPeriodVM>();
            int? payfrequencyid = _context.Employees.Where(x => x.EmployeeId == EmployeeIdDdl).Select(x => x.PayFrequencyId).FirstOrDefault();
            if (payfrequencyid != null)
            {
                payPeriodsList = _context.PayPeriods
                 .Where(m => m.PayFrequencyId == payfrequencyid && m.StartDate <= System.DateTime.UtcNow
                                                                && m.IsPayPeriodActive == true //SHOULD IMPORT 1 TO THIS COLUMN DURING MAIN EMPLOYEE IMPORT
                                                                && m.PayGroupCode == payGroupId
                                                                && m.IsDeleted == false
                                                                )
                  .Select(m => new PayPeriodVM
                  {
                      PayPeriodId = m.PayPeriodId,
                      EndDate = m.EndDate,
                      PayGroupCode = m.PayGroupCode,
                      PayPeriod = SqlFunctions.StringConvert((double)SqlFunctions.DatePart("mm", m.StartDate)).Trim() + "/" + SqlFunctions.DateName("DAY", m.StartDate) + "/" + SqlFunctions.DateName("YYYY", m.StartDate)
                                 + " - " + SqlFunctions.StringConvert((double)SqlFunctions.DatePart("mm", m.EndDate)).Trim() + "/" + SqlFunctions.DateName("DAY", m.EndDate) + "/" + SqlFunctions.DateName("YYYY", m.EndDate)
                  }).OrderByDescending(m => m.EndDate).ToList();//.Take(6).ToList();
            }
            return payPeriodsList;
        }

        public List<EmployeesVM> GetManagerEmployeesList(short? DepartmentIdDdl, string useridentityname, bool isActive)
        {
            //2857-Filter Employee by CompanyId
            var companycodeId = _context.Departments.Where(x => x.DepartmentId == (int)DepartmentIdDdl).Select(d => d.CompanyCodeId).FirstOrDefault();

            var aspNetUsersEmail = _context.AspNetUsers.Where(x => x.UserName == useridentityname).Select(x => x.Email).FirstOrDefault();
            var personId = _context.Persons.Where(x => x.eMail == aspNetUsersEmail).Select(x => x.PersonId).FirstOrDefault();

            var employeeList = (from x in _context.Employees
                                join per in _context.Persons on x.PersonId equals per.PersonId
                                join ep in _context.E_Positions on x.EmployeeId equals ep.EmployeeId
                                where (ep.DepartmentId == DepartmentIdDdl && ep.ReportsToID
                                == personId && ep.IsDeleted == false
                                            && x.CompanyCodeId == companycodeId) //2857-Filter Employee by CompanyId
                                select new EmployeesVM
                                {
                                    deptid = ep.DepartmentId,
                                    EmployeeId = x.EmployeeId,
                                    EmployeeFullName = per.Firstname + " " + per.Lastname,
                                    EmployeeRole = "Permanent",
                                    FileNumber = x.FileNumber,
                                    StatusCode = x.DdlEmploymentStatus.Code
                                }).OrderBy(m => m.EmployeeFullName).Distinct().ToList();
            if (isActive)
            {
                foreach (var item in employeeList)
                {
                    item.IsPostionEnded = IsActivePostionStatusbyManager(item.EmployeeId, personId, item.deptid);
                }
                employeeList = employeeList.Where(x => x.StatusCode == "A" && x.IsPostionEnded == true).ToList();
            }

            return employeeList;
        }


        public List<EmployeesVM> GetManagerEmployeesListByCompanyId(int companycodeId, string useridentityname)
        {         
            var aspNetUsersEmail = _context.AspNetUsers.Where(x => x.UserName == useridentityname).Select(x => x.Email).FirstOrDefault();
            var personId = _context.Persons.Where(x => x.eMail == aspNetUsersEmail).Select(x => x.PersonId).FirstOrDefault();
            var employeeList = (from x in _context.Employees
                                join per in _context.Persons on x.PersonId equals per.PersonId
                                join ep in _context.E_Positions on x.EmployeeId equals ep.EmployeeId
                                where (  ep.ReportsToID == personId && ep.IsDeleted == false
                                            && x.CompanyCodeId == companycodeId) 
                                select  new EmployeesVM
                                {
                                    //deptid = ep.DepartmentId,
                                    EmployeeId = x.EmployeeId,
                                    EmployeeFullName = per.Firstname + " " + per.Lastname,
                                    EmployeeRole = "Permanent",
                                    FileNumber = x.FileNumber,
                                    StatusCode = x.DdlEmploymentStatus.Code
                                }).OrderBy(m => m.EmployeeFullName).Distinct().ToList();            

            return employeeList.Distinct().ToList();
        }
            

        public List<Positions> GetManagerPositionByEmployeeId(int Employeeid, string useridentityname)
        {
            var aspNetUsersEmail = _context.AspNetUsers.Where(x => x.UserName == useridentityname).Select(x => x.Email).FirstOrDefault();
            var ReportsToID = _context.Persons.Where(x => x.eMail == aspNetUsersEmail).Select(x => x.PersonId).FirstOrDefault();
            if (ReportsToID == 0)
            {
                ReportsToID = 0;
            }
            var PositionList = (from x in _context.Employees
                                join EP in _context.E_Positions on x.EmployeeId equals EP.EmployeeId
                                join po in _context.Positions on EP.PositionId equals po.PositionId
                                where (EP.EmployeeId == Employeeid && EP.ReportsToID == ReportsToID)
                                select new Positions
                                {
                                    PositionId = EP.E_PositionId,
                                    PositionCode = po.PositionDescription,
                                }).OrderBy(m => m.PositionCode).ToList();
            return PositionList.Distinct().ToList();

        }


        /// <summary>
        /// Student Login: #1915: Returns the only Student/Employee
        /// </summary>
        /// <param name="DepartmentIdDdl"></param>
        /// <param name="useridentityname"></param>
        /// <returns></returns>
        public List<EmployeesVM> GetStudent(short? DepartmentIdDdl, string useridentityname)
        {
            //2857-Filter Employee by CompanyId
            var companycodeId = _context.Departments.Where(x => x.DepartmentId == (int)DepartmentIdDdl).Select(d => d.CompanyCodeId).FirstOrDefault();

            var employeeID = (from e in _context.Employees
                              where e.CompanyCodeId == companycodeId &&
                                    e.PersonId == (from p in _context.Persons where p.eMail.Equals(useridentityname) select p.PersonId).FirstOrDefault()
                              select e.EmployeeId
                         ).FirstOrDefault();

            //var aspNetUsersEmail = _context.AspNetUsers.Where(x => x.UserName == useridentityname).Select(x => x.Email).FirstOrDefault();
            //var personId = _context.Persons.Where(x => x.eMail == aspNetUsersEmail).Select(x => x.PersonId).FirstOrDefault();

            var employeeList = (from x in _context.Employees
                                join per in _context.Persons on x.PersonId equals per.PersonId
                                join ep in _context.E_Positions on x.EmployeeId equals ep.EmployeeId
                                where (ep.DepartmentId == DepartmentIdDdl 
                                            //&& ep.ReportsToID == personId 
                                            && x.EmployeeId == employeeID
                                            && ep.IsDeleted == false
                                            && x.CompanyCodeId == companycodeId) //2857-Filter Employee by CompanyId
                                select new EmployeesVM
                                {
                                    //deptid = dp.DepartmentId,
                                    EmployeeId = x.EmployeeId,
                                    EmployeeFullName = per.Firstname + " " + per.Lastname,
                                    EmployeeRole = "Permanent",
                                    FileNumber = x.FileNumber
                                }).OrderBy(m => m.EmployeeFullName).Distinct().ToList();

            return employeeList;
        }
        public List<EmployeesVM> GetEmployeesList()
        {
            var employeesList = _context.Employees
                        .Include("Person")
                        .Include("DdlEmploymentStatuses")
                        .GroupBy(x => x.PersonId)
                        .Select(m => m.OrderByDescending(x => x.EmploymentNumber).FirstOrDefault())
                        .Where(x => x.DdlEmploymentStatus.Code == "A")
                        .Select(m => new EmployeesVM
                        {
                            EmployeeId = m.EmployeeId,
                            EmployeeFullName = m.Person.Firstname + " " + m.Person.Lastname,
                            PersonName = m.Person.Firstname + " " + m.Person.Lastname
                        })
                        .OrderBy(s => s.PersonName).ToList();
            return employeesList;
        }


        /// <summary>
        /// Returns PayPeriod List
        /// </summary>
        /// <param name="CompanyCodeIdDdl"></param>
        /// <returns></returns>
        public List<PayPeriodVM> GetPayPeriodsList(int? EmployeeIdDdl, bool IsArchived)
        {
            var payGroupId = _context.E_Positions.Where(x => x.EmployeeId == EmployeeIdDdl && x.IsDeleted == false).Select(X => X.PayGroupId).First();

            List<PayPeriodVM> payPeriodsList = new List<PayPeriodVM>();
            int? payfrequencyid = _context.Employees.Where(x => x.EmployeeId == EmployeeIdDdl).Select(x => x.PayFrequencyId).FirstOrDefault();
            if (payfrequencyid != null)
            {
                payPeriodsList = _context.PayPeriods
                 // .Where(m => m.CompanyCodeId == CompanyCodeIdDdl && m.IsArchived == IsArchived)
                 .Where(m => m.PayFrequencyId == payfrequencyid && m.IsArchived == IsArchived
                                                                && m.StartDate <= System.DateTime.UtcNow
                                                                && m.IsPayPeriodActive == true //SHOULD IMPORT 1 TO THIS COLUMN DURING MAIN EMPLOYEE IMPORT
                                                                && m.PayGroupCode == payGroupId
                                                                && m.IsDeleted == false
                                                                )
                  .Select(m => new PayPeriodVM
                  {
                      PayPeriodId = m.PayPeriodId,
                      EndDate = m.EndDate,
                      PayGroupCode = m.PayGroupCode,
                      PayPeriod = SqlFunctions.StringConvert((double)SqlFunctions.DatePart("mm", m.StartDate)).Trim() + "/" + SqlFunctions.DateName("DAY", m.StartDate) + "/" + SqlFunctions.DateName("YYYY", m.StartDate)
                                 + " - " + SqlFunctions.StringConvert((double)SqlFunctions.DatePart("mm", m.EndDate)).Trim() + "/" + SqlFunctions.DateName("DAY", m.EndDate) + "/" + SqlFunctions.DateName("YYYY", m.EndDate)
                  }).OrderByDescending(m => m.EndDate).ToList();//.Take(6).ToList();
            }
            return payPeriodsList;
        }

        //Returns the Employee Current Pay Period only. Most Recente Date Pay Period. Should be used in MobileController.
        public PayPeriodVM GetEmployeeCurrentPayPeriod(int? EmployeeIdDdl, bool IsArchived)
        {
            var payGroupId = _context.E_Positions.Where(x => x.EmployeeId == EmployeeIdDdl && x.IsDeleted == false).Select(X => X.PayGroupId).First();
            PayPeriodVM payPeriodVM = new PayPeriodVM();
            int? payfrequencyid = _context.Employees.Where(x => x.EmployeeId == EmployeeIdDdl).Select(x => x.PayFrequencyId).FirstOrDefault();
            if (payfrequencyid != null)
            {
                payPeriodVM = _context.PayPeriods
                 .Where(m => m.PayFrequencyId == payfrequencyid && m.IsArchived == IsArchived && m.IsPayPeriodActive == true && m.IsDeleted == false
                 && (
                 (DbFunctions.TruncateTime(m.StartDate) <= DbFunctions.TruncateTime(DateTime.UtcNow))
                 &&
                 (DbFunctions.TruncateTime(m.EndDate) >= DbFunctions.TruncateTime(DateTime.UtcNow))
                 ) && m.PayGroupCode == payGroupId)
                  .Select(m => new PayPeriodVM
                  {
                      PayPeriodId = m.PayPeriodId,
                      StartDate = m.StartDate,
                      EndDate = m.EndDate,
                      PayGroupCode = m.PayGroupCode,
                      LockoutEmployees = m.LockoutEmployees,
                      PayPeriod = SqlFunctions.StringConvert((double)SqlFunctions.DatePart("mm", m.StartDate)).Trim() + "/" + SqlFunctions.DateName("DAY", m.StartDate) + "/" + SqlFunctions.DateName("YYYY", m.StartDate)
                                 + " - " + SqlFunctions.StringConvert((double)SqlFunctions.DatePart("mm", m.EndDate)).Trim() + "/" + SqlFunctions.DateName("DAY", m.EndDate) + "/" + SqlFunctions.DateName("YYYY", m.EndDate)
                  })
                 .OrderByDescending(m => m.StartDate).FirstOrDefault();
            }

            return payPeriodVM;
        }

        public List<PayPeriodVM> GetPayPeriodsList(int? CompanyCodeIdDdl)
        {
            var payPeriodsList = _context.PayPeriods
               .Where(m => m.CompanyCodeId == CompanyCodeIdDdl && m.StartDate <= System.DateTime.UtcNow && m.IsArchived == false && m.PayGroupCode==1 && m.IsDeleted == false)
               .Select(m => new PayPeriodVM
               {
                   PayPeriodId = m.PayPeriodId,
                   EndDate = m.EndDate,
                   PayPeriod = SqlFunctions.StringConvert((double)SqlFunctions.DatePart("mm", m.StartDate)).Trim() + "/" + SqlFunctions.DateName("DAY", m.StartDate) + "/" + SqlFunctions.DateName("YYYY", m.StartDate)
                              + " - " + SqlFunctions.StringConvert((double)SqlFunctions.DatePart("mm", m.EndDate)).Trim() + "/" + SqlFunctions.DateName("DAY", m.EndDate) + "/" + SqlFunctions.DateName("YYYY", m.EndDate)
               }).OrderByDescending(m => m.EndDate).ToList();//.Take(6).ToList();

            return payPeriodsList;
        }

        //#2959: Loads payperiods regardless of company       
        public List<PayPeriodVM> GetGlobalPayPeriodsList()
        {
            var payPeriodsList = _context.PayPeriods
               .Where(m => m.StartDate <= System.DateTime.UtcNow && m.IsArchived == false && m.PayGroupCode == 1 && m.IsDeleted == false)
               .Select(m => new PayPeriodVM
               {
                   PayPeriodId = m.PayPeriodId,
                   EndDate = m.EndDate,
                   PayPeriod = SqlFunctions.StringConvert((double)SqlFunctions.DatePart("mm", m.StartDate)).Trim() + "/" + SqlFunctions.DateName("DAY", m.StartDate) + "/" + SqlFunctions.DateName("YYYY", m.StartDate)
                              + " - " + SqlFunctions.StringConvert((double)SqlFunctions.DatePart("mm", m.EndDate)).Trim() + "/" + SqlFunctions.DateName("DAY", m.EndDate) + "/" + SqlFunctions.DateName("YYYY", m.EndDate)
               }).Distinct().OrderByDescending(m => m.EndDate).ToList();

            return payPeriodsList;
        }

        /// <summary>
        /// Returns Time Card Configuration settings
        /// </summary>        
        public TimeCardDisplayColumn TimeCardInOutDisplayColumns(short typeId)
        {
            TimeCardDisplayColumn timeCardDislayColumns = new TimeCardDisplayColumn();
            timeCardDislayColumns = _context.TimeCardDisplayColumns
            .Where(x => x.TimeCardTypeId == typeId).FirstOrDefault();
            return (timeCardDislayColumns);
        }

        #endregion

        #region Common Approvals Methods for both Time card and Time Card In and Out

        public bool GetTimeCard_Approved(TimeCardVm timeCardVm, string userName, string roleName)
        {

            TimeCardApproval timeCardApprovalRecordInDb = new TimeCardApproval();
            bool result = false;
            List<TimeCard> empTimeCardList = new List<TimeCard>();
            var timeCardApproval = _context.TimeCardApprovals
                              .Where(x => x.EmployeeId == timeCardVm.EmployeeId && x.PayPeriodId == timeCardVm.PayPeriodId).ToList();
            var payPeriod = _context.PayPeriods.Where(x => x.PayPeriodId == timeCardVm.PayPeriodId).FirstOrDefault();
            if (roleName == "ClientManagers")
            {
                var reportstoid = _context.Persons.Where(x => x.eMail == userName).Select(x => x.PersonId).FirstOrDefault();
                timeCardApprovalRecordInDb = timeCardApproval.Where(x => x.ManagerId == userName).SingleOrDefault();


                //var ePos = (from epos in _context.E_Positions                      
                //        where epos.ReportsToID == reportstoid
                //        select epos).ToList();

                //empTimeCardList = _context.TimeCards.Where(x => x.EmployeeId == timeCardVm.EmployeeId && x.DepartmentId == timeCardVm.DepartmentId && (x.ActualDate >= payPeriod.StartDate && x.ActualDate <= payPeriod.EndDate)).ToList();

                //empTimeCardList = (from tc in empTimeCardList
                //                   join ep in ePos on tc.PositionId equals ep.PositionId
                //                   select tc).ToList();

                empTimeCardList = (from t in _context.TimeCards
                                   join ep in _context.E_Positions on t.PositionId equals ep.PositionId
                                   where t.EmployeeId == timeCardVm.EmployeeId &&
                                   (t.ActualDate >= payPeriod.StartDate && t.ActualDate <= payPeriod.EndDate) &&
                                   ep.ReportsToID == reportstoid && ep.EmployeeId == timeCardVm.EmployeeId && t.IsDeleted == false && ep.IsDeleted == false
                                   select t
                                 ).Distinct().ToList();

                foreach (var item in empTimeCardList)
                {
                    item.IsApproved = timeCardVm.Approved;
                    //item.ApprovedBy = userName;
                    item.DisApprovedBy = userName;
                    item.UserId = userName;
                    item.LastModifiedDate = DateTime.Now;
                    item.DepartmentId = timeCardVm.DepartmentId;
                    _context.TimeCards.Attach(item);
                    _context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }
                _context.SaveChanges();

                if (timeCardApprovalRecordInDb != null)
                {
                    timeCardApprovalRecordInDb.Approved = timeCardVm.Approved;
                    timeCardApprovalRecordInDb.ManagerId = userName;
                    _context.TimeCardApprovals.Attach(timeCardApprovalRecordInDb);
                    _context.Entry(timeCardApprovalRecordInDb).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    TimeCardApproval newtimeCardApproval = new TimeCardApproval
                    {
                        EmployeeId = timeCardVm.EmployeeId,
                        PayPeriodId = timeCardVm.PayPeriodId.Value,
                        Approved = timeCardVm.Approved,
                        ManagerId = userName
                    };
                    _context.TimeCardApprovals.Add(newtimeCardApproval);
                }
            }
            else
            {
                empTimeCardList = _context.TimeCards.Where(x => x.EmployeeId == timeCardVm.EmployeeId && x.IsDeleted == false && (x.ActualDate >= payPeriod.StartDate && x.ActualDate <= payPeriod.EndDate)).ToList();

                foreach (var item in empTimeCardList)
                {
                    item.IsApproved = timeCardVm.Approved;
                    //item.ApprovedBy = userName;
                    item.DisApprovedBy = userName;
                    item.DepartmentId = timeCardVm.DepartmentId;
                    item.UserId = userName;
                    item.LastModifiedDate = DateTime.Now;
                    _context.TimeCards.Attach(item);
                    _context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }
                _context.SaveChanges();

                var reportToList = (from ep in _context.E_Positions
                                    join p in _context.Persons on ep.ReportsToID equals p.PersonId
                                    where ep.EmployeeId == timeCardVm.EmployeeId && ep.IsDeleted == false
                                    select new { p.eMail, ep.PositionId }).Distinct().ToList();

                reportToList.Insert(0, new { eMail = userName, PositionId = 0 });
                string userId = userName;
                foreach (var item in reportToList)
                {
                    if (item.PositionId != 0)
                    {
                        int count = empTimeCardList.Where(x => x.PositionId == item.PositionId).Count();
                        if (item.eMail == userName) { continue; }
                        if (count == 0) { continue; }
                        userName = item.eMail;
                    }

                    timeCardApprovalRecordInDb = timeCardApproval.Where(x => x.ManagerId == userName).SingleOrDefault();

                    if (timeCardApprovalRecordInDb != null)
                    {
                        timeCardApprovalRecordInDb.Approved = timeCardVm.Approved;
                        timeCardApprovalRecordInDb.ManagerId = userName;
                        _context.TimeCardApprovals.Attach(timeCardApprovalRecordInDb);
                        _context.Entry(timeCardApprovalRecordInDb).State = System.Data.Entity.EntityState.Modified;
                    }
                    else
                    {
                        TimeCardApproval newtimeCardApproval = new TimeCardApproval
                        {
                            EmployeeId = timeCardVm.EmployeeId,
                            PayPeriodId = timeCardVm.PayPeriodId.Value,
                            Approved = timeCardVm.Approved,
                            ManagerId = userName
                        };
                        _context.TimeCardApprovals.Add(newtimeCardApproval);
                    }
                }
            }
            result = _context.SaveChanges() > 0 ? true : false;
            return result;
        }

        //Approves All Time Card based on Weekly Session Time
        public int Getsessionvalue()
        {
            int maxhours = 0;
            var sessionList = _context.TimeCardSessionInOutConfigs.ToList();
            foreach (var item in sessionList)
            {
                if (item.Toggle == true)
                {
                    maxhours = item.MaxHours.Value;
                }
            }
            return maxhours;
        }
        public bool TimeCardApproveAll(TimeCardVm timeCardVm, string userName, string roleName)
        {
            TimeCardApproval timeCardApprovalRecordInDb = new TimeCardApproval();
            bool result = false;
            List<TimeCard> empTimeCardList = new List<TimeCard>();
            var timeCardApproval = _context.TimeCardApprovals
                              .Where(x => x.EmployeeId == timeCardVm.EmployeeId && x.PayPeriodId == timeCardVm.PayPeriodId).ToList();
            var payPeriod = _context.PayPeriods.Where(x => x.PayPeriodId == timeCardVm.PayPeriodId).FirstOrDefault();

            //Block Begin: RETURNS WEEKLY SUM FROM TIMECARDS
            int sessionMaxHours = Getsessionvalue();
            DateTime startdate = payPeriod.StartDate;
            DateTime WeekOneEndDate = payPeriod.StartDate.AddDays(6);
            DateTime WeekTwoStartDate = WeekOneEndDate.AddDays(1);
            DateTime enddate = payPeriod.EndDate;
            bool IsArchived = false;
            double weekOneTotalHours = 0;
            double weekTwoTotalHours = 0;
            List<TimeCardCollectionVm> TimeCardListByPayPeriod = new List<TimeCardCollectionVm>();
            TimeCardListByPayPeriod = Query<TimeCardCollectionVm>("sp_GetTimeCardsList", new { @empId = timeCardVm.EmployeeId, @isArchived = IsArchived, @PayPeriodId = timeCardVm.PayPeriodId }).ToList();
            if (TimeCardListByPayPeriod.Count != 0)
            {
                foreach (var item in TimeCardListByPayPeriod)
                {
                    int id = _context.TimeCardsNotes.Where(m => m.TimeCardId == item.TimeCardId).Select(x => x.TimeCardsNotesId).FirstOrDefault();
                    item.NotesId = id != 0 ? id : 0;

                    var sumHours = _context.TimeCards.Where(r => r.ActualDate == item.ActualDate && r.EmployeeId == item.EmployeeId).Sum(r => r.DailyHours);
                    item.LineTotal = sumHours == 0 ? null : sumHours;
                }
            }
            //Block End: RETURNS WEEKLY SUM FROM TIMECARDS

            if (roleName == "ClientManagers")
            {
                var reportstoid = _context.Persons.Where(x => x.eMail == userName).Select(x => x.PersonId).FirstOrDefault();
                timeCardApprovalRecordInDb = timeCardApproval.Where(x => x.ManagerId == userName).SingleOrDefault();            

                empTimeCardList = (from t in _context.TimeCards
                                   join ep in _context.E_Positions on t.PositionId equals ep.PositionId
                                   where t.EmployeeId == timeCardVm.EmployeeId &&
                                   (t.ActualDate >= payPeriod.StartDate && t.ActualDate <= payPeriod.EndDate) &&
                                   ep.ReportsToID == reportstoid && ep.EmployeeId == timeCardVm.EmployeeId && t.IsDeleted == false && ep.IsDeleted == false
                                   select t
                                 ).Distinct().ToList();

                //Block Begin: Weekly Approved Sum to compare with Session Maximum Time
                List<TimeCardCollectionVm> weekoneempTimeCardList = new List<TimeCardCollectionVm>();
                weekoneempTimeCardList = TimeCardListByPayPeriod.Where(x => x.ActualDate >= startdate && x.ActualDate <= WeekOneEndDate
                                                                                                         && x.IsLineApproved == true).ToList();
                List<TimeCardCollectionVm> weektwoempTimeCardList = new List<TimeCardCollectionVm>();
                weektwoempTimeCardList = TimeCardListByPayPeriod.Where(x => x.ActualDate >= WeekTwoStartDate && x.ActualDate <= enddate
                                                                                                                 && x.IsLineApproved == true).ToList();
                weekOneTotalHours = (double)weekoneempTimeCardList.Sum(x => x.DailyHours + (x.Hours ?? 0));
                weekTwoTotalHours = (double)weektwoempTimeCardList.Sum(x => x.DailyHours + (x.Hours ?? 0));

                List<TimeCard> empWeekOneTimeCardList = new List<TimeCard>();
                List<TimeCard> empWeekTwoTimeCardList = new List<TimeCard>();
                empWeekOneTimeCardList = empTimeCardList.Where(x => x.ActualDate >= startdate && x.ActualDate <= WeekOneEndDate).ToList();
                empWeekTwoTimeCardList = empTimeCardList.Where(x => x.ActualDate >= WeekTwoStartDate && x.ActualDate <= enddate).ToList();
                //Block End: Block Begin: Weekly Approved Sum to compare with Session Maximum Time

                foreach (var item in empWeekOneTimeCardList)//empTimeCardList)
                {
                    if (item.IsApproved == false)
                    {
                        weekOneTotalHours += (double)item.DailyHours + (double)(item.Hours ?? 0);
                        item.IsApproved = true;//timeCardVm.Approved;
                    }
                    if (timeCardVm.Approved == false)
                        item.IsApproved = false;

                    item.ApprovedBy = userName;
                    item.DisApprovedBy = null;
                    item.UserId = userName;
                    item.LastModifiedDate = DateTime.Now;
                    item.DepartmentId = timeCardVm.DepartmentId;
                    _context.TimeCards.Attach(item);
                    _context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }
                //if (weekOneTotalHours > sessionMaxHours)
                //    return result;

                foreach (var item in empWeekTwoTimeCardList) //SECOND WEEK BLOCK
                {
                    if (item.IsApproved == false)
                    {
                        weekTwoTotalHours += (double)item.DailyHours + (double)(item.Hours ?? 0);
                        item.IsApproved = true;//timeCardVm.Approved;
                    }
                    if (timeCardVm.Approved == false)
                        item.IsApproved = false;

                    item.ApprovedBy = userName;
                    item.UserId = userName;
                    item.DisApprovedBy = null;
                    item.LastModifiedDate = DateTime.Now;
                    item.DepartmentId = timeCardVm.DepartmentId;
                    _context.TimeCards.Attach(item);
                    _context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }
                //if (weekTwoTotalHours > sessionMaxHours)
                //    return result;

                _context.SaveChanges();

                //Save changes in TimeCardApprovals Table
                if (timeCardApprovalRecordInDb != null)
                {
                    timeCardApprovalRecordInDb.Approved = timeCardVm.Approved;
                    timeCardApprovalRecordInDb.ManagerId = userName;
                    _context.TimeCardApprovals.Attach(timeCardApprovalRecordInDb);
                    _context.Entry(timeCardApprovalRecordInDb).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    TimeCardApproval newtimeCardApproval = new TimeCardApproval
                    {
                        EmployeeId = timeCardVm.EmployeeId,
                        PayPeriodId = timeCardVm.PayPeriodId.Value,
                        Approved = timeCardVm.Approved,
                        ManagerId = userName
                    };
                    _context.TimeCardApprovals.Add(newtimeCardApproval);
                }
            }
            else
            {
                empTimeCardList = _context.TimeCards.Where(x => x.EmployeeId == timeCardVm.EmployeeId && x.IsDeleted == false && (x.ActualDate >= payPeriod.StartDate && x.ActualDate <= payPeriod.EndDate)).ToList();
                //Weeekly Session
                List<TimeCard> empWeekOneTimeCardList = new List<TimeCard>();
                List<TimeCard> empWeekTwoTimeCardList = new List<TimeCard>();
                empWeekOneTimeCardList = empTimeCardList.Where(x => x.ActualDate >= startdate && x.ActualDate <= WeekOneEndDate).ToList();
                //&& x.IsApproved == true).ToList();
                empWeekTwoTimeCardList = empTimeCardList.Where(x => x.ActualDate >= WeekTwoStartDate && x.ActualDate <= enddate).ToList();
                //&& x.IsApproved == true).ToList();
                weekOneTotalHours = (double)empWeekOneTimeCardList.Sum(x => x.DailyHours + (x.Hours ?? 0));
                weekTwoTotalHours = (double)empWeekTwoTimeCardList.Sum(x => x.DailyHours + (x.Hours ?? 0));
                //if (weekOneTotalHours > sessionMaxHours || weekTwoTotalHours > sessionMaxHours)
                //    return result;
                //End Block: Weekly Sesstion.

                foreach (var item in empWeekOneTimeCardList)//empTimeCardList)
                {
                    item.IsApproved = timeCardVm.Approved;
                    item.ApprovedBy = userName;
                    item.DisApprovedBy = null;
                    item.DepartmentId = timeCardVm.DepartmentId;
                    item.UserId = userName;
                    item.LastModifiedDate = DateTime.Now;
                    _context.TimeCards.Attach(item);
                    _context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }
                foreach (var item in empWeekTwoTimeCardList)
                {
                    item.IsApproved = timeCardVm.Approved;
                    item.ApprovedBy = userName;
                    item.DisApprovedBy = null;
                    item.DepartmentId = timeCardVm.DepartmentId;
                    item.UserId = userName;
                    item.LastModifiedDate = DateTime.Now;
                    _context.TimeCards.Attach(item);
                    _context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }
                _context.SaveChanges();

                //Save chahges in TimeCardApprovals table for all reporting Managers if Admin is approved
                var reportToList = (from ep in _context.E_Positions
                                    join p in _context.Persons on ep.ReportsToID equals p.PersonId
                                    where ep.EmployeeId == timeCardVm.EmployeeId && ep.IsDeleted == false
                                    select new { p.eMail, ep.PositionId }).Distinct().ToList();

                reportToList.Insert(0, new { eMail = userName, PositionId = 0 });
                string userId = userName;
                foreach (var item in reportToList)
                {
                    if (item.PositionId != 0)
                    {
                        int count = empTimeCardList.Where(x => x.PositionId == item.PositionId).Count();
                        if (item.eMail == userName) { continue; }
                        if (count == 0) { continue; }
                        userName = item.eMail;
                    }

                    timeCardApprovalRecordInDb = timeCardApproval.Where(x => x.ManagerId == userName).SingleOrDefault();

                    if (timeCardApprovalRecordInDb != null)
                    {
                        timeCardApprovalRecordInDb.Approved = timeCardVm.Approved;
                        timeCardApprovalRecordInDb.ManagerId = userName;
                        _context.TimeCardApprovals.Attach(timeCardApprovalRecordInDb);
                        _context.Entry(timeCardApprovalRecordInDb).State = System.Data.Entity.EntityState.Modified;
                    }
                    else
                    {
                        TimeCardApproval newtimeCardApproval = new TimeCardApproval
                        {
                            EmployeeId = timeCardVm.EmployeeId,
                            PayPeriodId = timeCardVm.PayPeriodId.Value,
                            Approved = timeCardVm.Approved,
                            ManagerId = userName
                        };
                        _context.TimeCardApprovals.Add(newtimeCardApproval);
                    }
                }
            }
            result = _context.SaveChanges() > 0 ? true : false;
            return result;
        }
        //Checks the Status whether the Administrator has approved the Pay Period.
        public bool getTimeCardApproval(int EmployeeId, int PayPeriodId, string roleName, string userName) //, bool isAdminApproveds
        {
            bool timeCardStatus = false;
            TimeCardApproval timeCardApprovalRecordInDb = new TimeCardApproval();
            if (roleName == "ClientManagers")
            {
                //var approvedBy = _context.TimeCards.Where(r => r.UserId == userName).Select(r => r.ApprovedBy).FirstOrDefault();
                timeCardApprovalRecordInDb = _context.TimeCardApprovals
                             .Where(x => x.EmployeeId == EmployeeId && x.PayPeriodId == PayPeriodId && x.ManagerId == userName).SingleOrDefault();
                if (timeCardApprovalRecordInDb != null)
                {
                    timeCardStatus = timeCardApprovalRecordInDb.Approved;
                }
            }
            else
            {
                var payPeriod = _context.PayPeriods.Where(x => x.PayPeriodId == PayPeriodId).FirstOrDefault();
                List<TimeCard> isTiCardRecordsExists = _context.TimeCards.Where(x => x.EmployeeId == EmployeeId && x.IsDeleted == false && (x.ActualDate >= payPeriod.StartDate && x.ActualDate <= payPeriod.EndDate)).ToList();
                if (isTiCardRecordsExists.Count > 0)
                {
                    List<TimeCard> empTimeCardList = _context.TimeCards.Where(x => x.EmployeeId == EmployeeId && x.IsDeleted == false && (x.ActualDate >= payPeriod.StartDate && x.ActualDate <= payPeriod.EndDate && x.IsApproved == false)).ToList();
                    if (empTimeCardList.Count == 0)
                    {
                        //timeCardStatus = false;
                        timeCardStatus = true;
                    }
                }
                else
                {
                    timeCardStatus = false;
                }
            }
            return timeCardStatus;
        }

        public string GetPersonName(int employeeId)
        {
            var personName = _context.Employees.Include("Persons").Where(x => x.EmployeeId == employeeId).Select(x => x.Person.Firstname + " " + x.Person.Lastname).Single();
            return personName;
        }

        public string GetEmpFilenumber(int empid)
        {
            return _context.Employees.Where(x => x.EmployeeId == empid).Select(x => x.FileNumber).Single();
        }

        #endregion

        #region Grid Editor Temaplates Dropdown

        public List<EmployeesVM> GetEmployeeDropdownList()
        {
            var employeeList = (_context.Employees
               .Include("DdlEmploymentStatuses")
               .GroupBy(x => x.PersonId)
               .Select(m => m.OrderByDescending(x => x.EmploymentNumber).FirstOrDefault())
               .Where(x => x.DdlEmploymentStatus.Code == "A")
               .Select(m => new EmployeesVM
               {
                   EmployeeId = m.EmployeeId,
                   EmployeeFullName = m.Person.Firstname + " " + m.Person.Lastname
               })).OrderBy(m => m.EmployeeFullName).ToList();
            return employeeList;
        }


        /// <summary>
        /// Returns Hours Code list 
        /// </summary>
        /// 
        public List<HoursCodeVm> GetHourCodes()
        {
            List<HoursCodeVm> hourCodesList = _context.HoursCodes
                    .Select(s => new HoursCodeVm
                    {
                        HoursCodeId = s.HoursCodeId,
                        HoursCodeCode = s.HoursCodeCode + "-" + s.HoursCodeDescription
                    })
                    .OrderBy(s => s.HoursCodeCode).ToList();
            return hourCodesList;
        }

        public List<ADPFieldMappingVM> GetADPFieldMappings()
        {
            List<ADPFieldMappingVM> getADPFieldMappingList = new List<ADPFieldMappingVM>();
            try
            {
                getADPFieldMappingList = _context.ADPFieldMappings
                    //.Include(x => x.ADPFieldMappingCode)
                    .Select(c => new ADPFieldMappingVM
                    {
                        ADPFieldMappingId = c.ADPFieldMappingId,
                        ADPFieldMappingCode = "Hours " + c.ADPFieldMappingCode //+ "-" + e.SkillId.ToString()
                    }).Distinct().OrderBy(s => s.ADPFieldMappingCode).ToList();
                return getADPFieldMappingList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ADPAccNumberVM> GetADPAccNumbers()
        {
            List<ADPAccNumberVM> adpAccNumbersList = new List<ADPAccNumberVM>();
            try
            {
                adpAccNumbersList = _context.ADPAccNumbers
                    //.Include(x => x.ADPAccNumberId)
                    .Select(c => new ADPAccNumberVM
                    {
                        ADPAccNumberId = c.ADPAccNumberId,
                        ADPAccNumberDesc = c.ADPAccNumberId.ToString() //+ "-" + e.SkillId.ToString()
                    }).Distinct().OrderBy(s => s.ADPAccNumberId).ToList();
                adpAccNumbersList.Insert(0, new ADPAccNumberVM { ADPAccNumberId = 0, ADPAccNumberDesc = "None" });
                return adpAccNumbersList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Returns Earning codes list 
        /// </summary>
        public List<EarningCodeVm> GetEarningCodes()
        {
            List<EarningCodeVm> earningCodesList = _context.EarningsCodes
                       .Select(s => new EarningCodeVm
                       {
                           EarningsCodeId = s.EarningsCodeId,
                           EarningsCodeCode = s.EarningsCodeCode + "-" + s.EarningsCodeDescription
                       })
                      .OrderBy(s => s.EarningsCodeCode).ToList();
            return earningCodesList;
        }

        /// <summary>
        /// Returns the Temp Dept Codes List
        /// </summary>
        public List<DepartmentVm> GetTempDepartmentCodes()
        {
            List<DepartmentVm> tempDepartmentCodesList = _context.Departments.Where(x=> x.IsDeleted == false)
                     .Select(s => new DepartmentVm
                     {
                         TempDeptId = s.DepartmentId,
                         TempDeptCode = s.DepartmentCode + "-" + s.DepartmentDescription
                     })
                     .OrderBy(s => s.TempDeptCode).ToList();
            return tempDepartmentCodesList;
        }

        /// <summary>
        ///  Returns the Temp Job Codes List 
        /// </summary>
        public List<JobsVM> GetTempJobCodes()
        {
            List<JobsVM> tempJobCodesList = _context.Jobs
                        .Select(s => new JobsVM
                        {
                            TempJobId = s.JobId,
                            TempJobCode = s.JobCode + "-" + s.JobDescription
                        })
                        .OrderBy(s => s.TempJobCode).ToList();
            return tempJobCodesList;
        }

        public List<PositionsVM> GetEmployeePositionList()
        {
            var lstPositions = _context.E_Positions.Where(ep=>ep.IsDeleted == false)
                             .Select(p => new PositionsVM
                             {
                                 //PositionId = x.PositionId,
                                 //PositionCode = x.PositionCode + "-" + x.PositionDescription,
                                 PositionId = p.PositionId,
                                 PositionCode = p.Position.PositionDescription+"-"+p.Position.Suffix, //p.Position.PositionCode + "-" + p.Position.PositionDescription,
                                 IsPositionActive = p.Position.IsPositionActive
                             }).ToList();
            return lstPositions;
        }

        public List<HoursCodeVm> ValidateHoursCodes(int? CompanyCodeIdDdl)
        {
            List<HoursCodeVm> hourscodelist = new List<HoursCodeVm>();
            hourscodelist = _context.HoursCodes
                       .Where(x => x.CompanyCodeId == CompanyCodeIdDdl && x.IsRetro == false && x.StartDate == null)
                       .Select(s => new HoursCodeVm
                       {
                           HoursCodeId = s.HoursCodeId,
                           HoursCodeCode = s.HoursCodeDescription //s.HoursCodeCode + "-" + s.HoursCodeDescription
                       })
                       .OrderBy(s => s.HoursCodeCode).ToList();
            return hourscodelist;
        }

        public List<EarningCodeVm> ValidEarningCodes(int? CompanyCodeIdDdl)
        {
            List<EarningCodeVm> earningcodelist = new List<EarningCodeVm>();
            earningcodelist = _context.EarningsCodes
                       .Where(x => x.CompanyCodeId == CompanyCodeIdDdl)
                       .Select(s => new EarningCodeVm
                       {
                           EarningsCodeId = s.EarningsCodeId,
                           //EarningsCode = s.EarningsCodeCode + "-" + s.EarningsCodeDescription
                           /*
                            In EarningCodeVm model created EarningsCodeCode property. The property should be same as Entity.
                            Check the EarningCode Main Form.  You have done it with the property EarningCode instead of EarningsCodeCode
                            */
                           EarningsCodeCode = s.EarningsCodeDescription//s.EarningsCodeCode + "-" + s.EarningsCodeDescription
                       })
                       .OrderBy(s => s.EarningsCodeCode).ToList();
            return earningcodelist;
        }

        public List<DepartmentVm> ValidTempDeptCodes(int? CompanyCodeIdDdl)
        {
            List<DepartmentVm> departmentlist = new List<DepartmentVm>();

            departmentlist = _context.Departments
                       .Where(x => x.CompanyCodeId == CompanyCodeIdDdl && x.IsDeleted == false)
                       .Select(s => new DepartmentVm()
                       {
                           TempDeptId = s.DepartmentId,
                           TempDeptCode = s.DepartmentDescription//s.DepartmentCode + " - " + s.DepartmentDescription
                       })
                       .OrderBy(s => s.TempDeptCode).ToList();
            return departmentlist;
        }

        public List<JobsVM> ValidateTempJobCodes(int? CompanyCodeIdDdl)
        {
            List<JobsVM> tempJobCodesList = new List<JobsVM>();
            tempJobCodesList = _context.Jobs
                   .Where(x => x.CompanyCodeId == CompanyCodeIdDdl)
                   .Select(s => new JobsVM
                   {
                       TempJobId = s.JobId,
                       TempJobCode = s.JobDescription//s.JobCode + " - " + s.JobDescription
                   })
                   .OrderBy(s => s.TempJobCode).ToList();
            return tempJobCodesList;
        }

        //public List<PositionsVM> ValidateEmployeePositions(int? EmployeeId)
        //{          
        //     List<PositionsVM> employeePositions = new List<PositionsVM>();            
        //    employeePositions = (_context.E_Positions
        //       .Include("Position")
        //       .Include("Person")
        //       .Where(x => x.Position.PositionId == x.PositionId && x.EmployeeId == EmployeeId)
        //       .Select(p => new PositionsVM
        //       {
        //           PositionId = p.PositionId,
        //           PositionCode = p.Position.PositionDescription, //p.Position.PositionCode + "-" + p.Position.PositionDescription,
        //           IsPositionActive = p.Position.IsPositionActive
        //       })).OrderBy(m => m.IsPositionActive == true).ToList();
        //    return employeePositions;
        //}

        //Loads Positions Other than Manager
        public List<PositionsVM> ValidateEmployeePositionsByPayPeriod(int? EmployeeId, int payPeriodId)
        {
            //List<E_PositioVm> posList = _context.E_Positions.Where(x => x.EmployeeId == EmployeeId).Select(x => x.StartDate,x=>x.ac).FirstOrDefault()
            List<PositionsVM> employeePositions = new List<PositionsVM>();
            List<PositionsVM> employeePositionsList = new List<PositionsVM>();
            var payPriod = _context.PayPeriods.Where(x => x.PayPeriodId == payPeriodId).FirstOrDefault();

            employeePositionsList = (_context.E_Positions
           .Include("Position")
           .Where(x => x.Position.PositionId == x.PositionId && x.PayGroupId == payPriod.PayGroupCode
                                                             && x.EmployeeId == EmployeeId && x.IsDeleted == false)
           .Select(p => new PositionsVM
           {
               PositionId = p.PositionId,
               PositionCode = p.Position.PositionDescription+"-"+p.Position.Suffix,
               IsPositionActive = p.Position.IsPositionActive,
               StartDate = p.StartDate,
               ActualEndDate = p.actualEndDate
           })).OrderBy(m => m.IsPositionActive == true).ToList();

            if (employeePositionsList.Count > 0)
            {
                foreach (PositionsVM pos in employeePositionsList)
                {
                    bool flag = false;

                    //If position Start Date is future date than pay period it should not load
                    if (pos.StartDate.HasValue && !pos.ActualEndDate.HasValue
                                                && (pos.StartDate <= payPriod.EndDate))

                    {
                        flag = true;
                    }
                    else if (pos.StartDate.HasValue && pos.ActualEndDate.HasValue
                                                   && (pos.ActualEndDate >= payPriod.StartDate && pos.ActualEndDate <= payPriod.EndDate))//&& pos.ActualEndDate >= DateTime.UtcNow)
                    {
                        flag = true;

                    }
                    //else if (pos.StartDate.HasValue && pos.ActualEndDate.HasValue
                    //                                && (pos.ActualEndDate >= payPriod.StartDate && pos.ActualEndDate >= payPriod.EndDate)) // Position End date should fall in all future pp
                    //{
                    //    flag = true;
                    //}
                    // Position End date should fall in all future pp
                    // Position should be only load in those pay period in which position is active
                    else if (pos.StartDate.HasValue && pos.ActualEndDate.HasValue
                                                    && (pos.StartDate <= payPriod.EndDate && pos.ActualEndDate >= payPriod.StartDate 
                                                                                          && pos.ActualEndDate >= payPriod.EndDate))
                    {
                        flag = true;
                    }
                    if (flag)
                        employeePositions.Add(pos);
                }
            }
            return employeePositions;
        }

        //Loads Positions of Current Pay Period by PayGroup as well ReportTo User (Manager Login)
        public List<PositionsVM> ValidateEmployeePositionManager(int? EmployeeId, int payPeriodId, string loggedInUserId)
        {
            var aspNetUsersEmail = _context.AspNetUsers.Where(x => x.UserName == loggedInUserId).Select(x => x.Email).FirstOrDefault();
            var ReportsToID = _context.Persons.Where(x => x.eMail == aspNetUsersEmail).Select(x => x.PersonId).FirstOrDefault();
            var payPriod = _context.PayPeriods.Where(x => x.PayPeriodId == payPeriodId).FirstOrDefault();
            List<PositionsVM> employeePositionsList = new List<PositionsVM>();
            List<PositionsVM> employeePositions = new List<PositionsVM>();
            if (ReportsToID == 0)
            {
                ReportsToID = 0;
            }
            employeePositionsList = (_context.E_Positions
               .Include("Position")
               .Include("Person")
               .Where(x => x.Position.PositionId == x.PositionId && x.PayGroupId == payPriod.PayGroupCode
                   && x.EmployeeId == EmployeeId && x.ReportsToID == ReportsToID && x.IsDeleted == false)
               .Select(p => new PositionsVM
               {
                   PositionId = p.PositionId,
                   PositionCode = p.Position.PositionDescription+"-"+p.Position.Suffix,
                   IsPositionActive = p.Position.IsPositionActive,
                   StartDate = p.StartDate,
                   ActualEndDate = p.actualEndDate
               })).OrderBy(m => m.IsPositionActive == true).ToList();

            if (employeePositionsList.Count > 0)
            {
                foreach (PositionsVM pos in employeePositionsList)
                {
                    bool flag = false;
                    //If position Start Date is future date than pay period it should not load                   
                    if (pos.StartDate.HasValue && !pos.ActualEndDate.HasValue
                                               && (pos.StartDate <= payPriod.EndDate))

                    {
                        flag = true;
                    }
                    else if (pos.StartDate.HasValue && pos.ActualEndDate.HasValue
                                                    && (pos.ActualEndDate >= payPriod.StartDate && pos.ActualEndDate <= payPriod.EndDate))//&& pos.ActualEndDate >= DateTime.UtcNow)
                    {
                        flag = true;

                    }
                    //else if (pos.StartDate.HasValue && pos.ActualEndDate.HasValue
                    //                                && (pos.ActualEndDate >= payPriod.StartDate && pos.ActualEndDate >= payPriod.EndDate)) // Position End date should fall in all future pp
                    //{
                    //    flag = true;
                    //}
                    // Position End date should fall in all future pp
                    // Position should be only load in those pay period in which position is active
                    else if (pos.StartDate.HasValue && pos.ActualEndDate.HasValue
                                                   && (pos.StartDate <= payPriod.EndDate && pos.ActualEndDate >= payPriod.StartDate 
                                                                                         && pos.ActualEndDate >= payPriod.EndDate)) 
                    {
                        flag = true;
                    }
                    if (flag)
                        employeePositions.Add(pos);
                }
            }
            return employeePositions;
        }
        public List<TimeCardCollectionVm> TimeCardApproveAllList(TimeCardVm timeCardVm, string userName, string roleName)
        {
            bool IsArchived = false;
            List<TimeCardCollectionVm> TimeCardListByPayPeriod = new List<TimeCardCollectionVm>();
            TimeCardListByPayPeriod = Query<TimeCardCollectionVm>("sp_GetTimeCardsList", new { @empId = timeCardVm.EmployeeId, @isArchived = IsArchived, @PayPeriodId = timeCardVm.PayPeriodId }).ToList();
            return TimeCardListByPayPeriod;
        }

        public bool IsActivePostionStatusbyManager(int employeeId, int ReportoId, int departmentId)
        {
            if (ReportoId == 0)
            {
                ReportoId = 0;
            }
            List<PositionsVM> ePostionlist = new List<PositionsVM>();
            bool result = false;
            var payPeriod = (from ep in _context.E_Positions
                             join ee in _context.Employees on ep.EmployeeId equals ee.EmployeeId
                             join pf in _context.DdlPayFrequencies on ep.PayFrequencyId equals pf.PayFrequencyId
                             join pp in _context.PayPeriods on pf.PayFrequencyId equals pp.PayFrequencyId
                             where ep.PayGroupId == pp.PayGroupCode && ep.EmployeeId == employeeId && pp.StartDate <= System.DateTime.UtcNow &&
                             pp.IsPayPeriodActive == true && ee.IsStudent==true
                             select pp
                             ).OrderByDescending(x => x.EndDate).FirstOrDefault();
            if (payPeriod != null)
            {
                ePostionlist = _context.E_Positions.Where(x => x.EmployeeId == employeeId && x.ReportsToID == ReportoId && x.DepartmentId == departmentId)
                    .Select(p => new PositionsVM
                    {
                        PositionId = p.PositionId,
                        PositionCode = p.Position.PositionDescription + "-" + p.Position.Suffix,
                        IsPositionActive = p.Position.IsPositionActive,
                        StartDate = p.StartDate,
                        ActualEndDate = p.actualEndDate
                    }).OrderBy(m => m.IsPositionActive == true).ToList();
                if (ePostionlist.Count > 0)
                {
                    foreach (PositionsVM pos in ePostionlist)
                    {
                        if (pos != null)
                        {
                            if (pos.StartDate.HasValue && !pos.ActualEndDate.HasValue
                                                               && (pos.StartDate <= payPeriod.EndDate))

                            {
                                result = true;
                            }
                            else if (pos.StartDate.HasValue && pos.ActualEndDate.HasValue
                                                            && (pos.ActualEndDate >= payPeriod.StartDate && pos.ActualEndDate <= payPeriod.EndDate))//&& pos.ActualEndDate >= DateTime.UtcNow)
                            {
                                result = true;

                            }
                            else if (pos.StartDate.HasValue && pos.ActualEndDate.HasValue
                                                           && (pos.StartDate <= payPeriod.EndDate && pos.ActualEndDate >= payPeriod.StartDate
                                                                                                 && pos.ActualEndDate >= payPeriod.EndDate))
                            {
                                result = true;
                            }
                        }
                    }
                }
            }
                
            return result;
        }

        #endregion

        #endregion
    }
}

