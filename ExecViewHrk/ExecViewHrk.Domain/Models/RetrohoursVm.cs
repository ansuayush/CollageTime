using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Models
{
    public class RetrohoursVm
    {
       public  decimal? Retrohours { get; set; }
        public DateTime? RetroHoursDate { get; set; }
        public int CompanyCodeId { get; set; }
        public string CompanyCodeDescription { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeFullName { get; set; }
        public int PayPeriodId { get; set; }
        public string PayPeriod { get; set; }
        public int PositionId { get; set; }

        public int E_PositionId { get; set; }
        public string PositionCode { get; set; }
        public int Id { get; set; }

        public int HoursCodeId { get; set; }

        public string HoursCodeCode { get; set; }

        public List<E_PositioVm> PositionList { get; set; }
        public DateTime? StartDate { get; set; }        
        public DateTime? actualEndDate { get; set; }

        public string PositionDescription { get; set; }

        public bool isedit { get; set; }
        
        public string HoursCodedesc { get; set; }

        public string PayperiodDate { get; set; }
        public string strRatroHourDate { get; set; }
        public string Username { get; set; }

        public int PersonId { get; set; }

        public List<DropDownModel> HourCodeList { get; set; }

    }
}
