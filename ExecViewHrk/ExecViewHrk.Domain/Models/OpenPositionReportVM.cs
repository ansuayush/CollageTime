namespace ExecViewHrk.Models
{
    public class OpenPositionReportVm
    {
        public string PositionCode { get; set; }
        public string PositionTitle { get; set; }
        public int TotalSlots { get; set; }
        public int SlotsFilled { get; set; }
        public int SlotsLeft { get; set; }
        public string Code { get; set; }

    }
}