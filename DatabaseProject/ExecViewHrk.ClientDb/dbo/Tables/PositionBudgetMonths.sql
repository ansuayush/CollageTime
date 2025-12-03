CREATE TABLE [dbo].[PositionBudgetMonths] (
    [ID]                INT             IDENTITY (1, 1) NOT NULL,
    [PositionBudgetsID] INT             NOT NULL,
    [BudgetMonth]       TINYINT         NOT NULL,
    [BudgetAmount]      DECIMAL (18, 2) NOT NULL,
    [DisplayPosition]   TINYINT         NOT NULL,
    CONSTRAINT [PK_PositionBudgetMonths] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_PositionBudgetMonths_PositionBudgets] FOREIGN KEY ([PositionBudgetsID]) REFERENCES [dbo].[PositionBudgets] ([ID])
);

