using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Domain.Repositories
{
    public class TimeReportRepository : RepositoryBase, ITimeReportRepository
    {

        public List<TimeReportVm> GetTimeReportList(DateTime startDate, DateTime endDate)
        {
            try
            {
                var list = (from tcd in _context.TimeCards
                            join emp in _context.Employees on tcd.EmployeeId equals emp.EmployeeId
                            join prsn in _context.Persons on emp.PersonId equals prsn.PersonId
                            join ep in _context.E_Positions on emp.EmployeeId equals ep.EmployeeId
                            join ps in _context.Positions on ep.PositionId equals ps.PositionId
                            where (tcd.ActualDate >= startDate && tcd.ActualDate <= endDate) && tcd.IsDeleted == false
                            select new TimeReportVm
                            {
                                ActualDate = tcd.ActualDate,
                                EmployeeId = tcd.EmployeeId,
                                Firstname = prsn.Firstname,
                                Lastname = prsn.Lastname,
                                LunchBack = tcd.LunchBack,
                                LunchOut = tcd.LunchOut,
                                TimeIn = tcd.TimeIn,
                                TimeOut = tcd.TimeOut,
                                UserId = tcd.UserId,
                                FileNumber = emp.FileNumber,
                                PositionDescription = ps.PositionDescription,
                                DailyHours = tcd.DailyHours
                            }).ToList();
                return list ?? new List<TimeReportVm>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<TimeReportVm> GetEmpTimeReportList(DateTime startDate, DateTime endDate)
        {
            List<TimeReportVm> empTimeReportList = new List<TimeReportVm>();
            empTimeReportList = Query<TimeReportVm>("GetTimeReports", new { @startdate = startDate, @enddate = endDate }).ToList();
            return empTimeReportList ?? new List<TimeReportVm>();
        }
    }
}
