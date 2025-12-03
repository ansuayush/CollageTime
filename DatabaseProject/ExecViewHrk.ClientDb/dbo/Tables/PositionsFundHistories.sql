CREATE TABLE [dbo].[PositionsFundHistories] (
    [ID]               INT             IDENTITY (1, 1) NOT NULL,
    [PositionBudgetID] INT             NOT NULL,
    [FundHistoryID]    INT             NOT NULL,
    [PositionAmount]   DECIMAL (18, 2) CONSTRAINT [DF_PositionsFundHistory_PositionAmount] DEFAULT ((0.0)) NOT NULL,
    CONSTRAINT [PK_PositionsFunds] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_PositionsFunds_Funds] FOREIGN KEY ([FundHistoryID]) REFERENCES [dbo].[FundHistories] ([ID]),
    CONSTRAINT [FK_PositionsFunds_Positions] FOREIGN KEY ([PositionBudgetID]) REFERENCES [dbo].[PositionBudgets] ([ID]),
    CONSTRAINT [IX_PositionsFunds] UNIQUE NONCLUSTERED ([PositionBudgetID] ASC, [FundHistoryID] ASC)
);

