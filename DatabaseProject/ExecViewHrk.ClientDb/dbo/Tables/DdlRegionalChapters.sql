CREATE TABLE [dbo].[DdlRegionalChapters] (
    [RegionalChapterId] INT     IDENTITY (1, 1) NOT NULL,
    [Description]       VARCHAR (50) NOT NULL,
    [Code]              VARCHAR (10) NOT NULL,
    [Active]            BIT          CONSTRAINT [DF_ddlRegionalChapters_active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vRegionalChapters] PRIMARY KEY CLUSTERED ([RegionalChapterId] ASC)
);

