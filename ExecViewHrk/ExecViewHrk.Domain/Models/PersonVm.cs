using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExecViewHrk.Models
{
    public class PersonVm
    {
        [Required]
        public int PersonId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string PersonName { get; set; }
        public string FileNumber { get; set; }
        public int CompanyCodeId { get; set; }
        public string CompanyCode { get; set; }
        public int BusinessLevelNbr { get; set; }
    }
    public class Users
    {
        public int PersonId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string PersonName { get; set; }
        public string SSN { get; set; }
        public string FileNumber { get; set; }
        public int CompanyCodeId { get; set; }
        public string CompanyCode { get; set; }

        public string Email { get; set; }
        public string ClientDBUserName { get; set; }
        public string MasterDBUserName { get; set; }
        public string MasterDBGuid { get; set; }
    }
}