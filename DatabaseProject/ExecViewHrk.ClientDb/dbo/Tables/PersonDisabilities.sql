CREATE TABLE [dbo].[PersonDisabilities] (
    [PersonDisabilityId] INT           IDENTITY (1, 1) NOT NULL,
    [PersonId]           INT           NOT NULL,
    [DisabilityTypeId]   INT      NOT NULL,
    [DocumentationDate]  DATETIME NULL,
    [Notes]              TEXT          NULL,
    [EnteredBy]          VARCHAR (50)  NULL,
    [EnteredDate]        DATETIME NULL,
    [ModifiedBy]         VARCHAR (50)  NULL,
    [ModifiedDate]       DATETIME NULL,
    CONSTRAINT [PK_pDisabilities] PRIMARY KEY CLUSTERED ([PersonDisabilityId] ASC),
    CONSTRAINT [FK_PersonDisabilities_ddlDisabilityTypes] FOREIGN KEY ([DisabilityTypeId]) REFERENCES [dbo].[DdlDisabilityTypes] ([DisabilityTypeId]),
    CONSTRAINT [FK_PersonDisabilities_Persons] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Persons] ([PersonId])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_PersonDisabilities]
    ON [dbo].[PersonDisabilities]([PersonId] ASC, [DisabilityTypeId] ASC);

