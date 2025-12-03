CREATE TABLE [dbo].[DdlEducationEstablishments] (
    [EducationEstablishmentId] INT           IDENTITY (1, 1) NOT NULL,
    [Description]              VARCHAR (50)  NOT NULL,
    [Code]                     VARCHAR (10)  NOT NULL,
    [AddressLineOne]           VARCHAR (50)  NULL,
    [AddressLineTwo]           VARCHAR (50)  NULL,
    [City]                     VARCHAR (50)  NULL,
    [StateId]                  INT       NULL,
    [ZipCode]                  VARCHAR (9)   NULL,
    [CountryId]                INT		      NULL,
    [PhoneNumber]              VARCHAR (14)  NULL,
    [FaxNumber]                VARCHAR (10)  NULL,
    [EducationTypeId]          INT      NULL,
    [AccountNumber]            VARCHAR (50)  NULL,
    [Contact]                  VARCHAR (50)  NULL,
    [WebAddress]               VARCHAR (100) NULL,
    [Notes]                    TEXT          NULL,
    [Active]                   BIT           CONSTRAINT [DF_DdlEducationEstablishments_Active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vEducationEst] PRIMARY KEY CLUSTERED ([EducationEstablishmentId] ASC),
    CONSTRAINT [FK_DdlEducationEstablishments_DdlCountries] FOREIGN KEY ([CountryId]) REFERENCES [dbo].[DdlCountries] ([CountryId]),
    CONSTRAINT [FK_DdlEducationEstablishments_DdlEducationTypes] FOREIGN KEY ([EducationTypeId]) REFERENCES [dbo].[DdlEducationTypes] ([EducationTypeId]),
    CONSTRAINT [FK_DdlEducationEstablishments_DdlStates] FOREIGN KEY ([StateId]) REFERENCES [dbo].[DdlStates] ([StateId])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_DdlEducationEstablishments]
    ON [dbo].[DdlEducationEstablishments]([Code] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_DdlEducationEstablishments_1]
    ON [dbo].[DdlEducationEstablishments]([Description] ASC);

