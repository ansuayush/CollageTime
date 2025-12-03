
go

if  exists (select 1 from sys.objects where object_id = object_id(N'dbo.spGetExportPayperiod') and type in (N'P', N'PC'))
drop procedure dbo.spGetExportPayperiod
go

create procedure dbo.spGetExportPayperiod
@PayPeriodNumber int,
@PayGroupId int,
@StartDate varchar(20),
@EndDate varchar(20)
as
/*
Changes
	Date		Author		TicketNo	Details
	13-Mar-18	Chandu		DU-1029		Changes made for PayRateExport based on PayPeriodNumber
	15-Mar-18	Chandu		DU-1029		Mande changes for Regular Earning calc and TempCostNumber and Department
	27-Mar-18	Chandu		DU-1029		Filter with TimeCardApprovals based on Approved flag
	28-Mar-18	Chandu		DU-1029		Changes made for PayGroupCode 1 EPIP export structure
	29-Mar-18	Chandu		DU-1214		Changes made for Position Line Item wise EPIP export(Student/Employee)
*/ 
declare 
	  @TempEPIPExport as table
	  (
		CoCode varchar(50),
		BatchID char(3),
		FileNumber varchar(50),
		CancelPay char(1),
		[Pay #] varchar(20),
		[Reg Hours] varchar(20),
		[O/T Hours] varchar(20),
		[Hours 3 Code] varchar(20),
		[Hours 3 Amount] varchar(20),
		[Hours 4 Code] varchar(20),
		[Hours 4 Amount] varchar(20),
		[RegEarnings] varchar(20),
		[O/T Earnings] varchar(20),
		[Earnings 3 Code] varchar(20),
		[Earnings 3 Amount] varchar(20),
		[Earnings 4 Code] varchar(20),
		[Earnings 4 Amount] varchar(20),
		[Earnings 5 Code] varchar(20),
		[Earnings 5 Amount] varchar(20),
		[CostNumber] varchar(100),
		[Department] varchar(50),
		[Temp Rate] varchar(20),
		[MemoCode1] varchar(20),
		[MemoAmount1] varchar(20),
		[MemoCode2] varchar(20),
		[MemoAmount2] varchar(20)
	  )

if(@PayPeriodNumber>=1 and @PayPeriodNumber<=18)
begin
	insert into
		@TempEPIPExport
	select
		distinct	
		'CoCode'=CompanyCodes.CompanyCodeDescription,
		'BatchID'='RSI',
		'FileNumber'=Employees.FileNumber,
		'CancelPay' = '',
		'Pay #'='',
		'Reg Hours'='',
		'O/T Hours'='',
		'Hours 3 Code'='',
		'Hours 3 Amount'='',
		'Hours 4 Code'='',
		'Hours 4 Amount'='',
		'RegEarnings'= case when @PayGroupId=1 then cast(round(sum(round(tc.DailyHours,2))over(partition by po.PositionId,Employees.FileNumber order by po.PositionId)* isnull(eps.PayRate,0),2) as varchar(20)) else '' end,
		'O/T Earnings'='',
		'Earnings 3 Code'='',
		'Earnings 3 Amount'='',
		'Earnings 4 Code'='',
		'Earnings 4 Amount'='',
		'Earnings 5 Code'='',
		'Earnings 5 Amount'='',
		'CostNumber' = po.CostNumber,
		'Department' = case when @PayGroupId=1 then '' else dp.DepartmentDescription end,
		'Temp Rate'='',
		'MemoCode1'=case when @PayGroupId=1 then '' else Contracts.MemoCode1 end,
		'MemoAmount1'= case when @PayGroupId=1 then '' else cast(Contracts.MemoCode1Amount as varchar(20)) end,
		'MemoCode2'=case when @PayGroupId=1 then '' else Contracts.MemoCode2 end,
		'MemoAmount2'= case when @PayGroupId=1 then '' else cast(Contracts.MemoCode2Amount as varchar(20)) end
	from 
		dbo.PayPeriods 
		inner join dbo.E_Positions on PayPeriods.PayGroupCode=E_Positions.PayGroupId
		inner join dbo.Positions po on po.PositionId = E_Positions.PositionId
		inner join dbo.Contracts on Contracts.Status1FlagCode=E_Positions.EmployeeTypeId and Contracts.ePositionId=E_Positions.E_PositionId
		and Contracts.PayPeriod=PayPeriods.PayPeriodNumber
		inner join dbo.Employees on E_Positions.EmployeeId=Employees.EmployeeId and Employees.IsStudent=0
		inner join dbo.CompanyCodes on CompanyCodes.CompanyCodeId = Employees.CompanyCodeId
		left join E_PositionSalaryHistories eps on eps.E_PositionId = E_Positions.E_PositionId and Contracts.ePositionId  = eps.E_PositionId and eps.EffectiveDate is not null
		inner join TimeCards tc on tc.EmployeeId = Employees.EmployeeId 
			and convert(date,tc.TimeIn) between convert(date,PayPeriods.StartDate) and convert(date,PayPeriods.EndDate) 
			and convert(date,tc.[TimeOut]) between convert(date,PayPeriods.StartDate) and convert(date,PayPeriods.EndDate) and tc.PositionId=po.PositionId
		inner join dbo.TimeCardApprovals tca on tc.EmployeeId = tca.EmployeeId and tc.ApprovedBy=tca.ManagerId and tca.Approved = 1
		left join dbo.Departments dp on dp.DepartmentId=Employees.DepartmentId
	where 
		convert(varchar(20),PayPeriods.StartDate,112)=@StartDate and
		convert(varchar(20),PayPeriods.EndDate,112)=@EndDate and
		PayPeriods.PayGroupCode= @PayGroupId and
		E_Positions.EmployeeTypeId=1 and
		Contracts.Status1FlagCode=1 and
		PayPeriods.PayPeriodNumber=@PayPeriodNumber
	union 
	select
		distinct 	
		'CoCode'=CompanyCodes.CompanyCodeDescription,
		'BatchID'='RSI',
		'FileNumber'=Employees.FileNumber,
		'CancelPay' = '',
		'Pay #'='',
		'Reg Hours'='',
		'O/T Hours'='',
		'Hours 3 Code'='',
		'Hours 3 Amount'='',
		'Hours 4 Code'='',
		'Hours 4 Amount'='',
		'RegEarnings'=case when @PayGroupId=1 then cast(round(sum(round(tc.DailyHours,2))over(partition by po.PositionId,Employees.FileNumber order by po.PositionId)* isnull(eps.PayRate,0),2) as varchar(20)) else '' end,
		'O/T Earnings'='',
		'Earnings 3 Code'='',
		'Earnings 3 Amount'='',
		'Earnings 4 Code'='',
		'Earnings 4 Amount'='',
		'Earnings 5 Code'='',
		'Earnings 5 Amount'='',
		'CostNumber' = po.CostNumber,
		'Department' =case when @PayGroupId=1 then '' else dp.DepartmentDescription end,
		'Temp Rate'='',
		'MemoCode1'='',
		'MemoAmount1'= '',
		'MemoCode2'='',
		'MemoAmount2'= ''
	from 
		dbo.PayPeriods 
		inner join dbo.E_Positions on PayPeriods.PayGroupCode=E_Positions.PayGroupId
		inner join dbo.Positions po on po.PositionId = E_Positions.PositionId
		inner join dbo.Contracts on Contracts.Status1FlagCode=E_Positions.EmployeeTypeId and Contracts.ePositionId=E_Positions.E_PositionId
		and Contracts.PayPeriod=PayPeriods.PayPeriodNumber
		inner join dbo.Employees on E_Positions.EmployeeId=Employees.EmployeeId and Employees.IsStudent=0
		inner join dbo.CompanyCodes on CompanyCodes.CompanyCodeId = Employees.CompanyCodeId
		left join E_PositionSalaryHistories eps on eps.E_PositionId = E_Positions.E_PositionId and Contracts.ePositionId  = eps.E_PositionId and eps.EffectiveDate is not null
		inner join TimeCards tc on tc.EmployeeId = Employees.EmployeeId 
			and convert(date,tc.TimeIn) between convert(date,PayPeriods.StartDate) and convert(date,PayPeriods.EndDate) 
			and convert(date,tc.[TimeOut]) between convert(date,PayPeriods.StartDate) and convert(date,PayPeriods.EndDate) and tc.PositionId=po.PositionId
		inner join dbo.TimeCardApprovals tca on tc.EmployeeId = tca.EmployeeId and tc.ApprovedBy=tca.ManagerId and tca.Approved = 1
		left join dbo.Departments dp on dp.DepartmentId=Employees.DepartmentId
	where 
		convert(varchar(20),PayPeriods.StartDate,112)=@StartDate and
		convert(varchar(20),PayPeriods.EndDate,112)=@EndDate and
		PayPeriods.PayGroupCode= @PayGroupId and
	    E_Positions.EmployeeTypeId=3 and
	    Contracts.Status1FlagCode=3 and
	    PayPeriods.PayPeriodNumber=@PayPeriodNumber
end
else if(@PayPeriodNumber>=19 and @PayPeriodNumber<=26)
begin
	insert into
		@TempEPIPExport
	select 	
		distinct
		'CoCode'=CompanyCodes.CompanyCodeDescription,
		'BatchID'='RSI',
		'FileNumber'=Employees.FileNumber,
		'CancelPay' = '',
		'PayNumber'='',
		'RegHours'='',
		'OTHours'='',
		'Hours3Code'='',
		'Hours3Amount'='',
		'Hours4Code'='',
		'Hours4Amount'='',
		'RegEarnings'= case when @PayGroupId=1 then cast(round(sum(round(tc.DailyHours,2))over(partition by po.PositionId,Employees.FileNumber order by po.PositionId)* isnull(eps.PayRate,0),2) as varchar(20)) else '' end,
		'OTEarnings'='',
		'Earnings3Code'='',
		'Earnings3Amount'='',
		'Earnings4Code'='',
		'Earnings4Amount'='',
		'Earnings5Code'='',
		'Earnings5Amount'='',
		'CostNumber' = po.CostNumber,
		'Department' =case when @PayGroupId=1 then '' else dp.DepartmentDescription end,
		'TempRate'='',
		'MemoCode1'='',
		'MemoAmount1'='',
		'MemoCode2'= case when @PayGroupId=1 then '' else Contracts.MemoCode2 end,
		'MemoAmount2'= case when @PayGroupId=1 then '' else cast(Contracts.MemoCode2Amount as varchar(20)) end
	from 
		dbo.PayPeriods 
		inner join dbo.E_Positions on PayPeriods.PayGroupCode=E_Positions.PayGroupId
		inner join dbo.Positions po on po.PositionId = E_Positions.PositionId
		inner join dbo.Contracts on Contracts.Status1FlagCode=E_Positions.EmployeeTypeId and Contracts.ePositionId=E_Positions.E_PositionId
		and Contracts.PayPeriod=PayPeriods.PayPeriodNumber
		inner join dbo.Employees on E_Positions.EmployeeId=Employees.EmployeeId and Employees.IsStudent=0
		inner join dbo.CompanyCodes on CompanyCodes.CompanyCodeId = Employees.CompanyCodeId
		left join E_PositionSalaryHistories eps on eps.E_PositionId = E_Positions.E_PositionId and Contracts.ePositionId  = eps.E_PositionId and eps.EffectiveDate is not null
		inner join TimeCards tc on tc.EmployeeId = Employees.EmployeeId 
			and convert(date,tc.TimeIn) between convert(date,PayPeriods.StartDate) and convert(date,PayPeriods.EndDate) 
			and convert(date,tc.[TimeOut]) between convert(date,PayPeriods.StartDate) and convert(date,PayPeriods.EndDate) and tc.PositionId=po.PositionId
		inner join dbo.TimeCardApprovals tca on tc.EmployeeId = tca.EmployeeId and tc.ApprovedBy=tca.ManagerId and tca.Approved = 1
		left join dbo.Departments dp on dp.DepartmentId=Employees.DepartmentId
	where 
		convert(varchar(20),PayPeriods.StartDate,112)=@StartDate and
		convert(varchar(20),PayPeriods.EndDate,112)=@EndDate and
		PayPeriods.PayGroupCode= @PayGroupId and
		E_Positions.EmployeeTypeId=1 and
		Contracts.Status1FlagCode=1 and
		PayPeriods.PayPeriodNumber=@PayPeriodNumber
		union 
		select 
			distinct	
			'CoCode'=CompanyCodes.CompanyCodeDescription,
			'BatchID'='RSI',
			'FileNumber'=Employees.FileNumber,
			'CancelPay' = 'Y',
			'PayNumber'='',
			'RegHours'='',
			'OTHours'='',
			'Hours3Code'='',
			'Hours3Amount'='',
			'Hours4Code'='',
			'Hours4Amount'='',
			'RegEarnings'=case when @PayGroupId=1 then cast(round(sum(round(tc.DailyHours,2))over(partition by po.PositionId,Employees.FileNumber order by po.PositionId)* isnull(eps.PayRate,0),2) as varchar(20)) else '' end,
			'OTEarnings'='',
			'Earnings3Code'='',
			'Earnings3Amount'='',
			'Earnings4Code'='',
			'Earnings4Amount'='',
			'Earnings5Code'='',
			'Earnings5Amount'='',
			'CostNumber' = po.CostNumber,
			'Department' = case when @PayGroupId=1 then '' else dp.DepartmentDescription end,
			'TempRate'='',
			'MemoCode1'='',
			'MemoAmount1'='',
			'MemoCode2'= '',--case when @PayGroupId=1 then '' else Contracts.MemoCode2,
			'MemoAmount2'= '' --cast(Contracts.MemoCode2Amount as varchar(20))
		from 
			dbo.PayPeriods 
			inner join dbo.E_Positions on PayPeriods.PayGroupCode=E_Positions.PayGroupId
			inner join dbo.Positions po on po.PositionId = E_Positions.PositionId
			inner join dbo.Contracts on Contracts.Status1FlagCode=E_Positions.EmployeeTypeId and Contracts.ePositionId=E_Positions.E_PositionId
			and Contracts.PayPeriod=PayPeriods.PayPeriodNumber
			inner join dbo.Employees on E_Positions.EmployeeId=Employees.EmployeeId and Employees.IsStudent=0
			inner join dbo.CompanyCodes on CompanyCodes.CompanyCodeId = Employees.CompanyCodeId
			left join E_PositionSalaryHistories eps on eps.E_PositionId = E_Positions.E_PositionId and Contracts.ePositionId  = eps.E_PositionId and eps.EffectiveDate is not null
			inner join TimeCards tc on tc.EmployeeId = Employees.EmployeeId 
			and convert(date,tc.TimeIn) between convert(date,PayPeriods.StartDate) and convert(date,PayPeriods.EndDate) 
			and convert(date,tc.[TimeOut]) between convert(date,PayPeriods.StartDate) and convert(date,PayPeriods.EndDate) and tc.PositionId=po.PositionId
			inner join dbo.TimeCardApprovals tca on tc.EmployeeId = tca.EmployeeId and tc.ApprovedBy=tca.ManagerId and tca.Approved = 1
			left join dbo.Departments dp on dp.DepartmentId=Employees.DepartmentId
		where 
			convert(varchar(20),PayPeriods.StartDate,112)=@StartDate and
			convert(varchar(20),PayPeriods.EndDate,112)=@EndDate and
			PayPeriods.PayGroupCode= @PayGroupId and
		    E_Positions.EmployeeTypeId=3 and
		    Contracts.Status1FlagCode=3 and
		    PayPeriods.PayPeriodNumber=@PayPeriodNumber
end

if(@PayGroupId=1)
begin
	insert into
			@TempEPIPExport
		select
			distinct	
			'CoCode'=CompanyCodes.CompanyCodeDescription,
			'BatchID'='RSI',
			'FileNumber'=Employees.FileNumber,
			'CancelPay' = '',
			'Pay #'='',
			'Reg Hours'='',
			'O/T Hours'='',
			'Hours 3 Code'='',
			'Hours 3 Amount'='',
			'Hours 4 Code'='',
			'Hours 4 Amount'='',
			'RegEarnings'= cast(round(sum(round(tc.DailyHours,2))over(partition by po.PositionId,Employees.FileNumber order by po.PositionId)* isnull(eps.PayRate,0),2) as varchar(20)),
			'O/T Earnings'='',
			'Earnings 3 Code'='',
			'Earnings 3 Amount'='',
			'Earnings 4 Code'='',
			'Earnings 4 Amount'='',
			'Earnings 5 Code'='',
			'Earnings 5 Amount'='',
			'CostNumber' = po.CostNumber,
			'Department' = '',
			'Temp Rate'='',
			'MemoCode1'='',
			'MemoAmount1'= '',
			'MemoCode2'='',
			'MemoAmount2'= ''
		from 
			dbo.PayPeriods 
			inner join dbo.E_Positions on PayPeriods.PayGroupCode=E_Positions.PayGroupId
			inner join dbo.Positions po on po.PositionId = E_Positions.PositionId
			inner join dbo.Employees on E_Positions.EmployeeId=Employees.EmployeeId and Employees.IsStudent=1
			inner join dbo.CompanyCodes on CompanyCodes.CompanyCodeId = Employees.CompanyCodeId
			left join E_PositionSalaryHistories eps on eps.E_PositionId = E_Positions.E_PositionId and eps.EffectiveDate is not null  
			inner join TimeCards tc on tc.EmployeeId = Employees.EmployeeId 
			and convert(date,tc.TimeIn) between convert(date,PayPeriods.StartDate) and convert(date,PayPeriods.EndDate) 
			and convert(date,tc.[TimeOut]) between convert(date,PayPeriods.StartDate) and convert(date,PayPeriods.EndDate) and tc.PositionId=po.PositionId
			inner join dbo.TimeCardApprovals tca on tc.EmployeeId = tca.EmployeeId and tc.ApprovedBy=tca.ManagerId and tca.Approved = 1
		where 
			convert(varchar(20),PayPeriods.StartDate,112)=@StartDate and
			convert(varchar(20),PayPeriods.EndDate,112)=@EndDate and
			PayPeriods.PayGroupCode= @PayGroupId and
			PayPeriods.PayPeriodNumber=@PayPeriodNumber
end
	select * from @TempEPIPExport
go

