CREATE TABLE [dbo].[FormTemplateFieldTypes] (
    [FormTemplateFieldTypeId] INT          IDENTITY (1, 1) NOT NULL,
    [Name]                    VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_FormTemplateFieldTypes] PRIMARY KEY CLUSTERED ([FormTemplateFieldTypeId] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_FormTemplateFieldTypes]
    ON [dbo].[FormTemplateFieldTypes]([Name] ASC);

