using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Models;
using ExecViewHrk.EfClient;
using System.Data.Entity.Validation;

namespace ExecViewHrk.Domain.Repositories
{
    public class PositionBudgetSchedulesRepository :RepositoryBase,IPositionBudgetSchedulesRepository
    {
        public List<PositionBudgetSchedulesVM> getpositionBudgetSchedulesList()
        {
            var budgetScheduleList = _context.PositionBudgetSchedules.Select(m => new PositionBudgetSchedulesVM
            {
                ID = m.ID,
                EffectiveDate = m.EffectiveDate,
                StartDate = m.StartDate,
                EndDate = m.EndDate,
                EligibleDate = m.EligibleDate,
                IncreaseType = m.IncreaseType,
                IncreaseAmount = m.IncreaseAmount,
                WashoutRule = m.WashoutRule,
                WashoutRuleSalary = m.WashoutRuleSalary,
                AutoFill = m.AutoFill,
                ScheduleType = m.ScheduleType,
                AutoFillYesNo = (m.AutoFill == true ? "YES" : "NO"),
                IncreaseTypeText = (m.IncreaseType == 0 ? "Percent" : "Dollars"),
                ScheduleTypeText = (m.ScheduleType == 0) ? "Salary" : (m.ScheduleType == 1) ? "Hourly" : (m.ScheduleType == 2) ? "Both" : "Unknown"
            }).
            OrderByDescending(e => e.EffectiveDate).ToList();
            return budgetScheduleList;
        }

        public PositionBudgetSchedulesVM getpositionBudgetSchedulesDetails(int ID) {

            var positionBudgetSchedulesVM = _context.PositionBudgetSchedules.Select(m => new PositionBudgetSchedulesVM
            {
                ID = m.ID,
                EffectiveDate = m.EffectiveDate,
                StartDate = m.StartDate,
                EndDate = m.EndDate,
                EligibleDate = m.EligibleDate,
                IncreaseType = m.IncreaseType,
                IncreaseAmount = m.IncreaseAmount,
                WashoutRule = m.WashoutRule,
                WashoutRuleSalary = m.WashoutRuleSalary,
                AutoFill = m.AutoFill,
                ScheduleType = m.ScheduleType,
                AutoFillYesNo = (m.AutoFill == true ? "YES" : "NO"),
                IncreaseTypeText = (m.IncreaseType == 0 ? "Percent" : "Dollars"),
                ScheduleTypeText = (m.ScheduleType == 0) ? "Salary" : (m.ScheduleType == 1) ? "Hourly" : (m.ScheduleType == 2) ? "Both" : "Unknown"
            }).Where(m => m.ID == ID).FirstOrDefault();
            return positionBudgetSchedulesVM;
        }

        public void deletepositionBudgetSchedule(int ID)
        {
            var dbRecord = _context.PositionBudgetSchedules.Where(x => x.ID == ID).SingleOrDefault();
            if (dbRecord != null)
            {
                _context.PositionBudgetSchedules.Remove(dbRecord);

                _context.SaveChanges();
            }            
        }

        public PositionBudgetSchedulesVM positionBudgetSchedulesSave(PositionBudgetSchedulesVM positionBudgetSchedulesVM)
        {
            PositionBudgetSchedules positionBudgetSchedulesEFClient = _context.PositionBudgetSchedules.Where(m => m.ID == positionBudgetSchedulesVM.ID).SingleOrDefault();                    
            if (positionBudgetSchedulesEFClient == null)
            {                
                positionBudgetSchedulesEFClient = new PositionBudgetSchedules();
                positionBudgetSchedulesEFClient.ScheduleType = positionBudgetSchedulesVM.ScheduleType;
                positionBudgetSchedulesEFClient.StartDate = positionBudgetSchedulesVM.StartDate;
                positionBudgetSchedulesEFClient.EndDate = positionBudgetSchedulesVM.EndDate;
                positionBudgetSchedulesEFClient.EffectiveDate = positionBudgetSchedulesVM.EffectiveDate;
                positionBudgetSchedulesEFClient.EligibleDate = positionBudgetSchedulesVM.EligibleDate;
                positionBudgetSchedulesEFClient.IncreaseType = positionBudgetSchedulesVM.IncreaseType;
                positionBudgetSchedulesEFClient.IncreaseAmount = positionBudgetSchedulesVM.IncreaseAmount;
                positionBudgetSchedulesEFClient.WashoutRule = positionBudgetSchedulesVM.WashoutRule;
                positionBudgetSchedulesEFClient.WashoutRuleSalary = positionBudgetSchedulesVM.WashoutRuleSalary;
                positionBudgetSchedulesEFClient.AutoFill = positionBudgetSchedulesVM.AutoFill;
                _context.PositionBudgetSchedules.Add(positionBudgetSchedulesEFClient);
            }
            else
            {
                positionBudgetSchedulesEFClient.ScheduleType = positionBudgetSchedulesVM.ScheduleType;
                positionBudgetSchedulesEFClient.StartDate = positionBudgetSchedulesVM.StartDate;
                positionBudgetSchedulesEFClient.EndDate = positionBudgetSchedulesVM.EndDate;
                positionBudgetSchedulesEFClient.EffectiveDate = positionBudgetSchedulesVM.EffectiveDate;
                positionBudgetSchedulesEFClient.EligibleDate = positionBudgetSchedulesVM.EligibleDate;
                positionBudgetSchedulesEFClient.IncreaseType = positionBudgetSchedulesVM.IncreaseType;
                positionBudgetSchedulesEFClient.IncreaseAmount = positionBudgetSchedulesVM.IncreaseAmount;
                positionBudgetSchedulesEFClient.WashoutRule = positionBudgetSchedulesVM.WashoutRule;
                positionBudgetSchedulesEFClient.WashoutRuleSalary = positionBudgetSchedulesVM.WashoutRuleSalary;
                positionBudgetSchedulesEFClient.AutoFill = positionBudgetSchedulesVM.AutoFill;
            }           
            _context.SaveChanges();    
            return positionBudgetSchedulesVM;
        }

        public PositionBudgetSchedulesVM GetRecordForEffectiveDateAndScheduleType(DateTime? dateEffective, byte? scheduleType)
        {
            var positionBudgetSchedulesVM = _context.PositionBudgetSchedules.Select(m => new PositionBudgetSchedulesVM
            {
                ID = m.ID,
                EffectiveDate = m.EffectiveDate,
                StartDate = m.StartDate,
                EndDate = m.EndDate,
                EligibleDate = m.EligibleDate,
                IncreaseType = m.IncreaseType,
                IncreaseAmount = m.IncreaseAmount,
                WashoutRule = m.WashoutRule,
                WashoutRuleSalary = m.WashoutRuleSalary,
                AutoFill = m.AutoFill,
                ScheduleType = m.ScheduleType,
            }).Where(m => m.EffectiveDate == dateEffective && m.ScheduleType == scheduleType).FirstOrDefault();
            return positionBudgetSchedulesVM;
        }

    }
}
