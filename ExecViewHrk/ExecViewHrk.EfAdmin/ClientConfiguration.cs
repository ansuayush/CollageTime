using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
namespace ExecViewHrk.EfAdmin
{
    [Table("ClientConfiguration")]
    public partial class ClientConfiguration
    {
        [Key]
        public int ClientConfigId { get; set; }

        public int EmployerId { get; set; }

        [StringLength(100)]
        public string ConfigurationName { get; set; }

        public int? ConfigurationValue { get; set; }

        public virtual Employer Employer { get; set; }


    }
}
