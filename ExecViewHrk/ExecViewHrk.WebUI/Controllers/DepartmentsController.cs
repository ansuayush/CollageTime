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
using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Models;

namespace ExecViewHrk.WebUI.Controllers
{
    public class DepartmentsController : Controller
    {
        readonly IDepartmetsRepository _departmetsRepository;
        readonly ILookupTablesRepository _lookupTablesRepo;

        public DepartmentsController(IDepartmetsRepository departmetsRepository, ILookupTablesRepository lookupTablesRepo)
        {
            _departmetsRepository = departmetsRepository;
            _lookupTablesRepo = lookupTablesRepo;
        }

        // GET: Departments
        public ActionResult DepartmentsMatrixPartial()
        {
            return View();
        }

        #region List and Details. Insert, Update and Delete

        public ActionResult DepartmentsList_Read([DataSourceRequest]DataSourceRequest request)
        {
            var departmentsList = _departmetsRepository.GetDepartmentList();
            return Json(departmentsList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DepartmentDetail(int departmentId)
        {
            ViewData["companyCodesList"] = _lookupTablesRepo.GetCompanyCodes();
            var objDepartment = _departmetsRepository.GetDepartmentDetails(departmentId);

            return View(objDepartment);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DepartmentSaveAjax([DataSourceRequest] DataSourceRequest request, DepartmentVm departmentVM)
        {
            var errors = ModelState.Values.SelectMany(m => m.Errors);
            bool succeed = false;
            try
            {
                if (departmentVM != null && ModelState.IsValid)
                {
                   
                        var departmentexist = _departmetsRepository.GetDepartmentList().Where(d => d.DepartmentCode == departmentVM.DepartmentCode && d.CompanyCodeId == departmentVM.CompanyCodeId && d.DepartmentId != departmentVM.DepartmentId).FirstOrDefault();
                    if (departmentexist != null)
                    {
                        return Json(new { succeed = false, Message = string.Format(CustomErrorMessages.ERROR_DUPLICATE_RECORD, "Department Code and Company Code") }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        
                        succeed = _departmetsRepository.DepartmentAddUpdate(departmentVM, User.Identity.Name);
                    }
            }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                string error = ModelState.Values.SelectMany(m => m.Errors).Select(m => m.ErrorMessage).LastOrDefault();
                return Json(new { succeed = false, Message = error }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { departmentVM, succeed }, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DepartmentsList_Destroy([DataSourceRequest] DataSourceRequest request, int departmentId)
        {
            if (departmentId != 0)
            {
                try
                {
                    bool result = _departmetsRepository.DepartmentDestroy(departmentId, User.Identity.Name);
                    if (result)
                    {
                        ModelState.AddModelError("", "Department already in use.");
                        string error = ModelState.Values.SelectMany(m => m.Errors).Select(m => m.ErrorMessage).LastOrDefault();
                        return Json(new { succeed = false, Message = error }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Department already in use.");
                    string error = ModelState.Values.SelectMany(m => m.Errors).Select(m => m.ErrorMessage).LastOrDefault();
                    return Json(new { succeed = false, Message = error }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new[] { departmentId }.ToDataSourceResult(request, ModelState));
        }
    }

    #endregion
}