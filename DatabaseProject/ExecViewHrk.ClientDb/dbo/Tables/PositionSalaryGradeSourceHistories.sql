CREATE TABLE [dbo].[PositionSalaryGradeSourceHistories] (
    [PositionSalaryGradeHistoriesID] INT           IDENTITY (1, 1) PRIMARY KEY NOT NULL,
    [SalaryGradeID]                  INT       NOT NULL,
    [ValidDate]                      SMALLDATETIME NULL,
    [salaryMinimum]                  MONEY         NOT NULL,
    [salaryMidpoint]                 MONEY         NOT NULL,
    [salaryMaximum]                  MONEY         NOT NULL,
    [ChangeEffectiveDate]            SMALLDATETIME NULL,
    [DdlSalaryGradeHistoriesID]      INT       NOT NULL
);

