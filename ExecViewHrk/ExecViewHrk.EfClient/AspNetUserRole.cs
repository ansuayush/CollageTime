namespace ExecViewHrk.EfClient
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
   
    public class AspNetUserRole
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AspNetUserRole()
        {
            AspNetRoles = new HashSet<AspNetRole>();
            AspNetUsers = new HashSet<AspNetUser>();
        }

        [Key]
        [Required]
        [Column(Order = 1)]
        [StringLength(128)]
        public string UserId { get; set; }


        [Key]
        [Required]
        [Column(Order = 2)]
        [StringLength(128)]
        public string RoleId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AspNetRole> AspNetRoles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AspNetUser> AspNetUsers { get; set; }
    }
}
