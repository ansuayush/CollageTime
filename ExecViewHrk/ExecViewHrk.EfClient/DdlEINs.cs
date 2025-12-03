namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    public partial class DdlEINs
    {
        [Key]
        public int FedralEINNbr { get; set; }
      
        [StringLength(50)]
        public string description { get; set; }
        public string EIN { get; set; }
        public string addressLineOne { get; set; }
        public string addressLineTwo { get; set; }

        public string city { get; set; }
        public byte? stateID { get; set; }
        public string zipCode { get; set; }
        public int? countryID { get; set; }
        public string phoneNumber { get; set; }
        public string faxNumber { get; set; }
        public byte? EEOFileStatusID { get; set; }
        public bool active { get; set; }
        public string notes { get; set; }
       
    }
}
