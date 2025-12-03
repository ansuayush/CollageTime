CREATE TABLE [dbo].[BenefitOptions] (
    [BenefitOptionId]          INT          IDENTITY (1, 1) NOT NULL,
    [CompanyCodeId]            INT     NOT NULL,
    [BenefitOptionCode]        VARCHAR (10) NOT NULL,
    [BenefitOptionDescription] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_vBenefitOptions] PRIMARY KEY CLUSTERED ([BenefitOptionId] ASC),
    CONSTRAINT [FK_BenefitOptions_CompanyCodes] FOREIGN KEY ([CompanyCodeId]) REFERENCES [dbo].[CompanyCodes] ([CompanyCodeId]),
    CONSTRAINT [uq_BenefitOptions] UNIQUE NONCLUSTERED ([CompanyCodeId] ASC, [BenefitOptionCode] ASC)
);

