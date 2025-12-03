CREATE TABLE [dbo].[FormTemplateSelectionGroups] (
    [FormTemplateSelectionGroupId] INT           IDENTITY (1, 1) NOT NULL,
    [Name]                         VARCHAR (50)  NOT NULL,
    [Description]                  VARCHAR (50)  NOT NULL,
    [ExecViewTable]                VARCHAR (50)  NULL,
    [ExecViewTextColumn]           VARCHAR (50)  NULL,
    [ExecViewValueColumn]          VARCHAR (50)  NULL,
    [ExecViewSql]                  VARCHAR (MAX) NULL,
    CONSTRAINT [PK_SelectionGroups] PRIMARY KEY CLUSTERED ([FormTemplateSelectionGroupId] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_SelectionGroups]
    ON [dbo].[FormTemplateSelectionGroups]([Name] ASC);

