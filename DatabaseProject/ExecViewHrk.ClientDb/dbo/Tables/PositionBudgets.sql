CREATE TABLE [dbo].[PositionBudgets] (
    [ID]            INT             IDENTITY (1, 1) NOT NULL,
    [PositionID]    INT        NOT NULL,
    [BudgetYear]    INT             NOT NULL,
    [BudgetMonth]   TINYINT         CONSTRAINT [DF_PositionBudgets_BudgetMonth] DEFAULT ('') NOT NULL,
    [BudgetAmount]  DECIMAL (18, 2) NOT NULL,
    [BurdenPercent] DECIMAL (18, 2) CONSTRAINT [DF_PositionBudgets_BurdenPercent] DEFAULT ((0.0)) NOT NULL,
    [EmployeeID]    INT             NULL,
    [FTE]           DECIMAL (18, 9) CONSTRAINT [DF_PositionBudgets_FTE] DEFAULT ((0.0)) NOT NULL,
    CONSTRAINT [PK_PositionBudgets] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_PositionBudgets_Employees] FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[Employees] ([EmployeeId]),
    CONSTRAINT [FK_PositionBudgets_Positions] FOREIGN KEY ([PositionID]) REFERENCES [dbo].[Positions] ([PositionId])
);

