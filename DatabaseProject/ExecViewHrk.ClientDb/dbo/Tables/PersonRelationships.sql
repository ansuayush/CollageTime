CREATE TABLE [dbo].[PersonRelationships] (
    [PersonRelationshipId] INT           IDENTITY (1, 1) NOT NULL,
    [PersonId]             INT           NOT NULL,
    [RelationshipTypeId]   INT           NOT NULL,
    [RelationPersonId]     INT           NOT NULL,
    [Dependent]            BIT           CONSTRAINT [DF_pRelationships_dependent] DEFAULT ((0)) NULL,
    [EmergencyContact]     BIT           CONSTRAINT [DF_pRelationships_emergencyContact] DEFAULT ((0)) NULL,
    [Garnishment]          BIT           CONSTRAINT [DF_pRelationships_garnishment] DEFAULT ((0)) NULL,
    [EnteredBy]            VARCHAR (50)  NULL,
    [EnteredDate]          SMALLDATETIME NULL,
    [ModifiedBy]           VARCHAR (50)  NULL,
    [ModifiedDate]         SMALLDATETIME NULL,
    CONSTRAINT [PK_pRelationships] PRIMARY KEY CLUSTERED ([PersonRelationshipId] ASC),
    CONSTRAINT [FK_PersonRelationships_DdlRelationshipTypes] FOREIGN KEY ([RelationshipTypeId]) REFERENCES [dbo].[DdlRelationshipTypes] ([RelationshipTypeId]),
    CONSTRAINT [FK_PersonRelationships_Persons] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Persons] ([PersonId]),
    CONSTRAINT [FK_PersonRelationships_RelationPersons] FOREIGN KEY ([RelationPersonId]) REFERENCES [dbo].[Persons] ([PersonId])
);

