CREATE TABLE [dbo].[DdlPropertyTypes] (
    [PropertyTypeId] INT     IDENTITY (1, 1) NOT NULL,
    [Description]    VARCHAR (50) NOT NULL,
    [Code]           VARCHAR (10) NOT NULL,
    [Active]         BIT          CONSTRAINT [DF_vPropertyTypes_active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vPropertyTypes] PRIMARY KEY CLUSTERED ([PropertyTypeId] ASC)
);

