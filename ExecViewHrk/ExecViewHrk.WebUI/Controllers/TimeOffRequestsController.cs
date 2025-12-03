using ExecViewHrk.Domain.Interface;
using ExecViewHrk.EfClient;
using ExecViewHrk.Models;
using ExecViewHrk.WebUI.Helpers;
using ExecViewHrk.WebUI.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    public class TimeOffRequestsController : Controller
    {
        readonly ITimeOffRequestsRepository _timeOffRequestRepo;

        public TimeOffRequestsController(ITimeOffRequestsRepository timeOffRequestRepo)
        {
            _timeOffRequestRepo = timeOffRequestRepo;
        }

        #region Employee View

        //****Employee View****
        // GET: TimeOffRequests  //To display calendar 
        public PartialViewResult TimeOffRequestsMatrixPartial()
        {            
            return PartialView(Emp_TimeOffRequests(DateTime.Now.Year, DateTime.Now.Month));
        }

        //Employee View   //Open window to request Time off
        public PartialViewResult RangeSelectionPartial(DateTime selectedDate)
        {
            TimeOffRequestVM timeOffRequestVM = new TimeOffRequestVM();
            if (selectedDate == null)
            {
                selectedDate = DateTime.Now;
            }
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                Employee empDetails = AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name);
                timeOffRequestVM = _timeOffRequestRepo.GetTimeOffRequestData(empDetails, selectedDate);
            }

                return PartialView("RangeSelectionPartial", timeOffRequestVM);
        }

        //Employee View
        private TimeOffRequestVM Emp_TimeOffRequests(int year, int month)
        {
            TimeOffRequestVM emp_TimeOffRequestsVM = new TimeOffRequestVM();

            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                Employee empDetails = AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name);

                if (empDetails != null)
                {
                    emp_TimeOffRequestsVM.EmpTimeOffRequest = _timeOffRequestRepo.GetEmpTimeOffRequest(empDetails, year, month);
                }
            }

            return (emp_TimeOffRequestsVM);
        }

        [HttpPost] //Employee View
        public JsonResult GetTimeOffRequestsDB_Ajax(int selectedYear, int selectedMonth)
        {
            return Json(Emp_TimeOffRequests(selectedYear, selectedMonth), JsonRequestBehavior.AllowGet);
        }

        //(Employee View) Add Timeoff Request
        [HttpPost]
        public ActionResult AddTimeOffRequest_Ajax(TimeOffRequestVM timeOffRequestVM)
        {
            //bool succeed = true;
            //string Message = "";
            if (timeOffRequestVM.end < timeOffRequestVM.start)
            {
                ModelState.AddModelError("End", "End date must be greater than or equal to start date");
                //succeed = false;
                //Message = "End must be greater than or equal to start date.";
                return Json(new { succeed = false, Message = "End must be greater than or equal to start date." }, JsonRequestBehavior.AllowGet);
            }
            if (ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                using (ClientDbContext clientDbContext = new ClientDbContext(connString))
                {
                    var empDetails = AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name);

                    if (empDetails == null)
                    {
                        return Json(new { succeed = false, Message = "Employee details not available." }, JsonRequestBehavior.AllowGet);
                    }
                    int days = (timeOffRequestVM.end - timeOffRequestVM.start).Days + 1;
                    DateTime timeOffDate = timeOffRequestVM.start;

                    for (int i = 0; i < days; i++)
                    {
                        bool TimeOffRequestDateInDb = _timeOffRequestRepo.GetTimeOffRequestDateInDb(empDetails, timeOffDate);

                        if (TimeOffRequestDateInDb)
                        {
                            return Json(new { succeed = false, Message = "Timeoff request date already submitted." }, JsonRequestBehavior.AllowGet);
                        }
                        timeOffDate = timeOffDate.AddDays(1);
                    }

                    string datesRequested = "";
                    timeOffDate = timeOffRequestVM.start;
                    try
                    {
                        bool succeed = _timeOffRequestRepo.AddTimeOffRequest(timeOffRequestVM, empDetails, timeOffDate, days);
                        //string personName = clientDbContext.Persons.Where(x => x.PersonId == empDetails.PersonId).Select(x => x.Firstname + " " + x.Lastname).Single();
                        //clientDbContext.SaveChanges();
                        //EmailToManager(personName, User.Identity.Name, datesRequested);
                    }
                    catch (Exception err)
                    { ModelState.AddModelError("", err.InnerException.Message); }

                }
            }
            return Json(new { succeed = true }, JsonRequestBehavior.AllowGet);
            // return RedirectToAction("TimeOffRequestsMatrixPartial", "TimeOffRequests");
        }

        //(Employee View) Delete Timeoff request
        [HttpPost] 
        public JsonResult DeleteTimeOffRequest_Ajax([DataSourceRequest] DataSourceRequest request, TimeOffRequestVM timeOffRequestVM)
        {
            var timeOffRequestList = new List<TimeOffRequest>();
            if (timeOffRequestVM.end < timeOffRequestVM.start)
            {
                ModelState.AddModelError("End", "End date must be greater than or equal to start date");
                return Json(new { succeed = false, Message = "End must be greater than or equal to start date." }, JsonRequestBehavior.AllowGet);
            }
            if (ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                using (ClientDbContext clientDbContext = new ClientDbContext(connString))
                {
                    var empDetails = AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name);

                    if (empDetails == null)
                    {
                        return Json(new { succeed = false, Message = "Employee details not available." }, JsonRequestBehavior.AllowGet);
                    }
                    int days = (timeOffRequestVM.end - timeOffRequestVM.start).Days + 1;
                    DateTime timeOffDate = timeOffRequestVM.start;
                    try
                    {
                        timeOffRequestList = _timeOffRequestRepo.DeleteTimeOffRequest(empDetails, timeOffDate, days);
                    }
                    catch (Exception err)
                    {
                        ModelState.AddModelError("", err.InnerException.Message);
                    }
                    //try
                    //{
                    //    clientDbContext.SaveChanges();
                    //}
                    //catch (Exception err)
                    //{ ModelState.AddModelError("", err.InnerException.Message); }

                }
            }

            ModelState.Clear();
            return Json(new[] { timeOffRequestList }.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region Admin,Manager View

        //(Admin,Manager View)
        public PartialViewResult AllTimeOffRequestsMatrixPartial()
        {
            return PartialView(AllEmp_TimeOffRequests(DateTime.Now.Year, DateTime.Now.Month));
        }

        [HttpPost] //(Admin,Manager View)
        public JsonResult GetAllTimeOffRequestsDB_Ajax(int selectedYear, int selectedMonth)
        {
            return Json(AllEmp_TimeOffRequests(selectedYear, selectedMonth), JsonRequestBehavior.AllowGet);
        }

        //(Admin,Manager View)
        private TimeOffVM AllEmp_TimeOffRequests(int sYear, int sMonth)
        {
            TimeOffVM allTimeOffRequestsVM = new TimeOffVM();

            allTimeOffRequestsVM.TimeOffRequestsList = _timeOffRequestRepo.GetAllEmpTimeOffRequestsList(sYear, sMonth);
            
            return (allTimeOffRequestsVM);
        }

        //(Admin,Manager View)
        public PartialViewResult TimeOffRequestsByDatePartial(DateTime selectedDate)
        {
            if (selectedDate == null)
            {
                selectedDate = DateTime.Now;
            }

            TimeOffVM allTimeOffRequestsVM = new TimeOffVM();
            
            allTimeOffRequestsVM.TimeOffEmpDetailsList = _timeOffRequestRepo.GetTimeOffEmpDetailsList(selectedDate);

            return PartialView("TimeOffRequestsByDatePartial", allTimeOffRequestsVM);
        }


        [HttpPost] //(Admin,Manager View)
        public ActionResult AddResposeTimeOffRequest_Ajax(TimeOffVM timeOffVM)  // 
        {
            int sYear = DateTime.Now.Year;
            int sMonth = DateTime.Now.Month;
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                try
                {
                    bool result = _timeOffRequestRepo.AddResposeTimeOffRequest(timeOffVM);
                    ViewBag.AlertMessage = "Success";
                    if (timeOffVM.TimeOffEmpDetailsList.Count > 0)
                    {
                        sYear = timeOffVM.TimeOffEmpDetailsList[0].TimeOffRequest.Year;
                        sMonth = timeOffVM.TimeOffEmpDetailsList[0].TimeOffRequest.Month;
                    }

                }
                catch (Exception err)
                {
                    ViewBag.AlertMessage = "";
                    IEnumerable<DbEntityValidationResult> errors = clientDbContext.GetValidationErrors();
                    if (errors.Count() == 0)
                        ModelState.AddModelError("", err.Message);
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

            //return RedirectToAction("GetAllTimeOffRequestsDB_Ajax", "TimeOffRequests", new { selectedYear = sYear, selectedMonth = sMonth });
            //return RedirectToAction("AllTimeOffRequestsMatrixPartial", "TimeOffRequests");
            return View("AllTimeOffRequestsMatrixPartial", AllEmp_TimeOffRequests(DateTime.Now.Year, DateTime.Now.Month));
        }

        #endregion
    }
}



