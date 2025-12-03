using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExecViewHrk.Models
{
    public class DepartmentVm
    {
        [Required]
        public int DepartmentId { get; set; }

        public string CompCode_DeptCode_DeptDescription { get; set; }

        [Required]
        public int CompanyCodeId { get; set; }

        [Required]
        [StringLength(50)]
        public string DepartmentDescription { get; set; }

        [Required]
        [StringLength(10)]
        public string DepartmentCode { get; set; }

        public bool Active { get; set; }
        public string Comapnycodecode { get; set; }


        //for tempdept dropdown
        public int TempDeptId { get; set; }
        public string TempDeptCode { get; set; }
    }
}