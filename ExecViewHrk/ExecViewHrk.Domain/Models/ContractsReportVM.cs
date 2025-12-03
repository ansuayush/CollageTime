namespace ExecViewHrk.Models
{
    public class ContractsReportVm
    {
        public string EmployeeName { get; set; }
        public string BusinessUnitTitle { get; set; }
        public string GLCode { get; set; }
        public string Semester { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public decimal Amount { get; set; }
        public string PayPeriods { get; set; }
        public decimal AmountToBePaid { get; set; }
        public decimal NewAmountToBePaid { get; set; }
        public decimal ExportedAmount { get; set; }
        public decimal RemainderDue { get; set; }
        public string PayPeriodsRemaining { get; set; }
        public string EarningCodes { get; set; }
        public string Notes { get; set; }

    }
}