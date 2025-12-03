using ExecViewHrk.Domain.Interface;
using ExecViewHrk.EfClient;
using ExecViewHrk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Domain.Repositories
{
    public class ManagerDepartmentRepository : RepositoryBase, IManagerDepartmentRepository
    {
        #region Dropdown List

        /// <summary>
        /// Populates Managers List for Dropdown
        /// </summary>
        /// <returns></returns>
        public List<ManagerVm> PopulateManagers()
        {
            List<ManagerVm> getManagerList = new List<ManagerVm>();
            try
            {
                getManagerList = _context.Managers
                     .Include("Person")
                    .Select(m => new ManagerVm
                    {
                        ManagerId = m.ManagerId,
                        PersonName = m.Person.Firstname + " " + m.Person.Lastname
                    }).Distinct().OrderBy(s => s.PersonName).ToList();
                return getManagerList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Populates Departments List for Dropdown
        /// </summary>
        /// <returns></returns>
        public List<DepartmentVm> PopulateDepartments()
        {
            List<DepartmentVm> getDepartmentList = new List<DepartmentVm>();
            try
            {
                getDepartmentList = _context.Departments
                     .Include("CompanyCode").Where(x=> x.IsDeleted == false)
                    .Select(m => new DepartmentVm
                    {
                        DepartmentId = m.DepartmentId,
                        CompCode_DeptCode_DeptDescription = m.CompanyCode.CompanyCodeCode + "-" + m.DepartmentCode + "-" + m.DepartmentDescription
                    }).Distinct().OrderBy(s => s.CompCode_DeptCode_DeptDescription).ToList();
                return getDepartmentList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region List and Details. Insert, Update and Delete

        /// <summary>
        /// Returns the List of Manager Departments
        /// </summary>
        /// <returns></returns>
        public List<ManagerDepartmentVM> GetManagerDepartmentList()
        {
            List<ManagerDepartmentVM> managerDepartmentList = new List<ManagerDepartmentVM>();
            try
            {
                managerDepartmentList = _context.ManagerDepartments.Select(m => new ManagerDepartmentVM()
                {
                    ManagerDepartmentId = m.ManagerDepartmentId,
                    ManagerId = m.ManagerId,
                    DepartmentId = m.DepartmentId
                }).OrderBy(m => m.ManagerDepartmentId).ToList();
                return managerDepartmentList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Returns Manager Department Details
        /// </summary>
        /// <param name="managerDepartmentId"></param>
        /// <returns></returns>
        public ManagerDepartmentVM GetManagerDepartmentDetails(int managerDepartmentId)
        {
            ManagerDepartmentVM managerDepartmentVM = new ManagerDepartmentVM();
            try
            {
                if (managerDepartmentId != 0)
                {
                    managerDepartmentVM = _context.ManagerDepartments.Where(m => m.ManagerDepartmentId == managerDepartmentId).Select(m => new ManagerDepartmentVM()
                    {
                        ManagerDepartmentId = m.ManagerDepartmentId,
                        ManagerId = m.ManagerId,
                        DepartmentId = m.DepartmentId
                    }).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return managerDepartmentVM;
        }

        /// <summary>
        /// Inserts and Updates the Mangaer Department record
        /// </summary>
        /// <param name="managerDepartmentVM"></param>
        /// <returns></returns>
        public bool ManagerDepartmentsSaveAjax(ManagerDepartmentVM managerDepartmentVM)
        {
            bool result = false;
            var managerdepartmentDetails = _context.ManagerDepartments.Where(x => x.ManagerDepartmentId == managerDepartmentVM.ManagerDepartmentId).FirstOrDefault();
            if (managerdepartmentDetails != null)
            {
                managerdepartmentDetails.ManagerId = managerDepartmentVM.ManagerId;
                managerdepartmentDetails.DepartmentId = managerDepartmentVM.DepartmentId;
                _context.SaveChanges();
                result = true;
            }
            else
            {
                var newManagerDepartment = new ManagerDepartment
                {
                    ManagerId = managerDepartmentVM.ManagerId,
                    DepartmentId = managerDepartmentVM.DepartmentId
                };
                _context.ManagerDepartments.Add(newManagerDepartment);
                _context.SaveChanges();
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Deletes the Record
        /// </summary>
        /// <param name="managerDepartmentId"></param>
        /// <returns></returns>
        public bool ManagerDepartmentsList_Destroy(int managerDepartmentId)
        {
            var result = false;
            var managerDepartment = _context.ManagerDepartments.Where(x => x.ManagerDepartmentId == managerDepartmentId).SingleOrDefault();
            if (managerDepartment != null)
            {
                _context.ManagerDepartments.Remove(managerDepartment);
                _context.SaveChanges();
                result = true;
            }
            return result;
        }
        #endregion

    }
}
