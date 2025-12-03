CREATE TABLE [dbo].[PersonWorkPermits] (
    [PersonWorkPermitId] INT           IDENTITY (1, 1) NOT NULL,
    [PersonId]           INT           NOT NULL,
    [CountryId]          INT      NOT NULL,
    [WorkPermitNumber]   VARCHAR (50)  NULL,
    [WorkPermitType]     VARCHAR (50)  NULL,
    [IssuingAuthority]   VARCHAR (50)  NULL,
    [IssueDate]          SMALLDATETIME NULL,
    [ExpirationDate]     SMALLDATETIME NULL,
    [ExtensionDate]      SMALLDATETIME NULL,
    [Notes]              TEXT          NULL,
    [EnteredBy]          VARCHAR (50)  NOT NULL,
    [EnteredDate]        SMALLDATETIME NOT NULL,
    CONSTRAINT [PK_pWorkPermits] PRIMARY KEY CLUSTERED ([PersonWorkPermitId] ASC),
    CONSTRAINT [FK_PersonWorkPermits_DdlCountries] FOREIGN KEY ([CountryId]) REFERENCES [dbo].[DdlCountries] ([CountryId]),
    CONSTRAINT [FK_PersonWorkPermits_Persons] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Persons] ([PersonId])
);

