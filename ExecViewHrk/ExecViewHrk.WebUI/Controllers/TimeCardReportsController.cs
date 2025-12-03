using ExecViewHrk.EfClient;
using ExecViewHrk.Models;
using ExecViewHrk.WebUI.Helpers;
using ExecViewHrk.WebUI.Infrastructure;
using ExecViewHrk.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    public class TimeCardReportsController : Controller
    {
        // GET: TimeCardReports
        public ActionResult TimeCardReportsMatrixPartial()
        {
            return View();
        }


        public JsonResult GetCompanyCodes()
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var companyCodes = Enumerable.Empty<CompanyCodeVm>();

                if (User.IsInRole("HrkAdministrators"))
                {
                    companyCodes = clientDbContext.CompanyCodes
                        .Where(x => x.IsCompanyCodeActive == true)
                        .Select(m => new CompanyCodeVm
                        {
                            CompanyCodeId = m.CompanyCodeId,
                            CompanyCodeDescription = m.CompanyCodeDescription
                        }).OrderBy(x => x.CompanyCodeDescription).ToList();
                }
                else if (User.IsInRole("ClientManagers"))
                {
                    companyCodes = clientDbContext.ManagerDepartments
                        .Include("Department.CompanyCode")
                        .Include("Manager.Person")
                        .Where(x => x.Manager.Person.eMail == User.Identity.Name)
                        .Select(m => new CompanyCodeVm
                        {
                            CompanyCodeId = m.Department.CompanyCodeId,
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
            if (User.IsInRole("HrkAdministrators"))
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

        public JsonResult GetPayPeriodsList(int? CompanyCodeIdDdl)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                CompanyCodeIdDdl = (User.IsInRole("ClientEmployees")) ? AccessEmployeeDetails.EmpDetails(clientDbContext, User.Identity.Name).CompanyCodeId : CompanyCodeIdDdl;

                var payPeriodsList = clientDbContext.PayPeriods
                   .Where(m => m.CompanyCodeId == CompanyCodeIdDdl)
                   .Select(m => new
                   {
                       PayPeriodId = m.PayPeriodId,
                       PayPeriod = SqlFunctions.StringConvert((double)SqlFunctions.DatePart("mm", m.StartDate)).Trim() + "/" + SqlFunctions.DateName("DAY", m.StartDate) + "/" + SqlFunctions.DateName("YYYY", m.StartDate)
                                  + " - " + SqlFunctions.StringConvert((double)SqlFunctions.DatePart("mm", m.EndDate)).Trim() + "/" + SqlFunctions.DateName("DAY", m.EndDate) + "/" + SqlFunctions.DateName("YYYY", m.EndDate)
                   }).OrderByDescending(m => m.PayPeriodId).Take(6).ToList();

                return Json(payPeriodsList, JsonRequestBehavior.AllowGet);
            }

        }

    }
}