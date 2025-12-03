IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('uspContractsReport'))
BEGIN
    DROP PROCEDURE uspContractsReport
END
GO
CREATE PROCEDURE [dbo].[uspContractsReport]         
AS        
BEGIN    
declare @AmountToBePaid as decimal    
declare @NewAmountToBePaid as decimal   
declare @ExportedAmount as decimal   
declare @RemainderDue as decimal   
SELECT     
EmployeeName,BusinessUnitTitle,GLCode,Semester,StartDate,    
EndDate,Amount,PayPeriods,AmountToBePaid,NewAmountToBePaid,    
ExportedAmount,RemainderDue,PayPeriodsRemaining,    
CASE     
WHEN CONVERT(VARCHAR,ROW%4) = '1' THEN 'STP'    
WHEN CONVERT(VARCHAR,ROW%4) = '2' THEN 'ADS'    
WHEN CONVERT(VARCHAR,ROW%4) = '3' THEN 'AS3'    
ELSE 'ADF'    
END AS EarningCodes,    
Notes,JobCode    
    
 FROM (      
select DISTINCT     
ROW_NUMBER() OVER(ORDER BY JobCode ASC) AS Row,    
Lastname +', '+ Firstname as EmployeeName ,PositionBusinessLevels.BusinessLevelTitle as  BusinessUnitTitle,        
--'' as GLCode,'' as Semester,'' as StartDate, '' as EndDate, '' as Amount,        
--'' as PayPeriods ,''as AmountToBePaid ,'' as NewAmountToBePaid,         
--'' as ExportedAmount,''as RemainderDue,'' as PayPeriodsRemaining,        
--'' as EarningCodes, '' as Notes    
-------Temporary Logic    
'91021008' as GLCode    
,'CONTRA' as Semester,    
CONVERT(VARCHAR, ISNULL(HireDate,''), 101) as StartDate,    
CASE WHEN TerminationDate IS NULL THEN ''    
ELSE CONVERT(VARCHAR, ISNULL(TerminationDate,''), 101) END as EndDate,     
CONVERT(decimal,ISNULL(A.Amount,0.00)) as Amount    
,'' as PayPeriods     
--,''as AmountToBePaid     
,CONVERT(decimal,ISNULL(@AmountToBePaid,0.00)) as AmountToBePaid   
--,'' as NewAmountToBePaid   
,CONVERT(decimal,ISNULL(@NewAmountToBePaid,0.00)) as NewAmountToBePaid   
--,'' as ExportedAmount    
,CONVERT(decimal,ISNULL(@ExportedAmount,0.00)) as ExportedAmount   
--,''as RemainderDue    
  
,CONVERT(decimal,ISNULL(@RemainderDue,0.00)) as RemainderDue   
,'0' as PayPeriodsRemaining    
,'' as EarningCodes    
,'' as Notes    
    
    
,JobCode       
        
from positions        
 inner join PositionBusinessLevels on PositionBusinessLevels.BusinessLevelNbr = positions.BusinessLevelNbr        
 inner join jobs on jobs.JobId = positions.jobId        
 left join E_Positions on E_Positions.positionid = positions.PositionId        
 left join employees on employees.EmployeeId = E_Positions.EmployeeId        
 left JOin persons on persons.PersonId = employees.personid        
 left join ddlEmploymentStatuses on ddlEmploymentStatuses.EmploymentStatusId = employees.employmentstatusid        
 LEFT JOIN (select PositionID, SUM(BudgetAmount) AS Amount  from positionBudgets GROUP BY PositionID)A  on A.PositionId = positions.PositionId    
    
where PositionCode is not null and ddlEmploymentStatuses.code = 'A'         
 and E_Positions.ActualEndDate is Null         
    
)t order by t.JobCode       
END     
    