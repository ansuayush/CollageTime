using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Models
{
    public class TestVm
    {
        public int CompanyCodeId { get; set; }
        public string CompanyCodeDescription { get; set; }

        public short DepartmentId { get; set; }
        public string DepartmentDescription { get; set; }

        public int ProjectNumber { get; set; }
    }
}