CREATE TABLE [dbo].[DdlPayGroups] (
    [PayGroupId]  INT          IDENTITY (1, 1) NOT NULL,
    [Code]        VARCHAR (50) NOT NULL,
    [Description] VARCHAR (50) NOT NULL,
    [Active]      BIT          NOT NULL,
    CONSTRAINT [PK_DdlPayGroups] PRIMARY KEY CLUSTERED ([PayGroupId] ASC),
    CONSTRAINT [IX_DdlPayGroups] UNIQUE NONCLUSTERED ([Code] ASC)
);

