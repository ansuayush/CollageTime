using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExecViewHrk.WebUI.Infrastructure;
using ExecViewHrk.EfAdmin;
using ExecViewHrk.EfClient;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Net;
using ExecViewHrk.WebUI.Models;
using System.Data.Entity.Validation;
using System.Globalization;
using ExecViewHrk.WebUI.Helpers;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.SqlServer;
using System.Collections.ObjectModel;
using ExecViewHrk.Domain.Models;
using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Models;
using ExecViewHrk.Domain.Helper;
namespace ExecViewHrk.WebUI.Controllers
{
    public class TimeCardArchiveController : BaseController
    {
        readonly ITimeCardMatrixReposotory _itimeinoutrepo;
        readonly ITimeCardArchiveRerpository _itimearchivrepo;
        readonly ILookupTablesRepository _ilookupRepo;
        public TimeCardArchiveController(ITimeCardMatrixReposotory itimeinoutrepo, ITimeCardArchiveRerpository itimearchivrepo, ILookupTablesRepository ilookupRepo)
        {
            _itimeinoutrepo = itimeinoutrepo;
            _itimearchivrepo = itimearchivrepo;
            _ilookupRepo = ilookupRepo;
        }

        public ActionResult TimeCardsArchiveMatrixPartial()
        {
            ViewData["hourCodesList"] = _ilookupRepo.GetHourCodes();
            ViewData["earningCodesList"] = _ilookupRepo.GetEarningCodes();
            ViewData["tempDepartmentCodesList"] = _ilookupRepo.GetTempDepartmentCodes();
            ViewData["tempJobCodesList"] = _ilookupRepo.GetTempJobCodes();
            ViewData["employeePositionsList"] = _ilookupRepo.GetEmployeePositionList();
            TimeCardsArchiveVM timeCardVm = new TimeCardsArchiveVM();
            timeCardVm.IsArchived = true;
            timeCardVm.timeCardDislayColumns = _ilookupRepo.TimeCardInOutDisplayColumns((int)TimeCardsDisplay.TimeCard);
            return View(timeCardVm);
        }

