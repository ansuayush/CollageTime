using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Models;
using System.Data.Entity;
using ExecViewHrk.EfClient;

namespace ExecViewHrk.Domain.Repositories
{
    public class SalaryComponenRepository : RepositoryBase, ISalaryComponent
    {

        public List<SalaryComponentViewModel> getSalaryComponentList(int empId)
        {

            var salaryComponentList = _context.EmployeeSalaryComponents
                    .Include(x => x.DdlSalaryComponents.description).Where(x => x.employeeID == empId)
                    .Select(x => new SalaryComponentViewModel
                    {
                        employeeID = x.employeeID,
                        id = x.id,
                        amount = x.amount,
                        startDate = x.startDate,
                        expirationDate = x.expirationDate,
                        SalaryComponentTitle = x.DdlSalaryComponents.description
                    })
                    .ToList();
            return salaryComponentList;
        }
        public SalaryComponentViewModel GetSalaryComponentDetail(int id)
        {
            SalaryComponentViewModel salaryComponentViewModel = new SalaryComponentViewModel();
            if (id != 0)
            {
                salaryComponentViewModel = _context.EmployeeSalaryComponents
              .Include(x => x.DdlSalaryComponents.description).Where(x => x.id == id)
              .Select(x => new SalaryComponentViewModel
              {
                  employeeID = x.employeeID,
                  id = x.id,
                  amount = x.amount,
                  startDate = x.startDate,
                  expirationDate = x.expirationDate,
                  SalaryComponentTitle = x.DdlSalaryComponents.description,
                  salaryComponentTypeID = x.salaryComponentTypeID,
                  SalaryComponentID = x.SalaryComponentID,
                  total = x.total,
                  Base = x.Base,
                  benefits = x.benefits,
                  PayFrequencyId=x.PayFrequencyId
              })
              .FirstOrDefault();
            }
            else
            {
                salaryComponentViewModel.id = 0;
            }
            salaryComponentViewModel.SalaryComponentsList = _context.DdlSalaryComponents.ToList();
            salaryComponentViewModel.SalaryComponentsList.Insert(0, new ddlSalaryComponents { SalaryComponentID = 0, description = "Select" });
            salaryComponentViewModel.PayFrequencyList = _context.DdlPayFrequencies.ToList();
            salaryComponentViewModel.PayFrequencyList.Insert(0, new DdlPayFrequency { PayFrequencyId = 0, Description = "Select" });
            salaryComponentViewModel.PayTypeyList = _context.DdlPayTypes.ToList();
            salaryComponentViewModel.PayTypeyList.Insert(0, new ddlPayTypes { payTypeID = 0, description = "Select" });
            return salaryComponentViewModel;
        }
        public SalaryComponentViewModel SaveSalaryComponent(SalaryComponentViewModel salaryComponentViewModel)
        {
            employeeSalaryComponents EmployeeSalaryComponents = new  employeeSalaryComponents();
            employeeSalaryComponents EmpSalaryComponentsRecord = _context.EmployeeSalaryComponents.Where(x => x.id == salaryComponentViewModel.id).SingleOrDefault();
            if (EmpSalaryComponentsRecord == null)
            {
                EmpSalaryComponentsRecord = new employeeSalaryComponents();
                EmpSalaryComponentsRecord.employeeID = salaryComponentViewModel.employeeID;
                EmpSalaryComponentsRecord.id = salaryComponentViewModel.id;
                EmpSalaryComponentsRecord.amount = salaryComponentViewModel.amount;
                EmpSalaryComponentsRecord.startDate = salaryComponentViewModel.startDate;
                EmpSalaryComponentsRecord.expirationDate =salaryComponentViewModel.expirationDate;
                EmpSalaryComponentsRecord.payTypeID = salaryComponentViewModel.payTypeID;
                EmpSalaryComponentsRecord.SalaryComponentID = salaryComponentViewModel.SalaryComponentID;
                EmpSalaryComponentsRecord.salaryComponentTypeID = salaryComponentViewModel.salaryComponentTypeID;
                EmpSalaryComponentsRecord.enteredBy = salaryComponentViewModel.enteredBy;
                EmpSalaryComponentsRecord.Base = salaryComponentViewModel.Base == null ? false : salaryComponentViewModel.Base;
                EmpSalaryComponentsRecord.PayFrequencyId = salaryComponentViewModel.PayFrequencyId;
                EmpSalaryComponentsRecord.benefits = salaryComponentViewModel.benefits == null ? false : salaryComponentViewModel.benefits;
                EmpSalaryComponentsRecord.total = salaryComponentViewModel.total == null ? false : salaryComponentViewModel.total;
                EmpSalaryComponentsRecord.enteredDate = salaryComponentViewModel.startDate;
                _context.EmployeeSalaryComponents.Add(EmployeeSalaryComponents);
            }
            else
            {
                EmpSalaryComponentsRecord.employeeID = salaryComponentViewModel.employeeID;
                EmpSalaryComponentsRecord.id = salaryComponentViewModel.id;
                EmpSalaryComponentsRecord.amount = salaryComponentViewModel.amount;
                EmpSalaryComponentsRecord.startDate = salaryComponentViewModel.startDate;
                EmpSalaryComponentsRecord.expirationDate = salaryComponentViewModel.expirationDate;
                EmpSalaryComponentsRecord.payTypeID = salaryComponentViewModel.payTypeID;
                EmpSalaryComponentsRecord.SalaryComponentID = salaryComponentViewModel.SalaryComponentID;
                EmpSalaryComponentsRecord.salaryComponentTypeID = salaryComponentViewModel.salaryComponentTypeID;
                EmpSalaryComponentsRecord.enteredBy = salaryComponentViewModel.enteredBy;
                EmpSalaryComponentsRecord.Base = salaryComponentViewModel.Base;
                EmpSalaryComponentsRecord.benefits = salaryComponentViewModel.benefits;
                EmpSalaryComponentsRecord.total = salaryComponentViewModel.total;
                EmpSalaryComponentsRecord.PayFrequencyId = salaryComponentViewModel.PayFrequencyId;
            }
            _context.SaveChanges();
            if (EmpSalaryComponentsRecord == null)
            {
                salaryComponentViewModel.id = EmployeeSalaryComponents.id != 0 ? EmployeeSalaryComponents.id : salaryComponentViewModel.id;
            }

            return salaryComponentViewModel;
        }
        public void SalaryComponentsDelete(int id)
        {
            var dbRecord = _context.EmployeeSalaryComponents.Where(x => x.id ==id).FirstOrDefault();
            if (dbRecord != null)
            {
                _context.EmployeeSalaryComponents.Remove(dbRecord);
                _context.SaveChanges();
            }
        }
    }
}
