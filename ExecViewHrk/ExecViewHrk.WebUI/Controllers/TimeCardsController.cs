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

namespace ExecViewHrk.WebUI.Controllers
{
    public class TimeCardsController : Controller
    {
        readonly ITimeCardsRepository _timeCardsRepository;

        public TimeCardsController(ITimeCardsRepository timeCardsRepository)
        {
            _timeCardsRepository = timeCardsRepository;
        }
        // GET: TimeCards
        public ActionResult TimeCardsMatrixPartial(bool IsArchived)
        {
            PopulateHourCodes();
            PopulateEarningCodes();
            PopulateTempDepartmentCodes();
            PopulateTempJobCodes();
            TimeCardVm timeCardVm = new TimeCardVm();
            timeCardVm.IsArchived = IsArchived;
            timeCardVm.timeCardDislayColumns = TimeCardInOutDisplayColumns(1);
            return View(timeCardVm);
        }

        public ActionResult GetEmployeeDropdownList()
        {
            var employeeDropdownList = _timeCardsRepository.GetEmployeeDropdownList();
            return View("TimeCardsIndex", employeeDropdownList);
           
        }

        public ActionResult TimeCardsInAndOutMatrixPartial(bool IsArchived)
        {
            PopulateHourCodes();
            PopulateEarningCodes();
            PopulateTempDepartmentCodes();
            PopulateTempJobCodes();
            TimeCardInOutVm timeCardVm = new TimeCardInOutVm();
            timeCardVm.IsArchived = IsArchived;
            timeCardVm.timeCardDislayColumns = TimeCardInOutDisplayColumns(2); //TimeCardInOutDisplayColumns().timeCardDislayColumns;
            return View(timeCardVm);
        }


        public PartialViewResult TimeCardsFastInOutMatrixPartial()
        {
           //TimeCardFastInOutVm emp = new TimeCardFastInOutVm();
           //emp.EmpTimeCard_List = TimeCardsFastInOutRecordPartial();
           return PartialView(TimeCardsFastInOutRecordPartial());
        }

        public PartialViewResult TimeCardSemiMonthlyMatrixPartial(bool IsArchived)
        {
            PopulateHourCodes();
            PopulateEarningCodes();
            PopulateTempDepartmentCodes();
            PopulateTempJobCodes();
            TimeCardSemiMonthlyVm timeCardVm = new TimeCardSemiMonthlyVm();
            timeCardVm.IsArchived = IsArchived;
            return PartialView(timeCardVm);
        }

        public PartialViewResult TimeCardSemiMonthlyInOutMatrixPartial(bool IsArchived)
        {
            PopulateHourCodes();
            PopulateEarningCodes();
            PopulateTempDepartmentCodes();
            PopulateTempJobCodes();
            TimeCardSemiMonthlyInOutVm timeCardVm = new TimeCardSemiMonthlyInOutVm();
            timeCardVm.IsArchived = IsArchived;
            return PartialView(timeCardVm);
        }
       
        //****Common code for all Time Cards***//
        // # GetDepartmentsList()
        // # GetEmployeesList()
        // # GetPayPeriodsList()
        // # PopulateHourCodes()
        // # ValidHourCodes()
        // # PopulateEarningCodes()
        // # ValidEarningCodes()
        // #PopulateTempDepartmentCodes()

        public JsonResult GetCompanyCodes()
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var companyCodes = Enumerable.Empty<CompanyCodeVm>();

                if (User.IsInRole("HrkAdministrators") || User.IsInRole("ClientAdministrators"))
                { 
                    companyCodes = clientDbContext.CompanyCodes
                        .Where(x => x.IsCompanyCodeActive == true)
                        .Select(m => new CompanyCodeVm
                        {
                            CompanyCodeId = m.CompanyCodeId,
                            CompanyCodeDescription = m.CompanyCodeDescription
                        }).OrderBy(x => x.CompanyCodeDescription).ToList();
                }
                else if(User.IsInRole("ClientManagers"))
                {
                    companyCodes = clientDbContext.ManagerDepartments
                        .Include("Department.CompanyCode")
                        .Include("Manager.Person")
                        .Where(x => x.Manager.Person.eMail == User.Identity.Name)
                        .Select(m => new CompanyCodeVm
                        {
                            CompanyCodeId =m.Department.CompanyCodeId,
                            CompanyCodeDescription = m.Department.CompanyCode.CompanyCodeDescription,
                        }).OrderBy(x => x.CompanyCodeDescription).ToList();
                }

                return Json(companyCodes, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult GetDepartmentsList(short? CompanyCodeIdDdl)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            var departmentList = Enumerable.Empty<DepartmentVm>();
            if (User.IsInRole("HrkAdministrators") || User.IsInRole("ClientAdministrators"))
            {
                departmentList = clientDbContext.Departments
                    .Where(x => x.CompanyCodeId == CompanyCodeIdDdl && x.IsDeleted == false)
                    .Select(m => new DepartmentVm
                    {
                        DepartmentId = m.DepartmentId,
                        CompCode_DeptCode_DeptDescription = m.DepartmentCode + "-" + m.DepartmentDescription
                    }).OrderBy(m => m.CompCode_DeptCode_DeptDescription).ToList();
            }
            else if (User.IsInRole("ClientManagers"))
            {
                 departmentList = clientDbContext.ManagerDepartments
                    .Include("Department.CompanyCode")
                    .Include("Manager.Person")
                    .Where(x => x.Manager.Person.eMail == User.Identity.Name && x.Department.CompanyCodeId == CompanyCodeIdDdl)
                    .Select(m => new DepartmentVm
                    {
                        DepartmentId = m.Department.DepartmentId,
                        CompCode_DeptCode_DeptDescription = m.Department.DepartmentCode + "-" + m.Department.DepartmentDescription
                    }).OrderBy(m => m.CompCode_DeptCode_DeptDescription).ToList();
            }
            return Json(departmentList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployeesList(short? DepartmentIdDdl)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            string requestType = User.Identity.GetRequestType();

            var employeeList = (clientDbContext.Employees
                .Include("DdlEmploymentStatuses")
                .GroupBy(x => x.PersonId)
                .Select(m => m.OrderByDescending(x => x.EmploymentNumber).FirstOrDefault())
                .Where(x => x.DepartmentId == DepartmentIdDdl && x.DdlEmploymentStatus.Code == "A")
                .Select(m => new
                {
                    EmployeeId = m.EmployeeId,
                    EmployeeFullName = m.Person.Firstname + " " + m.Person.Lastname,
                    EmployeeRole = "Permanent"
                }))
                .Union
                (clientDbContext.TimeCards
                .Where(x => x.TempDeptId == DepartmentIdDdl && x.Employee.DdlEmploymentStatus.Code == "A")
                .Select(m => new
                {
                    EmployeeId = m.EmployeeId,
                    EmployeeFullName = m.Employee.Person.Firstname + " " + m.Employee.Person.Lastname,
                    EmployeeRole = "Temporary"
                })).OrderBy(m => m.EmployeeFullName).ToList();

            //var employeeList = (clientDbContext.Employees               
            //   .Where(x => x.DepartmentId == DepartmentIdDdl)
            //   .Select(m => new
            //   {
            //       EmployeeId = m.EmployeeId,
            //       EmployeeFullName = m.Person.Firstname + " " + m.Person.Lastname,
            //       EmployeeRole = "Permanent"
            //   }))
            //   .Union
            //   (clientDbContext.TimeCards
            //   .Where(x => x.TempDeptId == DepartmentIdDdl)
            //   .Select(m => new
            //   {
            //       EmployeeId = m.EmployeeId,
            //       EmployeeFullName = m.Employee.Person.Firstname + " " + m.Employee.Person.Lastname,
            //       EmployeeRole = "Temporary"
            //   })).OrderBy(m => m.EmployeeFullName).ToList();

            return Json(employeeList, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetPayPeriodsList(int? CompanyCodeIdDdl, bool IsArchived)
        {
            string connString = User.Identity.GetClientConnectionString();
           
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                CompanyCodeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).CompanyCodeId : CompanyCodeIdDdl; 
                
                 var payPeriodsList = clientDbContext.PayPeriods
                    .Where(m => m.CompanyCodeId == CompanyCodeIdDdl && m.IsArchived == IsArchived)
                    .Select(m => new
                    {
                        PayPeriodId = m.PayPeriodId,
                        PayPeriod = SqlFunctions.StringConvert((double)SqlFunctions.DatePart("mm", m.StartDate)).Trim() + "/" + SqlFunctions.DateName("DAY", m.StartDate) + "/" + SqlFunctions.DateName("YYYY", m.StartDate)
                                   +" - " +  SqlFunctions.StringConvert((double)SqlFunctions.DatePart("mm", m.EndDate)).Trim() + "/" + SqlFunctions.DateName("DAY", m.EndDate) + "/" + SqlFunctions.DateName("YYYY", m.EndDate)
                    }).OrderByDescending(m => m.PayPeriodId).Take(6).ToList();                          
            
                return Json(payPeriodsList, JsonRequestBehavior.AllowGet);
            }
            
        }



        private void PopulateHourCodes()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var hourCodesList = new ClientDbContext(connString).HoursCodes
                        .Select(s => new
                        {
                            HoursCodeId = s.HoursCodeId,
                            HoursCodeCode = s.HoursCodeCode + "-" + s.HoursCodeDescription
                        })
                        .OrderBy(s => s.HoursCodeCode).ToList();

                //skillTypesList.Insert(0, new SkillTypeVm { SkillTypeId = 0, SkillTypeDescription = "--select one--" });

                ViewData["hourCodesList"] = hourCodesList;
                //ViewData["defaultHourCode"] = hourCodesList.First();
            }
        }


