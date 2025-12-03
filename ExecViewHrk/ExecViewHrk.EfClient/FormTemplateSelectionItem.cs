namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class FormTemplateSelectionItem
    {
        public int FormTemplateSelectionItemId { get; set; }

        public int FormTemplateSelectionGroupId { get; set; }

        [Required]
        [StringLength(50)]
        public string Text { get; set; }

        [Required]
        [StringLength(50)]
        public string Value { get; set; }

        public decimal Position { get; set; }

        public virtual FormTemplateSelectionGroup FormTemplateSelectionGroup { get; set; }
    }
}
