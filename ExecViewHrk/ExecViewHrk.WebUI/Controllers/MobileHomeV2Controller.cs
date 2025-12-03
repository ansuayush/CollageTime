using ExecViewHrk.Domain.Models;
using ExecViewHrk.EfClient;
using ExecViewHrk.Models;
using ExecViewHrk.WebUI.Filters;
using ExecViewHrk.WebUI.Helpers;
using ExecViewHrk.WebUI.Interface;
using ExecViewHrk.WebUI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    public class MobileHomeV2Controller : HrkBaseController
    {
        public MobileHomeV2Controller(IHttpClientWrapper clientWrapper, IServiceLocator serviceLocator)
        : base(clientWrapper, serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        #region API Call Methods

        private PayPeriodVM GetEmployeeCurrentPayPeriod(int empId, bool isArchived)
        {
            var returnValue = _clientWrapper.GetAsync(_serviceLocator.GetServiceUrl(ServiceModules.Mobile, new string[] { ServiceApiSubModules.GetEmployeeCurrentPayPeriod, empId.ToString(), isArchived.ToString() }));
            var empCurrentPayPeriod = JsonConvert.DeserializeObject<PayPeriodVM>(returnValue);
            return empCurrentPayPeriod;
        }

        private List<TimeCardWeekTotalCollectionVm> GetTimeCardWeekTotalList(int empid, int? departmentId, int payPeriodId, bool isArchive)
        {
            departmentId = departmentId ?? -1;
            var returnValue = _clientWrapper.GetAsync(_serviceLocator.GetServiceUrl(ServiceModules.Mobile, new string[] { ServiceApiSubModules.GetTimeCardWeekTotalList, empid.ToString(), departmentId.ToString(), payPeriodId.ToString(), isArchive.ToString() }));
            var timeCardWeekTotalList = JsonConvert.DeserializeObject<List<TimeCardWeekTotalCollectionVm>>(returnValue);
            return timeCardWeekTotalList;
        }

        private List<TimeCard> GetEmployeeTimeCardByDate(int employeeId, int? positionId, DateTime punchTime, int companyCodeId)
        {
            positionId = positionId ?? -1;
            var returnValue = _clientWrapper.GetAsync(_serviceLocator.GetServiceUrl(ServiceModules.Mobile, new string[] { ServiceApiSubModules.GetEmployeeTimeCardByDate, employeeId.ToString(), positionId.ToString(), Uri.EscapeDataString(punchTime.ToString().Replace("/", "-")), companyCodeId.ToString() }));
            var employeeTimeCardByDate = JsonConvert.DeserializeObject<List<TimeCard>>(returnValue);
            return employeeTimeCardByDate;
        }

        private List<E_PositioVm> GetEPositionList(int personId, int empId)
        {
            var returnValue = _clientWrapper.GetAsync(_serviceLocator.GetServiceUrl(ServiceModules.Mobile, new string[] { ServiceApiSubModules.GetEPositionList, personId.ToString(), empId.ToString() }));
            var ePositionList = JsonConvert.DeserializeObject<List<E_PositioVm>>(returnValue);
            return ePositionList;
        }

        private bool InsertEmployeePositionPunch(int employeeId, int positionId, int punchType, DateTime punchTime, int companyCode, string userName, string fileName)
        {
            var url = _serviceLocator.GetServiceUrl(ServiceModules.Mobile, new string[] { ServiceApiSubModules.InsertEmployeePositionPunch });
            TimeCardPayload payload = new TimeCardPayload() { EmployeeId = employeeId, PositionId = positionId, PunchType = punchType, PunchTime = punchTime, CompanyCode = companyCode, UserName = userName, FileName = fileName };
            var returnObj = _clientWrapper.PostAsync(url, JsonConvert.SerializeObject(payload));
            return JsonConvert.DeserializeObject<bool>(returnObj);
        }

        private List<PayPeriodVM> GetPayPeriodsList(int employeeId, bool isArchived)
        {
            var returnValue = _clientWrapper.GetAsync(_serviceLocator.GetServiceUrl(ServiceModules.Mobile, new string[] { ServiceApiSubModules.GetPayPeriodsList, employeeId.ToString(), isArchived.ToString() }));
            var payPeriodsList = JsonConvert.DeserializeObject<List<PayPeriodVM>>(returnValue);
            return payPeriodsList;
        }

        private List<TimeCardSummaryVm> GetEmployeeTimeCardsByPayPeriod(int employeeId, int payPeriodId, bool IsArchived)
        {
            var returnValue = _clientWrapper.GetAsync(_serviceLocator.GetServiceUrl(ServiceModules.Mobile, new string[] { ServiceApiSubModules.GetEmployeeTimeCardsByPayPeriod, employeeId.ToString(), payPeriodId.ToString(), IsArchived.ToString() }));
            var employeeTimeCardsByPayPeriod = JsonConvert.DeserializeObject<List<TimeCardSummaryVm>>(returnValue);
            return employeeTimeCardsByPayPeriod;
        }

        #endregion

        // GET: MobileHomeV2
        [NoCache]
        public ActionResult Index()
        {
            if (IsSessionExpired)
                return RedirectToAction("Login", "Account");
            AcceptPunchVM model = new AcceptPunchVM();
            model.NotificationMessage = Convert.ToString(TempData["WrongMessage"]);
            string connString = User.Identity.GetClientConnectionString();
            if (!String.IsNullOrEmpty(connString))
                using (ClientDbContext clientDbContext = new ClientDbContext(connString))
                {
                    Employee emp = AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name);
                    if (emp != null)
                    {
                        model.EmployeeId = emp.EmployeeId;
                        model.FileName = emp.FileNumber;
                        DateTime etcCurrentDate = Utils.ConvertTimeFromUtc(DateTime.UtcNow, ConfigurationManager.AppSettings["TimeZone"]);

                        // Case 1: Check to see if the employee/student is active or not
                        DateTime hireDate = emp.HireDate;
                        DateTime? terminationDate = emp.TerminationDate;
                        if ((DateTime.Compare(hireDate, etcCurrentDate) > 0) || (terminationDate.HasValue && DateTime.Compare(terminationDate.Value, etcCurrentDate) < 0))
                        {
                            model.IsInActive = true;
                        }
                        model.CurrentDate = etcCurrentDate;
                        model.FullName = emp.Person.Firstname + " " + emp.Person.Lastname;
                        TempData["EmployeeId"] = emp.EmployeeId;
                        TempData["EmployeeName"] = emp.Person.Firstname + " " + emp.Person.Lastname;
                        int? deptId = emp.DepartmentId;
                        model.CompanyCodeId = emp.CompanyCodeId;
                        int? payFreqId = emp.PayFrequencyId;
                        // To get the current Active Pay Period
                        PayPeriodVM empCurrentPayPeriod = GetEmployeeCurrentPayPeriod(emp.EmployeeId, false);
                        model.IsPayPeriodLocked = empCurrentPayPeriod.LockoutEmployees;
                        int? payGroupId = 0;
                        double? payPeriodTotal = 0;
                        if (empCurrentPayPeriod != null)
                        {
                            TempData["PayPeriodId"] = empCurrentPayPeriod.PayPeriodId;
                            model.PayPeriodStartDate = empCurrentPayPeriod.StartDate;
                            model.PayPeriodEndDate = empCurrentPayPeriod.EndDate;
                            payGroupId = empCurrentPayPeriod.PayGroupCode;
                            TempData["PayGroupId"] = empCurrentPayPeriod.PayGroupCode ?? 0;
                            // Get Pay Period Timecards for Pay Period Total
                            List<TimeCardWeekTotalCollectionVm> payPeriodTimeCardList = GetTimeCardWeekTotalList(emp.EmployeeId, deptId.Value, empCurrentPayPeriod.PayPeriodId, false);
                            if ((payPeriodTimeCardList != null) && (payPeriodTimeCardList.Count > 0))
                            {
                                payPeriodTotal = payPeriodTimeCardList.Sum(e => e.WeeklyTotal);
                            }
                            model.PayPeriodTotal = payPeriodTotal.Value;
                            // Get Daily Timecards for Daily Total
                            List<TimeCard> dailyTimeCardList = GetEmployeeTimeCardByDate(emp.EmployeeId, null, model.CurrentDate, model.CompanyCodeId ?? 0);
                            if ((dailyTimeCardList != null) && (dailyTimeCardList.Count > 0))
                            {
                                model.DailyTotal = dailyTimeCardList.Sum(p => p.DailyHours);
                            }
                            // Get all employee positions
                            List<E_PositioVm> posList = GetEPositionList(emp.PersonId, emp.EmployeeId);

                            List<E_PositioVm> activePositions = new List<E_PositioVm>();
                            foreach (E_PositioVm pos in posList)
                            {
                                // Picking all positions which has same "Pay Group" as that of the student
                                if ((pos.PayGroupId.HasValue) && (pos.PayGroupId.Value == payGroupId.Value))
                                {
                                    bool flag = false;
                                    if (pos.StartDate.HasValue && pos.actualEndDate.HasValue)
                                    {
                                        if (
                                        (DateTime.Compare(etcCurrentDate, pos.StartDate.Value) >= 0)
                                        &&
                                        (DateTime.Compare(etcCurrentDate, pos.actualEndDate.Value) <= 0)
                                        )
                                        {
                                            // If Position has both Start and End Date
                                            if ((DateTime.Compare(pos.StartDate.Value, empCurrentPayPeriod.StartDate) >= 0) && (DateTime.Compare(pos.actualEndDate.Value, empCurrentPayPeriod.EndDate) <= 0))
                                            {
                                                // Position Dates are in between Pay Period Dates
                                                flag = true;
                                            }
                                            else if ((DateTime.Compare(pos.StartDate.Value, empCurrentPayPeriod.StartDate) <= 0) && (DateTime.Compare(pos.actualEndDate.Value, empCurrentPayPeriod.EndDate) <= 0) && (DateTime.Compare(pos.actualEndDate.Value, empCurrentPayPeriod.StartDate) >= 0))
                                            {
                                                // Position starts before Pay Period and End within Pay Pariod
                                                flag = true;
                                            }
                                            else if ((DateTime.Compare(pos.StartDate.Value, empCurrentPayPeriod.StartDate) >= 0) && (DateTime.Compare(pos.actualEndDate.Value, empCurrentPayPeriod.EndDate) >= 0) && (DateTime.Compare(pos.StartDate.Value, empCurrentPayPeriod.EndDate) >= 0))
                                            {
                                                // Position Starts in Pay Period but ends after Pay Period end Date
                                                flag = true;
                                            }
                                            else if ((DateTime.Compare(pos.StartDate.Value, empCurrentPayPeriod.StartDate) <= 0) && (DateTime.Compare(pos.actualEndDate.Value, empCurrentPayPeriod.EndDate) >= 0))
                                            {
                                                // Position Starts before Pay Period start date and ends after Pay Period end date
                                                flag = true;
                                            }
                                        }
                                    }
                                    else if (pos.StartDate.HasValue && !pos.actualEndDate.HasValue)
                                    {
                                        // If Position has Start Date but NO End Date
                                        if
                                            (
                                            (
                                            (DateTime.Compare(pos.StartDate.Value, empCurrentPayPeriod.StartDate) <= 0)
                                            ||
                                            (
                                            (DateTime.Compare(pos.StartDate.Value, empCurrentPayPeriod.StartDate) >= 0)
                                            &&
                                            (DateTime.Compare(pos.StartDate.Value, empCurrentPayPeriod.EndDate) <= 0)
                                            )
                                            )
                                            &&
                                            (DateTime.Compare(etcCurrentDate, pos.StartDate.Value) >= 0)
                                            )
                                        {
                                            flag = true;
                                        }
                                    }
                                    if (flag)
                                        activePositions.Add(pos);
                                }
                            }
                            model.PositionList = activePositions;
                        }
                        model.PunchType = -1;
                        model.SelectedPosition = -1;
                        return View(model);
                    }
                }
            return View();
        }

        public ActionResult _PunchButtons(int employeeId, int? positionId, DateTime punchTime, int companyCodeId)
        {
            if (IsSessionExpired)
                return new EmptyResult();
            bool disablePunchIn = false, disableLunchOut = false, disableLunchBack = false, disablePunchOut = false;
            List<TimeCard> todayTimeCard = GetEmployeeTimeCardByDate(employeeId, positionId, punchTime, companyCodeId);
            if ((todayTimeCard != null) && (todayTimeCard.Count > 0))
            {
                if (todayTimeCard.Count == 1)
                {
                    TimeCard t = todayTimeCard.FirstOrDefault();
                    if ((!t.TimeIn.HasValue) && (!t.LunchOut.HasValue) && (!t.LunchBack.HasValue) && (!t.TimeOut.HasValue))
                    {
                        // New punch for the day - Enable Punch In Only and Disable All
                        disableLunchOut = true;
                        disableLunchBack = true;
                        disablePunchOut = true;
                    }
                    else if ((t.TimeIn.HasValue) && (!t.LunchOut.HasValue) && (!t.LunchBack.HasValue) && (!t.TimeOut.HasValue))
                    {
                        // If user already has In Punch - Enable Out Punches and Diable In Punches
                        disablePunchIn = true;
                        disableLunchBack = true;
                    }
                    else if ((t.TimeIn.HasValue) && (t.LunchOut.HasValue) && (!t.LunchBack.HasValue) && (!t.TimeOut.HasValue))
                    {
                        // If user went out for a lunch break - Enable and accept only Lunch Back Punch
                        disablePunchIn = true;
                        disableLunchOut = true;
                        disablePunchOut = true;
                    }
                    else if ((t.TimeIn.HasValue) && (t.LunchOut.HasValue) && (t.LunchBack.HasValue) && (!t.TimeOut.HasValue))
                    {
                        // If user has a Lunch Back Punch and all its previous punches - Enable and accept only Punch Out 
                        disablePunchIn = true;
                        disableLunchOut = true;
                        disableLunchBack = true;
                    }
                    else if ((t.TimeIn.HasValue) && (t.LunchOut.HasValue) && (t.LunchBack.HasValue) && (t.TimeOut.HasValue))
                    {
                        // When the user has all punches for the position
                        #region OLD CASE
                        // OLD CASE with one time card per day per position constraint - Disable All Punches
                        //disablePunchIn = false;
                        //disableLunchOut = false;
                        //disableLunchBack = false;
                        //disablePunchOut = false;
                        #endregion
                        #region NEW CASE
                        // Accept multiple timecards for the same position and date
                        // Enable Punch In Only and Disable All
                        disableLunchOut = true;
                        disableLunchBack = true;
                        disablePunchOut = true;
                        #endregion
                    }
                    else if ((t.TimeIn.HasValue) && (!t.LunchOut.HasValue) && (!t.LunchBack.HasValue) && (t.TimeOut.HasValue))
                    {
                        #region OLD CASE
                        //// When a user has Punch In and Punch Out for the position - Disable All Punches
                        //disablePunchIn = true;
                        //disableLunchOut = true;
                        //disableLunchBack = true;
                        //disablePunchOut = true;
                        #endregion
                        #region NEW CASE
                        // When a user has Punch In and Punch Out for the position - Disable All Punches
                        disableLunchOut = true;
                        disableLunchBack = true;
                        disablePunchOut = true;
                        #endregion
                    }
                }
                else
                {
                    // Multiple Punches for the day
                    TimeCard incompeteTimeCard = todayTimeCard.LastOrDefault();
                    if ((!incompeteTimeCard.TimeIn.HasValue) && (!incompeteTimeCard.LunchOut.HasValue) && (!incompeteTimeCard.LunchBack.HasValue) && (!incompeteTimeCard.TimeOut.HasValue))
                    {
                        // New punch for the day - Enable Punch In Only and Disable All
                        disableLunchOut = true;
                        disableLunchBack = true;
                        disablePunchOut = true;
                    }
                    else if ((incompeteTimeCard.TimeIn.HasValue) && (!incompeteTimeCard.LunchOut.HasValue) && (!incompeteTimeCard.LunchBack.HasValue) && (!incompeteTimeCard.TimeOut.HasValue))
                    {
                        // If user already has In Punch - Enable Out Punches and Diable In Punches
                        disablePunchIn = true;
                        disableLunchBack = true;
                    }
                    else if ((incompeteTimeCard.TimeIn.HasValue) && (incompeteTimeCard.LunchOut.HasValue) && (!incompeteTimeCard.LunchBack.HasValue) && (!incompeteTimeCard.TimeOut.HasValue))
                    {
                        // If user went out for a lunch break - Enable and accept only Lunch Back Punch
                        disablePunchIn = true;
                        disableLunchOut = true;
                        disablePunchOut = true;
                    }
                    else if ((incompeteTimeCard.TimeIn.HasValue) && (incompeteTimeCard.LunchOut.HasValue) && (incompeteTimeCard.LunchBack.HasValue) && (!incompeteTimeCard.TimeOut.HasValue))
                    {
                        // If user has a Lunch Back Punch and all its previous punches - Enable and accept only Punch Out 
                        disablePunchIn = true;
                        disableLunchOut = true;
                        disableLunchBack = true;
                    }
                    else if ((incompeteTimeCard.TimeIn.HasValue) && (incompeteTimeCard.LunchOut.HasValue) && (incompeteTimeCard.LunchBack.HasValue) && (incompeteTimeCard.TimeOut.HasValue))
                    {
                        // When the user has all punches for the position
                        #region OLD CASE
                        // OLD CASE with one time card per day per position constraint - Disable All Punches
                        //disablePunchIn = false;
                        //disableLunchOut = false;
                        //disableLunchBack = false;
                        //disablePunchOut = false;
                        #endregion
                        #region NEW CASE
                        // Accept multiple timecards for the same position and date
                        // Enable Punch In Only and Disable All
                        disableLunchOut = true;
                        disableLunchBack = true;
                        disablePunchOut = true;
                        #endregion
                    }
                    else if ((incompeteTimeCard.TimeIn.HasValue) && (!incompeteTimeCard.LunchOut.HasValue) && (!incompeteTimeCard.LunchBack.HasValue) && (incompeteTimeCard.TimeOut.HasValue))
                    {
                        #region OLD CASE
                        //// When a user has Punch In and Punch Out for the position - Disable All Punches
                        //disablePunchIn = true;
                        //disableLunchOut = true;
                        //disableLunchBack = true;
                        //disablePunchOut = true;
                        #endregion
                        #region NEW CASE
                        // When a user has Punch In and Punch Out for the position - Disable All Punches
                        disableLunchOut = true;
                        disableLunchBack = true;
                        disablePunchOut = true;
                        #endregion
                    }
                }
            }
            else
            {
                // New punch for the day - Enable Punch In Only and Disable All
                disableLunchOut = true;
                disableLunchBack = true;
                disablePunchOut = true;
            }
            ViewData["disablePunchIn"] = disablePunchIn;
            ViewData["disableLunchOut"] = disableLunchOut;
            ViewData["disableLunchBack"] = disableLunchBack;
            ViewData["disablePunchOut"] = disablePunchOut;
            return PartialView();
        }

        [HttpPost]
        public ActionResult AcceptPunch(AcceptPunchVM model)
        {
            if (String.IsNullOrEmpty(User.Identity.Name))
                return RedirectToAction("Login", "Account");
            if (!ModelState.IsValid)
                return View(model);
            DateTime estTime = Utils.ConvertTimeFromUtc(DateTime.UtcNow, ConfigurationManager.AppSettings["TimeZone"]);
            if (InsertEmployeePositionPunch(model.EmployeeId, model.SelectedPosition, model.PunchType, estTime, model.CompanyCodeId ?? 0, User.Identity.Name, model.FileName))
                return RedirectToAction("Index", "MobileHomeV2");
            else
            {
                TempData["WrongMessage"] = "Something went wrong; Please try again...";
                return RedirectToAction("Index", "MobileHomeV2");
            }
        }

        // GET: MobileHomeV2
        public ActionResult Summary()
        {
            if (IsSessionExpired)
                return RedirectToAction("Login", "Account");
            int empId = Convert.ToInt32(TempData.Peek("EmployeeId"));
            int payGroupId = Convert.ToInt32(TempData.Peek("PayGroupId"));
            int payPeriodId = Convert.ToInt32(TempData.Peek("PayPeriodId"));
            List<PayPeriodVM> employeePayPeriods = GetPayPeriodsList(empId, false);
            employeePayPeriods = employeePayPeriods.Where(p => p.PayGroupCode.Value == payGroupId).ToList();
            ViewData["PayPeriods"] = employeePayPeriods;
            return View(employeePayPeriods);
        }

        public ActionResult _ViewSummary(int PayPeriodId)
        {
            if (IsSessionExpired)
                return new EmptyResult();
            int empId = Convert.ToInt32(TempData.Peek("EmployeeId"));
            List<TimeCardSummaryVm> empTimeCards = GetEmployeeTimeCardsByPayPeriod(empId, PayPeriodId, false);
            double weeklyHours = empTimeCards.Sum(t => t.DailyHours);
            ViewData["PayPeriodTotal"] = weeklyHours;
            return PartialView(empTimeCards);
        }

        public bool IsSessionExpired
        {
            get
            {
                return String.IsNullOrEmpty(User.Identity.Name);
            }
        }
    }
}