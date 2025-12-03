CREATE PROCEDURE [dbo].[uspPositionReportNoBudgetYear]  
AS BEGIN   
SET NOCOUNT ON;         
select positions.PositionCode as PositionCode, positions.Title as positionTitle     
,PositionBusinessLevels.BusinessLevelTitle as BuTitle, PositionBusinessLevels.BusinessLevelCode as BuCode,   
jobs.JobCode as jobCode, jobs.title as JobTitle  ,  
lastname+', '+ firstname as EmployeeName,-- persons.ADP_PersonID  ,  
employees.CompanyCode, employees.FileNumber,   
ddlEmploymentStatuses.code as Status  ,  
E_Positions.PrimaryPosition, E_Positions.StartDate,   
E_Positions.actualEndDate  --,E_Positions.rateTypeCode,E_Positions.ImportedDate     
,DdlPayFrequencies.Code as PayFrequencyCode, E_Positions.EnteredDate,   
epsh.PayRate, --epsh.RateTypeCode,   
epsh.EffectiveDate--, epsh.AnnualSalary   
,AnnualSalary=(CASE DdlPayFrequencies.Code  
        WHEN  'BW' THEN ( 52 * epsh.HoursPerPayPeriod * epsh.PayRate)  
        WHEN 'W' THEN ( 26 * epsh.HoursPerPayPeriod * epsh.PayRate)  
  WHEN  'SM' THEN ( 24 * epsh.HoursPerPayPeriod * epsh.PayRate)  
     WHEN  'M' THEN ( 12 * epsh.HoursPerPayPeriod * epsh.PayRate)  
  WHEN  'Quarterly' THEN ( 4 * epsh.HoursPerPayPeriod * epsh.PayRate)  
   WHEN  'Year' THEN ( 1 * epsh.HoursPerPayPeriod * epsh.PayRate)  
  else null  
    END)   
from Positions    
left join PositionBusinessLevels on PositionBusinessLevels.BusinessLevelNbr = positions.BusinessLevelNbr    
left join jobs on jobs.JobId = positions.jobID     
left join E_Positions on E_Positions.PositionId = Positions.PositionId    
left join Employees on employees.EmployeeId = E_Positions.EmployeeId    
left join Persons on persons.PersonId = employees.PersonId    
left join ddlEmploymentStatuses on ddlEmploymentStatuses.EmploymentStatusId = employees.EmploymentStatusId    
left join E_PositionSalaryHistories epsh on epsh.E_PositionId = E_Positions.E_PositionId   
left join DdlPayFrequencies on DdlPayFrequencies.PayFrequencyId = Employees.PayFrequencyId  
order by Positions.Code    
END  