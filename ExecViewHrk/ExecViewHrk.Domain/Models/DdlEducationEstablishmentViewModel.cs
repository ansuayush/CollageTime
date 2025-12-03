using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Models
{
    public class DdlEducationEstablishmentViewModel
    {
        public int EducationEstablishmentId { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Code { get; set; }
        public string AddressLineOne { get; set; }
        public string AddressLineTwo { get; set; }
        public string City { get; set; }
        public int? StateId { get; set; }        
        public string ZipCode { get; set; }
        public int? CountryId { get; set; }
        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public int? EducationTypeId { get; set; }
        public string AccountNumber { get; set; }
        public string Contact { get; set; }
        public string WebAddress { get; set; }
        public string Notes { get; set; }
        public bool Active { get; set; }
        public List<ExecViewHrk.EfClient.DdlState> DdlState { get; set; }
        public List<ExecViewHrk.EfClient.DdlCountry> DdlCountry { get; set; }
        public List<ExecViewHrk.EfClient.DdlEducationType> DdlEducationType { get; set; }
        public List<DropDownModel> StateDropDownList { get; set; }
        public List<DropDownModel> CountryDropDownList { get; set; }
        public List<DropDownModel> EducationTypeDropDownList { get; set; }

    }
}
