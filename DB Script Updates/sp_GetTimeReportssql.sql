
go
if  exists (select 1 from sys.objects where object_id = object_id(N'dbo.GetTimeReports') and type in (N'P', N'PC'))
drop procedure dbo.GetTimeReports
go

create procedure dbo.GetTimeReports
@startdate datetime ,
@enddate datetime
as 
begin
declare @ActualStartDate datetime;
declare @Actualenddate datetime;
set @ActualStartDate=(CONVERT(date, @startdate))
set @Actualenddate=(CONVERT(date, @enddate))

select emp.EmployeeId, per.Firstname, per.Lastname,per.eMail, emp.FileNumber, emp.IsStudent,
tc.ActualDate, tc.TimeIn, tc.TimeOut,tc.LunchOut,tc.LunchBack,IsApproved, tc.UserId, tc.LastModifiedDate,ps.PositionDescription,DailyHours=isnull(tc.DailyHours,0),
(select p.eMail from persons p 
inner join employees e on p.PersonId = e.PersonId
where e.EmployeeId=eps.ReportstoID) as Supervisor,
dp.DepartmentCode as HomeDepartmentCode,
dp.DepartmentDescription as HomeDepartmentDescription,
(pm.Firstname +' ' + pm.Lastname) AS ManagerName
from persons per
inner join employees emp on per.personId = emp.personId
inner join timecards tc on emp.employeeId = tc.employeeId
inner join E_Positions eps on emp. employeeId = eps.EmployeeId and tc.PositionId = eps.PositionId
inner join departments dp on dp.DepartmentId = eps.DepartmentId
inner join Positions ps on ps.PositionId = eps.PositionId 
left outer join persons pm on eps.ReportsToID =pm.PersonId 
where CONVERT(date,tc.ActualDate) >= @ActualStartDate and CONVERT(date,tc.ActualDate) <=@Actualenddate 
order by emp.employeeId, ActualDate
end

go
