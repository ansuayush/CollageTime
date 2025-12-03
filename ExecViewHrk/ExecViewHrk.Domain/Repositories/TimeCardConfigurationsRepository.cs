using ExecViewHrk.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExecViewHrk.Domain.Models;
using ExecViewHrk.Models;

namespace ExecViewHrk.Domain.Repositories
{
    public class TimeCardConfigurationsRepository : RepositoryBase, ITimeCardConfigurationsRepository
    {

        public List<TimeCardDisplayColumnVM> GetTimeCardColumnsList()
        {
            try
            {
                var list = _context.TimeCardDisplayColumns
                    .Select(tc => new TimeCardDisplayColumnVM()
                    {
                        TimeCardTypeId = tc.TimeCardTypeId,
                        ActualDate = tc.ActualDate,
                        Day = tc.Day,
                        DailyHours = tc.DailyHours,
                        HoursCodeId = tc.HoursCodeId,
                        Hours = tc.Hours,
                        EarningsCodeId = tc.EarningsCodeId,
                        EarningsAmount = tc.EarningsAmount,
                        TempDeptId = tc.TempDeptId,
                        TempJobId = tc.TempJobId,
                        TimeIn = tc.TimeIn,
                        LunchOut = tc.LunchOut,
                        LunchBack = tc.LunchBack,
                        TimeOut = tc.TimeOut,
                        Total = tc.Total,
                        IsApproved = tc.IsApproved,
                        AutoFill = tc.AutoFill
                    }).OrderBy(e => e.TimeCardTypeId).ToList();
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TimeCardDisplayColumnVM GetTimeCardColumnsById(int timeCardTypeId)
        {
            TimeCardDisplayColumnVM timeCardDisplayColumnVM = new TimeCardDisplayColumnVM();
            try
            {
                timeCardDisplayColumnVM = _context.TimeCardDisplayColumns.Where(tc => tc.TimeCardTypeId == timeCardTypeId).Select(tc => new
                TimeCardDisplayColumnVM()
                {
                    TimeCardTypeId = tc.TimeCardTypeId,
                    ActualDate = tc.ActualDate,
                    Day = tc.Day,
                    DailyHours = tc.DailyHours,
                    HoursCodeId = tc.HoursCodeId,
                    Hours = tc.Hours,
                    EarningsCodeId = tc.EarningsCodeId,
                    EarningsAmount = tc.EarningsAmount,
                    TempDeptId = tc.TempDeptId,
                    TempJobId = tc.TempJobId,
                    TimeIn = tc.TimeIn,
                    LunchOut = tc.LunchOut,
                    LunchBack = tc.LunchBack,
                    TimeOut = tc.TimeOut,
                    Total = tc.Total,
                    IsApproved = tc.IsApproved,
                    AutoFill = tc.AutoFill

                }).FirstOrDefault();
                
                return timeCardDisplayColumnVM;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public bool TimeCardColumnsList_Update(TimeCardDisplayColumnVM timecardColumn)
        {
            var status = false;
            var timecardColumnsInDb = _context.TimeCardDisplayColumns
                        .Where(x => x.TimeCardTypeId == timecardColumn.TimeCardTypeId)
                        .SingleOrDefault();
            try
            {
                if (timecardColumnsInDb != null)
                {
                    timecardColumnsInDb.Day = timecardColumn.Day;
                    timecardColumnsInDb.ActualDate = timecardColumn.ActualDate;
                    timecardColumnsInDb.DailyHours = timecardColumn.DailyHours;
                    timecardColumnsInDb.HoursCodeId = timecardColumn.HoursCodeId;
                    timecardColumnsInDb.Hours = timecardColumn.Hours;
                    timecardColumnsInDb.EarningsCodeId = timecardColumn.EarningsCodeId;
                    timecardColumnsInDb.EarningsAmount = timecardColumn.EarningsAmount;
                    timecardColumnsInDb.TempDeptId = timecardColumn.TempDeptId;
                    timecardColumnsInDb.TempJobId = timecardColumn.TempJobId;
                    timecardColumnsInDb.TimeIn = timecardColumn.TimeIn;
                    timecardColumnsInDb.LunchOut = timecardColumn.LunchOut;
                    timecardColumnsInDb.LunchBack = timecardColumn.LunchBack;
                    timecardColumnsInDb.TimeOut = timecardColumn.TimeOut;
                    timecardColumnsInDb.IsApproved = timecardColumn.IsApproved;
                    timecardColumnsInDb.Total = timecardColumn.Total;
                    timecardColumnsInDb.AutoFill = timecardColumn.AutoFill;
                    _context.SaveChanges();
                    status = true;
                }
            }
            catch(Exception ex)
            {
                string message = ex.Message;
            }
            return status;
        }
        
        public List<TimeCardTypeVM> PopulateTimeCardTypes()
        {
            List<TimeCardTypeVM> timecardTypeList = new List<TimeCardTypeVM>();
            try
            {
                timecardTypeList = _context.DdlTimeCardTypes
                    //.Include(x => x.CompanyCodeCode)
                    .Select(s => new TimeCardTypeVM
                    {
                        timeCardTypeId = s.TimeCardTypeId,
                        timeCardTypeCode_Description = s.TimeCardTypeCode + "-" + s.TimeCardTypeDescription
                    }).Distinct().OrderBy(x => x.timeCardTypeCode_Description).ToList();
                return timecardTypeList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
