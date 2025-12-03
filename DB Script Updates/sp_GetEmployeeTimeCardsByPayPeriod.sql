USE [DUBlankDB]
GO

/****** Object:  StoredProcedure [dbo].[sp_GetEmployeeTimeCardsByPayPeriod]    Script Date: 2/26/2018 5:17:56 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_GetEmployeeTimeCardsByPayPeriod]        
	@EmployeeId int,
	@PayPeriodId int,
	@IsArchived bit
AS        
BEGIN         

--declare @empId int = 8    
--declare  @isArchived bit = 0 
--declare @PayPeriodId int= 1

select 
	t.TimeCardId
	, t.EmployeeId
	, t.ActualDate
	, t.TimeIn
	, t.TimeOut
	, t.LunchOut
	, t.LunchBack
	, t.DailyHours
	, t.Hours
	, pos.title
	, P.PayPeriodId
	, pos.PositionId
from TimeCards t, PayPeriods p , Positions pos
where t.EmployeeId = @EmployeeId  And p.PayPeriodId = @payPeriodId and t.ActualDate between p.StartDate and p.EndDate and p.IsArchived = @IsArchived 
 and (t.PositionId = pos.PositionId)
Order by ActualDate
   
End
GO

