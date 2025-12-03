   
ALTER PROCEDURE [dbo].[sp_GetTimeCardUnApprovalReportForAdministrator]
	@CompCodeId int,                      
	@PayPeriodId int,
	@IsApproved bit
AS    
BEGIN  

	--declare @compCodeId int = 1
	--declare @payPeriodId int = 34
	--declare @IsApproved bit = 0

	Create table #TempTable
	(
		EmployeeId int,
		EmployeeName Varchar(100),
		FileNumber Varchar(50),
		DepartmentId int,
		DepartmentCode Varchar(10),
		DepartmentDescription Varchar(50),
		Total numeric(18,2),
		IsApproved Bit
	)

	Insert into #TempTable
	Select 
		Employees.EmployeeId as EmployeeId
		,per.FirstName + ' ' + per.LastName as EmployeeName
		,Employees.FileNumber
		,E_Positions.DepartmentId as DepartmentId
		,Departments.DepartmentCode as DepartmentCode
		,Departments.DepartmentDescription as DepartmentDescription
		,sum(TimeCards.DailyHours) as Total
		,TimeCards.IsApproved as IsApproved
	from TimeCards with (NOLOCK)
	inner join E_Positions on Timecards.PositionId = E_Positions.PositionId and Timecards.EmployeeId = E_Positions.EmployeeId
	left outer join Persons man on E_Positions.ReportsToId = man.PersonId
	inner join Employees on TimeCards.EmployeeId = Employees.EmployeeId
	inner join Persons per on per.PersonId = Employees.PersonId
	inner join DdlPayFrequencies dp on Employees.PayFrequencyId = dp.PayFrequencyId
	inner join PayPeriods on PayPeriods.PayFrequencyId = dp.PayFrequencyId
	inner join Departments on Departments.DepartmentId = E_Positions.DepartmentId
	where 
		--TimeCards.IsApproved = @IsApproved and 
		E_Positions.ReportsToId is not null and 
		PayPeriods.PayPeriodId = @PayPeriodId and
		Timecards.CompanyCodeId = @CompCodeId and TimeCards.ActualDate between PayPeriods.StartDate and PayPeriods.EndDate
	group by 
		Employees.EmployeeId
		, per.FirstName + ' ' + per.LastName
		, Employees.FileNumber
		, E_Positions.DepartmentId
		, Departments.DepartmentCode
		, Departments.DepartmentDescription
		, TimeCards.IsApproved
	having sum(TimeCards.DailyHours) > 0

	-- Selecting only unapproved timecard list
	SELECT * INTO #TempUnApproved
	From #TempTable
	where IsApproved = 0

	-- Selecting only approved timecard list
	SELECT * INTO #TempApproved
	From #TempTable
	where IsApproved = 1

	-- Grouping on Employee to get further total
	SELECT
		#TempTable.EmployeeId
		,#TempTable.EmployeeName
		,#TempTable.FileNumber
		,null as DepartmentId
		,null as DepartmentCode
		,null DepartmentDescription
		,Sum(#TempTable.Total) as Total
	INTO #TempPayPeriod
	from #TempTable
	group by #TempTable.EmployeeId, #TempTable.EmployeeName, #TempTable.FileNumber

	--SELECT * from #TempTable
	--SELECT * from #TempApproved
	--SELECT * from #TempUnApproved
	--Select * from #TempPayPeriod

	Select 
		#TempUnApproved.EmployeeId
		,#TempUnApproved.EmployeeName
		,#TempUnApproved.FileNumber
		,#TempUnApproved.DepartmentId
		,#TempUnApproved.DepartmentCode
		,#TempUnApproved.DepartmentDescription
		,#TempUnApproved.Total as UnApprovedHours
		,IsNull(#TempApproved.Total, 0) as ApprovedHours
		,#TempUnApproved.Total + IsNull(#TempApproved.Total, 0) as DepartmentTotalHours
		,#TempPayPeriod.Total as PayPeriodHours
	from #TempUnApproved
	left outer join #TempApproved on #TempApproved.DepartmentId = #TempUnApproved.DepartmentId
	and #TempApproved.EmployeeId = #TempUnApproved.EmployeeId
	left outer join #TempPayPeriod on #TempPayPeriod.EmployeeId = #TempUnApproved.EmployeeId
	order by #TempUnApproved.EmployeeName, #TempUnApproved.DepartmentCode

	DROP TABLE #TempTable
	DROP TABLE #TempApproved
	DROP TABLE #TempUnApproved
	DROP TABLE #TempPayPeriod

End