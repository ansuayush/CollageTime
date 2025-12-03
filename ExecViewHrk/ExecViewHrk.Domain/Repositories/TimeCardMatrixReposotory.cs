using ExecViewHrk.Domain.Interface;
using ExecViewHrk.EfClient;
using ExecViewHrk.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExecViewHrk.Domain.Repositories
{
    public class TimeCardMatrixReposotory : RepositoryBase, ITimeCardMatrixReposotory
    {
        string timeZone = System.Configuration.ConfigurationManager.AppSettings["TimeZone"].ToString();
        readonly ILookupTablesRepository _ilookupRepo;
        public TimeCardMatrixReposotory(ILookupTablesRepository ilookupRepo)
        {
            _ilookupRepo = ilookupRepo;
        }

        #region Time Card Grid Read, Create, Update and Delete

        public List<TimeCardCollectionVm> GetTimeCardsList(int? employeeIdDdl, int? payPeriodId, bool? IsArchived)
        {
            List<TimeCardCollectionVm> empTimeCardList = new List<TimeCardCollectionVm>();
            empTimeCardList = Query<TimeCardCollectionVm>("sp_GetTimeCardsList", new { @empId = employeeIdDdl, @isArchived = IsArchived, @PayPeriodId = payPeriodId }).ToList();
            if (empTimeCardList.Count != 0)
            {
                foreach (var item in empTimeCardList)
                {
                    int id = _context.TimeCardsNotes.Where(m => m.TimeCardId == item.TimeCardId).Select(x => x.TimeCardsNotesId).FirstOrDefault();
                    item.NotesId = id != 0 ? id : 0;


                    var sumHours = _context.TimeCards.Where(r => r.ActualDate == item.ActualDate && r.EmployeeId == item.EmployeeId && r.IsDeleted == false).Sum(r => r.DailyHours);
                    item.LineTotal = sumHours == 0 ? null : sumHours;
                }
            }
            return empTimeCardList;
        }

        //Loads Time Card List for the Manager with corresponding Positions assigned
        public List<TimeCardCollectionVm> GetTimeCardsListReportToPositions(int? employeeIdDdl, int? payPeriodId, bool? IsArchived, string loggedInUserId)
        {
            var aspNetUsersEmail = _context.AspNetUsers.Where(x => x.UserName == loggedInUserId).Select(x => x.Email).FirstOrDefault();
            var @ReportToId = _context.Persons.Where(x => x.eMail == aspNetUsersEmail).Select(x => x.PersonId).FirstOrDefault();

            List<TimeCardCollectionVm> empTimeCardList = new List<TimeCardCollectionVm>();
            empTimeCardList = Query<TimeCardCollectionVm>("sp_GetTimeCardsListReportToPositions", new { @empId = employeeIdDdl, @isArchived = IsArchived, @PayPeriodId = payPeriodId, @ReportToId = @ReportToId }).ToList();
            if (empTimeCardList.Count != 0)
            {
                foreach (var item in empTimeCardList)
                {
                    int id = _context.TimeCardsNotes.Where(m => m.ActualDate == item.ActualDate).Select(x => x.TimeCardsNotesId).FirstOrDefault();
                    item.NotesId = id != 0 ? id : 0;


                    var sumHours = empTimeCardList.Where(r => r.EmployeeId == employeeIdDdl && r.ActualDate == item.ActualDate).Sum(r => r.DailyHours);
                    item.LineTotal = sumHours == 0 ? null : sumHours;
                }
            }
            return empTimeCardList;
        }
        public List<TimeCard> SaveTimeCardsList(IEnumerable<TimeCardVm> timeCardVmGridCollection, Dictionary<DateTime, int> Date_MaxProjectNum, int maxProjectNum, int employeeIdDdl, int? departmentId, int companyCodeIdDdl, string userId, int? payPeriodId)
        {
            var easternZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            var timeUtc = DateTime.Now;
            //DateTime lastModifiedDate = TimeZoneInfo.ConvertTime(timeUtc, easternZone);
            var timeCardsList = new List<TimeCard>();
            foreach (var timeCardVm in timeCardVmGridCollection)
            {
                if (timeCardVm.TimeIn != null || timeCardVm.TimeOut != null || timeCardVm.LunchOut != null || timeCardVm.LunchBack != null || timeCardVm.Hours > 0)
                {
                    var epositionlist = Query<TimeCardVm>("GetE_PositionIdbasedonpunchdate", new { @EmployeeId = employeeIdDdl, @positionId = timeCardVm.PositionId, @Companycodeid = companyCodeIdDdl, @payPeriodId = payPeriodId }).ToList();
                    var epositionid = epositionlist.Select(x => x.E_PositionId).FirstOrDefault();
                    if (Date_MaxProjectNum.ContainsKey(timeCardVm.ActualDate.Date))
                    {
                        maxProjectNum = Convert.ToInt32(Date_MaxProjectNum[timeCardVm.ActualDate.Date]) + 1;

                        //update the Dictionary
                        Date_MaxProjectNum[timeCardVm.ActualDate.Date] = Date_MaxProjectNum[timeCardVm.ActualDate.Date] + 1; ;
                    }
                    else
                    {
                        maxProjectNum = 1;

                        //Add to Dictionary
                        Date_MaxProjectNum.Add(timeCardVm.ActualDate.Date, 1);
                    }
                    //TimeZone Start
                    //timeCardVm.ActualDate = TimeZoneInfo.ConvertTime(timeCardVm.ActualDate, easternZone);
                    //if (timeCardVm.TimeIn != null)
                    //{
                    //    timeCardVm.TimeIn = TimeZoneInfo.ConvertTime(timeCardVm.TimeIn.Value, easternZone);
                    //}
                    //if (timeCardVm.LunchOut != null)
                    //{
                    //    timeCardVm.LunchOut = TimeZoneInfo.ConvertTime(timeCardVm.LunchOut.Value, easternZone);
                    //}
                    //if (timeCardVm.LunchBack != null)
                    //{
                    //    timeCardVm.LunchBack = TimeZoneInfo.ConvertTime(timeCardVm.LunchBack.Value, easternZone);
                    //}
                    //if (timeCardVm.TimeOut != null)
                    //{
                    //    timeCardVm.TimeOut = TimeZoneInfo.ConvertTime(timeCardVm.TimeOut.Value, easternZone);
                    //}                    
                    var hours = dailyHourscalc(timeCardVm);
                    double? dailyHours = Math.Round(hours, 2);
                    //TimeZone End
                    //Corresponding Posirion DepartmentId
                    var positionDepartmentId = _context.E_Positions
                                                      .Where(x => x.PositionId == timeCardVm.PositionId && x.EmployeeId == employeeIdDdl)
                                                      .Select(d => d.DepartmentId).FirstOrDefault();
                    var newTimeCardRecord = new TimeCard
                    {
                        CompanyCodeId = companyCodeIdDdl,
                        EmployeeId = employeeIdDdl,
                        //DepartmentId = departmentId,
                        DepartmentId = positionDepartmentId,
                        ActualDate = timeCardVm.ActualDate.Date,
                        ProjectNumber = maxProjectNum,
                        TimeIn = timeCardVm.TimeIn == null ? null : replaceWithActualDate(timeCardVm.ActualDate, timeCardVm.TimeIn),
                        TimeOut = timeCardVm.TimeOut == null ? null : replaceWithActualDate(timeCardVm.ActualDate, timeCardVm.TimeOut),
                        LunchOut = timeCardVm.LunchOut == null ? null : replaceWithActualDate(timeCardVm.ActualDate, timeCardVm.LunchOut),
                        LunchBack = timeCardVm.LunchBack == null ? null : replaceWithActualDate(timeCardVm.ActualDate, timeCardVm.LunchBack),
                        HoursCodeId = timeCardVm.HoursCodeId,
                        Hours = timeCardVm.Hours,
                        EarningsCodeId = timeCardVm.EarningsCodeId,
                        EarningsAmount = timeCardVm.EarningsAmount,
                        TempDeptId = timeCardVm.TempDeptId,
                        TempJobId = timeCardVm.TempJobId,
                        IsApproved = timeCardVm.IsLineApproved,
                        //FundsId = timeCardVm.FundsId,
                        //ProjectsId = timeCardVm.ProjectsId,
                        PositionId = timeCardVm.PositionId,
                        FileNumber = _ilookupRepo.GetEmpFilenumber(employeeIdDdl),
                        DailyHours = dailyHours == 0 ? null : dailyHours,
                        //TaskId = timeCardVm.TaskId
                        IsDeleted = false,
                        UserId = userId,
                        EnteredBy = userId,
                        LastModifiedDate = timeUtc,
                        ApprovedBy = timeCardVm.IsLineApproved ? userId : null,
                        E_PositionId= epositionid
                    };

                    _context.TimeCards.Add(newTimeCardRecord);
                    timeCardsList.Add(newTimeCardRecord);

                }
            }
            _context.SaveChanges();
            return timeCardsList;
        }

        public List<TimeCardVm> UpdateTimeCardsList(IEnumerable<TimeCardVm> timeCardVmGridCollection, Dictionary<DateTime, int> Date_MaxProjectNum, int maxProjectNum, int employeeIdDdl, int? departmentId, int companyCodeIdDdl, string userId, int? payPeriodId)
        {
            var easternZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            var timeUtc = DateTime.Now;
            //DateTime lastModifiedDate = TimeZoneInfo.ConvertTime(timeUtc, easternZone);
            var timeCardsList = new List<TimeCardVm>();
            foreach (var timeCardVm in timeCardVmGridCollection)
            {
                if (timeCardVm.TimeIn != null || timeCardVm.TimeOut != null || timeCardVm.LunchOut != null || timeCardVm.LunchBack != null || timeCardVm.Hours > 0)
                {
                    var epositionlist = Query<TimeCardVm>("GetE_PositionIdbasedonpunchdate", new { @EmployeeId = employeeIdDdl, @positionId = timeCardVm.PositionId, @Companycodeid = companyCodeIdDdl, @payPeriodId = payPeriodId }).ToList();
                    var epositionid = epositionlist.Select(x => x.E_PositionId).FirstOrDefault();
                    var timeCardRecordInDb = _context.TimeCards.Where(x => x.TimeCardId == timeCardVm.TimeCardId).SingleOrDefault();
                    if (timeCardRecordInDb != null)
                    {
                        //if updated record 'Actualdate' not same as of existing database record 'Actualdate' entry 
                        if (timeCardVm.ActualDate.Date != timeCardRecordInDb.ActualDate.Date)
                        {
                            if (Date_MaxProjectNum.ContainsKey(timeCardVm.ActualDate.Date))
                            {
                                maxProjectNum = Convert.ToInt32(Date_MaxProjectNum[timeCardVm.ActualDate.Date]) + 1;

                                //update Dictionary
                                Date_MaxProjectNum[timeCardVm.ActualDate.Date] = Date_MaxProjectNum[timeCardVm.ActualDate.Date] + 1;

                                //Remove the key or decrement the projectNumber for the key in Dictionary if ActualDate changes
                                if (Date_MaxProjectNum.ContainsKey(timeCardRecordInDb.ActualDate))
                                {
                                    if (Date_MaxProjectNum[timeCardRecordInDb.ActualDate] == 1)
                                        Date_MaxProjectNum.Remove(timeCardRecordInDb.ActualDate);
                                }
                            }
                            else
                            {
                                maxProjectNum = 1;
                                Date_MaxProjectNum.Add(timeCardVm.ActualDate.Date, 1);
                            }
                        }
                        else
                        {
                            maxProjectNum = timeCardVm.ProjectNumber;
                        }
                        //TimeZone Start
                        //timeCardVm.ActualDate = TimeZoneInfo.ConvertTime(timeCardVm.ActualDate, easternZone);
                        //if (timeCardVm.TimeIn != null)
                        //{
                        //    timeCardVm.TimeIn = TimeZoneInfo.ConvertTime(timeCardVm.TimeIn.Value, easternZone);
                        //}
                        //if (timeCardVm.LunchOut != null)
                        //{
                        //    timeCardVm.LunchOut = TimeZoneInfo.ConvertTime(timeCardVm.LunchOut.Value, easternZone);
                        //}
                        //if (timeCardVm.LunchBack != null)
                        //{
                        //    timeCardVm.LunchBack = TimeZoneInfo.ConvertTime(timeCardVm.LunchBack.Value, easternZone);
                        //}
                        //if (timeCardVm.TimeOut != null)
                        //{
                        //    timeCardVm.TimeOut = TimeZoneInfo.ConvertTime(timeCardVm.TimeOut.Value, easternZone);
                        //}
                        var hours = dailyHourscalc(timeCardVm);
                        double? dailyHours = Math.Round(hours, 2);
                        //TimeZone End
                        //Corresponding Posirion DepartmentId
                        var positionDepartmentId = _context.E_Positions
                                                          .Where(x => x.PositionId == timeCardVm.PositionId && x.EmployeeId == employeeIdDdl)
                                                          .Select(d => d.DepartmentId).FirstOrDefault();
                        {
                            //timeCardRecordInDb.DepartmentId = departmentId;
                            timeCardRecordInDb.DepartmentId = positionDepartmentId;
                            timeCardRecordInDb.ActualDate = timeCardVm.ActualDate.Date;
                            timeCardRecordInDb.ProjectNumber = maxProjectNum;
                            //timeCardRecordInDb.DailyHours = dailyHourscalc(timeCardVm);
                            timeCardRecordInDb.TimeIn = timeCardVm.TimeIn == null ? null : replaceWithActualDate(timeCardVm.ActualDate, timeCardVm.TimeIn);
                            timeCardRecordInDb.TimeOut = timeCardVm.TimeOut == null ? null : replaceWithActualDate(timeCardVm.ActualDate, timeCardVm.TimeOut);
                            timeCardRecordInDb.LunchOut = timeCardVm.LunchOut == null ? null : replaceWithActualDate(timeCardVm.ActualDate, timeCardVm.LunchOut);
                            timeCardRecordInDb.LunchBack = timeCardVm.LunchBack == null ? null : replaceWithActualDate(timeCardVm.ActualDate, timeCardVm.LunchBack);
                            timeCardRecordInDb.HoursCodeId = timeCardVm.HoursCodeId == 0 ? null : timeCardVm.HoursCodeId;
                            timeCardRecordInDb.Hours = timeCardVm.Hours;
                            timeCardRecordInDb.EarningsCodeId = timeCardVm.EarningsCodeId == 0 ? null : timeCardVm.EarningsCodeId;
                            timeCardRecordInDb.EarningsAmount = timeCardVm.EarningsAmount;
                            timeCardRecordInDb.TempDeptId = timeCardVm.TempDeptId == 0 ? null : timeCardVm.TempDeptId;
                            timeCardRecordInDb.TempJobId = timeCardVm.TempJobId == 0 ? null : timeCardVm.TempJobId;
                            timeCardRecordInDb.PositionId = timeCardVm.PositionId == 0 ? null : timeCardVm.PositionId;
                            timeCardRecordInDb.IsApproved = timeCardVm.IsLineApproved;
                            //  timeCardRecordInDb.FundsId = timeCardVm.FundsId;
                            // timeCardRecordInDb.ProjectsId = timeCardVm.ProjectsId;
                            timeCardRecordInDb.FileNumber = _ilookupRepo.GetEmpFilenumber(employeeIdDdl);
                            // timeCardRecordInDb.TaskId = timeCardVm.TaskId;
                            timeCardRecordInDb.DailyHours = dailyHours == 0 ? null : dailyHours;
                            timeCardRecordInDb.IsDeleted = false;
                            timeCardRecordInDb.UserId = userId;
                            timeCardRecordInDb.LastModifiedDate = timeUtc;
                            timeCardRecordInDb.ApprovedBy = timeCardVm.IsLineApproved ? userId : null;
                            timeCardRecordInDb.DisApprovedBy = timeCardVm.IsLineApproved ? null : timeCardRecordInDb.DisApprovedBy;
                            timeCardRecordInDb.E_PositionId = epositionid;
                        };

                        TimeCardVm timeCardVmRecordInDb = new TimeCardVm
                        {
                            TimeCardId = timeCardVm.TimeCardId,
                            CompanyCodeId = companyCodeIdDdl,
                            EmployeeId = employeeIdDdl,
                            ActualDate = timeCardVm.ActualDate.Date,
                            ProjectNumber = maxProjectNum,
                            DailyHours = timeCardVm.DailyHours,
                            HoursCodeId = timeCardVm.HoursCodeId == 0 ? null : timeCardVm.HoursCodeId,
                            Hours = timeCardVm.Hours,
                            EarningsCodeId = timeCardVm.EarningsCodeId == 0 ? null : timeCardVm.EarningsCodeId,
                            EarningsAmount = timeCardVm.EarningsAmount,
                            TempDeptId = timeCardVm.TempDeptId == 0 ? null : timeCardVm.TempDeptId,
                            TempJobId = timeCardVm.TempJobId == 0 ? null : timeCardVm.TempJobId,
                            IsLineApproved = timeCardVm.IsLineApproved,
                            FundsId = timeCardVm.FundsId,
                            ProjectsId = timeCardVm.ProjectsId,
                            TaskId = timeCardVm.TaskId,
                            PositionId = timeCardVm.PositionId,
                            TimeIn = timeCardVm.TimeIn,
                            TimeOut = timeCardVm.TimeOut,
                            LunchOut = timeCardVm.LunchOut,
                            LunchBack = timeCardVm.LunchBack,
                            UserId = userId,
                            LastModifiedDate = timeUtc,
                            ApprovedBy = timeCardVm.IsLineApproved && timeCardVm.ApprovedBy == null ? userId : timeCardVm.IsLineApproved && timeCardVm.ApprovedBy != null ? timeCardVm.ApprovedBy : null,
                            E_PositionId= epositionid
                        };

                        timeCardsList.Add(timeCardVmRecordInDb);
                        _context.TimeCards.Attach(timeCardRecordInDb);
                        _context.Entry(timeCardRecordInDb).State = System.Data.Entity.EntityState.Modified;

                    }
                }
                _context.SaveChanges();
            }

            return timeCardsList;
        }

        public bool DeleteTimeCards(int timecardId, string userId)
        {
            bool result = false;
            try
            {
                TimeCard timeCardRecordInDb = _context.TimeCards
                    .Where(x => x.TimeCardId == timecardId).SingleOrDefault();
                if (timeCardRecordInDb != null)
                {
                    timeCardRecordInDb.IsDeleted = true;
                    timeCardRecordInDb.DeletedBy = userId;
                    timeCardRecordInDb.LastModifiedDate = DateTime.Now;
                    result = _context.SaveChanges() > 0 ? true : false;
                }
            }
            catch (Exception err)
            {
                string message = err.Message;
                return result;
            }
            return result;
        }
        public TimeCardDisplayColumn TimeCardInOutDisplayColumns(short typeId)
        {
            TimeCardDisplayColumn timeCardDislayColumns = new TimeCardDisplayColumn();   //Timecard columns

            timeCardDislayColumns = _context.TimeCardDisplayColumns
            .Where(x => x.TimeCardTypeId == typeId).FirstOrDefault();

            return (timeCardDislayColumns);
        }

        public bool checkEmployeeStatus(int departmentId, int employeeIdDdl)
        {
            bool result = _context.Employees
                              .Include("Person")
                              .Where(x => x.DepartmentId == departmentId && x.EmployeeId == employeeIdDdl)
                              .Select(x => x.DepartmentId).Any();
            return result;
        }

        public List<TimeCardWeekTotalCollectionVm> GetTimeCardWeekTotalList(int empid, int? departmentId, int payperiodId, bool isArchive)
        {
            List<TimeCardWeekTotalCollectionVm> empTimeCardList = new List<TimeCardWeekTotalCollectionVm>();

            empTimeCardList = Query<TimeCardWeekTotalCollectionVm>("sp_GetTimeCardWeekTotalList", new { @empId = empid, @isArchived = isArchive, @PayPeriodId = payperiodId }).ToList();

            foreach (var item in empTimeCardList)
            {
                if (item.RegularHours != null)
                    item.RegularHours = Math.Round(item.RegularHours.Value, 2);
                if (item.OverTime != null)
                    item.OverTime = Math.Round(item.OverTime.Value, 2);
                if (item.CodedHours != null)
                    item.CodedHours = Math.Round(item.CodedHours.Value, 2);
                else
                    item.CodedHours = 0;

                item.WeeklyTotal = item.RegularHours + item.CodedHours + item.OverTime;
                if (item.WeeklyTotal != null)
                    item.WeeklyTotal = Math.Round(item.WeeklyTotal.Value, 2);
            }

            //foreach (var item in empTimeCardList)
            //{
            //    if (item.CodedHours == null)
            //    {
            //        item.CodedHours = 0;
            //    }
            //    item.WeeklyTotal = item.RegularHours + item.OverTime + item.CodedHours;
            //}
            return empTimeCardList;
        }

        public Dictionary<DateTime, int> GetDateMaxProjectNum(int? employeeIdDdl, int? companyCodeIdDdl)
        {
            var dateMaxProjectNum = _context.TimeCards
                        .Where(x => x.EmployeeId == employeeIdDdl && x.CompanyCodeId == companyCodeIdDdl && x.IsDeleted == false)
                        .GroupBy(x => x.ActualDate)
                        .ToDictionary(x => x.Key, x => x.Max(g => g.ProjectNumber));
            return dateMaxProjectNum;
        }

        public PayPeriodVM GetPayPeriodDates(int? payPeriodId)
        {
            var payPeriod = (from pp in _context.PayPeriods
                             join ddl in _context.DdlPayFrequencies on pp.PayFrequencyId equals ddl.PayFrequencyId
                             where (pp.PayPeriodId == payPeriodId)
                             select new PayPeriodVM()

                             {
                                 PayFrequencyId = pp.PayFrequencyId,
                                 StartDate = pp.StartDate,
                                 EndDate = pp.EndDate,
                                 PayFrequencyName = ddl.Code

                             }).FirstOrDefault();
            return payPeriod;
        }

        private DateTime? replaceWithActualDate(DateTime actualDate, DateTime? actualDateTime)
        {
            actualDateTime = Convert.ToDateTime(actualDate.ToString().Substring(0, actualDate.ToString().IndexOf(" ")) +
                                     " " + actualDateTime.ToString().Remove(0, actualDateTime.ToString().IndexOf(" ")));
            if (actualDateTime != null)
            {
                actualDateTime = TimeZoneInfo.ConvertTimeToUtc(actualDateTime.Value, TimeZoneInfo.Utc);
            }
            return actualDateTime;
        }

        #endregion

        public double gettotalhours(int empid, DateTime PPstartdate, DateTime PPenddate)
        {
            double totalhours = 0;
            List<TimeCardWeekTotalCollectionVm> empTimeCardList = new List<TimeCardWeekTotalCollectionVm>();
            empTimeCardList = (from tc in _context.TimeCards
                               where (tc.EmployeeId == empid && (tc.ActualDate >= PPstartdate && tc.ActualDate <= PPenddate && tc.IsDeleted == false))
                               select new TimeCardWeekTotalCollectionVm
                               {
                                   RegularHours = tc.DailyHours,
                                   CodedHours = tc.Hours,
                               }).ToList();
            //
            //empTimeCardList = Query<TimeCardWeekTotalCollectionVm>("sp_GetTimeCardWeekTotalList", new { @empId = empid, @isArchived = isArchive, @PayPeriodId = payperiodId }).ToList();
            foreach (var item in empTimeCardList)
            {
                if (item.RegularHours == null)
                {
                    item.RegularHours = 0;
                }
                if (item.CodedHours == null)
                {
                    item.CodedHours = 0;
                }
                totalhours += item.RegularHours.Value + item.CodedHours.Value;
            }
            return totalhours;
        }

        public EmployeesVM GetFlsaCode(int empId)
        {

            EmployeesVM employeedetails = new EmployeesVM();
            //employeedetails = (from emp in _context.Employees
            //                               join ddf in _context.DddlFLSAs on emp.FLSAID equals ddf.FLSAID
            //                               join de in _context.DdlEmploymentStatuses on emp.EmploymentStatusId equals de.EmploymentStatusId
            //                               where(emp.EmployeeId== empId && emp.DdlEmploymentStatus.Code == "A")
            //                               select new EmployeesVM
            //                               {
            //                                   FlsaCode=ddf.Code,
            //                                   Hours=emp.Hours,
            //                               }).FirstOrDefault();

            return employeedetails;
        }

        public decimal? GetNonExemptEmployeeFTE(int employeeId)
        {
            var NonExemptEmployeeFTE = (from emp in _context.Employees
                                        join emppositions in _context.E_Positions on emp.EmployeeId equals emppositions.EmployeeId
                                        join position in _context.Positions on emppositions.PositionId equals position.PositionId
                                        where emppositions.PrimaryPosition == true && emp.EmployeeId == employeeId
                                        select position.FTE).SingleOrDefault();

            return NonExemptEmployeeFTE;
        }

        private double dailyHourscalc(TimeCardVm timecardsvm)
        {
            double dailyhours = 0;
            timecardsvm.TimeIn = timecardsvm.TimeIn == null ? null : replaceWithActualDate(timecardsvm.ActualDate, timecardsvm.TimeIn);
            timecardsvm.TimeOut = timecardsvm.TimeOut == null ? null : replaceWithActualDate(timecardsvm.ActualDate, timecardsvm.TimeOut);
            timecardsvm.LunchOut = timecardsvm.LunchOut == null ? null : replaceWithActualDate(timecardsvm.ActualDate, timecardsvm.LunchOut);
            timecardsvm.LunchBack = timecardsvm.LunchBack == null ? null : replaceWithActualDate(timecardsvm.ActualDate, timecardsvm.LunchBack);

            if (timecardsvm.TimeIn != null && timecardsvm.TimeOut != null)
            {
                dailyhours = (double)(timecardsvm.TimeOut.Value.Subtract(timecardsvm.TimeIn.Value).TotalHours);
                if (timecardsvm.LunchOut != null && timecardsvm.LunchBack != null)
                {
                    double lunchours = (double)(timecardsvm.LunchBack.Value.Subtract(timecardsvm.LunchOut.Value).TotalHours);
                    dailyhours = dailyhours - lunchours;
                }
            }
            else if (timecardsvm.Hours != null && timecardsvm.HoursCodeId != null)
            {
                dailyhours = (double)(timecardsvm.Hours);
            }
            return dailyhours;
        }
        public List<TimeCardVm> Grid_ReadChild(int employeeIdDdl, DateTime ActualDate)
        {
            List<TimeCardVm> empTimeCardList = new List<TimeCardVm>();
            empTimeCardList = (from tc in _context.TimeCards
                               where (tc.EmployeeId == employeeIdDdl && tc.ActualDate == ActualDate && tc.IsDeleted == false)
                               //where (tc.EmployeeId == employeeIdDdl)
                               select new TimeCardVm
                               {
                                   TimeCardId = tc.TimeCardId,
                                   IsApproved = tc.IsApproved,
                                   EmployeeId = employeeIdDdl,
                                   DepartmentId = (int)tc.DepartmentId,
                                   ActualDate = tc.ActualDate,
                                   TimeIn = tc.TimeIn,//== null ? null : replaceWithActualDate(tc.ActualDate, tc.TimeIn),
                                   TimeOut = tc.TimeOut,// == null ? null : replaceWithActualDate(tc.ActualDate, tc.TimeOut),
                                   LunchOut = tc.LunchOut,// == null ? null : replaceWithActualDate(tc.ActualDate, tc.LunchOut),
                                   LunchBack = tc.LunchBack, //== null ? null : replaceWithActualDate(tc.ActualDate, tc.LunchBack),
                                   HoursCodeId = tc.HoursCodeId,
                                   Hours = tc.Hours,
                                   EarningsCodeId = tc.EarningsCodeId,
                                   EarningsAmount = tc.EarningsAmount,
                                   TempDeptId = tc.TempDeptId,
                                   TempJobId = tc.TempJobId,
                                   // FundsId = timeCardVm.FundsId,
                                   // ProjectsId = timeCardVm.ProjectsId,
                                   PositionId = tc.PositionId,
                                   UserId = tc.UserId,
                                   DailyHours = tc.DailyHours == 0 ? null : tc.DailyHours,
                                   IsLineApproved = tc.IsApproved,
                                   E_PositionId=tc.E_PositionId
                               }).OrderBy(o => o.TimeIn).ToList();//.ToList();

            foreach (var emp in empTimeCardList)
            {
                emp.TimeIn = emp.TimeIn == null ? null : replaceWithActualDate(emp.ActualDate, emp.TimeIn);
                emp.TimeOut = emp.TimeOut == null ? null : replaceWithActualDate(emp.ActualDate, emp.TimeOut);
                emp.LunchOut = emp.LunchOut == null ? null : replaceWithActualDate(emp.ActualDate, emp.LunchOut);
                emp.LunchBack = emp.LunchBack == null ? null : replaceWithActualDate(emp.ActualDate, emp.LunchBack);
                // emp.DailyHours = dailyHourscalc(emp);

            }
            return empTimeCardList;
        }

        //Grid_ReadChildPoistionByManager
        public List<TimeCardVm> Grid_ReadChildPoistionByManager(int employeeIdDdl, int? payPeriodId, bool IsArchived, string loggedInUserId, DateTime actualDate)
        {
            var aspNetUsersEmail = _context.AspNetUsers.Where(x => x.UserName == loggedInUserId).Select(x => x.Email).FirstOrDefault();
            var @ReportToId = _context.Persons.Where(x => x.eMail == aspNetUsersEmail).Select(x => x.PersonId).FirstOrDefault();

            List<TimeCardCollectionVm> empTimeCardList = new List<TimeCardCollectionVm>();
            empTimeCardList = Query<TimeCardCollectionVm>("sp_GetTimeCardsListReportToPositions", new { @empId = employeeIdDdl, @isArchived = IsArchived, @PayPeriodId = payPeriodId, @ReportToId = @ReportToId }).Where(r => r.ActualDate == actualDate).ToList();

            var empTimeCardList1 = (from tc in empTimeCardList
                                    select new TimeCardVm
                                    {
                                        TimeCardId = tc.TimeCardId,
                                        EmployeeId = employeeIdDdl,
                                        DepartmentId = (int)tc.DepartmentId,
                                        ActualDate = tc.ActualDate,
                                        TimeIn = tc.TimeIn,//== null ? null : replaceWithActualDate(tc.ActualDate, tc.TimeIn),
                                        TimeOut = tc.TimeOut,// == null ? null : replaceWithActualDate(tc.ActualDate, tc.TimeOut),
                                        LunchOut = tc.LunchOut,// == null ? null : replaceWithActualDate(tc.ActualDate, tc.LunchOut),
                                        LunchBack = tc.LunchBack, //== null ? null : replaceWithActualDate(tc.ActualDate, tc.LunchBack),
                                        HoursCodeId = tc.HoursCodeId,
                                        Hours = tc.Hours,
                                        EarningsCodeId = tc.EarningsCodeId,
                                        EarningsAmount = tc.EarningsAmount,
                                        TempDeptId = tc.TempDeptId,
                                        TempJobId = tc.TempJobId,
                                        PositionId = tc.PositionId,
                                        UserId = tc.UserId,
                                        DailyHours = tc.DailyHours == 0 ? null : tc.DailyHours,
                                        IsLineApproved = tc.IsLineApproved,
                                        E_PositionId=tc.E_PositionId
                                    }).OrderBy(o => o.TimeIn).ToList();//.ToList();

            foreach (var emp in empTimeCardList)
            {
                emp.TimeIn = emp.TimeIn == null ? null : replaceWithActualDate(emp.ActualDate, emp.TimeIn);
                emp.TimeOut = emp.TimeOut == null ? null : replaceWithActualDate(emp.ActualDate, emp.TimeOut);
                emp.LunchOut = emp.LunchOut == null ? null : replaceWithActualDate(emp.ActualDate, emp.LunchOut);
                emp.LunchBack = emp.LunchBack == null ? null : replaceWithActualDate(emp.ActualDate, emp.LunchBack);
            }
            return empTimeCardList1;
        }
        public List<TimeCardVm> Grid_UpdateChild(IEnumerable<TimeCardVm> timeCardVmGridCollection, Dictionary<DateTime, int> Date_MaxProjectNum, int maxProjectNum, int? employeeIdDdl, int? companyCodeIdDdl, int? departmentId, int? payPeriodId, bool isArchived, string userName, DateTime actualDate)
        {
            var timeCardsList = new List<TimeCardVm>();
            var easternZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            var timeUtc = DateTime.UtcNow;
            DateTime lastModifiedDate = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);
            foreach (var timeCardVm in timeCardVmGridCollection)
            {
                if (timeCardVm.TimeIn != null || timeCardVm.TimeOut != null || timeCardVm.LunchOut != null || timeCardVm.LunchBack != null || timeCardVm.Hours > 0)
                {
                    var epositionlist = Query<TimeCardVm>("GetE_PositionIdbasedonpunchdate", new { @EmployeeId = employeeIdDdl, @positionId = timeCardVm.PositionId, @Companycodeid = companyCodeIdDdl, @payPeriodId = payPeriodId }).ToList();
                    var epositionid = epositionlist.Select(x => x.E_PositionId).FirstOrDefault();
                    var timeCardRecordInDb = _context.TimeCards.Where(x => x.TimeCardId == timeCardVm.TimeCardId).SingleOrDefault();
                    if (timeCardRecordInDb != null)
                    {
                        //if updated record 'Actualdate' not same as of existing database record 'Actualdate' entry 
                        if (actualDate.Date != timeCardRecordInDb.ActualDate.Date)
                        {
                            if (Date_MaxProjectNum.ContainsKey(actualDate.Date))
                            {
                                maxProjectNum = Convert.ToInt32(Date_MaxProjectNum[actualDate.Date]) + 1;

                                //update Dictionary
                                Date_MaxProjectNum[timeCardVm.ActualDate.Date] = Date_MaxProjectNum[actualDate.Date] + 1;

                                //Remove the key or decrement the projectNumber for the key in Dictionary if ActualDate changes
                                if (Date_MaxProjectNum.ContainsKey(actualDate.Date))
                                {
                                    if (Date_MaxProjectNum[timeCardRecordInDb.ActualDate] == 1)
                                        Date_MaxProjectNum.Remove(timeCardRecordInDb.ActualDate);
                                }
                            }
                            else
                            {
                                maxProjectNum = 1;
                                Date_MaxProjectNum.Add(actualDate.Date, 1);
                            }
                        }
                        else
                        {
                            maxProjectNum = timeCardVm.ProjectNumber;
                        }
                        var hours = dailyHourscalc(timeCardVm);
                        double? dailyHours = Math.Round(hours, 2);
                        //Corresponding Posirion DepartmentId
                        var positionDepartmentId = _context.E_Positions
                                                      .Where(x => x.PositionId == timeCardVm.PositionId && x.EmployeeId == employeeIdDdl)
                                                      .Select(d => d.DepartmentId).FirstOrDefault();
                        timeCardRecordInDb.IsApproved = timeCardVm.IsLineApproved;
                        timeCardRecordInDb.ApprovedBy = timeCardVm.IsLineApproved ? userName : null;
                        timeCardRecordInDb.DisApprovedBy = timeCardVm.IsLineApproved ? null : timeCardRecordInDb.DisApprovedBy;
                        //timeCardRecordInDb.DepartmentId = departmentId;
                        timeCardRecordInDb.DepartmentId = positionDepartmentId;
                        // timeCardRecordInDb.ActualDate = actualDate.Date;
                        timeCardRecordInDb.DailyHours = dailyHours == 0 ? null : dailyHours;
                        timeCardRecordInDb.TimeIn = timeCardVm.TimeIn == null ? null : replaceWithActualDate(actualDate.Date, timeCardVm.TimeIn);
                        timeCardRecordInDb.TimeOut = timeCardVm.TimeOut == null ? null : replaceWithActualDate(actualDate.Date, timeCardVm.TimeOut);
                        timeCardRecordInDb.LunchOut = timeCardVm.LunchOut == null ? null : replaceWithActualDate(actualDate.Date, timeCardVm.LunchOut);
                        timeCardRecordInDb.LunchBack = timeCardVm.LunchBack == null ? null : replaceWithActualDate(actualDate.Date, timeCardVm.LunchBack);
                        timeCardRecordInDb.HoursCodeId = timeCardVm.HoursCodeId == 0 ? null : timeCardVm.HoursCodeId;
                        timeCardRecordInDb.PositionId = timeCardVm.PositionId == 0 ? null : timeCardVm.PositionId;
                        timeCardRecordInDb.Hours = timeCardVm.Hours;
                        timeCardRecordInDb.IsDeleted = false;
                        timeCardRecordInDb.UserId = userName;
                        timeCardRecordInDb.LastModifiedDate = timeUtc;
                        timeCardRecordInDb.E_PositionId = epositionid;


                        TimeCardVm timeCardVmRecordInDb = new TimeCardVm
                        {
                            CompanyCodeId = companyCodeIdDdl ?? 0,
                            EmployeeId = employeeIdDdl ?? 0,
                            DepartmentId = departmentId,
                            ActualDate = actualDate.Date,
                            TimeIn = timeCardVm.TimeIn == null ? null : replaceWithActualDate(actualDate.Date, timeCardVm.TimeIn),
                            TimeOut = timeCardVm.TimeOut == null ? null : replaceWithActualDate(actualDate.Date, timeCardVm.TimeOut),
                            LunchOut = timeCardVm.LunchOut == null ? null : replaceWithActualDate(actualDate.Date, timeCardVm.LunchOut),
                            LunchBack = timeCardVm.LunchBack == null ? null : replaceWithActualDate(actualDate.Date, timeCardVm.LunchBack),
                            HoursCodeId = timeCardVm.HoursCodeId,
                            Hours = timeCardVm.Hours,
                            PositionId = timeCardVm.PositionId,
                            UserId = userName,
                            DailyHours = dailyHours == 0 ? null : dailyHours,
                            IsLineApproved = timeCardVm.IsLineApproved,
                            E_PositionId=timeCardVm.E_PositionId

                        };
                        timeCardsList.Add(timeCardVmRecordInDb);
                    }
                }
            }
            _context.SaveChanges();
            return timeCardsList;
        }

        public List<TimeCardVm> Grid_CreateChild(IEnumerable<TimeCardVm> timeCardVmGridCollection, Dictionary<DateTime, int> Date_MaxProjectNum, int maxProjectNum, int? employeeIdDdl, int? companyCodeIdDdl, int? departmentId, int? payPeriodId, bool isArchived, string userName, DateTime actualDate)
        {
            var easternZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            var timeUtc = DateTime.UtcNow;
            DateTime lastModifiedDate = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);
            var timeCardsList = new List<TimeCardVm>();
            foreach (var timeCardVm in timeCardVmGridCollection)
            {
                if (timeCardVm.TimeIn != null || timeCardVm.TimeOut != null || timeCardVm.LunchOut != null || timeCardVm.LunchBack != null || timeCardVm.Hours > 0)
                {
                    var epositionlist = Query<TimeCardVm>("GetE_PositionIdbasedonpunchdate", new { @EmployeeId = employeeIdDdl, @positionId = timeCardVm.PositionId, @Companycodeid = companyCodeIdDdl, @payPeriodId = payPeriodId }).ToList();
                    var epositionid = epositionlist.Select(x => x.E_PositionId).FirstOrDefault();
                    if (Date_MaxProjectNum.ContainsKey(actualDate.Date))
                    {
                        maxProjectNum = Convert.ToInt32(Date_MaxProjectNum[actualDate.Date]) + 1;

                        //update the Dictionary
                        Date_MaxProjectNum[actualDate.Date] = Date_MaxProjectNum[actualDate.Date] + 1; ;
                    }
                    else
                    {
                        maxProjectNum = 1;
                        //Add to Dictionary
                        Date_MaxProjectNum.Add(actualDate.Date, 1);
                    }
                    var hours = dailyHourscalc(timeCardVm);
                    double? dailyHours = Math.Round(hours, 2);
                    //TimeZone End
                    //Corresponding Posirion DepartmentId
                    var positionDepartmentId = _context.E_Positions
                                                      .Where(x => x.PositionId == timeCardVm.PositionId && x.EmployeeId == employeeIdDdl)
                                                      .Select(d => d.DepartmentId).FirstOrDefault();
                    var newTimeCardRecord = new TimeCard
                    {
                        CompanyCodeId = companyCodeIdDdl ?? 0,
                        EmployeeId = employeeIdDdl ?? 0,
                        //DepartmentId = departmentId,
                        DepartmentId = positionDepartmentId,
                        ActualDate = actualDate.Date,
                        ProjectNumber = maxProjectNum,
                        TimeIn = timeCardVm.TimeIn == null ? null : replaceWithActualDate(actualDate.Date, timeCardVm.TimeIn),
                        TimeOut = timeCardVm.TimeOut == null ? null : replaceWithActualDate(actualDate.Date, timeCardVm.TimeOut),
                        LunchOut = timeCardVm.LunchOut == null ? null : replaceWithActualDate(actualDate.Date, timeCardVm.LunchOut),
                        LunchBack = timeCardVm.LunchBack == null ? null : replaceWithActualDate(actualDate.Date, timeCardVm.LunchBack),
                        HoursCodeId = timeCardVm.HoursCodeId,
                        Hours = timeCardVm.Hours,
                        EarningsCodeId = timeCardVm.EarningsCodeId,
                        EarningsAmount = timeCardVm.EarningsAmount,
                        TempDeptId = timeCardVm.TempDeptId,
                        TempJobId = timeCardVm.TempJobId,
                        IsApproved = timeCardVm.IsLineApproved,
                        //FundsId = timeCardVm.FundsId,
                        //ProjectsId = timeCardVm.ProjectsId,
                        PositionId = timeCardVm.PositionId,
                        FileNumber = _ilookupRepo.GetEmpFilenumber(employeeIdDdl ?? 0),
                        DailyHours = dailyHours == 0 ? null : dailyHours,
                        //TaskId = timeCardVm.TaskId
                        IsDeleted = false,
                        UserId = userName,
                        EnteredBy = userName,
                        LastModifiedDate = timeUtc,
                        ApprovedBy = timeCardVm.IsLineApproved ? userName : null,
                        E_PositionId=epositionid
                    };

                    _context.TimeCards.Add(newTimeCardRecord);
                    _context.SaveChanges();

                    TimeCardVm timeCardVmRecordInDb = new TimeCardVm
                    {
                        CompanyCodeId = companyCodeIdDdl ?? 0,
                        EmployeeId = employeeIdDdl ?? 0,
                        DepartmentId = departmentId,
                        ActualDate = actualDate.Date,
                        TimeIn = timeCardVm.TimeIn == null ? null : replaceWithActualDate(actualDate.Date, timeCardVm.TimeIn),
                        TimeOut = timeCardVm.TimeOut == null ? null : replaceWithActualDate(actualDate.Date, timeCardVm.TimeOut),
                        LunchOut = timeCardVm.LunchOut == null ? null : replaceWithActualDate(actualDate.Date, timeCardVm.LunchOut),
                        LunchBack = timeCardVm.LunchBack == null ? null : replaceWithActualDate(actualDate.Date, timeCardVm.LunchBack),
                        HoursCodeId = timeCardVm.HoursCodeId,
                        Hours = timeCardVm.Hours,
                        PositionId = timeCardVm.PositionId,
                        UserId = userName,
                        DailyHours = dailyHours == 0 ? null : dailyHours,
                        IsLineApproved = timeCardVm.IsLineApproved,
                        E_PositionId=epositionid
                    };
                    timeCardsList.Add(timeCardVmRecordInDb);
                }
            }
            return timeCardsList;
        }



        public List<TimeCardCollectionVm> GetEpositionID(int? employeeIdDdl, int? PositionId, int? companycodeId ,int? payPeriodId)
        {
            List<TimeCardCollectionVm> empTimeCardList = new List<TimeCardCollectionVm>();
            empTimeCardList = Query<TimeCardCollectionVm>("GetE_PositionIdList", new { @EmployeeId = employeeIdDdl, @positionId = PositionId, @Companycodeid = companycodeId, @payPeriodId = payPeriodId }).ToList();
            
            return empTimeCardList;
        }
        #region Timeoff Summary

        public List<TimeoffSummaryVM> GetTimeoffSummaryList(int? employeeIdDdl)
        {
            List<TimeoffSummaryVM> timeoffSummaryVM = new List<TimeoffSummaryVM>();
            try
            {
                if (employeeIdDdl != 0)
                {
                    var timeCardsTotal = (from t in _context.TimeCards
                                          where t.EmployeeId == employeeIdDdl && t.IsDeleted == false
                                          select new
                                          {
                                              Total = (t.DailyHours != null && t.Hours != null) ? t.DailyHours + t.Hours : (t.DailyHours == null && t.Hours != null) ? t.Hours : t.DailyHours,
                                              PositionId = t.PositionId,
                                          }).ToList();
                    if (timeCardsTotal.Count != 0)
                    {
                        var groupbyPositionData = timeCardsTotal.GroupBy(p => p.PositionId, p => p.Total, (key, g) => new { PositionId = key, Total = g.Sum() });

                        foreach (var item in groupbyPositionData)
                        {
                            if (item.PositionId != null)
                            {
                                var timedata = (from ep in _context.E_Positions
                                                join sh in _context.E_PositionSalaryHistories on ep.E_PositionId equals sh.E_PositionId
                                                join p in _context.Positions on ep.PositionId equals p.PositionId
                                                where ep.PositionId == item.PositionId && ep.EmployeeId == employeeIdDdl && sh.EffectiveDate != null
                                                select new
                                                {
                                                    PayRate = sh.PayRate,
                                                    PositionDescription = p.PositionDescription+"-"+p.Suffix,
                                                    AdpWSLimit = ep.AdpWSLimit
                                                }).FirstOrDefault();
                                //var empAdp = _context.Employees.Where(x => x.EmployeeId == employeeIdDdl).Select(y => y.AdpWSLimit).FirstOrDefault();
                                if (timedata != null)
                                {
                                    var taken = Math.Round(timedata.PayRate.Value * Convert.ToDecimal(item.Total), 2);
                                    TimeoffSummaryVM timeoffSummary = new TimeoffSummaryVM
                                    {
                                        Position = timedata.PositionDescription,
                                        Allowed = timedata.AdpWSLimit ?? 0,
                                        Taken = taken,
                                        Remainder = timedata.AdpWSLimit != null ? timedata.AdpWSLimit.Value - taken : -taken
                                    };
                                    timeoffSummaryVM.Add(timeoffSummary);
                                }
                            }
                        }
                    }
                    else
                    {
                        var timedata = (from ep in _context.E_Positions
                                        join p in _context.Positions on ep.PositionId equals p.PositionId
                                        where ep.EmployeeId == employeeIdDdl
                                        select new
                                        {
                                            PositionDescription = p.PositionDescription+"-"+p.Suffix,
                                            AdpWSLimit = ep.AdpWSLimit
                                        }).ToList();
                        foreach (var item in timedata)
                        {

                            TimeoffSummaryVM timeoffSummary = new TimeoffSummaryVM
                            {
                                Position = item.PositionDescription,
                                Allowed = item.AdpWSLimit ?? 0,
                                Taken = 0,
                                Remainder = 0
                            };
                            timeoffSummaryVM.Add(timeoffSummary);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
            return timeoffSummaryVM;
        }
        #endregion

    }
}
