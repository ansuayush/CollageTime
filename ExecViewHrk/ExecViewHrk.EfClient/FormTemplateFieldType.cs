namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class FormTemplateFieldType
    {
        public int FormTemplateFieldTypeId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
