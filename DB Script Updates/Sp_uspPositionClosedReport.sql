go

if exists (select 1 from sys.objects where object_id = object_id(N'dbo.uspPositionClosedReport') and type in (N'P', N'PC'))
drop procedure dbo.uspPositionClosedReport
go

create procedure dbo.uspPositionClosedReport --2018  
@BudgetYear as int  
as

Select   
	distinct   
	PositionCode,
	Code, 
	PositionTitle,
	PositionStatus,
	JobCode, 
	JobTitle   
	BusinessUnitCode, 
	BusinessUnitTitle, 
	'' as Notes, 
	BudgetAmount  
from  
(  
	select 
	E_Positions.E_PositionId as ePosId,
	Positions.Code as Code, 
	Positions.PositionId as positionID, 
	E_Positions.PrimaryPosition, 
	positions.PositionCode as PositionCode,  
	positions.Title as PositionTitle,  
	PositionStatus = 'Closed',  
	'$'+CONVERT(Varchar (20),CONVERT( MONEY, COALESCE( ( Select top 1 posBudg.budgetAmount   
               From PositionBudgets posBudg    
               where posBudg.positionId = positions.PositionId and budgetYear=@BudgetYear   
               order by budgetYear desc), 0.0  
               )  
             )  
    ,1) as BudgetAmount,  
	PositionBusinessLevels.BusinessLevelCode as BusinessUnitCode,   
	PositionBusinessLevels.BusinessLevelTitle as  BusinessUnitTitle,   
	jobs.JobCode as JobCode,  
	jobs.title as jobTitle,  
	lastName as LastName, 
	firstName as FirstName, 
	ddlEMploymentStatuses.code as employeeStatus,   
	employees.hireDate as hireDate, 
	employees.companyCode as companyCode, 
	employees.FileNumber as filenumber ,  
	CAST(( Select top 1 esh.PayRate   
		From E_PositionSalaryHistories esh    
		where esh.E_PositionId = E_Positions.E_PositionId order by effectivedate desc) as varchar(20)) as Rate,
	CONVERT(Varchar(20), ( Select top 1 esh.effectiveDate   
	From E_PositionSalaryHistories esh    
	where esh.E_PositionId = E_Positions.E_PositionId order by effectivedate desc) , 101 ) as effectiveDate,   
	'' budgetFileGlCode,  
	'' as HoursPerPayPeriod,  
	'' as payFrequencyCode,  
	'' as HoursPerWeek   
from 
	dbo.positions  
	 inner join dbo.PositionBusinessLevels on PositionBusinessLevels.BusinessLevelNbr = positions.BusinessLevelNbr  
	 inner join dbo.jobs on jobs.JobId = positions.jobId  
	 left join dbo.E_Positions on E_Positions.positionid = positions.PositionId  
	 left join dbo.employees on employees.EmployeeId = E_Positions.EmployeeId  
	 left JOin dbo.persons on persons.PersonId = employees.personid  
	 left join dbo.ddlEmploymentStatuses on ddlEmploymentStatuses.EmploymentStatusId = employees.employmentstatusid  
where 
	PositionCode is not null   
	and positions.PositionId not in (  Select DISTINCT  p2.PositionId   
          FROM dbo.Positions p2  
           INNER JOIN dbo.E_Positions ep2 On eP2.positionID = P2.PositionId  
           INNER JOIN dbo.Employees e2 on E2.EmployeeId = eP2.employeeId  
           INNER JOIN dbo.ddlEmploymentStatuses es2 on es2.EmploymentStatusId = e2.employmentstatusid  
          WHERE 
		  p2.status! = 3)  
) x  
order by JobCode  
go