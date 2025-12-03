CREATE TABLE [dbo].[PayPeriods] (
    [PayPeriodId]       INT           IDENTITY (1, 1) NOT NULL,
    [CompanyCodeId]     INT      NULL,
    [StartDate]         SMALLDATETIME NOT NULL,
    [EndDate]           SMALLDATETIME NOT NULL,
    [PayFrequencyId]    int       NOT NULL,
    [IsPayPeriodActive] BIT           CONSTRAINT [DF_vPayPeriods_IsPayPeriodActive] DEFAULT ((1)) NOT NULL,
    [LockoutEmployees]  BIT           CONSTRAINT [DF_vPayPeriods_LockoutEmployees] DEFAULT ((0)) NOT NULL,
    [LockoutManagers]   BIT           CONSTRAINT [DF_vPayPeriods_LockoutManagers] DEFAULT ((0)) NOT NULL,
    [IsArchived]        BIT           CONSTRAINT [DF_PayPeriods_IsArchived] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_PayPeriods] PRIMARY KEY CLUSTERED ([PayPeriodId] ASC),
    CONSTRAINT [FK_PayPeriods_CompanyCodes] FOREIGN KEY ([CompanyCodeId]) REFERENCES [dbo].[CompanyCodes] ([CompanyCodeId]),
    CONSTRAINT [FK_PayPeriods_DdlPayFrequencies] FOREIGN KEY ([PayFrequencyId]) REFERENCES [dbo].[DdlPayFrequencies] ([PayFrequencyId])
);

