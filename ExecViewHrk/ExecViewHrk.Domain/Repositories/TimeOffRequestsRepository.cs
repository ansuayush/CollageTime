using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Models;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using ExecViewHrk.EfClient;
using System;


namespace ExecViewHrk.Domain.Repositories
{
    public class TimeOffRequestsRepository : RepositoryBase, ITimeOffRequestsRepository
    {
        #region Employee View

        /// <summary>
        /// Employee view
        /// </summary>
        /// <param name="empDetails"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public IList<TimeOffRequestStatusVM> GetEmpTimeOffRequest(Employee empDetails, int year, int month)
        {
            List<TimeOffRequestStatusVM> timeOffRequestStatusVM = new List<TimeOffRequestStatusVM>();
            try
            {
                timeOffRequestStatusVM = _context.TimeOffRequests
                                        .Where(x => x.EmployeeId == empDetails.EmployeeId && x.CompanyCodeId == empDetails.CompanyCodeId && x.TimeOffDate.Year == year && x.TimeOffDate.Month == month)
                                        .Select(x => new TimeOffRequestStatusVM { timeOffRequestDate = x.TimeOffDate, statusOfTimeOffRequest = x.RequestStatus })
                                        .OrderBy(x => x.timeOffRequestDate).ToList();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
            return timeOffRequestStatusVM;
        }


        /// <summary>
        /// Check TimeOff Request Date InDb
        /// </summary>
        /// <param name="empDetails"></param>
        /// <param name="timeOffDate"></param>
        /// <returns></returns>
        public bool GetTimeOffRequestDateInDb(Employee empDetails, DateTime timeOffDate)
        {
            bool timeOffRequestDateInDb = false;
            try
            {
                timeOffRequestDateInDb = _context.TimeOffRequests
                                        .Where(x => x.CompanyCodeId == empDetails.CompanyCodeId && x.EmployeeId == empDetails.EmployeeId && DateTime.Compare(x.TimeOffDate, timeOffDate) == 0)
                                        .Select(x => x.TimeOffRequestId).Any();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
            return timeOffRequestDateInDb;
        }


        public TimeOffRequestVM GetTimeOffRequestData(Employee empDetails, DateTime timeOffDate)
        {
            TimeOffRequestVM timeOffRequestVM = new TimeOffRequestVM();
            try
            {
                bool TimeOffRequestDateInDb = GetTimeOffRequestDateInDb(empDetails, timeOffDate);
                if (TimeOffRequestDateInDb)
                {
                    timeOffRequestVM = _context.TimeOffRequests
                                            .Where(x => x.CompanyCodeId == empDetails.CompanyCodeId && x.EmployeeId == empDetails.EmployeeId && DateTime.Compare(x.TimeOffDate, timeOffDate) == 0)
                                            .Select(x => new TimeOffRequestVM
                                            {
                                                start = timeOffDate,
                                                end = timeOffDate,
                                                CompanyCodeId = x.CompanyCodeId,
                                                HoursCodeId = x.HoursCodeId,
                                                TimeOfftHours = x.TimeOfftHours,
                                            }).FirstOrDefault();
                }
                else
                {
                    timeOffRequestVM = new TimeOffRequestVM()
                    {
                        start = timeOffDate,
                        end = timeOffDate,
                        CompanyCodeId = empDetails.CompanyCodeId.Value
                    };
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
            return timeOffRequestVM;
        }
        /// <summary>
        /// Insert Time off request
        /// </summary>
        /// <param name="empDetails"></param>
        /// <param name="timeOffDate"></param>
        /// <returns></returns>
        public bool AddTimeOffRequest(TimeOffRequestVM timeOffRequestVM, Employee empDetails, DateTime timeOffDate, int days)
        {
            bool succed = false;
            string datesRequested = "";
            for (int i = 0; i < days; i++)
            {
                //val = (int) timeOffDate.DayOfWeek;

                if ((timeOffDate.DayOfWeek != DayOfWeek.Saturday) && (timeOffDate.DayOfWeek != DayOfWeek.Sunday))
                {
                    var newTimeoffRequestRecord = new TimeOffRequest
                    {
                        //TimeOffRequestId = timeOffRequestVm
                        CompanyCodeId = empDetails.CompanyCodeId.Value,
                        EmployeeId = empDetails.EmployeeId,
                        TimeOffDate = timeOffDate,
                        HoursCodeId = timeOffRequestVM.HoursCodeId,
                        TimeOfftHours = timeOffRequestVM.TimeOfftHours,
                        RequestStatus = timeOffRequestVM.RequestStatus
                    };

                    _context.TimeOffRequests.Add(newTimeoffRequestRecord);

                    datesRequested = datesRequested + Environment.NewLine + timeOffDate.DayOfWeek + "  " + timeOffDate.Date.ToString("d");
                }
                timeOffDate = timeOffDate.AddDays(1);
            }
            try
            {
                succed = _context.SaveChanges() > 0 ? true : false;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
            return succed;
        }

        /// <summary>
        /// Delete Time off request
        /// </summary>
        /// <param name="empDetails"></param>
        /// <param name="timeOffDate"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public List<TimeOffRequest> DeleteTimeOffRequest(Employee empDetails, DateTime timeOffDate, int days)
        {
            var timeOffRequestList = new List<TimeOffRequest>();
            for (int i = 0; i < days; i++)
            {
                TimeOffRequest timeOffRecordInDb = _context.TimeOffRequests
                .Where(x => x.CompanyCodeId == empDetails.CompanyCodeId && x.EmployeeId == empDetails.EmployeeId && DateTime.Compare(x.TimeOffDate, timeOffDate) == 0).SingleOrDefault();

                if (timeOffRecordInDb != null)
                {
                    timeOffRequestList.Add(timeOffRecordInDb);
                    _context.TimeOffRequests.Attach(timeOffRecordInDb);
                    _context.TimeOffRequests.Remove(timeOffRecordInDb);
                    try
                    {
                        _context.SaveChanges();
                    }
                    catch (Exception err)
                    {
                        string message = err.Message;
                    }
                }

                timeOffDate = timeOffDate.AddDays(1);
            }
            return timeOffRequestList;
        }

        #endregion

        #region Admin,Manager View

        public List<TimeOffTotalRequestsVM> GetAllEmpTimeOffRequestsList(int sYear, int sMonth)
        {
            List<TimeOffTotalRequestsVM> timeOffTotalRequestsVM = new List<TimeOffTotalRequestsVM>();
            try
            {
                timeOffTotalRequestsVM = _context.TimeOffRequests
                    .Where(x => x.TimeOffDate.Year == sYear && x.TimeOffDate.Month == sMonth)
                    .GroupBy(x => x.TimeOffDate)
                    .Select(m => new TimeOffTotalRequestsVM
                    {
                        timeOffRequestDate = m.Key,
                        totalRequests = m.Select(x => x.RequestStatus).Count(),
                        pendingRequests = m.Count(x => x.RequestStatus == 0),
                        approvedRequests = m.Count(x => x.RequestStatus == 1),
                        disapprovedRequests = m.Count(x => x.RequestStatus == 2)
                    }).OrderBy(x => x.timeOffRequestDate).ToList();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
            return timeOffTotalRequestsVM;
        }

        public List<TimeOffEmpDetailsVM> GetTimeOffEmpDetailsList(DateTime selectedDate)
        {
            List<TimeOffEmpDetailsVM> timeOffEmpDetailsVM = new List<TimeOffEmpDetailsVM>();
            try
            {
                timeOffEmpDetailsVM = _context.TimeOffRequests
                    .Include("Employees.Person")
                    .Include("HoursCode")
                    .Where(x => DateTime.Compare(x.TimeOffDate, selectedDate) == 0)
                    .Select(x => new TimeOffEmpDetailsVM
                    {
                        TimeOffRequestId = x.TimeOffRequestId,
                        PersonName = x.Employee.Person.Firstname + " " + x.Employee.Person.Lastname,
                        EmployeeId = x.EmployeeId,
                        HoursCodeId = x.HoursCodeId,
                        HoursCodeCode = x.HoursCode.HoursCodeCode + "-" + x.HoursCode.HoursCodeDescription,
                        CompanyCodeId = x.CompanyCodeId,
                        TimeOfftHours = x.TimeOfftHours,
                        RequestStatus = x.RequestStatus,
                        TimeOffRequest = x.TimeOffDate
                    }).OrderBy(x => x.RequestStatus).ToList();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
            return timeOffEmpDetailsVM;
        }

        public bool AddResposeTimeOffRequest(TimeOffVM timeOffVM)
        {
            bool status = false;
            for (int i = 0; i < timeOffVM.TimeOffEmpDetailsList.Count(); i++)
            {
                var Id = timeOffVM.TimeOffEmpDetailsList[i].TimeOffRequestId;
                var s = timeOffVM.TimeOffEmpDetailsList[i].RequestStatus;
                if (s != 0)
                {
                    var employeeId = timeOffVM.TimeOffEmpDetailsList[i].EmployeeId;
                    var timeOffRecordInDb = _context.TimeOffRequests.Where(x => x.TimeOffRequestId == Id).FirstOrDefault();
                    var employeeTaken = _context.EmployeesAllowedTakens.Where(x => x.EmployeeId == employeeId).FirstOrDefault();
                    if (timeOffRecordInDb != null)
                    {
                        timeOffRecordInDb.RequestStatus = timeOffVM.TimeOffEmpDetailsList[i].RequestStatus;
                    }
                    _context.TimeOffRequests.Attach(timeOffRecordInDb);
                    _context.Entry(timeOffRecordInDb).State = System.Data.Entity.EntityState.Modified;

                    if (s == 1 && employeeTaken != null)
                    {
                        employeeTaken.Taken = employeeTaken.Taken + timeOffVM.TimeOffEmpDetailsList[i].TimeOfftHours;
                        employeeTaken.Remainder = employeeTaken.Remainder - timeOffVM.TimeOffEmpDetailsList[i].TimeOfftHours;

                        _context.EmployeesAllowedTakens.Attach(employeeTaken);
                        _context.Entry(employeeTaken).State = System.Data.Entity.EntityState.Modified;
                    }
                }
            }
            status = _context.SaveChanges() > 0 ? true : false;

            return status;
        }

        #endregion
    }
}
