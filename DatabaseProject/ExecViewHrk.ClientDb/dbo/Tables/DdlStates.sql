CREATE TABLE [dbo].[DdlStates] (
    [StateId] INT      IDENTITY (1, 1) NOT NULL,
    [Title]   VARCHAR (25) NOT NULL,
    [Code]    CHAR (2)     NOT NULL,
    CONSTRAINT [PK_vhStates] PRIMARY KEY CLUSTERED ([StateId] ASC)
);

