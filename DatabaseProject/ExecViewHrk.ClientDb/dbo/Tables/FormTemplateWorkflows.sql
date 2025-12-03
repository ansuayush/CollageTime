CREATE TABLE [dbo].[FormTemplateWorkflows] (
    [FormTemplateWorkflowId]   INT           IDENTITY (1, 1) NOT NULL,
    [FormTemplateId]           INT           NOT NULL,
    [WorkflowId]               INT           NOT NULL,
    [FormTemplateWorkflowName] VARCHAR (500) NOT NULL,
    CONSTRAINT [PK_FormTemplateWorkflows] PRIMARY KEY CLUSTERED ([FormTemplateWorkflowId] ASC),
    CONSTRAINT [FK_FormTemplateWorkflows_FormTemplates] FOREIGN KEY ([FormTemplateId]) REFERENCES [dbo].[FormTemplates] ([FormTemplateId]),
    CONSTRAINT [FK_FormTemplateWorkflows_Workflows] FOREIGN KEY ([WorkflowId]) REFERENCES [dbo].[Workflows] ([WorkflowId])
);

