CREATE TABLE [dbo].[ddlPerformancePotentials] (
    [Id]          SMALLINT     IDENTITY (1, 1) NOT NULL,
    [Description] VARCHAR (50) NOT NULL,
    [Code]        VARCHAR (10) NOT NULL,
    [Active]      BIT          NOT NULL,
    CONSTRAINT [PK_vPerformancePotentials] PRIMARY KEY CLUSTERED ([Id] ASC)
);

