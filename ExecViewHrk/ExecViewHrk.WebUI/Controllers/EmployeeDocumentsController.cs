using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Helpers;
using ExecViewHrk.WebUI.Infrastructure;
using ExecViewHrk.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    /// <summary>
    /// Employee Document Scanning &amp; Management.
    /// Workflow: select employee → add scanned pages (or PDF) → preview → save PDF to employee folder → list/download.
    /// Note: Browsers cannot talk to TWAIN/WIA scanners directly. Use your scanner software / WIA driver to produce
    /// images or PDF, then add those pages here. For native TWAIN, use NTwain/NAPS2 in a desktop helper later.
    /// </summary>
    [Authorize]
    public class EmployeeDocumentsController : Controller
    {
        private const string StorageFolderName = "EmployeeDocuments";

        public ActionResult Index()
        {
            return RedirectToAction("EmployeeDocumentsMatrixPartial");
        }

        public PartialViewResult EmployeeDocumentsMatrixPartial()
        {
            EnsureAdminAccess();
            string requestType = User.Identity.GetRequestType();
            if (requestType != "IsSelfService")
                SessionStateHelper.CheckForPersonSelectedValue();
            return DocumentsPartialForSelectedOrLoginEmployee(isSelfService: false);
        }

        public PartialViewResult MyDocumentsPartial()
        {
            return DocumentsPartialForSelectedOrLoginEmployee(isSelfService: true);
        }

        /// <summary>
        /// Admin: documents for the employee selected on Personal Profile (session PERSON_SELECTED_ID).
        /// Employee self-service: documents for the logged-in identity.
        /// </summary>
        private PartialViewResult DocumentsPartialForSelectedOrLoginEmployee(bool isSelfService)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                EnsureEmployeeDocumentsSchema(db);

                EmployeeDocumentSearchResultVm target = isSelfService
                    ? ResolveEmployeeByLogin(db)
                    : ResolveEmployeeBySelectedPerson(db);

                ViewBag.IsSelfService = isSelfService;
                ViewBag.CurrentEmployee = target;
                ViewBag.UsesSelectedPerson = !isSelfService;

                if (target == null)
                {
                    ViewBag.IdentityMessage = isSelfService
                        ? "No employee record is linked to this login."
                        : "Select an employee on Personal Profile first, then open Documents.";
                    return PartialView("EmployeeDocumentsMatrixPartial", new List<EmployeeDocumentVm>());
                }

                var docs = db.EmployeeDocuments
                    .Where(d => d.EmployeeId == target.EmployeeId)
                    .OrderByDescending(d => d.UploadedDate)
                    .Select(d => new EmployeeDocumentVm
                    {
                        DocumentId = d.DocumentId,
                        EmployeeId = d.EmployeeId,
                        FileName = d.FileName,
                        FilePath = d.FilePath,
                        UploadedBy = d.UploadedBy,
                        UploadedDate = d.UploadedDate,
                        IsSigned = d.IsSigned,
                        SignedBy = d.SignedBy,
                        SignedDate = d.SignedDate,
                        SignerRole = d.SignerRole,
                        SignatureName = d.SignatureName
                    })
                    .ToList();

                return PartialView("EmployeeDocumentsMatrixPartial", docs);
            }
        }

        private EmployeeDocumentSearchResultVm ResolveEmployeeByLogin(ClientDbContext db)
        {
            string userName = User.Identity.Name ?? "";
            var person = db.Persons.FirstOrDefault(p => p.eMail == userName);
            if (person == null) return null;
            return ResolveEmployeeByPersonId(db, person.PersonId);
        }

        private EmployeeDocumentSearchResultVm ResolveEmployeeBySelectedPerson(ClientDbContext db)
        {
            object selected = SessionStateHelper.Get(SessionStateKeys.PERSON_SELECTED_ID);
            if (selected == null) return null;

            int personId;
            try { personId = Convert.ToInt32(selected); }
            catch { return null; }

            if (personId <= 0) return null;
            return ResolveEmployeeByPersonId(db, personId);
        }

        private static EmployeeDocumentSearchResultVm ResolveEmployeeByPersonId(ClientDbContext db, int personId)
        {
            var person = db.Persons.FirstOrDefault(p => p.PersonId == personId);
            if (person == null) return null;

            var emp = db.Employees
                .Where(e => e.PersonId == person.PersonId)
                .OrderByDescending(e => e.EmploymentNumber)
                .FirstOrDefault();
            if (emp == null) return null;

            return new EmployeeDocumentSearchResultVm
            {
                PersonId = person.PersonId,
                EmployeeId = emp.EmployeeId,
                PersonName = person.Lastname + ", " + person.Firstname,
                FileNumber = emp.FileNumber,
                EmploymentNumber = emp.EmploymentNumber,
                CompanyCode = emp.CompanyCode
            };
        }

        private EmployeeDocumentSearchResultVm ResolveCurrentEmployee(ClientDbContext db)
        {
            return ResolveEmployeeByLogin(db);
        }

        [HttpGet]
        public JsonResult HelperPing()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                string userName = User.Identity.Name ?? "";
                bool isEmployee = User.IsInRole("ClientEmployees");
                bool isAdmin = User.IsInRole("HrkAdministrators")
                               || User.IsInRole("ClientAdministrators")
                               || User.IsInRole("ClientManagers");

                var currentEmployee = ResolveCurrentEmployee(db);

                return Json(new
                {
                    success = true,
                    user = userName,
                    isEmployee = isEmployee,
                    isAdmin = isAdmin,
                    currentEmployee = currentEmployee,
                    message = "Scanner helper session OK."
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetCurrentEmployee()
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                string userName = User.Identity.Name ?? "";
                var person = db.Persons.FirstOrDefault(p => p.eMail == userName);
                if (person == null)
                    return Json(new { success = false, message = "No person linked to this login." }, JsonRequestBehavior.AllowGet);

                var emp = db.Employees
                    .Where(e => e.PersonId == person.PersonId)
                    .OrderByDescending(e => e.EmploymentNumber)
                    .FirstOrDefault();
                if (emp == null)
                    return Json(new { success = false, message = "No employee record for this login." }, JsonRequestBehavior.AllowGet);

                return Json(new
                {
                    success = true,
                    PersonId = person.PersonId,
                    EmployeeId = emp.EmployeeId,
                    PersonName = person.Lastname + ", " + person.Firstname,
                    FileNumber = emp.FileNumber,
                    EmploymentNumber = emp.EmploymentNumber,
                    CompanyCode = emp.CompanyCode
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult SearchEmployees(string text)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text) || text.Trim().Length < 2)
                    return Json(new List<EmployeeDocumentSearchResultVm>(), JsonRequestBehavior.AllowGet);

                string connString = User.Identity.GetClientConnectionString();
                string q = text.Trim().ToLower();
                bool employeeOnly = User.IsInRole("ClientEmployees")
                                    && !User.IsInRole("ClientAdministrators")
                                    && !User.IsInRole("HrkAdministrators")
                                    && !User.IsInRole("ClientManagers");

                using (var db = new ClientDbContext(connString))
                {
                    int selfPersonId = 0;
                    if (employeeOnly)
                    {
                        selfPersonId = db.Persons.Where(x => x.eMail == User.Identity.Name).Select(x => x.PersonId).SingleOrDefault();
                    }

                    var results = (from e in db.Employees
                                   join p in db.Persons on e.PersonId equals p.PersonId
                                   where (!employeeOnly || e.PersonId == selfPersonId)
                                         && (p.Lastname.ToLower().Contains(q)
                                             || p.Firstname.ToLower().Contains(q)
                                             || (p.eMail != null && p.eMail.ToLower().Contains(q))
                                             || (e.FileNumber != null && e.FileNumber.ToLower().Contains(q)))
                                   orderby p.Lastname, p.Firstname, e.EmploymentNumber
                                   select new EmployeeDocumentSearchResultVm
                                   {
                                       PersonId = p.PersonId,
                                       EmployeeId = e.EmployeeId,
                                       PersonName = p.Lastname + ", " + p.Firstname,
                                       FileNumber = e.FileNumber,
                                       EmploymentNumber = e.EmploymentNumber,
                                       CompanyCode = e.CompanyCode
                                   })
                        .Take(25)
                        .ToList();

                    return Json(results, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetDocuments(int employeeId)
        {
            EnsureCanAccessEmployee(employeeId);

            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                EnsureEmployeeDocumentsSchema(db);

                var docs = db.EmployeeDocuments
                    .Where(d => d.EmployeeId == employeeId)
                    .OrderByDescending(d => d.UploadedDate)
                    .Select(d => new EmployeeDocumentVm
                    {
                        DocumentId = d.DocumentId,
                        EmployeeId = d.EmployeeId,
                        FileName = d.FileName,
                        UploadedBy = d.UploadedBy,
                        UploadedDate = d.UploadedDate,
                        IsSigned = d.IsSigned,
                        SignedBy = d.SignedBy,
                        SignedDate = d.SignedDate,
                        SignerRole = d.SignerRole,
                        SignatureName = d.SignatureName
                    })
                    .ToList();

                return Json(docs, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Optional e-sign by Admin or Employee. Typed name and/or signature image (base64). Both optional individually if the other is present.
        /// </summary>
        [HttpPost]
        public JsonResult SignDocument(int documentId, string signerRole, string signatureName, string signatureImageBase64 = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(signatureName) && string.IsNullOrWhiteSpace(signatureImageBase64))
                    return Json(new { success = false, message = "Provide a typed name and/or a signature image." });

                if (string.IsNullOrWhiteSpace(signatureName))
                    signatureName = "Signed";

                signerRole = (signerRole ?? "").Trim();
                bool asAdmin = string.Equals(signerRole, "Admin", StringComparison.OrdinalIgnoreCase);
                bool asEmployee = string.Equals(signerRole, "Employee", StringComparison.OrdinalIgnoreCase);
                if (!asAdmin && !asEmployee)
                    return Json(new { success = false, message = "Signer role must be Admin or Employee." });

                bool isAdmin = User.IsInRole("HrkAdministrators")
                               || User.IsInRole("HrkAccountManagers")
                               || User.IsInRole("ClientAdministrators")
                               || User.IsInRole("ClientAdminsMultipleCompanies")
                               || User.IsInRole("ClientManagers");
                bool isEmployee = User.IsInRole("ClientEmployees") || isAdmin;

                if (asAdmin && !isAdmin)
                    return Json(new { success = false, message = "Only admins can sign as Admin." });
                if (asEmployee && !isAdmin && !isEmployee)
                    return Json(new { success = false, message = "Not authorized to sign as Employee." });

                string connString = User.Identity.GetClientConnectionString();
                using (var db = new ClientDbContext(connString))
                {
                    EnsureEmployeeDocumentsSchema(db);

                    var doc = db.EmployeeDocuments.FirstOrDefault(d => d.DocumentId == documentId);
                    if (doc == null)
                        return Json(new { success = false, message = "Document not found." });

                    EnsureCanAccessEmployee(doc.EmployeeId);

                    if (asEmployee && !isAdmin)
                    {
                        int personId = db.Persons.Where(x => x.eMail == User.Identity.Name).Select(x => x.PersonId).SingleOrDefault();
                        var emp = db.Employees.FirstOrDefault(e => e.EmployeeId == doc.EmployeeId);
                        if (emp == null || emp.PersonId != personId)
                            return Json(new { success = false, message = "You can only sign your own documents." });
                    }

                    string signaturePath = null;
                    if (!string.IsNullOrWhiteSpace(signatureImageBase64))
                    {
                        var raw = signatureImageBase64;
                        string ext = ".png";
                        if (raw.StartsWith("data:image/jpeg", StringComparison.OrdinalIgnoreCase)
                            || raw.StartsWith("data:image/jpg", StringComparison.OrdinalIgnoreCase))
                            ext = ".jpg";
                        else if (raw.StartsWith("data:image/gif", StringComparison.OrdinalIgnoreCase))
                            ext = ".gif";
                        var comma = raw.IndexOf(',');
                        if (comma >= 0) raw = raw.Substring(comma + 1);
                        byte[] bytes = Convert.FromBase64String(raw);
                        string root = Server.MapPath("~/App_Data/" + StorageFolderName);
                        string employeeFolder = Path.Combine(root, doc.EmployeeId.ToString());
                        Directory.CreateDirectory(employeeFolder);
                        string file = "sign_" + doc.DocumentId + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ext;
                        string physical = Path.Combine(employeeFolder, file);
                        System.IO.File.WriteAllBytes(physical, bytes);
                        signaturePath = Path.Combine(StorageFolderName, doc.EmployeeId.ToString(), file).Replace('\\', '/');

                        // Append signature as the last page of the stored PDF
                        try
                        {
                            string docPhysical = MapStoragePath(doc.FilePath);
                            if (System.IO.File.Exists(docPhysical))
                            {
                                byte[] existingPdf = System.IO.File.ReadAllBytes(docPhysical);
                                using (var sigMs = new MemoryStream(bytes))
                                {
                                    byte[] withSig = PdfSignatureHelper.AppendSignaturePage(existingPdf, sigMs, signatureName.Trim());
                                    System.IO.File.WriteAllBytes(docPhysical, withSig);
                                }
                            }
                        }
                        catch
                        {
                            // Metadata signature still saved even if PDF append fails
                        }
                    }

                    doc.IsSigned = true;
                    doc.SignedBy = User.Identity.Name ?? "";
                    doc.SignedDate = DateTime.Now;
                    doc.SignerRole = asAdmin ? "Admin" : "Employee";
                    doc.SignatureName = signatureName.Trim();
                    if (!string.IsNullOrEmpty(signaturePath))
                        doc.SignatureImagePath = signaturePath;

                    db.SaveChanges();

                    return Json(new
                    {
                        success = true,
                        message = "Document signed by " + doc.SignerRole + ".",
                        document = new EmployeeDocumentVm
                        {
                            DocumentId = doc.DocumentId,
                            EmployeeId = doc.EmployeeId,
                            FileName = doc.FileName,
                            UploadedBy = doc.UploadedBy,
                            UploadedDate = doc.UploadedDate,
                            IsSigned = doc.IsSigned,
                            SignedBy = doc.SignedBy,
                            SignedDate = doc.SignedDate,
                            SignerRole = doc.SignerRole,
                            SignatureName = doc.SignatureName
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = FormatExceptionMessage(ex) });
            }
        }

        [HttpPost]
        public JsonResult UploadAndSave(int employeeId, string documentTitle, string signerRole = null, string signatureName = null)
        {
            // Employees may upload to their own folder; EnsureCanAccessEmployee enforces that.
            EnsureCanAccessEmployee(employeeId);

            if (Request.Files == null || Request.Files.Count == 0)
                return Json(new { success = false, message = "Add at least one scanned page or PDF." });

            if (string.IsNullOrWhiteSpace(documentTitle))
                return Json(new { success = false, message = "Document name is required." });

            documentTitle = EnsurePdfExtension(documentTitle.Trim());

            var imageStreams = new List<Stream>();
            byte[] directPdf = null;
            string originalPdfName = null;
            MemoryStream signatureStream = null;
            bool attachSignature = false;

            try
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    if (file == null || file.ContentLength == 0)
                        continue;

                    string fieldName = (Request.Files.AllKeys != null && i < Request.Files.AllKeys.Length)
                        ? (Request.Files.AllKeys[i] ?? "")
                        : "";
                    string ext = Path.GetExtension(file.FileName ?? string.Empty).ToLowerInvariant();
                    string contentType = (file.ContentType ?? string.Empty).ToLowerInvariant();

                    // Optional signature image (form field name: signatureFile)
                    if (string.Equals(fieldName, "signatureFile", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(fieldName, "signature", StringComparison.OrdinalIgnoreCase))
                    {
                        if (IsImage(ext, contentType))
                        {
                            signatureStream = new MemoryStream();
                            file.InputStream.CopyTo(signatureStream);
                            signatureStream.Position = 0;
                            attachSignature = true;
                        }
                        continue;
                    }

                    if (ext == ".pdf" || contentType.Contains("pdf"))
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.InputStream.CopyTo(ms);
                            directPdf = ms.ToArray();
                        }
                        originalPdfName = Path.GetFileName(file.FileName);
                        continue;
                    }

                    if (IsImage(ext, contentType))
                    {
                        var copy = new MemoryStream();
                        file.InputStream.CopyTo(copy);
                        copy.Position = 0;
                        imageStreams.Add(copy);
                    }
                }

                // Also accept attachSignature flag without requiring checkbox naming
                if (!attachSignature && !string.IsNullOrWhiteSpace(Request["attachSignature"])
                    && Request["attachSignature"] == "1")
                {
                    // no image — skip append
                }

                byte[] pdfBytes;
                string fileName;

                if (directPdf != null)
                {
                    pdfBytes = directPdf;
                    fileName = SanitizeFileName(string.IsNullOrWhiteSpace(documentTitle)
                        ? (originalPdfName ?? ("Document_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf"))
                        : EnsurePdfExtension(documentTitle));
                }
                else if (imageStreams.Count > 0)
                {
                    pdfBytes = SimplePdfBuilder.BuildPdfFromImageStreams(imageStreams);
                    fileName = SanitizeFileName(string.IsNullOrWhiteSpace(documentTitle)
                        ? ("Scan_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf")
                        : EnsurePdfExtension(documentTitle));
                }
                else
                {
                    return Json(new { success = false, message = "Only image pages (JPG/PNG) or a PDF are supported." });
                }

                // Optional: append signature as last page of the PDF
                if (attachSignature && signatureStream != null)
                {
                    signatureStream.Position = 0;
                    string nameForPage = string.IsNullOrWhiteSpace(signatureName) ? null : signatureName.Trim();
                    pdfBytes = PdfSignatureHelper.AppendSignaturePage(pdfBytes, signatureStream, nameForPage);
                }

                string relativePath = SavePdfToEmployeeFolder(employeeId, fileName, pdfBytes);
                var saved = SaveMetadata(employeeId, fileName, relativePath);

                // Optional: also mark signed in metadata when signature was attached
                if (attachSignature && saved != null && saved.DocumentId > 0)
                {
                    if (signatureStream != null)
                        signatureStream.Position = 0;
                    ApplySignatureMetadata(
                        saved.DocumentId,
                        string.IsNullOrWhiteSpace(signerRole) ? "Employee" : signerRole,
                        string.IsNullOrWhiteSpace(signatureName) ? "Signed" : signatureName.Trim(),
                        signatureStream);
                    saved.IsSigned = true;
                    saved.SignerRole = string.IsNullOrWhiteSpace(signerRole) ? "Employee" : signerRole;
                    saved.SignatureName = string.IsNullOrWhiteSpace(signatureName) ? "Signed" : signatureName.Trim();
                }

                return Json(new
                {
                    success = true,
                    message = attachSignature
                        ? "Document saved with signature page at the end."
                        : "Document saved to employee folder.",
                    document = saved
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = FormatExceptionMessage(ex) });
            }
            finally
            {
                foreach (var s in imageStreams)
                    s.Dispose();
                if (signatureStream != null)
                    signatureStream.Dispose();
            }
        }

        private void ApplySignatureMetadata(int documentId, string signerRole, string signatureName, Stream signatureImageStream)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                EnsureEmployeeDocumentsSchema(db);
                var doc = db.EmployeeDocuments.FirstOrDefault(d => d.DocumentId == documentId);
                if (doc == null) return;

                bool asAdmin = string.Equals(signerRole, "Admin", StringComparison.OrdinalIgnoreCase);
                doc.IsSigned = true;
                doc.SignedBy = User.Identity.Name ?? "";
                doc.SignedDate = DateTime.Now;
                doc.SignerRole = asAdmin ? "Admin" : "Employee";
                doc.SignatureName = signatureName;

                if (signatureImageStream != null)
                {
                    signatureImageStream.Position = 0;
                    string root = Server.MapPath("~/App_Data/" + StorageFolderName);
                    string employeeFolder = Path.Combine(root, doc.EmployeeId.ToString());
                    Directory.CreateDirectory(employeeFolder);
                    string file = "sign_" + doc.DocumentId + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
                    string physical = Path.Combine(employeeFolder, file);
                    using (var fs = System.IO.File.Create(physical))
                        signatureImageStream.CopyTo(fs);
                    doc.SignatureImagePath = Path.Combine(StorageFolderName, doc.EmployeeId.ToString(), file).Replace('\\', '/');
                }

                db.SaveChanges();
            }
        }

        [HttpPost]
        public JsonResult DeleteDocument(int documentId)
        {
            EnsureAdminAccess();

            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                var doc = db.EmployeeDocuments.FirstOrDefault(d => d.DocumentId == documentId);
                if (doc == null)
                    return Json(new { success = false, message = "Document not found." });

                string physical = MapStoragePath(doc.FilePath);
                if (System.IO.File.Exists(physical))
                    System.IO.File.Delete(physical);

                db.EmployeeDocuments.Remove(doc);
                db.SaveChanges();
                return Json(new { success = true, message = "Document deleted." });
            }
        }

        [HttpGet]
        public ActionResult Download(int documentId)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                var doc = db.EmployeeDocuments.FirstOrDefault(d => d.DocumentId == documentId);
                if (doc == null)
                    return HttpNotFound();

                EnsureCanAccessEmployee(doc.EmployeeId);

                string physical = MapStoragePath(doc.FilePath);
                if (!System.IO.File.Exists(physical))
                    return HttpNotFound("File missing on server.");

                return File(physical, "application/pdf", doc.FileName);
            }
        }

        private EmployeeDocumentVm SaveMetadata(int employeeId, string fileName, string relativePath)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                EnsureEmployeeDocumentsSchema(db);

                var entity = new EmployeeDocument
                {
                    EmployeeId = employeeId,
                    FileName = fileName,
                    FilePath = relativePath,
                    UploadedBy = Truncate(User.Identity.Name ?? "system", 100),
                    UploadedDate = DateTime.Now,
                    IsSigned = false
                };
                db.EmployeeDocuments.Add(entity);
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    throw new InvalidOperationException(FormatExceptionMessage(ex), ex);
                }

                return new EmployeeDocumentVm
                {
                    DocumentId = entity.DocumentId,
                    EmployeeId = entity.EmployeeId,
                    FileName = entity.FileName,
                    FilePath = entity.FilePath,
                    UploadedBy = entity.UploadedBy,
                    UploadedDate = entity.UploadedDate,
                    IsSigned = entity.IsSigned,
                    SignedBy = entity.SignedBy,
                    SignedDate = entity.SignedDate,
                    SignerRole = entity.SignerRole,
                    SignatureName = entity.SignatureName
                };
            }
        }

        /// <summary>
        /// Creates EmployeeDocuments / adds signature columns if the client DB was created before those scripts ran.
        /// </summary>
        private static void EnsureEmployeeDocumentsSchema(ClientDbContext db)
        {
            db.Database.ExecuteSqlCommand(@"
IF OBJECT_ID(N'[dbo].[EmployeeDocuments]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[EmployeeDocuments] (
        [DocumentId]   INT            IDENTITY (1, 1) NOT NULL,
        [EmployeeId]   INT            NOT NULL,
        [FileName]     NVARCHAR (260) NOT NULL,
        [FilePath]     NVARCHAR (500) NOT NULL,
        [UploadedBy]   NVARCHAR (100) NOT NULL,
        [UploadedDate] DATETIME       NOT NULL,
        [IsSigned]     BIT            NOT NULL CONSTRAINT [DF_EmployeeDocuments_IsSigned] DEFAULT (0),
        [SignedBy]     NVARCHAR (100) NULL,
        [SignedDate]   DATETIME       NULL,
        [SignerRole]   NVARCHAR (20)  NULL,
        [SignatureName] NVARCHAR (150) NULL,
        [SignatureImagePath] NVARCHAR (500) NULL,
        CONSTRAINT [PK_EmployeeDocuments] PRIMARY KEY CLUSTERED ([DocumentId] ASC),
        CONSTRAINT [FK_EmployeeDocuments_Employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([EmployeeId])
    );
    CREATE NONCLUSTERED INDEX [IX_EmployeeDocuments_EmployeeId]
        ON [dbo].[EmployeeDocuments]([EmployeeId] ASC);
END

IF COL_LENGTH('dbo.EmployeeDocuments', 'IsSigned') IS NULL
BEGIN
    ALTER TABLE [dbo].[EmployeeDocuments] ADD
        [IsSigned] BIT NOT NULL CONSTRAINT [DF_EmployeeDocuments_IsSigned] DEFAULT (0),
        [SignedBy] NVARCHAR(100) NULL,
        [SignedDate] DATETIME NULL,
        [SignerRole] NVARCHAR(20) NULL,
        [SignatureName] NVARCHAR(150) NULL,
        [SignatureImagePath] NVARCHAR(500) NULL;
END
");
        }

        private static string FormatExceptionMessage(Exception ex)
        {
            var parts = new List<string>();
            for (var cur = ex; cur != null; cur = cur.InnerException)
            {
                if (!string.IsNullOrWhiteSpace(cur.Message) && (parts.Count == 0 || parts[parts.Count - 1] != cur.Message))
                    parts.Add(cur.Message);
            }
            return string.Join(" → ", parts);
        }

        private static string Truncate(string value, int max)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= max) return value ?? "";
            return value.Substring(0, max);
        }

        private string SavePdfToEmployeeFolder(int employeeId, string fileName, byte[] pdfBytes)
        {
            string root = Server.MapPath("~/App_Data/" + StorageFolderName);
            string employeeFolder = Path.Combine(root, employeeId.ToString());
            Directory.CreateDirectory(employeeFolder);

            string uniqueName = Path.GetFileNameWithoutExtension(fileName)
                + "_" + DateTime.Now.ToString("yyyyMMddHHmmss")
                + Path.GetExtension(fileName);
            string physical = Path.Combine(employeeFolder, uniqueName);
            System.IO.File.WriteAllBytes(physical, pdfBytes);

            return Path.Combine(StorageFolderName, employeeId.ToString(), uniqueName).Replace('\\', '/');
        }

        private string MapStoragePath(string relativePath)
        {
            string safe = (relativePath ?? string.Empty).Replace('/', Path.DirectorySeparatorChar);
            if (safe.IndexOf("..", StringComparison.Ordinal) >= 0)
                throw new InvalidOperationException("Invalid file path.");
            return Path.Combine(Server.MapPath("~/App_Data"), safe);
        }

        private static bool IsImage(string ext, string contentType)
        {
            return ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp" || ext == ".tif" || ext == ".tiff"
                   || contentType.StartsWith("image/");
        }

        private static string EnsurePdfExtension(string name)
        {
            name = name.Trim();
            if (!name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                name += ".pdf";
            return name;
        }

        private static string SanitizeFileName(string name)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name;
        }

        private void EnsureAdminAccess()
        {
            string requestType = User.Identity.GetRequestType();
            if (requestType == "NSS" && User.IsInRole("ClientEmployees") && !User.IsInRole("ClientManagers"))
                throw new Exception("Client Employee trying to access NSS document scan.");
        }

        private void EnsureCanAccessEmployee(int employeeId)
        {
            string connString = User.Identity.GetClientConnectionString();
            using (var db = new ClientDbContext(connString))
            {
                var emp = db.Employees.FirstOrDefault(e => e.EmployeeId == employeeId);
                if (emp == null)
                    throw new Exception("Employee not found.");

                if (User.IsInRole("ClientEmployees") && !User.IsInRole("ClientAdministrators") && !User.IsInRole("HrkAdministrators") && !User.IsInRole("ClientManagers"))
                {
                    int personId = db.Persons.Where(x => x.eMail == User.Identity.Name).Select(x => x.PersonId).SingleOrDefault();
                    if (emp.PersonId != personId)
                        throw new Exception("Not authorized to access this employee document.");
                }
            }
        }
    }
}
