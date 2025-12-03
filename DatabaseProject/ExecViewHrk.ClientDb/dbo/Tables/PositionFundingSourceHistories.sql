CREATE TABLE [dbo].[PositionFundingSourceHistories] (
    [FundingSourceHistoriesID] INT           IDENTITY (1, 1) PRIMARY KEY NOT NULL,
    [EffectiveDate]            SMALLDATETIME NULL,
    [FundCodeID]               TINYINT       NOT NULL,
    [Percentage]               TINYINT       NULL,
    [ChangeEffectiveDate]      SMALLDATETIME NULL,
    [PositionFundingSourceID]  TINYINT       NOT NULL
);

