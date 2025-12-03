CREATE TABLE [dbo].[PositionBudgetSchedules] (
    [ID]                INT             IDENTITY (1, 1) NOT NULL,
    [EffectiveDate]     SMALLDATETIME   NOT NULL,
    [StartDate]         SMALLDATETIME   NOT NULL,
    [EndDate]           SMALLDATETIME   NOT NULL,
    [EligibleDate]      SMALLDATETIME   NOT NULL,
    [IncreaseType]      TINYINT         NOT NULL,
    [IncreaseAmount]    DECIMAL (18, 2) NOT NULL,
    [WashoutRule]       DECIMAL (18, 2) CONSTRAINT [DF_PositionBudgetSchedules_WashoutRule] DEFAULT ((0.0)) NOT NULL,
    [WashoutRuleSalary] DECIMAL (18, 2) CONSTRAINT [DF_PositionBudgetSchedules_WashoutRuleSalary] DEFAULT ((0.0)) NOT NULL,
    [AutoFill]          BIT             CONSTRAINT [DF_PositionBudgetSchedules_AutoFill] DEFAULT ((0)) NOT NULL,
    [ScheduleType]      TINYINT         CONSTRAINT [DF_PositionBudgetSchedules_ScheduleType] DEFAULT ((2)) NOT NULL,
    CONSTRAINT [PK_PositionBudgetSchedules] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_PositionBudgetSchedules] UNIQUE NONCLUSTERED ([EffectiveDate] ASC, [ScheduleType] ASC)
);

