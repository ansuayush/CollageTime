using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExecViewHrk.EfClient
{
    public class DesignatedManagerDepartment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ManagerPersonId { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ManagerDepartmentId { get; set; }
    }
}