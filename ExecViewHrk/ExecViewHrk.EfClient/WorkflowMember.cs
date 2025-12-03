namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class WorkflowMember
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WorkflowMember()
        {
            FormWorkflowFieldPermissions = new HashSet<FormWorkflowFieldPermission>();
        }

        public int WorkflowMemberId { get; set; }

        public int WorkflowId { get; set; }

        public bool IsGroup { get; set; }

        [Required]
        [StringLength(100)]
        public string UserOrGroupName { get; set; }

        public decimal Position { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormWorkflowFieldPermission> FormWorkflowFieldPermissions { get; set; }

        public virtual Workflow Workflow { get; set; }
    }
}
