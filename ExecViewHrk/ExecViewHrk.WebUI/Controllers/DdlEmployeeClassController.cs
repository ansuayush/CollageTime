using ExecViewHrk.EfClient;
using ExecViewHrk.Models;
using ExecViewHrk.WebUI.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    public class DdlEmployeeClassController : Controller
    {
        // GET: DdlEmployeeClass
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult EmployeeClassGrid()
        {
            return View();
        }
        public ActionResult EmployeeAddDetail()
        {
            EmployeeClassVm obj = new EmployeeClassVm();
            return View(obj);
        }
        [AcceptVerbs(HttpVerbs.Post)]

        //public EmployeeClassVm GetEmployeeClassDetails(int id)
        //{
        //    string connString = User.Identity.GetClientConnectionString();
        //    ClientDbContext clientDbContext = new ClientDbContext(connString);
        //    EmployeeClassVm obj = new EmployeeClassVm();
        //    if (id != 0)
        //    {
        //        obj = clientDbContext.EmployeeClass.Select(x => new EmployeeClassVm
        //        {

        //            ClassName = x.ClassName,
        //            IsActive = x.IsActive,
        //        }
        //        obj.ClassName = ClassName;
        //        obj.IsActive = IsActive;
        //    }
        //    return obj;
        //}




        public ActionResult DdlEmployeeClassList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var employeeclassList = clientDbContext.EmployeeClass.ToList();
                return Json(employeeclassList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult EditEmployeeclass(int EmployeeClassId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var EmployeeClass = new EmployeeClassVm();
            var Employeeobj = clientDbContext.EmployeeClass.Where(x => x.EmployeeClassId == EmployeeClassId).FirstOrDefault();
            EmployeeClass.ClassName = Employeeobj.ClassName;
            EmployeeClass.IsActive= Employeeobj.IsActive;
            return View(EmployeeClass);
        }


        public ActionResult EmployeeSaveAjax(EmployeeClassVm EmployeeClassVm)
        {
            string connString = User.Identity.GetClientConnectionString();
            var clientDbContext = new ClientDbContext(connString);
            bool recordIsNew = false;
            var EmployeeClass = clientDbContext.EmployeeClass
               .Where(x => x.EmployeeClassId == EmployeeClassVm.EmployeeClassId).SingleOrDefault();
            if (EmployeeClass == null)
            {
                var EmployeeCls = new EmployeeClass();
                EmployeeCls.ClassName = EmployeeClassVm.ClassName;
                EmployeeCls.IsActive = EmployeeClassVm.IsActive;
                EmployeeCls.CreatedDate = DateTime.Now;
                EmployeeCls.CreatedBy = User.Identity.Name;
                clientDbContext.EmployeeClass.Add(EmployeeCls);
            }
            else
            {

                EmployeeClass.ClassName = EmployeeClassVm.ClassName;
                EmployeeClass.IsActive = EmployeeClassVm.IsActive;
                EmployeeClass.ModifiedBy = User.Identity.Name;
            }
            clientDbContext.SaveChanges();
            recordIsNew = true;
            ViewBag.AlertMessage = recordIsNew == true ? "New Employee Class  Record Added" : "Employee Class Record Saved";

            return Json(new { EmployeeClassVm, succeed = true, recordIsNew }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DdlEmployeeclassList_Destroy([DataSourceRequest] DataSourceRequest request
           , int Id)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var clientDbContext = new ClientDbContext(connString))
            {
                if (Id != 0)
                {
                    var EmployeeClassobj = clientDbContext.EmployeeClass
                        .Where(x => x.EmployeeClassId == Id).SingleOrDefault();
                    if (clientDbContext.E_Positions.Where(x => x.EmployeeClassId == Id).Count() > 0)
                    {
                        return Json(new { Message = CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE, succeed = false }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (EmployeeClassobj != null)
                        {
                            clientDbContext.EmployeeClass.Remove(EmployeeClassobj);
                            clientDbContext.SaveChanges();
                        }
                    }
                }
            }
            return Json(new { Message = "Record deleted successfully.", succeed = true }, JsonRequestBehavior.AllowGet);
        }
    }
}




