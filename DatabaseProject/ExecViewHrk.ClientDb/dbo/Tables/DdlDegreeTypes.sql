CREATE TABLE [dbo].[DdlDegreeTypes] (
    [DegreeTypeId] INT     IDENTITY (1, 1) NOT NULL,
    [Description]  VARCHAR (50) NOT NULL,
    [Code]         VARCHAR (10) NOT NULL,
    [Active]       BIT          CONSTRAINT [DF_vMajors_active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vMajors] PRIMARY KEY CLUSTERED ([DegreeTypeId] ASC)
);

