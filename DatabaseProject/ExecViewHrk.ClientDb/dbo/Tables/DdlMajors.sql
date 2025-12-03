CREATE TABLE [dbo].[DdlMajors] (
    [MajorId]     INT     IDENTITY (1, 1) NOT NULL,
    [Description] VARCHAR (50) NOT NULL,
    [Code]        VARCHAR (10) NOT NULL,
    [Active]      BIT          CONSTRAINT [DF_vDdlMajors_Active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vDdlMajors] PRIMARY KEY CLUSTERED ([MajorId] ASC)
);

