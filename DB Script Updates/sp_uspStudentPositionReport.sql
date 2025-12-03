go

if exists (select 1 from sys.objects where object_id = object_id(N'dbo.uspStudentPositionReport') and type in (N'P', N'PC'))
drop procedure dbo.uspStudentPositionReport
go

create procedure dbo.uspStudentPositionReport
as
/*
	Changes
	Date		Author		Ticket		Details
	20-Aug-18	Chandu		DU-2661		Add suffix to PositionCode and proc format changes
*/
set nocount on;
select 
	emp.EmployeeId, 
	per.Firstname, 
	per.Lastname, 
	per.SSN, 
	per.eMail, 
	emp.FileNumber, 
	po.PositionId, 
	po.PositionDescription, 
	PositionCode = po.PositionCode+isnull(po.Suffix,'00'),
	HomeDepartmentCode =dp.DepartmentCode,
	HomeDepartmentDescription = dp.DepartmentDescription,
	jo.JobCode,
	Supervisor = (select top 1 p.LastName+','+p.FirstName+'    '+eMail from persons p 
	inner join employees e on p.PersonId = e.PersonId
	where e.EmployeeId=eps.ReportstoID),
	TotalTimes = (select sum(DailyHours) from timecards
	where EmployeeId = emp.employeeId and PositionId = po.PositionId
	Group by EmployeeId, PositionId),
	PositionStartDate =eps.Startdate, 
	PositionEndDate = eps.actualEndDate
from 
	dbo.Persons per
	inner join dbo.Employees emp on per.personId = emp.personId
	inner join dbo.E_Positions eps on emp. employeeId = eps.EmployeeId
	inner join dbo.Positions po on po.PositionId = eps.PositionId
	inner join dbo.jobs jo on po.JobId= jo.JobId
	inner join dbo.Departments dp on dp.DepartmentId = eps.DepartmentId
where  
	emp.IsStudent =1 and 
	convert(date,isnull(eps.actualEndDate,GetDate()))>=convert(date,GetDate())
order by 
	emp.employeeId
go

