CREATE TABLE [dbo].[DdlRelationshipTypes] (
    [RelationshipTypeId] INT          IDENTITY (1, 1) NOT NULL,
    [Description]        VARCHAR (50) NOT NULL,
    [Code]               VARCHAR (10) NOT NULL,
    [IsSpouse]           BIT          CONSTRAINT [DF_vRelationshipTypes_spouse] DEFAULT ((0)) NULL,
    [CobraEligible]      BIT          CONSTRAINT [DF_vRelationshipTypes_cobra] DEFAULT ((0)) NULL,
    [Active]             BIT          CONSTRAINT [DF_ddlRelationshipTypes_active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vRelationshipTypes] PRIMARY KEY CLUSTERED ([RelationshipTypeId] ASC)
);

