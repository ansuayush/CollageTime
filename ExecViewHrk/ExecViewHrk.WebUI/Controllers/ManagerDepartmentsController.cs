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
using ExecViewHrk.WebUI.Helpers;
using ExecViewHrk.Models;
using ExecViewHrk.Domain.Interface;

namespace ExecViewHrk.WebUI.Controllers
{
    public class ManagerDepartmentsController : Controller
    {
        readonly IManagerDepartmentRepository _managerDepartmentRepo;

        public ManagerDepartmentsController(IManagerDepartmentRepository managerDepartmentRepo)
        {
            _managerDepartmentRepo = managerDepartmentRepo;
        }
        // GET: ManagerDepartments
        public ActionResult ManagerDepartmentsMatrixPartial()
        {
            PopulateManagers();
            PopulateDepartments();
            //PopulateADPAccNumber();
            return View();
        }

        #region Insert, Update and Delete

        public ActionResult ManagerDepartmentsList_Read([DataSourceRequest]DataSourceRequest request)
        {
            var managerDepartmentsList = _managerDepartmentRepo.GetManagerDepartmentList();
            return Json(managerDepartmentsList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ManagerDepartmentDetail(int managerDepartmentId)
        {
            PopulateManagers();
            PopulateDepartments();
            var objDepartment = _managerDepartmentRepo.GetManagerDepartmentDetails(managerDepartmentId);

            return View("ManagerDepartmentEdit", objDepartment);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ManagerDepartmentsSaveAjax([DataSourceRequest] DataSourceRequest request, ManagerDepartmentVM managerDepartmentVM)
        {
            var errors = ModelState.Values.SelectMany(m => m.Errors);
            bool succeed = false;
            try
            {
                if (managerDepartmentVM != null && ModelState.IsValid)
                {
                    var managerDepartment = _managerDepartmentRepo.GetManagerDepartmentList().Where(x => x.ManagerDepartmentId == managerDepartmentVM.ManagerDepartmentId).SingleOrDefault();
                    if (managerDepartment != null)
                    {
                        var isManagerdepartmentExsists = _managerDepartmentRepo.GetManagerDepartmentList().Where(x => x.ManagerId == managerDepartmentVM.ManagerId
                                                                                          && x.DepartmentId == managerDepartmentVM.DepartmentId).Count();
                        if (isManagerdepartmentExsists > 0)
                        {
                            if (managerDepartment.ManagerId == managerDepartmentVM.ManagerId && managerDepartment.DepartmentId == managerDepartmentVM.DepartmentId)
                                return Json(new { succeed = false, Message = string.Format(CustomErrorMessages.ERROR_NOCHANGES_RECORD, "Department") }, JsonRequestBehavior.AllowGet);
                            else
                                return Json(new { succeed = false, Message = string.Format(CustomErrorMessages.ERROR_DUPLICATE_RECORD, "Department") }, JsonRequestBehavior.AllowGet);
                        }
                        else
                            succeed = _managerDepartmentRepo.ManagerDepartmentsSaveAjax(managerDepartmentVM);
                    }
                    else
                    {
                        var isManagerdepartmentExsists = _managerDepartmentRepo.GetManagerDepartmentList().Where(x => x.ManagerId == managerDepartmentVM.ManagerId
                                                                                          && x.DepartmentId == managerDepartmentVM.DepartmentId).Count();
                        if (isManagerdepartmentExsists > 0)
                            return Json(new { succeed = false, Message = string.Format(CustomErrorMessages.ERROR_DUPLICATE_RECORD, "Department") }, JsonRequestBehavior.AllowGet);
                        else
                            succeed = _managerDepartmentRepo.ManagerDepartmentsSaveAjax(managerDepartmentVM);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                string error = ModelState.Values.SelectMany(m => m.Errors).Select(m => m.ErrorMessage).LastOrDefault();
                return Json(new { succeed = false, Message = error }, JsonRequestBehavior.AllowGet);
            }
            return Json(new[] { managerDepartmentVM }.ToDataSourceResult(request, ModelState));
        }

        /// <summary>
        /// Delete the record
        /// </summary>
        /// <param name="request"></param>
        /// <param name="managerDepartmentId"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ManagerDepartmentsList_Destroy([DataSourceRequest] DataSourceRequest request, int managerDepartmentId)
        {
            if (managerDepartmentId != 0)
            {
                try
                {
                    _managerDepartmentRepo.ManagerDepartmentsList_Destroy(managerDepartmentId);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                    string error = ModelState.Values.SelectMany(m => m.Errors).Select(m => m.ErrorMessage).LastOrDefault();
                    return Json(new { succeed = false, Message = error }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new[] { managerDepartmentId }.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region Dropdown List

        /// <summary>
        /// Populates Managers List for Dropdown
        /// </summary>
        public void PopulateManagers()
        {
            var managersList = _managerDepartmentRepo.PopulateManagers();
            ViewData["managersList"] = managersList;
        }

        /// <summary>
        /// Populates Departments List for Dropdown
        /// </summary>
        public void PopulateDepartments()
        {
            var departmentsList = _managerDepartmentRepo.PopulateDepartments();
            ViewData["departmentsList"] = departmentsList;
        }

        #endregion

    }
}