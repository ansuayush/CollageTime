CREATE TABLE [dbo].[EarningsCodes] (
    [EarningsCodeId]          INT     IDENTITY (1, 1) NOT NULL,
    [CompanyCodeId]           INT     NOT NULL,
    [EarningsCodeCode]        VARCHAR (2)  NOT NULL,
    [EarningsCodeDescription] VARCHAR (20) NOT NULL,
    [ADPFieldMappingId]       INT      NOT NULL,
    [IsEarningsCodeActive]    BIT          CONSTRAINT [DF_vEarningsCodes_Active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_EarningsCodes] PRIMARY KEY CLUSTERED ([EarningsCodeId] ASC),
    CONSTRAINT [FK_EarningsCodes_ADPFieldMappings] FOREIGN KEY ([ADPFieldMappingId]) REFERENCES [dbo].[ADPFieldMappings] ([ADPFieldMappingId]),
    CONSTRAINT [FK_EarningsCodes_CompanyCodes] FOREIGN KEY ([CompanyCodeId]) REFERENCES [dbo].[CompanyCodes] ([CompanyCodeId]),
    CONSTRAINT [uq_EarningsCodes] UNIQUE NONCLUSTERED ([CompanyCodeId] ASC, [EarningsCodeId] ASC)
);