        public JsonResult ValidHourCodes(int? CompanyCodeIdDdl)
        {
            IEnumerable<HoursCodeVm> hourCodesList = new List<HoursCodeVm>();
            
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                CompanyCodeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).CompanyCodeId : CompanyCodeIdDdl; 

                hourCodesList = new ClientDbContext(connString).HoursCodes
                       .Where(x => x.CompanyCodeId == CompanyCodeIdDdl)
                       .Select(s => new HoursCodeVm
                       {
                           HoursCodeId = s.HoursCodeId,
                           HoursCodeCode = s.HoursCodeCode + "-" + s.HoursCodeDescription
                       })
                       .OrderBy(s => s.HoursCodeCode).ToList();
            }
            return Json(hourCodesList, JsonRequestBehavior.AllowGet);
        }


        private void PopulateEarningCodes()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var earningCodesList = new ClientDbContext(connString).EarningsCodes
                        .Select(s => new
                        {
                            EarningsCodeId = s.EarningsCodeId,
                            EarningsCodeCode = s.EarningsCodeCode + "-" + s.EarningsCodeDescription
                        })
                        .OrderBy(s => s.EarningsCodeCode).ToList();

                //skillTypesList.Insert(0, new SkillTypeVm { SkillTypeId = 0, SkillTypeDescription = "--select one--" });

                ViewData["earningCodesList"] = earningCodesList;
               // ViewData["defaultEarningCode"] = earningCodesList.First();
            }
        }


        public JsonResult ValidEarningCodes(int? CompanyCodeIdDdl)
        {
            IEnumerable<EarningCodeVm> earningCodesList = new List<EarningCodeVm>();

            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                CompanyCodeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).CompanyCodeId : CompanyCodeIdDdl; 

                earningCodesList = new ClientDbContext(connString).EarningsCodes
                       .Where(x => x.CompanyCodeId == CompanyCodeIdDdl)
                       .Select(s => new EarningCodeVm
                       {
                           EarningsCodeId = s.EarningsCodeId,
                           EarningsCode = s.EarningsCodeCode + "-" + s.EarningsCodeDescription
                       })
                       .OrderBy(s => s.EarningsCode).ToList();
            }
            return Json(earningCodesList, JsonRequestBehavior.AllowGet);
        }


        private void PopulateTempDepartmentCodes()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var tempDepartmentCodesList = new ClientDbContext(connString).Departments.Where(x=> x.IsDeleted == false)
                        .Select(s => new
                        {
                            TempDeptId = s.DepartmentId,
                            TempDeptCode = s.DepartmentCode + "-" + s.DepartmentDescription
                        })
                        .OrderBy(s => s.TempDeptCode).ToList();

                ViewData["tempDepartmentCodesList"] = tempDepartmentCodesList;
                //ViewData["defaultDepartmentCode"] = tempDepartmentCodesList.First();
            }
        }


        public JsonResult ValidTempDeptCodes(int? CompanyCodeIdDdl)
        {
            IEnumerable<TempDeptVm> tempDepartmentCodesList = new List<TempDeptVm>();

            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                CompanyCodeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).CompanyCodeId : CompanyCodeIdDdl;                               

                tempDepartmentCodesList = new ClientDbContext(connString).Departments
                       .Where(x => x.CompanyCodeId == CompanyCodeIdDdl && x.IsDeleted == false)
                       .Select(s => new TempDeptVm()
                       {
                           TempDeptId = s.DepartmentId,
                           TempDeptCode = s.DepartmentCode + "-" + s.DepartmentDescription
                       })
                       .OrderBy(s => s.TempDeptCode).ToList();

            }

            return Json(tempDepartmentCodesList, JsonRequestBehavior.AllowGet);
        }


        private void PopulateTempJobCodes()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var tempJobCodesList = new ClientDbContext(connString).Jobs
                        .Select(s => new
                        {
                            TempJobId = s.JobId,
                            TempJobCode = s.JobCode + "-" + s.JobDescription
                        })
                        .OrderBy(s => s.TempJobCode).ToList();

                //skillTypesList.Insert(0, new SkillTypeVm { SkillTypeId = 0, SkillTypeDescription = "--select one--" });

                ViewData["tempJobCodesList"] = tempJobCodesList;
                //ViewData["defaultEarningCode"] = tempJobCodesList.First();
            }
        }

        public JsonResult ValidTempJobCodes(int? CompanyCodeIdDdl)
        {
            IEnumerable<TempJobVm> tempJobCodesList = new List<TempJobVm>();

            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                CompanyCodeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).CompanyCodeId : CompanyCodeIdDdl;                

                tempJobCodesList = new ClientDbContext(connString).Jobs
                      .Where(x => x.CompanyCodeId == CompanyCodeIdDdl)
                      .Select(s => new TempJobVm
                      {
                          TempJobId = s.JobId,
                          TempJobCode = s.JobCode + "-" + s.JobDescription
                      })
                      .OrderBy(s => s.TempJobCode).ToList();
            }

            return Json(tempJobCodesList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult getTimeCardApprovalStatus_Ajax(TimeCardInOutVm timeCardVm)
        {
            string connString = User.Identity.GetClientConnectionString();
            bool timeCardStatus = false;

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var empId = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).EmployeeId : timeCardVm.EmployeeId;
    
                if (timeCardVm.PayPeriodId.HasValue && empId != 0)
                {
                    TimeCardApproval timeCardApprovalRecordInDb = clientDbContext.TimeCardApprovals
                                .Where(x => x.EmployeeId == empId && x.PayPeriodId == timeCardVm.PayPeriodId).SingleOrDefault();

                    if (timeCardApprovalRecordInDb != null)
                    {
                        timeCardStatus = timeCardApprovalRecordInDb.Approved;
                    }
                }
            }
            return Json(timeCardStatus, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult TimeCard_Approved_Ajax(TimeCardInOutVm timeCardVm)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (timeCardVm.PayPeriodId.HasValue && timeCardVm.EmployeeId != 0)
                {
                    TimeCardApproval timeCardApprovalRecordInDb = clientDbContext.TimeCardApprovals
                                .Where(x => x.EmployeeId == timeCardVm.EmployeeId && x.PayPeriodId == timeCardVm.PayPeriodId).SingleOrDefault();

                    if (timeCardApprovalRecordInDb != null)
                    {
                        timeCardApprovalRecordInDb.Approved = timeCardVm.Approved;
                        clientDbContext.TimeCardApprovals.Attach(timeCardApprovalRecordInDb);
                        clientDbContext.Entry(timeCardApprovalRecordInDb).State = System.Data.Entity.EntityState.Modified;
                    }
                    else
                    {
                        TimeCardApproval newtimeCardApproval = new TimeCardApproval
                        {
                            EmployeeId = timeCardVm.EmployeeId,
                            PayPeriodId = timeCardVm.PayPeriodId.Value,
                            Approved = timeCardVm.Approved
                        };
                        clientDbContext.TimeCardApprovals.Add(newtimeCardApproval);
                    }
                    clientDbContext.SaveChanges();
                }
            }

            return Json(timeCardVm, JsonRequestBehavior.AllowGet);
        }

        //**************************************************************//


        //Time card DailyHours Read
        public ActionResult WeeksList_Read([DataSourceRequest]DataSourceRequest request, int? employeeIdDdl, int? payPeriodId, int? departmentId, bool IsArchived)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {    
                
                var employeeWeeklyTimeCardList = Enumerable.Empty<ExecViewHrk.SqlData.TimeCardCollection>();
                
                employeeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).EmployeeId : employeeIdDdl;
                             

                if (employeeIdDdl.HasValue && payPeriodId.HasValue && IsArchived != null)
                {
                    try
                    { 
                        //employeeWeeklyTimeCardList = new ExecViewHrk.SqlData.SqlTimeCards(ClientDbConnection.clientDbConn)
                        //    .LoadEmployeeWeeklyTimeCardPP(employeeIdDdl.Value, payPeriodId.Value).ToList();

                        employeeWeeklyTimeCardList = new ExecViewHrk.SqlData.SqlTimeCards(connString)
                            .LoadEmployeeWeeklyTimeCardPP(employeeIdDdl.Value, payPeriodId.Value, IsArchived).ToList();


                        if (User.IsInRole("HrkAdministrators") || User.IsInRole("ClientAdministrators"))
                        {
                            foreach (var employeeTC in employeeWeeklyTimeCardList)
                            {
                                employeeTC.ShowLineApprovedActive = true;
                            }
                        }

                        if (User.IsInRole("ClientManagers"))
                        {
                            //check employee is permanent or temporary
                            bool checkEmployeeStatus = clientDbContext.Employees
                            .Include("Person")
                            .Where(x => x.DepartmentId == departmentId && x.EmployeeId == employeeIdDdl)
                            .Select(x => x.DepartmentId).Any();

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

                return Json(employeeWeeklyTimeCardList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }


        //Time card DailyHours Create [Weekly, BiWeekly, SemiMonthly]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WeeksList_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<TimeCardVm> timeCardVmGridCollection,
            int? employeeIdDdl,int? companyCodeIdDdl)       
        {
            var timeCardList = new List<TimeCard>();

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
                   
                    Dictionary<DateTime, int> Date_MaxProjectNum = clientDbContext.TimeCards
                        .Where(x => x.EmployeeId == employeeIdDdl && x.CompanyCodeId == companyCodeIdDdl)
                        .GroupBy(x => x.ActualDate)
                        .ToDictionary(x => x.Key, x => x.Max(g => g.ProjectNumber));
                    
                    int maxProjectNum = 1;
                    try
                    {
                        foreach (var timeCardVm in timeCardVmGridCollection)
                        {
                            if (Date_MaxProjectNum.ContainsKey(timeCardVm.ActualDate.Date))
                            {
                                maxProjectNum = Convert.ToInt32(Date_MaxProjectNum[timeCardVm.ActualDate.Date]) + 1;
                                
                                //update the Dictionary
                                Date_MaxProjectNum[timeCardVm.ActualDate.Date] = Date_MaxProjectNum[timeCardVm.ActualDate.Date] + 1; ;
                            }
                            else
                            {
                                maxProjectNum = 1;

                                //Add to Dictionary
                                Date_MaxProjectNum.Add(timeCardVm.ActualDate.Date, 1);
                            }

                            var newTimeCardRecord = new TimeCard
                            {
                                CompanyCodeId = companyCodeIdDdl.Value,
                                EmployeeId = employeeIdDdl.Value,
                                ActualDate = timeCardVm.ActualDate,
                                ProjectNumber = maxProjectNum,
                                DailyHours = timeCardVm.DailyHours,
                                HoursCodeId = timeCardVm.HoursCodeId,
                                Hours = timeCardVm.Hours,
                                EarningsCodeId = timeCardVm.EarningsCodeId,
                                EarningsAmount = timeCardVm.EarningsAmount,
                                TempDeptId = timeCardVm.TempDeptId,
                                TempJobId = timeCardVm.TempJobId,
                                IsApproved = timeCardVm.IsLineApproved
                            };
                      
                            clientDbContext.TimeCards.Add(newTimeCardRecord);
                            timeCardList.Add(newTimeCardRecord);
                            //timeCardVm.ProjectNumber = newTimeCardRecord.ProjectNumber;
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
            //return Json(new[] { timeCardList }.ToDataSourceResult(request, ModelState));
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
                IsApproved = timeCard.IsApproved
            }));
        }


        //Time card DailyHours Update [Weekly, BiWeekly, SemiMonthly]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WeeksList_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<TimeCardVm> timeCardVmGridCollection,
            int? employeeIdDdl,int? companyCodeIdDdl)                
        {
            var timeCardList = new List<TimeCardVm>();

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

                    Dictionary<DateTime, int> Date_MaxProjectNum = clientDbContext.TimeCards
                        .Where(x => x.EmployeeId == employeeIdDdl && x.CompanyCodeId == companyCodeIdDdl)
                        .GroupBy(x => x.ActualDate)
                        .ToDictionary(x => x.Key, x => x.Max(g => g.ProjectNumber));

                    int maxProjectNum = 1;
                    try
                    {
                        foreach (var timeCardVm in timeCardVmGridCollection)
                        {
                            var timeCardRecordInDb = clientDbContext.TimeCards.Where(x => x.TimeCardId == timeCardVm.TimeCardId).Single();
                            if(timeCardRecordInDb != null)
                            {
                                //if updated record 'Actualdate' not same as of existing database record 'Actualdate' entry 
                                if(timeCardVm.ActualDate.Date != timeCardRecordInDb.ActualDate)
                                {
                                    if (Date_MaxProjectNum.ContainsKey(timeCardVm.ActualDate.Date))
                                    {
                                        maxProjectNum = Convert.ToInt32(Date_MaxProjectNum[timeCardVm.ActualDate.Date]) + 1;

                                        //update Dictionary
                                        Date_MaxProjectNum[timeCardVm.ActualDate.Date] = Date_MaxProjectNum[timeCardVm.ActualDate.Date] + 1;

                                        //Remove the key or decrement the projectNumber for the key in Dictionary if ActualDate changes
                                        if(Date_MaxProjectNum.ContainsKey(timeCardRecordInDb.ActualDate))
                                        {
                                            if(Date_MaxProjectNum[timeCardRecordInDb.ActualDate] == 1)
                                                Date_MaxProjectNum.Remove(timeCardRecordInDb.ActualDate);
                                            //else if(Date_MaxProjectNum[timeCardRecordInDb.ActualDate] > 1)
                                            //   Date_MaxProjectNum[timeCardRecordInDb.ActualDate] = Date_MaxProjectNum[timeCardRecordInDb.ActualDate] - 1 ;
                                        }
                                    }
                                    else
                                    {
                                        maxProjectNum = 1;
                                        Date_MaxProjectNum.Add(timeCardVm.ActualDate.Date, 1);
                                    }
                                }
                                else
                                {
                                    maxProjectNum = timeCardVm.ProjectNumber;
                                }

                                {
                                    //timeCardRecordInDb.CompanyCodeId = companyCodeIdDdl.Value,
                                    //EmployeeId = employeeIdDdl.Value,
                                    timeCardRecordInDb.ActualDate = timeCardVm.ActualDate;
                                    timeCardRecordInDb.ProjectNumber = maxProjectNum;
                                    timeCardRecordInDb.DailyHours = timeCardVm.DailyHours;
                                    timeCardRecordInDb.HoursCodeId = timeCardVm.HoursCodeId == 0 ? null : timeCardVm.HoursCodeId;
                                    timeCardRecordInDb.Hours = timeCardVm.Hours;
                                    timeCardRecordInDb.EarningsCodeId = timeCardVm.EarningsCodeId == 0 ? null : timeCardVm.EarningsCodeId;
                                    timeCardRecordInDb.EarningsAmount = timeCardVm.EarningsAmount;
                                    timeCardRecordInDb.TempDeptId = timeCardVm.TempDeptId == 0 ? null : timeCardVm.TempDeptId;
                                    timeCardRecordInDb.TempJobId = timeCardVm.TempJobId == 0 ? null : timeCardVm.TempJobId;
                                    timeCardRecordInDb.IsApproved = timeCardVm.IsLineApproved;
                                };

                                TimeCardVm timeCardVmRecordInDb = new TimeCardVm
                                {
                                    TimeCardId = timeCardVm.TimeCardId,
                                    CompanyCodeId = companyCodeIdDdl.Value,
                                    EmployeeId = employeeIdDdl.Value,
                                    ActualDate = timeCardVm.ActualDate,
                                    ProjectNumber = maxProjectNum,
                                    DailyHours = timeCardVm.DailyHours,
                                    HoursCodeId = timeCardVm.HoursCodeId == 0 ? null : timeCardVm.HoursCodeId,
                                    Hours = timeCardVm.Hours,
                                    EarningsCodeId = timeCardVm.EarningsCodeId == 0 ? null : timeCardVm.EarningsCodeId,
                                    EarningsAmount = timeCardVm.EarningsAmount,
                                    TempDeptId = timeCardVm.TempDeptId == 0 ? null : timeCardVm.TempDeptId,
                                    TempJobId = timeCardVm.TempJobId == 0 ? null : timeCardVm.TempJobId,
                                    IsLineApproved = timeCardVm.IsLineApproved
                                };
                                timeCardList.Add(timeCardVmRecordInDb);
                                clientDbContext.TimeCards.Attach(timeCardRecordInDb);
                                clientDbContext.Entry(timeCardRecordInDb).State = System.Data.Entity.EntityState.Modified;
                            }
                            
                            //timeCardVm.ProjectNumber = newTimeCardRecord.ProjectNumber;
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

            //return Json(new[] { timeCardList }.ToDataSourceResult(request, ModelState));

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
                IsLineApproved = timeCardVm.IsLineApproved
            }));
        }



        //Time card DailyHours Delete [Weekly, BiWeekly, SemiMonthly]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WeeksList_Destroy([DataSourceRequest] DataSourceRequest request
            , [Bind(Prefix = "models")]IEnumerable<TimeCardVm> timeCardVmGridCollection)
        {
            var timeCardList = new List<TimeCard>();

            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (timeCardVmGridCollection != null) //&& ModelState.IsValid)
                {
                    try
                    {
                        foreach (var timeCardVm in timeCardVmGridCollection)
                        {
                            TimeCard timeCardRecordInDb = clientDbContext.TimeCards
                                .Where(x => x.TimeCardId == timeCardVm.TimeCardId).SingleOrDefault();

                            if (timeCardRecordInDb != null)
                            {
                                timeCardList.Add(timeCardRecordInDb);
                                clientDbContext.TimeCards.Attach(timeCardRecordInDb);
                                clientDbContext.TimeCards.Remove(timeCardRecordInDb);
                                clientDbContext.SaveChanges();
                            }
                        }
                    }
                    catch// (Exception err)
                    {
                        ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                    }
                }     
            }

            ModelState.Clear();
            return Json(new[] { timeCardList }.ToDataSourceResult(request, ModelState));
        }


        //Time card Total hours Read
        public ActionResult WeeksTotalList_Read([DataSourceRequest]DataSourceRequest request, int? employeeIdDdl, int? payPeriodId, bool IsArchived)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var employeeWeeklyTimeCardList = Enumerable.Empty<ExecViewHrk.SqlData.Models.TimeCardWeekTotalCollection>();
                try
                {                    
                    employeeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).EmployeeId : employeeIdDdl;


                    if (employeeIdDdl.HasValue && payPeriodId.HasValue && IsArchived != null)
                    {
                        //employeeWeeklyTimeCardList = new ExecViewHrk.SqlData.SqlTimeCards(ClientDbConnection.clientDbConn)
                        //    .LoadEmployeeWeeklyTotalTimeCardPP(employeeIdDdl.Value, payPeriodId.Value).ToList();
                        employeeWeeklyTimeCardList = new ExecViewHrk.SqlData.SqlTimeCards(connString)
                            .LoadEmployeeWeeklyTotalTimeCardPP(employeeIdDdl.Value, payPeriodId.Value, IsArchived).ToList();
                    }
                   
                }
                catch(Exception e)
                {
                    ModelState.AddModelError("", e.InnerException.Message);
                }
                return Json(employeeWeeklyTimeCardList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }


        //Time card In Out Read
        public ActionResult WeeksInOutList_Read([DataSourceRequest]DataSourceRequest request, int? employeeIdDdl, int? payPeriodId, int? departmentId, bool IsArchived) //DateTime? payPeriodStartDate, DateTime? payPeriodEndDate)
        {
            string connString = User.Identity.GetClientConnectionString();

            //using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            //{                
            //    return Json(EmpWeeklyTimeCardList(employeeIdDdl, payPeriodId).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            //}

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var employeeWeeklyTimeCardList = Enumerable.Empty<ExecViewHrk.SqlData.Models.TimeCardInOutCollection>();

                employeeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).EmployeeId : employeeIdDdl;

                if (employeeIdDdl.HasValue && payPeriodId.HasValue && IsArchived != null)
                {
                    try
                    { 
                    //employeeWeeklyTimeCardList = new ExecViewHrk.SqlData.SqlTimeCards(ClientDbConnection.clientDbConn)
                    //    .LoadEmployeeWeeklyTimeCardInOutPP(employeeIdDdl.Value, payPeriodId.Value).ToList();
                        employeeWeeklyTimeCardList = new ExecViewHrk.SqlData.SqlTimeCards(connString)
                    .LoadEmployeeWeeklyTimeCardInOutPP(employeeIdDdl.Value, payPeriodId.Value, IsArchived).ToList();


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
                        bool checkEmployeeStatus = clientDbContext.Employees
                        .Include("Person")
                        .Where(x => x.DepartmentId == departmentId && x.EmployeeId == employeeIdDdl)
                        .Select(x => x.DepartmentId).Any();

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
                    catch(Exception e)
                    {
                        ModelState.AddModelError("", e.InnerException.Message);
                    }
                }

                return Json(employeeWeeklyTimeCardList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        //Return Time card In Out Data through Sql
        //private List<ExecViewHrk.SqlData.Models.TimeCardInOutCollection> EmpWeeklyTimeCardList(int? empId, int? payPeriodId)
        //{
        //    var employeeWeeklyTimeCardInOutList = Enumerable.Empty<ExecViewHrk.SqlData.Models.TimeCardInOutCollection>();

        //    if (empId.HasValue && payPeriodId.HasValue)
        //    {
        //        employeeWeeklyTimeCardInOutList = new ExecViewHrk.SqlData.SqlTimeCards(ClientDbConnection.clientDbConn)
        //            .LoadEmployeeWeeklyTimeCardInOutPP(empId.Value, payPeriodId.Value);
        //    }

        //    return employeeWeeklyTimeCardInOutList.ToList();
        //}

        //Time card In Out Create
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WeeksInOutList_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<TimeCardInOutVm> timeCardInOutVmGridCollection,
            int? employeeIdDdl, int? companyCodeIdDdl)
        {
            var timeCardInOutList = new List<TimeCard>();

            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (timeCardInOutVmGridCollection != null && ModelState.IsValid)
                {
                    if (User.IsInRole("ClientEmployees"))
                    {
                        var EmpDetails = AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name);

                        companyCodeIdDdl = EmpDetails.CompanyCodeId;
                        employeeIdDdl = EmpDetails.EmployeeId;
                    }

                    Dictionary<DateTime, int> Date_MaxProjectNum = clientDbContext.TimeCards
                        .Where(x => x.EmployeeId == employeeIdDdl && x.CompanyCodeId == companyCodeIdDdl)
                        .GroupBy(x => x.ActualDate)
                        .ToDictionary(x => x.Key, x => x.Max(g => g.ProjectNumber));

                    int maxProjectNum = 1;
                    try
                    {
                        foreach (var timeCardVm in timeCardInOutVmGridCollection)
                        {
                            if (Date_MaxProjectNum.ContainsKey(timeCardVm.ActualDate.Date))
                            {
                                maxProjectNum = Convert.ToInt32(Date_MaxProjectNum[timeCardVm.ActualDate.Date]) + 1;

                                //update the Dictionary
                                Date_MaxProjectNum[timeCardVm.ActualDate.Date] = Date_MaxProjectNum[timeCardVm.ActualDate.Date] + 1; ;
                            }
                            else
                            {
                                maxProjectNum = 1;

                                //Add to Dictionary
                                Date_MaxProjectNum.Add(timeCardVm.ActualDate.Date, 1);
                            }

                           
                            var newTimeCardRecord = new TimeCard
                            {
                                CompanyCodeId = companyCodeIdDdl.Value,
                                EmployeeId = employeeIdDdl.Value,
                                ActualDate = timeCardVm.ActualDate,
                                ProjectNumber = maxProjectNum,
                                TimeIn = timeCardVm.TimeIn == null ? null : replaceWithActualDate(timeCardVm.ActualDate, timeCardVm.TimeIn),
                                LunchOut = timeCardVm.LunchOut == null ? null : replaceWithActualDate(timeCardVm.ActualDate, timeCardVm.LunchOut),
                                LunchBack = timeCardVm.LunchBack == null ? null : replaceWithActualDate(timeCardVm.ActualDate, timeCardVm.LunchBack),
                                TimeOut = timeCardVm.TimeOut == null ? null : replaceWithActualDate(timeCardVm.ActualDate, timeCardVm.TimeOut), 
                                HoursCodeId = timeCardVm.HoursCodeId,
                                Hours = timeCardVm.Hours,
                                EarningsCodeId = timeCardVm.EarningsCodeId,
                                EarningsAmount = timeCardVm.EarningsAmount,
                                TempDeptId = timeCardVm.TempDeptId,
                                TempJobId = timeCardVm.TempJobId,
                                IsApproved = timeCardVm.IsLineApproved
                            };
                            
                            clientDbContext.TimeCards.Add(newTimeCardRecord);
                            timeCardInOutList.Add(newTimeCardRecord);
                            
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

            //return Json(EmpWeeklyTimeCardList(employeeIdDdl,payPeriodStartDate, payPeriodEndDate).ToDataSourceResult(request, ModelState));

            return Json(timeCardInOutList.ToDataSourceResult(request, ModelState, timeCard => new TimeCard
            {
                TimeCardId = timeCard.TimeCardId,
                CompanyCodeId = timeCard.CompanyCodeId,
                EmployeeId = timeCard.EmployeeId,
                ActualDate = timeCard.ActualDate,
                ProjectNumber = timeCard.ProjectNumber,
                TimeIn = timeCard.TimeIn == null ? null : replaceWithActualDate(timeCard.ActualDate, timeCard.TimeIn),
                LunchOut = timeCard.LunchOut == null ? null : replaceWithActualDate(timeCard.ActualDate, timeCard.LunchOut),
                LunchBack = timeCard.LunchBack == null ? null : replaceWithActualDate(timeCard.ActualDate, timeCard.LunchBack),
                TimeOut = timeCard.TimeOut == null ? null : replaceWithActualDate(timeCard.ActualDate, timeCard.TimeOut),
                HoursCodeId = timeCard.HoursCodeId,
                Hours = timeCard.Hours,
                EarningsCodeId = timeCard.EarningsCodeId,
                EarningsAmount = timeCard.EarningsAmount,
                TempDeptId = timeCard.TempDeptId,
                TempJobId = timeCard.TempJobId,
                IsApproved = timeCard.IsApproved
            }));
        }


        private DateTime? replaceWithActualDate(DateTime actualDate, DateTime? actualDateTime)
        {
            actualDateTime = Convert.ToDateTime(actualDate.ToString().Substring(0, actualDate.ToString().IndexOf(" ")) +
                                     " " + actualDateTime.ToString().Remove(0, actualDateTime.ToString().IndexOf(" ")));     
            return actualDateTime;
        }

        //Time card In Out Update
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WeeksInOutList_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<TimeCardInOutVm> timeCardVmGridCollection,
            int? employeeIdDdl, short? companyCodeIdDdl)
        {
            var timeCardList = new List<TimeCardInOutVm>();

            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (timeCardVmGridCollection != null && ModelState.IsValid)
                {
                    Dictionary<DateTime, int> Date_MaxProjectNum = clientDbContext.TimeCards
                        .Where(x => x.EmployeeId == employeeIdDdl && x.CompanyCodeId == companyCodeIdDdl)
                        .GroupBy(x => x.ActualDate)
                        .ToDictionary(x => x.Key, x => x.Max(g => g.ProjectNumber));

                    int maxProjectNum = 1;
                    try
                    {
                        foreach (var timeCardVm in timeCardVmGridCollection)
                        {
                            var timeCardRecordInDb = clientDbContext.TimeCards.Where(x => x.TimeCardId == timeCardVm.TimeCardId).Single();
                            if (timeCardRecordInDb != null)
                            {
                                //if updated record 'Actualdate' not same as of existing database record 'Actualdate' entry 
                                if (timeCardVm.ActualDate.Date != timeCardRecordInDb.ActualDate)
                                {
                                    if (Date_MaxProjectNum.ContainsKey(timeCardVm.ActualDate.Date))
                                    {
                                        maxProjectNum = Convert.ToInt32(Date_MaxProjectNum[timeCardVm.ActualDate.Date]) + 1;

                                        //update Dictionary
                                        Date_MaxProjectNum[timeCardVm.ActualDate.Date] = Date_MaxProjectNum[timeCardVm.ActualDate.Date] + 1;

                                        //Remove the key or decrement the projectNumber for the key in Dictionary if ActualDate changes
                                        if (Date_MaxProjectNum.ContainsKey(timeCardRecordInDb.ActualDate))
                                        {
                                            if (Date_MaxProjectNum[timeCardRecordInDb.ActualDate] == 1)
                                                Date_MaxProjectNum.Remove(timeCardRecordInDb.ActualDate);
                                            //else if(Date_MaxProjectNum[timeCardRecordInDb.ActualDate] > 1)
                                            //   Date_MaxProjectNum[timeCardRecordInDb.ActualDate] = Date_MaxProjectNum[timeCardRecordInDb.ActualDate] - 1 ;
                                        }
                                    }
                                    else
                                    {
                                        maxProjectNum = 1;
                                        Date_MaxProjectNum.Add(timeCardVm.ActualDate.Date, 1);
                                    }
                                }
                                else
                                {
                                    maxProjectNum = timeCardVm.ProjectNumber;
                                }
                              
                                {
                                    //timeCardRecordInDb.CompanyCodeId = companyCodeIdDdl.Value,
                                    //EmployeeId = employeeIdDdl.Value,
                                    timeCardRecordInDb.ActualDate = timeCardVm.ActualDate;
                                    timeCardRecordInDb.ProjectNumber = maxProjectNum;
                                    timeCardRecordInDb.TimeIn = timeCardVm.TimeIn == null ? null : replaceWithActualDate(timeCardVm.ActualDate, timeCardVm.TimeIn); // timeCardVm.TimeIn;
                                    timeCardRecordInDb.LunchOut = timeCardVm.LunchOut == null ? null : replaceWithActualDate(timeCardVm.ActualDate, timeCardVm.LunchOut);
                                    timeCardRecordInDb.LunchBack = timeCardVm.LunchBack == null ? null : replaceWithActualDate(timeCardVm.ActualDate, timeCardVm.LunchBack);
                                    timeCardRecordInDb.TimeOut = timeCardVm.TimeOut == null ? null : replaceWithActualDate(timeCardVm.ActualDate, timeCardVm.TimeOut); // timeCardVm.TimeOut;
                                    timeCardRecordInDb.HoursCodeId = timeCardVm.HoursCodeId == 0 ? null:timeCardVm.HoursCodeId;
                                    timeCardRecordInDb.Hours = timeCardVm.Hours;
                                    timeCardRecordInDb.EarningsCodeId = timeCardVm.EarningsCodeId == 0 ? null : timeCardVm.EarningsCodeId;
                                    timeCardRecordInDb.EarningsAmount = timeCardVm.EarningsAmount;
                                    timeCardRecordInDb.TempDeptId = timeCardVm.TempDeptId == 0 ? null : timeCardVm.TempDeptId;
                                    timeCardRecordInDb.TempJobId = timeCardVm.TempJobId == 0 ? null : timeCardVm.TempJobId;
                                    timeCardRecordInDb.IsApproved = timeCardVm.IsLineApproved;
                                };

                                TimeCardInOutVm timeCardVmRecordInDb = new TimeCardInOutVm
                                {
                                    TimeCardId = timeCardVm.TimeCardId,
                                    CompanyCodeId = companyCodeIdDdl.Value,
                                    EmployeeId = employeeIdDdl.Value,
                                    ActualDate = timeCardVm.ActualDate,
                                    ProjectNumber = maxProjectNum,
                                    TimeIn = timeCardVm.TimeIn == null ? null : replaceWithActualDate(timeCardVm.ActualDate, timeCardVm.TimeIn), //timeCardVm.TimeIn,
                                    LunchOut = timeCardVm.LunchOut == null ? null : replaceWithActualDate(timeCardVm.ActualDate, timeCardVm.LunchOut),
                                    LunchBack = timeCardVm.LunchBack == null ? null : replaceWithActualDate(timeCardVm.ActualDate, timeCardVm.LunchBack),
                                    TimeOut = timeCardVm.TimeOut == null ? null : replaceWithActualDate(timeCardVm.ActualDate, timeCardVm.TimeOut), //timeCardVm.TimeOut,
                                    HoursCodeId =  timeCardVm.HoursCodeId == 0 ? null:timeCardVm.HoursCodeId,
                                    Hours = timeCardVm.Hours,
                                    EarningsCodeId = timeCardVm.EarningsCodeId == 0 ? null : timeCardVm.EarningsCodeId,
                                    EarningsAmount = timeCardVm.EarningsAmount,
                                    TempDeptId = timeCardVm.TempDeptId == 0 ? null : timeCardVm.TempDeptId,
                                    TempJobId = timeCardVm.TempJobId == 0 ? null : timeCardVm.TempJobId,
                                    IsLineApproved = timeCardVm.IsLineApproved
                                };
                                timeCardList.Add(timeCardVmRecordInDb);
                                clientDbContext.TimeCards.Attach(timeCardRecordInDb);
                                clientDbContext.Entry(timeCardRecordInDb).State = System.Data.Entity.EntityState.Modified;
                            }

                            //timeCardVm.ProjectNumber = newTimeCardRecord.ProjectNumber;
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

            //return Json(new[] { timeCardList }.ToDataSourceResult(request, ModelState));
           // return Json(EmpWeeklyTimeCardList(employeeIdDdl, payPeriodStartDate, payPeriodEndDate).ToDataSourceResult(request, ModelState));
            return Json(timeCardList.ToDataSourceResult(request, ModelState, timeCardVm => new TimeCardInOutVm
            {
                TimeCardId = timeCardVm.TimeCardId,
                CompanyCodeId = companyCodeIdDdl.Value,
                EmployeeId = employeeIdDdl.Value,
                ActualDate = timeCardVm.ActualDate,
                ProjectNumber = timeCardVm.ProjectNumber,
                TimeIn = timeCardVm.TimeIn == null ? null : replaceWithActualDate(timeCardVm.ActualDate, timeCardVm.TimeIn), //timeCardVm.TimeIn,
                LunchOut = timeCardVm.LunchOut == null ? null : replaceWithActualDate(timeCardVm.ActualDate, timeCardVm.LunchOut),
                LunchBack = timeCardVm.LunchBack == null ? null : replaceWithActualDate(timeCardVm.ActualDate, timeCardVm.LunchBack),
                TimeOut = timeCardVm.TimeOut == null ? null : replaceWithActualDate(timeCardVm.ActualDate, timeCardVm.TimeOut), //timeCardVm.TimeOut,
                HoursCodeId = timeCardVm.HoursCodeId == 0 ? null : timeCardVm.HoursCodeId,
                Hours = timeCardVm.Hours,
                EarningsCodeId = timeCardVm.EarningsCodeId == 0 ? null : timeCardVm.EarningsCodeId,
                EarningsAmount = timeCardVm.EarningsAmount,
                TempDeptId = timeCardVm.TempDeptId == 0 ? null : timeCardVm.TempDeptId,
                TempJobId = timeCardVm.TempJobId == 0 ? null : timeCardVm.TempJobId
            }));
        }



        //Time card DailyHours Delete
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WeeksInOutList_Destroy([DataSourceRequest] DataSourceRequest request
            , [Bind(Prefix = "models")]IEnumerable<TimeCardVm> timeCardVmGridCollection)
        {
            var timeCardList = new List<TimeCard>();
           
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                //ModelState.Remove("PayPeriodId");
                if (timeCardVmGridCollection != null )  //&& ModelState.IsValid
                {
                    try
                    {
                        foreach (var timeCardVm in timeCardVmGridCollection)
                        {
                            //timeCardVm.PayPeriodId == 0 ? null:timeCardVm.PayPeriodId;

                            TimeCard timeCardRecordInDb = clientDbContext.TimeCards
                                .Where(x => x.TimeCardId == timeCardVm.TimeCardId).SingleOrDefault();

                            if (timeCardRecordInDb != null)
                            {
                                timeCardList.Add(timeCardRecordInDb);
                                clientDbContext.TimeCards.Attach(timeCardRecordInDb);
                                clientDbContext.TimeCards.Remove(timeCardRecordInDb);
                                clientDbContext.SaveChanges();
                            }
                        }
                    }
                    catch// (Exception err)
                    {
                        ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                    }
                }
            }

            ModelState.Clear();
            return Json(new[] { timeCardList }.ToDataSourceResult(request, ModelState));
        }



        //Time card In Out Total hours Read
        public ActionResult WeeksTotalInOutList_Read([DataSourceRequest]DataSourceRequest request, int? employeeIdDdl, int? payPeriodId, bool IsArchived) 
        {
            string connString = User.Identity.GetClientConnectionString();          

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var employeeWeeklyTimeCardList = Enumerable.Empty<ExecViewHrk.SqlData.Models.TimeCardWeekTotalCollection>();

                employeeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).EmployeeId : employeeIdDdl;


                if (employeeIdDdl.HasValue && payPeriodId.HasValue && IsArchived != null)
                {
                    //employeeWeeklyTimeCardList = new ExecViewHrk.SqlData.SqlTimeCards(ClientDbConnection.clientDbConn)
                    //    .LoadEmployeeWeeklyTotalTimeCardInOutPP(employeeIdDdl.Value, payPeriodId.Value).ToList();
                    employeeWeeklyTimeCardList = new ExecViewHrk.SqlData.SqlTimeCards(connString)
                        .LoadEmployeeWeeklyTotalTimeCardInOutPP(employeeIdDdl.Value, payPeriodId.Value, IsArchived).ToList();
                }

                return Json(employeeWeeklyTimeCardList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        //TimeCard In out Columns to display
        //private TimeCardInOutVm TimeCardInOutDisplayColumns()
        //{

        //    TimeCardInOutVm tc_columns = new TimeCardInOutVm();   //Timecard columns

        //    string connString = User.Identity.GetClientConnectionString();

        //    using (ClientDbContext clientDbContext = new ClientDbContext(connString))
        //    {
        //        tc_columns.timeCardDislayColumns = clientDbContext.TimeCardDisplayColumns
        //        .Where(x => x.TimeCardTypeId == 2).FirstOrDefault();                                                    
        //    }
           
        //    return (tc_columns);
        //}


        private TimeCardDisplayColumn TimeCardInOutDisplayColumns(short typeId)
        {

            TimeCardDisplayColumn timeCardDislayColumns = new TimeCardDisplayColumn();   //Timecard columns

            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                timeCardDislayColumns = clientDbContext.TimeCardDisplayColumns
                .Where(x => x.TimeCardTypeId == typeId).FirstOrDefault();
            }

            return (timeCardDislayColumns);
        }


        //TimeCard Fast In Out Fetch records from database
        private TimeCardFastInOutVm TimeCardsFastInOutRecordPartial()
        {
            
            TimeCardFastInOutVm emp = new TimeCardFastInOutVm();            

            DateTime browser_Date = Convert.ToDateTime(User.Identity.GetBrowserLoginTime());
            browser_Date = Convert.ToDateTime(browser_Date.ToString().Substring(0, browser_Date.ToString().IndexOf(" ")));

            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (browser_Date != null && User.IsInRole("ClientEmployees"))
                {
                    var empDetails = AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name);

                    if (empDetails.CompanyCodeId != null)
                    {                      
                        emp.EmpTimeCard_List = clientDbContext.TimeCards
                        .Where(x => x.EmployeeId == empDetails.EmployeeId && x.CompanyCodeId == empDetails.CompanyCodeId && DateTime.Compare(x.ActualDate, browser_Date)  == 0)
                        .OrderBy(x => x.ProjectNumber)
                        .Select(x => new TimeCardRecordVm {TimeCardId = x.TimeCardId, ActualDate = x.ActualDate, ProjectNumber = x.ProjectNumber ,TimeIn = x.TimeIn, TimeOut = x.TimeOut, 
                            LunchOut = x.LunchOut,LunchBack = x.LunchBack }).ToList();

                        emp.newTimeCardRecord = new TimeCardRecordVm();
                        emp.newTimeCardRecord.ActualDate = browser_Date;
                    }
                }
            }
            
            //return Json(EmployeeTodays_TimeCard_List, JsonRequestBehavior.AllowGet);
            return (emp);
        }

      
        //Timecard Fast In and Out insert/update records
        [HttpPost]
        public PartialViewResult EnterTime_FastInOut_Ajax(TimeCardFastInOutVm timeCardFastInOutVm, string btn_TimeCardSubmit)
        {
            //bool timeEnteredSuccessfully = false;
            
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (User.IsInRole("ClientEmployees"))
                {
                    switch (btn_TimeCardSubmit)
                    {
                        case "Time In":
                            var empDetails = AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name);

                            var maxProjectNumberQuery = clientDbContext.TimeCards
                                .Where(x => x.EmployeeId == empDetails.EmployeeId && x.CompanyCodeId == empDetails.CompanyCodeId.Value && DateTime.Compare(x.ActualDate, timeCardFastInOutVm.newTimeCardRecord.ActualDate) == 0);
                              
                            int maxProjectNum = maxProjectNumberQuery.Any() ? maxProjectNumberQuery.Max(x => x.ProjectNumber) + 1 : 1;                          

                            TimeCard newTimeCardRecord = new TimeCard
                            {
                                CompanyCodeId = empDetails.CompanyCodeId.Value,
                                EmployeeId = empDetails.EmployeeId,
                                ActualDate = timeCardFastInOutVm.newTimeCardRecord.ActualDate,
                                ProjectNumber = maxProjectNum,
                                TimeIn = timeCardFastInOutVm.newTimeCardRecord.TimeIn,
                                LunchOut = timeCardFastInOutVm.newTimeCardRecord.LunchOut,
                                LunchBack = timeCardFastInOutVm.newTimeCardRecord.LunchBack,
                                TimeOut = timeCardFastInOutVm.newTimeCardRecord.TimeOut
                                //TimeIn = timeCardVm.TimeIn == null ? null : replaceWithActualDate(timeCardVm.ActualDate, timeCardVm.TimeIn),
                                //LunchOut = timeCardVm.LunchOut == null ? null : replaceWithActualDate(timeCardVm.ActualDate, timeCardVm.LunchOut),
                                //LunchBack = timeCardVm.LunchBack == null ? null : replaceWithActualDate(timeCardVm.ActualDate, timeCardVm.LunchBack),
                                //TimeOut = timeCardVm.TimeOut == null ? null : replaceWithActualDate(timeCardVm.ActualDate, timeCardVm.TimeOut),                                
                            };
                            
                            clientDbContext.TimeCards.Add(newTimeCardRecord);
                            clientDbContext.SaveChanges();
                            break;
                        case "Lunch Out":                         
                            TimeCardFastInOut_Update(clientDbContext, timeCardFastInOutVm.newTimeCardRecord.TimeCardId, timeCardFastInOutVm.newTimeCardRecord.LunchOut, btn_TimeCardSubmit);
                            break;
                        case "Lunch Back":
                            TimeCardFastInOut_Update(clientDbContext, timeCardFastInOutVm.newTimeCardRecord.TimeCardId, timeCardFastInOutVm.newTimeCardRecord.LunchBack, btn_TimeCardSubmit);
                            break;
                        case "Time Out":
                            TimeCardFastInOut_Update(clientDbContext, timeCardFastInOutVm.newTimeCardRecord.TimeCardId, timeCardFastInOutVm.newTimeCardRecord.TimeOut, btn_TimeCardSubmit);
                            break;
                    }
                }
            }
           
            TimeCardFastInOutVm empTC = TimeCardsFastInOutRecordPartial();
            //return Json(empTC, JsonRequestBehavior.AllowGet);
            //return Content(empTC.ToString());
            return PartialView("TimeCardsFastInOutMatrixPartial", empTC);
        }


        //Update TimeCard (Fast In Out) Record 
        private void TimeCardFastInOut_Update(ClientDbContext clientDbContext, int timeCardId, DateTime? timeToEnter, String btn_Title)
        {
            var timeCardRecordInDb = clientDbContext.TimeCards.Where(x => x.TimeCardId == timeCardId).Single();
            if (timeCardRecordInDb != null)
            {
                {
                    if (btn_Title == "Lunch Out") { timeCardRecordInDb.LunchOut = timeToEnter; }
                    else if (btn_Title == "Lunch Back") { timeCardRecordInDb.LunchBack = timeToEnter; }
                    else if (btn_Title == "Time Out") { timeCardRecordInDb.TimeOut = timeToEnter; }                    
                };
                clientDbContext.TimeCards.Attach(timeCardRecordInDb);
                clientDbContext.Entry(timeCardRecordInDb).State = System.Data.Entity.EntityState.Modified;
            }

            clientDbContext.SaveChanges();    
        }

        //Time Card Semi-monthly DailyHours Read
        public ActionResult SemiMonthlysList_Read([DataSourceRequest]DataSourceRequest request, int? employeeIdDdl, int? payPeriodId, int? departmentId, bool IsArchived)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var employeeWeeklyTimeCardList = Enumerable.Empty<ExecViewHrk.SqlData.Models.TimeCardSemiMonthlyCollection>();
                employeeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).EmployeeId : employeeIdDdl;

                if (employeeIdDdl.HasValue && payPeriodId.HasValue && IsArchived != null)
                {
                    try
                    {
                        employeeWeeklyTimeCardList = new ExecViewHrk.SqlData.SqlTimeCards(connString)
                            .LoadEmployeeSemiMonthlyTimeCardPP(employeeIdDdl.Value, payPeriodId.Value, IsArchived).ToList();


                        if (User.IsInRole("HrkAdministrators") || User.IsInRole("ClientAdministrators"))
                        {
                            foreach (var employeeTC in employeeWeeklyTimeCardList)
                            {
                                employeeTC.ShowLineApprovedActive = true;
                            }
                        }

                        if (User.IsInRole("ClientManagers"))
                        {
                            //check employee is permanent or temporary
                            bool checkEmployeeStatus = clientDbContext.Employees
                            .Include("Person")
                            .Where(x => x.DepartmentId == departmentId && x.EmployeeId == employeeIdDdl)
                            .Select(x => x.DepartmentId).Any();

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

                return Json(employeeWeeklyTimeCardList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }


        //Semi-Monthly Time card Total hours Read
        public ActionResult PayPeriodTotalList_Read([DataSourceRequest]DataSourceRequest request, int? employeeIdDdl, int? payPeriodId, bool IsArchived)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var employeePPTimeCardList = Enumerable.Empty<ExecViewHrk.SqlData.Models.TimeCardWeekTotalCollection>();
                try
                {
                    employeeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).EmployeeId : employeeIdDdl;


                    if (employeeIdDdl.HasValue && payPeriodId.HasValue && IsArchived != null)
                    {
                        employeePPTimeCardList = new ExecViewHrk.SqlData.SqlTimeCards(connString)
                            .LoadEmployeePayPeriodTotal_TimeCard(employeeIdDdl.Value, payPeriodId.Value, IsArchived).ToList();
                    }

                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.InnerException.Message);
                }
                return Json(employeePPTimeCardList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }


        //Semi-Monthly Time card In Out Total hours Read
        public ActionResult PayPeriodTotalInOutList_Read([DataSourceRequest]DataSourceRequest request, int? employeeIdDdl, int? payPeriodId, bool IsArchived)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var employeePPTimeCardInOutList = Enumerable.Empty<ExecViewHrk.SqlData.Models.TimeCardWeekTotalCollection>();

                employeeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).EmployeeId : employeeIdDdl;


                if (employeeIdDdl.HasValue && payPeriodId.HasValue && IsArchived != null)
                {
                    employeePPTimeCardInOutList = new ExecViewHrk.SqlData.SqlTimeCards(connString)
                        .LoadEmployeePayPeriodTotal_TimeCardInOut(employeeIdDdl.Value, payPeriodId.Value, IsArchived).ToList();
                }

                return Json(employeePPTimeCardInOutList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }



    }
}


