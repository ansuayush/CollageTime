CREATE TABLE [dbo].[FormWorkflowFieldPermissions] (
    [FormWorkflowFieldPermissionId] INT IDENTITY (1, 1) NOT NULL,
    [FormTemplateWorkflowId]        INT NOT NULL,
    [FormTemplateFieldId]           INT NOT NULL,
    [WorlflowMemberId]              INT NOT NULL,
    [CanView]                       BIT NOT NULL,
    [CanEdit]                       BIT NOT NULL,
    CONSTRAINT [PK_FormWorkflowFieldPermissions] PRIMARY KEY CLUSTERED ([FormWorkflowFieldPermissionId] ASC),
    CONSTRAINT [FK_FormWorkflowFieldPermissions_FormTemplateFields] FOREIGN KEY ([FormTemplateFieldId]) REFERENCES [dbo].[FormTemplateFields] ([FormTemplateFieldId]),
    CONSTRAINT [FK_FormWorkflowFieldPermissions_FormTemplateWorkflows] FOREIGN KEY ([FormTemplateWorkflowId]) REFERENCES [dbo].[FormTemplateWorkflows] ([FormTemplateWorkflowId]),
    CONSTRAINT [FK_FormWorkflowFieldPermissions_WorkflowMembers] FOREIGN KEY ([WorlflowMemberId]) REFERENCES [dbo].[WorkflowMembers] ([WorkflowMemberId])
);

