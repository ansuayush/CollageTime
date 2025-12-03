using ExecViewHrk.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExecViewHrk.Models;
using ExecViewHrk.EfClient;

namespace ExecViewHrk.Domain.Repositories
{
    public class DepartmetsRepository : RepositoryBase, IDepartmetsRepository
    {
        public List<DepartmentVm> GetDepartmentList()
        {
            List<DepartmentVm> getDepartmentList = new List<DepartmentVm>();
            try
            {
                getDepartmentList = _context.Departments.Where(x => x.IsDeleted == false).Select(d => new DepartmentVm
                {
                    DepartmentId = d.DepartmentId,
                    CompanyCodeId = d.CompanyCodeId,
                    DepartmentCode = d.DepartmentCode,
                    DepartmentDescription = d.DepartmentDescription,
                    Comapnycodecode = d.CompanyCode.CompanyCodeCode,
                    Active = d.IsDepartmentActive
                }).Distinct().OrderBy(s => s.DepartmentDescription).ToList();
                return getDepartmentList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DepartmentVm GetDepartmentDetails(int departmentId)
        {
            DepartmentVm departmentVm = new DepartmentVm();
            try
            {
                if (departmentId != 0)
                {
                    departmentVm = _context.Departments.Where(d => d.DepartmentId == departmentId && d.IsDeleted == false).Select(d => new DepartmentVm
                    {
                        DepartmentId = d.DepartmentId,
                        CompanyCodeId = d.CompanyCodeId,
                        DepartmentCode = d.DepartmentCode,
                        DepartmentDescription = d.DepartmentDescription,
                        Active = d.IsDepartmentActive,
                    }).FirstOrDefault();
                }

                return departmentVm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DepartmentAddUpdate(DepartmentVm departmentVM, string UserId)
        {
            bool result = false;
            var departmentDetails = _context.Departments.Where(x => x.DepartmentId == departmentVM.DepartmentId && x.IsDeleted == false).FirstOrDefault();
            if (departmentDetails != null)
            {
                departmentDetails.CompanyCodeId = departmentVM.CompanyCodeId;
                departmentDetails.DepartmentId = departmentVM.DepartmentId;
                departmentDetails.DepartmentCode = departmentVM.DepartmentCode;
                departmentDetails.DepartmentDescription = departmentVM.DepartmentDescription;
                departmentDetails.IsDepartmentActive = departmentVM.Active;
                departmentDetails.IsDeleted = false;
                departmentDetails.UserId = UserId;
                departmentDetails.LastModifiedDate = DateTime.Now;
                _context.SaveChanges();
                result = true;
            }
            else
            {
                var department = new Department()
                {
                    CompanyCodeId = departmentVM.CompanyCodeId,
                    DepartmentCode = departmentVM.DepartmentCode,
                    DepartmentDescription = departmentVM.DepartmentDescription,
                    IsDepartmentActive = departmentVM.Active,
                    IsDeleted = false,
                    UserId = UserId,
                    LastModifiedDate = DateTime.Now
                };
                _context.Departments.Add(department);
                _context.SaveChanges();
                result = true;
            }
            return result;
        }

        public bool DepartmentDestroy(int departmentID, string UserId)
        {
            //var result = false;
            bool isExist = _context.TimeCards.Any(x => x.DepartmentId == departmentID) || _context.Positions.Any(x => x.DepartmentId == departmentID) ||
                           _context.ManagerDepartments.Any(x => x.DepartmentId == departmentID);
            if (!isExist)
            {
                var department = _context.Departments.Where(x => x.DepartmentId == departmentID && x.IsDeleted == false).SingleOrDefault();
                if (department != null)
                {
                    department.IsDeleted = true;
                    department.DeletedBy = UserId;
                    department.LastModifiedDate = DateTime.Now;
                    _context.SaveChanges();
                    //result = true;
                }
            }
            return isExist;
        }
    }
}
