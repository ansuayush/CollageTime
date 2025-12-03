CREATE TABLE [dbo].[FundHistories] (
    [ID]            INT             IDENTITY (1, 1) NOT NULL,
    [FundID]        INT             NOT NULL,
    [EffectiveDate] SMALLDATETIME   NOT NULL,
    [Amount]        DECIMAL (18, 2) CONSTRAINT [DF_FundHistory_Amount] DEFAULT ((0.0)) NOT NULL,
    CONSTRAINT [PK_FundHistory] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_FundHistory_Funds] FOREIGN KEY ([FundID]) REFERENCES [dbo].[Funds] ([ID]),
    CONSTRAINT [IX_FundHistory] UNIQUE NONCLUSTERED ([FundID] ASC, [EffectiveDate] ASC)
);

