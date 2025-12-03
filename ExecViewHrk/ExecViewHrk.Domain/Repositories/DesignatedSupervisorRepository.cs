using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Domain.Models;
using ExecViewHrk.EfClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExecViewHrk.Domain.Repositories
{
    public class DesignatedSupervisorRepository : RepositoryBase, IDesignatedSupervisorRepository
    {
        public List<DesignatedSupervisorDM> GetDesignatedSupervisors(int? employerId, bool isHrkAdmin)
        {
            return Query<DesignatedSupervisorDM>("sp_GetDesignatedSupervisors", new { @ManagerPersonId = employerId, @IsHrkAdmin = isHrkAdmin }).ToList();
        }

        public List<CurrentSupervisorDM> GetCurrentSupervisors(int? employerId, bool isHrkAdmin)
        {
            return Query<CurrentSupervisorDM>("sp_GetCurrentSupervisors", new { @ManagerPersonId = employerId, @IsHrkAdmin = isHrkAdmin }).ToList();
        }

        public List<ReplaceWithSupervisorDM> GetReplaceWithSupervisors()
        {
            return Query<ReplaceWithSupervisorDM>("sp_GetReplaceWithSupervisors").ToList();
        }

        public bool SaveDesignatedSupervisor(AddDesignatedSupervisorDM model, string userName)
        {
            try
            {
                var newRec = new DesignatedSupervisors()
                {
                    ManagerPersonId = model.SelectedCurrentSupervisor,
                    DesignatedManagerPersonId = model.SelectedReplaceWithSupervisor,
                    CreatedBy = userName,
                    CreatedDate = DateTime.Now
                };
                _context.DesignatedSupervisors.Add(newRec);

                // To Get all Employee Positions with the original manager
                var managerPositions = (from epos in _context.E_Positions
                                        where epos.ReportsToID == model.SelectedCurrentSupervisor
                                        select epos
                          ).ToList();

                DesignatedPositions designatedPosition;
                foreach (var pos in managerPositions)
                {
                    designatedPosition = new DesignatedPositions();
                    // Add E_Position Id and Original Manager Id to the table "DesignatedPositions"
                    designatedPosition.ManagerPersonId = model.SelectedCurrentSupervisor;
                    designatedPosition.E_PositionId = pos.E_PositionId;
                    _context.DesignatedPositions.Add(designatedPosition);
                    // Update all E_Positions ReportsToId with new designated supervisor and set "IsDesignated" column to true
                    pos.ReportsToID = model.SelectedReplaceWithSupervisor;
                    pos.IsDesignated = true;
                }

                var currentManagerId = (from mgr in _context.Managers
                                        where mgr.PersonId == model.SelectedCurrentSupervisor
                                        select mgr.ManagerId).FirstOrDefault();

                var designatedManagerId = (from mgr in _context.Managers
                                           where mgr.PersonId == model.SelectedReplaceWithSupervisor
                                           select mgr.ManagerId).FirstOrDefault();

                // To Get all Manager Departments with the original manager
                var managerDepartments = (from mDep in _context.ManagerDepartments
                                          where mDep.ManagerId == currentManagerId
                                          select mDep).ToList();

                DesignatedManagerDepartment designatedManagerDept;
                foreach (var mDept in managerDepartments)
                {
                    designatedManagerDept = new DesignatedManagerDepartment();
                    // Add ManagerDepartment Id and Original Manager Person Id to the table "DesignatedManagerDepartment"
                    designatedManagerDept.ManagerPersonId = model.SelectedCurrentSupervisor;
                    designatedManagerDept.ManagerDepartmentId = mDept.ManagerDepartmentId;
                    _context.DesignatedManagerDepartment.Add(designatedManagerDept);
                    // Update all ManagerDepartments ManagerId with new designated supervisor with ManagerId and set "IsDesignated" column to true
                    mDept.ManagerId = designatedManagerId;
                    mDept.IsDesignated = true;
                }

                return _context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
                //return false;
            }
        }

        public bool DeleteDesignatedSupervisor(int ManagerPersonId)
        {
            DesignatedSupervisors managerRecord = _context.DesignatedSupervisors.Where(x => x.ManagerPersonId == ManagerPersonId).SingleOrDefault();
            if (managerRecord != null)
            {
                var currentManagerId = (from mgr in _context.Managers
                                        where mgr.PersonId == managerRecord.ManagerPersonId
                                        select mgr.ManagerId).FirstOrDefault();

                var designatedManagerId = (from mgr in _context.Managers
                                           where mgr.PersonId == managerRecord.DesignatedManagerPersonId
                                           select mgr.ManagerId).FirstOrDefault();

                // To Get all Employee Positions with the original manager
                var ePosIdList = (from dPos in _context.DesignatedPositions
                            where dPos.ManagerPersonId == managerRecord.ManagerPersonId
                            select dPos.E_PositionId).ToList();
                var ePositions = _context.E_Positions.Where(e => ePosIdList.Contains(e.E_PositionId));
                foreach (var pos in ePositions)
                {
                    // Update all E_Positions ReportsToId with new designated supervisor and set "IsDesignated" column to true
                    pos.ReportsToID = ManagerPersonId;
                    pos.IsDesignated = null;
                }
                var designatedPositions = (from desPos in _context.DesignatedPositions
                                           where desPos.ManagerPersonId == ManagerPersonId
                                           select desPos).ToList();
                designatedPositions.ForEach(e => _context.DesignatedPositions.Remove(e));

                // To Get all Manager Departments with the original manager
                var managerDep = (from dmd in _context.DesignatedManagerDepartment
                                  where dmd.ManagerPersonId == managerRecord.ManagerPersonId
                                  select dmd.ManagerDepartmentId).ToList();
                var managerDepartments = _context.ManagerDepartments.Where(e => managerDep.Contains(e.ManagerDepartmentId));
                foreach (var mDept in managerDepartments)
                {
                    mDept.ManagerId = currentManagerId;
                    mDept.IsDesignated = null;
                }
                var designatedManagers = (from desMgr in _context.DesignatedManagerDepartment
                                          where desMgr.ManagerPersonId == ManagerPersonId
                                          select desMgr).ToList();

                designatedManagers.ForEach(e => _context.DesignatedManagerDepartment.Remove(e));

                _context.DesignatedSupervisors.Remove(managerRecord);
                return _context.SaveChanges() > 0;
            }
            return false;
        }
    }
}