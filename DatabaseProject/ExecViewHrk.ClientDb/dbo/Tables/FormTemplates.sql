CREATE TABLE [dbo].[FormTemplates] (
    [FormTemplateId]   INT           IDENTITY (1, 1) NOT NULL,
    [FormTemplateName] NVARCHAR (50) NOT NULL,
    [Description]      NVARCHAR (50) NULL,
    CONSTRAINT [PK_DesignTables] PRIMARY KEY CLUSTERED ([FormTemplateId] ASC)
);

