using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Globalization;
using ExecViewHrk.SqlData.Models;
using System.Data;



namespace ExecViewHrk.SqlData
{
    public class SqlTimeCards
    {            

        //Load Employee weekly Timecard values based on selected pay period

        public SqlTimeCards() { }
       
        string _connectionString;
        public SqlTimeCards(string connStr)
        {
            _connectionString = connStr;
        }

        //Biweekly Timecard
        public Collection<TimeCardCollection> LoadEmployeeWeeklyTimeCardPP(int empId, int payPeriodId, bool isArchived)
        {
            var employeeWeeklyTimeCardList = new Collection<TimeCardCollection>();

            using (SqlConnection sqlConn = new SqlConnection(_connectionString))
            {
                using(SqlCommand cmd = new SqlCommand())
                    {
                        try
                        {
                            cmd.CommandText = (@"select t.TimeCardId, t.CompanyCodeId, t.EmployeeId, t.ActualDate, t.ProjectNumber, t.DailyHours, t.HoursCodeId, t.Hours,
                                    t.EarningsCodeId, t.EarningsAmount, t.TempDeptId, t.TempJobId, P.PayPeriodId, 
                                    case when t.ActualDate >= p.StartDate and t.ActualDate <= DateAdd(day,6,p.StartDate) Then 1
                                         when t.ActualDate > DateAdd(day,6,p.StartDate) and t.ActualDate <= p.EndDate Then 2
                                    End as WeekNum ,  (t.DailyHours +  t.Hours) as LineTotal, IsApproved as IsLineApproved
                                    from TimeCards t, PayPeriods p 
                                    where t.CompanyCodeId = p.CompanyCodeId and t.ActualDate between p.StartDate AND p.EndDate And t.EmployeeId = @empId
                                     And p.PayPeriodId = @payPeriodId and p.IsArchived = @isArchived Order by ActualDate, WeekNum");

                                cmd.Parameters.Add("@empId", SqlDbType.Int);
                                cmd.Parameters["@empId"].Value = empId;

                                cmd.Parameters.Add("@payPeriodId", SqlDbType.Int);
                                cmd.Parameters["@payPeriodId"].Value = payPeriodId;

                                cmd.Parameters.Add("@isArchived", SqlDbType.Bit);
                                cmd.Parameters["@isArchived"].Value = isArchived;

                                cmd.CommandType = System.Data.CommandType.Text;
                                cmd.Connection = sqlConn;
                                sqlConn.Open();

                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    if (reader != null && reader.HasRows)
                                    {                                       
                                        while (reader.Read())
                                        {
                                            TimeCardCollection e_timecardRecord = new TimeCardCollection();
                                            e_timecardRecord.TimeCardId = (reader["TimeCardId"] as int?) ?? default(int);
                                            e_timecardRecord.CompanyCodeId = (reader["CompanyCodeId"] as Int16?) ?? default(Int16);
                                            e_timecardRecord.EmployeeId = (reader["EmployeeId"] as int?) ?? default(int);
                                            e_timecardRecord.ActualDate = (reader["ActualDate"] as DateTime?) ?? default(DateTime);
                                            e_timecardRecord.Day = (reader["ActualDate"] as DateTime?) ?? default(DateTime);
                                            e_timecardRecord.ProjectNumber = (reader["ProjectNumber"] as int?) ?? default(int);
                                            e_timecardRecord.DailyHours = (reader["DailyHours"] as Double?) ?? default(Double);
                                            e_timecardRecord.HoursCodeId = (reader["HoursCodeId"] as int?) ?? default(int);
                                            e_timecardRecord.Hours = (reader["Hours"] as Double?) ?? default(Double);
                                            e_timecardRecord.EarningsCodeId = (reader["EarningsCodeId"] as Int16?) ?? default(Int16);
                                            e_timecardRecord.EarningsAmount = (reader["EarningsAmount"] as Double?) ?? default(Double);
                                            e_timecardRecord.TempDeptId = (reader["TempDeptId"] as Int16?) ?? default(Int16);
                                            e_timecardRecord.TempJobId = (reader["TempJobId"] as Int32?) ?? default(Int32);
                                            e_timecardRecord.WeekNum = (reader["WeekNum"] as Int32?) ?? default(Int32);
                                            e_timecardRecord.LineTotal = (reader["LineTotal"] as Double?) ?? default(Double);
                                            e_timecardRecord.IsLineApproved = (reader["IsLineApproved"] as bool?) ?? default(bool);
                                            e_timecardRecord.ShowLineApprovedActive = false;

                                            employeeWeeklyTimeCardList.Add(e_timecardRecord);
                                        }
                                    }
                                }                               
                        }
                        catch(Exception e)
                        {
                            throw e;
                        }
                    }                        
             } 
            return employeeWeeklyTimeCardList;
        }


