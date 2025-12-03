CREATE TABLE [dbo].[CompanyCodes] (
    [CompanyCodeId]          INT     IDENTITY (1, 1) NOT NULL,
    [CompanyCodeCode]        VARCHAR (10) NOT NULL,
    [CompanyCodeDescription] VARCHAR (50) NOT NULL,
    [IsCompanyCodeActive]    BIT          CONSTRAINT [DF_vCompanyCodes_Active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vCompanyCodes] PRIMARY KEY CLUSTERED ([CompanyCodeId] ASC)
);

