CREATE TABLE [dbo].[DdlGenders] (
    [GenderId]    INT          IDENTITY (1, 1) NOT NULL,
    [Code]        VARCHAR (50) NOT NULL,
    [Description] VARCHAR (50) NOT NULL,
    [Active]      BIT          NOT NULL,
    CONSTRAINT [PK_DdlGenders] PRIMARY KEY CLUSTERED ([GenderId] ASC),
    CONSTRAINT [IX_DdlGenders] UNIQUE NONCLUSTERED ([Code] ASC)
);

