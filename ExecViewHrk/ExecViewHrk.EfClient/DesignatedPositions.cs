using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExecViewHrk.EfClient
{
    public partial class DesignatedPositions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ManagerPersonId { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int E_PositionId { get; set; }
    }
}