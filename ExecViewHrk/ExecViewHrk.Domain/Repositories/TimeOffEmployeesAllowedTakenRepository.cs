using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Models;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using ExecViewHrk.EfClient;
using System;

namespace ExecViewHrk.Domain.Repositories
{
    public class TimeOffEmployeesAllowedTakenRepository : RepositoryBase, ITimeOffEmployeesAllowedTakenRepository
    {
        public bool InsertEmployeesAllowedTaken(List<TimeOffEmployeesAllowedTakenVM> timeOffEmployeesAllowedTakenVM)
        {
            bool result = false;
            foreach (var item in timeOffEmployeesAllowedTakenVM)
            {
                var recordinDb = _context.EmployeesAllowedTakens.Where(x => x.EmployeeId == item.EmployeeId && x.TypeId == item.TypeId).FirstOrDefault();
                if (recordinDb == null)
                {
                    var timeOffEmployeesAllowedTaken = new EmployeesAllowedTakens()
                    {
                        //EmployeesAllowedTakenId = item.EmployeesAllowedTakenId,              
                        EmployeeId = item.EmployeeId,
                        CompanyCodeId = item.CompanyCodeId,
                        FileNumber = item.FileNumber,
                        TypeId = item.TypeId,
                        Allowed = item.Allowed,
                        Taken = item.Taken,
                        Remainder = item.Remainder
                    };
                    _context.EmployeesAllowedTakens.Add(timeOffEmployeesAllowedTaken);
                }
            }
            _context.SaveChanges();
            result = true;
            return result;
        }

        public List<TimeoffSummaryVM> GetTimeoffSummaryList()
        {
            var timeoffSummaryList = new List<TimeoffSummaryVM>();

            try
            {
                timeoffSummaryList = (from emp in _context.EmployeesAllowedTakens
                                      join p in _context.Positions on emp.TypeId equals p.PositionId
                                      select new TimeoffSummaryVM()
                                      {
                                          EmployeesAllowedTakenId = emp.EmployeesAllowedTakenId,
                                          EmployeeId = emp.EmployeeId,
                                          Allowed = emp.Allowed,
                                          TypeId = emp.TypeId,
                                          Typecode = p.PositionDescription,
                                          Taken = emp.Taken,
                                          Remainder = emp.Remainder
                                      }).ToList();
            }

            catch (Exception ex)
            {
                string message = ex.Message;
            }
            return timeoffSummaryList;
        }

        public List<TimeoffSummaryVM> UpdateEmployeesAllowedTaken(int? employeeIdDdl)
        {
            List<TimeoffSummaryVM> timeoffSummaryVM = new List<TimeoffSummaryVM>();
            var timeCardsTotal = (from t in _context.TimeCards
                                  where t.EmployeeId == employeeIdDdl && t.IsDeleted == false
                                  select new
                                  {
                                      Total = (t.DailyHours != null && t.Hours != null) ? t.DailyHours + t.Hours : (t.DailyHours == null && t.Hours != null) ? t.Hours : t.DailyHours,
                                      PositionId = t.PositionId,
                                  }).ToList();
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
                                        PositionDescription = p.PositionDescription
                                    }).FirstOrDefault();
                    var empAdp = _context.Employees.Where(x => x.EmployeeId == employeeIdDdl).Select(y => y.AdpWSLimit).FirstOrDefault();
                    if (timedata.PayRate != null && empAdp != null)
                    {
                        var taken = timedata.PayRate.Value * Convert.ToDecimal(item.Total);
                        TimeoffSummaryVM timeoffSummary = new TimeoffSummaryVM
                        {
                            Position = timedata.PositionDescription,
                            Allowed = empAdp,
                            Taken = taken,
                            Remainder = empAdp.Value - taken
                        };
                        timeoffSummaryVM.Add(timeoffSummary);
                    }
                }
            }

            return timeoffSummaryVM;
        }
    }
}
