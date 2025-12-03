CREATE TABLE [dbo].[DdlLicenseTypes] (
    [LicenseTypeId] TINYINT      IDENTITY (1, 1) NOT NULL,
    [Description]   VARCHAR (50) NOT NULL,
    [Code]          VARCHAR (10) NOT NULL,
    [Active]        BIT          CONSTRAINT [DF_ddlLicenseTypes_active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vLicenseTypes] PRIMARY KEY CLUSTERED ([LicenseTypeId] ASC)
);

