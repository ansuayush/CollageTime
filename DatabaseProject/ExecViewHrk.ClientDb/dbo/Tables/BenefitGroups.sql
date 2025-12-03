CREATE TABLE [dbo].[BenefitGroups] (
    [BenefitGroupId]          INT          IDENTITY (1, 1) NOT NULL,
    [CompanyCodeId]           INT     NOT NULL,
    [BenefitGroupCode]        VARCHAR (10) NOT NULL,
    [BenefitGroupDescription] VARCHAR (50) NOT NULL,
    [DeductionCode]           VARCHAR (10) NOT NULL,
    [DisplayPosition]         INT          CONSTRAINT [DF_BenefitGroups_DisplayPosition] DEFAULT ((1)) NOT NULL,
    [CanBeDeclined]           BIT          CONSTRAINT [DF_BenefitGroups_CanBeDeclined] DEFAULT ((0)) NOT NULL,
    [ScheduleGroup]           VARCHAR (20) NULL,
    CONSTRAINT [PK_BenefitGroups] PRIMARY KEY CLUSTERED ([BenefitGroupId] ASC),
    CONSTRAINT [FK_BenefitGroups_CompanyCodes] FOREIGN KEY ([CompanyCodeId]) REFERENCES [dbo].[CompanyCodes] ([CompanyCodeId]),
    CONSTRAINT [uq_BenefitGroups] UNIQUE NONCLUSTERED ([CompanyCodeId] ASC, [BenefitGroupCode] ASC)
);

