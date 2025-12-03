CREATE TABLE [dbo].[DdlApplicantSources] (
    [ApplicantSourceId] INT           IDENTITY (1, 1) NOT NULL,
    [Description]       VARCHAR (50)  NOT NULL,
    [Code]              VARCHAR (10)  NOT NULL,
    [AddressLineOne]    VARCHAR (50)  NULL,
    [AddressLineTwo]    VARCHAR (50)  NULL,
    [City]              VARCHAR (50)  NULL,
    [StateId]           TINYINT       NULL,
    [ZipCode]           VARCHAR (9)   NULL,
    [CountryId]         INT           NULL,
    [PhoneNumber]       VARCHAR (10)  NULL,
    [FaxNumber]         VARCHAR (10)  NULL,
    [Contact]           VARCHAR (50)  NULL,
    [WebAddress]        VARCHAR (100) NULL,
    [AccountNumber]     VARCHAR (50)  NULL,
    [Notes]             TEXT          NULL,
    [Active]            BIT           CONSTRAINT [DF_ddlApplicantSources_active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vApplicantSources] PRIMARY KEY CLUSTERED ([ApplicantSourceId] ASC),
    CONSTRAINT [FK_DdlApplicantSources_DdlApplicantSources] FOREIGN KEY ([ApplicantSourceId]) REFERENCES [dbo].[DdlApplicantSources] ([ApplicantSourceId])
);

