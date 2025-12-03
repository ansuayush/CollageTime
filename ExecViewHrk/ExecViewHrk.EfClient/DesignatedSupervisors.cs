using System;
using System.ComponentModel.DataAnnotations;

namespace ExecViewHrk.EfClient
{
    public partial class DesignatedSupervisors
    {
        [Key]       
        public int ManagerPersonId { get; set; }

        public int DesignatedManagerPersonId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}