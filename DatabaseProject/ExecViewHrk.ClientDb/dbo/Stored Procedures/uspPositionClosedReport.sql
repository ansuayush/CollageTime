-- [uspPositionClosedReport] 2017  
CREATE PROCEDURE [dbo].[uspPositionClosedReport]  
-- @Month as int,   
 @BudgetYear as int  
-- @StartDate as datetime ,  
-- @EndDate as datetime   
AS  
BEGIN  
  
Select   
distinct   
PositionCode, PositionTitle,PositionStatus
 , JobCode, JobTitle   
 --, LastName as [Last Name], FirstName as [First Name]  
 --, Division as [HRB Business Unit], EmployeeGLCode as [GL]  
 , BusinessUnitCode, BusinessUnitTitle 
 --, hireDate as [Hire Date], RateTypeCode as [Rate Type], Rate, HoursPerWeek as [Hours/Week], Classification  
 --, effectiveDate as [Effective Date]  
 , '' as Notes, BudgetAmount  
 --, AnnualSalary as [Annual Salary]  
 --, employeeStatus, companyCode, filenumber  
   
FROM  
(  
select E_Positions.E_PositionId as ePosId, Positions.PositionId as positionID, E_Positions.PrimaryPosition --'', '', ''   
 ,positions.PositionCode as PositionCode  
 ,positions.Title as PositionTitle  
 ,PositionStatus = 'Closed'  
-- ,PositionStatus = Case  
--       WHEN jobs.code in ('00640', '00641', '00642', '00663', '00664') THEN 'in use'  
--       ELSE 'open'  
--      END   
    
 ,'$'+CONVERT(Varchar (20),CONVERT( MONEY, COALESCE( ( Select top 1 posBudg.budgetAmount   
               From PositionBudgets posBudg    
               where posBudg.positionId = positions.PositionId and budgetYear=@BudgetYear   
               order by budgetYear desc), 0.0  
               )  
             )  
    ,1) as BudgetAmount  
 --,'$'+CONVERT(Varchar (20),CONVERT( MONEY, COALESCE(( Select sum(pbm.budgetAmount)   
 --              From PositionBudgets posBudg  
 --               inner join positionBudgetMonths pbm on pbm.PositionBudgetsId = PosBudg.id    
 --              where posBudg.positionId = positions.id and budgetYear = @BudgetYear and pbm.DisplayPosition <= @Month),0.0) ), 1) as BudgetToDate  
 --,'$'+CONVERT(Varchar (20),CONVERT( MONEY, COALESCE(( Select SUM(ActualPay)   
 --              FROM EmployeeActuals   
 --              where employeeActuals.PositionId = Positions.Id AND @StartDate <= PayPeriodDate AND payPeriodDate <= @EndDate), 0.0)), 1) as ActualPay  
   
   
   
 --,'$'+CONVERT(Varchar (20),CONVERT( MONEY, ((COALESCE(( Select sum(pbm.budgetAmount)   
 --              From PositionBudgets posBudg  
 --               inner join positionBudgetMonths pbm on pbm.PositionBudgetsId = PosBudg.id    
 --              where posBudg.positionId = positions.id and budgetYear = @BudgetYear and pbm.DisplayPosition <= @Month),0.0) ) )  
 --           - (  
 --             COALESCE(( Select SUM(ActualPay)   
 --              FROM EmployeeActuals   
 --              where employeeActuals.PositionId = Positions.Id AND @StartDate <= PayPeriodDate AND payPeriodDate <= @EndDate), 0.0)  
 --            )  
 --          ), 1) as Variance  
 --,'$'+CONVERT(Varchar (20),CONVERT( MONEY, COALESCE(( Select SUM(Overtime)   
 --              FROM EmployeeActuals   
 --              where employeeActuals.PositionId = Positions.Id), 0.0)), 1) as Overtime   
 --,hrBusinessLevels.GlCode  
 ,PositionBusinessLevels.BusinessLevelCode as BusinessUnitCode   
 ,PositionBusinessLevels.BusinessLevelTitle as  BusinessUnitTitle   
 ,jobs.JobCode as JobCode  
 ,jobs.title as jobTitle  
 ,lastName as LastName, firstName as FirstName, ddlEMploymentStatuses.code as employeeStatus   
 ,employees.hireDate as hireDate, employees.companyCode as companyCode, employees.FileNumber as filenumber   
 --,( Select top 1 esh.RateTypeCode   
 -- From E_PositionSalaryHistories esh    
 -- where esh.E_PositionId = E_Positions.E_PositionId order by effectivedate desc) as RateTypeCode  
 ,CAST(( Select top 1 esh.PayRate   
  From E_PositionSalaryHistories esh    
  where esh.E_PositionId = E_Positions.E_PositionId order by effectivedate desc) as varchar(20)) as Rate  
 --,'$'+CONVERT(VARCHAR (20),CONVERT(MONEY, ( Select top 1 esh.annualSalary   
 --          From E_PositionSalaryHistories esh    
 --          where esh.E_PositionId = E_Positions.E_PositionId  and effectiveDate <= '6/30/'+CONVERT(varchar(4),@BudgetYear)  
 --          order by effectivedate desc)),1) as AnnualSalary   
 ,CONVERT(Varchar(20), ( Select top 1 esh.effectiveDate   
  From E_PositionSalaryHistories esh    
  where esh.E_PositionId = E_Positions.E_PositionId order by effectivedate desc) , 101 ) as effectiveDate   
 --,( Select top 1 ecg.name   
 -- From employeeCorpGroup ecg  
 -- where ecg.type='class'   
 --  and ecg.startDate <= ( Select top 1 esh.effectiveDate   
 --        From E_PositionSalaryHistories esh    
 --        where esh.epositionId = epositions.id  and effectiveDate <= '6/30/'+CONVERT(varchar(4),@BudgetYear)  
 --        order by effectivedate desc)  
 --  and ecg.companyCode = employees.companycode and    
 --  ecg.filenumber = employees.filenumber order by ecg.startdate desc) as Classification   
 --,( Select top 1 ecg.name   
 -- From employeeCorpGroup ecg  
 -- where ecg.type='division'   
 --  and ecg.startDate <= ( Select top 1 esh.effectiveDate   
 --        From ePositionSalaryHistory esh    
 --        where esh.epositionId = epositions.id  and effectiveDate <= '6/30/'+CONVERT(varchar(4),@BudgetYear)  
 --        order by effectivedate desc)  
 --  and ecg.companyCode = employees.companycode and    
 --  ecg.filenumber = employees.filenumber order by ecg.startdate desc  
 -- ) as Division   
 --,( Select top 1 corpGroup.code  
 --  From CorpGroup   
 --   Inner Join EmployeeCorpGroup ecg on ecg.type = corpGroup.Type and ecg.Name = corpGroup.Name  
 --  where corpgroup.type='division'   
 --   and ecg.startDate <= ( Select top 1 esh.effectiveDate   
 --         From ePositionSalaryHistory esh    
 --         where esh.epositionId = epositions.id  and effectiveDate <= '6/30/'+CONVERT(varchar(4),@BudgetYear)  
 --         order by effectivedate desc)  
 --   and ecg.companyCode = employees.companycode     
 --   and ecg.filenumber = employees.filenumber order by ecg.startdate desc  
 -- ) as EmployeeGlCode  
 , '' budgetFileGlCode  
 ,'' as HoursPerPayPeriod  
 ,'' as payFrequencyCode  
 ,'' as HoursPerWeek    
from positions  
 inner join PositionBusinessLevels on PositionBusinessLevels.BusinessLevelNbr = positions.BusinessLevelNbr  
 inner join jobs on jobs.JobId = positions.jobId  
 left join E_Positions on E_Positions.positionid = positions.PositionId  
 left join employees on employees.EmployeeId = E_Positions.EmployeeId  
 left JOin persons on persons.PersonId = employees.personid  
 left join ddlEmploymentStatuses on ddlEmploymentStatuses.EmploymentStatusId = employees.employmentstatusid  
where PositionCode is not null   
 and positions.PositionId not in (  Select DISTINCT  p2.PositionId   
          FROM Positions p2  
           INNER JOIN E_Positions ep2 On eP2.positionID = P2.PositionId  
           INNER JOIN Employees e2 on E2.EmployeeId = eP2.employeeId  
           INNER JOIN ddlEmploymentStatuses es2 on es2.EmploymentStatusId = e2.employmentstatusid  
          WHERE eP2.actualEndDate IS NULL and es2.code = 'A' )  
 --AND jobs.code NOT IN ('00640', '00641', '00642', '00663', '00664')  
 --AND (jobs.code in (Select jobs.code from employeeJobs) OR createdInExecView = 1)  
 --AND PositionBusinessLevels.title <> 'xxx'  
 AND Positions.status = 3  
 --and (Positions.CreatedFromActuals is null OR Positions.CreatedFromActuals = 0)  
) x  
order by JobCode  
  
END  
  