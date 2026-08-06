using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Helpers;
using ExecViewHrk.WebUI.Models;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    [Authorize]
    public class OnboardingSetupController : Controller
    {
        public PartialViewResult ProfilesPartial()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);
                var list = db.OnboardingProfiles
                    .OrderBy(p => p.ProfileName)
                    .Select(p => new OnboardingProfileVm
                    {
                        ProfileId = p.ProfileId,
                        ProfileName = p.ProfileName,
                        Description = p.Description,
                        IsActive = p.IsActive,
                        DocumentCount = p.Documents.Count(d => d.IsActive)
                    })
                    .ToList();
                return PartialView(list);
            }
        }

        [HttpGet]
        public JsonResult GetProfiles()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);
                var list = db.OnboardingProfiles
                    .OrderBy(p => p.ProfileName)
                    .Select(p => new
                    {
                        p.ProfileId,
                        p.ProfileName,
                        p.Description,
                        p.IsActive,
                        DocumentCount = p.Documents.Count(d => d.IsActive)
                    })
                    .ToList();
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveProfile(int profileId, string profileName, string description, bool isActive)
        {
            if (string.IsNullOrWhiteSpace(profileName))
                return Json(new { success = false, message = "Profile name is required." });

            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);
                OnboardingProfile entity;
                if (profileId > 0)
                {
                    entity = db.OnboardingProfiles.FirstOrDefault(p => p.ProfileId == profileId);
                    if (entity == null)
                        return Json(new { success = false, message = "Profile not found." });
                    entity.ModifiedBy = User.Identity.Name;
                    entity.ModifiedDate = DateTime.Now;
                }
                else
                {
                    entity = new OnboardingProfile
                    {
                        CreatedBy = User.Identity.Name,
                        CreatedDate = DateTime.Now
                    };
                    db.OnboardingProfiles.Add(entity);
                }

                entity.ProfileName = profileName.Trim();
                entity.Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
                entity.IsActive = isActive;
                db.SaveChanges();
                return Json(new { success = true, profileId = entity.ProfileId, message = "Profile saved." });
            }
        }

        [HttpPost]
        public JsonResult DeleteProfile(int profileId)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);
                var entity = db.OnboardingProfiles.FirstOrDefault(p => p.ProfileId == profileId);
                if (entity == null)
                    return Json(new { success = false, message = "Profile not found." });

                var docs = db.OnboardingProfileDocuments.Where(d => d.ProfileId == profileId).ToList();
                foreach (var d in docs)
                {
                    if (!string.IsNullOrEmpty(d.FilePath))
                    {
                        string physical = Server.MapPath("~/App_Data/" + d.FilePath);
                        if (System.IO.File.Exists(physical))
                            System.IO.File.Delete(physical);
                    }
                    db.OnboardingProfileDocuments.Remove(d);
                }
                db.OnboardingProfiles.Remove(entity);
                db.SaveChanges();
                return Json(new { success = true, message = "Profile deleted." });
            }
        }

        [HttpGet]
        public JsonResult GetProfileDocuments(int profileId)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);
                var types = db.OnboardingLookups.Where(l => l.LookupType == "DocumentType").ToList();
                var docs = db.OnboardingProfileDocuments
                    .Where(d => d.ProfileId == profileId)
                    .OrderBy(d => d.SortOrder)
                    .ThenBy(d => d.DocumentName)
                    .ToList()
                    .Select(d => new OnboardingProfileDocumentVm
                    {
                        ProfileDocumentId = d.ProfileDocumentId,
                        ProfileId = d.ProfileId,
                        DocumentName = d.DocumentName,
                        DocumentTypeId = d.DocumentTypeId,
                        DocumentTypeName = types.Where(t => t.LookupId == d.DocumentTypeId).Select(t => t.Description).FirstOrDefault(),
                        FileName = d.FileName,
                        FilePath = d.FilePath,
                        RequiresSignature = d.RequiresSignature,
                        EnableUpload = d.EnableUpload,
                        SortOrder = d.SortOrder,
                        IsActive = d.IsActive
                    })
                    .ToList();
                return Json(new { success = true, data = docs }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveProfileDocument(int profileDocumentId, int profileId, string documentName,
            int? documentTypeId, bool requiresSignature, bool enableUpload, int sortOrder, bool isActive)
        {
            if (string.IsNullOrWhiteSpace(documentName))
                return Json(new { success = false, message = "Document name is required." });

            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);
                if (!db.OnboardingProfiles.Any(p => p.ProfileId == profileId))
                    return Json(new { success = false, message = "Profile not found." });

                OnboardingProfileDocument entity;
                if (profileDocumentId > 0)
                {
                    entity = db.OnboardingProfileDocuments.FirstOrDefault(d => d.ProfileDocumentId == profileDocumentId);
                    if (entity == null)
                        return Json(new { success = false, message = "Document not found." });
                }
                else
                {
                    entity = new OnboardingProfileDocument { ProfileId = profileId };
                    db.OnboardingProfileDocuments.Add(entity);
                }

                entity.DocumentName = documentName.Trim();
                entity.DocumentTypeId = documentTypeId;
                entity.RequiresSignature = requiresSignature;
                entity.EnableUpload = enableUpload;
                entity.SortOrder = sortOrder;
                entity.IsActive = isActive;

                if (Request.Files != null && Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    if (file != null && file.ContentLength > 0)
                    {
                        string relative = SaveLibraryFile(profileId, file);
                        entity.FileName = Path.GetFileName(file.FileName);
                        entity.FilePath = relative;
                    }
                }

                db.SaveChanges();
                return Json(new { success = true, profileDocumentId = entity.ProfileDocumentId, message = "Document saved." });
            }
        }

        [HttpPost]
        public JsonResult DeleteProfileDocument(int profileDocumentId)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);
                var entity = db.OnboardingProfileDocuments.FirstOrDefault(d => d.ProfileDocumentId == profileDocumentId);
                if (entity == null)
                    return Json(new { success = false, message = "Document not found." });

                if (!string.IsNullOrEmpty(entity.FilePath))
                {
                    string physical = Server.MapPath("~/App_Data/" + entity.FilePath);
                    if (System.IO.File.Exists(physical))
                        System.IO.File.Delete(physical);
                }
                db.OnboardingProfileDocuments.Remove(entity);
                db.SaveChanges();
                return Json(new { success = true, message = "Document deleted." });
            }
        }

        [HttpGet]
        public ActionResult DownloadProfileDocument(int profileDocumentId)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                var doc = db.OnboardingProfileDocuments.FirstOrDefault(d => d.ProfileDocumentId == profileDocumentId);
                if (doc == null || string.IsNullOrEmpty(doc.FilePath))
                    return HttpNotFound();
                string physical = Server.MapPath("~/App_Data/" + doc.FilePath);
                if (!System.IO.File.Exists(physical))
                    return HttpNotFound();
                return File(physical, "application/pdf", doc.FileName ?? "document.pdf");
            }
        }

        [HttpGet]
        public JsonResult GetLookups(string lookupType)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);
                var q = db.OnboardingLookups.AsQueryable();
                if (!string.IsNullOrWhiteSpace(lookupType))
                    q = q.Where(l => l.LookupType == lookupType);
                var list = q.OrderBy(l => l.LookupType).ThenBy(l => l.SortOrder).ThenBy(l => l.Description)
                    .Select(l => new { l.LookupId, l.LookupType, l.Code, l.Description, l.SortOrder, l.IsActive })
                    .ToList();
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveLookup(int lookupId, string lookupType, string code, string description, int sortOrder, bool isActive)
        {
            if (string.IsNullOrWhiteSpace(lookupType) || string.IsNullOrWhiteSpace(description))
                return Json(new { success = false, message = "Type and description are required." });

            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                SelfOnboardingSchemaHelper.EnsureSchema(db);
                OnboardingLookup entity;
                if (lookupId > 0)
                {
                    entity = db.OnboardingLookups.FirstOrDefault(l => l.LookupId == lookupId);
                    if (entity == null)
                        return Json(new { success = false, message = "Lookup not found." });
                }
                else
                {
                    entity = new OnboardingLookup();
                    db.OnboardingLookups.Add(entity);
                }
                entity.LookupType = lookupType.Trim();
                entity.Code = string.IsNullOrWhiteSpace(code) ? null : code.Trim();
                entity.Description = description.Trim();
                entity.SortOrder = sortOrder;
                entity.IsActive = isActive;
                db.SaveChanges();
                return Json(new { success = true, lookupId = entity.LookupId });
            }
        }

        [HttpPost]
        public JsonResult DeleteLookup(int lookupId)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                var entity = db.OnboardingLookups.FirstOrDefault(l => l.LookupId == lookupId);
                if (entity == null)
                    return Json(new { success = false, message = "Lookup not found." });
                db.OnboardingLookups.Remove(entity);
                db.SaveChanges();
                return Json(new { success = true });
            }
        }

        private string SaveLibraryFile(int profileId, HttpPostedFileBase file)
        {
            string root = Server.MapPath("~/App_Data/" + SelfOnboardingSchemaHelper.StorageFolderName + "/Library/" + profileId);
            Directory.CreateDirectory(root);
            string safe = Path.GetFileNameWithoutExtension(file.FileName);
            foreach (char c in Path.GetInvalidFileNameChars())
                safe = safe.Replace(c, '_');
            string name = safe + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(file.FileName);
            string physical = Path.Combine(root, name);
            file.SaveAs(physical);
            return Path.Combine(SelfOnboardingSchemaHelper.StorageFolderName, "Library", profileId.ToString(), name).Replace('\\', '/');
        }
    }
}
