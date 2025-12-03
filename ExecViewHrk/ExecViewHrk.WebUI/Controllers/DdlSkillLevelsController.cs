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
    public class DdlSkillLevelsController : Controller
    {
        // GET: DdlSkillLevels
        public ActionResult DdlSkillLevelsListMaintenance()
        {
            return View();
        }

        public ActionResult DdlSkillLevelsList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var skillLevelList = clientDbContext.DdlSkillLevels.OrderBy(e => e.Description).ToList();
                return Json(skillLevelList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlSkillLevelsList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlSkillLevel skillLevel)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (skillLevel != null && ModelState.IsValid)
                {
                    var skillLevelInDb = clientDbContext.DdlSkillLevels
                        .Where(x => x.Code == skillLevel.Code)
                        .SingleOrDefault();
                    var skillLevelInDbDesc = clientDbContext.DdlSkillLevels
                       .Where(x => x.Description == skillLevel.Description)
                       .SingleOrDefault();
                    if (skillLevelInDb != null || skillLevelInDbDesc!=null)
                    {
                        ModelState.AddModelError("", "The Skill Level" + CustomErrorMessages.ERROR_ALREADY_DEFINED);
                    }
                    else
                    {
                        var newSkillLevel = new DdlSkillLevel
                        {
                            Description = skillLevel.Description,
                            Code = skillLevel.Code,
                            Active = skillLevel.Active
                        };

                        clientDbContext.DdlSkillLevels.Add(newSkillLevel);
                        clientDbContext.SaveChanges();
                        skillLevel.SkillLevelId = newSkillLevel.SkillLevelId;
                    }
                }

                return Json(new[] { skillLevel }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlSkillLevelsList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlSkillLevel skillLevel)
        {
            if (skillLevel != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var skillLevelInDb = clientDbContext.DdlSkillLevels.Where(x => x.SkillLevelId != skillLevel.SkillLevelId && (x.Code == skillLevel.Code || x.Description == skillLevel.Description)).SingleOrDefault();
                if (skillLevelInDb == null)
                {
                    skillLevelInDb = clientDbContext.DdlSkillLevels.Where(x => x.SkillLevelId == skillLevel.SkillLevelId).SingleOrDefault();

                    if (skillLevelInDb.Active != skillLevel.Active && skillLevel.Active == false)
                    {
                        if (clientDbContext.PersonSkills.Where(x => x.SkillId == skillLevel.SkillLevelId).Count() == 0)
                        {
                            skillLevelInDb.Code = skillLevel.Code;
                            skillLevelInDb.Description = skillLevel.Description;
                            skillLevelInDb.Active = skillLevel.Active;
                            clientDbContext.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("", CustomErrorMessages.ERROR_CANNOT_BE_INACTIVE);
                        }
                    }
                    else
                    {
                        skillLevelInDb.Code = skillLevel.Code;
                        skillLevelInDb.Description = skillLevel.Description;
                        skillLevelInDb.Active = skillLevel.Active;
                        clientDbContext.SaveChanges();
                    }
                }
                else
                {
                    ModelState.AddModelError("", string.Format(CustomErrorMessages.ERROR_LOOKUP_DUPLICATE_RECORD, "skill Levels"));
                }
            }
            return Json(new[] { skillLevel }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlSkillLevelsList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlSkillLevel skillLevel)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (skillLevel != null)
                {
                    DdlSkillLevel skillLevelInDb = clientDbContext.DdlSkillLevels
                        .Where(x => x.SkillLevelId == skillLevel.SkillLevelId).SingleOrDefault();

                    if (skillLevelInDb != null)
                    {
                        clientDbContext.DdlSkillLevels.Remove(skillLevelInDb);

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

                return Json(new[] { skillLevel }.ToDataSourceResult(request, ModelState));
            }
        }


        public JsonResult GetSkillLevels(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var skillLevels = clientDbContext.DdlSkillLevels
                    .Where(x => x.Active == true)
                    .Select(m => new
                    {
                        SkillLevelId = m.SkillLevelId,
                        SkillLevelDescription = m.Description
                    }).OrderBy(x => x.SkillLevelDescription).ToList();

                return Json(skillLevels, JsonRequestBehavior.AllowGet);
            }

        }
    }
}