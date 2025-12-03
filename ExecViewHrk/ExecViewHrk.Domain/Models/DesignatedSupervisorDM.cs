using System;

namespace ExecViewHrk.Domain.Models
{
    public class DesignatedSupervisorDM
    {
        public int ManagerPersonId { get; set; }
        public string ManagerName { get; set; }

        public int NewManagerPersonId { get; set; }
        public string NewManagerPersonName { get; set; }

        public string CreatedByUser { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}