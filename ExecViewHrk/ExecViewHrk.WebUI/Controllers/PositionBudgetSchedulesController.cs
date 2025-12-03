using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExecViewHrk.WebUI.Models;
using System.Data.Entity.Validation;
using ExecViewHrk.Models;
using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Helpers;
using ExecViewHrk.Domain.Interface;


namespace ExecViewHrk.WebUI.Controllers
{
    public class PositionBudgetSchedulesController : Controller
    {
        private IPositionBudgetSchedulesRepository _budgetScheduleRepository;

        public PositionBudgetSchedulesController(IPositionBudgetSchedulesRepository budgetSchedulesRepository)
        {
            _budgetScheduleRepository = budgetSchedulesRepository;
        }
        public ActionResult PositionBudgetSchedulesList()
        {
            // PositionBudgetSchedulesVM positionBudgetSchedulesVM = new PositionBudgetSchedulesVM();
            return View();
        }

        public ActionResult PositionBudgetSchedulesList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var budgetScheduleList = _budgetScheduleRepository.getpositionBudgetSchedulesList();
            return Json(budgetScheduleList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult PositionBudgetSchedulesDetails(int ID)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            PositionBudgetSchedulesVM positionBudgetSchedulesVM = new PositionBudgetSchedulesVM();
            positionBudgetSchedulesVM = _budgetScheduleRepository.getpositionBudgetSchedulesDetails(ID);
            return View(positionBudgetSchedulesVM);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PositionBudgetSchedulesDelete(int ID)
        {
            try
            {
                _budgetScheduleRepository.deletepositionBudgetSchedule(ID);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, succeed = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Message = "", succeed = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PositionBudgetSchedulesAddEdit(int ID)
        {
            string connString = User.Identity.GetClientConnectionString();
            PositionBudgetSchedulesVM positionBudgetSchedulesVM = new PositionBudgetSchedulesVM();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            if (ID == 0)
            {
                positionBudgetSchedulesVM.ID = 0;
                positionBudgetSchedulesVM.ScheduleType = 2;//Default selected Both radio Button
                positionBudgetSchedulesVM.IncreaseType = 0;//Default selected percent radio Button
                positionBudgetSchedulesVM.AutoFill = false;
            }
            else
            {
                positionBudgetSchedulesVM = _budgetScheduleRepository.getpositionBudgetSchedulesDetails(ID);
            }

            return View(positionBudgetSchedulesVM);
        }

        public ActionResult PositionBudgetSchedulesSaveAjax(PositionBudgetSchedulesVM positionBudgetSchedulesVM)
        {
            bool isNewRecord = false;
            string _message = "";
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            PositionBudgetSchedulesVM budgetscheduledSalary;
            PositionBudgetSchedulesVM budgetscheduledHourly;
            PositionBudgetSchedulesVM budgetscheduledBoth;
            if (ModelState.IsValid)
            {
                PositionBudgetSchedules positionBudgetSchedulesEFClient = clientDbContext.PositionBudgetSchedules.Where(m => m.ID == positionBudgetSchedulesVM.ID).SingleOrDefault();
                budgetscheduledSalary = _budgetScheduleRepository.GetRecordForEffectiveDateAndScheduleType(positionBudgetSchedulesVM.EffectiveDate, 0);
                budgetscheduledHourly = _budgetScheduleRepository.GetRecordForEffectiveDateAndScheduleType(positionBudgetSchedulesVM.EffectiveDate, 1);
                budgetscheduledBoth = _budgetScheduleRepository.GetRecordForEffectiveDateAndScheduleType(positionBudgetSchedulesVM.EffectiveDate, 2);
                if (positionBudgetSchedulesEFClient == null)
                {
                    //validation for already exist record as per effective date and ScheduleType selection                   
                    if (budgetscheduledSalary != null && positionBudgetSchedulesVM.ScheduleType == 0)
                    {
                        _message = "Salary schedule already exists for the effective date. Cannot add.";
                        return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
                    }
                    else if (budgetscheduledHourly != null && positionBudgetSchedulesVM.ScheduleType == 1)
                    {
                        _message = "Hourly schedule already exists for the effective date. Cannot add.";
                        return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
                    }
                    else if (budgetscheduledBoth != null)
                    {
                        _message = "Schedule for both hourly and salary already exists for the effective date. Cannot add";
                        return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
                    }
                    else if ((budgetscheduledHourly != null || budgetscheduledSalary != null) && positionBudgetSchedulesVM.ScheduleType == 2)
                    {
                        _message = "Cannot add schedule type of Both because either salary or hourly already exists for the effective date. Cannot add.";
                        return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    if (positionBudgetSchedulesEFClient.ID == positionBudgetSchedulesVM.ID)
                    {
                        if (budgetscheduledSalary != null && positionBudgetSchedulesVM.ScheduleType == 0 && budgetscheduledSalary.ID != positionBudgetSchedulesVM.ID)
                        {
                            _message = "Salary schedule already exists for the effective date. Cannot add.";
                            return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);

                        }
                        else if (budgetscheduledHourly != null && positionBudgetSchedulesVM.ScheduleType == 1 && budgetscheduledHourly.ID != positionBudgetSchedulesVM.ID)
                        {
                            _message = "Hourly schedule already exists for the effective date. Cannot add.";
                            return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
                        }
                        else if (budgetscheduledBoth != null && budgetscheduledBoth.ID != positionBudgetSchedulesVM.ID)
                        {
                            _message = "Schedule for both hourly and salary already exists for the effective date. Cannot add";
                            return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
                        }
                        else if (((budgetscheduledHourly != null && budgetscheduledHourly.ID != positionBudgetSchedulesVM.ID) || (budgetscheduledSalary != null && budgetscheduledSalary.ID != positionBudgetSchedulesVM.ID)) && positionBudgetSchedulesVM.ScheduleType == 2)
                        {
                            _message = "Cannot add schedule type of Both because either salary or hourly already exists for the effective date. Cannot add.";
                            return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                try
                {
                    if (positionBudgetSchedulesVM.ID == 0)
                    {
                        isNewRecord = true;
                    }
                    positionBudgetSchedulesVM = _budgetScheduleRepository.positionBudgetSchedulesSave(positionBudgetSchedulesVM);
                    return Json(new { positionBudgetSchedulesVM, succeed = true, isNewRecord }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    IEnumerable<DbEntityValidationResult> errors = clientDbContext.GetValidationErrors();
                    if (errors.Count() == 0)
                    {
                        _message += err.InnerException.Message;
                        if (_message.Contains("Cannot insert duplicate key"))
                        {
                            return Json(new { Message = CustomErrorMessages.ERROR_DUPLICATE_RECORD, success = false }, JsonRequestBehavior.AllowGet);
                        }
                    }

                }
            }
            else
            {
                var modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);
                foreach (var item in modelStateErrors)
                {
                    if (_message != "") _message += "<br />";
                    _message += item.ErrorMessage;
                }
                return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { positionBudgetSchedulesVM, succeed = true, isNewRecord }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PositionHierachy()
        {
            return View();
        }
        public JsonResult GetAllRootLevelsWithBusUnitName(int? PositionId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            if (PositionId != null)
            {
                var e_PositionList = from pbl in clientDbContext.PositionBusinessLevels
                                     join ps in clientDbContext.Positions
                                    on pbl.BusinessLevelNbr equals ps.BusinessLevelNbr
                                     join jb in clientDbContext.Jobs on ps.JobId equals jb.JobId
                                     where (ps.ReportsToPositionId == PositionId)
                                     select new
                                     {
                                         PositionId = ps.PositionId,
                                         Title = pbl.BusinessLevelTitle + ":" + ps.Title,
                                         hasChildren = clientDbContext.Positions.Where(m => m.ReportsToPositionId == PositionId).Any(),
                                     };
                return Json(e_PositionList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var e_PositionList = from pbl in clientDbContext.PositionBusinessLevels
                                     join ps in clientDbContext.Positions
                                    on pbl.BusinessLevelNbr equals ps.BusinessLevelNbr
                                     join jb in clientDbContext.Jobs on ps.JobId equals jb.JobId
                                    
                                     select new
                                     {
                                         PositionId = ps.PositionId,
                                         Title = pbl.BusinessLevelTitle + ":" + ps.Title,
                                         hasChildren = true  
                                     };
                return Json(e_PositionList, JsonRequestBehavior.AllowGet);
            }
        }
    }
}