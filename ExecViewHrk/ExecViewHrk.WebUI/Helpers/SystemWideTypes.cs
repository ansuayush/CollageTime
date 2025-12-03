namespace ExecViewHrk.Domain.Helper
{
    public enum MATCH_STATUS
    {
        NEW = 0,
        EMPLOYER_INTERESTED = 1,
        ME_INTERESTED = 2,
        EMPLOYER_ACCEPTED = 3,
        ME_ACCEPTED = 4,
        EMPLOYER_DENIED_ME = 5,
        ME_DENIED_EMPLOYER = 6

    };

    public static class ExecViewHrkConstants
    {
        public static decimal ExecviewVersionNumber
        {
            get
            {
                return System.Convert.ToDecimal("0.006");
            }
        }


    }

   
    public enum TimeCardsDisplay
    {
        TimeCard = 1,
        TimeCradInandOut = 2

    };
    public enum PayFrequency
    {
        Weekly,
        BiWeekly,
        SemiMonthly,
        Monthly,
    };

}