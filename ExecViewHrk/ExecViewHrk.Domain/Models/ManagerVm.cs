using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Models
{
    public class ManagerVm
    {
        public int ID { get; set; }
        public int ManagerId { get; set; }

        [Required]
        public string PersonName { get; set; }
        public int PositionID { get; set; }
        public int DepartmentID { get; set; }
        public string managerslockedlist { get; set; }
        public string managersnotlockedlist { get; set; }
        public string Departmentlockedlist { get; set; }
        public string Departmentnotlockedlist { get; set; }
        public List<DropDownModel> PersonsList { get; set; }
        public List<DropDownModel> PositionsList { get; set; }
        public List<DropDownModel> DepartmentList { get; set; }
    }
}