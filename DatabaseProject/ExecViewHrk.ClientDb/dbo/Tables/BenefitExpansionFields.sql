CREATE TABLE [dbo].[BenefitExpansionFields] (
    [BenefitExpansionFieldId]          INT          IDENTITY (1, 1) NOT NULL,
    [CompanyCodeId]                    INT     NOT NULL,
    [BenefitExpansionFieldCode]        VARCHAR (10) NOT NULL,
    [BenefitExpansionFieldDescription] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_vBenefitExpansionFields] PRIMARY KEY CLUSTERED ([BenefitExpansionFieldId] ASC),
    CONSTRAINT [FK_BenefitExpansionFields_CompanyCodes] FOREIGN KEY ([CompanyCodeId]) REFERENCES [dbo].[CompanyCodes] ([CompanyCodeId]),
    CONSTRAINT [uq_BenefitExpansionFields] UNIQUE NONCLUSTERED ([CompanyCodeId] ASC, [BenefitExpansionFieldCode] ASC)
);

