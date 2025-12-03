CREATE TABLE [dbo].[FormTemplateFields] (
    [FormTemplateFieldId]    INT            IDENTITY (1, 1) NOT NULL,
    [FormTemplateId]         INT            NOT NULL,
    [HtmlId]                 VARCHAR (50)   NOT NULL,
    [Type]                   VARCHAR (50)   NOT NULL,
    [Value]                  VARCHAR (50)   NULL,
    [Label]                  VARCHAR (500)  NULL,
    [VisualWidth]            INT            NULL,
    [Position]               DECIMAL (6, 2) NOT NULL,
    [Required]               BIT            NOT NULL,
    [ColumnNumber]           INT            NOT NULL,
    [SelectionGroup]         VARCHAR (50)   NULL,
    [CheckBoxRadioGroupName] VARCHAR (50)   NULL,
    CONSTRAINT [PK_FormTemplateFields] PRIMARY KEY CLUSTERED ([FormTemplateFieldId] ASC),
    CONSTRAINT [FK_FormTemplateFields_FormTemplates] FOREIGN KEY ([FormTemplateId]) REFERENCES [dbo].[FormTemplates] ([FormTemplateId])
);

