using ExecViewHrk.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using ExecViewHrk.Models;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using ExecViewHrk.EfClient;

namespace ExecViewHrk.Domain.Repositories
{
    public class PositionImportRepository : RepositoryBase, IPositionImportRepository
    {
        public List<PositionImportVM> GetSalaryList(string strPositionCode)
        {

            var list = (from pos in _context.Positions
                        join epo in _context.E_Positions
                         on pos.PositionId equals epo.PositionId
                        where pos.Code == strPositionCode && epo.IsDeleted == false
                        select new PositionImportVM { code = pos.Code }).ToList();
            return list;
        }

        public List<PersonEmployeeVm> GetPersonsList(string strPayGroup, string strFileNumber)
        {

            var list = (from pers in _context.Persons
                        join emp in _context.Employees
                        on pers.PersonId equals emp.PersonId
                        where emp.CompanyCode == strPayGroup && emp.FileNumber == strFileNumber
                        select new PersonEmployeeVm { PersonId = pers.PersonId }).ToList();
            return list;
        }

        public List<PersonEmployeeVm> GetEmployeeList(string strPayGroup, string strFileNumber)
        {

            var list = (from pers in _context.Employees
                        join emp in _context.Persons
                        on pers.PersonId equals emp.PersonId
                        join empstat in _context.DdlEmploymentStatuses on pers.EmploymentStatusId equals empstat.EmploymentStatusId
                        where pers.CompanyCode == strPayGroup && pers.FileNumber == strFileNumber
                        select new PersonEmployeeVm { EmployeeId = pers.EmployeeId }).ToList();
            return list;
        }
        public List<PositionImportVM> GetEmployeePositions()
        {
            var list = (from ep in _context.E_Positions
                        join e in _context.Employees on ep.EmployeeId equals e.EmployeeId
                        join p in _context.Positions on ep.PositionId equals p.PositionId
                        join b in _context.PositionBusinessLevels on p.BusinessLevelNbr equals b.BusinessLevelNbr
                        join j in _context.Jobs on p.JobId equals j.JobId
                        where ep.IsDeleted == false
                        select new PositionImportVM
                        {
                            E_PositionId = ep.E_PositionId,
                            Id = p.PositionId,
                            Title = p.Title,
                            jobcode = j.JobCode,
                            JobDescription = j.JobDescription,
                            code = p.Code,
                            PrimaryPosition = ep.PrimaryPosition,
                            BuCode = b.BusinessLevelCode,
                            BuTitle = b.BusinessLevelTitle,
                            JobTitle = j.title,
                            StartDate = ep.StartDate,
                            PositionId = ep.PositionId
                        }).ToList();
            return list;
        }

        public List<PositionImportVM> GetSalaryhistory(string strPositionCode, decimal? dPayRate)
        {
            var list = (from c in _context.E_PositionSalaryHistories
                        join ep in _context.E_Positions on c.E_PositionId equals ep.E_PositionId
                        join po in _context.Positions on c.E_PositionId equals po.PositionId
                        where po.Code == strPositionCode && c.PayRate == dPayRate
                        select new PositionImportVM { E_PositionId = c.E_PositionId }).ToList();

            return list;
        }

        public List<PositionImportVM> GetEffective(int nEPositionID, decimal? dPayRate)
        {
            var list = (from c in _context.E_PositionSalaryHistories
                        join ep in _context.E_Positions on c.E_PositionId equals ep.E_PositionId
                        join po in _context.Positions on c.E_PositionId equals po.PositionId
                        where c.E_PositionId == nEPositionID && c.PayRate == dPayRate
                        select new PositionImportVM { E_PositionId = c.E_PositionId }).ToList();

            return list;
        }
    }
}
