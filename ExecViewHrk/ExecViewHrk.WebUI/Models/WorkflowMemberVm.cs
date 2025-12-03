using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ExecViewHrk.WebUI.Models
{
    public class WorkflowMemberVm
    {

        public int WorkflowMemberId { get; set; }

        public int WorkflowId { get; set; }

        public bool IsGroup { get; set; }

        [Required]
        public string UserOrGroupName { get; set; }

        [Required]
        [UIHint("DecimalSpin")]
        public Decimal Position { get; set; }
        
    }
}