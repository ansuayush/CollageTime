using System;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExecViewHrk.Models;
using ExecViewHrk.EfClient;
using ExecViewHrk.Domain.Interface;
using ExecViewHrk.WebUI.Models;
using ExecViewHrk.Domain.Models;
using System.Security.Principal;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.SqlServer;
using System.Data;


namespace ExecViewHrk.Domain.Repositories
{
    public class TimeCardArchiveRerpository : RepositoryBase, ITimeCardArchiveRerpository
    {
        public List<TimeCardsArchiveVM> GetTimaCardarchiveList(int EmployeeId, int payperiodId)
        {

            List<TimeCardsArchiveVM> timecardArchiveList = new List<TimeCardsArchiveVM>();
            timecardArchiveList = Query<TimeCardsArchiveVM>("sp_GetEmpWeeklyTimeCardArchive", new { @PayPeriodId = payperiodId, @empid = EmployeeId }).ToList();
            return timecardArchiveList;
        }


        public List<TimeCardWeekTotalCollectionVm> GetWeeklyTotalTimaCardArchiveList(int EmployeeId, int payperiodId)
        {

            List<TimeCardWeekTotalCollectionVm> empTimeCardList = new List<TimeCardWeekTotalCollectionVm>();
            empTimeCardList = Query<TimeCardWeekTotalCollectionVm>("sp_GetEmpWeeklyTotalTimeCardArchive", new { @empid = EmployeeId, @PayPeriodId = payperiodId }).ToList();
            foreach (var item in empTimeCardList)
            {
                item.CodedHours = 0;
                if (item.RegularHours != null)
                    item.RegularHours = Math.Round(item.RegularHours.Value, 2);
                if (item.OverTime != null)
                    item.OverTime = Math.Round(item.OverTime.Value, 2);
                item.WeeklyTotal = item.RegularHours + item.OverTime;
                if (item.WeeklyTotal != null)
                    item.WeeklyTotal = Math.Round(item.WeeklyTotal.Value, 2);
            }
            return empTimeCardList;
        }



        public List<PayPeriodVM> GetPayPeriodsList(int? EmployeeIdDdl, bool IsArchived)
        {
            List<PayPeriodVM> payPeriodsList = new List<PayPeriodVM>();
            int? payfrequencyid = _context.Employees.Where(x => x.EmployeeId == EmployeeIdDdl).Select(x => x.PayFrequencyId).FirstOrDefault();
            if (payfrequencyid != null)
            {
                payPeriodsList = _context.PayPeriods
                 // .Where(m => m.CompanyCodeId == CompanyCodeIdDdl && m.IsArchived == IsArchived)
                 .Where(m => m.PayFrequencyId == payfrequencyid && m.IsArchived == IsArchived)
                  .Select(m => new PayPeriodVM
                  {
                      PayPeriodId = m.PayPeriodId,
                      PayPeriod = SqlFunctions.StringConvert((double)SqlFunctions.DatePart("mm", m.StartDate)).Trim() + "/" + SqlFunctions.DateName("DAY", m.StartDate) + "/" + SqlFunctions.DateName("YYYY", m.StartDate)
                                 + " - " + SqlFunctions.StringConvert((double)SqlFunctions.DatePart("mm", m.EndDate)).Trim() + "/" + SqlFunctions.DateName("DAY", m.EndDate) + "/" + SqlFunctions.DateName("YYYY", m.EndDate)
                  }).OrderByDescending(m => m.PayPeriodId).Take(6).ToList();
            }
            return payPeriodsList;

        }

        #region Timeoff Summary

        public List<TimeoffSummaryVM> GetTimeoffSummaryList(int? employeeIdDdl)
        {
            List<TimeoffSummaryVM> timeoffSummaryVM = new List<TimeoffSummaryVM>();
            try
            {
                if (employeeIdDdl != 0)
                {
                    var timeCardsTotal = (from t in _context.TimeCardsArchive
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
                                                where ep.PositionId == item.PositionId && ep.EmployeeId == employeeIdDdl && sh.EffectiveDate != null && ep.IsDeleted == false
                                                select new
                                                {
                                                    PayRate = sh.PayRate,
                                                    PositionDescription = p.PositionDescription,
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
                                            PositionDescription = p.PositionDescription,
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

        public TimeCardsNotesVM GetTimecardArchiveNotes(int? timecardId)
        {
            TimeCardsNotesVM timeCardsNotesVM = new TimeCardsNotesVM();

            try
            {
                timeCardsNotesVM = _context.TimeCardNotesArchives.Where(m => m.TimeCardId == timecardId)
                    .Select(x => new TimeCardsNotesVM
                    {
                        EmployeeId = x.EmployeeId,
                        CompanyCodeId = x.CompanyCodeId,
                        FileNumber = x.FileNumber,
                        TimeCardId = timecardId.Value,
                        ActualDate = x.ActualDate,
                        Notes = x.Notes
                    }).FirstOrDefault();
                if (timeCardsNotesVM == null)
                {
                    timeCardsNotesVM = _context.TimeCardsArchive.Where(m => m.TimeCardId == timecardId)
                        .Select(x => new TimeCardsNotesVM
                        {
                            EmployeeId = x.EmployeeId,
                            CompanyCodeId = x.CompanyCodeId,
                            FileNumber = x.FileNumber,
                            TimeCardId = timecardId,
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

    }
}
