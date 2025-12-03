
go

if exists (select 1 from sys.objects where object_id = object_id(N'dbo.uspEmployeePositionReportNoBudgetYear') and type in (N'P', N'PC'))
drop procedure dbo.uspEmployeePositionReportNoBudgetYear
go

create procedure dbo.uspEmployeePositionReportNoBudgetYear
as 
/*
	Changes
	Date		Author		Ticket		Details
	20-Aug-18	Chandu		DU-2661		Add suffix to PositionCode and proc format changes
*/
set nocount on;      

select 
	distinct EmployeeName = lastname+', '+ firstname, 
	persons.PersonID  ,
	employees.companyCode, 
	employees.filenumber, 
	[Status] = ddlEmploymentStatuses.code,  
	PositionCode = positions.PositionCode+isnull(positions.Suffix,'00'), 
	positionTitle = positions.Title,
	BuTitle = PositionBusinessLevels.BusinessLevelTitle, 
	BuCode = PositionBusinessLevels.BusinessLevelCode, 
	jobCode = jobs.JobCode, 
	JobTitle = jobs.title,
	E_Positions.primaryPosition, 
	E_Positions.StartDate, 
	E_Positions.actualEndDate, 
	PayFrequencyCode = DdlPayFrequencies.Code,
	E_Positions.EnteredDate,
	epsh.PayRate, 
	epsh.HoursPerPayPeriod,
	epsh.EffectiveDate ,
	epsh.AnnualSalary
from 
	dbo.Persons   
	inner join dbo.Employees on employees.personID = persons.PersonId  
	inner join dbo.E_Positions on E_Positions.employeeID = employees.EmployeeId  
	inner join dbo.positions on positions.PositionId = E_Positions.positionid  
	inner join dbo.PositionBusinessLevels on PositionBusinessLevels.BusinessLevelNbr = positions.BusinessLevelNbr  
	inner join dbo.jobs on jobs.Jobid = positions.jobID  
	left join dbo.ddlEmploymentStatuses on ddlEmploymentStatuses.EmploymentStatusId = employees.employmentStatusID  
	left join dbo.E_PositionSalaryHistories epsh on epsh.E_PositionId = E_Positions.E_PositionId 
	left join dbo.DdlPayFrequencies on DdlPayFrequencies.PayFrequencyId = Employees.PayFrequencyId
go

