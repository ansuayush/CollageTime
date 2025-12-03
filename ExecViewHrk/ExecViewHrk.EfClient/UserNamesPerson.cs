using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExecViewHrk.EfClient
{
    public  class UserNamesPerson
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(256)]
        public string UserName { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PersonID { get; set; }

        public DateTime? CreationDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [StringLength(50)]
        public string EnteredBy { get; set; }

        public virtual Person Person { get; set; }
    }
}
