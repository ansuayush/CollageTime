using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ExecViewHrk.Models;

namespace ExecViewHrk.WebUI.Models
{
    public class FedralEinVM
    {
        public int FedralEINNbr { get; set; }

        [Required]
        [StringLength(10)]
        public string EIN { get; set; }
        [Required]
        [StringLength(50)]
        public string Description { get; set; }

        public string addressLineOne { get; set; }

        public string addressLineTwo { get; set; }

        public string city { get; set; }

        public byte? stateID { get; set; }

        public string zipCode { get; set; }
        public int? countryID { get; set; }

        public string phoneNumber { get; set; }

        public string faxNumber { get; set; }

        public byte? EEOFileStatusID { get; set; }

        public string notes { get; set; }

        public bool Active { get; set; }

        public bool isNewRecord { get; set; }

        public List<DropDownModel> lstCountries { get; set; }
        public List<DropDownModel> lstStates { get; set; }

        public List<DropDownModel> lstEEOFileStatuses { get; set; }

    }
}