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
using ExecViewHrk.Models;
using System.Data.Entity.Validation;

namespace ExecViewHrk.WebUI.Controllers
{
    public class DdlSkillsController : Controller
    {
        // GET: DdlSkills
        public ActionResult DdlSkillsListMaintenance()
        {
            PopulateSkillTypes();
            //string connString = User.Identity.GetClientConnectionString();

            //using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            //{
            //    ViewData["skillTypesList"] = PopulateField(clientDbContext).PopulateSkillTypes();
            //}
            return View();
        }

        public ActionResult DdlSkillsList_Read([DataSourceRequest]DataSourceRequest request)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var skillList = clientDbContext.DdlSkills.OrderBy(e => e.Description).ToList();
                return Json(skillList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlSkillsList_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlSkill skill)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (skill != null && ModelState.IsValid)
                {
                    var skillInDb = clientDbContext.DdlSkills
                        .Where(x => x.Code == skill.Code)
                        .SingleOrDefault();

                    if (skillInDb != null)
                    {
                        ModelState.AddModelError("", "The skill" + CustomErrorMessages.ERROR_ALREADY_DEFINED);
                    }
                    else
                    {
                        var newSkill = new DdlSkill
                        {
                            Description = skill.Description,
                            Code = skill.Code,
                            Active = true,
                            SkillTypeId = skill.SkillTypeId,
                           // CobraEligible = false,
                        };

                        clientDbContext.DdlSkills.Add(newSkill);
                        clientDbContext.SaveChanges();
                        skill.SkillId = newSkill.SkillId;
                    }
                }

                return Json(new[] { skill }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlSkillsList_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.DdlSkill skill)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (skill != null && ModelState.IsValid)
                {
                    var skillInDb = clientDbContext.DdlSkills
                        .Where(x => x.SkillId == skill.SkillId)
                        .SingleOrDefault();

                    if (skillInDb != null)
                    {
                        skillInDb.Code = skill.Code;
                        skillInDb.Description = skill.Description;
                        skillInDb.Active = skill.Active;
                        skillInDb.SkillTypeId = skill.SkillTypeId;
                       // skillInDb.CobraEligible = skill.CobraEligible;
                        clientDbContext.SaveChanges();
                    }
                }

                return Json(new[] { skill }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DdlSkillsList_Destroy([DataSourceRequest] DataSourceRequest request
            , DdlSkill skill)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                if (skill != null)
                {
                    DdlSkill skillInDb = clientDbContext.DdlSkills
                        .Where(x => x.SkillId == skill.SkillId).SingleOrDefault();

                    if (skillInDb != null)
                    {
                        clientDbContext.DdlSkills.Remove(skillInDb);

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

                return Json(new[] { skill }.ToDataSourceResult(request, ModelState));
            }
        }

        public JsonResult GetSkills(string text)
        {
            string connString = User.Identity.GetClientConnectionString();

            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {

                var skills = clientDbContext.DdlSkills
                    .Where(x => x.Active == true)
                    .Select(m => new
                    {
                       
                        SkillId = m.SkillId,
                        SkillDescription = m.Description,
                        SkillTypeId = m.SkillTypeId,
                       // SkillCobraEligible = m.CobraEligible,
                    }).OrderBy(x => x.SkillDescription).ToList();

                return Json(skills, JsonRequestBehavior.AllowGet);
            }

        }

        private void PopulateSkillTypes()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                var skillTypesList = new ClientDbContext(connString).DdlSkillTypes
                        .Select(s => new SkillTypeVm
                        {
                            SkillTypeId = s.SkillTypeId,
                            SkillTypeDescription = s.Description //+ "-" + e.SkillId.ToString()
                        })
                        .OrderBy(s => s.SkillTypeDescription).ToList();

                skillTypesList.Insert(0, new SkillTypeVm { SkillTypeId = 0, SkillTypeDescription = "--select one--" });

                ViewData["skillTypesList"] = skillTypesList;
               // ViewData["defaultSkillType"] = skillTypesList.First();
            }
        }


