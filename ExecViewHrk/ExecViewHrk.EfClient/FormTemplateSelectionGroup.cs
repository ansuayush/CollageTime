namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class FormTemplateSelectionGroup
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FormTemplateSelectionGroup()
        {
            FormTemplateSelectionItems = new HashSet<FormTemplateSelectionItem>();
        }

        public int FormTemplateSelectionGroupId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Description { get; set; }

        [StringLength(50)]
        public string ExecViewTable { get; set; }

        [StringLength(50)]
        public string ExecViewTextColumn { get; set; }

        [StringLength(50)]
        public string ExecViewValueColumn { get; set; }

        public string ExecViewSql { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormTemplateSelectionItem> FormTemplateSelectionItems { get; set; }
    }
}
