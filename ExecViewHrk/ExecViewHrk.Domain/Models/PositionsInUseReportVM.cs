using System;
using System.Collections.Generic;

namespace ExecViewHrk.Models
{
    public class PositionsInUseReportVm
    {
        public string PositionCode { get; set; }

        public string PositionTitle { get; set; }

        public string PositionStatus { get; set; }
        public string JobCode { get; set; }
        public string JobTitle { get; set; }

        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string BusinessUnitCode { get; set; }
        public string BusinessUnitTitle { get; set; }
        public string hireDate { get; set; }
        public decimal Rate { get; set; }
        public string HoursPerWeek { get; set; }
        public string effectiveDate { get; set; }
        public string Notes { get; set; }
        public string BudgetAmount { get; set; }
        public string employeeStatus { get; set; }
        public string companyCode { get; set; }
        public string filenumber { get; set; }
        public DateTime? EPositionStartDate { get; set; }

        public List<DropDownModel> lstBudgetYears { get; set; }

        public int budgetYear { get; set; }

        public string RateTypeCode { get; set; }
        public decimal AnnualSalary { get; set; }

        public string Division { get; set; }
        public string EmployeeGLCode { get; set; }

        public string Code { get; set; }



    }
}