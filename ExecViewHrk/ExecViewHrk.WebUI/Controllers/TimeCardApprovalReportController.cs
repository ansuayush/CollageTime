using ExecViewHrk.Domain.Interface;
using ExecViewHrk.EfClient;
using ExecViewHrk.Models;
using ExecViewHrk.WebUI.Helpers;
using ExecViewHrk.WebUI.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    public class TimeCardApprovalReportController : BaseController
    {
        readonly ILookupTablesRepository _ilookuprepo;
        readonly ITimeCardApprovalReportRepository _itimecardApprovedRepo;

        public TimeCardApprovalReportController(ILookupTablesRepository ilookuprepo, ITimeCardApprovalReportRepository itimecardApprovedRepo)
        {
            _ilookuprepo = ilookuprepo;
            _itimecardApprovedRepo = itimecardApprovedRepo;
        }
        // GET: TimeCardApprovalReport
        public PartialViewResult TimeCardApprovalReportMatrixPartial()
        {
            ViewData["employeesList"] = _ilookuprepo.GetEmployeesList();
            TimeCardApprovalReportVm tR = new TimeCardApprovalReportVm();
            return PartialView(tR);
        }

        public JsonResult GetPayPeriodsList(int? CompanyCodeIdDdl)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                CompanyCodeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).CompanyCodeId : CompanyCodeIdDdl;

                var payPeriodsList = _ilookuprepo.GetPayPeriodsList(CompanyCodeIdDdl);

                return Json(payPeriodsList, JsonRequestBehavior.AllowGet);
            }

        }

        //#2959: Loads payperiods regardless of company       
        public JsonResult GetGlobalPayPeriodsList()
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var payPeriodsList = _ilookuprepo.GetGlobalPayPeriodsList();
                return Json(payPeriodsList, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult getPayPeriodApprovalStatus_Ajax(TimeCardApprovalReportVm timeCardApprovalReportVm)
        {
            //string connString = User.Identity.GetClientConnectionString();
            //bool approvedStatus = false;

            //ClientDbContext clientDbContext = new ClientDbContext(connString);

            bool approvedStatus = false;
                if (timeCardApprovalReportVm.PayPeriodId.HasValue && timeCardApprovalReportVm.CompanyCodeId > 0 && timeCardApprovalReportVm.DepartmentId > 0)
                {
                    try
                    {
                        List<int> employeesList = _itimecardApprovedRepo.LoadEmpList(timeCardApprovalReportVm.CompanyCodeId, timeCardApprovalReportVm.DepartmentId, timeCardApprovalReportVm.PayPeriodId).ToList();

                        List<int> timecardApproval_empList = _itimecardApprovedRepo.GetTimecardApprovalEmpList(timeCardApprovalReportVm.CompanyCodeId, timeCardApprovalReportVm.DepartmentId, timeCardApprovalReportVm.PayPeriodId).ToList();

                    //if all Timecard records exist in Timecardapproval based on CompCodeId, DeptId, PayperiodId set approvedStatus true
                    if (employeesList.Any() && !employeesList.Except(timecardApproval_empList).ToList().Any())
                        {
                            approvedStatus = true;
                        }
                    }
                    catch (Exception err)
                    {
                        ViewBag.AlertMessage = "";

                        IEnumerable<DbEntityValidationResult> errors = _itimecardApprovedRepo.GetValidationErrors();
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
            return Json(approvedStatus, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Emp_PayPeriodTotalList_Read([DataSourceRequest]DataSourceRequest request, int? companyCodeId, int? payPeriodId)
        {
                var employeeWeeklyTimeCardList = new List<TimeCardApprovalReportCollectionVm>();
                try
                {
                if (companyCodeId.HasValue && payPeriodId.HasValue)
                {
                    if (User.IsInRole("ClientManagers"))
                    {
                        employeeWeeklyTimeCardList = _itimecardApprovedRepo.GetTimeCardApprovalReportList(companyCodeId.Value, payPeriodId.Value, User.Identity.Name);
                    }
                    else
                    {
                        employeeWeeklyTimeCardList = _itimecardApprovedRepo.GetTimeCardApprovalReportList(companyCodeId.Value, payPeriodId.Value, null);
                    }
                }

                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.InnerException.Message);
                }
                return Json(employeeWeeklyTimeCardList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PayPeriod_Approved_Ajax(TimeCardApprovalReportVm timeCardApprovalReportVm)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var employeesList = Enumerable.Empty<int>();

                if (timeCardApprovalReportVm.PayPeriodId.HasValue && timeCardApprovalReportVm.CompanyCodeId > 0 && timeCardApprovalReportVm.DepartmentId > 0)
                {
                    try
                    {
                        employeesList = _itimecardApprovedRepo.LoadEmpList(timeCardApprovalReportVm.CompanyCodeId, timeCardApprovalReportVm.DepartmentId, timeCardApprovalReportVm.PayPeriodId);

                        foreach (int empId in employeesList)
                        {
                            TimeCardApproval timeCardApprovalRecordInDb = _itimecardApprovedRepo.GetTimeCardApprovals(empId, timeCardApprovalReportVm.PayPeriodId);

                            if (timeCardApprovalRecordInDb != null)
                            {
                                timeCardApprovalRecordInDb.Approved = timeCardApprovalReportVm.Approved;
                                clientDbContext.TimeCardApprovals.Attach(timeCardApprovalRecordInDb);
                                clientDbContext.Entry(timeCardApprovalRecordInDb).State = System.Data.Entity.EntityState.Modified;
                            }
                            else
                            {
                                TimeCardApproval newtimeCardApproval = new TimeCardApproval
                                {
                                    EmployeeId = timeCardApprovalReportVm.EmployeeId,
                                    PayPeriodId = timeCardApprovalReportVm.PayPeriodId.Value,
                                    Approved = timeCardApprovalReportVm.Approved
                                };
                                clientDbContext.TimeCardApprovals.Add(newtimeCardApproval);
                            }
                        }
                        clientDbContext.SaveChanges();
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

            return Json(timeCardApprovalReportVm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ExportTimeCardApprovalReportByDept_Ajax(TimeCardApprovalReportVm timeCardApprovalReportVm)
        {

            var exportTimeCardList = new List<TimeCardApprovalReportCollectionVm>();
            string connString = User.Identity.GetClientConnectionString();

            //System.IO.Directory.CreateDirectory(System.Configuration.ConfigurationManager.AppSettings["appRoot"] + "TimeCardApproval");
            System.IO.Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/TimeCardApproval"));
            string filename = "TimeCardApproval" + DateTime.Now.ToString("_MM-dd-yyyy_HH-mm-ss-fff") + ".csv";
            //string FilePath = Path.Combine(System.Configuration.ConfigurationManager.AppSettings["appRoot"] + "TimeCardApproval", filename);
            string FilePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/TimeCardApproval"), filename);

            if (ModelState.IsValid)
            {
                try
                {
                    if (User.IsInRole("ClientManagers"))
                    {
                        exportTimeCardList = _itimecardApprovedRepo.GetTimeCardApprovalReportList(timeCardApprovalReportVm.CompanyCodeId,  timeCardApprovalReportVm.PayPeriodId.Value, User.Identity.Name).ToList();
                    }
                    else
                    {
                        exportTimeCardList = _itimecardApprovedRepo.GetTimeCardApprovalReportList(timeCardApprovalReportVm.CompanyCodeId, timeCardApprovalReportVm.PayPeriodId.Value, null).ToList();
                    }

                    if (exportTimeCardList.Count() > 0)
                    {
                        List<String> listExportTC = new List<string>();
                        listExportTC.Add("Employee, Regular Hours, Coded Hours, Overtime, Payperiod Total, Approved");

                        foreach (var exportTC in exportTimeCardList)
                        {
                            exportTC.PersonName = _ilookuprepo.GetPersonName(exportTC.EmployeeId);
                            //clientDbContext.Employees.Include("Persons").Where(x => x.EmployeeId == exportTC.EmployeeId).Select(x => x.Person.Firstname + " " + x.Person.Lastname).Single();
                            listExportTC.Add(exportTC.ToString());   //override ToString() in TimeCardApprovalReportCollection
                        }

                        System.IO.File.WriteAllLines(FilePath, listExportTC);
                    }
                    else
                    {
                        filename = string.Empty;
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }
            else
            {
                //ModelState.AddModelError("", "Error..could not be saved at this time.");
                return Json(new { succeed = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { fileName = filename, succeed = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public FileContentResult Download(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new Exception(String.Format("File not exist"));

            byte[] filedata = null;
            string contentType = string.Empty;

            try
            {
                string fullPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/TimeCardApproval"), fileName);
                if (System.IO.File.Exists(fullPath))
                {
                    filedata = System.IO.File.ReadAllBytes(fullPath);
                    contentType = MimeMapping.GetMimeMapping(fullPath);

                    var cd = new System.Net.Mime.ContentDisposition
                    {
                        FileName = fileName,
                        Inline = true,
                    };
                    Response.AppendHeader("Content-Disposition", cd.ToString());
                }
                else
                {
                    throw new Exception(String.Format("File not exist", fileName));
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }
            return File(filedata, contentType);
        }
    }
}