        //Bi-Weekly Total hours (Daily Hours Timecard)
        public Collection<TimeCardWeekTotalCollection> LoadEmployeeWeeklyTotalTimeCardPP(int empId, int payPeriodId, bool isArchived)
        {
            var employeeWeeklyTotalTimeCardInOutList = new Collection<TimeCardWeekTotalCollection>();

            using (SqlConnection sqlConn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandText = (@"select WeekNum, WeekTotalRegularHour = case when (WeekRegularHour + WeekTotalCodedHour) > 40 Then
                            WeekRegularHour- ((WeekRegularHour + WeekTotalCodedHour)-40) else WeekRegularHour end,
                            WeekTotalOverTime = case when (WeekRegularHour + WeekTotalCodedHour) > 40 Then
                            ((WeekRegularHour + WeekTotalCodedHour)-40) else 0 end, WeekTotalCodedHour  from
                            (select  
                            case when t.ActualDate >= p.StartDate and t.ActualDate <= DateAdd(day,6,p.StartDate) Then 1
                                    when t.ActualDate > DateAdd(day,6,p.StartDate) and t.ActualDate <= p.EndDate Then 2
                            End as WeekNum ,  sum(t.DailyHours) as WeekRegularHour, sum(Hours) as WeekTotalCodedHour
                            from TimeCards t, PayPeriods p 
                            where t.CompanyCodeId = p.CompanyCodeId and t.ActualDate between p.StartDate AND p.EndDate And t.EmployeeId = @empId 
                            And p.PayPeriodId = @payPeriodId and p.IsArchived = @isArchived
                            group by case when t.ActualDate >= p.StartDate and t.ActualDate <= DateAdd(day,6,p.StartDate) Then 1
                            when t.ActualDate > DateAdd(day,6,p.StartDate) and t.ActualDate <= p.EndDate Then 2 end) temp");

                        cmd.Parameters.Add("@empId", SqlDbType.Int);
                        cmd.Parameters["@empId"].Value = empId;

                        cmd.Parameters.Add("@payPeriodId", SqlDbType.Int);
                        cmd.Parameters["@payPeriodId"].Value = payPeriodId;

                        cmd.Parameters.Add("@isArchived", SqlDbType.Bit);
                        cmd.Parameters["@isArchived"].Value = isArchived;

                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.Connection = sqlConn;
                        sqlConn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader != null && reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    TimeCardWeekTotalCollection e_timecardRecord = new TimeCardWeekTotalCollection();
                                    e_timecardRecord.WeekNum = (reader["WeekNum"] as int?) ?? default(int);
                                    e_timecardRecord.RegularHours = (reader["WeekTotalRegularHour"] as Double?) ?? default(Double);
                                    e_timecardRecord.OverTime = (reader["WeekTotalOverTime"] as Double?) ?? default(Double);
                                    e_timecardRecord.CodedHours = (reader["WeekTotalCodedHour"] as Double?) ?? default(Double);
                                    e_timecardRecord.WeeklyTotal = e_timecardRecord.RegularHours + e_timecardRecord.CodedHours + e_timecardRecord.OverTime;

                                    employeeWeeklyTotalTimeCardInOutList.Add(e_timecardRecord);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
            return employeeWeeklyTotalTimeCardInOutList;
        }


        //Load Timecard In and Out Employee Records according to the pay period (Biweekly)
        public Collection<TimeCardInOutCollection> LoadEmployeeWeeklyTimeCardInOutPP(int empId, int payPeriodId, bool isArchived) // DateTime payPeriodStartDate, DateTime payPeriodEndDate)  
        {
            var employeeWeeklyTimeCardInOutList = new Collection<TimeCardInOutCollection>();

            using (SqlConnection sqlConn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandText = (@"select t.TimeCardId, t.CompanyCodeId, t.EmployeeId, t.ActualDate, t.ProjectNumber, t.HoursCodeId, t.Hours,
                                    t.EarningsCodeId, t.EarningsAmount, t.TempDeptId, t.TempJobId, t.TimeIn, t.TimeOut, LunchOut, LunchBack, 
                                    case when t.ActualDate >= p.StartDate and t.ActualDate <= DateAdd(day,6,p.StartDate) Then 1
                                         when t.ActualDate > DateAdd(day,6,p.StartDate) and t.ActualDate <= p.EndDate Then 2
                                    End as WeekNum ,  IsApproved as IsLineApproved,
                                    Case When (TimeIn IS NOT NULL) and (TimeOut IS NOT NULL) and (LunchOut IS NOT NULL) and (LunchBack IS NOT NULL) Then
                                            Convert( Varchar(5),(DATEDIFF(MINUTE, TimeIn, TimeOut)-DATEDIFF(MINUTE, LunchOut, LunchBack))/60) + '.' + 
                                            RIGHT('0' + CAST((DATEDIFF(MINUTE, TimeIn, TimeOut)-DATEDIFF(MINUTE, LunchOut, LunchBack))%60 AS varchar(2)),2)
                                        When (TimeIn IS NOT NULL) and (TimeOut IS NOT NULL) and (LunchOut IS NULL) and (LunchBack IS NULL) Then
                                            Convert( Varchar(5),DATEDIFF(MINUTE, TimeIn, TimeOut)/60) + '.'+
                                            RIGHT('0' + CAST(DATEDIFF(MINUTE, TimeIn, TimeOut) % 60 AS varchar(2)),2)
                                        Else NULL
                                        End as LineTotal
                                    from TimeCards t, PayPeriods p 
                                    where t.CompanyCodeId = p.CompanyCodeId and t.ActualDate between p.StartDate AND p.EndDate And t.EmployeeId = @empId
                                    and p.PayPeriodId= @payPeriodId and p.IsArchived = @isArchived Order by ActualDate, WeekNum");
                                   //" and p.StartDate= '" + payPeriodStartDate.Date + "' and p.EndDate= '" + payPeriodEndDate.Date + "' Order by ActualDate, WeekNum");

                        cmd.Parameters.Add("@empId", SqlDbType.Int);
                        cmd.Parameters["@empId"].Value = empId;

                        cmd.Parameters.Add("@payPeriodId", SqlDbType.Int);
                        cmd.Parameters["@payPeriodId"].Value = payPeriodId;

                        cmd.Parameters.Add("@isArchived", SqlDbType.Bit);
                        cmd.Parameters["@isArchived"].Value = isArchived;

                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.Connection = sqlConn;
                        sqlConn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader != null && reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    TimeCardInOutCollection e_timecardRecord = new TimeCardInOutCollection();
                                    e_timecardRecord.TimeCardId = (reader["TimeCardId"] as int?) ?? default(int);
                                    e_timecardRecord.CompanyCodeId = (reader["CompanyCodeId"] as Int16?) ?? default(Int16);
                                    e_timecardRecord.EmployeeId = (reader["EmployeeId"] as int?) ?? default(int);
                                    e_timecardRecord.ActualDate = (reader["ActualDate"] as DateTime?) ?? default(DateTime);
                                    e_timecardRecord.Day = (reader["ActualDate"] as DateTime?) ?? default(DateTime);
                                    e_timecardRecord.ProjectNumber = (reader["ProjectNumber"] as int?) ?? default(int);
                                    e_timecardRecord.TimeIn = (reader["TimeIn"] == DBNull.Value) ? (DateTime?)null : ((DateTime)reader["TimeIn"]);
                                    e_timecardRecord.TimeOut = (reader["TimeOut"] == DBNull.Value) ? (DateTime?)null : ((DateTime)reader["TimeOut"]);
                                    //e_timecardRecord.LunchOut = (reader["LunchOut"] as DateTime?) ?? default(DateTime);
                                    e_timecardRecord.LunchOut = (reader["LunchOut"] == DBNull.Value) ? (DateTime?)null : ((DateTime)reader["LunchOut"]);
                                    e_timecardRecord.LunchBack = (reader["LunchBack"] == DBNull.Value) ? (DateTime?)null : ((DateTime)reader["LunchBack"]);
                                    e_timecardRecord.HoursCodeId = (reader["HoursCodeId"] as int?) ?? default(int);
                                    e_timecardRecord.Hours = (reader["Hours"] as Double?) ?? default(Double);
                                    e_timecardRecord.EarningsCodeId = (reader["EarningsCodeId"] as Int16?) ?? default(Int16);
                                    e_timecardRecord.EarningsAmount = (reader["EarningsAmount"] as Double?) ?? default(Double);
                                    e_timecardRecord.TempDeptId = (reader["TempDeptId"] as Int16?) ?? default(Int16);
                                    e_timecardRecord.TempJobId = (reader["TempJobId"] as Int32?) ?? default(Int32);
                                    e_timecardRecord.WeekNum = (reader["WeekNum"] as Int32?) ?? default(Int32);
                                    e_timecardRecord.LineTotal = (reader["LineTotal"] == DBNull.Value) ? (String)null : ((String)reader["LineTotal"]);
                                    e_timecardRecord.IsLineApproved = (reader["IsLineApproved"] as bool?) ?? default(bool);
                                    e_timecardRecord.ShowLineApprovedActive = false;

                                    employeeWeeklyTimeCardInOutList.Add(e_timecardRecord);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
            return employeeWeeklyTimeCardInOutList;
        }

        //Time Card In Out weekly totals (Biweekly)
        public Collection<TimeCardWeekTotalCollection> LoadEmployeeWeeklyTotalTimeCardInOutPP(int empId, int payPeriodId, bool isArchived)   
        {
            var employeeWeeklyTotalTimeCardInOutList = new Collection<TimeCardWeekTotalCollection>();

            using (SqlConnection sqlConn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandText = (@"select WeekNum, WeekTotalRegularHour = case when (WeekRegularHour + WeekTotalCodedHour) > 40 Then
                                WeekRegularHour- ((WeekRegularHour + WeekTotalCodedHour)-40) else WeekRegularHour end,
                                WeekTotalOverTime = case when (WeekRegularHour + WeekTotalCodedHour) > 40 Then
                                ((WeekRegularHour + WeekTotalCodedHour)-40) else 0 end, WeekTotalCodedHour 
                                from
                                (select case when t.ActualDate >= p.StartDate and t.ActualDate <= DateAdd(day,6,p.StartDate) Then 1
                                when t.ActualDate > DateAdd(day,6,p.StartDate) and t.ActualDate <= p.EndDate Then 2 End as WeekNum,
                                Sum(cast(Case When (TimeIn IS NOT NULL) and (TimeOut IS NOT NULL) and (LunchOut IS NOT NULL) and (LunchBack IS NOT NULL) Then
                                Convert( Varchar(5),(DATEDIFF(MINUTE, TimeIn, TimeOut)-DATEDIFF(MINUTE, LunchOut, LunchBack))/60) + '.' + 
                                RIGHT('0' + CAST((DATEDIFF(MINUTE, TimeIn, TimeOut)-DATEDIFF(MINUTE, LunchOut, LunchBack))%60 AS varchar(2)),2)
                                When (TimeIn IS NOT NULL) and (TimeOut IS NOT NULL) and (LunchOut IS NULL) and (LunchBack IS NULL) Then
                                Convert( Varchar(5),DATEDIFF(MINUTE, TimeIn, TimeOut)/60) + '.'+ RIGHT('0' + CAST(DATEDIFF(MINUTE, TimeIn, TimeOut) % 60 AS varchar(2)),2)
                                Else NULL End as Decimal(6,2))) as WeekRegularHour, sum(hours) as WeekTotalCodedHour from TimeCards t, PayPeriods p
                                where t.EmployeeId = @empId AND t.CompanyCodeId = p.CompanyCodeId and t.ActualDate between p.StartDate AND p.EndDate
                                and p.PayPeriodId= @payPeriodId and p.IsArchived = @isArchived group by case when t.ActualDate >= p.StartDate and t.ActualDate <= DateAdd(day,6,p.StartDate) Then 1
                                when t.ActualDate > DateAdd(day,6,p.StartDate) and t.ActualDate <= p.EndDate Then 2 End) temp");
                    
                        cmd.Parameters.Add("@empId", SqlDbType.Int);
                        cmd.Parameters["@empId"].Value = empId;

                        cmd.Parameters.Add("@payPeriodId", SqlDbType.Int);
                        cmd.Parameters["@payPeriodId"].Value = payPeriodId;

                        cmd.Parameters.Add("@isArchived", SqlDbType.Bit);
                        cmd.Parameters["@isArchived"].Value = isArchived;

                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.Connection = sqlConn;
                        sqlConn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader != null && reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    TimeCardWeekTotalCollection e_timecardRecord = new TimeCardWeekTotalCollection();
                                    e_timecardRecord.WeekNum = (reader["WeekNum"] as int?) ?? default(int);
                                    e_timecardRecord.RegularHours = (reader["WeekTotalRegularHour"] as Double?) ?? default(Double);
                                    e_timecardRecord.OverTime = (reader["WeekTotalOverTime"] as Double?) ?? default(Double);
                                    e_timecardRecord.CodedHours = (reader["WeekTotalCodedHour"] as Double?) ?? default(Double);
                                    e_timecardRecord.WeeklyTotal = e_timecardRecord.RegularHours + e_timecardRecord.CodedHours + e_timecardRecord.OverTime;

                                    employeeWeeklyTotalTimeCardInOutList.Add(e_timecardRecord);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
            return employeeWeeklyTotalTimeCardInOutList;
        }


        //Semi-Monthly Timecard DailyHours
        public Collection<TimeCardSemiMonthlyCollection> LoadEmployeeSemiMonthlyTimeCardPP(int empId, int payPeriodId, bool isArchived)
        {
            var employeeSemiMonthlyTimeCardList = new Collection<TimeCardSemiMonthlyCollection>();

            using (SqlConnection sqlConn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandText = (@"select t.TimeCardId, t.CompanyCodeId, t.EmployeeId, t.ActualDate, t.ProjectNumber, t.DailyHours, t.HoursCodeId, t.Hours,
                                    t.EarningsCodeId, t.EarningsAmount, t.TempDeptId, t.TempJobId, P.PayPeriodId, 
                                    (t.DailyHours +  t.Hours) as LineTotal, IsApproved as IsLineApproved
                                    from TimeCards t, PayPeriods p 
                                    where t.CompanyCodeId = p.CompanyCodeId and t.ActualDate between p.StartDate AND p.EndDate And t.EmployeeId = @empId
                                    And p.PayPeriodId = @payPeriodId and p.IsArchived = @isArchived Order by ActualDate");

                        cmd.Parameters.Add("@empId", SqlDbType.Int);
                        cmd.Parameters["@empId"].Value = empId;

                        cmd.Parameters.Add("@payPeriodId", SqlDbType.Int);
                        cmd.Parameters["@payPeriodId"].Value = payPeriodId;

                        cmd.Parameters.Add("@isArchived", SqlDbType.Bit);
                        cmd.Parameters["@isArchived"].Value = isArchived;

                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.Connection = sqlConn;
                        sqlConn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader != null && reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    TimeCardSemiMonthlyCollection e_timecardRecord = new TimeCardSemiMonthlyCollection();
                                    e_timecardRecord.TimeCardId = (reader["TimeCardId"] as int?) ?? default(int);
                                    e_timecardRecord.CompanyCodeId = (reader["CompanyCodeId"] as Int16?) ?? default(Int16);
                                    e_timecardRecord.EmployeeId = (reader["EmployeeId"] as int?) ?? default(int);
                                    e_timecardRecord.ActualDate = (reader["ActualDate"] as DateTime?) ?? default(DateTime);
                                    e_timecardRecord.Day = (reader["ActualDate"] as DateTime?) ?? default(DateTime);
                                    e_timecardRecord.ProjectNumber = (reader["ProjectNumber"] as int?) ?? default(int);
                                    e_timecardRecord.DailyHours = (reader["DailyHours"] as Double?) ?? default(Double);
                                    e_timecardRecord.HoursCodeId = (reader["HoursCodeId"] as int?) ?? default(int);
                                    e_timecardRecord.Hours = (reader["Hours"] as Double?) ?? default(Double);
                                    e_timecardRecord.EarningsCodeId = (reader["EarningsCodeId"] as Int16?) ?? default(Int16);
                                    e_timecardRecord.EarningsAmount = (reader["EarningsAmount"] as Double?) ?? default(Double);
                                    e_timecardRecord.TempDeptId = (reader["TempDeptId"] as Int16?) ?? default(Int16);
                                    e_timecardRecord.TempJobId = (reader["TempJobId"] as Int32?) ?? default(Int32);
                                    //e_timecardRecord.WeekNum = (reader["WeekNum"] as Int32?) ?? default(Int32);
                                    e_timecardRecord.LineTotal = (reader["LineTotal"] as Double?) ?? default(Double);
                                    e_timecardRecord.IsLineApproved = (reader["IsLineApproved"] as bool?) ?? default(bool);
                                    e_timecardRecord.ShowLineApprovedActive = false;

                                    employeeSemiMonthlyTimeCardList.Add(e_timecardRecord);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
            return employeeSemiMonthlyTimeCardList;
        }

        //Semi-Monthly Total hours (Daily Hours Timecard)
        public Collection<TimeCardWeekTotalCollection> LoadEmployeePayPeriodTotal_TimeCard(int empId, int payPeriodId, bool isArchived)
        {
            var employeePayPeriodTotalTimeCardList = new Collection<TimeCardWeekTotalCollection>();

            using (SqlConnection sqlConn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandText = (@"select TotalRegularHours = case when (sum(t.DailyHours) + sum(Hours)) > 40 Then
                            sum(t.DailyHours)- ((sum(t.DailyHours) + sum(Hours))-40) else sum(t.DailyHours) end,
                            TotalOverTime = case when (sum(t.DailyHours) + sum(Hours)) > 40 Then
                            ((sum(t.DailyHours) + sum(Hours))-40) else 0 end, sum(Hours) as TotalCodedHours 
                            from TimeCards t, PayPeriods p 
                            where t.CompanyCodeId = p.CompanyCodeId and t.ActualDate between p.StartDate AND p.EndDate And t.EmployeeId = @empId
                            And p.PayPeriodId = @payPeriodId and p.IsArchived = @isArchived");

                        cmd.Parameters.Add("@empId", SqlDbType.Int);
                        cmd.Parameters["@empId"].Value = empId;

                        cmd.Parameters.Add("@payPeriodId", SqlDbType.Int);
                        cmd.Parameters["@payPeriodId"].Value = payPeriodId;

                        cmd.Parameters.Add("@isArchived", SqlDbType.Bit);
                        cmd.Parameters["@isArchived"].Value = isArchived;

                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.Connection = sqlConn;
                        sqlConn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader != null && reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    TimeCardWeekTotalCollection e_timecardRecord = new TimeCardWeekTotalCollection();
                                    e_timecardRecord.RegularHours = (reader["TotalRegularHours"] as Double?) ?? default(Double);
                                    e_timecardRecord.OverTime = (reader["TotalOverTime"] as Double?) ?? default(Double);
                                    e_timecardRecord.CodedHours = (reader["TotalCodedHours"] as Double?) ?? default(Double);
                                    //semi-monthly payperiod total
                                    e_timecardRecord.WeeklyTotal = e_timecardRecord.RegularHours + e_timecardRecord.CodedHours + e_timecardRecord.OverTime; 

                                    employeePayPeriodTotalTimeCardList.Add(e_timecardRecord);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
            return employeePayPeriodTotalTimeCardList;
        }


        //TimeCardIn-Out pay-period totals 
        public Collection<TimeCardWeekTotalCollection> LoadEmployeePayPeriodTotal_TimeCardInOut(int empId, int payPeriodId, bool isArchived)
        {
            var employeePPTotalTimeCardInOutList = new Collection<TimeCardWeekTotalCollection>();

            using (SqlConnection sqlConn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandText = (@"select  TotalRegularHours = case when (PPRegularHour + TotalCodedHours) > 40 Then
                                PPRegularHour- ((PPRegularHour + TotalCodedHours)-40) else PPRegularHour end,
                                TotalOverTime = case when (PPRegularHour + TotalCodedHours) > 40 Then
                                ((PPRegularHour + TotalCodedHours)-40) else 0 end, TotalCodedHours 
                                from
                                (select Sum(cast(Case When (TimeIn IS NOT NULL) and (TimeOut IS NOT NULL) and (LunchOut IS NOT NULL) and (LunchBack IS NOT NULL) Then
                                Convert( Varchar(5),(DATEDIFF(MINUTE, TimeIn, TimeOut)-DATEDIFF(MINUTE, LunchOut, LunchBack))/60) + '.' + 
                                RIGHT('0' + CAST((DATEDIFF(MINUTE, TimeIn, TimeOut)-DATEDIFF(MINUTE, LunchOut, LunchBack))%60 AS varchar(2)),2)
                                When (TimeIn IS NOT NULL) and (TimeOut IS NOT NULL) and (LunchOut IS NULL) and (LunchBack IS NULL) Then
                                Convert( Varchar(5),DATEDIFF(MINUTE, TimeIn, TimeOut)/60) + '.'+ RIGHT('0' + CAST(DATEDIFF(MINUTE, TimeIn, TimeOut) % 60 AS varchar(2)),2)
                                Else NULL End as Decimal(6,2))) as PPRegularHour, sum(hours) as TotalCodedHours from TimeCards t, PayPeriods p
                                where t.EmployeeId = @empId AND t.CompanyCodeId = p.CompanyCodeId and t.ActualDate between p.StartDate AND p.EndDate
                                and p.PayPeriodId= @payPeriodId and p.IsArchived = @isArchived) temp");

                        cmd.Parameters.Add("@empId", SqlDbType.Int);
                        cmd.Parameters["@empId"].Value = empId;

                        cmd.Parameters.Add("@payPeriodId", SqlDbType.Int);
                        cmd.Parameters["@payPeriodId"].Value = payPeriodId;

                        cmd.Parameters.Add("@isArchived", SqlDbType.Bit);
                        cmd.Parameters["@isArchived"].Value = isArchived;

                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.Connection = sqlConn;
                        sqlConn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader != null && reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    TimeCardWeekTotalCollection e_timecardRecord = new TimeCardWeekTotalCollection();
                                    //e_timecardRecord.WeekNum = (reader["WeekNum"] as int?) ?? default(int);
                                    e_timecardRecord.RegularHours = (reader["TotalRegularHours"] as Double?) ?? default(Double);
                                    e_timecardRecord.OverTime = (reader["TotalOverTime"] as Double?) ?? default(Double);
                                    e_timecardRecord.CodedHours = (reader["TotalCodedHours"] as Double?) ?? default(Double);
                                    e_timecardRecord.WeeklyTotal = e_timecardRecord.RegularHours + e_timecardRecord.CodedHours + e_timecardRecord.OverTime;

                                    employeePPTotalTimeCardInOutList.Add(e_timecardRecord);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
            return employeePPTotalTimeCardInOutList;
        }


        //Timecard Aprroval Report
        public Collection<TimeCardApprovalReportCollection> LoadTimeCardApprovalReportPP(int compCodeId, short deptId, int payPeriodId)
        {
            var employees_PayPeriodTotalList = new Collection<TimeCardApprovalReportCollection>();

            using (SqlConnection sqlConn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandText = (@"select EmployeeId, PPTotalRegularHour = case when (PPRegularHour + PPTotalCodedHour) > 80 Then
                            PPRegularHour- ((PPRegularHour + PPTotalCodedHour)-80) else PPRegularHour end,
                            PPTotalOverTime = case when (PPRegularHour + PPTotalCodedHour) > 80 Then
                            ((PPRegularHour + PPTotalCodedHour)-80) else 0 end, PPTotalCodedHour, Approved  from
                            (select  t.EmployeeId,sum(t.DailyHours) as PPRegularHour, sum(t.Hours) as PPTotalCodedHour, a.Approved as Approved
                            from TimeCards t, PayPeriods p, Employees e, TimeCardApprovals a 
                            where t.CompanyCodeId = p.CompanyCodeId and t.ActualDate >= p.StartDate AND t.ActualDate <= p.EndDate 
                            And t.EmployeeId = e.EmployeeId and t.CompanyCodeId = e.CompanyCodeId And a.EmployeeId = t.EmployeeId and a.EmployeeId = e.EmployeeId
                            and p.CompanyCodeId = e.CompanyCodeId and p.PayPeriodId = a.PayPeriodId And p.PayPeriodId = @payPeriodId And e.DepartmentId = @deptId and t.CompanyCodeId = @compCodeId
                            group by t.EmployeeId, a.Approved) temp
                            UNION
                            select EmployeeId, PPTotalRegularHour = case when (PPRegularHour + PPTotalCodedHour) > 80 Then
                            PPRegularHour- ((PPRegularHour + PPTotalCodedHour)-80) else PPRegularHour end,
                            PPTotalOverTime = case when (PPRegularHour + PPTotalCodedHour) > 80 Then
                            ((PPRegularHour + PPTotalCodedHour)-80) else 0 end, PPTotalCodedHour, Approved 
                            from
                            (select t.EmployeeId,
                            Sum(cast(Case When (TimeIn IS NOT NULL) and (TimeOut IS NOT NULL) and (LunchOut IS NOT NULL) and (LunchBack IS NOT NULL) Then
                            Convert( Varchar(5),(DATEDIFF(MINUTE, TimeIn, TimeOut)-DATEDIFF(MINUTE, LunchOut, LunchBack))/60) + '.' + 
                            RIGHT('0' + CAST((DATEDIFF(MINUTE, TimeIn, TimeOut)-DATEDIFF(MINUTE, LunchOut, LunchBack))%60 AS varchar(2)),2)
                            When (TimeIn IS NOT NULL) and (TimeOut IS NOT NULL) and (LunchOut IS NULL) and (LunchBack IS NULL) Then
                            Convert( Varchar(5),DATEDIFF(MINUTE, TimeIn, TimeOut)/60) + '.'+ RIGHT('0' + CAST(DATEDIFF(MINUTE, TimeIn, TimeOut) % 60 AS varchar(2)),2)
                            Else NULL End as Decimal(6,2))) as PPRegularHour, sum(t.hours) as PPTotalCodedHour, a.Approved as Approved from TimeCards t, PayPeriods p, Employees e, TimeCardApprovals a 
                            where t.CompanyCodeId = p.CompanyCodeId and t.ActualDate >= p.StartDate AND t.ActualDate <= p.EndDate 
                            And t.EmployeeId = e.EmployeeId and t.CompanyCodeId = e.CompanyCodeId And a.EmployeeId = t.EmployeeId and a.EmployeeId = e.EmployeeId
                            and p.CompanyCodeId = e.CompanyCodeId and p.PayPeriodId = a.PayPeriodId And p.PayPeriodId = @payPeriodId And e.DepartmentId = @deptId and t.CompanyCodeId = @compCodeId
                            group by t.EmployeeId, a.Approved) temp where PPRegularHour != NULL OR PPRegularHour > 0");

                        cmd.Parameters.Add("@compCodeId", SqlDbType.SmallInt);
                        cmd.Parameters["@compCodeId"].Value = compCodeId;

                        cmd.Parameters.Add("@deptId", SqlDbType.SmallInt);
                        cmd.Parameters["@deptId"].Value = deptId;

                        cmd.Parameters.Add("@payPeriodId", SqlDbType.Int);
                        cmd.Parameters["@payPeriodId"].Value = payPeriodId;

                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.Connection = sqlConn;
                        sqlConn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader != null && reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    TimeCardApprovalReportCollection e_timecardRecord = new TimeCardApprovalReportCollection();
                                    e_timecardRecord.EmployeeId = (reader["EmployeeId"] as int?) ?? default(int);
                                    e_timecardRecord.RegularHours = (reader["PPTotalRegularHour"] as Double?) ?? default(Double);
                                    e_timecardRecord.OverTime = (reader["PPTotalOverTime"] as Double?) ?? default(Double);
                                    e_timecardRecord.CodedHours = (reader["PPTotalCodedHour"] as Double?) ?? default(Double);
                                    e_timecardRecord.Approved = (reader["Approved"] as bool?) ?? default(bool);
                                    e_timecardRecord.Emp_PayPeriodTotal = e_timecardRecord.RegularHours + e_timecardRecord.CodedHours + e_timecardRecord.OverTime;

                                    employees_PayPeriodTotalList.Add(e_timecardRecord);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
            return employees_PayPeriodTotalList;
        }


        public Collection<int> LoadEmpList(int compCodeId, short deptId, int? payPeriodId)
        {
            var employees_List = new Collection<int>();

            using (SqlConnection sqlConn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandText = (@"select t.EmployeeiD as EmployeeId from Timecards t , Payperiods p, Employees e 
                            where t.CompanyCodeId = p.CompanyCodeId and t.ActualDate >= p.startdate and t.ActualDate <= p.enddate
                            and t.CompanyCodeId = e.CompanyCodeId and t.EmployeeId = e.EmployeeId and e.DepartmentId = @deptId and t.CompanyCodeId = @compCodeId
                            and p.PayPeriodId = @payPeriodId GROUP BY t.EmployeeId");

                        cmd.Parameters.Add("@compCodeId", SqlDbType.SmallInt);
                        cmd.Parameters["@compCodeId"].Value = compCodeId;

                        cmd.Parameters.Add("@deptId", SqlDbType.SmallInt);
                        cmd.Parameters["@deptId"].Value = deptId;

                        cmd.Parameters.Add("@payPeriodId", SqlDbType.Int);
                        cmd.Parameters["@payPeriodId"].Value = payPeriodId;

                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.Connection = sqlConn;
                        sqlConn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader != null && reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    employees_List.Add((reader["EmployeeId"] as int?) ?? default(int));
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
            return employees_List;
        }

        
        //Export Timecard
        public Collection<TimeCardExportCollection> Export_Employee_TimecardDetails(int companyCodeId, int payPeriodId)
        {
            var TimeCardExportData = new Collection<TimeCardExportCollection>();

            using (SqlConnection sqlConn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandText = ("sp_TimeCardExport");

                        cmd.Parameters.Add("@companyCodeId", SqlDbType.Int);
                        cmd.Parameters["@companyCodeId"].Value = companyCodeId;

                        cmd.Parameters.Add("@payPeriodId", SqlDbType.Int);
                        cmd.Parameters["@payPeriodId"].Value = payPeriodId;

                        //System.IO.Directory.CreateDirectory(System.Configuration.ConfigurationManager.AppSettings["appRoot"] + "\\TimeCardExport");

                        //cmd.Parameters.Add("@filePath", SqlDbType.VarChar);
                        //cmd.Parameters["@filePath"].Value = System.Configuration.ConfigurationManager.AppSettings["appRoot"] + "\\TimeCardExport\\FileTimeCard.csv";

                        
                            //+ DateTime.Now.ToString("_MM-dd-yyyy_HH:mm:ss:fff") + ".csv";
                        //cmd.Parameters["@filePath"].Value = "C:\\ExportFile.csv";  
                            //DateTime.Now.ToString("_MM-dd-yyyy_HH:mm:ss:fff") + ".csv";

                        //cmd.Parameters.Add(new SqlParameter("@RETURN_VALUE", SqlDbType.Bit));
                        //cmd.Parameters["@RETURN_VALUE"].Direction = ParameterDirection.ReturnValue;

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = sqlConn;
                        sqlConn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader != null && reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    TimeCardExportCollection e_timecardExportRecord = new TimeCardExportCollection();
                                    e_timecardExportRecord.CompanyCode = ((reader["Co Code"] as short?) ?? default(short));
                                    e_timecardExportRecord.BatchId = ((reader["Batch ID"] as short?) ?? default(short));
                                    e_timecardExportRecord.FileNumber = (reader["File #"] == DBNull.Value) ? (String)null : ((String)reader["File #"]);
                                    e_timecardExportRecord.RegularHours = ((reader["Reg Hours"] as double?) ?? default(double));
                                    e_timecardExportRecord.OverTimeHours = ((reader["O/T Hours"] as double?) ?? default(double));
                                    e_timecardExportRecord.Hours3Code = (reader["Hours 3 Code"] == DBNull.Value) ? (String)null : ((String)reader["Hours 3 Code"]);
                                    e_timecardExportRecord.Hours3Amount = ((reader["Hours 3 Amount"] as double?) ?? default(double));
                                    e_timecardExportRecord.Hours4Code = (reader["Hours 4 Code"] == DBNull.Value) ? (String)null : ((String)reader["Hours 4 Code"]);
                                    e_timecardExportRecord.Hours4Amount = ((reader["Hours 4 Amount"] as double?) ?? default(double));
                                    e_timecardExportRecord.Hours5Code = (reader["Hours 5 Code"] == DBNull.Value) ? (String)null : ((String)reader["Hours 5 Code"]);
                                    e_timecardExportRecord.Hours5Amount = ((reader["Hours 5 Amount"] as double?) ?? default(double));
                                    e_timecardExportRecord.Earnings3Code = (reader["Earnings 3 Code"] == DBNull.Value) ? (String)null : ((String)reader["Earnings 3 Code"]);
                                    e_timecardExportRecord.Earnings3Amount = ((reader["Earnings 3 Amount"] as double?) ?? default(double));
                                    e_timecardExportRecord.Earnings4Code = (reader["Earnings 4 Code"] == DBNull.Value) ? (String)null : ((String)reader["Earnings 4 Code"]);
                                    e_timecardExportRecord.Earnings4Amount = ((reader["Earnings 4 Amount"] as double?) ?? default(double));
                                    e_timecardExportRecord.Earnings5Code = (reader["Earnings 5 Code"] == DBNull.Value) ? (String)null : ((String)reader["Earnings 5 Code"]);
                                    e_timecardExportRecord.Earnings5Amount = ((reader["Earnings 5 Amount"] as double?) ?? default(double));
                                    
                                    TimeCardExportData.Add(e_timecardExportRecord);
                                }
                            }
                        }

                        //cmd.ExecuteNonQuery();
                        //return Convert.ToBoolean(cmd.Parameters["@RETURN_VALUE"].Value);
                    }
                     catch (Exception e)
                    {
                        throw e;
                    }
                }
            }

            return TimeCardExportData;
        }



    }
   
}




