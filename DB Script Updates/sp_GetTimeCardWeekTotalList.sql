    
Alter PROCEDURE [dbo].[sp_GetTimeCardWeekTotalList]    
@empId int,        
 @isArchived bit,        
 @PayPeriodId int         
AS        
BEGIN         
select WeekNum, RegularHours = case when (WeekRegularHour + CodedHours) > 40 Then        
                            WeekRegularHour- ((WeekRegularHour + CodedHours)-40) else WeekRegularHour end,        
    OverTime = case when (WeekRegularHour + CodedHours) > 40 Then        
                            ((WeekRegularHour + CodedHours)-40) else 0 end,       
 CodedHours    from        
                            (select          
                            case when t.ActualDate >= p.StartDate and t.ActualDate <= DateAdd(day,6,p.StartDate) Then 1        
                                    when t.ActualDate > DateAdd(day,6,p.StartDate) and t.ActualDate <= p.EndDate Then 2        
                            End as WeekNum ,        
       sum(t.DailyHours) as WeekRegularHour, sum(Hours) as CodedHours  
                            from TimeCards t, PayPeriods p    
        where t.EmployeeId = @empId   and t.IsDeleted=0      
                            And p.PayPeriodId = @payPeriodId And t.ActualDate between p.StartDate AND p.EndDate And  p.IsArchived = @isArchived             
                            --where t.CompanyCodeId = p.CompanyCodeId and t.ActualDate between p.StartDate AND p.EndDate And t.EmployeeId = @empId         
                            --And p.PayPeriodId = @payPeriodId and p.IsArchived = @isArchived        
                            group by case when t.ActualDate >= p.StartDate and t.ActualDate <= DateAdd(day,6,p.StartDate) Then 1        
                            when t.ActualDate > DateAdd(day,6,p.StartDate) and t.ActualDate <= p.EndDate Then 2 end) temp        
End