using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Helpers
{
    public class LengthOfEmployment
    {
        DateTime hDate;
        int hireYear;
        int hireMonth;
        int hireDay;
        int endYear;
        int endMonth;
        int endDay;

        public LengthOfEmployment(DateTime HireDate, DateTime EndDate)
        {
            hDate = HireDate;
            hireYear = HireDate.Year;
            hireMonth = HireDate.Month;
            hireDay = HireDate.Day;
            endYear = EndDate.Year;
            endMonth = EndDate.Month;
            endDay = EndDate.Day;
        }

        public String LengthOfEmploymentCalculation()
        {
            DateTime curDate;
            int curYear;
            int curMonth;
            int curDay ;
            int year;
            int month;
            int day;
            int increment = 0;
            int[] monthDay = new int[12] { 31, -1, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            string empLength = "";
           
            if (endYear == 1)
            {
                curDate = DateTime.Now.ToLocalTime();
                curYear = curDate.Year;
                curMonth = curDate.Month;
                curDay = curDate.Day;
            }
            else
            {
                curYear = endYear;
                curMonth = endMonth;
                curDay = endDay;
            }
            if (hireDay > curDay)
            {
                increment = monthDay[hireMonth - 1];
            }
            if (increment == -1)
            {
                if (DateTime.IsLeapYear(hireYear))
                {
                    increment = 29;
                }
                else
                {
                    increment = 28;
                }
            }
            if (increment != 0)
            {
                day = (curDay + increment) - hireDay;
                increment = 1;
            }
            else
            {
                day = curDay - hireDay;
            }
            if ((hireMonth + increment) > curMonth)
            {
                month = (curMonth + 12) - (hireMonth + increment);
                increment = 1;
            }
            else
            {
                month = curMonth - (hireMonth + increment);
                increment = 0;
            }
            year = curYear - (hireYear + increment);
            empLength = year + "Year(s), " + month + " month(s), " + day + " day(s)";
           
            return empLength;
        }

    }
}