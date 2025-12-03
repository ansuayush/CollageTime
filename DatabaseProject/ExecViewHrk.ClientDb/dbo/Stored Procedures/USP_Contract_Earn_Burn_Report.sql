CREATE PROC USP_Contract_Earn_Burn_Report
AS
BEGIN

SELECT DISTINCT * FROM(
select 
--PositionBudgets.ID,
--PositionBudgets.PositionID AS 'POSID',
--positions.PositionCode,
--positions.Title,
--PositionFunds.*,
--positions.PositionId,


Funds.code AS 'FundingSource'
,Funds.Description AS 'FundingSourceDescription'
,Funds.EffectiveStartDate AS 'StartDate'
,Funds.EffectiveEndDate AS 'EndDate'
,'' AS 'FundingSourceComments'
,PositionBusinessLevels.BusinessLevelCode AS 'BudgetCenter'
,PositionBusinessLevels.BusinessLevelTitle AS 'BudgetCenterDescription'
,0.00 AS 'ContractAmount'
,0.00 AS 'AmountSpent'
,0.00 AS 'BalanceRemaining'
,'' AS 'TimeElapsed'
,'' AS 'ContractSpent'
,PositionBusinessLevels.BusinessLevelNotes AS 'BudgetComments'
 from positions
INNER JOIN PositionBusinessLevels ON PositionBusinessLevels.BusinessLevelNbr = positions.BusinessLevelNbr
INNER JOIN PositionBudgets ON positions.PositionID = PositionBudgets.PositionID
LEFT OUTER JOIN PositionFunds ON  PositionBudgets.ID = PositionFunds.PositionBudgetID
LEFT OUTER JOIN Funds ON PositionFunds.FundID = Funds.ID

)t

END

