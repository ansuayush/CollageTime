using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ExecViewHrk.WebUI.Models
{
    public class FormWorkflowFieldPermissionVm
    {

        public int FormWorkflowFieldPermissionId { get; set; }

        public int FormTemplateWorkflowId { get; set; }
        public int FormTemplateFieldId { get; set; }
        public int WorkflowMemberId { get; set; }

        public string FieldName { get; set; }
        public string WorkflowMemberName { get; set; }

        public bool CanView { get; set; }
        public bool CanEdit { get; set; }

        
    }
}