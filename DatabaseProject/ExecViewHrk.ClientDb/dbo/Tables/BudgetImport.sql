CREATE TABLE [dbo].[BudgetImport] (
    [ID]             INT           IDENTITY (1, 1) NOT NULL,
    [PositionCode]   VARCHAR (100) NULL,
    [Year]           VARCHAR (100) NULL,
    [RevisionNumber] VARCHAR (100) NULL,
    [StartDate]      VARCHAR (100) NULL,
    [EndDate]        VARCHAR (100) NULL,
    [Fte]            VARCHAR (100) NULL,
    [BudgetAmount]   VARCHAR (100) NULL,
    [RevisionRank]   VARCHAR (100) NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

