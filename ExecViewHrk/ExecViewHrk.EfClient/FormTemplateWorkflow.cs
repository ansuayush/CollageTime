namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class FormTemplateWorkflow
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FormTemplateWorkflow()
        {
            FormWorkflowFieldPermissions = new HashSet<FormWorkflowFieldPermission>();
        }

        public int FormTemplateWorkflowId { get; set; }

        public int FormTemplateId { get; set; }

        public int WorkflowId { get; set; }

        [Required]
        [StringLength(500)]
        public string FormTemplateWorkflowName { get; set; }

        public virtual FormTemplate FormTemplate { get; set; }

        public virtual Workflow Workflow { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormWorkflowFieldPermission> FormWorkflowFieldPermissions { get; set; }
    }
}
