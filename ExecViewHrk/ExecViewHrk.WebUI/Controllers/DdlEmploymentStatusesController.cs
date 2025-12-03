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
using ExecViewHrk.Models;

namespace ExecViewHrk.WebUI.Controllers
{
    public class DdlEmploymentStatusesController : Controller
    {
        // GET: DdlEmploymentStatuses
        public ActionResult DdlEmploymentStatusesListMaintenance()
        {
            return View();
        }
        public ActionResult StatusesListEditMaintenance(int EmploymentStatusId, FormCollection fc)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                DdlEmploymentStatus ddlEmploymentStatus = new DdlEmploymentStatus();
                PersonEmployeeVm personEmployeeVm = new PersonEmployeeVm();
                var isdefault = clientDbContext.DdlEmploymentStatuses.Where(m => m.EmploymentStatusId == EmploymentStatusId).Select(x => x.isDefault).FirstOrDefault();
                var dlEmploymentStatuses = clientDbContext.DdlEmploymentStatuses.Where(m => m.EmploymentStatusId == EmploymentStatusId).FirstOrDefault();
                ddlEmploymentStatus.Code = dlEmploymentStatuses.Code;
                ddlEmploymentStatus.Description = dlEmploymentStatuses.Description;
                ddlEmploymentStatus.EmploymentStatusId = dlEmploymentStatuses.EmploymentStatusId;
                ddlEmploymentStatus.Active = dlEmploymentStatuses.Active;
                ViewBag.isEdit = EmploymentStatusId != 0;
                ViewBag.isDefault = isdefault;
                if (isdefault == true)
                {
                    ViewBag.isDefault = 1;
                }
                else
                {
                    ViewBag.isDefault = 0;
                }
                return View(ddlEmploymentStatus);
            }
        }



        public ActionResult DdlEmploymentStatusesList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var employmentStatusList = clientDbContext.DdlEmploymentStatuses.OrderBy(e => e.Description).ToList();
                return Json(employmentStatusList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult saveajx([DataSourceRequest] DataSourceRequest request
           , ExecViewHrk.EfClient.DdlEmploymentStatus employmentStatus)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (employmentStatus != null)
                {
                    var employmentStatusInDb = clientDbContext.DdlEmploymentStatuses
                                           .Where(x => x.Code == employmentStatus.Code)
                                           .SingleOrDefault();
                    if (employmentStatus != null)
                    {
                        if (employmentStatus.Code != null)
                        {
                            employmentStatusInDb.Code = employmentStatus.Code;
                        }
                        if (employmentStatus.Description != null)
                        {
                            employmentStatusInDb.Description = employmentStatus.Description;
                        }
                        employmentStatusInDb.Active = employmentStatus.Active;
                        clientDbContext.SaveChanges();
                    }
                }

            }

            return Json(new[] { employmentStatus }.ToDataSourceResult(request, ModelState));
        }



        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlEmploymentStatusesList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlEmploymentStatus employmentStatus)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (employmentStatus != null && ModelState.IsValid)
                {
                    var employmentStatusInDb = clientDbContext.DdlEmploymentStatuses
                        .Where(x => x.Code == employmentStatus.Code)
                        .SingleOrDefault();
                    var empStatusRecord = clientDbContext.DdlEmploymentStatuses.Where(cd => (cd.Code == employmentStatus.Code || cd.Description == employmentStatus.Description)).FirstOrDefault();
                    if (empStatusRecord !=null)
                    {
                        ModelState.AddModelError("", "Employment Status" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                    }
                    else
                    {
                        if (employmentStatusInDb != null)
                        {
                            ModelState.AddModelError("", "The Employement status is already defined.");
                        }
                        else
                        {
                            var newEmploymentStatus = new DdlEmploymentStatus
                            {
                                Description = employmentStatus.Description,
                                Code = employmentStatus.Code,
                                Active=employmentStatus.Active,
                            };

                            clientDbContext.DdlEmploymentStatuses.Add(newEmploymentStatus);
                            clientDbContext.SaveChanges();
                            employmentStatus.EmploymentStatusId = newEmploymentStatus.EmploymentStatusId;
                        }
                    }
                }

                return Json(new[] { employmentStatus }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlEmploymentStatusesList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlEmploymentStatus employmentStatus)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (employmentStatus != null && ModelState.IsValid)
                {
                    var employmentStatusInDb = clientDbContext.DdlEmploymentStatuses
                        .Where(x => x.EmploymentStatusId == employmentStatus.EmploymentStatusId)
                        .SingleOrDefault();
                    var empStatusRecord = clientDbContext.DdlEmploymentStatuses.Where(cd =>cd.EmploymentStatusId !=employmentStatus.EmploymentStatusId &&(cd.Code == employmentStatus.Code || cd.Description == employmentStatus.Description)).FirstOrDefault();
                    if (empStatusRecord !=null)
                    {
                        ModelState.AddModelError("", "Employment Status" + CustomErrorMessages.ERROR_LOOKUP_ALREADY_DEFINED);
                    }
                    else
                    {
                        if (employmentStatusInDb != null)
                        {
                            employmentStatusInDb.Code = employmentStatus.Code;
                            employmentStatusInDb.Description = employmentStatus.Description;
                            employmentStatusInDb.Active = employmentStatus.Active;
                            clientDbContext.SaveChanges();
                        }
                    }
                }

                return Json(new[] { employmentStatus }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlEmploymentStatusesList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlEmploymentStatus employmentStatus)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (employmentStatus != null)
                {
                    DdlEmploymentStatus employmentStatusInDb = clientDbContext.DdlEmploymentStatuses
                        .Where(x => x.EmploymentStatusId == employmentStatus.EmploymentStatusId).SingleOrDefault();

                    if (employmentStatusInDb != null)
                    {
                        clientDbContext.DdlEmploymentStatuses.Remove(employmentStatusInDb);

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

                return Json(new[] { employmentStatus }.ToDataSourceResult(request, ModelState));
            }
        }


        public JsonResult GetEmploymentStatuses(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var employmentStatuses = clientDbContext.DdlEmploymentStatuses
                    .Select(m => new
                    {
                        EmploymentStatusId = m.EmploymentStatusId,
                        EmploymentStatusDescription = m.Description,
                    }).OrderBy(x => x.EmploymentStatusDescription).ToList();

                return Json(employmentStatuses, JsonRequestBehavior.AllowGet);
            }

        }
    }
}