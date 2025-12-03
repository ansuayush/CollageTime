namespace ExecViewHrk.Models
{
    public class TimeCardUnApprovedReportDM
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string FileNumber { get; set; }
        public short DepartmentId { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentDescription { get; set; }

        public double? UnApprovedHours { get; set; }
        public double? ApprovedHours { get; set; }
        public double? DepartmentTotalHours { get; set; }
        public double? PayPeriodHours { get; set; }

        //public double? Total { get; set; }
        //public bool IsApproved { get; set; }
        //public double? RegularHours { get; set; }
        //public double? CodedHours { get; set; }
        //public double? OverTime { get; set; }
        //public double? Emp_PayPeriodTotal { get; set; }
        //public bool Approved { get; set; }        
        //public string ManagerName { get; set; }
        //public override string ToString()
        //{
        //    return string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\""
        //                     , EmployeeName, RegularHours, CodedHours, OverTime, Emp_PayPeriodTotal, Approved);

        //}
    }
}