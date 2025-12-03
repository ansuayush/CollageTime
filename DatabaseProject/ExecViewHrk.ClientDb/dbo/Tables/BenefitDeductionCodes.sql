CREATE TABLE [dbo].[BenefitDeductionCodes] (
    [BenefitDeductionCodeId]          INT          IDENTITY (1, 1) NOT NULL,
    [CompanyCodeId]                   Int     NOT NULL,
    [BenefitDeductionCodeCode]        VARCHAR (10) NOT NULL,
    [BenefitDeductionCodeDescription] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_vBenefitDeductionCodes] PRIMARY KEY CLUSTERED ([BenefitDeductionCodeId] ASC),
    CONSTRAINT [FK_BenefitDeductionCodes_CompanyCodes] FOREIGN KEY ([CompanyCodeId]) REFERENCES [dbo].[CompanyCodes] ([CompanyCodeId]),
    CONSTRAINT [uq_BenefitDeductionCodes] UNIQUE NONCLUSTERED ([CompanyCodeId] ASC, [BenefitDeductionCodeCode] ASC)
);

