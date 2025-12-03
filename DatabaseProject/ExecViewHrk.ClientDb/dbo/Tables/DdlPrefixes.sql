CREATE TABLE [dbo].[DdlPrefixes] (
    [PrefixId]    INT          IDENTITY (1, 1) NOT NULL,
    [Description] VARCHAR (50) NOT NULL,
    [Code]        VARCHAR (10) NOT NULL,
    [Active]      BIT          CONSTRAINT [DF_ddlPrefixes_active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vPrefixes] PRIMARY KEY CLUSTERED ([PrefixId] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_ddlPrefixes]
    ON [dbo].[DdlPrefixes]([Code] ASC);

