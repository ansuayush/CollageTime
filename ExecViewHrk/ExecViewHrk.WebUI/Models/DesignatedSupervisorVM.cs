using System;

namespace ExecViewHrk.WebUI.Models
{
    public class DesignatedSupervisorVM
    {
        public int ManagerPersonId { get; set; }
        public string ManagerName { get; set; }

        public int NewManagerPersonId { get; set; }
        public string NewManagerPersonName { get; set; }

        public string CreatedByUser { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}