        public ActionResult WeeksArchiveList_Read([DataSourceRequest]DataSourceRequest request, int? employeeIdDdl, int? payPeriodId, int? departmentId) //DateTime? payPeriodStartDate, DateTime? payPeriodEndDate)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                //var employeeWeeklyTimeCardList = Enumerable.Empty<ExecViewHrk.SqlData.Models.TimeCardInOutCollection>();
                var employeeWeeklyTimeCardList = new List<TimeCardsArchiveVM>();
                employeeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).EmployeeId : employeeIdDdl;
                if (employeeIdDdl.HasValue && payPeriodId.HasValue)
                {
                    try
                    {
                        //employeeWeeklyTimeCardList = new ExecViewHrk.SqlData.SqlTimeCards(ClientDbConnection.clientDbConn)
                        //    .LoadEmployeeWeeklyTimeCardInOutPP(employeeIdDdl.Value, payPeriodId.Value).ToList();

                        /* employeeWeeklyTimeCardList = new ExecViewHrk.SqlData.SqlTimeCards(connString)
                     .LoadEmployeeWeeklyTimeCardInOutPP(employeeIdDdl.Value, payPeriodId.Value, IsArchived).ToList();
                     */
                        //string filenumber = _ilookupRepo.GetEmpFilenumber(employeeIdDdl.Value);
                        employeeWeeklyTimeCardList = _itimearchivrepo.GetTimaCardarchiveList(employeeIdDdl.Value, payPeriodId.Value);
                        //foreach (var item in employeeWeeklyTimeCardList)
                        //{
                        //    item.Day = TimeZoneInfo.ConvertTimeToUtc(item.ActualDate, TimeZoneInfo.Utc);
                        //    item.ActualDate = TimeZoneInfo.ConvertTimeToUtc(item.ActualDate, TimeZoneInfo.Utc);
                        //    if (item.DailyHours != null)
                        //    {
                        //        item.DailyHours = Math.Round(item.DailyHours.Value, 2);
                        //    }
                        //    if (item.LineTotal != null)
                        //    {
                        //        item.LineTotal = Math.Round(item.LineTotal.Value, 2);
                        //    }
                        //    if (item.TimeIn != null)
                        //    {
                        //        item.TimeIn = TimeZoneInfo.ConvertTimeToUtc(item.TimeIn.Value, TimeZoneInfo.Utc);
                        //    }
                        //    if (item.LunchOut != null)
                        //    {
                        //        item.LunchOut = TimeZoneInfo.ConvertTimeToUtc(item.LunchOut.Value, TimeZoneInfo.Utc);
                        //    }
                        //    if (item.LunchBack != null)
                        //    {
                        //        item.LunchBack = TimeZoneInfo.ConvertTimeToUtc(item.LunchBack.Value, TimeZoneInfo.Utc);
                        //    }
                        //    if (item.TimeOut != null)
                        //    {
                        //        item.TimeOut = TimeZoneInfo.ConvertTimeToUtc(item.TimeOut.Value, TimeZoneInfo.Utc);
                        //    }
                        //    if (item.LastModifiedDate != null)
                        //    {
                        //        item.LastModifiedDate = TimeZoneInfo.ConvertTimeToUtc(item.LastModifiedDate.Value, TimeZoneInfo.Utc);
                        //    }
                        //}

                        if (User.IsInRole("HrkAdministrators") || User.IsInRole("ClientAdministrators"))
                        {
                            foreach (var employeeTC in employeeWeeklyTimeCardList)
                            {
                                employeeTC.ShowLineApprovedActive = true;
                            }
                        }

                        if (User.IsInRole("ClientManagers"))
                        {
                            //check employee is permanent or department
                            bool checkEmployeeStatus = _itimeinoutrepo.checkEmployeeStatus((int)departmentId, (int)employeeIdDdl);
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
        //public JsonResult GetPayPeriodsList(int? CompanyCodeIdDdl, bool IsArchived)
        public JsonResult GetPayPeriodsList(int? EmployeeIdDdl)
        {
            bool IsArchived = true;
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                EmployeeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).EmployeeId : EmployeeIdDdl;
            }
            var payPeriodsList = _itimearchivrepo.GetPayPeriodsList(EmployeeIdDdl, IsArchived);
            return Json(payPeriodsList, JsonRequestBehavior.AllowGet);
        }



        #region TimeCards Weekly Totals 

        public ActionResult WeeksTotalArchiveList_Read([DataSourceRequest]DataSourceRequest request, int? employeeIdDdl, int? payPeriodId, bool IsArchived)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var employeeWeeklyTimeCardList = new List<TimeCardWeekTotalCollectionVm>();
                employeeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).EmployeeId : employeeIdDdl;


                if (employeeIdDdl.HasValue && payPeriodId.HasValue && IsArchived != null)
                {
                    //string filenumber = _ilookupRepo.GetEmpFilenumber(employeeIdDdl.Value);
                    employeeWeeklyTimeCardList = _itimearchivrepo.GetWeeklyTotalTimaCardArchiveList(employeeIdDdl.Value, payPeriodId.Value);
                }
                return Json(employeeWeeklyTimeCardList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Timeoff Summary

        [HttpPost]
        public JsonResult GetTimeoffSummary_Read([DataSourceRequest] DataSourceRequest request, int? employeeIdDdl)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                employeeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).EmployeeId : employeeIdDdl;
            }
            var timeoffSummaryList = _itimearchivrepo.GetTimeoffSummaryList(employeeIdDdl);
            return Json(timeoffSummaryList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Timecard Notes

        public ActionResult GetTimecardArchiveNotes(int? timecardId)
        {
            var timeCardsNotesVM = _itimearchivrepo.GetTimecardArchiveNotes(timecardId);
            return View("TimecardNotesDetails", timeCardsNotesVM);
        }
        
        #endregion


    }
}