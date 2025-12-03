namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class FormTemplateField
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FormTemplateField()
        {
            FormWorkflowFieldPermissions = new HashSet<FormWorkflowFieldPermission>();
        }

        public int FormTemplateFieldId { get; set; }

        public int FormTemplateId { get; set; }

        [Required]
        [StringLength(50)]
        public string HtmlId { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; }

        [StringLength(50)]
        public string Value { get; set; }

        [StringLength(500)]
        public string Label { get; set; }

        public int? VisualWidth { get; set; }

        public decimal Position { get; set; }

        public bool Required { get; set; }

        public int ColumnNumber { get; set; }

        [StringLength(50)]
        public string SelectionGroup { get; set; }

        [StringLength(50)]
        public string CheckBoxRadioGroupName { get; set; }

        public virtual FormTemplate FormTemplate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormWorkflowFieldPermission> FormWorkflowFieldPermissions { get; set; }
    }
}
