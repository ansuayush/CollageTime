IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('uspPositionInUseReport'))
BEGIN
    DROP PROCEDURE [uspPositionInUseReport]
END
GO

create PROCEDURE [dbo].[uspPositionInUseReport] --2018
 --@Month as int,   
 @BudgetYear as int  
 --@StartDate as datetime ,  
 --@EndDate as datetime   
AS  
BEGIN  
 SET NOCOUNT ON;  
 DECLARE @FiscalMonth as varchar(2)
 SELECT @FiscalMonth= cc.ConfigurationValue FROM [dbo].[ClientConfiguration] as cc where ConfigurationName = 'Fiscal Month' 

 Select  DISTINCT
    Code,PositionCode, PositionTitle,PositionStatus  
 , JobCode, JobTitle   
 , LastName, FirstName  
 , Division 
 , EmployeeGLCode 
 , BusinessUnitCode, BusinessUnitTitle  
 , hireDate
 , RateTypeCode
,CONVERT(decimal,isnull(Rate,0.00)) as Rate  
 , HoursPerWeek
 --, Classification  
 , effectiveDate, '' as Notes, 
 BudgetAmount
, CONVERT(decimal,isnull(AnnualSalary,0.00))  AS AnnualSalary 
 , employeeStatus, companyCode, filenumber  
 , EPositionStartDate  
   
