CREATE TABLE [dbo].[DdlSuffixes] (
    [SuffixId]    INT          IDENTITY (1, 1) NOT NULL,
    [Description] VARCHAR (50) NOT NULL,
    [Code]        VARCHAR (10) NOT NULL,
    [Active]      BIT          CONSTRAINT [DF_vSuffixes_active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vSuffixes] PRIMARY KEY CLUSTERED ([SuffixId] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_ddlSuffixes]
    ON [dbo].[DdlSuffixes]([Code] ASC);

