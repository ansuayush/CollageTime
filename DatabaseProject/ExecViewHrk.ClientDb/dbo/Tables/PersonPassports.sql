CREATE TABLE [dbo].[PersonPassports] (
    [PersonPassportId] INT           IDENTITY (1, 1) NOT NULL,
    [PersonId]         INT           NOT NULL,
    [CountryId]        INT      NOT NULL,
    [PassportNumber]   VARCHAR (50)  NULL,
    [PassportStorage]  VARCHAR (50)  NULL,
    [IssueDate]        SMALLDATETIME NULL,
    [ExpirationDate]   SMALLDATETIME NULL,
    [EnteredBy]        VARCHAR (50)  NULL,
    [EnteredDate]      SMALLDATETIME NULL,
    [ModifiedBy]       VARCHAR (50)  NULL,
    [ModifiedDate]     SMALLDATETIME NULL,
    CONSTRAINT [PK_pPassports] PRIMARY KEY CLUSTERED ([PersonPassportId] ASC),
    CONSTRAINT [FK_PersonPassports_DdlCountries] FOREIGN KEY ([CountryId]) REFERENCES [dbo].[DdlCountries] ([CountryId]),
    CONSTRAINT [FK_PersonPassports_Persons] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Persons] ([PersonId])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_PersonPassports]
    ON [dbo].[PersonPassports]([PersonId] ASC, [CountryId] ASC, [IssueDate] ASC);

