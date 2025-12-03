CREATE TABLE [dbo].[PersonLicenses] (
    [PersonLicenseId] INT           IDENTITY (1, 1) NOT NULL,
    [PersonId]        INT           NOT NULL,
    [LicenseTypeId]   TINYINT       NOT NULL,
    [LicenseNumber]   VARCHAR (50)  NULL,
    [StateId]         INT       NOT NULL,
    [CountryId]       INT      NOT NULL,
    [ExpirationDate]  SMALLDATETIME NOT NULL,
    [RevokedDate]     SMALLDATETIME NULL,
    [ReinstatedDate]  SMALLDATETIME NULL,
    [Notes]           TEXT          NULL,
    [EnteredBy]       VARCHAR (50)  NULL,
    [EnteredDate]     SMALLDATETIME NULL,
    [ModifiedBy]      VARCHAR (50)  NULL,
    [ModifiedDate]    SMALLDATETIME NULL,
    CONSTRAINT [PK_pLicenses] PRIMARY KEY CLUSTERED ([PersonLicenseId] ASC),
    CONSTRAINT [FK_PersonLicenses_DdlCountries] FOREIGN KEY ([CountryId]) REFERENCES [dbo].[DdlCountries] ([CountryId]),
    CONSTRAINT [FK_PersonLicenses_DdlLicenseTypes] FOREIGN KEY ([LicenseTypeId]) REFERENCES [dbo].[DdlLicenseTypes] ([LicenseTypeId]),
    CONSTRAINT [FK_PersonLicenses_DdlStates] FOREIGN KEY ([StateId]) REFERENCES [dbo].[DdlStates] ([StateId]),
    CONSTRAINT [FK_PersonLicenses_Persons] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Persons] ([PersonId])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_PersonLicenses]
    ON [dbo].[PersonLicenses]([PersonId] ASC, [LicenseTypeId] ASC, [ExpirationDate] ASC, [CountryId] ASC, [StateId] ASC);

