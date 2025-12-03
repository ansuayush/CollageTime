CREATE TABLE [dbo].[PositionFunds] (
    [PositionFundID]   INT          IDENTITY (1, 1) NOT NULL,
    [PositionBudgetID] INT          NOT NULL,
    [FundID]           INT          NOT NULL,
    [Amount]           DECIMAL (18) NULL,
    CONSTRAINT [PK_PositionFunds] PRIMARY KEY CLUSTERED ([PositionFundID] ASC),
    CONSTRAINT [FK_PositionBudgets_PositionFunds] FOREIGN KEY ([PositionBudgetID]) REFERENCES [dbo].[PositionBudgets] ([ID])
);

