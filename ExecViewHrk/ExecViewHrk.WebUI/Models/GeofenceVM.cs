using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public  class GeofenceVM
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string GeofenceName { get; set; }
        public string Coordinate { get; set; }
        public string Radius { get; set; }
        public string PunchType { get; set; }
        public string Allocation { get; set; }
        public Nullable<System.Guid> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.Guid> ModifiedBy { get; set; }
        public Nullable<System.DateTime> Modifiedon { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string PlaceAddress { get; set; }
        public string PlaceName { get; set; }
    }
}