        public ActionResult skillEdit(int skillid)
        {
            bool isNewRecord = false;
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            SkillVM skillVM = new SkillVM();
            if (skillid == 0)
            {
                isNewRecord = true;
                skillVM.SkillId = 0;
                skillVM.isNewRecord = isNewRecord;
            }
            else
            {
                skillVM = clientDbContext.DdlSkills
                         .Where(x => x.SkillId == skillid)
                         .Select(e => new SkillVM
                         {
                             SkillId=e.SkillId,
                             Description = e.Description,
                            Code=e.Code,
                             Active = e.Active,
                             SkillTypeId=e.SkillTypeId,
                             
                         }).FirstOrDefault();
            }
            skillVM.SkillTypeDrop = getAllskilltype().CleanUp();
            return View("skillEdit", skillVM);
        }
        public List<DropDownModel> getAllskilltype()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            var skilltype = clientDbContext.DdlSkillTypes
                 .Where(s => s.Active == true)
                    .Select(s => new DropDownModel
                    {
                        keyvalue = s.SkillTypeId.ToString(),
                        keydescription = s.Description
                    }).OrderBy(s => s.keydescription).ToList();

            return skilltype;
        }


        [HttpPost]
        

        public ActionResult SkillSaveAjax(SkillVM skillVM)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            string _message = "";
            bool recordIsNew = false;

            DdlSkill DdlSkill = clientDbContext.DdlSkills
                .Where(x => x.SkillId == skillVM.SkillId).SingleOrDefault();

            if (DdlSkill == null)
            {
                var isRecordExists = clientDbContext.DdlSkills.Where(x => x.SkillId == skillVM.SkillId || x.Code == skillVM.Code || x.Description == skillVM.Description).SingleOrDefault();
                if (isRecordExists != null)
                {
                    return Json(new { succeed = false, Message = "The Skill  record  exists for the selected Skill code or Description." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    DdlSkill = new DdlSkill();
                    DdlSkill.Code = skillVM.Code;
                    DdlSkill.Description = skillVM.Description;
                    DdlSkill.Active = skillVM.Active;
                    DdlSkill.SkillTypeId = skillVM.SkillTypeId;
                    clientDbContext.DdlSkills.Add(DdlSkill);
                    recordIsNew = true;
                }
            }
            else
            {
                var isRecordExists = clientDbContext.DdlSkills.Where(x => x.SkillId != skillVM.SkillId && (x.Code == skillVM.Code || x.Description == skillVM.Description)).SingleOrDefault();
                if (isRecordExists != null)
                {
                    return Json(new { succeed = false, Message = "The Skill record  exists for the selected Skill code or Title." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    DdlSkill.Code = skillVM.Code;
                    DdlSkill.Description = skillVM.Description;
                    DdlSkill.Active = skillVM.Active;
                    DdlSkill.SkillTypeId = skillVM.SkillTypeId;
                }
            }

            try
            {
                clientDbContext.SaveChanges();
                ViewBag.AlertMessage = recordIsNew == true ? "New Skill  Record Added" : "Skill  Record Saved";
            }
            catch (Exception err)
            {

                IEnumerable<DbEntityValidationResult> errors = clientDbContext.GetValidationErrors();
                if (errors.Count() == 0)
                {
                    _message += err.InnerException.InnerException.Message;
                }
                else
                {
                    foreach (DbEntityValidationResult error in errors)
                    {
                        foreach (var valError in error.ValidationErrors)
                        {
                            if (_message != "") _message += "<br />";
                            _message += valError.ErrorMessage;
                        }
                    }
                }
                return Json(new { Message = _message, succeed = false }, JsonRequestBehavior.AllowGet);
            }
           
            return Json(new { skillVM, succeed = true, recordIsNew }, JsonRequestBehavior.AllowGet);
        }
    }
}