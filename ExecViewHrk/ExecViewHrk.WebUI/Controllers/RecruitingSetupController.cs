using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Helpers;
using ExecViewHrk.WebUI.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    [Authorize]
    public class RecruitingSetupController : Controller
    {
        public PartialViewResult QuestionsPartial()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                JobRecruitingSchemaHelper.EnsureSchema(db);
                var list = db.RecruitingQuestions.OrderBy(q => q.WizardPage).ThenBy(q => q.SortOrder).ToList()
                    .Select(q => new RecruitingQuestionVm
                    {
                        QuestionId = q.QuestionId,
                        QuestionText = q.QuestionText,
                        QuestionType = q.QuestionType,
                        Choices = q.Choices,
                        WizardPage = q.WizardPage,
                        SortOrder = q.SortOrder,
                        IsRequired = q.IsRequired,
                        IsActive = q.IsActive
                    }).ToList();
                return PartialView(list);
            }
        }

        [HttpPost]
        public JsonResult SaveQuestion(RecruitingQuestionVm model)
        {
            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.QuestionText))
                    return Json(new { success = false, message = "Question text is required." });

                string connString = User.Identity.GetClientConnectionString();
                using (var db = new ClientDbContext(connString))
                {
                    JobRecruitingSchemaHelper.EnsureSchema(db);
                    RecruitingQuestion entity;
                    if (model.QuestionId > 0)
                    {
                        entity = db.RecruitingQuestions.FirstOrDefault(x => x.QuestionId == model.QuestionId);
                        if (entity == null) return Json(new { success = false, message = "Not found" });
                    }
                    else
                    {
                        entity = new RecruitingQuestion();
                        db.RecruitingQuestions.Add(entity);
                    }
                    entity.QuestionText = model.QuestionText.Trim();
                    entity.QuestionType = string.IsNullOrWhiteSpace(model.QuestionType) ? "Text" : model.QuestionType;
                    entity.Choices = model.Choices;
                    entity.WizardPage = model.WizardPage < 2 ? 2 : (model.WizardPage > 5 ? 5 : model.WizardPage);
                    entity.SortOrder = model.SortOrder;
                    entity.IsRequired = model.IsRequired;
                    entity.IsActive = model.IsActive;
                    db.SaveChanges();
                    return Json(new { success = true, id = entity.QuestionId });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteQuestion(int id)
        {
            try
            {
                string connString = User.Identity.GetClientConnectionString();
                using (var db = new ClientDbContext(connString))
                {
                    JobRecruitingSchemaHelper.EnsureSchema(db);
                    var entity = db.RecruitingQuestions.FirstOrDefault(x => x.QuestionId == id);
                    if (entity == null) return Json(new { success = false, message = "Not found" });
                    db.RecruitingQuestions.Remove(entity);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public PartialViewResult DocumentsPartial()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                JobRecruitingSchemaHelper.EnsureSchema(db);
                var list = db.RecruitingDocuments.OrderBy(d => d.SortOrder).ToList()
                    .Select(d => new RecruitingDocumentVm
                    {
                        DocumentSetupId = d.DocumentSetupId,
                        DocumentName = d.DocumentName,
                        Instructions = d.Instructions,
                        IsRequired = d.IsRequired,
                        RequiresSignature = d.RequiresSignature,
                        SortOrder = d.SortOrder,
                        IsActive = d.IsActive
                    }).ToList();
                return PartialView(list);
            }
        }

        [HttpPost]
        public JsonResult SaveDocument(RecruitingDocumentVm model)
        {
            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.DocumentName))
                    return Json(new { success = false, message = "Document name is required." });

                string connString = User.Identity.GetClientConnectionString();
                using (var db = new ClientDbContext(connString))
                {
                    JobRecruitingSchemaHelper.EnsureSchema(db);
                    RecruitingDocument entity;
                    if (model.DocumentSetupId > 0)
                    {
                        entity = db.RecruitingDocuments.FirstOrDefault(x => x.DocumentSetupId == model.DocumentSetupId);
                        if (entity == null) return Json(new { success = false, message = "Not found" });
                    }
                    else
                    {
                        entity = new RecruitingDocument();
                        db.RecruitingDocuments.Add(entity);
                    }
                    entity.DocumentName = model.DocumentName.Trim();
                    entity.Instructions = model.Instructions;
                    entity.IsRequired = model.IsRequired;
                    entity.RequiresSignature = model.RequiresSignature;
                    entity.SortOrder = model.SortOrder;
                    entity.IsActive = model.IsActive;
                    db.SaveChanges();
                    return Json(new { success = true, id = entity.DocumentSetupId });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteDocument(int id)
        {
            try
            {
                string connString = User.Identity.GetClientConnectionString();
                using (var db = new ClientDbContext(connString))
                {
                    JobRecruitingSchemaHelper.EnsureSchema(db);
                    var entity = db.RecruitingDocuments.FirstOrDefault(x => x.DocumentSetupId == id);
                    if (entity == null) return Json(new { success = false, message = "Not found" });
                    db.RecruitingDocuments.Remove(entity);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public PartialViewResult ConfigPartial()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                JobRecruitingSchemaHelper.EnsureSchema(db);
                var cfg = JobRecruitingSchemaHelper.GetOrCreateConfig(db);
                int employerId = 0;
                int.TryParse(User.Identity.GetClientAdminEmployerID(), out employerId);
                if (employerId <= 0)
                {
                    int.TryParse(User.Identity.GetSelectedClientID(), out employerId);
                }
                var vm = new RecruitingConfigVm
                {
                    ConfigId = cfg.ConfigId,
                    HomePageHtml = cfg.HomePageHtml,
                    IntroductionHtml = cfg.IntroductionHtml,
                    ReviewSubmitHtml = cfg.ReviewSubmitHtml,
                    AttestationHtml = cfg.AttestationHtml,
                    EmployerId = employerId,
                    ExternalApplyUrl = Url.Action("Index", "Apply", new { employerId = employerId }, Request.Url.Scheme)
                };
                return PartialView(vm);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult SaveConfig(RecruitingConfigVm model)
        {
            try
            {
                string connString = User.Identity.GetClientConnectionString();
                using (var db = new ClientDbContext(connString))
                {
                    JobRecruitingSchemaHelper.EnsureSchema(db);
                    var cfg = JobRecruitingSchemaHelper.GetOrCreateConfig(db);
                    cfg.HomePageHtml = model.HomePageHtml;
                    cfg.IntroductionHtml = model.IntroductionHtml;
                    cfg.ReviewSubmitHtml = model.ReviewSubmitHtml;
                    cfg.AttestationHtml = model.AttestationHtml;
                    cfg.ModifiedBy = User.Identity.Name;
                    cfg.ModifiedDate = DateTime.Now;
                    db.SaveChanges();
                    return Json(new { success = true });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
