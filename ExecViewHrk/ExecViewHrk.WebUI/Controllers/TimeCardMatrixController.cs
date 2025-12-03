using ExecViewHrk.Domain.Helper;
using ExecViewHrk.Domain.Interface;
using ExecViewHrk.EfClient;
using ExecViewHrk.Models;
using ExecViewHrk.WebUI.Helpers;
//using ExecViewHrk.WebUI.Infrastructure;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
namespace ExecViewHrk.WebUI.Controllers
{
    public class TimeCardMatrixController : BaseController
    {
        readonly ITimeCardMatrixReposotory _timeCardsMatrixRepo;
        readonly ILookupTablesRepository _ilookuprepo;
        readonly ITimeCardSessionInOutRepository _itimecardssession;
        readonly ITimeCardApprovalReportRepository _iTimeCardApprovalReportRepository;

        public TimeCardMatrixController(ITimeCardMatrixReposotory timeCardsMatrixRepo,
                                        ILookupTablesRepository ilookuprepo,
                                        ITimeCardSessionInOutRepository itimecardssession,
                                        ITimeCardApprovalReportRepository iTimeCardApprovalReportRepository)
        {
            _timeCardsMatrixRepo = timeCardsMatrixRepo;
            _ilookuprepo = ilookuprepo;
            _itimecardssession = itimecardssession;
            _iTimeCardApprovalReportRepository = iTimeCardApprovalReportRepository;
        }

        public ActionResult TimeCardsMatrixPartial(bool IsArchived = false)
        {
            ViewData["ShowBack"] = false;
            ViewData["hourCodesList"] = _ilookuprepo.GetHourCodes();
            ViewData["earningCodesList"] = _ilookuprepo.GetEarningCodes();
            ViewData["tempDepartmentCodesList"] = _ilookuprepo.GetTempDepartmentCodes();
            ViewData["tempJobCodesList"] = _ilookuprepo.GetTempJobCodes();
            ViewData["employeePositionsList"] = _ilookuprepo.GetEmployeePositionList();
            ViewBag.MaxHours = Getsessionvalue();
            //ViewData["fundcodesList"] = _ilookuprepo.GetFunds();
            //ViewData["ProjectsList"] = _ilookuprepo.GetProjects();        
            //if (User.Identity.GetValue("IsEmployeeProjectsEnabled") == "1")
            //{
            //    ViewData["ProjectsList"] = _ilookuprepo.GetEmployeeProjects();
            //}
            //else
            //{
            //    ViewData["ProjectsList"] = _ilookuprepo.GetProjects();
            //}
            if (User.IsInRole("ClientEmployees"))
            {
                string connString = User.Identity.GetClientConnectionString();
                using (ClientDbContext clientDbContext = new ClientDbContext(connString))
                {
                    var EmpDetails = AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name);
                    if (EmpDetails != null)
                    {
                        ViewBag.Employeeid = EmpDetails.EmployeeId;
                        ViewBag.EmployeeName = EmpDetails.Person.Firstname + " " + EmpDetails.Person.Lastname;
                        ViewBag.IsStudent = EmpDetails.IsStudent;
                    }
                }
            }
            //ViewData["TasksList"] = _ilookuprepo.GetTaskList();
            //ViewData["AllProjectsList"] = _ilookuprepo.GetProjectsDropdownList(); //For bottom Grid ProjectPercentage
            TimeCardVm timeCardVm = new TimeCardVm();

            timeCardVm.IsArchived = IsArchived;
            timeCardVm.timeCardDislayColumns = _ilookuprepo.TimeCardInOutDisplayColumns((int)TimeCardsDisplay.TimeCard);
            return View(timeCardVm);
        }

