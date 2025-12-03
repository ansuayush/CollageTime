using ExecViewHrk.Domain.Interface;
using ExecViewHrk.EfClient;
using ExecViewHrk.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ExecViewHrk.Domain.Repositories
{
    public class TimeCardsMobileRepository : RepositoryBase, ITimeCardsMobileRepository
    {
        IEPositionRepository _ePosRepo;

        public TimeCardsMobileRepository(IEPositionRepository ePosRepo)
        {
            _ePosRepo = ePosRepo;
        }

        public List<E_PositioVm> GetEmployeePositions(int employeeId, int personId)
        {
            return _ePosRepo.GetEPositionList(personId, employeeId);
        }

        //public List<TimeCardMobileVm> GetEmployeePunchesbyPosition(int employeeId, int positionId)
        //{
        //    throw new NotImplementedException();
        //}

        public List<TimeCardMobileVm> GetEmployeeTimeCards(int employeeId, int positionId, DateTime startDate, DateTime endDate)
        {
            var list = (from timeCards in _context.TimeCards
                        where timeCards.EmployeeId == employeeId && timeCards.IsDeleted == false && timeCards.PositionId == positionId && (timeCards.ActualDate > startDate && timeCards.ActualDate < endDate)
                        select new TimeCardMobileVm
                        {
                            EmployeeId = timeCards.EmployeeId,
                            PositionId = timeCards.PositionId ?? -1,
                            FileNumber = timeCards.FileNumber,
                            TimeIn = timeCards.TimeIn,
                            TimeOut = timeCards.TimeOut,
                            LunchOut = timeCards.LunchOut,
                            LunchBack = timeCards.LunchBack
                        }).ToList();
            return list;
        }

        private double GetTimeCardDailyTotal(DateTime? TimeIn, DateTime? TimeOut, DateTime? LunchOut, DateTime? LunchBack)
        {
            double dailyhours = 0;
            if (TimeIn != null && TimeOut != null)
            {
                dailyhours = (double)(TimeOut.Value.Subtract(TimeIn.Value).TotalHours);
                if (LunchOut != null && LunchBack != null)
                {
                    double lunchours = (double)(LunchBack.Value.Subtract(LunchOut.Value).TotalHours);
                    dailyhours = dailyhours - lunchours;
                }
            }
            return dailyhours;
        }

        public List<TimeCard> GetEmployeeTimeCardByDate(int? employeeId, int? positionId, DateTime punchTime, int? companyCodeId, int? personId)
        {
            IQueryable<TimeCard> query;
            if ((employeeId == null) && (companyCodeId == null))
            {
                // Use this case when we select all timecards for a person irrespective of EmployeeId or CompanyCode
                var perTC = (from emp in _context.Employees
                             where emp.PersonId == personId.Value
                             select emp.EmployeeId);
                var allTC = (from tc in _context.TimeCards
                             select tc);
                return allTC.Where(x => perTC.Any(empId => empId == x.EmployeeId && x.Hours == null  && (DbFunctions.TruncateTime(x.ActualDate) == DbFunctions.TruncateTime(punchTime)))).ToList();
            }
            else
            {

                if (positionId != null)
                {
                    // Used this case when we pick existing time card to update punch values
                    // or get all existing timecards for the same position
                    query = (from card in _context.TimeCards
                             where ((card.EmployeeId == employeeId.Value) && card.IsDeleted == false && (card.PositionId == positionId) &&
                             (DbFunctions.TruncateTime(card.ActualDate) == DbFunctions.TruncateTime(punchTime))
                             && (card.CompanyCodeId == companyCodeId.Value)
                             && card.Hours == null
                             )
                             orderby card.TimeIn ascending
                             select card);
                }
                else
                {
                    // Used this case when retrieving daily time card details for mobile index screen where there is no position selected by default
                    query = (from card in _context.TimeCards
                             where ((card.EmployeeId == employeeId.Value) && card.IsDeleted == false && card.Hours == null &&
                             (DbFunctions.TruncateTime(card.ActualDate) == DbFunctions.TruncateTime(punchTime))
                             && (card.CompanyCodeId == companyCodeId.Value)
                             )
                             select card);
                }
            }
            List<TimeCard> timecards = query.ToList();
            return timecards;
        }

        public List<CompanyCode> GetAllCompanyCodes(string email)
        {
            var employeeCompanyIDs = (from e in _context.Employees
                                      where e.PersonId == (from p in _context.Persons where p.eMail.Equals(email) select p.PersonId).FirstOrDefault()
                                      select e.CompanyCodeId);
            var allCC = (from cc in _context.CompanyCodes
                         orderby cc.CompanyCodeId
                         select cc);
            return allCC.Where(x => employeeCompanyIDs.Any(y => y.Value == x.CompanyCodeId)).ToList();
        }

        //public bool InsertEmployeePositionPunch(int employeeId, int positionId, int punchType, DateTime punchTime, int companyCodeId, string userName, string fileName, string nightShiftTimeCardId)
        public bool InsertEmployeePositionPunch(int employeeId, int positionId, int punchType, DateTime punchTime, int companyCodeId, string userName, string fileName, string nightShiftTimeCardId, int personId,int epositionid)
        {
            // To fetch department id for the employee and selected position
            var depId = (from epos in _context.E_Positions
                         where epos.EmployeeId == employeeId && epos.PositionId == positionId
                         select epos.DepartmentId).FirstOrDefault();

            if (!String.IsNullOrEmpty(nightShiftTimeCardId))
            {
                //Previous Day time card ends at midnight 11:59
                int tId = Convert.ToInt32(nightShiftTimeCardId);
                TimeCard nightTC = (from tc in _context.TimeCards
                                    where tc.TimeCardId == tId
                                    select tc).FirstOrDefault();
                nightTC.LastModifiedDate = punchTime; // Bug 1172
                nightTC.EmployeeId = employeeId;
                nightTC.FileNumber = fileName; // FileNameInTimeCard branch
                nightTC.CompanyCodeId = companyCodeId;
                nightTC.PositionId = nightTC.PositionId;
                //nightTC.ActualDate = punchTime;
                nightTC.UserId = userName;
                nightTC.EnteredBy = userName;
                nightTC.IsDeleted = false;
                nightTC.EnteredDate = punchTime;
                nightTC.DepartmentId = depId;
                nightTC.TimeOut = new DateTime(nightTC.ActualDate.Year, nightTC.ActualDate.Month, nightTC.ActualDate.Day, 23, 59, 59);
                if ((nightTC.LunchOut.HasValue) && (!nightTC.LunchBack.HasValue))
                {
                    nightTC.LunchBack = nightTC.TimeOut.Value.AddMinutes(-1);
                }
                nightTC.DailyHours = GetTimeCardDailyTotal(nightTC.TimeIn, nightTC.TimeOut, nightTC.LunchOut, nightTC.LunchBack);

                //int res1 = _context.SaveChanges();

                // New Day starts from midnight 12:00
                TimeCard newTC = new TimeCard();
                newTC.LastModifiedDate = punchTime; // Bug 1172
                newTC.EmployeeId = employeeId;
                newTC.FileNumber = fileName; // FileNameInTimeCard branch
                newTC.CompanyCodeId = companyCodeId;
                newTC.PositionId = nightTC.PositionId;
                newTC.ActualDate = punchTime;
                newTC.UserId = userName;
                nightTC.EnteredBy = userName;
                nightTC.IsDeleted = false;
                nightTC.EnteredDate = punchTime;
                newTC.TimeIn = new DateTime(punchTime.Year, punchTime.Month, punchTime.Day, 0, 0, 0);
                newTC.TimeOut = punchTime;
                newTC.DepartmentId = depId;
                newTC.E_PositionId = epositionid;
                newTC.DailyHours = GetTimeCardDailyTotal(newTC.TimeIn, newTC.TimeOut, newTC.LunchOut, newTC.LunchBack);

                _context.TimeCards.Add(newTC);
                int res2 = _context.SaveChanges();

                if (res2 > 0)
                    return true;
            }

            // Step 1: To check if there is any Time card for the same employee on the same day without 
            // TimeOut for different Position than that we are inserting

            // If there is no such record - Go ahead and insert the new Time Card into DB
            // Else update the previous Time Card's TimeOut or LunchBack values of diffrent position 
            // and Insert the new time card

            //// To get the incompleted time card for the same employee with different position
            //var inCompletePositionTimeCard = (
            //    from card in _context.TimeCards
            //    where (
            //        (card.EmployeeId == employeeId) && card.IsDeleted == false &&
            //        (DbFunctions.TruncateTime(card.ActualDate) == DbFunctions.TruncateTime(punchTime)) &&
            //        (card.PositionId != positionId) &&
            //        ((card.TimeIn.HasValue) && (!card.TimeOut.HasValue))
            //    )
            //    select card).FirstOrDefault();

            // To get the incomplete time card records for the same person with different positions within multiple companies
            var inCompletePositionTimeCard = (
                from card in _context.TimeCards
                join emp in _context.Employees on card.EmployeeId equals emp.EmployeeId
                where (
                    //(card.EmployeeId == employeeId) &&
                    emp.PersonId == personId &&
                    card.IsDeleted == false &&
                    (DbFunctions.TruncateTime(card.ActualDate) == DbFunctions.TruncateTime(punchTime)) &&
                    (card.PositionId != positionId) &&
                    ((card.TimeIn.HasValue) && (!card.TimeOut.HasValue))
                )
                select card).FirstOrDefault();

            if (inCompletePositionTimeCard != null)
            {
                if ((inCompletePositionTimeCard.LunchOut.HasValue) && (!inCompletePositionTimeCard.LunchBack.HasValue))
                {
                    inCompletePositionTimeCard.LunchBack = punchTime.AddSeconds(-10);
                }
                inCompletePositionTimeCard.DepartmentId = depId;
                inCompletePositionTimeCard.TimeOut = punchTime.AddSeconds(-5);
                inCompletePositionTimeCard.DailyHours = GetTimeCardDailyTotal(inCompletePositionTimeCard.TimeIn, inCompletePositionTimeCard.TimeOut, inCompletePositionTimeCard.LunchOut, inCompletePositionTimeCard.LunchBack);
                _context.SaveChanges();
            }

            // Get previous time cards for the employee with that position on that day
            var existingTimeCard = GetEmployeeTimeCardByDate(employeeId, positionId, punchTime, companyCodeId, null);

            // CASE
            // Check to see if the last time card has "Punch Out" value or not
            // If Punch Out is missing then update the last time card
            // Else create a new time card for the position

            TimeCard timecard;
            TimeCard lastPrevious = existingTimeCard.LastOrDefault();
            if ((lastPrevious != null) && (!lastPrevious.TimeOut.HasValue))
            {
                // Previous Record
                timecard = lastPrevious;
                timecard.LastModifiedDate = punchTime; // Bug 1172
            }
            else
            {
                // New Time Card Record
                timecard = new TimeCard();
            }

            timecard.EmployeeId = employeeId;
            timecard.FileNumber = fileName; // FileNameInTimeCard branch
            timecard.CompanyCodeId = companyCodeId;
            timecard.PositionId = positionId;
            timecard.ActualDate = punchTime;
            timecard.UserId = userName;
            timecard.EnteredBy = userName;
            timecard.IsDeleted = false;
            timecard.EnteredDate = punchTime;
            timecard.DepartmentId = depId;
            timecard.E_PositionId = epositionid;
            switch (punchType)
            {
                case 0: // Clock In
                    if (timecard.TimeIn.HasValue)
                        return false;
                    timecard.TimeIn = punchTime;
                    break;
                case 1: // Lunch Out
                    if (!timecard.TimeIn.HasValue || timecard.LunchOut.HasValue)
                        return false;
                    timecard.LunchOut = punchTime;
                    break;
                case 2: // Lunch Back
                    if (!timecard.TimeIn.HasValue || !timecard.LunchOut.HasValue || timecard.LunchBack.HasValue)
                        return false;
                    timecard.LunchBack = punchTime;
                    break;
                case 3: // Clock Out
                    if (!timecard.TimeIn.HasValue || timecard.TimeOut.HasValue)
                        return false;
                    timecard.TimeOut = punchTime;
                    break;
            }
            timecard.DailyHours = GetTimeCardDailyTotal(timecard.TimeIn, timecard.TimeOut, timecard.LunchOut, timecard.LunchBack);
            if (timecard.TimeCardId > 0) { }
            else
            {
                _context.TimeCards.Add(timecard);
            }
            return _context.SaveChanges() > 0;
        }

        public List<TimeCardSummaryVm> GetEmployeeTimeCardsByPayPeriod(int? employeeId, int payPeriodId, bool IsArchived)
        {
            List<TimeCardSummaryVm> empTimeCardList = new List<TimeCardSummaryVm>();
            empTimeCardList = Query<TimeCardSummaryVm>("sp_GetEmployeeTimeCardsByPayPeriod", new { @EmployeeId = employeeId, @PayPeriodId = payPeriodId, @IsArchived = IsArchived }).ToList();
            return empTimeCardList;
        }

        public List<TimeCardSummaryVm> GetEmployeeTimeCardsByPayPeriodbyCompanycodeId(int? employeeId, int payPeriodId, bool IsArchived, int CompanyCodeId)
        {
            List<TimeCardSummaryVm> empTimeCardList = new List<TimeCardSummaryVm>();
            empTimeCardList = Query<TimeCardSummaryVm>("sp_GetEmployeeTimeCardsByPayPeriodbyCompanyCodeId", new { @EmployeeId = employeeId, @PayPeriodId = payPeriodId, @IsArchived = IsArchived, @CompanyCodeId = CompanyCodeId }).ToList();
            return empTimeCardList;
        }
    }
}