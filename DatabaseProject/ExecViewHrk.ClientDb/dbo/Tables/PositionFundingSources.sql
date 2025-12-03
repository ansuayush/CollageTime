CREATE TABLE [dbo].[PositionFundingSources] (
    [PositionFundingSourceID] INT           IDENTITY (1, 1) PRIMARY KEY NOT NULL,
    [EffectiveDate]           DATETIME NULL,
    [FundCodeID]              INT       NOT NULL,
    [Percentage]              INT       NULL,
    [PositionId]              INT      NULL,
    [FundingGroup]            VARCHAR(50) NULL
);

