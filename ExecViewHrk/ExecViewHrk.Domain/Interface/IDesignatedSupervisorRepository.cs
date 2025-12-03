using ExecViewHrk.Domain.Models;
using System.Collections.Generic;

namespace ExecViewHrk.Domain.Interface
{
    public interface IDesignatedSupervisorRepository
    {
        List<DesignatedSupervisorDM> GetDesignatedSupervisors(int? employerId, bool isHrkAdmin);

        List<CurrentSupervisorDM> GetCurrentSupervisors(int? employerId, bool isHrkAdmin);

        List<ReplaceWithSupervisorDM> GetReplaceWithSupervisors();

        bool SaveDesignatedSupervisor(AddDesignatedSupervisorDM model, string userName);

        bool DeleteDesignatedSupervisor(int ManagerPersonId);
    }
}