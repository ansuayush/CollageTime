CREATE TABLE [dbo].[E_PositionSalaryHistories] (
    [E_PositionSalaryHistoryId] INT             IDENTITY (1, 1) NOT NULL,
    [E_PositionId]              INT             NOT NULL,
    [EffectiveDate]             SMALLDATETIME   NULL,
    [PayRate]                   DECIMAL (8, 2)  NULL,
    [HoursPerPayPeriod]         DECIMAL (18, 2) NULL,
    [Notes]                     TEXT            NULL,
    [EnteredBy]                 VARCHAR (50)    NULL,
    [EnteredDate]               SMALLDATETIME   NULL,
    [ModifiedBy]                VARCHAR (50)    NULL,
    [ModifiedDate]              SMALLDATETIME   NULL,
    [EndDate]                   DATETIME        NULL,
    [RateTypeId]                INT             NULL,
    CONSTRAINT [PK_E_PositionSalaryHistories] PRIMARY KEY CLUSTERED ([E_PositionSalaryHistoryId] ASC),
    CONSTRAINT [FK_E_PositionSalaryHistories_E_Positions] FOREIGN KEY ([E_PositionId]) REFERENCES [dbo].[E_Positions] ([E_PositionId])
);

