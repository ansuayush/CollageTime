      
Alter PROCEDURE [dbo].[sp_GetEmpWeeklyTotalTimeCardArchive]      
 @empid int,       
 @PayPeriodId int       
AS      
BEGIN       
select WeekNum, RegularHours = case when (RegularHours + CodedHours) > 40 Then      
                                RegularHours- ((RegularHours + CodedHours)-40) else RegularHours end,      
                                OverTime = case when (RegularHours + CodedHours) > 40 Then      
                                ((RegularHours + CodedHours)-40) else 0 end, CodedHours       
               
                                from      
                                (select case when t.ActualDate >= p.StartDate and t.ActualDate <= DateAdd(day,6,p.StartDate) Then 1      
                                when t.ActualDate > DateAdd(day,6,p.StartDate) and t.ActualDate <= p.EndDate Then 2 End as WeekNum,      
                                --Sum(cast(Case When (TimeIn IS NOT NULL) and (TimeOut IS NOT NULL) and (LunchOut IS NOT NULL) and (LunchBack IS NOT NULL) Then      
                                --Convert( Varchar(20),(DATEDIFF(MINUTE, TimeIn, TimeOut)-DATEDIFF(MINUTE, LunchOut, LunchBack))/60) + '.' +       
                                --RIGHT('0' + CAST((DATEDIFF(MINUTE, TimeIn, TimeOut)-DATEDIFF(MINUTE, LunchOut, LunchBack))%60 AS varchar(20)),2)      
                                --When (TimeIn IS NOT NULL) and (TimeOut IS NOT NULL) and (LunchOut IS NULL) and (LunchBack IS NULL) Then      
                                --Convert( Varchar(20),DATEDIFF(MINUTE, TimeIn, TimeOut)/60) + '.'+ RIGHT('0' + CAST(DATEDIFF(MINUTE, TimeIn, TimeOut) % 60 AS varchar(20)),2)      
                                --Else NULL End as Decimal(6,2)))     
        ISNULL(Sum(t.DailyHours), 0) as RegularHours, ISNULL(sum(hours),0) as CodedHours from TimeCardsArchives t, PayPeriods p      
                                where t.EmployeeId = @empid and t.IsDeleted = 0 and p.PayPeriodId= @payPeriodId  and t.ActualDate between p.StartDate AND p.EndDate      
                              group by case when t.ActualDate >= p.StartDate and t.ActualDate <= DateAdd(day,6,p.StartDate) Then 1      
                                when t.ActualDate > DateAdd(day,6,p.StartDate) and t.ActualDate <= p.EndDate Then 2 End) temp      
End