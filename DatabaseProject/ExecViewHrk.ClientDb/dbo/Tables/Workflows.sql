CREATE TABLE [dbo].[Workflows] (
    [WorkflowId]          INT           IDENTITY (1, 1) NOT NULL,
    [WorkflowName]        VARCHAR (500) NOT NULL,
    [WorkflowDescription] VARCHAR (500) NULL,
    CONSTRAINT [PK_Workflows] PRIMARY KEY CLUSTERED ([WorkflowId] ASC)
);

