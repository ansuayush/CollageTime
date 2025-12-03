using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ExecViewHrk.Models;

namespace ExecViewHrk.WebUI.Models
{
    public class PersonRelationshipVm
    {
        public int PersonRelationshipId { get; set; }

        [Required]
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        [Required]
        public int RelationshipTypeId { get; set; }
        //[Required]
        public string RelationshipDescription { get; set; }
        [Required]
        public int RelationPersonId { get; set; }
        //[Required]
        public string RelationPersonName { get; set; }

        public bool? Dependent { get; set; }

        public bool? EmergencyContact { get; set; }

        public bool? Garnishment { get; set; }

        [DataType(DataType.MultilineText)]
        public string EnteredBy { get; set; }
        public DateTime? EnteredDate { get; set; }

        public List<DropDownModel> lstDdlPersonRelationshipsType { get; set; }

        public List<DropDownModel> lstRelationPersonsList { get; set; }

        public List<PersonPhoneNumberVm> personPhoneNumberVm { get; set; }

        public List<PersonAddressVm> personAddressVm { get; set; }


    }
}