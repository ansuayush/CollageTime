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
using ExecViewHrk.WebUI.Helpers;
namespace ExecViewHrk.WebUI.Controllers
{
    public class DdlEmployeeTypesController : Controller
    {
        // GET: DdlEmployeeTypes
        public ActionResult DdlEmployeeTypesListMaintenance()
        {
            return View();
        }

        public ActionResult DdlEmployeeTypesList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var employeeTypeList = clientDbContext.DdlEmployeeTypes.OrderBy(e => e.Description).ToList();
                return Json(employeeTypeList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlEmployeeTypesList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlEmployeeType employeeType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (employeeType != null && ModelState.IsValid)
                {
                    var employeeTypeInDb = clientDbContext.DdlEmployeeTypes
                        .Where(x => x.Code == employeeType.Code)
                        .SingleOrDefault();

                    if (employeeTypeInDb != null)
                    {
                        ModelState.AddModelError("", "The Employee type is already defined.");
                    }
                    else
                    {
                        var newEmployeeType = new DdlEmployeeType
                        {
                            Description =  employeeType.Description,
                            Code = employeeType.Code,
                            Active=employeeType.Active
                        };

                        clientDbContext.DdlEmployeeTypes.Add(newEmployeeType);
                        clientDbContext.SaveChanges();
                        employeeType.EmployeeTypeId = newEmployeeType.EmployeeTypeId;
                        employeeType.Code = newEmployeeType.Code;
                        employeeType.Active = newEmployeeType.Active;
                        employeeType.Description = newEmployeeType.Description;
                    }
                }

                return Json(new[] { employeeType }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlEmployeeTypesList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlEmployeeType employeeType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (employeeType != null && ModelState.IsValid)
                {
                    var employeeTypeInDb = clientDbContext.DdlEmployeeTypes
                        .Where(x => x.EmployeeTypeId== employeeType.EmployeeTypeId)
                        .SingleOrDefault();

                    if (employeeTypeInDb != null)
                    {
                        employeeTypeInDb.Code = employeeType.Code;
                        employeeTypeInDb.Description = employeeType.Description;
                        employeeTypeInDb.Active = employeeType.Active;
                        clientDbContext.SaveChanges();
                    }
                }

                return Json(new[] { employeeType }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlEmployeeTypesList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlEmployeeType employeeType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (employeeType != null)
                {
                    DdlEmployeeType employeeTypeInDb = clientDbContext.DdlEmployeeTypes
                        .Where(x => x.EmployeeTypeId == employeeType.EmployeeTypeId).SingleOrDefault();
                    if (clientDbContext.E_Positions.Where(x => x.EmployeeTypeId == employeeType.EmployeeTypeId).Count() > 0)
                    {
                        ModelState.AddModelError("", CustomErrorMessages.ERROR_RECORD_ALREADY_IN_USE);
                    }
                    else
                    {
                        if (employeeTypeInDb != null)
                        {
                            clientDbContext.DdlEmployeeTypes.Remove(employeeTypeInDb);

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
                }

                return Json(new[] { employeeType }.ToDataSourceResult(request, ModelState));
            }
        }


        public JsonResult GetEmployeeTypes(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var employeeTypes = clientDbContext.DdlEmployeeTypes
                    .Select(m => new
                    {
                        EmployeeTypeId = m.EmployeeTypeId,
                        EmployeeTypeDescription = m.Description,
                    }).OrderBy(x => x.EmployeeTypeDescription).ToList();

                return Json(employeeTypes, JsonRequestBehavior.AllowGet);
            }

        }
    }
}