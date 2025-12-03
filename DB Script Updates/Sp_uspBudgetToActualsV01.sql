

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('uspBudgetToActualsV01'))
BEGIN
    DROP PROCEDURE [uspBudgetToActualsV01]
END
GO 
create PROCEDURE [dbo].[uspBudgetToActualsV01] 
	-- Add the parameters for the stored procedure here
	@BudgetYear as Integer,
	@Month as Int
    --@StartDate as DateTime,
    --@EndDate as DateTime
AS
BEGIN

SELECT DISTINCT PositionCode,Code,PositionTitle,PositionStatus,
IsNull(BudgetAmount,0) AS BudgetAmount,
IsNull(BudgetToDate,0) AS BudgetToDate,
IsNull(ActualPay,0)  AS ActualPay,
(IsNull(BudgetToDate,0)-ActualPay) AS Variance,
BusinessUnitCode,BusinessUnitTitle,JobCode,JobTitle
 FROM ( 

SELECT 
CASE WHEN E_Positions.PrimaryPosition IS NULL THEN 0 ELSE E_Positions.PrimaryPosition END AS PrimaryPosition
,positions.PositionCode as PositionCode ,Positions.Code as Code
,positions.Title as PositionTitle 
,PositionStatus = 'In Use' 
,CONVERT( MONEY, (Select SUM(posBudg.budgetAmount) From PositionBudgets posBudg  where posBudg.positionId = positions.PositionId and budgetYear = @BudgetYear  ) )  as BudgetAmount 
,CONVERT( MONEY, (Select SUM(posBudg.budgetAmount) From PositionBudgets posBudg  where posBudg.positionId = positions.PositionId and budgetYear = @BudgetYear and posBudg.BudgetMonth between 1 and @Month ) ) as  BudgetToDate
,0.00 as ActualPay
,PositionBusinessLevels.BusinessLevelCode as BusinessUnitCode 
,PositionBusinessLevels.BusinessLevelTitle as  BusinessUnitTitle 
,jobs.JobCode as JobCode 
,jobs.title as jobTitle
from positions 
inner join PositionBusinessLevels on PositionBusinessLevels.BusinessLevelNbr = positions.BusinessLevelNbr
inner join jobs on jobs.JobId = positions.jobId 
left join E_Positions on E_Positions.positionid = positions.PositionId 
left join employees on employees.EmployeeId = E_Positions.EmployeeId 
left JOin persons on persons.PersonId = employees.personid 
left join ddlEmploymentStatuses on ddlEmploymentStatuses.EmploymentStatusId = employees.employmentstatusid 
            where positions.PositionCode is not null and ddlEmploymentStatuses.code = 'A' and E_Positions.ActualEndDate is Null 


            UNION ALL 

SELECT
CASE WHEN E_Positions.PrimaryPosition IS NULL THEN 0 ELSE E_Positions.PrimaryPosition END AS PrimaryPosition
,positions.PositionCode as PositionCode,Positions.Code as Code
,positions.Title as PositionTitle 
,PositionStatus =Case 
						WHEN jobs.JobCode in ('00640', '00641', '00642', '00663', '00664') THEN 'In Use' 
						ELSE 'Open' 
					END              		
,CONVERT( MONEY, (Select SUM(posBudg.budgetAmount) From PositionBudgets posBudg  where posBudg.positionId = positions.PositionId and budgetYear = @BudgetYear  ) )  as BudgetAmount 
,CONVERT( MONEY, (Select SUM(posBudg.budgetAmount) From PositionBudgets posBudg  where posBudg.positionId = positions.PositionId and budgetYear = @BudgetYear and posBudg.BudgetMonth between 1 and @Month ) ) as  BudgetToDate
,0.00 as ActualPay
,PositionBusinessLevels.BusinessLevelCode as BusinessUnitCode 
,PositionBusinessLevels.BusinessLevelTitle as  BusinessUnitTitle 
,jobs.JobCode as JobCode 
,jobs.title as jobTitle
from positions 
inner join PositionBusinessLevels on PositionBusinessLevels.BusinessLevelNbr = positions.BusinessLevelNbr
inner join jobs on jobs.JobId = positions.jobId 
left join E_Positions on E_Positions.positionid = positions.PositionId 
left join employees on employees.EmployeeId = E_Positions.EmployeeId 
left JOin persons on persons.PersonId = employees.personid 
left join ddlEmploymentStatuses on ddlEmploymentStatuses.EmploymentStatusId = employees.employmentstatusid 
            where positions.PositionCode is not null and positions.PositionId not in (  Select p2.PositionId  
									FROM Positions p2 
										INNER JOIN E_Positions ep2 On eP2.positionID = P2.PositionId 
										INNER JOIN Employees e2 on E2.EmployeeId = eP2.employeeId 
										INNER JOIN ddlEmploymentStatuses es2 on es2.EmploymentStatusId = e2.employmentstatusid 
									WHERE eP2.actualEndDate IS NULL and es2.code = 'A' ) 
           

) t



END
GO

