CREATE TABLE [dbo].[PersonInnoculations] (
    [PersonInnoculationId] INT           IDENTITY (1, 1) NOT NULL,
    [PersonId]             INT           NOT NULL,
    [InnoculationTypeId]   INT      NOT NULL,
    [InnoculationDate]     DATETIME NULL,
    [ExpirationDate]       DATETIME NULL,
    [Notes]                TEXT          NULL,
    [EnteredBy]            VARCHAR (50)  NULL,
    [EnteredDate]          DATETIME NULL,
    [ModifiedBy]           VARCHAR (50)  NULL,
    [ModifiedDate]         DATETIME NULL,
    CONSTRAINT [PK_pInnoculations] PRIMARY KEY CLUSTERED ([PersonInnoculationId] ASC),
    CONSTRAINT [FK_PersonInnoculations_DdlInnoculationTypes] FOREIGN KEY ([InnoculationTypeId]) REFERENCES [dbo].[DdlInnoculationTypes] ([InnoculationTypeId]),
    CONSTRAINT [FK_PersonInnoculations_Persons] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Persons] ([PersonId])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_PersonInnoculations]
    ON [dbo].[PersonInnoculations]([PersonId] ASC, [InnoculationTypeId] ASC, [InnoculationDate] ASC);

