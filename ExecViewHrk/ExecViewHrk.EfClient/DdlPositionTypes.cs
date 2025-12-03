
namespace ExecViewHrk.EfClient
{
    using System.ComponentModel.DataAnnotations;
    public partial class DdlPositionTypes
    {
        [Key]
        public int PositionTypeId { get; set; }
        public string description { get; set; }
        public string code { get; set; }
        public bool active { get; set; }

    }
}
