        
Alter PROCEDURE [dbo].[sp_GetEmpWeeklyTimeCardArchive]         
 @PayPeriodId int,        
 @empid int        
        
AS        
BEGIN         
select t.TimeCardsArchiveId, t.CompanyCodeId, t.FileNumber, t.ActualDate, t.ProjectNumber, t.HoursCodeId, t.Hours,        
                                    t.EarningsCodeId, t.EarningsAmount, t.TempDeptId, t.TempJobId, t.TimeIn, t.TimeOut, LunchOut, LunchBack,         
                                    case when t.ActualDate >= p.StartDate and t.ActualDate <= DateAdd(day,6,p.StartDate) Then 1        
                                         when t.ActualDate > DateAdd(day,6,p.StartDate) and t.ActualDate <= p.EndDate Then 2        
                                    End as WeekNum ,  IsApproved as IsLineApproved,        
                                    --Case When (TimeIn IS NOT NULL) and (TimeOut IS NOT NULL) and (LunchOut IS NOT NULL) and (LunchBack IS NOT NULL) Then        
                                    --        Convert( Varchar(5),(DATEDIFF(MINUTE, TimeIn, TimeOut)-DATEDIFF(MINUTE, LunchOut, LunchBack))/60) + '.' +         
                                    --        RIGHT('0' + CAST((DATEDIFF(MINUTE, TimeIn, TimeOut)-DATEDIFF(MINUTE, LunchOut, LunchBack))%60 AS varchar(2)),2)        
                                    --    When (TimeIn IS NOT NULL) and (TimeOut IS NOT NULL) and (LunchOut IS NULL) and (LunchBack IS NULL) Then        
                                    --        Convert( Varchar(5),DATEDIFF(MINUTE, TimeIn, TimeOut)/60) + '.'+        
                                    --        RIGHT('0' + CAST(DATEDIFF(MINUTE, TimeIn, TimeOut) % 60 AS varchar(2)),2)        
                                    --    Else NULL        
                                    --    End     
         ISNULL(t.DailyHours, 0) as LineTotal,t.ActualDate as Day, t.PositionId, t.UserId, t.TimeCardId, t.IsDeleted  
                                    from TimecardsArchives t, PayPeriods p        
                                    where t.EmployeeId=@empid and t.IsDeleted = 0 and p.PayPeriodId=@PayPeriodId and t.ActualDate between p.StartDate and p.EndDate         
         order by t.ActualDate, WeekNum        
End