CREATE TABLE [dbo].[BusinessUnits] (
    [BusinessUnitId]          INT     IDENTITY (1, 1) NOT NULL,
    [BusinessUnitCode]        VARCHAR (10) NOT NULL,
    [BusinessUnitDescription] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_vBusinessUnits] PRIMARY KEY CLUSTERED ([BusinessUnitId] ASC)
);