        // Duplicate action method used when navigating from "TimeCard Unapproval Report" to "TimeCard" page
        public ActionResult TimeCardsMatrixPartialFromUnapproval(int companyCodeId, int departmentId, int employeeId, int payPeriodId, bool IsArchived = false)
        {
            ViewData["ShowBack"] = true;
            ViewData["hourCodesList"] = _ilookuprepo.GetHourCodes();
            ViewData["earningCodesList"] = _ilookuprepo.GetEarningCodes();
            ViewData["tempDepartmentCodesList"] = _ilookuprepo.GetTempDepartmentCodes();
            ViewData["tempJobCodesList"] = _ilookuprepo.GetTempJobCodes();
            ViewData["employeePositionsList"] = _ilookuprepo.GetEmployeePositionList();
            ViewBag.MaxHours = Getsessionvalue();
            if (User.IsInRole("ClientEmployees"))
            {
                string connString = User.Identity.GetClientConnectionString();
                using (ClientDbContext clientDbContext = new ClientDbContext(connString))
                {
                    var EmpDetails = AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name);
                    if (EmpDetails != null)
                    {
                        ViewBag.Employeeid = EmpDetails.EmployeeId;
                        ViewBag.EmployeeName = EmpDetails.Person.Firstname + " " + EmpDetails.Person.Lastname;
                        ViewBag.IsStudent = EmpDetails.IsStudent;
                    }
                }
            }
            TimeCardVm timeCardVm = new TimeCardVm();
            timeCardVm.IsArchived = IsArchived;
            timeCardVm.timeCardDislayColumns = _ilookuprepo.TimeCardInOutDisplayColumns((int)TimeCardsDisplay.TimeCard);
            return View("TimeCardsMatrixPartial", timeCardVm);
        }



        public ActionResult TimeCardsInAndOutMatrixPartial()
        {
            bool IsArchived = false;
            ViewData["hourCodesList"] = _ilookuprepo.GetHourCodes();
            ViewData["earningCodesList"] = _ilookuprepo.GetEarningCodes();
            ViewData["tempDepartmentCodesList"] = _ilookuprepo.GetTempDepartmentCodes();
            ViewData["tempJobCodesList"] = _ilookuprepo.GetTempJobCodes();
            ViewData["employeePositionsList"] = _ilookuprepo.GetEmployeePositionList();
            ViewBag.MaxHours = Getsessionvalue();
            // ViewData["fundcodesList"] = _ilookuprepo.GetFunds();
            //ViewData["ProjectsList"] = _ilookuprepo.GetProjects();
            // ViewData["ProjectsList"] = _ilookuprepo.GetEmployeeProjects();
            //if (User.Identity.GetValue("IsEmployeeProjectsEnabled") == "1")
            //{
            //    ViewData["ProjectsList"] = _ilookuprepo.GetEmployeeProjects();
            //}
            //else
            //{
            //    ViewData["ProjectsList"] = _ilookuprepo.GetProjects();
            //}
            //ViewData["TasksList"] = _ilookuprepo.GetTaskList();
            //ViewData["AllProjectsList"] = _ilookuprepo.GetProjectsDropdownList(); //For Bottom Grid - ProjectPercentage
            if (User.IsInRole("ClientEmployees"))
            {
                string connString = User.Identity.GetClientConnectionString();
                using (ClientDbContext clientDbContext = new ClientDbContext(connString))
                {
                    var EmpDetails = AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name);
                    if (EmpDetails != null)
                    {
                        ViewBag.Employeeid = EmpDetails.EmployeeId;
                        ViewBag.EmployeeName = EmpDetails.Person.Firstname + " " + EmpDetails.Person.Lastname;
                        ViewBag.IsStudent = EmpDetails.IsStudent;
                    }
                }
            }
            TimeCardVm timeCardVm = new TimeCardVm();
            timeCardVm.IsArchived = IsArchived;
            timeCardVm.timeCardDislayColumns = _ilookuprepo.TimeCardInOutDisplayColumns((int)TimeCardsDisplay.TimeCradInandOut);
            return View(timeCardVm);
        }

        #region Time Card Grid Read, Create, Update and Delete

        public ActionResult WeeksList_Read([DataSourceRequest]DataSourceRequest request, int? employeeIdDdl, int? payPeriodId, int? departmentId, bool IsArchived)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var employeeWeeklyTimeCardList = new List<TimeCardCollectionVm>();
                if (User.IsInRole("ClientEmployees"))
                {
                    //#3049: commented a student belongs to multiple companies and EmployeeId should get from the dropdown for the selected company
                    //employeeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).EmployeeId : employeeIdDdl;
                    departmentId = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).DepartmentId : employeeIdDdl;
                }
                string falsacode = string.Empty;

                //Returns the Employee Current Pay Period only. Most Recente Date Pay Period. Should be used in MobileController.
                //var employeecurrentpayperiod = _ilookuprepo.GetEmployeeCurrentPayPeriod(employeeIdDdl, IsArchived);

                if (employeeIdDdl.HasValue && payPeriodId.HasValue && IsArchived != null)
                {
                    try
                    {
                        /*//#3049: commented a student belongs to multiple companies and EmployeeId should get the from the dropdown for the selected company
                        if (User.IsInRole("ClientEmployees"))
                        {
                            var EmpDetails = AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name);
                            employeeIdDdl = EmpDetails.EmployeeId;
                        }
                        EmployeesVM empdetails = _timeCardsMatrixRepo.GetFlsaCode(employeeIdDdl.Value);
                        if (empdetails != null)
                        {
                            //   falsacode = empdetails.FlsaCode;
                        }
                        */

                        //COMMENTED AS THE LOGIC IS RELATED TO EMPLOYEE EXEMPT AND NON EXEMPT. AND IT IS CONFLICT WITH SESSION IN AND OUT CONFIGURATION.
                        /*
                        PayPeriodVM PayPeriodVM = _timeCardsMatrixRepo.GetPayPeriodDates(payPeriodId);
                        ViewBag.falsacode = falsacode;
                        switch (falsacode)
                        {
                            case "NE":
                                decimal? FTE = _timeCardsMatrixRepo.GetNonExemptEmployeeFTE(employeeIdDdl.Value);
                                if (FTE != null)
                                {
                                    if (PayPeriodVM.PayFrequencyName == "W" || PayPeriodVM.PayFrequencyName == "BW")
                                        employeeWeeklyTimeCardList = PrePopulateNonExemptTimeCardByFTE(FTE, payPeriodId.Value, employeeIdDdl.Value, departmentId.Value, IsArchived);
                                        //employeeWeeklyTimeCardList = PrePopulateNonExemptTimeCardByFTE(FTE, PayPeriodVM, employeeIdDdl.Value, departmentId.Value, IsArchived);
                                    else
                                        employeeWeeklyTimeCardList = _timeCardsMatrixRepo.GetTimeCardsList(employeeIdDdl.Value, payPeriodId.Value, IsArchived);
                                }
                                break;
                            case "E":
                                //Populate Weekly Time Card with 8 Hours.
                                if (PayPeriodVM.PayFrequencyName == "W" || PayPeriodVM.PayFrequencyName == "BW")
                                    employeeWeeklyTimeCardList = PrePopulateExemptEmployeeTimeCards(payPeriodId.Value, employeeIdDdl.Value, departmentId.Value, IsArchived);
                                    //employeeWeeklyTimeCardList = PrePopulateExemptEmployeeTimeCards(PayPeriodVM, employeeIdDdl.Value, departmentId.Value, IsArchived);
                                else
                                    employeeWeeklyTimeCardList = _timeCardsMatrixRepo.GetTimeCardsList(employeeIdDdl.Value, payPeriodId.Value, IsArchived);
                                break;
                            default:
                                employeeWeeklyTimeCardList = _timeCardsMatrixRepo.GetTimeCardsList(employeeIdDdl.Value, payPeriodId.Value, IsArchived);
                                break;
                        }
                        */

                        int? departmentid = departmentId == null ? departmentId : departmentId.Value;

                        employeeWeeklyTimeCardList = PrePopulateTimeCardsByPayperiod(payPeriodId.Value, employeeIdDdl.Value, departmentid, IsArchived);
                        foreach (var item in employeeWeeklyTimeCardList)
                        {
                            employeeWeeklyTimeCardList[0].Falsacode = falsacode;
                            item.Day = TimeZoneInfo.ConvertTimeToUtc(item.ActualDate, TimeZoneInfo.Utc);
                            item.ActualDate = TimeZoneInfo.ConvertTimeToUtc(item.ActualDate, TimeZoneInfo.Utc);
                            if (item.DailyHours != null)
                            {
                                item.DailyHours = Math.Round(item.DailyHours.Value, 2);
                            }
                            if (item.LineTotal != null)
                            {
                                item.LineTotal = Math.Round(item.LineTotal.Value, 2);
                            }
                            if (item.TimeIn != null)
                            {
                                item.TimeIn = TimeZoneInfo.ConvertTimeToUtc(item.TimeIn.Value, TimeZoneInfo.Utc);
                            }
                            if (item.LunchOut != null)
                            {
                                item.LunchOut = TimeZoneInfo.ConvertTimeToUtc(item.LunchOut.Value, TimeZoneInfo.Utc);
                            }
                            if (item.LunchBack != null)
                            {
                                item.LunchBack = TimeZoneInfo.ConvertTimeToUtc(item.LunchBack.Value, TimeZoneInfo.Utc);
                            }
                            if (item.TimeOut != null)
                            {
                                item.TimeOut = TimeZoneInfo.ConvertTimeToUtc(item.TimeOut.Value, TimeZoneInfo.Utc);
                            }
                            if (item.LastModifiedDate != null)
                            {
                                item.LastModifiedDate = TimeZoneInfo.ConvertTimeToUtc(item.LastModifiedDate.Value, TimeZoneInfo.Utc);
                            }
                            if (item.LineTotal > 0) { item.isChild = true; } else { item.isChild = false; }
                        }
                        if (User.IsInRole("HrkAdministrators") || User.IsInRole("ClientAdministrators"))
                        {
                            foreach (var employeeTC in employeeWeeklyTimeCardList)
                            {
                                employeeTC.ShowLineApprovedActive = true;
                            }
                        }
                        if (User.IsInRole("ClientManagers"))
                        {
                            bool checkEmployeeStatus = _timeCardsMatrixRepo.checkEmployeeStatus((int)departmentId, (int)employeeIdDdl);

                            if (checkEmployeeStatus)
                            {
                                foreach (var employeeTC in employeeWeeklyTimeCardList)
                                {
                                    employeeTC.ShowLineApprovedActive = employeeTC.TempDeptId == null || employeeTC.TempDeptId == 0 ? true : false;
                                }
                            }
                            else
                            {
                                foreach (var employeeTC in employeeWeeklyTimeCardList)
                                {
                                    employeeTC.ShowLineApprovedActive = employeeTC.TempDeptId == departmentId ? true : false;
                                }
                            }
                        }

                        ////Check whether Time Card is Approved
                        //var isTimeCardApproved = _iTimeCardApprovalReportRepository.GetTimeCardApprovals(employeeIdDdl.Value, payPeriodId.Value);
                        //if(isTimeCardApproved != null)
                        //{
                        //    ViewBag.isTimeCardApproved = "Approved";
                        //}
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", e.InnerException.Message);
                    }
                }
                //return Json(employeeWeeklyTimeCardList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
                return KendoCustomResult(employeeWeeklyTimeCardList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult WeeksTotalList_Read([DataSourceRequest]DataSourceRequest request, int? employeeIdDdl, int? departmentId, int? payPeriodId, bool IsArchived)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                int? flsaid = 0;
                var employeeWeeklyTimeCardList = new List<TimeCardWeekTotalCollectionVm>();
                try
                {
                    //#3017: commented a student belongs to multiple companies and EmployeeId should get the from the dropdown for the selected company
                    //employeeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).EmployeeId : employeeIdDdl;
                    //  flsaid = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).FLSAID : employeeIdDdl;

                    if (employeeIdDdl.HasValue && payPeriodId.HasValue && IsArchived != null)
                    {
                        employeeWeeklyTimeCardList = _timeCardsMatrixRepo.GetTimeCardWeekTotalList(employeeIdDdl.Value, departmentId, payPeriodId.Value, IsArchived);
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.InnerException.Message);
                }
                return Json(employeeWeeklyTimeCardList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult WeeksList_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<TimeCardVm> timeCardVmGridCollection,
            int? employeeIdDdl, int? companyCodeIdDdl, int? departmentId, int? payPeriodId, bool isArchived)
        {
            var timeCardList = new List<TimeCard>();
            string connString = User.Identity.GetClientConnectionString();
            int? flsaid = 0;
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (timeCardVmGridCollection != null && ModelState.IsValid)
                {
                    if (User.IsInRole("ClientEmployees"))
                    {
                        var EmpDetails = AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name);
                        companyCodeIdDdl = EmpDetails.CompanyCodeId;
                        employeeIdDdl = EmpDetails.EmployeeId;
                        // flsaid = EmpDetails.FLSAID;
                    }
                    else
                    {
                        // flsaid = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).FLSAID : employeeIdDdl;
                    }
                    Dictionary<DateTime, int> Date_MaxProjectNum = _timeCardsMatrixRepo.GetDateMaxProjectNum(employeeIdDdl, companyCodeIdDdl);
                    try
                    {
                        //string validate = ValidateTimeCards(employeeIdDdl, payPeriodId, isArchived, flsaid, timeCardVmGridCollection);
                        string validate = ValidateTimeCards(employeeIdDdl, payPeriodId, isArchived, timeCardVmGridCollection);
                        if (validate == "")
                        {
                            int maxProjectNum = 1;
                           // bool Isvalid = false;
                            DateTime? effdate = null;
                            Session["Isvalid"] = false;
                            bool isValidPunchDate = true;
                            if (employeeIdDdl != null)
                            {
                                var timeCardsList = new List<TimeCard>();
                                foreach (var timeCardVm in timeCardVmGridCollection)
                                {
                                    if (timeCardVm.PositionId != null)
                                    {
                                        var epositionlist = _timeCardsMatrixRepo.GetEpositionID(employeeIdDdl, timeCardVm.PositionId, companyCodeIdDdl, payPeriodId);
                                        var epositionid = epositionlist.Select(x => x.E_PositionId).FirstOrDefault();
                                        var epStartDate = clientDbContext.E_Positions.Where(x => x.E_PositionId == epositionid).Select(x => x.StartDate).FirstOrDefault();
                                        var epEndDate = clientDbContext.E_Positions.Where(x => x.E_PositionId == epositionid).Select(x => x.actualEndDate).FirstOrDefault();
                                        var epEffectiveDate = clientDbContext.E_PositionSalaryHistories.Where(x => x.E_PositionId == epositionid && x.EffectiveDate != null).OrderByDescending(x => x.E_PositionSalaryHistoryId).Select(x => x.EffectiveDate).FirstOrDefault();
                                        if (epEndDate == null)
                                        {
                                            epEndDate = DateTime.Now;
                                        }
                                        if(epEffectiveDate != null)
                                         {
                                            isValidPunchDate = (timeCardVm.ActualDate.Date >= epStartDate.Value.Date && timeCardVm.ActualDate.Date <= epEndDate.Value.Date); /*&& timeCardVm.ActualDate.Date >= epEffectiveDate.Value.Date;*/ /*changes for future effective date*/
                                         }
                                        if (!isValidPunchDate)
                                        {
                                            ModelState.AddModelError("", "Please review Student's Position record. Either Position is not started or Position is ended or Rate Effective is not active for entered time record.");
                                        }
  
                                        var effdatecount = clientDbContext.E_PositionSalaryHistories.Where(x => x.E_PositionId == epositionid).Select(x => x.EffectiveDate).Count();
                                        if (effdatecount > 0)
                                        {
                                           var effdate1 = clientDbContext.E_PositionSalaryHistories.Where(x => x.E_PositionId == epositionid).ToList();
                                            if(effdate1.Count > 0)
                                            {
                                                var effdate2 = effdate1.Where(x => x.EndDate == null).ToList();
                                                if(effdate2.Count>0)
                                                {
                                                    effdate = effdate2.Select(x=>x.EffectiveDate).FirstOrDefault();
                                                }
                                                else
                                                {
                                                    effdate = effdate1.Where(x => x.EndDate >DateTime.Now).Select(x => x.EffectiveDate).FirstOrDefault();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            effdate = clientDbContext.E_PositionSalaryHistories.Where(x => x.E_PositionId == epositionid).Select(x => x.EffectiveDate).FirstOrDefault();
                                        }
                                        //comment as QA not require on 06/07/2019
                                        //if (effdate != null)
                                        //{
                                        //    if (effdate > DateTime.Now)
                                        //    {
                                        //       // Isvalid = true;
                                        //        Session["Isvalid"] = true;
                                        //        ModelState.AddModelError("", "Pay Rate Effective Date is not active for an approved day. An administrative review of the student’s position record is needed.");
                                        //    }
                                        //}
                                    }
                                }
                                bool isvalid =Convert.ToBoolean(Session["Isvalid"]);                            
                                if (!isvalid && isValidPunchDate)
                                {
                                    timeCardList = _timeCardsMatrixRepo.SaveTimeCardsList(timeCardVmGridCollection, Date_MaxProjectNum, maxProjectNum, employeeIdDdl.Value, departmentId, companyCodeIdDdl.Value, User.Identity.Name, payPeriodId);
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", validate);
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        ViewBag.AlertMessage = "";
                        IEnumerable<DbEntityValidationResult> errors = clientDbContext.GetValidationErrors();
                        if (errors.Count() == 0)
                            ModelState.AddModelError("", err.InnerException.Message);
                        else
                        {
                            foreach (DbEntityValidationResult error in errors)
                            {
                                foreach (var valError in error.ValidationErrors)
                                {
                                    ModelState.AddModelError("", valError.ErrorMessage);
                                }
                            }
                        }
                    }
                }
            }
            return Json(timeCardList.ToDataSourceResult(request, ModelState, timeCard => new TimeCard
            {
                TimeCardId = timeCard.TimeCardId,
                CompanyCodeId = timeCard.CompanyCodeId,
                EmployeeId = timeCard.EmployeeId,
                ActualDate = timeCard.ActualDate,
                ProjectNumber = timeCard.ProjectNumber,
                DailyHours = timeCard.DailyHours,
                HoursCodeId = timeCard.HoursCodeId,
                Hours = timeCard.Hours,
                EarningsCodeId = timeCard.EarningsCodeId,
                EarningsAmount = timeCard.EarningsAmount,
                TempDeptId = timeCard.TempDeptId,
                TempJobId = timeCard.TempJobId,
                IsApproved = timeCard.IsApproved,
                TimeIn = timeCard.TimeIn == null ? null : ReplaceWithActualDate(timeCard.ActualDate, timeCard.TimeIn),
                LunchOut = timeCard.LunchOut == null ? null : ReplaceWithActualDate(timeCard.ActualDate, timeCard.LunchOut),
                LunchBack = timeCard.LunchBack == null ? null : ReplaceWithActualDate(timeCard.ActualDate, timeCard.LunchBack),
                TimeOut = timeCard.TimeOut == null ? null : ReplaceWithActualDate(timeCard.ActualDate, timeCard.TimeOut),
            }));
        }


        [HttpPost]
        public ActionResult WeeksList_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<TimeCardVm> timeCardVmGridCollection,
            int? employeeIdDdl, int? companyCodeIdDdl, int? departmentId, int? payPeriodId, bool isArchived)
        {
            var timeCardList = new List<TimeCardVm>();
            int? flsaid = 0;
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (timeCardVmGridCollection != null && ModelState.IsValid)
                {
                    if (User.IsInRole("ClientEmployees"))
                    {
                        var EmpDetails = AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name);

                        companyCodeIdDdl = EmpDetails.CompanyCodeId;
                        employeeIdDdl = EmpDetails.EmployeeId;
                        //  var excempt = EmpDetails.FLSAID;
                        //flsaid = EmpDetails.FLSAID;
                    }
                    else
                    {
                        //flsaid = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).FLSAID : employeeIdDdl;
                    }
                    Dictionary<DateTime, int> Date_MaxProjectNum = _timeCardsMatrixRepo.GetDateMaxProjectNum(employeeIdDdl, companyCodeIdDdl);
                    int maxProjectNum = 1;
                    try
                    {
                        //string validate = ValidateTimeCards(employeeIdDdl, payPeriodId, isArchived, flsaid, timeCardVmGridCollection);
                        string validate = ValidateTimeCards(employeeIdDdl, payPeriodId, isArchived, timeCardVmGridCollection);
                        if (validate == "")
                        {
                            // bool Isvalid = false;
                            Session["Isvalid"] = false;
                            bool isValidPunchDate = true;
                            DateTime? effdate = null;
                            var timeCardsList = new List<TimeCard>();
                         
                            foreach (var timeCardVm in timeCardVmGridCollection)
                            {
                                if (timeCardVm.PositionId != null)
                                {
                                    var epositionlist = _timeCardsMatrixRepo.GetEpositionID(employeeIdDdl, timeCardVm.PositionId, companyCodeIdDdl, payPeriodId);
                                    var epositionid = epositionlist.Select(x => x.E_PositionId).FirstOrDefault();
                                    var epStartDate = clientDbContext.E_Positions.Where(x => x.E_PositionId == epositionid).Select(x => x.StartDate).FirstOrDefault();
                                    var epEndDate = clientDbContext.E_Positions.Where(x => x.E_PositionId == epositionid).Select(x => x.actualEndDate).FirstOrDefault();
                                    var epEffectiveDate = clientDbContext.E_PositionSalaryHistories.Where(x => x.E_PositionId == epositionid && x.EffectiveDate != null).OrderByDescending(x => x.E_PositionSalaryHistoryId).Select(x => x.EffectiveDate).FirstOrDefault();
                                    if(epEndDate == null)
                                    {
                                        epEndDate = DateTime.Now;
                                    }
                                    if (epEffectiveDate != null)
                                    {
                                        isValidPunchDate = (timeCardVm.ActualDate.Date >= epStartDate.Value.Date && timeCardVm.ActualDate.Date <= epEndDate.Value.Date); /*&& timeCardVm.ActualDate.Date >= epEffectiveDate.Value.Date;*/ /*changes for future effective date*/
                                    }
                                    if (!isValidPunchDate)
                                    {
                                        ModelState.AddModelError("", "Punch Date should between Start Date and End Date/Punch Date greater than or equal to Effective Date");
                                    }                                 
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
                                    else
                                    {
                                        effdate = clientDbContext.E_PositionSalaryHistories.Where(x => x.E_PositionId == epositionid).Select(x => x.EffectiveDate).FirstOrDefault();
                                    }
                                        if (effdate != null)
                                        {
                                            if (effdate > DateTime.Now)
                                            {
                                            //Isvalid = true;
                                            Session["Isvalid"] = true;
                                            ModelState.AddModelError("", "Pay Rate Effective Date is not active for an approved day. An administrative review of the student’s position record is needed.");
                                            }
                                        }
                                }
                            }
                            bool isvalid = Convert.ToBoolean(Session["Isvalid"]);
                          
                            if (!isvalid && isValidPunchDate)
                            {
                                timeCardList = _timeCardsMatrixRepo.UpdateTimeCardsList(timeCardVmGridCollection, Date_MaxProjectNum, maxProjectNum, employeeIdDdl.Value, departmentId, companyCodeIdDdl.Value, User.Identity.Name, payPeriodId);

                            }
                            }
                        else
                        {
                            ModelState.AddModelError("", validate);
                        }
                    }
                    catch (Exception err)
                    {
                        ViewBag.AlertMessage = "";
                        IEnumerable<DbEntityValidationResult> errors = clientDbContext.GetValidationErrors();
                        if (errors.Count() == 0)
                            ModelState.AddModelError("", err.InnerException.Message);
                        else
                        {
                            foreach (DbEntityValidationResult error in errors)
                            {
                                foreach (var valError in error.ValidationErrors)
                                {
                                    ModelState.AddModelError("", valError.ErrorMessage);
                                }
                            }
                        }
                    }
                }
            }
            return Json(timeCardList.ToDataSourceResult(request, ModelState, timeCardVm => new TimeCardVm
            {
                TimeCardId = timeCardVm.TimeCardId,
                CompanyCodeId = timeCardVm.CompanyCodeId,
                EmployeeId = timeCardVm.EmployeeId,
                ActualDate = timeCardVm.ActualDate,
                ProjectNumber = timeCardVm.ProjectNumber,
                DailyHours = timeCardVm.DailyHours,
                HoursCodeId = timeCardVm.HoursCodeId == 0 ? null : timeCardVm.HoursCodeId,
                Hours = timeCardVm.Hours,
                EarningsCodeId = timeCardVm.EarningsCodeId == 0 ? null : timeCardVm.EarningsCodeId,
                EarningsAmount = timeCardVm.EarningsAmount,
                TempDeptId = timeCardVm.TempDeptId == 0 ? null : timeCardVm.TempDeptId,
                TempJobId = timeCardVm.TempJobId == 0 ? null : timeCardVm.TempJobId,
                IsLineApproved = timeCardVm.IsLineApproved,
                TimeIn = timeCardVm.TimeIn == null ? null : ReplaceWithActualDate(timeCardVm.ActualDate, timeCardVm.TimeIn),
                LunchOut = timeCardVm.LunchOut == null ? null : ReplaceWithActualDate(timeCardVm.ActualDate, timeCardVm.LunchOut),
                LunchBack = timeCardVm.LunchBack == null ? null : ReplaceWithActualDate(timeCardVm.ActualDate, timeCardVm.LunchBack),
                TimeOut = timeCardVm.TimeOut == null ? null : ReplaceWithActualDate(timeCardVm.ActualDate, timeCardVm.TimeOut),
            }));
        }

        [HttpPost]
        public ActionResult TimecardsDelete(int timecardId)
        {
            bool succeed = false;
            try
            {
                //var actualDate = DateTime.Parse(new string(activeDate.Take(24).ToArray()));
                succeed = _timeCardsMatrixRepo.DeleteTimeCards(timecardId, User.Identity.Name);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, succeed }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Message = "Please Save the record then click on delete", succeed }, JsonRequestBehavior.AllowGet);
        }

        private DateTime? ReplaceWithActualDate(DateTime actualDate, DateTime? actualDateTime)
        {
            actualDateTime = Convert.ToDateTime(actualDate.ToString().Substring(0, actualDate.ToString().IndexOf(" ")) +
                                     " " + actualDateTime.ToString().Remove(0, actualDateTime.ToString().IndexOf(" ")));
            return actualDateTime;
        }


        #endregion

        //#region Employee Projects

        //public ActionResult GetEmployeeProjectsbyEmployeeId([DataSourceRequest] DataSourceRequest request, int? employeeIdDdl)
        //{
        //    string connString = User.Identity.GetClientConnectionString();
        //    using (ClientDbContext clientDbContext = new ClientDbContext(connString))
        //    {
        //        employeeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).EmployeeId : employeeIdDdl;
        //    }
        //    var projectsList = _ilookuprepo.GetEmployeeProjectsbyEmployeeId(employeeIdDdl);
        //    return Json(projectsList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult ProjectsPercentage_Read([DataSourceRequest] DataSourceRequest request, int? employeeIdDdl)
        //{
        //    string connString = User.Identity.GetClientConnectionString();
        //    using (ClientDbContext clientDbContext = new ClientDbContext(connString))
        //    {
        //        employeeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).EmployeeId : employeeIdDdl;
        //    }
        //    var projectsList = _ilookuprepo.GetEmployeeProjectPercentage(employeeIdDdl);

        //    return Json(projectsList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult ProjectsPercentage_Create([DataSourceRequest] DataSourceRequest request, EmployeeProjectPercentageVM model, int? employeeIdDdl)
        //{
        //    if (model != null && ModelState.IsValid)
        //    {
        //        string connString = User.Identity.GetClientConnectionString();
        //        using (ClientDbContext clientDbContext = new ClientDbContext(connString))
        //        {
        //            employeeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).EmployeeId : employeeIdDdl;
        //        }
        //        using (var clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString()))
        //        {
        //            var row = new ProjectPercentage { EmployeeID = employeeIdDdl, ProjectID = model.ProjectID, Percentage = model.Percentage };

        //            clientDbContext.ProjectPercentages.Add(row);
        //            clientDbContext.SaveChanges();
        //            model.EmployeeID = row.EmployeeID;
        //            model.ProjectPercentageID = row.ProjectPercentageID;
        //        }
        //    }

        //    return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult ProjectsPercentage_Update([DataSourceRequest] DataSourceRequest request, EmployeeProjectPercentageVM model, int employeeIdDdl)
        //{
        //    if (model != null && ModelState.IsValid)
        //    {

        //        using (var clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString()))
        //        {
        //            var row = clientDbContext.ProjectPercentages.Where(x => x.ProjectPercentageID == model.ProjectPercentageID).FirstOrDefault();

        //            if (row != null)
        //            {
        //                row.ProjectID = model.ProjectID;
        //                row.Percentage = model.Percentage;
        //            }
        //            clientDbContext.SaveChanges();
        //        }
        //    }
        //    return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult ProjectsPercentage_Delete(int projectPercentageID)
        //{
        //    bool succeed = false;
        //    try
        //    {
        //        succeed = _timeCardsMatrixRepo.DeleteProjectPercentage(projectPercentageID);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Message = ex.Message, succeed }, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(new { Message = "Please Save the record then click on delete", succeed }, JsonRequestBehavior.AllowGet);
        //}

        //#endregion

        #region Timeoff Summary

        [HttpPost]
        public JsonResult GetTimeoffSummary_Read([DataSourceRequest] DataSourceRequest request, int? employeeIdDdl)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                employeeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).EmployeeId : employeeIdDdl;
            }
            var timeoffSummaryList = _timeCardsMatrixRepo.GetTimeoffSummaryList(employeeIdDdl);
            return Json(timeoffSummaryList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region ValidateTimeCards

        private bool ValidateTime(TimeCardVm Timecardsvm)
        {
            //Sets the Actual Date to Time Fields
            Timecardsvm.TimeIn = Timecardsvm.TimeIn == null ? null : replaceWithActualDate(Timecardsvm.ActualDate, Timecardsvm.TimeIn);
            Timecardsvm.TimeOut = Timecardsvm.TimeOut == null ? null : replaceWithActualDate(Timecardsvm.ActualDate, Timecardsvm.TimeOut);
            Timecardsvm.LunchOut = Timecardsvm.LunchOut == null ? null : replaceWithActualDate(Timecardsvm.ActualDate, Timecardsvm.LunchOut);
            Timecardsvm.LunchBack = Timecardsvm.LunchBack == null ? null : replaceWithActualDate(Timecardsvm.ActualDate, Timecardsvm.LunchBack);

            bool result = true;
            TimeCardDisplayColumn timeconfig = _ilookuprepo.TimeCardInOutDisplayColumns((int)TimeCardsDisplay.TimeCard);
            if (timeconfig.TimeIn == true && timeconfig.TimeOut == true)
            {
                if (Timecardsvm.TimeOut != null && Timecardsvm.TimeIn != null)
                {
                    if (Timecardsvm.TimeOut >= Timecardsvm.TimeIn)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                        return false;
                    }
                }
                else
                {
                    result = true;
                }
            }
            if (timeconfig.LunchOut == true && timeconfig.LunchBack == true)
            {
                if (Timecardsvm.LunchBack > Timecardsvm.LunchOut || Timecardsvm.LunchBack == null || Timecardsvm == null)
                {
                    result = true;
                }
                else

                {
                    result = false;
                }
            }
            return result;
        }

        //public string ValidateTimeCards(int? employeeIdDdl, int? payPeriodId, bool IsArchived, int? flsaid, IEnumerable<TimeCardVm> timeCardInOutVmGridCollection)
        public string ValidateTimeCards(int? employeeIdDdl, int? payPeriodId, bool IsArchived, IEnumerable<TimeCardVm> timeCardInOutVmGridCollection)
        {
            string result = "";
            PayPeriodVM PayPeriodVM = _timeCardsMatrixRepo.GetPayPeriodDates(payPeriodId);
            //double totalhours = _timeCardsMatrixRepo.gettotalhours(employeeIdDdl.Value, PayPeriodVM.StartDate, PayPeriodVM.EndDate);
            var empTimeCardList = _timeCardsMatrixRepo.GetTimeCardsList(employeeIdDdl, payPeriodId, IsArchived);

            foreach (var item in timeCardInOutVmGridCollection)
            {
                //double codedhours = 0;
                //double dailyhours = 0;
                //if (item.Hours != null)
                //    codedhours = item.Hours.Value;
                //if (item.DailyHours != null)
                //    dailyhours = item.DailyHours.Value;
                //totalhours = totalhours + codedhours + dailyhours;
                //if (totalhours > 80)
                //{
                //    resullt = "Total Hours can not excced 80 hrs in the payperiod";
                //    //return resullt;
                //}
                var empTimeCardCount = empTimeCardList.Where(x => x.ActualDate == item.ActualDate).ToList();
                if (empTimeCardCount.Count != 0)
                {
                    bool isOverlapping = OverlappingHours(item, empTimeCardCount, null, out result);
                    if (isOverlapping)
                    {
                        return result;
                    }
                }

                if (PayPeriodVM.StartDate.Date <= item.ActualDate.Date && PayPeriodVM.EndDate.Date >= item.ActualDate.Date)
                {
                }
                else
                {
                    result = result + "<br>" + "Date Is Out Side of Pay Period Range";
                    //return resullt;
                }
                bool validatetime = ValidateTime(item);
                if (validatetime == false)
                {
                    result = result + "<br>" + "Out Time must be Greater Than In Time.";
                    return result;
                }
                //if(item.TimeIn != null && item.TimeOut != null)
                //{
                //    if(item.PositionId == null)
                //        resullt = resullt + "<br>" + "Position is required. Please select the Position";
                //}
            }
            return result;
        }

        private bool OverlappingHours(TimeCardVm timeCardVm, List<TimeCardCollectionVm> empTimeCardList, DateTime? actualDate, out string message)
        {
            bool result = false;
            message = "";
            DateTime ActualDate = actualDate ?? timeCardVm.ActualDate;
            if (timeCardVm.TimeIn != null && empTimeCardList.Count != 0)
            {
                if (actualDate == null)
                {
                    timeCardVm.TimeIn = timeCardVm.TimeIn == null ? null : replaceWithActualDate(timeCardVm.ActualDate, timeCardVm.TimeIn);
                    timeCardVm.TimeOut = timeCardVm.TimeOut == null ? null : replaceWithActualDate(timeCardVm.ActualDate, timeCardVm.TimeOut);
                }
                else
                {
                    timeCardVm.TimeIn = timeCardVm.TimeIn == null ? null : replaceWithActualDate(actualDate.Value, timeCardVm.TimeIn);
                    timeCardVm.TimeOut = timeCardVm.TimeOut == null ? null : replaceWithActualDate(actualDate.Value, timeCardVm.TimeOut);
                }
                foreach (var item in empTimeCardList)
                {
                    if (timeCardVm.TimeCardId != item.TimeCardId)
                    {
                        if (timeCardVm.TimeIn >= item.TimeIn && timeCardVm.TimeIn < item.TimeOut)
                        {
                            result = true;
                            message = "Time In / Time Out Overlap with Other/Same Position.";
                            break;
                        }
                        if (timeCardVm.TimeOut != null)
                        {
                            if (timeCardVm.TimeOut > item.TimeIn && timeCardVm.TimeOut <= item.TimeOut)
                            {
                                result = true;
                                message = "Time In / Time Out Overlap with Other/Same Position.";
                                break;
                            }
                            if (timeCardVm.TimeIn < item.TimeIn && timeCardVm.TimeOut > item.TimeIn)
                            {
                                result = true;
                                message = "Time In / Time Out Overlap with Other/Same Position.";
                                break;
                            }
                        }
                    }
                }
            }
            return result;
        }

        public string ValidateTimeCards1(int? employeeIdDdl, int? payPeriodId, bool IsArchived, IEnumerable<TimeCardVm> timeCardInOutVmGridCollection, DateTime actualDate)
        {
            string result = "";
            PayPeriodVM PayPeriodVM = _timeCardsMatrixRepo.GetPayPeriodDates(payPeriodId);
            //double totalhours = _timeCardsMatrixRepo.gettotalhours(employeeIdDdl.Value, PayPeriodVM.StartDate, PayPeriodVM.EndDate);
            var empTimeCardList = _timeCardsMatrixRepo.GetTimeCardsList(employeeIdDdl, payPeriodId, IsArchived);

            foreach (var item in timeCardInOutVmGridCollection)
            {
                //double codedhours = 0;
                //double dailyhours = 0;
                //if (item.Hours != null)
                //    codedhours = item.Hours.Value;
                //if (item.DailyHours != null)
                //    dailyhours = item.DailyHours.Value;
                //totalhours = totalhours + codedhours + dailyhours;
                //if (totalhours > 80)
                //{
                //    resullt = "Total Hours can not excced 80 hrs in the payperiod";
                //    //return resullt;
                //}
                var empTimeCardCount = empTimeCardList.Where(x => x.ActualDate == actualDate).ToList();
                if (empTimeCardCount.Count != 0)
                {
                    bool isOverlapping = OverlappingHours(item, empTimeCardCount, actualDate, out result);
                    if (isOverlapping)
                    {
                        return result;
                    }
                }
                bool validatetime = ValidateTime(item);
                if (validatetime == false)
                {
                    result = result + "<br>" + "Out Time must be Greater Than In Time.";
                    return result;
                }

            }
            return result;
        }

        private DateTime? replaceWithActualDate(DateTime actualDate, DateTime? actualDateTime)
        {
            actualDateTime = Convert.ToDateTime(actualDate.ToString().Substring(0, actualDate.ToString().IndexOf(" ")) +
                                     " " + actualDateTime.ToString().Remove(0, actualDateTime.ToString().IndexOf(" ")));
            if (actualDateTime != null)
            {
                actualDateTime = TimeZoneInfo.ConvertTimeToUtc(actualDateTime.Value, TimeZoneInfo.Utc);
            }
            return actualDateTime;
        }
        #endregion

        #region Exempt and Non-Exempt Prepopulate Time Card -CAUSHI & JIFF
        //Non Exempt Employee - Pre populate FTE with 2.5 = 2 Hours, FTE with 0.5 = 4 Hours, FTE with 1 = 8 Hours
        /*
        private List<TimeCardCollectionVm> PrePopulateTimeCards(int payPeriodId, int EmployeeId, int DepartmentId, bool IsArchived)
        {

            double FTE = 0.5;
            List<TimeCardCollectionVm> timecardsList = new List<TimeCardCollectionVm>();
            var EmpTimecardDetailsLits = new List<TimeCardCollectionVm>();
            PayPeriodVM PayPeriodVM = _timeCardsMatrixRepo.GetPayPeriodDates(payPeriodId);
            DateTime startdate = PayPeriodVM.StartDate;
            DateTime enddate = PayPeriodVM.EndDate;
            EmpTimecardDetailsLits = _timeCardsMatrixRepo.GetTimeCardsList(EmployeeId, payPeriodId, IsArchived);
            if (PayPeriodVM.PayFrequencyName == "BW")
            {
                if (FTE == 0.5)
                {
                    int days = (int)(enddate - startdate).TotalDays;
                    for (int i = 0; i <= days; i++)
                    {
                        if (i > 0)
                        {
                            startdate = startdate.AddDays(1);
                        }
                        TimeCardCollectionVm tcdvm = new TimeCardCollectionVm();

                        foreach (var item in EmpTimecardDetailsLits)
                        {
                            if (startdate == item.ActualDate)
                            {
                                tcdvm.TimeCardId = item.TimeCardId;
                                tcdvm.DepartmentId = item.DepartmentId;
                                tcdvm.EarningsAmount = item.EarningsAmount;
                                tcdvm.EarningsCodeId = item.EarningsCodeId;
                                tcdvm.EmployeeId = item.EmployeeId;
                                tcdvm.FundsId = item.FundsId;
                                tcdvm.Hours = item.Hours;
                                tcdvm.HoursCodeId = item.HoursCodeId;
                                tcdvm.IsLineApproved = item.IsLineApproved;
                                tcdvm.ProjectNumber = item.ProjectNumber;
                                tcdvm.ProjectsId = item.ProjectsId;
                                tcdvm.TaskId = item.TaskId;
                                tcdvm.TempDeptId = item.TempDeptId;
                                tcdvm.TempJobCode = item.TempJobCode;
                                tcdvm.TempJobId = item.TempJobId;
                                tcdvm.TimeIn = item.TimeIn;
                                tcdvm.TimeOut = item.TimeOut;
                                tcdvm.LunchOut = item.LunchBack;
                                tcdvm.LineTotal = item.LineTotal;
                                tcdvm.ShowLineApprovedActive = item.ShowLineApprovedActive;

                            }

                        }
                        tcdvm.Day = startdate;

                        tcdvm.ActualDate = startdate;
                        string dayofweek = tcdvm.ActualDate.DayOfWeek.ToString();
                        tcdvm.DailyHours = 4;
                        if (dayofweek != "Sunday" && dayofweek != "Saturday")
                            timecardsList.Add(tcdvm);
                    }
                }
                if (FTE == 1)
                {
                    int days = (int)(enddate - startdate).TotalDays;
                    for (int i = 1; i <= days; i++)
                    {
                        if (i > 0)
                        {
                            startdate = startdate.AddDays(1);
                        }
                        TimeCardCollectionVm tcdvm = new TimeCardCollectionVm();
                        foreach (var item in EmpTimecardDetailsLits)
                        {
                            if (startdate == item.ActualDate)
                            {
                                tcdvm.TimeCardId = item.TimeCardId;
                                tcdvm.DepartmentId = item.DepartmentId;
                                tcdvm.EarningsAmount = item.EarningsAmount;
                                tcdvm.EarningsCodeId = item.EarningsCodeId;
                                tcdvm.EmployeeId = item.EmployeeId;
                                tcdvm.FundsId = item.FundsId;
                                tcdvm.Hours = item.Hours;
                                tcdvm.HoursCodeId = item.HoursCodeId;
                                tcdvm.IsLineApproved = item.IsLineApproved;
                                tcdvm.ProjectNumber = item.ProjectNumber;
                                tcdvm.ProjectsId = item.ProjectsId;
                                tcdvm.TaskId = item.TaskId;
                                tcdvm.TempDeptId = item.TempDeptId;
                                tcdvm.TempJobCode = item.TempJobCode;
                                tcdvm.TempJobId = item.TempJobId;
                                tcdvm.TimeIn = item.TimeIn;
                                tcdvm.TimeOut = item.TimeOut;
                                tcdvm.LunchOut = item.LunchBack;
                                tcdvm.LineTotal = item.LineTotal;
                                tcdvm.ShowLineApprovedActive = item.ShowLineApprovedActive;
                            }
                        }
                        tcdvm.Day = startdate;
                        tcdvm.ActualDate = startdate;
                        string dayofweek = tcdvm.ActualDate.DayOfWeek.ToString();
                        tcdvm.DailyHours = 8;
                        if (dayofweek != "Sunday" && dayofweek != "Saturday")
                            timecardsList.Add(tcdvm);
                    }
                }
            }
            return timecardsList;
        }
        */

        //Non Exempt Employee - Pre populate Time Card by reading FTE * 8 Hours.  
        /*
        private List<TimeCardCollectionVm> PrePopulateNonExemptTimeCardByFTE(decimal? FTE, int? payPeriodId, int EmployeeId, int DepartmentId, bool IsArchived)
        {
            if (FTE == null)
                return null; //Show alert for No FTE is there for Non Exempt Employee.

            List<TimeCardCollectionVm> timecardsList = new List<TimeCardCollectionVm>();
            var EmpTimecardDetailsLits = new List<TimeCardCollectionVm>();
            PayPeriodVM PayPeriodVM = _timeCardsMatrixRepo.GetPayPeriodDates(payPeriodId);
            //int payPeriodId = PayPeriodVM.PayPeriodId;
            DateTime startdate = PayPeriodVM.StartDate;
            DateTime enddate = PayPeriodVM.EndDate;
            EmpTimecardDetailsLits = _timeCardsMatrixRepo.GetTimeCardsList(EmployeeId, payPeriodId, IsArchived);
            if (PayPeriodVM.PayFrequencyName == "W" || PayPeriodVM.PayFrequencyName == "BW")
            {
                int days = (int)(enddate - startdate).TotalDays;
                for (int i = 0; i <= days; i++)
                {
                    if (i > 0)
                    {
                        startdate = startdate.AddDays(1);
                    }
                    TimeCardCollectionVm tcdvm = new TimeCardCollectionVm();
                    tcdvm.DailyHours = Convert.ToDouble(FTE * 8);
                    foreach (var item in EmpTimecardDetailsLits)
                    {
                        if (startdate == item.ActualDate)
                        {
                            tcdvm.TimeCardId = item.TimeCardId;
                            tcdvm.DepartmentId = item.DepartmentId;
                            tcdvm.EarningsAmount = item.EarningsAmount;
                            tcdvm.EarningsCodeId = item.EarningsCodeId;
                            tcdvm.EmployeeId = item.EmployeeId;
                            tcdvm.FundsId = item.FundsId;
                            tcdvm.Hours = item.Hours;
                            tcdvm.HoursCodeId = item.HoursCodeId;
                            tcdvm.IsLineApproved = item.IsLineApproved;
                            tcdvm.ProjectNumber = item.ProjectNumber;
                            tcdvm.ProjectsId = item.ProjectsId;
                            tcdvm.TaskId = item.TaskId;
                            tcdvm.TempDeptId = item.TempDeptId;
                            tcdvm.TempJobCode = item.TempJobCode;
                            tcdvm.TempJobId = item.TempJobId;
                            tcdvm.TimeIn = item.TimeIn;
                            tcdvm.TimeOut = item.TimeOut;
                            tcdvm.LunchOut = item.LunchBack;
                            tcdvm.LineTotal = item.LineTotal;
                            tcdvm.ShowLineApprovedActive = item.ShowLineApprovedActive;
                            tcdvm.DailyHours = item.DailyHours;
                        }
                    }
                    tcdvm.Day = startdate;

                    tcdvm.ActualDate = startdate;
                    string dayofweek = tcdvm.ActualDate.DayOfWeek.ToString();
                    //tcdvm.DailyHours =Convert.ToDouble(FTE * 8);
                    if (dayofweek != "Sunday" && dayofweek != "Saturday")
                        timecardsList.Add(tcdvm);
                }
            }
            return timecardsList;
        }
        */

        //Exempt Employee - Pre populate 8 Hours
        /*
        private List<TimeCardCollectionVm> PrePopulateExemptEmployeeTimeCards(int? payPeriodId, int EmployeeId, int DepartmentId, bool IsArchived)
        {
            List<TimeCardCollectionVm> timecardsList = new List<TimeCardCollectionVm>();
            var EmpTimecardDetailsLits = new List<TimeCardCollectionVm>();
            PayPeriodVM PayPeriodVM = _timeCardsMatrixRepo.GetPayPeriodDates(payPeriodId);
            //int payPeriodId = PayPeriodVM.PayPeriodId;
            DateTime startdate = PayPeriodVM.StartDate;
            DateTime enddate = PayPeriodVM.EndDate;

            EmpTimecardDetailsLits = _timeCardsMatrixRepo.GetTimeCardsList(EmployeeId, payPeriodId, IsArchived);

            if (PayPeriodVM.PayFrequencyName == "W" || PayPeriodVM.PayFrequencyName == "BW")
            {
                int days = (int)(enddate - startdate).TotalDays;
                for (int i = 0; i <= days; i++)
                {
                    if (i > 0)
                    {
                        startdate = startdate.AddDays(1);
                    }
                    TimeCardCollectionVm tcdvm = new TimeCardCollectionVm();
                    tcdvm.DailyHours = 8;
                    foreach (var item in EmpTimecardDetailsLits)
                    {
                        if (startdate == item.ActualDate)
                        {
                            tcdvm.TimeCardId = item.TimeCardId;
                            tcdvm.DepartmentId = item.DepartmentId;
                            tcdvm.EarningsAmount = item.EarningsAmount;
                            tcdvm.EarningsCodeId = item.EarningsCodeId;
                            tcdvm.EmployeeId = item.EmployeeId;
                            tcdvm.FundsId = item.FundsId;
                            tcdvm.Hours = item.Hours;
                            tcdvm.HoursCodeId = item.HoursCodeId;
                            tcdvm.IsLineApproved = item.IsLineApproved;
                            tcdvm.ProjectNumber = item.ProjectNumber;
                            tcdvm.ProjectsId = item.ProjectsId;
                            tcdvm.TaskId = item.TaskId;
                            tcdvm.TempDeptId = item.TempDeptId;
                            tcdvm.TempJobCode = item.TempJobCode;
                            tcdvm.TempJobId = item.TempJobId;
                            tcdvm.TimeIn = item.TimeIn;
                            tcdvm.TimeOut = item.TimeOut;
                            tcdvm.LunchOut = item.LunchBack;
                            tcdvm.LineTotal = item.LineTotal;
                            tcdvm.ShowLineApprovedActive = item.ShowLineApprovedActive;
                            tcdvm.DailyHours = item.DailyHours;
                        }
                    }
                    tcdvm.Day = startdate;
                    tcdvm.ActualDate = startdate;
                    string dayofweek = tcdvm.ActualDate.DayOfWeek.ToString();
                    //tcdvm.DailyHours = 8;
                    if (dayofweek != "Sunday" && dayofweek != "Saturday")
                        timecardsList.Add(tcdvm);
                }
            }
            return timecardsList;
        }
        */

        #endregion 

        #region PREPOPULATE TIMECARD FOR DrewUniversity

        private List<TimeCardCollectionVm> PrePopulateTimeCardsByPayperiod(int? payPeriodId, int EmployeeId, int? DepartmentId, bool IsArchived)
        {
            List<TimeCardCollectionVm> timecardsList = new List<TimeCardCollectionVm>();
            var EmpTimecardDetailsLits = new List<TimeCardCollectionVm>();
            PayPeriodVM PayPeriodVM = _timeCardsMatrixRepo.GetPayPeriodDates(payPeriodId);
            //int payPeriodId = PayPeriodVM.PayPeriodId;
            DateTime startdate = PayPeriodVM.StartDate;
            DateTime enddate = PayPeriodVM.EndDate;
            string loggedInUserId = User.Identity.Name;

            if (User.IsInRole("ClientEmployees") || User.IsInRole("HrkAdministrators") || User.IsInRole("ClientAdministrators"))
            {
                EmpTimecardDetailsLits = _timeCardsMatrixRepo.GetTimeCardsList(EmployeeId, payPeriodId, IsArchived);
            }
            else
            {
                EmpTimecardDetailsLits = _timeCardsMatrixRepo.GetTimeCardsListReportToPositions(EmployeeId, payPeriodId, IsArchived, loggedInUserId);
            }
            //if (PayPeriodVM.PayFrequencyName == "W" || PayPeriodVM.PayFrequencyName == "BW")
            //{
            int days = (int)(enddate - startdate).TotalDays;
            for (int i = 0; i <= days; i++)
            {
                if (i > 0)
                {
                    startdate = startdate.AddDays(1);
                }
                TimeCardCollectionVm tcdvm = new TimeCardCollectionVm();
                foreach (var item in EmpTimecardDetailsLits)
                {
                    if (startdate.Date == item.ActualDate.Date)
                    {
                        tcdvm.TimeCardId = item.TimeCardId;
                        tcdvm.DepartmentId = item.DepartmentId;
                        tcdvm.EarningsAmount = item.EarningsAmount;
                        tcdvm.EarningsCodeId = item.EarningsCodeId;
                        tcdvm.EmployeeId = item.EmployeeId;
                        tcdvm.FundsId = item.FundsId;
                        tcdvm.Hours = item.Hours;
                        tcdvm.HoursCodeId = item.HoursCodeId;
                        tcdvm.IsLineApproved = item.IsLineApproved;
                        tcdvm.ProjectNumber = item.ProjectNumber;
                        tcdvm.ProjectsId = item.ProjectsId;
                        tcdvm.TaskId = item.TaskId;
                        tcdvm.TempDeptId = item.TempDeptId;
                        tcdvm.TempJobCode = item.TempJobCode;
                        tcdvm.TempJobId = item.TempJobId;
                        tcdvm.TimeIn = item.TimeIn;
                        tcdvm.TimeOut = item.TimeOut;
                        tcdvm.LunchOut = item.LunchOut;
                        tcdvm.LunchBack = item.LunchBack;
                        tcdvm.LineTotal = item.LineTotal;
                        tcdvm.ShowLineApprovedActive = item.ShowLineApprovedActive;
                        tcdvm.DailyHours = item.DailyHours;
                        tcdvm.PositionId = item.PositionId;
                        tcdvm.UserId = item.UserId;
                        tcdvm.LastModifiedDate = item.LastModifiedDate;
                        tcdvm.ApprovedBy = item.ApprovedBy;
                        tcdvm.E_PositionId = item.E_PositionId;
                    }
                }
                tcdvm.Day = startdate;
                tcdvm.ActualDate = startdate.Date;
                string dayofweek = tcdvm.ActualDate.DayOfWeek.ToString();
                //tcdvm.DailyHours = 8;
                //if (dayofweek != "Sunday" && dayofweek != "Saturday")
                timecardsList.Add(tcdvm);
                // }
            }
            return timecardsList;
        }

        public int Getsessionvalue()
        {
            int maxhours = 0;
            var timeCardColumnList = _itimecardssession.GetTimeCardSessionList();
            foreach (var item in timeCardColumnList)
            {
                if (item.Toggle == true)
                {
                    maxhours = item.MaxHours.Value;
                }
            }
            return maxhours;
        }

        #endregion

        #region Send Email

        public JsonResult GetEmployeeEmailId(int? employeeIdDdl)
        {
            string emailId = _ilookuprepo.GetEmployeeEmailId(employeeIdDdl);
            return Json(emailId, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SendEmailtoEmployee(string MailToAddress, string message)
        {
            try
            {
                var subject = User.Identity.Name + "-" + "CollegeTime";
                EmailProcessorCommunity.SendEmail("", "CollegeTimeNOREPLY@resnav.com", MailToAddress, subject, message, true);
            }
            catch (Exception ex)
            {
                string Message = ex.Message;
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Timecard Notes

        public ActionResult GetTimecardNotes(int? timecardId)
        {
            var timeCardsNotesVM = _ilookuprepo.GetTimecardNotes(timecardId);
            return View("TimecardNotesDetails", timeCardsNotesVM);
        }

        public ActionResult TimecardNotes_SaveAjax(TimeCardsNotesVM timeCardsNotesVM)
        {
            bool result = _ilookuprepo.TimecardNotes_SaveAjax(timeCardsNotesVM);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Send SMS

        public JsonResult GetEmployeeMobileNumber(int? employeeIdDdl)
        {
            PersonPhoneNumberVm mobileNumber = _ilookuprepo.GetEmployeeMobileNumber(employeeIdDdl);
            return Json(mobileNumber, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SendSMStoEmployee(string Number, string message)
        {
            try
            {                      
                EmailProcessorCommunity.Send("", "No-Reply@Resnav.com", Number, "", message, true);
            }
            catch (Exception ex)
            {
                string Message = ex.Message;
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Sub Grid to Show Multiple Posistio for the same Date
        [HttpPost]
        public ActionResult Grid_ReadChild([DataSourceRequest] DataSourceRequest request, string ActualDate, int timeCard, int? employeeIdDdl, int? departmentId, int? payPeriodId, bool isArchived)
        {
            string connString = User.Identity.GetClientConnectionString();
            List<TimeCardVm> timecardsList = new List<TimeCardVm>();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (User.IsInRole("ClientEmployees"))
                {
                    //#3049: commented a student belongs to multiple companies and EmployeeId should get the from the dropdown for the selected company
                    //employeeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).EmployeeId : employeeIdDdl;
                    departmentId = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).DepartmentId : employeeIdDdl;
                }
                string falsacode = string.Empty;
                var varActualDate = DateTime.Parse(new string(ActualDate.Take(24).ToArray()));
                string loggedInUserId = User.Identity.Name;

                if (User.IsInRole("ClientEmployees") || User.IsInRole("HrkAdministrators") || User.IsInRole("ClientAdministrators"))
                {
                    timecardsList = _timeCardsMatrixRepo.Grid_ReadChild(employeeIdDdl ?? 0, varActualDate).Where(r => r.TimeCardId != timeCard).ToList();
                }
                else
                {
                    timecardsList = _timeCardsMatrixRepo.Grid_ReadChildPoistionByManager(employeeIdDdl ?? 0, payPeriodId, isArchived, loggedInUserId, varActualDate).Where(r => r.TimeCardId != timeCard).ToList();
                }

                foreach (var item in timecardsList)
                {

                    item.Day = TimeZoneInfo.ConvertTimeToUtc(item.ActualDate, TimeZoneInfo.Utc);
                    item.ActualDate = TimeZoneInfo.ConvertTimeToUtc(item.ActualDate, TimeZoneInfo.Utc);
                    if (item.DailyHours != null)
                    {
                        item.DailyHours = Math.Round(item.DailyHours.Value, 2);
                    }
                    if (item.LineTotal != null)
                    {
                        item.LineTotal = Math.Round(item.LineTotal.Value, 2);
                    }
                    if (item.TimeIn != null)
                    {
                        item.TimeIn = TimeZoneInfo.ConvertTimeToUtc(item.TimeIn.Value, TimeZoneInfo.Utc);
                    }
                    if (item.LunchOut != null)
                    {
                        item.LunchOut = TimeZoneInfo.ConvertTimeToUtc(item.LunchOut.Value, TimeZoneInfo.Utc);
                    }
                    if (item.LunchBack != null)
                    {
                        item.LunchBack = TimeZoneInfo.ConvertTimeToUtc(item.LunchBack.Value, TimeZoneInfo.Utc);
                    }
                    if (item.TimeOut != null)
                    {
                        item.TimeOut = TimeZoneInfo.ConvertTimeToUtc(item.TimeOut.Value, TimeZoneInfo.Utc);
                    }
                    if (item.LastModifiedDate != null)
                    {
                        item.LastModifiedDate = TimeZoneInfo.ConvertTimeToUtc(item.LastModifiedDate.Value, TimeZoneInfo.Utc);
                    }
                }
                if (User.IsInRole("HrkAdministrators") || User.IsInRole("ClientAdministrators"))
                {
                    foreach (var employeeTC in timecardsList)
                    {
                        employeeTC.ShowLineApprovedActive = true;
                    }
                }
                if (User.IsInRole("ClientManagers"))
                {
                    bool checkEmployeeStatus = _timeCardsMatrixRepo.checkEmployeeStatus((int)departmentId, (int)employeeIdDdl);

                    if (checkEmployeeStatus)
                    {
                        foreach (var employeeTC in timecardsList)
                        {
                            employeeTC.ShowLineApprovedActive = employeeTC.TempDeptId == null || employeeTC.TempDeptId == 0 ? true : false;
                        }
                    }
                    else
                    {
                        foreach (var employeeTC in timecardsList)
                        {
                            employeeTC.ShowLineApprovedActive = employeeTC.TempDeptId == departmentId ? true : false;
                        }
                    }
                }
                ViewData["hourCodesList"] = _ilookuprepo.GetHourCodes();
                ViewData["employeePositionsList"] = _ilookuprepo.GetEmployeePositionList();

                return Json(timecardsList.ToDataSourceResult(request));
            }


        }
        
        [HttpPost]
        public ActionResult Grid_UpdateChild([DataSourceRequest] DataSourceRequest request, string ActualDate, [Bind(Prefix = "models")]IEnumerable<TimeCardVm> timeCardVmGridCollection,
            int? employeeIdDdl, int? companyCodeIdDdl, int? departmentId, int? payPeriodId, bool isArchived)
        {
            Dictionary<DateTime, int> Date_MaxProjectNum = _timeCardsMatrixRepo.GetDateMaxProjectNum(employeeIdDdl, companyCodeIdDdl);
            int maxProjectNum = 1;
            var actualDate = DateTime.Parse(new string(ActualDate.Take(24).ToArray()));
            string validate = ValidateTimeCards1(employeeIdDdl, payPeriodId, isArchived, timeCardVmGridCollection, actualDate);
            List<TimeCardVm> timecardsList = new List<TimeCardVm>();
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (timeCardVmGridCollection != null && ModelState.IsValid)
                {
                    if (User.IsInRole("ClientEmployees"))
                    {
                        var EmpDetails = AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name);
                        companyCodeIdDdl = EmpDetails.CompanyCodeId;
                        employeeIdDdl = EmpDetails.EmployeeId;
                    }
                    if (validate == "")
                    {
                        bool Isvalid = false;
                        DateTime? effdate = null;
                        if (employeeIdDdl != null)
                        {
                            var timeCardsList = new List<TimeCard>();
                            foreach (var timeCardVm in timeCardVmGridCollection)
                            {
                                if (timeCardVm.PositionId != null)
                                {
                                    var epositionlist = _timeCardsMatrixRepo.GetEpositionID(employeeIdDdl, timeCardVm.PositionId, companyCodeIdDdl,payPeriodId);
                                    var epositionid = epositionlist.Select(x => x.E_PositionId).FirstOrDefault();
                                    var effdatecount = clientDbContext.E_PositionSalaryHistories.Where(x => x.E_PositionId == epositionid && x.EffectiveDate != null).Select(x => x.EffectiveDate).Count();
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
                                    else
                                    {
                                        effdate = clientDbContext.E_PositionSalaryHistories.Where(x => x.E_PositionId == epositionid).Select(x => x.EffectiveDate).FirstOrDefault();
                                    }
                                    if (effdate != null)
                                    {
                                        if (effdate > DateTime.Now)
                                        {
                                            Isvalid = true;
                                            ModelState.AddModelError("", "Pay Rate Effective Date is not active for an approved day. An administrative review of the student’s position record is needed.");
                                        }
                                    }

                                }
                            }
                        }
                            if (Isvalid == false)
                            {

                                timecardsList = _timeCardsMatrixRepo.Grid_UpdateChild(timeCardVmGridCollection, Date_MaxProjectNum, maxProjectNum, employeeIdDdl, companyCodeIdDdl, departmentId, payPeriodId, isArchived, User.Identity.Name, actualDate);
                            }
                    }
                    else
                    {
                        ModelState.AddModelError("", validate);
                    }
                }
            }
            return Json(timecardsList.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public ActionResult Grid_CreateChild([DataSourceRequest] DataSourceRequest request, string ActualDate, [Bind(Prefix = "models")]IEnumerable<TimeCardVm> timeCardVmGridCollection,
           int? employeeIdDdl, int? companyCodeIdDdl, int? departmentId, int? payPeriodId, bool isArchived)
        {
            var timeCardList = new List<TimeCardVm>();
            string connString = User.Identity.GetClientConnectionString();
            var actualDate = DateTime.Parse(new string(ActualDate.Take(24).ToArray()));

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (timeCardVmGridCollection != null && ModelState.IsValid)
                {
                    if (User.IsInRole("ClientEmployees"))
                    {
                        var EmpDetails = AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name);
                        companyCodeIdDdl = EmpDetails.CompanyCodeId;
                        employeeIdDdl = EmpDetails.EmployeeId;
                    }

                    Dictionary<DateTime, int> Date_MaxProjectNum = _timeCardsMatrixRepo.GetDateMaxProjectNum(employeeIdDdl, companyCodeIdDdl);
                    int maxProjectNum = 1;
                    try
                    {
                        string validate = ValidateTimeCards1(employeeIdDdl, payPeriodId, isArchived, timeCardVmGridCollection, actualDate);
                        if (validate == "")
                        {
                            bool Isvalid = false;
                            DateTime? effdate = null;
                            if (employeeIdDdl != null)
                            {
                                var timeCardsList = new List<TimeCard>();
                                foreach (var timeCardVm in timeCardVmGridCollection)
                                {
                                    if (timeCardVm.PositionId != null)
                                    {
                                        var epositionlist = _timeCardsMatrixRepo.GetEpositionID(employeeIdDdl, timeCardVm.PositionId, companyCodeIdDdl, payPeriodId);
                                        var epositionid = epositionlist.Select(x => x.E_PositionId).FirstOrDefault();
                                        var effdatecount = clientDbContext.E_PositionSalaryHistories.Where(x => x.E_PositionId == epositionid && x.EffectiveDate != null).Select(x => x.EffectiveDate).Count();
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
                                        else
                                        {
                                            effdate = clientDbContext.E_PositionSalaryHistories.Where(x => x.E_PositionId == epositionid).Select(x => x.EffectiveDate).FirstOrDefault();
                                        }
                                        if (effdate != null)
                                        {
                                            if (effdate > DateTime.Now)
                                            {
                                                Isvalid = true;
                                                ModelState.AddModelError("", "Pay Rate Effective Date is not active for an approved day. An administrative review of the student’s position record is needed.");
                                            }
                                        }

                                    }
                                }

                                if (Isvalid == false)
                                {

                                    timeCardList = _timeCardsMatrixRepo.Grid_CreateChild(timeCardVmGridCollection, Date_MaxProjectNum, maxProjectNum, employeeIdDdl, companyCodeIdDdl, departmentId, payPeriodId, isArchived, User.Identity.Name, actualDate);
                                }
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", validate);
                        }
                    }
                    catch (Exception err)
                    {
                        ViewBag.AlertMessage = "";
                        IEnumerable<DbEntityValidationResult> errors = clientDbContext.GetValidationErrors();
                        if (errors.Count() == 0)
                            ModelState.AddModelError("", err.InnerException.Message);
                        else
                        {
                            foreach (DbEntityValidationResult error in errors)
                            {
                                foreach (var valError in error.ValidationErrors)
                                {
                                    ModelState.AddModelError("", valError.ErrorMessage);
                                }
                            }
                        }
                    }
                }
            }
            return Json(timeCardList.ToDataSourceResult(request, ModelState));
        }


        //Color of icon change if it has extra hours and Check approve limits exceeds session time.

        public ActionResult GetTimeCardsList(int? employeeIdDdl, int? payPeriodId, bool? IsArchived, int? departmentId)
        {
            List<TimeCardCollectionVm> empTimeCardList = new List<TimeCardCollectionVm>();
           
            if (User.IsInRole("ClientEmployees") || User.IsInRole("HrkAdministrators") || User.IsInRole("ClientAdministrators"))
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
             //   int personId = User.Identity.GetRequestType() == "IsSelfService" ? clientDbContext.Employees.Where(x => x.Person.eMail == User.Identity.Name).Select(x => x.PersonId).SingleOrDefault()
             //: Convert.ToInt32(SessionStateHelper.Get(SessionStateKeys.EMPLOYEE_PERSON_SELECTED_ID));
             //   if(employeeIdDdl==0)
             //   {
             //       employeeIdDdl = personId;
             //   }
                //employeeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).EmployeeId : employeeIdDdl;
              
                empTimeCardList = _timeCardsMatrixRepo.GetTimeCardsList(employeeIdDdl, payPeriodId, IsArchived);
            }
            else
            {
                empTimeCardList = _timeCardsMatrixRepo.GetTimeCardsListReportToPositions(employeeIdDdl, payPeriodId, IsArchived, User.Identity.Name);
            }
            var groupbyActualDate = empTimeCardList.GroupBy(p => p.ActualDate, p => p.ActualDate, (key, g) => new { ActualDate = key.ToString("MM-dd-yyyy"), Count = g.Count() });
            return Json(groupbyActualDate, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Sum of All Weeks, and Week wise Approved TimeCard Hours
        public ActionResult GetSumofTimeCardsList(int? employeeIdDdl, int? payPeriodId, bool IsArchived, int? maxhours)
        {
            List<TimeCardCollectionVm> empTimeCardList = new List<TimeCardCollectionVm>();
            if (User.IsInRole("ClientEmployees") || User.IsInRole("HrkAdministrators") || User.IsInRole("ClientAdministrators"))
            {
                empTimeCardList = _timeCardsMatrixRepo.GetTimeCardsList(employeeIdDdl, payPeriodId, IsArchived);
            }
            else
            {
                empTimeCardList = _timeCardsMatrixRepo.GetTimeCardsListReportToPositions(employeeIdDdl, payPeriodId, IsArchived, User.Identity.Name);
            }
            var sumofDailyHours = empTimeCardList.Sum(x => x.DailyHours);// empTimeCardList.Sum(x => x.DailyHours + (x.Hours ?? 0));
            if (sumofDailyHours > maxhours)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSumofWeekWiseApprovedTotal(int? employeeIdDdl, int? payPeriodId, bool IsArchived, int? maxhours)
        {
            PayPeriodVM PayPeriodVM = _timeCardsMatrixRepo.GetPayPeriodDates(payPeriodId);
            //int payPeriodId = PayPeriodVM.PayPeriodId;
            DateTime startdate = PayPeriodVM.StartDate;
            DateTime WeekOneEndDate = PayPeriodVM.StartDate.AddDays(6);
            DateTime WeekTwoStartDate = WeekOneEndDate.AddDays(1);
            DateTime enddate = PayPeriodVM.EndDate;

            List<TimeCardCollectionVm> weekoneempTimeCardList = new List<TimeCardCollectionVm>();
            weekoneempTimeCardList = _timeCardsMatrixRepo.GetTimeCardsList(employeeIdDdl, payPeriodId, IsArchived)
                                                                    .Where(x => x.ActualDate >= startdate && x.ActualDate <= WeekOneEndDate
                                                                                                          && x.IsLineApproved == true).ToList();
            List<TimeCardCollectionVm> weektwoempTimeCardList = new List<TimeCardCollectionVm>();
            weektwoempTimeCardList = _timeCardsMatrixRepo.GetTimeCardsList(employeeIdDdl, payPeriodId, IsArchived)
                                                                    .Where(x => x.ActualDate >= WeekTwoStartDate && x.ActualDate <= enddate
                                                                                                                 && x.IsLineApproved == true).ToList();
            var weekOneTotalHours = weekoneempTimeCardList.Sum(x => x.DailyHours);
            var weekTwoTotalHours = weektwoempTimeCardList.Sum(x => x.DailyHours);
            var result = Json(new { weekOneTotalHours, weekTwoTotalHours }, JsonRequestBehavior.AllowGet);
            return result;
        }

        public ActionResult GetSumofTotalApprovedTotal(int? employeeIdDdl, int? payPeriodId, bool IsArchived)
        {
            PayPeriodVM PayPeriodVM = _timeCardsMatrixRepo.GetPayPeriodDates(payPeriodId);
            DateTime startdate1 = PayPeriodVM.StartDate;
            DateTime enddate1 = PayPeriodVM.StartDate.AddDays(6);
            DateTime startdate2 = PayPeriodVM.StartDate.AddDays(7);
            DateTime enddate2 = PayPeriodVM.EndDate;
            var weekList = _timeCardsMatrixRepo.GetTimeCardsList(employeeIdDdl, payPeriodId, IsArchived).ToList();
            //var week1 = weekList.Where(x => x.ActualDate >= startdate1 && x.ActualDate <= enddate1).Sum(x => x.DailyHours + (x.Hours ?? 0));
            //var week2 = weekList.Where(x => x.ActualDate >= startdate2 && x.ActualDate <= enddate2).Sum(x => x.DailyHours + (x.Hours ?? 0));
            var week1 = weekList.Where(x => x.ActualDate >= startdate1 && x.ActualDate <= enddate1).Sum(x => x.DailyHours);
            var week2 = weekList.Where(x => x.ActualDate >= startdate2 && x.ActualDate <= enddate2).Sum(x => x.DailyHours);
            return Json(new { week1 = Math.Round(week1.Value, 2), week2 = Math.Round(week2.Value, 2) }, JsonRequestBehavior.AllowGet);
        }


        /*
        public ActionResult GetSumofWeekWiseTotal(int? employeeIdDdl, int? payPeriodId, bool IsArchived, int? maxhours)
        {
            PayPeriodVM PayPeriodVM = _timeCardsMatrixRepo.GetPayPeriodDates(payPeriodId);
            //int payPeriodId = PayPeriodVM.PayPeriodId;
            DateTime startdate = PayPeriodVM.StartDate;
            DateTime WeekOneEndDate = PayPeriodVM.StartDate.AddDays(6);
            DateTime WeekTwoStartDate = WeekOneEndDate.AddDays(1);
            DateTime enddate = PayPeriodVM.EndDate;

            List<TimeCardCollectionVm> weekoneempTimeCardList = new List<TimeCardCollectionVm>();
            weekoneempTimeCardList = _timeCardsMatrixRepo.GetTimeCardsList(employeeIdDdl, payPeriodId, IsArchived)
                                                                    .Where(x => x.ActualDate >= startdate && x.ActualDate <= WeekOneEndDate).ToList();
            List<TimeCardCollectionVm> weektwoempTimeCardList = new List<TimeCardCollectionVm>();
            weektwoempTimeCardList = _timeCardsMatrixRepo.GetTimeCardsList(employeeIdDdl, payPeriodId, IsArchived)
                                                                    .Where(x => x.ActualDate >= WeekTwoStartDate && x.ActualDate <= enddate).ToList();
            var weekOneTotalHours = weekoneempTimeCardList.Sum(x => x.DailyHours + (x.Hours ?? 0));
            var weekTwoTotalHours = weektwoempTimeCardList.Sum(x => x.DailyHours + (x.Hours ?? 0));
            var result = Json(new { weekOneTotalHours, weekTwoTotalHours }, JsonRequestBehavior.AllowGet);
            return result;
        }
        */

        #endregion

        #region Lockout
        [HttpPost]
        //public ActionResult GetPayPeriodLockOutStatus(int EmployeeId, DateTime PayStartDate, DateTime PayEndDate)
        //{
        //    string connString = User.Identity.GetClientConnectionString();
        //    var ModelList = new TimeCardVm();
        //    using (ClientDbContext clientDbContext = new ClientDbContext(connString))
        //    {
        //        var PayPeriodLock = clientDbContext.PayPeriods.Where(x => x.StartDate == PayStartDate && x.EndDate == PayEndDate).Select(i => new { i.LockoutEmployees, i.LockoutManagers }).FirstOrDefault();

        //        if (PayPeriodLock != null)
        //        {
        //            ModelList.LockEmployee = PayPeriodLock.LockoutEmployees;
        //            ModelList.LockManger = PayPeriodLock.LockoutManagers;
        //        }
        //    }
        //    return Json(ModelList, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult GetPayPeriodLockOutStatus(int payPeriodId, int employeeID)
        {
            string connString = User.Identity.GetClientConnectionString();
            var clientDbContext = new ClientDbContext(connString);
            var ModelList = new TimeCardVm();
            var userName = User.Identity.GetPersonName();
            var PayPeriodLock = clientDbContext.PayPeriods.Where(x => x.PayPeriodId == payPeriodId).Select(i => new { i.LockoutEmployees, i.LockoutManagers, i.StartDate, i.EndDate }).SingleOrDefault();
            //var PayPeriodList = clientDbContext.EmployeeSubmission.Where(x => x.EmployeeId == EmployeeId && x.PayPriodStartDate == PayPeriodLock.StartDate && x.PayPeriodEndDate == PayPeriodLock.EndDate).FirstOrDefault();
            var mgrlock = clientDbContext.ManagerLockouts.Where(x => x.PayPeriodId == payPeriodId && x.ManagerUserName == userName).Count();
            //var Flasa = clientDbContext.Employees.Where(x => x.EmployeeId == EmployeeId).Select(p => new { p.FLSAID }).FirstOrDefault();
            //ModelList.FlasaId = Flasa.FLSAID;
            //if (PayPeriodList != null)
            //{
            //    ModelList.MgrApproveDate = PayPeriodList.MgrApproveDate.ToString();
            //    ModelList.MgrUnApproveDate = PayPeriodList.MgrUnApproveDate.ToString();
            //}
            if (PayPeriodLock != null)
            {
                ModelList.LockEmployee = PayPeriodLock.LockoutEmployees;
                ModelList.LockManger = PayPeriodLock.LockoutManagers;
            }
            if (mgrlock > 0)
            {
                ModelList.LockAssignedManager = true;
            }
            return Json(ModelList, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}