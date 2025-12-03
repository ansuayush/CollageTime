using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.EfClient
{
    public partial class Treatylimithistories
    {
        [Key]
        public int ID { get; set; }
        public int EmployeeId { get; set; }
        public int CompanyCodeId { get; set; }
        public decimal? TreatyLimit { get; set; }
        public decimal? UsedAmount { get; set; }
        public decimal? RemainingAmount { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string EmployeeName { get; set; }
        public string CompanyCodeDescription { get; set; }

    }
}
