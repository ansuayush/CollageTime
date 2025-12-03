using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExecViewHrk.Domain.Repositories
{
    public class TimeCardAuditsRepository : RepositoryBase, ITimeCardAuditsRepository
    {
        public List<TimeCardAuditsVM> GetTimecardAuditList()
        {
            List<TimeCardAuditsVM> timecardAuditList = new List<TimeCardAuditsVM>();

            try
            {
                timecardAuditList = (from ta in _context.TimeCardsAudits
                                     join emp in _context.Employees on ta.EmployeeId equals emp.EmployeeId
                                     join per in _context.Persons on emp.PersonId equals per.PersonId
                                     join p in _context.Positions on ta.PositionId equals p.PositionId
                                     select new TimeCardAuditsVM
                                     {
                                         AuditId = ta.AuditId,
                                         EmployeeId = ta.EmployeeId,
                                         EmployeeFullName = per.Firstname + " " + per.Lastname,
                                         FileNumber = ta.FileNumber,
                                         ActualDate = ta.ActualDate,
                                         TimeCardId = ta.TimeCardId,
                                         TimeIn = ta.TimeIn,
                                         TimeOut = ta.TimeOut,
                                         Position = p.PositionDescription,
                                         AuditType = ta.AuditType,
                                         //AuditRecType = ta.AuditRecType,
                                         AuditDate = ta.AuditDate,
                                         AuditUserId = ta.AuditUserId,
                                         CompanyCodeId = ta.CompanyCodeId,
                                     }).OrderByDescending(m => m.AuditDate).ToList();
                string timeZone = System.Configuration.ConfigurationManager.AppSettings["TimeZone"].ToString();
                var easternZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                foreach (var item in timecardAuditList)
                {
                    item.AuditDate = TimeZoneInfo.ConvertTimeFromUtc(item.AuditDate.Value, easternZone);
                }
                var cc = _context.CompanyCodes.ToList();
                foreach(var companycode in timecardAuditList)
                {
                    companycode.CompanyCodeDescription = cc.Where(x => x.CompanyCodeId == companycode.CompanyCodeId).FirstOrDefault().CompanyCodeDescription;
                }

            }

            catch (Exception ex)
            {
                string message = ex.Message;
            }

            return timecardAuditList;
        }
    }
}
