CREATE TABLE [dbo].[BenefitProviders] (
    [BenefitProviderId]          INT          IDENTITY (1, 1) NOT NULL,
    [CompanyCodeId]              INT     NOT NULL,
    [BenefitProviderCode]        VARCHAR (10) NOT NULL,
    [BenefitProviderDescription] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_vBenefitProviders] PRIMARY KEY CLUSTERED ([BenefitProviderId] ASC),
    CONSTRAINT [FK_BenefitProviders_CompanyCodes] FOREIGN KEY ([CompanyCodeId]) REFERENCES [dbo].[CompanyCodes] ([CompanyCodeId]),
    CONSTRAINT [uq_BenefitProviders] UNIQUE NONCLUSTERED ([CompanyCodeId] ASC, [BenefitProviderCode] ASC)
);

