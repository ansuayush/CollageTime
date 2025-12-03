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
    public class DdlSkillTypesController : Controller
    {
        // GET: DdlSkillTypes
        public ActionResult DdlSkillTypesListMaintenance()
        {
            return View();
        }

        public ActionResult DdlSkillTypesList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var skillTypeList = clientDbContext.DdlSkillTypes.OrderBy(e => e.Description).ToList();
                return Json(skillTypeList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlSKillTypesList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlSkillType skillType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (skillType != null && ModelState.IsValid)
                {
                    var skillTypeInDb = clientDbContext.DdlSkillTypes
                        .Where(x => x.Code == skillType.Code)
                        .SingleOrDefault();
                    var skillTypeInDbDesc = clientDbContext.DdlSkillTypes
                        .Where(x => x.Description == skillType.Description)
                        .SingleOrDefault();
                    if (skillTypeInDb != null || skillTypeInDbDesc!=null)
                    {
                        ModelState.AddModelError("", "The Skill Type with same code or description already defined.");
                    }
                    else
                    {
                        var newSKillType = new DdlSkillType
                        {
                            Description = skillType.Description,
                            Code = skillType.Code,
                            Active = skillType.Active
                        };

                        clientDbContext.DdlSkillTypes.Add(newSKillType);
                        clientDbContext.SaveChanges();
                        skillType.SkillTypeId = newSKillType.SkillTypeId;
                    }
                }

                return Json(new[] { skillType }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlSkillTypesList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlSkillType skillType)
        {
            if (skillType != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var SkillTypeInDb = clientDbContext.DdlSkillTypes.Where(x => x.SkillTypeId != skillType.SkillTypeId && (x.Code == skillType.Code || x.Description == skillType.Description)).SingleOrDefault();
                if (SkillTypeInDb == null)
                {
                    SkillTypeInDb = clientDbContext.DdlSkillTypes.Where(x => x.SkillTypeId == skillType.SkillTypeId).SingleOrDefault();

                    if (SkillTypeInDb.Active != skillType.Active && skillType.Active == false)
                    {
                        if (clientDbContext.DdlSkills.Where(x => x.SkillTypeId == skillType.SkillTypeId).Count() == 0)
                        {
                            SkillTypeInDb.Code = skillType.Code;
                            SkillTypeInDb.Description = skillType.Description;
                            SkillTypeInDb.Active = skillType.Active;
                            clientDbContext.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                        }
                    }
                    else
                    {
                        SkillTypeInDb.Code = skillType.Code;
                        SkillTypeInDb.Description = skillType.Description;
                        SkillTypeInDb.Active = skillType.Active;
                        clientDbContext.SaveChanges();
                    }
                }
                else
                {
                    ModelState.AddModelError("", string.Format(CustomErrorMessages.ERROR_LOOKUP_DUPLICATE_RECORD, "skill Type"));
                }
            }

            return Json(new[] { skillType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlSkillTypesList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlSkillType skillType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (skillType != null)
                {
                    DdlSkillType skillTypeInDb = clientDbContext.DdlSkillTypes
                        .Where(x => x.SkillTypeId == skillType.SkillTypeId).SingleOrDefault();

                    if (skillTypeInDb != null)
                    {
                        clientDbContext.DdlSkillTypes.Remove(skillTypeInDb);

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

                return Json(new[] { skillType }.ToDataSourceResult(request, ModelState));
            }
        }


        public JsonResult GetSkillTypes(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var addressTypes = clientDbContext.DdlSkillTypes
                    .Select(m => new
                    {
                        SkillTypeId = m.SkillTypeId,
                        SkillTypeTitle = m.Description,
                    }).OrderBy(x => x.SkillTypeTitle).ToList();

                return Json(addressTypes, JsonRequestBehavior.AllowGet);
            }

        }
    }
}