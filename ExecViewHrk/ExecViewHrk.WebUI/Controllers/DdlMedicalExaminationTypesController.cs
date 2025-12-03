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
    public class DdlMedicalExaminationTypesController : Controller
    {
        // GET: DdlMedicalExaminationTypes
        public ActionResult DdlMedicalExaminationTypesListMaintenance()
        {
             return View();
        }

        public ActionResult DdlMedicalExaminationTypesList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var medicalExaminationTypeList = clientDbContext.DdlMedicalExaminationTypes.OrderBy(e => e.Description).ToList();
                return Json(medicalExaminationTypeList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlMedicalExaminationTypesList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlMedicalExaminationType medicalExaminationType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (medicalExaminationType != null && ModelState.IsValid)
                {
                    var medicalExaminationTypeInDb = clientDbContext.DdlMedicalExaminationTypes
                        .Where(x => x.Code == medicalExaminationType.Code)
                        .SingleOrDefault();

                    if (medicalExaminationTypeInDb != null)
                    {
                        ModelState.AddModelError("", "The Medical Examination Type" + CustomErrorMessages.ERROR_ALREADY_DEFINED);
                    }
                    else
                    {
                        var newMedicalExaminationType = new DdlMedicalExaminationType
                        {
                            Description = medicalExaminationType.Description,
                            Code = medicalExaminationType.Code,
                            Active = medicalExaminationType.Active
                        };

                        clientDbContext.DdlMedicalExaminationTypes.Add(newMedicalExaminationType);
                        clientDbContext.SaveChanges();
                        medicalExaminationType.MedicalExaminationTypeId = newMedicalExaminationType.MedicalExaminationTypeId;
                    }
                }

                return Json(new[] { medicalExaminationType }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlMedicalExaminationTypesList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlMedicalExaminationType medicalExaminationType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (medicalExaminationType != null && ModelState.IsValid)
                {
               
                    var medicalExaminationTypeInDb = clientDbContext.DdlMedicalExaminationTypes
                        .Where(x => x.MedicalExaminationTypeId == medicalExaminationType.MedicalExaminationTypeId)
                        .SingleOrDefault();
                    var medicalExaminationTypelist = clientDbContext.DdlMedicalExaminationTypes.Where(n => n.MedicalExaminationTypeId != medicalExaminationType.MedicalExaminationTypeId).ToList();
                    var usedmedicalExaminationTypelist = clientDbContext.PersonExaminations.ToList();
                    if (medicalExaminationTypeInDb != null)
                    {
                        if (medicalExaminationTypelist.Select(m => m.Code).Contains(medicalExaminationType.Code) || medicalExaminationTypelist.Select(s => s.Description).Contains(medicalExaminationType.Description))
                        {
                            ModelState.AddModelError("", "The medical Examination Type  Code or Description already exists!");
                        }
                        else if (usedmedicalExaminationTypelist.Select(s => s.DdlMedicalExaminationType.Description).Contains(medicalExaminationType.Description))
                        {
                            ModelState.AddModelError("", "Can not Inactive due to record is in Use");
                        }
                        else
                        {
                            medicalExaminationTypeInDb.Code = medicalExaminationType.Code;
                            medicalExaminationTypeInDb.Description = medicalExaminationType.Description;
                            medicalExaminationTypeInDb.Active = medicalExaminationType.Active;
                            clientDbContext.SaveChanges();
                        }
                    }
                }

                return Json(new[] { medicalExaminationType }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlMedicalExaminationTypesList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlMedicalExaminationType medicalExaminationType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (medicalExaminationType != null)
                {
                    DdlMedicalExaminationType medicalExaminationTypeInDb = clientDbContext.DdlMedicalExaminationTypes
                        .Where(x => x.MedicalExaminationTypeId == medicalExaminationType.MedicalExaminationTypeId).SingleOrDefault();

                    if (medicalExaminationTypeInDb != null)
                    {
                        clientDbContext.DdlMedicalExaminationTypes.Remove(medicalExaminationTypeInDb);

                        try
                        {
                            clientDbContext.SaveChanges();
                        }
                        catch// (Exception err)
                        {
                            ModelState.AddModelError("", "Can not be deleted due to record is already in use");
                        }

                    }
                }

                return Json(new[] { medicalExaminationType }.ToDataSourceResult(request, ModelState));
            }
        }


        public JsonResult GetMedicalExaminationTypes()
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var medicalExaminationTypes = clientDbContext.DdlMedicalExaminationTypes.Where(x => x.Active == true).OrderBy(x => x.Description).ToList();

                return Json(medicalExaminationTypes, JsonRequestBehavior.AllowGet);
            }

        }
    }
}