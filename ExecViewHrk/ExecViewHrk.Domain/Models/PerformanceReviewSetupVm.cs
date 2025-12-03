using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Models
{
    public class PerformanceReviewSetupVm
    {
        public int ID { get; set; }
        public string SectionName { get; set; }
        public string Header { get; set; }
        public int ProfileID { get; set; }
        public int NumRows { get; set; }
        public int MaxCharacters { get; set; }
        public decimal Weight { get; set; }
        public int Position { get; set; }
        public string EnteredBy { get; set; }
        public DateTime? EnteredDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int PerProfileId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public List<DropDownModel> PerformanceProfileList { get; set; }
    }
}
