CREATE TABLE [dbo].[BenefitOESchedules] (
    [BenefitOEScheduleId] INT           IDENTITY (1, 1) NOT NULL,
    [StartDate]           SMALLDATETIME NOT NULL,
    [EndDate]             SMALLDATETIME NOT NULL,
    [EffectiveDate]       SMALLDATETIME NOT NULL,
    [CompanyCodeId]       INT      NOT NULL,
    [ScheduleGroup]       VARCHAR (50)  NULL,
    CONSTRAINT [PK_BenefitsOESchedule] PRIMARY KEY CLUSTERED ([BenefitOEScheduleId] ASC),
    CONSTRAINT [CheckEndDateAfterStartDate] CHECK ([EndDate]>[StartDate]),
    CONSTRAINT [FK_BenefitOESchedules_CompanyCodes] FOREIGN KEY ([CompanyCodeId]) REFERENCES [dbo].[CompanyCodes] ([CompanyCodeId]),
    CONSTRAINT [uq_BenefitOESchedules] UNIQUE NONCLUSTERED ([StartDate] ASC, [CompanyCodeId] ASC)
);

