
/****** Object:  StoredProcedure [dbo].[uspEmployeePositionReportNoBudgetYear]    Script Date: 11/2/2017 2:54:50 PM ******/

Create PROCEDURE [dbo].[uspEmployeePositionReportNoBudgetYear]   
   
AS BEGIN  
-- SET NOCOUNT ON added to prevent extra result sets from  
-- interfering with SELECT statements.  
SET NOCOUNT ON;      
-- Insert statements for procedure here  
select distinct  lastname+', '+ firstname as EmployeeName, persons.PersonID  ,
employees.companyCode, employees.filenumber, ddlEmploymentStatuses.code as Status  
,positions.PositionCode as PositionCode, positions.Title as positionTitle   ,
PositionBusinessLevels.BusinessLevelTitle as BuTitle, PositionBusinessLevels.BusinessLevelCode as BuCode, jobs.JobCode as jobCode, jobs.title as JobTitle ,E_Positions.primaryPosition 
, E_Positions.StartDate, E_Positions.actualEndDate  --,E_Positions.rateTypeCode  
,DdlPayFrequencies.Code as PayFrequencyCode
, E_Positions.EnteredDate--, E_Positions.ImportedDate 
 ,epsh.PayRate , epsh.HoursPerPayPeriod
, epsh.EffectiveDate ,
--null as AnnualSalary
AnnualSalary=(CASE DdlPayFrequencies.Code
        WHEN  'BW' THEN ( 52 * epsh.HoursPerPayPeriod * epsh.PayRate)
        WHEN 'W' THEN ( 26 * epsh.HoursPerPayPeriod * epsh.PayRate)
		WHEN  'SM' THEN ( 24 * epsh.HoursPerPayPeriod * epsh.PayRate)
	    WHEN  'M' THEN ( 12 * epsh.HoursPerPayPeriod * epsh.PayRate)
		WHEN  'Quarterly' THEN ( 4 * epsh.HoursPerPayPeriod * epsh.PayRate)
		 WHEN  'Year' THEN ( 1 * epsh.HoursPerPayPeriod * epsh.PayRate)
		else null
    END)  
	
from Persons   
inner join Employees on employees.personID = persons.PersonId  
left join E_Positions on E_Positions.employeeID = employees.EmployeeId  
left join positions on positions.PositionId = E_Positions.positionid  
left join PositionBusinessLevels on PositionBusinessLevels.BusinessLevelNbr = positions.BusinessLevelNbr  
left join jobs on jobs.Jobid = positions.jobID  
left join ddlEmploymentStatuses on ddlEmploymentStatuses.EmploymentStatusId = employees.employmentStatusID  
left join E_PositionSalaryHistories epsh on epsh.E_PositionId = E_Positions.E_PositionId 
left join DdlPayFrequencies on DdlPayFrequencies.PayFrequencyId = Employees.PayFrequencyId
--order by lastname, firstname
  END
