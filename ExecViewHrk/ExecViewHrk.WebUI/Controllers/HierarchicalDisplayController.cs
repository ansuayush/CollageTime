using ExecViewHrk.WebUI.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExecViewHrk.EfAdmin;
using ExecViewHrk.EfClient;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Net;
using ExecViewHrk.WebUI.Models;
using ExecViewHrk.WebUI.Helpers;

namespace ExecViewHrk.WebUI.Controllers
{
    //[CustomErrorHandler]
    public class HierarchicalDisplayController : Controller
    {
        // GET: HierarchicalDisplay
        [HandleError]
        public ActionResult HierarchicalDisplay()
        {
            int x = 0;
            var result = 1;
            result /= x  ;
            return View();
        }

        public ActionResult HierarchyBinding_CompanyCodes_Read([DataSourceRequest] DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ExecViewHrk.EfClient.ClientDbContext clientDbContext = new ExecViewHrk.EfClient.ClientDbContext(connString))
            {
                var companyCodesList = clientDbContext.CompanyCodes.OrderBy(e => e.CompanyCodeCode).ToList();
                return Json(companyCodesList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }           
        }


        public ActionResult HierarchyBinding_Departments(int companyCodeId, [DataSourceRequest] DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ExecViewHrk.EfClient.ClientDbContext clientDbContext = new ExecViewHrk.EfClient.ClientDbContext(connString))
            {
                var departmentsList = clientDbContext.Departments
                    .Where(x => x.CompanyCodeId == companyCodeId && x.IsDeleted == false)
                    .OrderBy(e => e.DepartmentDescription).ToList();
                return Json(departmentsList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }                  
        }


        public ActionResult HierarchyBinding_ManagerDepartments(int departmentId, [DataSourceRequest] DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ExecViewHrk.EfClient.ClientDbContext clientDbContext = new ExecViewHrk.EfClient.ClientDbContext(connString))
            {
                var departmentsList = clientDbContext.ManagerDepartments
                    .Include("Manager.Persons")
                    .Where(x => x.DepartmentId == departmentId)
                    .Select(x => new HierarchicalDisplayManagerVm
                    {
                      ManagerId = x.ManagerId,
                      Manager = x.Manager.Person.Firstname + " " + x.Manager.Person.Lastname
                    })                   
                    .OrderBy(e => e.Manager).ToList();
                return Json(departmentsList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }
    }
}