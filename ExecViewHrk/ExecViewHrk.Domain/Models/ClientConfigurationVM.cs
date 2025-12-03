using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ExecViewHrk.Models
{
   public class ClientConfigurationVM
    {
        public int ClientConfigId { get; set; }

        public int EmployerId { get; set; }
        
        public string ConfigurationName { get; set; }
        
        public int? ConfigurationValue { get; set; }

        public string EmployerName { get; set; }

        public int? FiscalYear { get; set; }

        public int? FiscalMonth { get; set; }

        public string FiscalMonthText { get; set; }
        public List<DropDownModel> FiscalYearList { get; set; }
        public List<DropDownModel> FiscalMonthList { get; set; }
    }
}
