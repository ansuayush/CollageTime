CREATE TABLE [dbo].[PersonADA] (
    [PersonAdaId]            INT           IDENTITY (1, 1) NOT NULL,
    [PersonId]               INT           NOT NULL,
    [AccommodationTypeId]    INT      NOT NULL,
    [AssociatedDisabilityId] INT      NOT NULL,
    [RequestedDate]          DATETIME NULL,
    [EstimatedCost]          MONEY         NULL,
    [AccommodationProvided]  BIT           NULL,
    [ProvidedDate]           DATETIME NULL,
    [ActualCost]             MONEY         NULL,
    [Notes]                  TEXT          NULL,
    [EnteredBy]              VARCHAR (50)  NULL,
    [EnteredDate]            DATETIME NULL,
    [ModifiedBy]             VARCHAR (50)  NULL,
    [ModifiedDate]           DATETIME NULL,
    CONSTRAINT [PK_pADA] PRIMARY KEY CLUSTERED ([PersonAdaId] ASC),
    CONSTRAINT [FK_PersonADA_ddlAccommodationTypes] FOREIGN KEY ([AccommodationTypeId]) REFERENCES [dbo].[DdlAccommodationTypes] ([AccommodationTypeId]),
    CONSTRAINT [FK_PersonADA_ddlDisabilityTypes] FOREIGN KEY ([AssociatedDisabilityId]) REFERENCES [dbo].[DdlDisabilityTypes] ([DisabilityTypeId]),
    CONSTRAINT [FK_PersonADA_Person] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Persons] ([PersonId])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_PersonADA]
    ON [dbo].[PersonADA]([PersonId] ASC, [AccommodationTypeId] ASC, [AssociatedDisabilityId] ASC);

