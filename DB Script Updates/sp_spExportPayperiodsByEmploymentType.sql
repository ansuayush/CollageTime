
go

if  exists (select 1 from sys.objects where object_id = object_id(N'dbo.spExportPayperiodsByEmploymentType') and type in (N'P', N'PC'))
drop procedure dbo.spExportPayperiodsByEmploymentType
go

if  exists (select 1 from sys.objects where object_id = object_id(N'dbo.Sp_Payperiod') and type in (N'P', N'PC'))
drop procedure dbo.Sp_Payperiod
go

if  exists (select 1 from sys.objects where object_id = object_id(N'dbo.Sp_Payperiodwithemptype') and type in (N'P', N'PC'))
drop procedure dbo.Sp_Payperiodwithemptype
go

create procedure dbo.spExportPayperiodsByEmploymentType 
@EmploymentType varchar(20)
as
/*
Changes
	Date		Author		TicketNo	Details
	08-Mar-18	Chandu		DU-847		Changes made for PayRateExport based on EmploymentType
*/ 
begin
	if(@EmploymentType='9*9/1-18')
	begin
		select 	
			'Co Code'=Employees.CompanyCode,
			'Batch ID'='RSI',
			'File #'=E_Positions.FileNumber,
			'Pay #'='',
			'Reg Hours'='',
			'O/T Hours'='',
			'Hours 3 Code'='',
			'Hours 3 Amount'='',
			'Hours 4 Code'='',
			'Hours 4 Amount'='',
			'Reg Earnings'='',
			'O/T Earnings'='',
			'Earnings 3 Code'='',
			'Earnings 3 Amount'='',
			'Earnings 4 Code'='',
			'Earnings 4 Amount'='',
			'Earnings 5 Code'='',
			'Earnings 5 Amount'='',
			'Temp Rate'='',
			'Memo Code'=Contracts.MemoCode1,
			'Memo Amount'= cast(Contracts.MemoCode1Amount as varchar(20)),
			'Memo Code'=Contracts.MemoCode2,
			'Memo Amount'= cast(Contracts.MemoCode2Amount as varchar(20))
	from 
		dbo.PayPeriods 
		inner join dbo.E_Positions on PayPeriods.PayGroupCode=E_Positions.PayGroupId
		inner join dbo.Contracts on Contracts.Status1FlagCode=E_Positions.EmployeeTypeId
		inner join dbo.Employees on E_Positions.EmployeeId=Employees.EmployeeId
	where 
		Contracts.StartDate=PayPeriods.StartDate and
	    Contracts.EndDate=PayPeriods.EndDate and
	    E_Positions.EmployeeTypeId=3 and
	    Contracts.Status1FlagCode=3 and
	    Contracts.PayPeriod between 1 and 18
	end
	else if(@EmploymentType='9*9/19-26')
	begin
		select 	
			'CoCode'=Employees.CompanyCode,
			'BatchID'='RSI',
			'FileNumber'=E_Positions.FileNumber,
			'CancelPay' = 'Y',
			'PayNumber'='',
			'RegHours'='',
			'OTHours'='',
			'Hours3Code'='',
			'Hours3Amount'='',
			'Hours4Code'='',
			'Hours4Amount'='',
			'RegEarnings'='',
			'OTEarnings'='',
			'Earnings3Code'='',
			'Earnings3Amount'='',
			'Earnings4Code'='',
			'Earnings4Amount'='',
			'Earnings5Code'='',
			'Earnings5Amount'='',
			'TempRate'='',
			'MemoCode1'='',
			'MemoAmount1'='',
			'MemoCode2'=Contracts.MemoCode2,
			'MemoAmount2'= cast(Contracts.MemoCode2Amount as varchar(20))
		from 
			dbo.PayPeriods 
			inner join dbo.E_Positions on PayPeriods.PayGroupCode=E_Positions.PayGroupId
			inner join dbo.Contracts on Contracts.Status1FlagCode=E_Positions.EmployeeTypeId
			inner join dbo.Employees on E_Positions.EmployeeId=Employees.EmployeeId
		where 
			Contracts.StartDate=PayPeriods.StartDate and
		    Contracts.EndDate=PayPeriods.EndDate and
		    E_Positions.EmployeeTypeId=3 and
		    Contracts.Status1FlagCode=3 and
		    Contracts.PayPeriod between 19 and 26
	end
	else if(@EmploymentType='9*12/1-18')
	begin
		select 	
			'Co Code'=Employees.CompanyCode,
			'Batch ID'='RSI',
			'File #'=E_Positions.FileNumber,
			'Pay #'='',
			'Reg Hours'='',
			'O/T Hours'='',
			'Hours 3 Code'='',
			'Hours 3 Amount'='',
			'Hours 4 Code'='',
			'Hours 4 Amount'='',
			'Reg Earnings'='',
			'O/T Earnings'='',
			'Earnings 3 Code'='',
			'Earnings 3 Amount'='',
			'Earnings 4 Code'='',
			'Earnings 4 Amount'='',
			'Earnings 5 Code'='',
			'Earnings 5 Amount'='',
			'Temp Rate'='',
			'Memo Code'=Contracts.MemoCode1,
			'Memo Amount'= cast(Contracts.MemoCode1Amount as varchar(20)),
			'Memo Code'=Contracts.MemoCode2,
			'Memo Amount'= cast(Contracts.MemoCode2Amount as varchar(20))
	from 
		dbo.PayPeriods 
		inner join dbo.E_Positions on PayPeriods.PayGroupCode=E_Positions.PayGroupId
		inner join dbo.Contracts on Contracts.Status1FlagCode=E_Positions.EmployeeTypeId
		inner join dbo.Employees on E_Positions.EmployeeId=Employees.EmployeeId
	where 
		Contracts.StartDate=PayPeriods.StartDate and
	    Contracts.EndDate=PayPeriods.EndDate and
	    E_Positions.EmployeeTypeId=1 and
	    Contracts.Status1FlagCode=1 and
	    Contracts.PayPeriod between 1 and 18
	end
	else if(@EmploymentType='9*12/19-26')
	begin
		select 	
			'CoCode'=Employees.CompanyCode,
			'BatchID'='RSI',
			'FileNumber'=E_Positions.FileNumber,
			'PayNumber'='',
			'RegHours'='',
			'OTHours'='',
			'Hours3Code'='',
			'Hours3Amount'='',
			'Hours4Code'='',
			'Hours4Amount'='',
			'RegEarnings'='',
			'OTEarnings'='',
			'Earnings3Code'='',
			'Earnings3Amount'='',
			'Earnings4Code'='',
			'Earnings4Amount'='',
			'Earnings5Code'='',
			'Earnings5Amount'='',
			'TempRate'='',
			'MemoCode1'='',
			'MemoAmount1'='',
			'MemoCode2'=Contracts.MemoCode2,
			'MemoAmount2'= cast(Contracts.MemoCode2Amount as varchar(20))
		from 
			dbo.PayPeriods 
			inner join dbo.E_Positions on PayPeriods.PayGroupCode=E_Positions.PayGroupId
			inner join dbo.Contracts on Contracts.Status1FlagCode=E_Positions.EmployeeTypeId
			inner join dbo.Employees on E_Positions.EmployeeId=Employees.EmployeeId
		where 
			Contracts.StartDate=PayPeriods.StartDate and
		    Contracts.EndDate=PayPeriods.EndDate and
		    E_Positions.EmployeeTypeId=1 and
		    Contracts.Status1FlagCode=1 and
		    Contracts.PayPeriod between 19 and 26
	end
end
go

