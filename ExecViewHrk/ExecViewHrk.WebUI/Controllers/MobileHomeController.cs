using ExecViewHrk.Domain.Helper;
using ExecViewHrk.Domain.Interface;
using ExecViewHrk.EfClient;
using ExecViewHrk.Models;
using ExecViewHrk.WebUI.Filters;
using ExecViewHrk.WebUI.Helpers;
using ExecViewHrk.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    public class MobileHomeController : Controller
    {
        private IEPositionRepository _positionRepo;
        private ITimeCardsMobileRepository _timecardMobileRepo;
        private ILookupTablesRepository _ilookuprepo;
        private ITimeCardMatrixReposotory _timeCardsMatrixRepo;
        private ITimeCardSessionInOutRepository _timeCardSessionInOutRepo;

        public MobileHomeController(
            IEPositionRepository positionRepo,
            ITimeCardsMobileRepository timecardMobileRepo,
            ILookupTablesRepository ilookuprepo,
            ITimeCardMatrixReposotory timeCardsMatrixRepo,
            ITimeCardSessionInOutRepository timeCardSessionInOutRepo)
        {
            _positionRepo = positionRepo;
            _timecardMobileRepo = timecardMobileRepo;
            _ilookuprepo = ilookuprepo;
            _timeCardsMatrixRepo = timeCardsMatrixRepo;
            _timeCardSessionInOutRepo = timeCardSessionInOutRepo;
        }

        // GET: MobileHome
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
                    DateTime etcCurrentDate = Utils.ConvertTimeFromUtc(DateTime.UtcNow, ConfigurationManager.AppSettings["TimeZone"]);
                    
                    // Get all Company Codes
                    List<CompanyCode> ccList = _timecardMobileRepo.GetAllCompanyCodes(User.Identity.Name);
                    model.CompanyCodeList = ccList;
                    TempData["CompanyCodeIndex"] = TempData.Peek("CompanyCodeIndex") == null ? 0 : (int)TempData.Peek("CompanyCodeIndex");
                    //TempData["CompanyCodeId"] = TempData.Peek("CompanyCodeId") == null ? ccList.FirstOrDefault().CompanyCodeId : Convert.ToInt32(TempData.Peek("CompanyCodeId"));

                    TempData["CompanyCodeId"] = TempData.Peek("CompanyCodeId") == null ? ccList[(int)TempData.Peek("CompanyCodeIndex")].CompanyCodeId : Convert.ToInt32(TempData.Peek("CompanyCodeId"));
                    model.SelectedCompanyCode = Convert.ToInt32(TempData.Peek("CompanyCodeId"));

                    Employee emp = AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name, model.SelectedCompanyCode);
                    if (emp != null)
                    {
                        TempData["PersonId"] = emp.PersonId;
                        model.EmployeeId = emp.EmployeeId;
                        model.FileName = emp.FileNumber;

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
                        var epositionid = Session["Epositionid"];
                        var epositionid1 = 0;
                        if (epositionid==null)
                        {
                            epositionid1 = 0;
                        }
                        else
                        {
                            var epos = epositionid.ToString();
                            var eposid = epos.Split('-');
                            epositionid1 = Convert.ToInt32(eposid[0]);
                        }
                        var epostiondepartmentid = clientDbContext.E_Positions.Where(x => x.E_PositionId == epositionid1).Select(x => x.DepartmentId).FirstOrDefault();
                        int? deptId = epostiondepartmentid;
                        TempData["DeptId"] = deptId.ToString();
                        model.CompanyCodeId = emp.CompanyCodeId;
                        int? payFreqId = emp.PayFrequencyId;
                        //To Get Max Session Limit Hours
                        int TreatyTime = Convert.ToInt32(ConfigurationManager.AppSettings["TreatyTime"]);
                        var MaxHours = _timeCardSessionInOutRepo.GetTimeCardSessionList();
                        model.MaxHours = MaxHours.Where(x => x.Toggle == true).Select(x => x.MaxHours).SingleOrDefault();
                        model.TreatyTime = Math.Round((double)(model.MaxHours * TreatyTime) / 100, 2);
                        // To get the current Active Pay Period
                        PayPeriodVM empCurrentPayPeriod = _ilookuprepo.GetEmployeeCurrentPayPeriod(emp.EmployeeId, false);
                        TempData["PayperiodId"]=empCurrentPayPeriod.PayPeriodId;
                        int? payGroupId = 0;
                        if (empCurrentPayPeriod != null)
                        {
                            TempData["CurrentPayPeriod"] = empCurrentPayPeriod;
                            if ((empCurrentPayPeriod.LockoutEmployees || !empCurrentPayPeriod.IsPayPeriodActive))
                            {
                                model.IsPayPeriodLocked = empCurrentPayPeriod.LockoutEmployees;
                            }
                            model.IsPayPeriodLocked = empCurrentPayPeriod.LockoutEmployees;
                            TempData["PayPeriodId"] = empCurrentPayPeriod.PayPeriodId;
                            model.PayPeriodStartDate = empCurrentPayPeriod.StartDate;
                            model.PayPeriodEndDate = empCurrentPayPeriod.EndDate;
                            payGroupId = empCurrentPayPeriod.PayGroupCode;
                            TempData["PayGroupId"] = empCurrentPayPeriod.PayGroupCode ?? 0;
                            // Get Pay Period Timecards for Pay Period Total
                            List<TimeCardWeekTotalCollectionVm> payPeriodTimeCardList = _timeCardsMatrixRepo.GetTimeCardWeekTotalList(emp.EmployeeId, deptId.Value, empCurrentPayPeriod.PayPeriodId, false);
                            model.Week1Hours = payPeriodTimeCardList.Where(x => x.WeekNum == 1).Select(x => x.WeeklyTotal).SingleOrDefault();
                            model.Week2Hours = payPeriodTimeCardList.Where(x => x.WeekNum == 2).Select(x => x.WeeklyTotal).SingleOrDefault();
                            double? payPeriodTotal = 0;
                            if ((payPeriodTimeCardList != null) && (payPeriodTimeCardList.Count > 0))
                            {
                                payPeriodTotal = payPeriodTimeCardList.Sum(e => e.WeeklyTotal);
                            }
                            
                            model.PayPeriodTotal = payPeriodTotal.Value;
                            // Get Daily Timecards for Daily Total
                            List<TimeCard> dailyTimeCardList = _timecardMobileRepo.GetEmployeeTimeCardByDate(emp.EmployeeId, null, model.CurrentDate, model.CompanyCodeId ?? 1, (int)TempData.Peek("PersonId"));
                            double? dailyTotal = 0;
                            if ((dailyTimeCardList != null) && (dailyTimeCardList.Count > 0))
                            {
                                dailyTotal = dailyTimeCardList.Sum(p => p.DailyHours);
                            }
                            model.DailyTotal = dailyTotal;
                            // Get all employee positions
                            //List<E_PositioVm> posList = _positionRepo.GetEPositionList_v2(emp.PersonId, emp.EmployeeId, model.SelectedCompanyCode);
                            List<E_PositioVm> posList = _positionRepo.GetEPositionList_v2((int)TempData.Peek("PersonId"), emp.EmployeeId, model.SelectedCompanyCode);
                            var activePos = GetActiveE_Positions(posList);
                            model.PositionList = activePos;
                           // model.EpositionId = activePos[0].EpositionId;

                            if ((activePos == null) || (activePos.Count == 0))
                            {
                                if (ccList.Count > 1)
                                {
                                    if ((int)TempData.Peek("CompanyCodeIndex") + 1 < ccList.Count)
                                    {
                                        TempData["CompanyCodeIndex"] = (int)TempData.Peek("CompanyCodeIndex") + 1;
                                        TempData["CompanyCodeId"] = ccList[(int)TempData.Peek("CompanyCodeIndex")].CompanyCodeId;
                                        return RedirectToAction("Index", "MobileHome");
                                    }
                                }
                            }
                        }
                        TempData.Keep();
                        model.PunchType = -1;
                        model.SelectedPosition = -1;
                        return View(model);
                    }
                }
            return View();
        }

        /// <summary>
        /// Common function used to get employees active positions for the current pay period and pay group - Filter method
        /// </summary>
        /// <param name="posList">List of all employee positions</param>
        /// <returns>Active position list</returns>
        private List<E_PositioVm> GetActiveE_Positions(List<E_PositioVm> posList)
        {
            DateTime etcCurrentDate = Utils.ConvertTimeFromUtc(DateTime.UtcNow, ConfigurationManager.AppSettings["TimeZone"]);
            int payGroupId = Convert.ToInt32(TempData.Peek("PayGroupId"));
            PayPeriodVM empCurrentPayPeriod = (PayPeriodVM)(TempData.Peek("CurrentPayPeriod"));
            var activePositions = new List<E_PositioVm>();
            foreach (E_PositioVm pos in posList)
            {
                // Picking all positions which has same "Pay Group" as that of the student
                if ((pos.PayGroupId.HasValue) && (pos.PayGroupId.Value == payGroupId))
                {
                    bool flag = false;
                    if (pos.StartDate.HasValue && pos.actualEndDate.HasValue)
                    {
                        if ((DateTime.Compare(etcCurrentDate.Date, pos.StartDate.Value) >= 0) &&
                        (DateTime.Compare(etcCurrentDate.Date, pos.actualEndDate.Value) <= 0))
                        {
                            //flag = true;
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
                            else if ((DateTime.Compare(pos.StartDate.Value, empCurrentPayPeriod.StartDate) >= 0) && (DateTime.Compare(pos.actualEndDate.Value, empCurrentPayPeriod.EndDate) >= 0) && (DateTime.Compare(pos.StartDate.Value, empCurrentPayPeriod.EndDate) <= 0))
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
                            ((
                            (DateTime.Compare(pos.StartDate.Value, empCurrentPayPeriod.StartDate) <= 0) ||
                            ((DateTime.Compare(pos.StartDate.Value, empCurrentPayPeriod.StartDate) >= 0) &&
                            (DateTime.Compare(pos.StartDate.Value, empCurrentPayPeriod.EndDate) <= 0))) &&
                            (DateTime.Compare(etcCurrentDate.Date, pos.StartDate.Value) >= 0))
                        {
                            flag = true;
                        }
                    }
                    if (flag)
                        activePositions.Add(pos);
                }
            }
            return activePositions;
        }

        public ActionResult _PunchButtons(int employeeId, int? positionId, DateTime punchTime, int companyCodeId, string EpositionId)
        {
            if (EpositionId != null)
            {
                var epositionArr = EpositionId.Split('-');
                positionId = Convert.ToInt32(epositionArr[1]);
            }
            //DateTime etcCurrentDate = Utils.ConvertTimeFromUtc(DateTime.UtcNow, ConfigurationManager.AppSettings["TimeZone"]);
            employeeId = Convert.ToInt32(TempData.Peek("EmployeeId"));
            companyCodeId = Convert.ToInt32(TempData.Peek("CompanyCodeId"));

            if (IsSessionExpired)
                return new EmptyResult();
            bool disablePunchIn = false, disableLunchOut = false, disableLunchBack = false, disablePunchOut = false;
            bool nightShiftOn = false;
            int nightShiftTimeCardId = 0;
            // Get all timecards for that day
            //if(punchTime == null)
            //{
            //    punchTime = etcCurrentDate;
            //}
            List<TimeCard> todayTimeCard = _timecardMobileRepo.GetEmployeeTimeCardByDate(null, null, punchTime, null, (int)TempData.Peek("PersonId"));
            if ((todayTimeCard == null) || (todayTimeCard.Count == 0))
            {
                // If no timecards available then check for night shift timecard that starts previous day
                // Check for night shift timecard
                List<TimeCard> previousDayNightShiftTC = _timecardMobileRepo.GetEmployeeTimeCardByDate(null, null, punchTime.AddHours(-12), null, null);
                if ((previousDayNightShiftTC != null) && (previousDayNightShiftTC.Count > 0))
                {
                    TimeCard old = previousDayNightShiftTC.LastOrDefault();
                    if (!old.TimeOut.HasValue)
                    {
                        nightShiftOn = true;
                        nightShiftTimeCardId = old.TimeCardId;
                    }
                }
            }

            List<TimeCard> positionBasedTC = todayTimeCard.Where(p => p.PositionId == positionId && p.CompanyCodeId == companyCodeId).ToList();
            if ((positionBasedTC != null) && (positionBasedTC.Count > 0))
            {
                if (positionBasedTC.Count == 1)
                {
                    TimeCard t = positionBasedTC.FirstOrDefault();
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
                    TimeCard incompeteTimeCard = positionBasedTC.LastOrDefault();
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
            ViewData["nightShiftOn"] = ViewData["nightShiftOn"] = nightShiftOn == true ? 1 : 0;
            ViewData["nightShiftTimeCardId"] = nightShiftTimeCardId;
            return PartialView();
        }

        [HttpPost]
        public ActionResult AcceptPunch(AcceptPunchVM model)
        {
            var connString = User.Identity.GetClientConnectionString();
            var clientDbContext = new ClientDbContext(connString);
            if (String.IsNullOrEmpty(User.Identity.Name))
                return RedirectToAction("Login", "Account");
            if (!ModelState.IsValid)
                return View(model);
            if (model.EpositionId == "0")
            {
                model.EpositionId = Session["Epositionid"].ToString();
            }
            var Epositionid = model.EpositionId.Split('-');
            var epositionid = Convert.ToInt32(Epositionid[0]);
           Session["Epositionid"] = epositionid;
            var positionid = Convert.ToInt32(Epositionid[1]);
            DateTime? effdate = null;
            DateTime estTime = Utils.ConvertTimeFromUtc(DateTime.UtcNow, ConfigurationManager.AppSettings["TimeZone"]);
            model.EmployeeId = Convert.ToInt32(TempData.Peek("EmployeeId"));
            if ((model.NightShiftOn != null) && (model.NightShiftOn.Equals("1")) && (model.NightShiftTimeCardId.Length > 0))
            {
                if (_timecardMobileRepo.InsertEmployeePositionPunch(model.EmployeeId, positionid, model.PunchType, estTime, model.CompanyCodeId ?? 0, User.Identity.Name, model.FileName, model.NightShiftTimeCardId, (int)TempData.Peek("PersonId"), epositionid))
                    return RedirectToAction("Index", "MobileHome");
            }
            if (_timecardMobileRepo.InsertEmployeePositionPunch(model.EmployeeId, positionid, model.PunchType, estTime, model.CompanyCodeId ?? 0, User.Identity.Name, model.FileName, null, (int)TempData.Peek("PersonId"), epositionid))
            {
                if (model.PunchType == 0)
                {
                    var effdatecount = clientDbContext.E_PositionSalaryHistories.Where(x => x.E_PositionId == epositionid).Select(x => x.EffectiveDate).Count();
                    if (effdatecount > 0)
                    {
                        var effdate1 = clientDbContext.E_PositionSalaryHistories.Where(x => x.E_PositionId == epositionid).ToList();
                        if (effdate1.Count > 0)
                        {
                            var effdate2 = effdate1.Where(x => x.EndDate == null).ToList();
                            if (effdate2.Count > 0)
                            {
                                effdate = effdate2.Select(x => x.EffectiveDate).FirstOrDefault();
                            }
                            else
                            {
                                effdate = effdate1.Where(x => x.EndDate > DateTime.Now).Select(x => x.EffectiveDate).FirstOrDefault();
                            }
                        }
                    }
                   // var effdate = clientDbContext.E_PositionSalaryHistories.Where(x => x.E_PositionId == epositionid && x.EndDate == null).Select(x => x.EffectiveDate).FirstOrDefault();
                    if (effdate > DateTime.Now)
                    {
                        EmailResults(epositionid, model.EmployeeId);
                    }
                }
                return RedirectToAction("Index", "MobileHome");
            }
            else
            {
                TempData["WrongMessage"] = "Something went wrong; Please try again...";
                return RedirectToAction("Index", "MobileHome");
            }
        }

        // GET: MobileHome
        public ActionResult Summary()
        {
            if (IsSessionExpired)
                return RedirectToAction("Login", "Account");
            int empId = Convert.ToInt32(TempData.Peek("EmployeeId"));
            int payGroupId = Convert.ToInt32(TempData.Peek("PayGroupId"));
            int payPeriodId = Convert.ToInt32(TempData.Peek("PayPeriodId"));
            List<PayPeriodVM> employeePayPeriods = _ilookuprepo.GetPayPeriodsList(empId, false);
            employeePayPeriods = employeePayPeriods.Where(p => p.PayGroupCode.Value == payGroupId).ToList();
            ViewData["PayPeriods"] = employeePayPeriods;
            return View(employeePayPeriods);
        }

        public ActionResult _ViewSummary(int PayPeriodId)
        {
            if (IsSessionExpired)
                return new EmptyResult();
            int empId = 0;
            empId = Convert.ToInt32(TempData.Peek("EmployeeId"));
            int companycodeid = Convert.ToInt32(TempData.Peek("CompanyCodeId"));
            List<TimeCardSummaryVm> empTimeCards = _timecardMobileRepo.GetEmployeeTimeCardsByPayPeriodbyCompanycodeId(empId, PayPeriodId, false, companycodeid);
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

        public JsonResult CompanyCodeOnChange(int companyCodeId)
        {
            string connString = User.Identity.GetClientConnectionString();
            List<E_PositioVm> posList = new List<E_PositioVm>();
            object result = null;
            if (!String.IsNullOrEmpty(connString))
                using (ClientDbContext clientDbContext = new ClientDbContext(connString))
                {
                    Employee emp = AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name, companyCodeId);
                    posList = GetActiveE_Positions(_positionRepo.GetEPositionList_v2((int)TempData.Peek("PersonId"), emp.EmployeeId, companyCodeId));
                    TempData["EmployeeId"] = emp.EmployeeId;
                    TempData["CompanyCodeId"] = companyCodeId;
                    int deptid = Convert.ToInt32(TempData["DeptId"]);
                    int payperiodid=Convert.ToInt32(TempData["PayperiodId"]);
                    List<TimeCardWeekTotalCollectionVm> payPeriodTimeCardList = _timeCardsMatrixRepo.GetTimeCardWeekTotalList(emp.EmployeeId, deptid, payperiodid, false);                   
                    var payPeriodTotal = " 0.00";
                    if ((payPeriodTimeCardList != null) && (payPeriodTimeCardList.Count > 0))
                    {
                        payPeriodTotal = Convert.ToString(payPeriodTimeCardList.Sum(e => e.WeeklyTotal));
                    }
                    if (posList.Count > 0)
                    {
                        Session["Epositionid"] = posList[0].EpositionId;
                    }
                     result = new { posList = posList, payPeriodTotal = payPeriodTotal };
                    
                };
            return Json(result, JsonRequestBehavior.AllowGet);
        }




        public ActionResult EmailResults(int epositionid, int employeeId)
        {
            string constring = User.Identity.GetClientConnectionString();
            using (var clientDbContext = new ClientDbContext(constring))
            {
                DateTime? effdate = null;
                var Filenum = clientDbContext.Employees.Where(n => n.EmployeeId == employeeId).Select(s => s.FileNumber).FirstOrDefault();
                var ReportsToID = clientDbContext.E_Positions.Where(n => n.EmployeeId == employeeId).Select(s => s.ReportsToID).FirstOrDefault();
                var perid = clientDbContext.Employees.Where(n => n.EmployeeId == employeeId).Select(s => s.PersonId).FirstOrDefault();
                var personname = clientDbContext.Persons.Where(n => n.PersonId == perid).Select(s => s.Firstname + " " + s.Lastname).FirstOrDefault();
                var getManagername = clientDbContext.Persons.Where(n => n.PersonId == ReportsToID).Select(s => s.Firstname + " " + s.Lastname).FirstOrDefault();
                var effdatecount = clientDbContext.E_PositionSalaryHistories.Where(x => x.E_PositionId == epositionid).Select(x => x.EffectiveDate).Count();
                if (effdatecount > 0)
                {
                    var effdate1 = clientDbContext.E_PositionSalaryHistories.Where(x => x.E_PositionId == epositionid).ToList();
                    if (effdate1.Count > 0)
                    {
                        var effdate2 = effdate1.Where(x => x.EndDate == null).ToList();
                        if (effdate2.Count > 0)
                        {
                            effdate = effdate2.Select(x => x.EffectiveDate).FirstOrDefault();
                        }
                        else
                        {
                            effdate = effdate1.Where(x => x.EndDate > DateTime.Now).Select(x => x.EffectiveDate).FirstOrDefault();
                        }
                    }
                }
                // var effdate = clientDbContext.E_PositionSalaryHistories.Where(x => x.E_PositionId == epositionid).Select(x => x.EffectiveDate).FirstOrDefault();
                var effecdatetime = effdate.Value.ToShortDateString();
                var positionId = clientDbContext.E_Positions.Where(x => x.E_PositionId == epositionid).Select(x => x.PositionId).FirstOrDefault();
                var positiondetails = clientDbContext.Positions.Where(x => x.PositionId == positionId).Select(x => x.PositionDescription).FirstOrDefault();
                var punchdate = DateTime.Now.Date.ToShortDateString();
                StringBuilder sb = new StringBuilder();
                string strFrom = ConfigurationManager.AppSettings["fromAddress"].ToString();
                string strTo = ConfigurationManager.AppSettings["toAddress"].ToString();
                EmailProcessorCommunity.Send("", strFrom, strTo, "Log Hours before it is effective: " + personname + "(" + Filenum + ") - " + positiondetails, "Hello"
               + Environment.NewLine + Environment.NewLine
                + Environment.NewLine + Environment.NewLine
            + "<table  style ='width:100%;border: 1px solid #1a8099;border-collapse: collapse'>"
            + "<tr role = row style = 'background-color: #1a8099; color:#fff;border-top: 2px solid #1a8099;text-align: left;'>"
            + "<th style='border: 1px solid #fff;border-left: 2px solid #1a8099;'>Person Name</th>"
            + "<th style='border: 1px solid #fff;border-right: 3px solid #1a8099;'>File Number</th>"
            + "<th style='border: 1px solid #fff;border-right: 3px solid #1a8099;'>Position Details</th>"
            + "<th style='border: 1px solid #fff;border-right: 3px solid #1a8099;'>Supervisor Name</th>"
            + "<th style='border: 1px solid #fff;border-right: 3px solid #1a8099;'>Punch Date</th>"
            + "<th style='border: 1px solid #fff;border-right: 3px solid #1a8099;'>Effective Date</th>"
            + "</tr>"
            + "<tr>"
            + "<td style = 'border:1px solid #1a8099;'>" + personname + "</td>"
            + "<td style = 'border:1px solid #1a8099;'>" + Filenum + "</td>"
            + "<td style = 'border:1px solid #1a8099;'>" + positiondetails + "</td>"
            + "<td style = 'border:1px solid #1a8099;'>" + getManagername + "</td>"
            + "<td style = 'border:1px solid #1a8099;'>" + punchdate + "</td>"
            + "<td style = 'border:1px solid #1a8099;'>" + effecdatetime + "</td>"
            + "</tr>"
            + "</ table >"
            + Environment.NewLine + Environment.NewLine, true);

                return null;
            }
        }

    }
}