FROM  
(  
select E_Positions.E_PositionId as ePosId, Positions.PositionId as positionID, E_Positions.PrimaryPosition  
 ,PositionCode ,positions.Code 
 ,positions.Title as PositionTitle  
 ,PositionStatus = 'In Use'  
 ,'$'+CONVERT(Varchar (20),CONVERT( MONEY, COALESCE( ( Select top 1 posBudg.budgetAmount   
               From PositionBudgets posBudg    
               where posBudg.positionId = positions.PositionId and budgetYear = @BudgetYear  
               order by budgetYear desc), 0.0  
               )  
             )  
    ,1) as BudgetAmount  
 --,'$'+CONVERT(Varchar (20),CONVERT( MONEY, COALESCE(( Select sum(pbm.budgetAmount)   
               --From PositionBudgets posBudg  
               --inner join positionBudgetMonths pbm on pbm.PositionBudgetsId = PosBudg.id    
               --where posBudg.positionId = positions.PositionId and budgetYear = @BudgetYear /*and pbm.DisplayPosition <= @Month*/),0.0) ), 1) as BudgetToDate  
   
 ,--'$'+CONVERT(Varchar (20),CONVERT( MONEY, COALESCE(( Select SUM(ActualPay)   
               --FROM EmployeeActuals   
               --where employeeActuals.PositionId = Positions.Id AND @StartDate <= PayPeriodDate AND payPeriodDate <= @EndDate), 0.0)), 1) as ActualPay  
   
   
   
 --,'$'+CONVERT(Varchar (20),CONVERT( MONEY, ((COALESCE(( Select sum(pbm.budgetAmount)   
 --              From PositionBudgets posBudg  
 --                 --inner join positionBudgetMonths pbm on pbm.PositionBudgetsId = PosBudg.id    
 --              where posBudg.positionId = positions.id and budgetYear = @BudgetYear /*and pbm.DisplayPosition <= @Month*/),0.0) ) )  
 --           - (  
 --             COALESCE(( Select SUM(ActualPay)   
 --              FROM EmployeeActuals   
 --              where employeeActuals.PositionId = Positions.Id AND @StartDate <= PayPeriodDate AND payPeriodDate <= @EndDate), 0.0)  
 --            )  
 --          ), 1) as Variance  
   
 --,'$'+CONVERT(Varchar (20),CONVERT( MONEY, COALESCE(( Select SUM(Overtime)   
 --              FROM EmployeeActuals   
 --              where employeeActuals.PositionId = Positions.Id), 0.0)), 1) as Overtime   
 --,PositionBusinessLevels.GlCode  
 departments.DepartmentCode as BusinessUnitCode   
 ,departments.DepartmentDescription as BusinessUnitTitle   

 ,jobs.JobCode as JobCode  
 ,jobs.title as jobTitle  
 ,LastName, firstName, ddlEMploymentStatuses.code as employeeStatus   
 ,convert(varchar(10), hireDate, 101) as hireDate, employees.companycode, employees.filenumber   
 ,( Select top 1 A.Code FROM DdlRateTypes A INNER JOIN  E_PositionSalaryHistories B  ON A.RateTypeId = B.RateTypeId) as RateTypeCode 
 ,CAST(( Select top 1 esh.PayRate   
  From E_PositionSalaryHistories esh    
  where esh.E_PositionId = E_Positions.E_PositionId and effectiveDate <= ''+@FiscalMonth+'/30/'+CONVERT(varchar(4),@BudgetYear)  
  order by effectivedate desc) as varchar(20)) as Rate  
,(CASE DdlPayFrequencies.Code  
        WHEN  'BW' THEN ( 52 * epsh.HoursPerPayPeriod * epsh.PayRate)  
        WHEN 'W' THEN ( 26 * epsh.HoursPerPayPeriod * epsh.PayRate)  
  WHEN  'SM' THEN ( 24 * epsh.HoursPerPayPeriod * epsh.PayRate)  
     WHEN  'M' THEN ( 12 * epsh.HoursPerPayPeriod * epsh.PayRate)  
  WHEN  'Quarterly' THEN ( 4 * epsh.HoursPerPayPeriod * epsh.PayRate)  
   WHEN  'Year' THEN ( 1 * epsh.HoursPerPayPeriod * epsh.PayRate)  
  else null  
    END) AS AnnualSalary


   
 ,CONVERT(Varchar(20), ( Select top 1 esh.effectiveDate   
  From E_PositionSalaryHistories esh    
  where esh.E_PositionId = E_Positions.E_PositionId  and effectiveDate <= ''+@FiscalMonth+'/30/'+CONVERT(varchar(4),@BudgetYear)  
  order by effectivedate desc) , 101 ) as effectiveDate   
 --,( Select top 1 ecg.name   
 -- From employeeCorpGroup ecg  
 -- where ecg.type='class'   
 --  and ecg.startDate <= ( Select top 1 esh.effectiveDate   
 --        From E_PositionSalaryHistories esh    
 --        where esh.epositionId = E_Positions.id  and effectiveDate <= '6/30/'+CONVERT(varchar(4),@BudgetYear)  
 --        order by effectivedate desc)  
 --  and ecg.companyCode = employees.companycode and    
 --  ecg.filenumber = employees.filenumber order by ecg.startdate desc) as Classification   
 --,( Select top 1 ecg.name   
 -- From employeeCorpGroup ecg  
 -- where ecg.type='division'   
 --  and ecg.startDate <= ( Select top 1 esh.effectiveDate   
 --        From E_PositionSalaryHistories esh    
 --        where esh.epositionId = E_Positions.id  and effectiveDate <= '6/30/'+CONVERT(varchar(4),@BudgetYear)  
 --        order by effectivedate desc)  
 --  and ecg.companyCode = employees.companycode and    
 --  ecg.filenumber = employees.filenumber order by ecg.startdate desc  
 -- ) as Division   
 --,( Select top 1 corpGroup.code  
 --  From CorpGroup   
 --   Inner Join EmployeeCorpGroup ecg on ecg.type = corpGroup.Type and ecg.Name = corpGroup.Name  
 --  where corpgroup.type='division'   
 --   and ecg.startDate <= ( Select top 1 esh.effectiveDate   
 --         From E_PositionSalaryHistories esh    
 --         where esh.epositionId = E_Positions.id and effectiveDate <= '6/30/'+CONVERT(varchar(4),@BudgetYear)  
 --          order by effectivedate desc)  
 --   and ecg.companyCode = employees.companycode     
 --   and ecg.filenumber = employees.filenumber order by ecg.startdate desc  
 -- ) as EmployeeGlCode  
 --,PositionBusinessLevels.GlCode as BudgetFileGlCode  
 ,CONVERT(Varchar(10), ( Select top 1 esh.hoursPerPayPeriod   
  From E_PositionSalaryHistories esh    
  where esh.E_PositionId = E_Positions.E_PositionId  and effectiveDate <= ''+@FiscalMonth+'/30/'+CONVERT(varchar(4),@BudgetYear)   
  order by effectivedate desc)) as hoursPerPayPeriod  
 , E_Positions.payFrequencyCode   
 , CONVERT(Varchar(10), employees.hours) as hoursPerWeek  
 --, createdInExecView   
 ,E_Positions.startDate as EPositionStartDate   
 ,PositionBusinessLevels.BusinessLevelTitle AS Division
 ,'' as EmployeeGLCode                      
from positions  
 inner join PositionBusinessLevels on PositionBusinessLevels.BusinessLevelNbr = positions.BusinessLevelNbr  
 inner join jobs on jobs.JobId = positions.jobId  
 left join E_Positions on E_Positions.positionid = positions.PositionId  
 left join employees on employees.EmployeeId = E_Positions.EmployeeId  
 left JOin persons on persons.PersonId = employees.personid  
 left join ddlEmploymentStatuses on ddlEmploymentStatuses.EmploymentStatusId = employees.employmentstatusid  
 left join E_PositionSalaryHistories epsh on epsh.E_PositionId = E_Positions.E_PositionId  
 left join DdlPayFrequencies on DdlPayFrequencies.PayFrequencyId = Employees.PayFrequencyId  
 left join departments on departments.DepartmentId = positions.DepartmentId

where PositionCode is not null and ddlEmploymentStatuses.code = 'A'   
 and E_Positions.ActualEndDate is Null   
 --and E_Positions.PrimaryPosition = 1  
 and E_Positions.StartDate <= ''+@FiscalMonth+'/30/'+CONVERT(varchar(4),@BudgetYear)  
 --and (Positions.CreatedFromActuals is null OR Positions.CreatedFromActuals = 0)  
) x  
--WHERE (jobcode in (Select jobcode from employeeJobs) OR createdInExecView = 1)  
-- and JobCode not in ('000640', '000641', '00642', '000663', '00664')  
order by JobCode  
END
GO


