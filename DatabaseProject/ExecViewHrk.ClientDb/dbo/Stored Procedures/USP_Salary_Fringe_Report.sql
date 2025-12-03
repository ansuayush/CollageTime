CREATE PROC USP_Salary_Fringe_Report
AS
BEGIN

SELECT DISTINCT * FROM(
select 

 PositionBusinessLevels.BusinessLevelCode AS 'BudgetCenter'
,PositionBusinessLevels.BusinessLevelTitle AS 'BudgetCenterDescription'
,Funds.code AS 'FundingSource'
,Funds.Description AS 'FundingSourceDescription'
,(Persons.Firstname + ' ' + Persons.Lastname) AS 'Resource'
,Funds.EffectiveStartDate AS 'PeriodStart'
,Funds.EffectiveEndDate AS 'PeriodEnd'
,0.00 AS 'Hours'
,0.00 AS 'Salary'
,0.00 AS 'FICA'
,0.00 AS '401K'
,0.00 AS 'Medical'
,0.00 AS 'Dental'
,0.00 AS '403b'
,0.00 AS 'LifeIns'
,0.00 AS 'Disability'
,0.00 AS 'TotalEmployerCost'
FROM positions
INNER JOIN PositionBusinessLevels ON PositionBusinessLevels.BusinessLevelNbr = positions.BusinessLevelNbr
INNER JOIN PositionBudgets ON positions.PositionID = PositionBudgets.PositionID
LEFT OUTER JOIN PositionFunds ON  PositionBudgets.ID = PositionFunds.PositionBudgetID
LEFT OUTER JOIN Funds ON PositionFunds.FundID = Funds.ID
INNER JOIN E_Positions ON E_Positions.PositionID =  positions.PositionID
INNER JOIN EMPLOYEES ON EMPLOYEES.EmployeeID =  E_Positions.EmployeeId
INNER JOIN Persons ON Persons.PersonID =  EMPLOYEES.PersonID
)t

END

