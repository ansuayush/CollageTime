CREATE TABLE [dbo].[PersonProperties] (
    [PersonPropertyTypeId] INT           IDENTITY (1, 1) NOT NULL,
    [PersonId]             INT           NOT NULL,
    [PropertyTypeId]       INT      NOT NULL,
    [AcquiredDate]         SMALLDATETIME NULL,
    [ReleaseDate]          SMALLDATETIME NULL,
    [EstimatedValue]       MONEY         NULL,
    [AssetNumber]          VARCHAR (50)  NULL,
    [PropertyDescription]  TEXT          NULL,
    [Notes]                TEXT          NULL,
    [EnteredBy]            VARCHAR (50)  NULL,
    [EnteredDate]          SMALLDATETIME NULL,
    [ModifiedBy]           VARCHAR (50)  NULL,
    [ModifiedDate]         SMALLDATETIME NULL,
    CONSTRAINT [PK_pProperties] PRIMARY KEY CLUSTERED ([PersonPropertyTypeId] ASC),
    CONSTRAINT [FK_PersonProperties_DdlPropertyTypes] FOREIGN KEY ([PropertyTypeId]) REFERENCES [dbo].[DdlPropertyTypes] ([PropertyTypeId]),
    CONSTRAINT [FK_PersonProperties_Persons] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Persons] ([PersonId])
);


GO
CREATE NONCLUSTERED INDEX [IX_PersonProperties]
    ON [dbo].[PersonProperties]([PersonId] ASC, [PersonPropertyTypeId] ASC, [AcquiredDate] ASC);

