namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PersonImage
    {
        public int PersonImageId { get; set; }

        public int PersonId { get; set; }

        [Required]
        public byte[] PersonImageData { get; set; }

        [Required]
        [StringLength(50)]
        public string PersonImageMimeType { get; set; }

        public virtual Person Person { get; set; }
    }
}
