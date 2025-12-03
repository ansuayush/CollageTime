using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Models;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using ExecViewHrk.EfClient;
using System;
using Dapper;
using System.Data;
using System.Text;
using System.Data.SqlClient;

namespace ExecViewHrk.Domain.Repositories
{
    public class PayPeriodRepository : RepositoryBase, IPayPeriodRepository
    {
        string timeZone = System.Configuration.ConfigurationManager.AppSettings["TimeZone"].ToString();

        public List<PayPeriodVM> GetPayPeriodList()
        {
            List<PayPeriodVM> _payperiodList = new List<PayPeriodVM>();

            try
            {
                _payperiodList = Query<PayPeriodVM>("sp_GetPayPeriodList").ToList();
                //_payperiodList = _context.PayPeriods
                //    .Include(x => x.DdlPayFrequency)
                //    .Include(x => x.CompanyCode)                   
                //    .Select(c => new PayPeriodVM
                //    {
                //        ModifiedStartDate = c.StartDate.ToString(),
                //        ModifiedEndDate=c.EndDate.ToString(),
                //        PayPeriodId = c.PayPeriodId,
                //        PayFrequencyId = c.PayFrequencyId,
                //        StartDate = c.StartDate,
                //        EndDate = c.EndDate,
                //        CompanyCodeId = (int)c.CompanyCodeId,
                //        CompanyCode = c.CompanyCode.CompanyCodeCode,
                //        PayFrequencyName = c.DdlPayFrequency.Description,
                //        IsArchived = c.IsArchived,
                //        IsPayPeriodActive = c.IsPayPeriodActive,
                //        LockoutEmployees = c.LockoutEmployees,
                //        LockoutManagers = c.LockoutManagers,
                //        PayGroupCode = c.PayGroupCode,
                //        PayPeriodNumber = c.PayPeriodNumber
                //    })
                //        .Distinct().OrderBy(s => s.PayPeriodId).ToList();
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return _payperiodList;
        }

        public PayPeriodVM GetPayPeriodDetail(int _payPeriodId)
        {
            PayPeriodVM paperiodvm = new PayPeriodVM();

            if (_payPeriodId != 0)
            {
                paperiodvm = _context.PayPeriods
                   .Where(x => x.PayPeriodId == _payPeriodId)
                   .Select(x => new PayPeriodVM
                   {
                       PayPeriodId = x.PayPeriodId,
                       StartDate = x.StartDate,
                       EndDate = x.EndDate,
                       CompanyCodeId = x.CompanyCodeId,
                       IsArchived = x.IsArchived,
                       IsPayPeriodActive = x.IsPayPeriodActive,
                       LockoutEmployees = x.LockoutEmployees,
                       LockoutManagers = x.LockoutManagers,
                       PayGroupCode = x.PayGroupCode,
                       PayPeriodNumber = x.PayPeriodNumber,
                       PayFrequencyId = x.PayFrequencyId,
                       PayDate=x.PayDate,
                       Isexported=x.Isexported
                   })
                   .FirstOrDefault();
                paperiodvm.ManagersList = GetmanagersLockedoutlist(_payPeriodId);
                paperiodvm.ManagersdonthaveLockeList = Getmanagersdonthavelocklist(_payPeriodId);
            }

            paperiodvm.CompanyCodeDropdown = _context.CompanyCodes.Where(x => x.IsCompanyCodeActive == true)
               .Select(m => new DropDownModel
               {
                   keyvalue = m.CompanyCodeId.ToString(),
                   keydescription = m.CompanyCodeCode.ToString(),
               })
               .OrderBy(x => x.keyvalue)
               .ToList();

            paperiodvm.PayFrequencyDropdown = _context.DdlPayFrequencies.Where(x => x.Active == true)
               .Select(m => new DropDownModel
               {
                   keyvalue = m.PayFrequencyId.ToString(),
                   keydescription = m.Description.ToString(),
               })
               .OrderBy(x => x.keyvalue)
               .ToList();
            paperiodvm.PayPeriodDropdown = _context.DdlPayGroups.Where(x => x.Active == true)
                .Select(m => new DropDownModel
                {
                    keyvalue = m.Code.ToString(),
                    keydescription = m.Description.ToString(),
                }).OrderBy(x => x.keyvalue)
               .ToList();
            if (_payPeriodId == 0)
            {
                //  paperiodvm.ManagersList = Getmanagerslist();
                paperiodvm.ManagersList = GetmanagersLockedoutlist(_payPeriodId);
                paperiodvm.ManagersdonthaveLockeList = Getmanagersdonthavelocklist(_payPeriodId);
            }
            return paperiodvm;
        }

        public PayPeriodVM savePayPeriod(PayPeriodVM payperiodvm,string userId)
        {

            //var easternZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            //payperiodvm.StartDate = TimeZoneInfo.ConvertTimeFromUtc(payperiodvm.StartDate, easternZone);
            //payperiodvm.EndDate = TimeZoneInfo.ConvertTimeFromUtc(payperiodvm.EndDate, easternZone);

            if (payperiodvm.PayPeriodId != 0)
            {
                var payperiodrecord = _context.PayPeriods.Where(x => x.PayPeriodId == payperiodvm.PayPeriodId).FirstOrDefault();
                if (payperiodrecord != null)
                {

                    payperiodrecord.StartDate = payperiodvm.StartDate.Date;
                    payperiodrecord.EndDate = payperiodvm.EndDate.Date;
                    payperiodrecord.CompanyCodeId = payperiodvm.CompanyCodeId;
                    payperiodrecord.PayFrequencyId = payperiodvm.PayFrequencyId;
                    payperiodrecord.IsPayPeriodActive = payperiodvm.IsPayPeriodActive;
                    payperiodrecord.IsArchived = payperiodvm.IsArchived;
                    payperiodrecord.LockoutEmployees = payperiodvm.LockoutEmployees;
                    payperiodrecord.LockoutManagers = payperiodvm.LockoutManagers;
                    payperiodrecord.PayGroupCode = payperiodvm.PayGroupCode;
                    payperiodrecord.PayPeriodNumber = payperiodvm.PayPeriodNumber;
                    payperiodrecord.PayDate = payperiodvm.PayDate;
                    payperiodrecord.UserId = userId;
                    payperiodrecord.LastModifiedDate = DateTime.Now;
                    _context.SaveChanges();
                    DeleteManagersLockoutlist(payperiodvm.PayPeriodId);
                    SaveManagersLockoutlist(payperiodvm, payperiodvm.PayPeriodId);
                }
                return GetPayPeriodDetail(payperiodvm.PayPeriodId);
            }
            else
            {
                var ppvm = new PayPeriod()
                {
                    StartDate = payperiodvm.StartDate.Date,
                    EndDate = payperiodvm.EndDate.Date,
                    PayFrequencyId = payperiodvm.PayFrequencyId,
                    CompanyCodeId = payperiodvm.CompanyCodeId,
                    IsPayPeriodActive = payperiodvm.IsPayPeriodActive,
                    IsArchived = payperiodvm.IsArchived,
                    LockoutEmployees = payperiodvm.LockoutEmployees,
                    LockoutManagers = payperiodvm.LockoutManagers,
                    PayGroupCode = payperiodvm.PayGroupCode,
                    PayPeriodNumber = payperiodvm.PayPeriodNumber,
                    PayDate=payperiodvm.PayDate,
                    IsDeleted = false,
                    UserId = userId,
                    LastModifiedDate = DateTime.Now
            };
                _context.PayPeriods.Add(ppvm);
                _context.SaveChanges();
                DeleteManagersLockoutlist(ppvm.PayPeriodId);
                SaveManagersLockoutlist(payperiodvm, ppvm.PayPeriodId);
                return GetPayPeriodDetail(ppvm.PayPeriodId);
            }

        }

        //public void DeletePayPeriod(int payPeriodId)
        //{
        //    DeleteManagersLockoutlist(payPeriodId);
        //    var payperioddetails = _context.PayPeriods.Where(x => x.PayPeriodId == payPeriodId).FirstOrDefault();
        //    if (payperioddetails != null)
        //    {
        //        _context.PayPeriods.Remove(payperioddetails);
        //        _context.SaveChanges();
        //    }
        //}
        public void DeletePayPeriod(int payPeriodId,string userId)
        {
            DeleteManagersLockoutlist(payPeriodId);
            bool result = false;
            try
            {
                PayPeriod payperioddetails = _context.PayPeriods
                    .Where(x => x.PayPeriodId == payPeriodId).SingleOrDefault();
                if (payperioddetails != null)
                {
                    payperioddetails.IsDeleted = true;
                    payperioddetails.DeletedBy = userId;
                    payperioddetails.LastModifiedDate = DateTime.Now;
                    result = _context.SaveChanges() > 0 ? true : false;
                }
            }
            catch (Exception err)
            {
                string message = err.Message;
            }
        }
        public void LockoutemployeeUpdate(int payPeriodId, bool lockoutemployee)
        {
            var payperioddetails = _context.PayPeriods.Where(x => x.PayPeriodId == payPeriodId).FirstOrDefault();
            if (payperioddetails != null)
            {
                if (lockoutemployee == true)
                {
                    payperioddetails.LockoutEmployees = false;
                }
                else
                {
                    payperioddetails.LockoutEmployees = true;
                }
                _context.SaveChanges();
            }
        }

        public void LockoutManagersUpdate(int payPeriodId, bool lockoutManagers)
        {
            var payperioddetails = _context.PayPeriods.Where(x => x.PayPeriodId == payPeriodId).FirstOrDefault();
            if (payperioddetails != null)
            {
                if (lockoutManagers == true)
                {
                    payperioddetails.LockoutManagers = false;
                }
                else
                {
                    payperioddetails.LockoutManagers = true;
                }
                _context.SaveChanges();
            }
        }
        public List<PayPeriodVM> GetPayPeriodListByCompanyCode(int CompanyCodeId)
        {
            PayPeriodVM payperiodcmycode = new PayPeriodVM();
            var payperiodcmycode1 = (from pp in _context.PayPeriods
                                     join cc in _context.CompanyCodes on pp.CompanyCodeId equals cc.CompanyCodeId
                                     where pp.CompanyCodeId == CompanyCodeId
                                     select new PayPeriodVM
                                     {
                                         PayFrequencyId = pp.PayFrequencyId,
                                         CompanyCodeDescription = cc.CompanyCodeDescription,
                                         CompanyCodeId = pp.CompanyCodeId,
                                         StartDate = pp.StartDate,
                                         EndDate = pp.EndDate,
                                         LockoutManagers = pp.LockoutManagers,
                                         LockoutEmployees = pp.LockoutEmployees,
                                         Description = _context.DdlPayGroups.Where(x=>x.PayGroupId==pp.PayGroupCode).Select(x=>x.Description).FirstOrDefault(),
                                         PayDate = pp.PayDate,
                                         IsArchived = pp.IsArchived,
                                         PayPeriodNumber = pp.PayPeriodNumber,
                                         PayFrequencyName = _context.DdlPayFrequencies.Where(x=>x.PayFrequencyId==pp.PayFrequencyId).Select(x=>x.Description).FirstOrDefault()
                                     }).ToList();
            return payperiodcmycode1;
        }


        #region ManagersLockoutList

        public string GetManagersCount(int payperiodId)
        {
            string managerstotalcount = "";
            string managerscount = "0";
            string managerslockoutcount = "0";

            var mangerscountlist = Getmanagerslist();
            var mangreslockoutlist = GetmanagersLockedoutlist(payperiodId);
            if (mangreslockoutlist.Count > 0)
            {
                managerslockoutcount = mangreslockoutlist.Count.ToString();
            }
            if (mangerscountlist.Count > 0)
            {
                managerscount = mangerscountlist.Count.ToString();
            }
            managerstotalcount = managerslockoutcount + "/" + managerscount;
            return managerstotalcount;
        }

        public List<ManagerLockoutsVM> Getmanagerslist()
        {

            int?[] txt = _context.Positions.Select(x => x.ReportsToPositionId).ToArray();
            List<ManagerLockoutsVM> managerslist = (from pers in _context.Persons
                                                    join emp in _context.Employees on pers.PersonId equals emp.PersonId
                                                    join eposi in _context.E_Positions on emp.EmployeeId equals eposi.EmployeeId
                                                    join user in _context.UserNamesPersons on pers.PersonId equals user.PersonID
                                                    join posi in _context.Positions on eposi.PositionId equals posi.PositionId
                                                    where txt.Contains(posi.PositionId) && eposi.IsDeleted == false
                                                    select new ManagerLockoutsVM
                                                    {
                                                        PersonId = pers.PersonId,
                                                        ManagerUserName = user.UserName,

                                                    }).GroupBy(x => x.PersonId).Select(z => z.OrderBy(i => i.PersonId).FirstOrDefault()).ToList();
            return managerslist;
        }

        public bool DeleteManagersLockoutlist(int payperiodid)
        {
            bool result = false;
            var managerslist = _context.ManagerLockouts.Where(x => x.PayPeriodId == payperiodid).ToList();
            if (managerslist != null)
            {
                foreach (var item in managerslist)
                {
                    var managersdetails = _context.ManagerLockouts.Where(x => x.PayPeriodId == item.PayPeriodId).FirstOrDefault();
                    _context.ManagerLockouts.Remove(managersdetails);
                    _context.SaveChanges();
                    result = true;
                }
            }
            return result;
        }
        public bool SaveManagersLockoutlist(PayPeriodVM ppvm, int payperiodid)
        {
            bool result = false;
            if (ppvm.ManagersList != null)
            {
                foreach (var item in ppvm.ManagersList)
                {
                    var mnaagerslockout = new ManagerLockouts
                    {
                        PayPeriodId = payperiodid,
                        ManagerUserName = item.ManagerUserName,
                    };
                    _context.ManagerLockouts.Add(mnaagerslockout);
                    _context.SaveChanges();
                    result = true;
                }
            }
            return result;
        }

        public List<ManagerLockoutsVM> GetmanagersLockedoutlist(int payperiodId)
        {
            List<ManagerLockoutsVM> managerslist = (from mangers in _context.ManagerLockouts
                                                   .Where(x => x.PayPeriodId == payperiodId)
                                                    select new ManagerLockoutsVM
                                                    {
                                                        PersonId = mangers.PayPeriodId,
                                                        ManagerUserName = mangers.ManagerUserName,
                                                    }).ToList();
            return managerslist;
        }
        public List<ManagerLockoutsVM> Getmanagersdonthavelocklist(int payperiodId)
        {
            List<ManagerLockoutsVM> Managerddonathavelock = new List<ManagerLockoutsVM>();
            var managerslist = new List<ManagerLockoutsVM>();
            string[] managerslockedlist = _context.ManagerLockouts.Where(x => x.PayPeriodId == payperiodId).Select(x => x.ManagerUserName).ToArray();
            //int?[] txt = _context.Positions.Select(x => x.ReportsToPositionId).ToArray();
            //List<ManagerLockoutsVM> managerslist = (from pers in _context.Persons
            //                                        join emp in _context.Employees on pers.PersonId equals emp.PersonId
            //                                        join eposi in _context.E_Positions on emp.EmployeeId equals eposi.EmployeeId
            //                                        join user in _context.UserNamesPersons on pers.PersonId equals user.PersonID
            //                                        join posi in _context.Positions on eposi.PositionId equals posi.PositionId
            //                                        where txt.Contains(posi.PositionId)
            //                                        select new ManagerLockoutsVM
            //                                        {
            //                                            PersonId = pers.PersonId,
            //                                            ManagerUserName = user.UserName,

            //                                        }).GroupBy(x => x.PersonId).Select(z => z.OrderBy(i => i.PersonId).FirstOrDefault()).ToList();
            managerslist = (from epos in _context.E_Positions
                            join per in _context.Persons
                            on epos.ReportsToID equals per.PersonId
                            where epos.IsDeleted == false
                            select new ManagerLockoutsVM
                            {
                                PersonId = epos.ReportsToID,
                                ManagerUserName = per.Lastname + " " + per.Firstname
                            }).OrderBy(e => e.ManagerUserName).Distinct().ToList();

            foreach (var item in managerslist)
            {
                if (managerslockedlist.Contains(item.ManagerUserName))
                {

                }
                else
                {
                    var mnaagerslockout = new ManagerLockoutsVM
                    {
                        PersonId = item.PersonId,
                        ManagerUserName = item.ManagerUserName,
                    };
                    Managerddonathavelock.Add(mnaagerslockout);
                }
            }
            return Managerddonathavelock;
        }
        #endregion

        #region ArchivePayperiod
        public bool ArchivePayperiod(short companyCodeId, int PayFrequencyid, DateTime startdate, DateTime enddate, int PayPeriodId)
        {
            bool IsTimeCardArchive = false;
            bool IstimeCardsNotesArchive = false;
            //List<EmployeesVM> emplist = GetAllEmpWithpayperiod(PayFrequencyid);
            List<TimeCardVm> timecardslist = GettimecardsBetweendates(companyCodeId, startdate, enddate);
            foreach (var item in timecardslist)
            {
                //int companycodeid = Convert.ToInt32(emprow.CompanyCodeId);

                ///  string filenumber = emprow.FileNumber;
                //int employeeid = emprow.EmployeeId;

                //Get TimecardNotes, Archive and Delelte
                TimeCardsNotesVM timcardsnoteslist = GettimecardsNotesBetweendates(companyCodeId, item.TimeCardId);
                if (timcardsnoteslist != null)
                {
                    IstimeCardsNotesArchive = InsertTimecardsNotesArchive(timcardsnoteslist);
                    DeleteTimecardsNotes(timcardsnoteslist.TimeCardsNotesId);
                }
                //timcardsnoteslist.Remove(itemtimenotes);
                /*in Legacy they are calling lyk this after insertion of TimeCardsNotes to TimeCardsNotesArchive table*/
                // timeCardNotes.Update(dtTimeCardNotes);

                //Get TimecardList, Archive and Delelte
                //List<TimeCardVm> timecardslist = GettimecardsBetweendates(companycodeid, employeeid, startdate, enddate);

                IsTimeCardArchive = InsertTimecardsArchive(item);
                DeleteTimecards(item.TimeCardId);
                /*in Legacy they are calling lyk this after insertion of TimeCards to TimeCardsArchive table*/
                // timeCards.Update(dtTimeCards);

            }
            /* Update the Payperiod Atble set the IsArchived to true*/
            bool result = false;
            if (IsTimeCardArchive == true || IstimeCardsNotesArchive == true)
            {
                var payperioddetails = _context.PayPeriods
                                       .Where(x => x.PayPeriodId == PayPeriodId && x.CompanyCodeId == (int)companyCodeId).FirstOrDefault();
                if (payperioddetails != null)
                {
                    payperioddetails.IsArchived = true;
                    _context.SaveChanges();
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            return result;
        }

        public List<EmployeesVM> GetAllEmpWithpayperiod(int payfrequencyid)
        {
            List<EmployeesVM> EmloyeeList = new List<EmployeesVM>();
            EmloyeeList = (from emp in _context.Employees
                           where (emp.PayFrequencyId == payfrequencyid)
                           select new EmployeesVM
                           {
                               EmployeeId = emp.EmployeeId,
                               CompanyCodeId = (short)emp.CompanyCodeId,
                               FileNumber = emp.FileNumber,
                               PersonId = emp.PersonId,
                           }).ToList();
            return EmloyeeList;
        }

        public List<TimeCardVm> GettimecardsBetweendates(int Companycodeid, DateTime strtdate, DateTime enddate)
        {
            List<TimeCardVm> timecardlist = new List<TimeCardVm>();
            timecardlist = (from tc in _context.TimeCards
                            where (tc.CompanyCodeId == Companycodeid && tc.ActualDate >= strtdate && tc.ActualDate <= enddate /*&& tc.IsApproved == true*/)
                            select new TimeCardVm
                            {
                                TimeCardId = tc.TimeCardId,
                                CompanyCodeId = tc.CompanyCodeId,
                                EmployeeId = tc.EmployeeId,
                                FileNumber = tc.FileNumber,
                                ActualDate = tc.ActualDate,
                                ProjectNumber = tc.ProjectNumber,
                                DailyHours = tc.DailyHours,
                                TimeIn = tc.TimeIn,
                                LunchOut = tc.LunchOut,
                                LunchBack = tc.LunchBack,
                                TimeOut = tc.TimeOut,
                                HoursCodeId = tc.HoursCodeId,
                                Hours = tc.Hours,
                                EarningsAmount = tc.EarningsAmount,
                                EarningsCodeId = tc.EarningsCodeId,
                                HoursCodeReasonId = tc.HoursCodeReasonId,
                                DepartmentId = tc.DepartmentId,
                                JobId = tc.JobId,
                                TempDeptId = tc.TempDeptId,
                                TempJobId = tc.TempJobId,
                                Project = tc.Project,
                                Task = tc.Task,
                                OT = tc.OT,
                                MealsTaken = tc.MealsTaken,
                                Rate = tc.Rate,
                                HoursCodeRate = tc.HoursCodeRate,
                                EnteredBy = tc.EnteredBy,
                                EnteredDate = System.DateTime.Now,
                                IsApproved = tc.IsApproved,
                                ApprovedBy = tc.ApprovedBy,
                                PositionId = tc.PositionId,
                                UserId = tc.UserId,
                                LastModifiedDate = tc.LastModifiedDate,
                                IsDeleted = tc.IsDeleted
                            }).ToList();
            return timecardlist;
        }

        public TimeCardsNotesVM GettimecardsNotesBetweendates(int Companycodeid, int timeCardId)
        {
            TimeCardsNotesVM timecardnoteslist = new TimeCardsNotesVM();
            timecardnoteslist = (from tc in _context.TimeCardsNotes
                                 where (tc.CompanyCodeId == Companycodeid && tc.TimeCardId == timeCardId)
                                 select new TimeCardsNotesVM
                                 {
                                     TimeCardsNotesId = tc.TimeCardsNotesId,
                                     EmployeeId = tc.EmployeeId,
                                     CompanyCodeId = tc.CompanyCodeId,
                                     FileNumber = tc.FileNumber,
                                     ActualDate = tc.ActualDate,
                                     Notes = tc.Notes,
                                     TimeCardId = tc.TimeCardId
                                 }).FirstOrDefault();
            return timecardnoteslist;
        }

        public bool InsertTimecardsArchive(TimeCardVm tcvm)
        {
            bool result = false;
            _context.TimeCardsArchive.Add(new TimeCardsArchive
            {
                CompanyCodeId = tcvm.CompanyCodeId,
                FileNumber = tcvm.FileNumber,
                ActualDate = tcvm.ActualDate,
                ProjectNumber = tcvm.ProjectNumber,
                DailyHours = tcvm.DailyHours,
                TimeIn = tcvm.TimeIn,
                LunchOut = tcvm.LunchOut,
                LunchBack = tcvm.LunchBack,
                TimeOut = tcvm.TimeOut,
                HoursCodeId = tcvm.HoursCodeId,
                Hours = tcvm.Hours,
                HoursCodeReasonId = tcvm.HoursCodeReasonId,
                EarningsCodeId = tcvm.EarningsCodeId,
                EarningsAmount = tcvm.EarningsAmount,
                DepartmentId = tcvm.DepartmentId,
                JobId = tcvm.JobId,
                //TempDeptId = tcvm.TempDeptId,
                TempJobId = tcvm.TempJobId,
                Project = tcvm.Project,
                Task = tcvm.Task,
                OT = tcvm.OT,
                MealsTaken = tcvm.MealsTaken,
                Rate = tcvm.Rate,
                HoursCodeRate = tcvm.HoursCodeRate,
                EnteredBy = tcvm.EnteredBy,
                EnteredDate = DateTime.Now,
                EmployeeId = tcvm.EmployeeId,
                IsApproved = tcvm.IsApproved,
                ApprovedBy = tcvm.ApprovedBy,
                PositionId = tcvm.PositionId,
                UserId = tcvm.UserId,
                LastModifiedDate = tcvm.LastModifiedDate,
                TimeCardId = tcvm.TimeCardId,
                IsDeleted = tcvm.IsDeleted
            });
            // _context.TimeCardsArchives.Add(tcarc);
            //_context.SaveChanges();
            // result = true;
            result = _context.SaveChanges() > 0 ? true : false;
            return result;
        }

        public bool DeleteTimecards(int timeCardsId)
        {
            bool result = false;

            try
            {
                var timeCards = _context.TimeCards.Where(x => x.TimeCardId == timeCardsId).SingleOrDefault();

                if (timeCards != null)
                {
                    _context.TimeCards.Remove(timeCards);
                    result = _context.SaveChanges() > 0 ? true : false;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public bool InsertTimecardsNotesArchive(TimeCardsNotesVM tcvm)
        {
            bool result = false;
            var tcarc = new TimeCardNotesArchive()
            {
                CompanyCodeId = tcvm.CompanyCodeId,
                EmployeeId = tcvm.EmployeeId,
                FileNumber = tcvm.FileNumber,
                ActualDate = tcvm.ActualDate,
                Notes = tcvm.Notes,
                TimeCardId = tcvm.TimeCardId
            };
            _context.TimeCardNotesArchives.Add(tcarc);
            //_context.SaveChanges();
            result = _context.SaveChanges() > 0 ? true : false;
            return result;
        }

        public bool DeleteTimecardsNotes(int timeCardsNotesId)
        {
            bool result = false;

            try
            {
                var timeCardsNotes = _context.TimeCardsNotes.Where(x => x.TimeCardsNotesId == timeCardsNotesId).SingleOrDefault();

                if (timeCardsNotes != null)
                {
                    _context.TimeCardsNotes.Remove(timeCardsNotes);
                    result = _context.SaveChanges() > 0 ? true : false;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public List<TimeCardExportCollectionVM> Export_Employee_TimecardDetails(int companyCodeId, int payPeriodId)
        {
            List<TimeCardExportCollectionVM> Emptimecardlist = new List<TimeCardExportCollectionVM>();
            Emptimecardlist = Query<TimeCardExportCollectionVM>("sp_TimeCardExport", new { @companyCodeId = companyCodeId, @payPeriodId = payPeriodId }).ToList();
            return Emptimecardlist;
        }

        public List<T> Query<T>(string storedProcName)
        {
            try
            {
                return _connection.Query<T>(storedProcName, commandType: CommandType.StoredProcedure, commandTimeout: _timeout).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }

        public int GetUnApprovedTimecards(int? companyCodeId, DateTime startdate, DateTime enddate)
        {
            int count = _context.TimeCards.Where(x => x.CompanyCodeId == companyCodeId && x.IsDeleted == false && (x.ActualDate >= startdate && x.ActualDate <= enddate) && x.IsApproved == false).Count();
            return count;
        }
        public int GetTimecardCount(int? companyCodeId, DateTime startdate, DateTime enddate)
        {
            int count = _context.TimeCards.Where(x => x.CompanyCodeId == companyCodeId && x.IsDeleted == false && (x.ActualDate >= startdate && x.ActualDate <= enddate)).Count();
            return count;
        }
        #endregion

        #region Global Archive for all Companies
        public bool ArchiveGlobalPayperiod(int PayFrequencyid, DateTime startdate, DateTime enddate, int PayPeriodId)
        {
            List<TimeCardVm> timecardslist = GetGlobalTimeCardsBetweendates(startdate, enddate);
            var dtArchieveData = TimecardsArchiveData();
            foreach (var item in timecardslist)
            {
                dtArchieveData.Rows.Add(item.TimeCardId, item.EmployeeId);
            }
            SqlParameter param = new SqlParameter();
            param.SqlDbType = SqlDbType.Structured;
            param.ParameterName = "@TimeCardArchieveData";
            param.TypeName = "dbo.DtoTimeCardArchieveData";
            param.Value = dtArchieveData;
           
             var objValue = _context.Database.SqlQuery<int>("exec dbo.spInsertTimeCardArchiveData @TimeCardArchieveData", param).FirstOrDefault();
           /* Update the Payperiod Atble set the IsArchived to true*/
            bool result = false;
            if (objValue > 0)
            {
                var payperioddetails = _context.PayPeriods
                                       .Where(x => x.PayPeriodId == PayPeriodId).Distinct().FirstOrDefault();
                if (payperioddetails != null)
                {
                    payperioddetails.IsArchived = true;
                    _context.SaveChanges();
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            return result;
            //using (DbContextTransaction dbTran = _context.Database.BeginTransaction())
            //{
            //    try
            //    {
            //        //   var Inserttimecarddatalist= Query<TimeCardVm>("sp_GetTimecardInsertList", new { @startdate = startdate, @enddate = enddate }).ToList();
            //        foreach (var item in timecardslist)
            //        {

            //            //Get TimecardNotes, Archive and Delelte
            //            TimeCardsNotesVM timcardsnoteslist = GetGlobalTimeCardsNotesBetweendates(item.TimeCardId);
            //            if (timcardsnoteslist != null)
            //            {
            //                IstimeCardsNotesArchive = InsertTimecardsNotesArchive(timcardsnoteslist);
            //                DeleteTimecardsNotes(timcardsnoteslist.TimeCardsNotesId);
            //            }

            //            IsTimeCardArchive = InsertTimecardsArchive(item);
            //            //public List<BudgetToActualsReportVm> GetBudgetToActualsReportList(int budgetYear, int month)
            //            //{
            //            //    return Query<BudgetToActualsReportVm>("uspBudgetToActualsV01", new { BudgetYear = budgetYear, Month = month });
            //            //}
            //            DeleteTimecards(item.TimeCardId);
            //            //var DeleteTimecarddata = Query<TimeCardVm>("sp_GetTimecardDeleteList ", new {@TimeCardId =item.TimeCardId}).ToList();
            //        }
            //        // var DeleteTimecarddatalist = Query<TimeCardVm>("sp_GetTimecardDeleteList",new {@startdate= startdate, @enddate= enddate}).ToList();
            //        dbTran.Commit();
            //    }
            //    catch (Exception ex)
            //    {
            //        dbTran.Rollback();
            //        throw;
            //    }
            //}

        }

        public List<TimeCardVm> GetGlobalTimeCardsBetweendates(DateTime strtdate, DateTime enddate)
        {
            List<TimeCardVm> timecardlist = new List<TimeCardVm>();
            timecardlist = (from tc in _context.TimeCards
                            where (tc.ActualDate >= strtdate && tc.ActualDate <= enddate /*&& tc.IsApproved == true*/)
                            select new TimeCardVm
                            {
                                TimeCardId = tc.TimeCardId,
                                CompanyCodeId = tc.CompanyCodeId,
                                EmployeeId = tc.EmployeeId,
                                FileNumber = tc.FileNumber,
                                ActualDate = tc.ActualDate,
                                ProjectNumber = tc.ProjectNumber,
                                DailyHours = tc.DailyHours,
                                TimeIn = tc.TimeIn,
                                LunchOut = tc.LunchOut,
                                LunchBack = tc.LunchBack,
                                TimeOut = tc.TimeOut,
                                HoursCodeId = tc.HoursCodeId,
                                Hours = tc.Hours,
                                EarningsAmount = tc.EarningsAmount,
                                EarningsCodeId = tc.EarningsCodeId,
                                HoursCodeReasonId = tc.HoursCodeReasonId,
                                DepartmentId = tc.DepartmentId,
                                JobId = tc.JobId,
                                TempDeptId = tc.TempDeptId,
                                TempJobId = tc.TempJobId,
                                Project = tc.Project,
                                Task = tc.Task,
                                OT = tc.OT,
                                MealsTaken = tc.MealsTaken,
                                Rate = tc.Rate,
                                HoursCodeRate = tc.HoursCodeRate,
                                EnteredBy = tc.EnteredBy,
                                EnteredDate = System.DateTime.Now,
                                IsApproved = tc.IsApproved,
                                ApprovedBy = tc.ApprovedBy,
                                PositionId = tc.PositionId,
                                UserId = tc.UserId,
                                LastModifiedDate = tc.LastModifiedDate,
                                IsDeleted = tc.IsDeleted
                            }).ToList();
            return timecardlist;
        }

        public TimeCardsNotesVM GetGlobalTimeCardsNotesBetweendates(int timeCardId)
        {
            TimeCardsNotesVM timecardnoteslist = new TimeCardsNotesVM();
            timecardnoteslist = (from tc in _context.TimeCardsNotes
                                 where (tc.TimeCardId == timeCardId)
                                 select new TimeCardsNotesVM
                                 {
                                     TimeCardsNotesId = tc.TimeCardsNotesId,
                                     EmployeeId = tc.EmployeeId,
                                     CompanyCodeId = tc.CompanyCodeId,
                                     FileNumber = tc.FileNumber,
                                     ActualDate = tc.ActualDate,
                                     Notes = tc.Notes,
                                     TimeCardId = tc.TimeCardId
                                 }).FirstOrDefault();
            return timecardnoteslist;
        }

        public int GetGlobalUnApprovedTimecards(DateTime startdate, DateTime enddate)
        {
            int count = _context.TimeCards.Where(x => x.IsDeleted == false && (x.ActualDate >= startdate && x.ActualDate <= enddate) && x.IsApproved == false).Count();
            return count;
        }
        public int GetGlobalTimecardCount(DateTime startdate, DateTime enddate)
        {
            int count = _context.TimeCards.Where(x => x.IsDeleted == false && (x.ActualDate >= startdate && x.ActualDate <= enddate)).Count();
            return count;
        }

        #endregion

        public string AutoSendEmailTreatyLimit(int companyCode, int? payGroupId)
        {
            StringBuilder sw = new StringBuilder();
            //sw.AppendLine("The fallowing Students are exceeded Treaty Limit.");
            sw.AppendLine("");
            sw.Append("<table style=\"text-align: center\" border =\"1\"><tr><th>Name</th><th>File Number</th><th>Company Code</th></tr>");
            //var empList = _context.Employees.Include("Persons").Include("CompanyCodes").Where(x => x.CompanyCodeId == companyCode && x.UsedAmount > x.TreatyLimit)
            //     .Select(x => new { x.FileNumber, Name = x.Person.Firstname+" "+ x.Person.Lastname, x.CompanyCode1.CompanyCodeCode}).ToList();
            List<TreatyLimit> Emptimecardlist = new List<TreatyLimit>();
           var  empList = Query<TreatyLimit>("spExceededTreatyLimitEmpList", new { @CompanyCodeId = companyCode }).ToList();            
            if (empList.Count > 0)
            {
                foreach (var item in empList)
                {
                    sw.Append("<tr><td>"+ item.Name + "</td><td>" + item.FileNumber + "</td><td>" + item.CompanyCodeCode + "</td></tr>");
                }
                return sw.ToString();
            }
            return null;
        }

        public class TreatyLimit
        {
            public string Name {get; set;}
            public string FileNumber { get; set; }
            public string CompanyCodeCode { get; set; }
        }

        public DataTable TimecardsArchiveData()
        {
            var dt = new DataTable();
            dt.Columns.Add("TimeCardId", typeof(int));
            dt.Columns.Add("EmployeeId", typeof(int));
            return dt;

        }
    }
}

