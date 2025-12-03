using AutoMapper;
using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Domain.Models;
using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Helpers;
using ExecViewHrk.WebUI.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    public class DesignateSupervisorController : Controller
    {
        IDesignatedSupervisorRepository _superRepo;

        public DesignateSupervisorController(IDesignatedSupervisorRepository superRepo)
        {
            _superRepo = superRepo;
        }

        // GET: DesignateSupervisor
        public ActionResult Index()
        {
            if (!string.IsNullOrEmpty(User.Identity.GetClientConnectionString()))
                using (ClientDbContext clientDbContext = new ClientDbContext(User.Identity.GetClientConnectionString()))
                {
                    Employee emp = AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name);
                    TempData["SuEmployeeId"] = emp != null ? emp.EmployeeId : 0;
                }
            else
                TempData["SuEmployeeId"] = 0;
            return View();
        }

        public ActionResult DesignatedSupervisor_Read([DataSourceRequest]DataSourceRequest request)
        {
            var designatedSupervisorsList = GetDesignatedSupervisors();
            return Json(designatedSupervisorsList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<DesignatedSupervisorVM> GetDesignatedSupervisors()
        {
            bool adminFlag = User.IsInRole("HrkAdministrators") || User.IsInRole("ClientAdministrators");
            List<DesignatedSupervisorDM> list = _superRepo.GetDesignatedSupervisors(Convert.ToInt32(TempData.Peek("SuEmployeeId")), adminFlag);
            return Mapper.Map<List<DesignatedSupervisorDM>, List<DesignatedSupervisorVM>>(list);
        }

        public ActionResult DesignatedSupervisor_Destroy(int ManagerPersonId)
        {
            if (_superRepo.DeleteDesignatedSupervisor(ManagerPersonId))
                return Json("deleted");
            else
                return Json("error");
        }

        public ActionResult _NewDesignatedSupervisor()
        {
            int empId = Convert.ToInt32(TempData.Peek("SuEmployeeId"));
            bool adminFlag = User.IsInRole("HrkAdministrators") || User.IsInRole("ClientAdministrators");
            ViewData["CurrentSupervisors"] = _superRepo.GetCurrentSupervisors(empId, adminFlag);
            ViewData["ReplaceWithSupervisors"] = _superRepo.GetReplaceWithSupervisors();
            return View();
        }

        public ActionResult SaveDesignatedSupervisor(AddDesignatedSupervisorVM model)
        {
            if (!ModelState.IsValid)
                return Json(new { Message = "Something went wrong!", succeed = false }, JsonRequestBehavior.AllowGet);
            AddDesignatedSupervisorDM modelDM = Mapper.Map<AddDesignatedSupervisorVM, AddDesignatedSupervisorDM>(model);
            try
            {
                if (_superRepo.SaveDesignatedSupervisor(modelDM, User.Identity.Name))
                    return Json(new { Message = "Success", succeed = true }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { Message = "Something went wrong!", succeed = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.InnerException.Message, succeed = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}