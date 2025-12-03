using System.Collections.Generic;

namespace ExecViewHrk.Models
{
    public class PersonProfileVm
    {

        public int PersonId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string CompanyCode { get; set; }
        public string FileNumber { get; set; }
        public string PositionTitle { get; set; }
        public string HireDate { get; set; }
        public string Salary { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public byte[] PersonImageData { get; set; }
        public string PersonImageMimeType { get; set; }
        public string PersonImageBase64 { get; set; }
        public List<PersonVm> PersonsList { get; set; }
        public List<PersonVm> EmployeeList { get; set; }
        public int SearchIndex { get; set; }
        public string PreviousPersonId { get; set; }
        public string NextPersonId { get; set; }
        public string PositionDescription { get; set; }
        public int EmployeeId { get; set; }
        public int ManagerID { get; set; }

    }
}