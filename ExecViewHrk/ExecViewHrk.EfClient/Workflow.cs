namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Workflow
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Workflow()
        {
            FormTemplateWorkflows = new HashSet<FormTemplateWorkflow>();
            WorkflowMembers = new HashSet<WorkflowMember>();
        }

        public int WorkflowId { get; set; }

        [Required]
        [StringLength(500)]
        public string WorkflowName { get; set; }

        [StringLength(500)]
        public string WorkflowDescription { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormTemplateWorkflow> FormTemplateWorkflows { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkflowMember> WorkflowMembers { get; set; }
    }
}
