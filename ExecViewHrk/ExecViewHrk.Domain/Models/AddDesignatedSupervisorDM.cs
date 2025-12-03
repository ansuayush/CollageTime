using System.Collections.Generic;

namespace ExecViewHrk.Domain.Models
{
    public class AddDesignatedSupervisorDM
    {
        public List<CurrentSupervisorDM> CurrentSupervisors { get; set; }

        public int SelectedCurrentSupervisor { get; set; }

        public List<ReplaceWithSupervisorDM> ReplaceWithSupervisors { get; set; }

        public int SelectedReplaceWithSupervisor { get; set; }
    }

    public class SupervisorDM
    {
        public int ManagerPersonId { get; set; }

        public string ManagerPersonName { get; set; }
    }

    public class CurrentSupervisorDM : SupervisorDM
    { }

    public class ReplaceWithSupervisorDM : SupervisorDM
    { }
}