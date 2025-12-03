CREATE TABLE [dbo].[FormTemplateSelectionItems] (
    [FormTemplateSelectionItemId]  INT            IDENTITY (1, 1) NOT NULL,
    [FormTemplateSelectionGroupId] INT            NOT NULL,
    [Text]                         VARCHAR (50)   NOT NULL,
    [Value]                        VARCHAR (50)   NOT NULL,
    [Position]                     DECIMAL (6, 2) NOT NULL,
    CONSTRAINT [PK_FormTemplateSelectionItems] PRIMARY KEY CLUSTERED ([FormTemplateSelectionItemId] ASC),
    CONSTRAINT [FK_FormTemplateSelectionItems_FormTemplateSelectionGroups] FOREIGN KEY ([FormTemplateSelectionGroupId]) REFERENCES [dbo].[FormTemplateSelectionGroups] ([FormTemplateSelectionGroupId])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_FormTemplateSelectionItems]
    ON [dbo].[FormTemplateSelectionItems]([FormTemplateSelectionGroupId] ASC, [Text] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_FormTemplateSelectionItems_1]
    ON [dbo].[FormTemplateSelectionItems]([FormTemplateSelectionGroupId] ASC, [Value] ASC);

