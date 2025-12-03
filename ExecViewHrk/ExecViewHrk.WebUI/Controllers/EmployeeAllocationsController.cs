using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Infrastructure;
using ExecViewHrk.WebUI.Models;
using ExecViewHrk.WebUI.Helpers;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using ExecViewHrk.Models;

namespace ExecViewHrk.WebUI.Controllers
{
    public class EmployeeAllocationsController : Controller
    {
        // GET: EmployeeAllocations        
        public ActionResult EmployeeAllocationsMatrixPartial()
        {
            PopulateEmployees();
            PopulateDepartments();
            return View();
        }

        public ActionResult EmployeeAllocationsList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var empAllocationsList = clientDbContext.EmployeeAllocations.OrderBy(e => e.EmployeeAllocationId).ToList();
                return Json(empAllocationsList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeAllocationsList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.EmployeeAllocation empAllocation)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (empAllocation != null && ModelState.IsValid)
                {
                    var empAllocationInDb = clientDbContext.EmployeeAllocations
                        .Where(x => x.EmployeeId == empAllocation.EmployeeId && x.DepartmentId == empAllocation.DepartmentId)
                        .SingleOrDefault();

                    if (empAllocationInDb != null)
                    {
                        ModelState.AddModelError("", "The Employee with Department is already allocated.");
                    }
                    else
                    {
                        var newEmpAllocation = new EmployeeAllocation
                        {
                            EmployeeId = empAllocation.EmployeeId,
                            DepartmentId = empAllocation.DepartmentId,
                            AllocationPercent = empAllocation.AllocationPercent
                        };

                        clientDbContext.EmployeeAllocations.Add(newEmpAllocation);
                        clientDbContext.SaveChanges();
                        empAllocation.EmployeeAllocationId = newEmpAllocation.EmployeeAllocationId;
                    }
                }

                return Json(new[] { empAllocation }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeAllocationsList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.EmployeeAllocation empAllocation)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (empAllocation != null && ModelState.IsValid)
                {
                    var empAllocationInDb = clientDbContext.EmployeeAllocations
                        .Where(x => x.EmployeeAllocationId == empAllocation.EmployeeAllocationId)
                        .SingleOrDefault();

                    if (empAllocationInDb != null)
                    {
                        var unique_EmpDeptInDb = clientDbContext.EmployeeAllocations
                            .Where(x => x.EmployeeAllocationId != empAllocation.EmployeeAllocationId &&
                                x.EmployeeId == empAllocation.EmployeeId && x.DepartmentId == empAllocation.DepartmentId)
                            .SingleOrDefault();

                        if (unique_EmpDeptInDb != null)
                        {
                            ModelState.AddModelError("", "The Employee with Department is already allocated.");
                        }
                        else
                        {
                            empAllocationInDb.EmployeeId = empAllocation.EmployeeId;
                            empAllocationInDb.DepartmentId = empAllocation.DepartmentId;
                            empAllocationInDb.AllocationPercent = empAllocation.AllocationPercent;
                            clientDbContext.SaveChanges();
                        }                       
                    }
                }

                return Json(new[] { empAllocation }.ToDataSourceResult(request, ModelState));
            }
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeAllocationsList_Destroy([DataSourceRequest] DataSourceRequest request
            , EmployeeAllocation empAllocation)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (empAllocation != null)
                {
                    EmployeeAllocation empAllocationInDb = clientDbContext.EmployeeAllocations
                        .Where(x => x.EmployeeAllocationId == empAllocation.EmployeeAllocationId).SingleOrDefault();

                    if (empAllocationInDb != null)
                    {
                        clientDbContext.EmployeeAllocations.Remove(empAllocationInDb);

                        try
                        {
                            clientDbContext.SaveChanges();
                        }
                        catch// (Exception err)
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                        }

                    }
                }

                return Json(new[] { empAllocation }.ToDataSourceResult(request, ModelState));
            }
        }

       
        private void PopulateEmployees()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var employeesList = new ClientDbContext(connString).Employees
                        .Include("Person")
                        .Include("DdlEmploymentStatuses")
                        .GroupBy(x => x.PersonId)
                        .Select(m => m.OrderByDescending(x => x.EmploymentNumber).FirstOrDefault())
                        .Where(x => x.DdlEmploymentStatus.Code == "A")
                        .Select(m => new
                        {
                            EmployeeId = m.EmployeeId,
                            PersonName = m.Person.Firstname + " " + m.Person.Lastname
                        })
                        .OrderBy(s => s.PersonName).ToList();

                //var employeesList = new ClientDbContext(connString).Employees
                //       .Include("Person")                       
                //       .Select(m => new
                //       {
                //           EmployeeId = m.EmployeeId,
                //           PersonName = m.Person.Firstname + " " + m.Person.Lastname
                //       })
                //       .OrderBy(s => s.PersonName).ToList();

               // managersList.Insert(0, new ManagerVm { ManagerId = 0, PersonName = "--select one--" });

                ViewData["employeesList"] = employeesList;
                //ViewData["defaultEmployee"] = employeesList.First();
            }
        }


        private void PopulateDepartments()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var departmentsList = new ClientDbContext(connString).Departments
                    .Include("CompanyCode").Where(x=> x.IsDeleted == false)
                    .Select(m => new DepartmentVm
                    {
                        DepartmentId = m.DepartmentId,
                        CompCode_DeptCode_DeptDescription = m.CompanyCode.CompanyCodeCode + "-" + m.DepartmentCode + "-" + m.DepartmentDescription
                    })
                    .OrderBy(s => s.CompCode_DeptCode_DeptDescription).ToList();

               // departmentsList.Insert(0, new DepartmentVm { DepartmentId = 0, CompCode_DeptCode_DeptDescription = "--select one--" });

                ViewData["departmentsList"] = departmentsList;
                //ViewData["defaultdepartments"] = departmentsList.First();
            }
        }
    }
}