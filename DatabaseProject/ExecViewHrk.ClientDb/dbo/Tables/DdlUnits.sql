CREATE TABLE [dbo].[DdlUnits] (
    [UnitId]      INT     IDENTITY (1, 1) NOT NULL,
    [Description] VARCHAR (50) NOT NULL,
    [Code]        VARCHAR (10) NOT NULL,
    CONSTRAINT [PK_vUnits] PRIMARY KEY CLUSTERED ([UnitId] ASC)
);

