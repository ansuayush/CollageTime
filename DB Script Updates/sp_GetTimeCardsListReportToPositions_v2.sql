

CREATE PROCEDURE [dbo].[sp_GetTimeCardsListReportToPositions_v2]      
	@empId int,
	@isArchived bit,
	@PayPeriodId int,
	@ReportToId nvarchar(max)
AS                  
BEGIN
	Select   
		t.TimeCardId
		,t.CompanyCodeId
		,t.EmployeeId
		,t.ActualDate
		,t.ProjectNumber
		,t.TimeIn
		,t.TimeOut
		,t.LunchOut
		,t.LunchBack
		,t.DailyHours
		,t.HoursCodeId
		,t.Hours
		,t.EarningsCodeId
		,t.EarningsAmount
		,t.TempDeptId
		,t.TempJobId
		,P.PayPeriodId,
		case 
			when t.ActualDate >= p.StartDate and t.ActualDate <= DateAdd(day,6,p.StartDate) 
			Then 1
			when t.ActualDate > DateAdd(day,6,p.StartDate) and t.ActualDate <= p.EndDate 
			Then 2
		End as WeekNum
		,case 
			when (t.Hours is not null) and (t.DailyHours is not null) 
			Then (t.Hours+t.DailyHours)
			when (t.Hours is null ) and (t.DailyHours is not null)  
			then (t.DailyHours)
			when (t.Hours is not null ) and (t.DailyHours is null) 
			then (t.Hours)
			when (t.Hours is null ) and (t.DailyHours is  null)  
			then 0 end  as LineTotal
		,IsApproved as IsLineApproved
		,t.FundsId
		,t.ProjectsId
		,t.TaskId
		,t.PositionId
		,t.UserId
		,t.LastModifiedDate   
	from Timecards t  
	inner join PayPeriods p on t.ActualDate between p.StartDate and p.endDate  
	inner join E_Positions on E_Positions.PositionId = t.PositionId   
	where t.EmployeeId = @empId and p.PayPeriodId = @PayPeriodId and t.PositionId is not null  
	and E_Positions.ReportsToID in (Select value from STRING_SPLIT(@ReportToId,',')) and 
	E_Positions.EmployeeId = @empId  
	Order by TimeCardId desc   
End
GO

