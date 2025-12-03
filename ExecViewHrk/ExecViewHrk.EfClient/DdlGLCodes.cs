using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace ExecViewHrk.EfClient
{
    public partial class DdlGLCodes
    {
            [Key]
            public int GLCodeId { get; set; }

            [Required]
            [StringLength(50)]
            public string Code { get; set;}

            [Required]
            [StringLength(50)]
            public string Description { get; set; }

            public bool Active { get; set; }
        }
    }
