     
ALTER PROCEDURE [dbo].[sp_GetTimeCardSummaryAdministratorReportUnApproval]
	@CompCodeId int,                      
	@PayPeriodId int,
	@IsApproved bit
AS    
BEGIN  

	-- Query written by Sreekanth 
	--declare @compCodeId int = 1
	--declare @payPeriodId int = 33
	--declare @IsApproved bit = 0
	Select 
		Employees.EmployeeId as EmployeeId,
		per.FirstName + ' ' + per.LastName as EmployeeName,
		Employees.FileNumber,
		E_Positions.DepartmentId as DepartmentId,
		Departments.DepartmentCode as DepartmentCode,
		Departments.DepartmentDescription as DepartmentDescription,
		sum(TimeCards.DailyHours) as Total,
		TimeCards.IsApproved as IsApproved
	from TimeCards
	inner join E_Positions on Timecards.PositionId = E_Positions.PositionId and Timecards.EmployeeId = E_Positions.EmployeeId
	left outer join Persons man on E_Positions.ReportsToId = man.PersonId
	inner join Employees on TimeCards.EmployeeId = Employees.EmployeeId
	inner join Persons per on per.PersonId = Employees.PersonId
	inner join DdlPayFrequencies dp on Employees.PayFrequencyId = dp.PayFrequencyId
	inner join PayPeriods on PayPeriods.PayFrequencyId = dp.PayFrequencyId
	inner join Departments on Departments.DepartmentId = E_Positions.DepartmentId
	where 
		TimeCards.IsApproved = @IsApproved and 
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
	 order by EmployeeName, DepartmentCode 

-- Query written by Soma to get all approved or unapproved timecards for a pay period
   
--SELECT  
--t.EmployeeId,
--(SELECT Lastname+','+Firstname FROM PERSONS PR INNER JOIN Employees EM ON PR.PersonId = EM.PersonId WHERE EmployeeId = T.EMPLOYEEID) AS Name,
----t.ActualDate,
----t.DailyHours,
--e.FileNumber,
--d.DepartmentCode,d.DepartmentDescription,
--sum(Isnull(t.DailyHours,0)) as RegularHours,  
--t.isApproved,
--(pr.Firstname +' '+pr.LastName) as Manager,

--p.PayPeriodId
--from TimeCards t    
--inner join Employees e on t.EmployeeId=e.EmployeeId    
--inner join DdlPayFrequencies dp on e.PayFrequencyId = dp.PayFrequencyId
--inner join PayPeriods p on dp.PayFrequencyId = p.PayFrequencyId
--inner join E_Positions ep on e.EmployeeId=ep.EmployeeId  
----inner join Departments d  on ep.DepartmentId = d.DepartmentId
--inner join Departments d  on t.DepartmentId = d.DepartmentId
--inner join ManagerDepartments md on d.DepartmentId = md.DepartmentId
--inner join Persons pr on ep.ReportsToID = pr.PersonId
--where     
--t.CompanyCodeId = p.CompanyCodeId and t.EmployeeId = e.EmployeeId 
--								  and t.IsApproved = 0
--								  and t.ActualDate between p.StartDate and p.EndDate 
--								  and p.PayPeriodId = @payPeriodId     
								  
--and t.CompanyCodeId = @compCodeId
--and t.PositionId = ep.PositionId
--group by t.EmployeeId, t.IsApproved, ep.DepartmentId,pr.Firstname,pr.Lastname, d.DepartmentCode,d.DepartmentDescription,p.PayPeriodId,e.FileNumber
--ORDER BY T.EmployeeId
End
