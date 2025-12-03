using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ExecViewHrk.WebUI.Models
{
    public class PersonAddressVm
    {
        public int PersonAddressId { get; set; }

        [Required]
        public int PersonId { get; set; }
        public string PersonName { get; set; }

        [Required]
        public int AddressTypeId { get; set; }
        public string AddressDescription { get; set; }

        [Required]
        public string AddressLineOne { get; set; }

        public string AddressLineTwo { get; set; }

        public string City { get; set; }

        public int? StateId { get; set; }
       
        public string StateTitle { get; set; }

        [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Invalid Zip")]
        public string ZipCode { get; set; }

        public int? CountryId { get; set; }
       
        public string CountryDescription { get; set; }

        public bool ?CheckPayrollAddress { get; set; }

        public bool ?CorrespondenceAddress { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        public string EnteredBy { get; set; }
        public DateTime? EnteredDate { get; set; }
        public string ModifiedBy { get; set; }     
        public DateTime? ModifiedDate { get; set; }

        public List<ExecViewHrk.EfClient.DdlAddressType> GetAddressTypes { get; set; }
        public List<ExecViewHrk.EfClient.DdlState> GetStates { get; set; }
        public List<ExecViewHrk.EfClient.DdlCountry> GetCountries { get; set; }    
        public bool IsPrimaryAddress { get; set; }
    }
}