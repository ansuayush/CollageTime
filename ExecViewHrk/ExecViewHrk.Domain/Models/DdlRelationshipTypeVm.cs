using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExecViewHrk.Models
{
    public class DdlRelationshipTypeVm
    {
        public int RelationshipTypeId { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public bool? IsSpouse { get; set; }
        public bool? CobraEligible { get; set; }
        public bool Active { get; set; }
        //public List<DropDownModel> lstDdlPersonRelationshipsType { get; set; }
        //public List<DropDownModel> lstRelationPersonsList { get; set; }

    }
}
