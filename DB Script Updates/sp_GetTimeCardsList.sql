alter PROCEDURE [dbo].[sp_GetTimeCardsList]                    
@empId int,                    
 @isArchived bit,                    
 @PayPeriodId int                
--Company Code commented for getting the list and Changed the Order             
AS                    
BEGIN                     
select t.TimeCardId, t.CompanyCodeId, t.EmployeeId, t.ActualDate, t.ProjectNumber, t.TimeIn, t.TimeOut, t.LunchOut, t.LunchBack, t.DailyHours, t.HoursCodeId, t.Hours,                  
                                    t.EarningsCodeId, t.EarningsAmount, t.TempDeptId, t.TempJobId, P.PayPeriodId,                   
                                    case when t.ActualDate >= p.StartDate and t.ActualDate <= DateAdd(day,6,p.StartDate) Then 1                  
                                         when t.ActualDate > DateAdd(day,6,p.StartDate) and t.ActualDate <= p.EndDate Then 2                  
                                    End as WeekNum ,              
         --(t.DailyHours +  t.Hours) as LineTotal,             
         case when (t.Hours is not null) and (t.DailyHours is not null) Then (t.Hours+t.DailyHours)               
when (t.Hours is null ) and (t.DailyHours is not null)  then (t.DailyHours)            
when (t.Hours is not null ) and (t.DailyHours is null)  then (t.Hours)            
when (t.Hours is null ) and (t.DailyHours is  null)  then 0 end  as LineTotal,            
         IsApproved as IsLineApproved,t.FundsId,t.ProjectsId,t.TaskId, t.PositionId, t.UserId, t.LastModifiedDate, t.IsDeleted ,t.E_PositionId   
                                    from TimeCards t, PayPeriods p             
            where t.EmployeeId = @empId   and t.IsDeleted = 0               
                                     And p.PayPeriodId = @payPeriodId and t.ActualDate between p.StartDate AND p.EndDate And  p.IsArchived = @isArchived Order by ActualDate, WeekNum,TimeCardId desc            
                                    --where t.CompanyCodeId = p.CompanyCodeId and t.ActualDate between p.StartDate AND p.EndDate And t.EmployeeId = @empId                  
                                    -- And p.PayPeriodId = @payPeriodId and p.IsArchived = @isArchived Order by ActualDate, WeekNum                  
End