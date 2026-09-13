using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Helpers;
using ExecViewHrk.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    [Authorize(Roles = "HrkAdministrators,ClientAdministrators,ClientAdminsMultipleCompanies,ClientManagers")]
    public class MetadataManagementController : Controller
    {
        public PartialViewResult IndexPartial()
        {
            try
            {
                Ensure();
            }
            catch (Exception ex)
            {
                ViewBag.SchemaError = ex.GetBaseException().Message;
            }
            return PartialView();
        }

        [HttpGet]
        public JsonResult GetModules()
        {
            using (var db = OpenDb())
            {
                var list = db.StModules.OrderBy(m => m.ModuleName)
                    .Select(m => new StModuleVm { Id = m.Id, ModuleName = m.ModuleName, ControlId = m.ControlId })
                    .ToList();
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetForms(int? moduleId)
        {
            using (var db = OpenDb())
            {
                var q = db.StForms.AsQueryable();
                if (moduleId.HasValue) q = q.Where(f => f.ModuleId == moduleId.Value);
                var list = q.OrderBy(f => f.FormName)
                    .Select(f => new StFormVm
                    {
                        Id = f.Id,
                        FormName = f.FormName,
                        ModuleId = f.ModuleId,
                        ControlId = f.ControlId,
                        IsUsed = f.IsUsed,
                        TableName = f.TableName
                    }).ToList();
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetSections(int? formId)
        {
            using (var db = OpenDb())
            {
                var q = db.StFormSections.AsQueryable();
                if (formId.HasValue) q = q.Where(s => s.FormId == formId.Value);
                var list = q.OrderBy(s => s.Sort).ThenBy(s => s.SectionName)
                    .Select(s => new StSectionVm
                    {
                        Id = s.Id,
                        FormId = s.FormId,
                        SectionName = s.SectionName,
                        Sort = s.Sort,
                        TableName = s.TableName
                    }).ToList();
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetCompanies()
        {
            using (var db = OpenDb())
            {
                var list = db.CompanyCodes.Where(c => c.IsCompanyCodeActive)
                    .OrderBy(c => c.CompanyCodeDescription)
                    .Select(c => new { Id = c.CompanyCodeId, Name = c.CompanyCodeCode + " — " + c.CompanyCodeDescription })
                    .ToList();
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetFields(int? sectionId, int? companyId)
        {
            using (var db = OpenDb())
            {
                if (!sectionId.HasValue)
                    return Json(new { success = true, data = new List<StMetaFieldVm>() }, JsonRequestBehavior.AllowGet);

                var fields = db.StMetaTables.Where(f => f.SectionId == sectionId.Value)
                    .OrderBy(f => f.Id).ToList();
                var fieldIds = fields.Select(f => f.Id).ToList();
                var companyLabels = companyId.HasValue
                    ? db.StMetaDatas.Where(d => fieldIds.Contains(d.FieldId) && d.CompanyId == companyId.Value)
                        .ToDictionary(d => d.FieldId, d => d.DisplayName)
                    : new Dictionary<int, string>();

                var list = fields.Select(f =>
                {
                    string companyLabel = null;
                    companyLabels.TryGetValue(f.Id, out companyLabel);
                    string effective = !string.IsNullOrWhiteSpace(companyLabel) ? companyLabel : f.DisplayName;
                    return new StMetaFieldVm
                    {
                        Id = f.Id,
                        SectionId = f.SectionId,
                        Field = f.Field,
                        FieldType = f.FieldType,
                        DisplayName = f.DisplayName,
                        CompanyDisplayName = companyLabel,
                        EffectiveLabel = effective,
                        IsRequired = f.IsRequired,
                        IsVisible = f.IsVisible ?? f.IsAllowed,
                        IsAllowed = f.IsAllowed,
                        IsReadOnly = f.IsReadOnly,
                        PanelName = f.PanelName,
                        SampleData = f.SampleData
                    };
                }).ToList();

                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetField(int id, int? companyId)
        {
            using (var db = OpenDb())
            {
                var f = db.StMetaTables.FirstOrDefault(x => x.Id == id);
                if (f == null) return Json(new { success = false, message = "Field not found." }, JsonRequestBehavior.AllowGet);

                string companyLabel = null;
                if (companyId.HasValue)
                {
                    var md = db.StMetaDatas.FirstOrDefault(d => d.FieldId == id && d.CompanyId == companyId.Value);
                    if (md != null) companyLabel = md.DisplayName;
                }

                var fieldSec = db.StFieldSecurities.FirstOrDefault(s => s.FieldId == id && s.IsActive);
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        f.Id,
                        f.Field,
                        f.FieldType,
                        DefaultLabel = f.DisplayName,
                        DisplayName = !string.IsNullOrWhiteSpace(companyLabel) ? companyLabel : f.DisplayName,
                        CompanyDisplayName = companyLabel,
                        IsRequired = f.IsRequired == true,
                        IsVisible = (f.IsVisible ?? f.IsAllowed) == true,
                        IsReadOnly = f.IsReadOnly == true,
                        AllowAdd = fieldSec == null || fieldSec.CanAdd,
                        AllowEdit = fieldSec == null || fieldSec.CanEdit
                    }
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveFieldLabel(StFieldSaveVm model)
        {
            if (model == null || model.Id <= 0)
                return Json(new { success = false, message = "Invalid field." });

            using (var db = OpenDb())
            {
                var f = db.StMetaTables.FirstOrDefault(x => x.Id == model.Id);
                if (f == null) return Json(new { success = false, message = "Field not found." });

                f.IsRequired = model.IsRequired;
                f.IsVisible = model.IsVisible;
                f.IsAllowed = model.IsVisible;
                f.IsReadOnly = model.IsReadOnly;

                if (model.CompanyId.HasValue && model.CompanyId.Value > 0)
                {
                    var md = db.StMetaDatas.FirstOrDefault(d => d.FieldId == model.Id && d.CompanyId == model.CompanyId.Value);
                    if (md == null)
                    {
                        md = new StMetaData { FieldId = model.Id, CompanyId = model.CompanyId.Value };
                        db.StMetaDatas.Add(md);
                    }
                    md.DisplayName = model.DisplayName;
                }
                else
                {
                    f.DisplayName = model.DisplayName;
                }

                db.SaveChanges();
                StLabelService.Invalidate();
                return Json(new { success = true, message = "Saved." });
            }
        }

        [HttpGet]
        public JsonResult GetRoles()
        {
            using (var db = OpenDb())
            {
                var list = db.AspNetRoles.OrderBy(r => r.Name)
                    .Select(r => new { r.Id, r.Name })
                    .ToList();
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetFormSecurity(int? formId)
        {
            using (var db = OpenDb())
            {
                var forms = db.StForms.ToDictionary(f => f.Id, f => f.FormName);
                var q = db.StFormSecurities.AsQueryable();
                if (formId.HasValue) q = q.Where(s => s.FormId == formId.Value);
                var list = q.OrderBy(s => s.RoleName).ToList().Select(s => new StSecurityRowVm
                {
                    Id = s.Id,
                    TargetId = s.FormId,
                    TargetName = forms.ContainsKey(s.FormId) ? forms[s.FormId] : ("Form #" + s.FormId),
                    RoleName = s.RoleName,
                    CanView = s.CanView,
                    CanAdd = s.CanAdd,
                    CanEdit = s.CanEdit,
                    CanDelete = s.CanDelete,
                    CanExport = s.CanExport,
                    IsActive = s.IsActive
                }).ToList();
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveFormSecurity(int id, int formId, string roleName, bool canView, bool canAdd, bool canEdit, bool canDelete, bool canExport, bool isActive)
        {
            if (string.IsNullOrWhiteSpace(roleName) || formId <= 0)
                return Json(new { success = false, message = "Form and role are required." });

            using (var db = OpenDb())
            {
                StFormSecurity entity;
                if (id > 0)
                {
                    entity = db.StFormSecurities.FirstOrDefault(x => x.Id == id);
                    if (entity == null) return Json(new { success = false, message = "Not found." });
                }
                else
                {
                    entity = db.StFormSecurities.FirstOrDefault(x => x.FormId == formId && x.RoleName == roleName)
                             ?? new StFormSecurity { FormId = formId, RoleName = roleName.Trim() };
                    if (entity.Id == 0) db.StFormSecurities.Add(entity);
                }
                entity.FormId = formId;
                entity.RoleName = roleName.Trim();
                entity.CanView = canView;
                entity.CanAdd = canAdd;
                entity.CanEdit = canEdit;
                entity.CanDelete = canDelete;
                entity.CanExport = canExport;
                entity.IsActive = isActive;
                db.SaveChanges();
                return Json(new { success = true });
            }
        }

        [HttpPost]
        public JsonResult SaveModule(int id, string moduleName, string controlId)
        {
            using (var db = OpenDb())
            {
                StModule entity;
                if (id > 0)
                {
                    entity = db.StModules.FirstOrDefault(x => x.Id == id);
                    if (entity == null) return Json(new { success = false, message = "Not found." });
                }
                else
                {
                    entity = new StModule();
                    db.StModules.Add(entity);
                }
                entity.ModuleName = moduleName;
                entity.ControlId = controlId;
                db.SaveChanges();
                return Json(new { success = true, id = entity.Id });
            }
        }

        [HttpPost]
        public JsonResult SaveForm(int id, string formName, int moduleId, string controlId, bool isUsed, string tableName)
        {
            using (var db = OpenDb())
            {
                StForm entity;
                if (id > 0)
                {
                    entity = db.StForms.FirstOrDefault(x => x.Id == id);
                    if (entity == null) return Json(new { success = false, message = "Not found." });
                }
                else
                {
                    int nextId = db.StForms.Any() ? db.StForms.Max(f => f.Id) + 1 : 1;
                    entity = new StForm { Id = nextId };
                    db.StForms.Add(entity);
                }
                entity.FormName = formName;
                entity.ModuleId = moduleId;
                entity.ControlId = controlId;
                entity.IsUsed = isUsed;
                entity.TableName = tableName;
                db.SaveChanges();
                return Json(new { success = true, id = entity.Id });
            }
        }

        [HttpPost]
        public JsonResult SaveSection(int id, int formId, string sectionName, int sort, string tableName)
        {
            using (var db = OpenDb())
            {
                StFormSection entity;
                if (id > 0)
                {
                    entity = db.StFormSections.FirstOrDefault(x => x.Id == id);
                    if (entity == null) return Json(new { success = false, message = "Not found." });
                }
                else
                {
                    int nextId = db.StFormSections.Any() ? db.StFormSections.Max(s => s.Id) + 1 : 1;
                    entity = new StFormSection { Id = nextId };
                    db.StFormSections.Add(entity);
                }
                entity.FormId = formId;
                entity.SectionName = sectionName;
                entity.Sort = sort;
                entity.TableName = tableName;
                entity.IsAllowedInWorkFlow = true;
                entity.IsAllowedInReports = true;
                entity.IsAllowedInTemplate = true;
                db.SaveChanges();
                return Json(new { success = true, id = entity.Id });
            }
        }

        [HttpPost]
        public JsonResult SaveMetaField(int id, int sectionId, string field, string fieldType, string displayName, string panelName, bool isRequired, bool isVisible)
        {
            using (var db = OpenDb())
            {
                StMetaTable entity;
                if (id > 0)
                {
                    entity = db.StMetaTables.FirstOrDefault(x => x.Id == id);
                    if (entity == null) return Json(new { success = false, message = "Not found." });
                }
                else
                {
                    entity = new StMetaTable { SectionId = sectionId, IsValid = true, IsAllowed = true };
                    db.StMetaTables.Add(entity);
                }
                entity.SectionId = sectionId;
                entity.Field = field;
                entity.FieldType = fieldType;
                entity.DisplayName = displayName;
                entity.PanelName = panelName;
                entity.IsRequired = isRequired;
                entity.IsVisible = isVisible;
                entity.IsAllowed = isVisible;
                db.SaveChanges();
                StLabelService.Invalidate();
                return Json(new { success = true, id = entity.Id });
            }
        }

        private ClientDbContext OpenDb()
        {
            string conn = User.Identity.GetClientConnectionString();
            var db = new ClientDbContext(conn);
            StFormMetadataSchemaHelper.EnsureSchema(db);
            return db;
        }

        private void Ensure()
        {
            using (OpenDb()) { }
        }
    }
}
