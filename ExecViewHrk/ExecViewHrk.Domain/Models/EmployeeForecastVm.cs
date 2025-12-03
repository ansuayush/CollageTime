using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExecViewHrk.EfClient;

namespace ExecViewHrk.EfClient
{
    public class EmployeeForecastVm
    {

        //public EmployeeForecastVm()
        //{
        //    personbasicVm = new List<Person>();
        //}

       // public List<Person> personbasicVm { get; set; }

            public int? ePosSalHistoryId { get; set; }

        public string Lastname { get; set; }
        public string Firstname { get; set; }
       // public List<Employee> empentityVm { get; set; }

        public string CompanyCode { get; set; }
        public int FileNumber { get; set; }
       // public List<DdlEmploymentStatus> ddlempstatusVm { get; set; }

        public string Code { get; set; }
        public bool Active { get; set; } //status


       // public List<Positions> positionVm {get ;set;}
        public string Positiontitle { get; set; }  //positiontitle
        public List<E_Positions> epositionVm { get; set; }
        public bool? PrimaryPosition { get; set; }
       
        public DateTime? StartDate { get; set; }

        

        public int? RateTypeId{ get; set; }
        
        public DateTime? EndDate { get; set; }
        public int? PayFrequencyId { get; set; }

        public decimal? Wages { get; set; }
        public decimal? HoursPerPayPeriod { get; set; }

        public int? NumberOfDays { get; set; }

        public decimal? IncreasePercent { get; set; }
        public int? NewRate { get; set; }
        public int? AnnualSalary { get; set; }
        public int? NewAnnualSalary { get; set; }

        
    }
}
