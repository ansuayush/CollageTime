namespace ExecViewHrk.WebUI.Helpers
{
    public class ServiceApiSubModules
    {
        private ServiceApiSubModules()
        { }

        public static string GetEmployeeCurrentPayPeriod
        {
            get { return "getemployeecurrentpayperiod"; }
        }

        public static string GetTimeCardWeekTotalList
        {
            get { return "gettimecardweektotallist"; }
        }

        public static string GetEmployeeTimeCardByDate
        {
            get { return "getemployeetimecardbydate"; }
        }

        public static string GetEPositionList
        {
            get { return "getepositionlist"; }
        }

        public static string InsertEmployeePositionPunch
        {
            get { return "insertemployeepositionpunch"; }
        }

        public static string GetPayPeriodsList
        {
            get { return "getpayperiodlist"; }
        }

        public static string GetEmployeeTimeCardsByPayPeriod
        {
            get { return "getemployeetimecardsbypayperiod"; }
        }
    }
}