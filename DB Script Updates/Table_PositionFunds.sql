
IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('PositionFunds'))
BEGIN

CREATE TABLE [dbo].[PositionFunds](
	[PositionFundID] [int] IDENTITY(1,1) NOT NULL,
	[PositionBudgetID] [int] NOT NULL,
	[FundID] [int] NOT NULL,
	[Amount] [decimal](18, 0) NULL
)


END

GO

