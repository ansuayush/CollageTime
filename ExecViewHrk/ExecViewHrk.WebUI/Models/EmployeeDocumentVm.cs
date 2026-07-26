using System;

namespace ExecViewHrk.WebUI.Models
{
    public class EmployeeDocumentVm
    {
        public int DocumentId { get; set; }
        public int EmployeeId { get; set; }
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string UploadedBy { get; set; }
        public DateTime UploadedDate { get; set; }

        public bool IsSigned { get; set; }
        public string SignedBy { get; set; }
        public DateTime? SignedDate { get; set; }
        public string SignerRole { get; set; }
        public string SignatureName { get; set; }
    }

    public class EmployeeDocumentSearchResultVm
    {
        public int PersonId { get; set; }
        public int EmployeeId { get; set; }
        public string PersonName { get; set; }
        public string FileNumber { get; set; }
        public int EmploymentNumber { get; set; }
        public string CompanyCode { get; set; }
    }
}
