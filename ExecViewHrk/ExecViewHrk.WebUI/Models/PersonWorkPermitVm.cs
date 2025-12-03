using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Models
{
    public class PersonWorkPermitVm
    {
        public int PersonWorkPermitId { get; set; }

        [Required]
        public int PersonId { get; set; }
        public string PersonName { get; set; }

        [Required]
        public int CountryId { get; set; }
       
        public string CountryDescription { get; set; }

        
        public string WorkPermitNumber { get; set; }
        
        public string WorkPermitType { get; set; }
        
        public string IssuingAuthority { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? ExtensionDate { get; set; }

        
        public string Notes { get; set; }
        public string EnteredBy { get; set; }
        public DateTime EnteredDate { get; set; }

        public List<ExecViewHrk.EfClient.DdlCountry> GetCountries { get; set; }
    }
}