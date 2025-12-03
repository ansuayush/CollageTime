CREATE TABLE [dbo].[ADPFieldMappings] (
    [ADPFieldMappingId]   INT      IDENTITY (1, 1) NOT NULL,
    [ADPFieldMappingCode] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_ADPFieldMappings] PRIMARY KEY CLUSTERED ([ADPFieldMappingId] ASC)
);

