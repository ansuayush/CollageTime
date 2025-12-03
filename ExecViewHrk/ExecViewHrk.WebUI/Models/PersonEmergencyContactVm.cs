using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class PersonEmergencyContactVm
    {
        public int RelationPersonId { get; set; }
        public string PersonName { get; set; }
        public DateTime? DOB { get; set; }
        public string RelationshipType { get; set; }
        //public string PersonPhoneNumbers { get; set; }
        //public List<String> PhoneNumber { get; set; }
        //public HashSet<ExecViewHrk.EfClient.PersonPhoneNumber> PhoneNumber { get; set; }
       public virtual ICollection<ExecViewHrk.EfClient.PersonPhoneNumber> PersonPhoneNumbers { get; set; }
    }
}