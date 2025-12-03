namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class FormWorkflowFieldPermission
    {
        public int FormWorkflowFieldPermissionId { get; set; }

        public int FormTemplateWorkflowId { get; set; }

        public int FormTemplateFieldId { get; set; }

        public int WorlflowMemberId { get; set; }

        public bool CanView { get; set; }

        public bool CanEdit { get; set; }

        public virtual FormTemplateField FormTemplateField { get; set; }

        public virtual FormTemplateWorkflow FormTemplateWorkflow { get; set; }

        public virtual WorkflowMember WorkflowMember { get; set; }
    }
}
