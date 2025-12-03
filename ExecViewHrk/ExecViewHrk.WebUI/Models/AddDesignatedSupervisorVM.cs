using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class AddDesignatedSupervisorVM
    {
        public List<CurrentSupervisor> CurrentSupervisors { get; set; }

        public int SelectedCurrentSupervisor { get; set; }

        public List<ReplaceWithSupervisor> ReplaceWithSupervisors { get; set; }

        public int SelectedReplaceWithSupervisor { get; set; }
    }

    public class SupervisorVM
    {
        public int ManagerPersonId { get; set; }

        public string ManagerPersonName { get; set; }
    }

    public class CurrentSupervisor : SupervisorVM
    { }

    public class ReplaceWithSupervisor : SupervisorVM
    { }
}